import { createFileRoute, Link } from "@tanstack/react-router";
import { BarChart3, CalendarCheck, DollarSign, PlusCircle, Star } from "lucide-react";
import { PageHeading, StatCard, WorkspaceShell } from "@/components/havenly/shells";
import { hostNav } from "@/components/havenly/workspace-nav";
import { StatusBadge } from "@/components/havenly/status-badge";
import { Button } from "@/components/ui/button";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { useAppState } from "@/lib/app-state";
import { formatDate, formatMoney, getProperty, properties, revenueSeries } from "@/lib/data";

export const Route = createFileRoute("/host/")({
  head: () => ({
    meta: [
      { title: "Host dashboard — Havenly" },
      { name: "description", content: "Track earnings, occupancy and guest requests across every home you host on Havenly." },
      { property: "og:title", content: "Host dashboard — Havenly" },
      { property: "og:description", content: "Earnings, occupancy and guest requests for Havenly hosts." },
      { property: "og:type", content: "website" },
      { name: "twitter:card", content: "summary_large_image" },
    ],
  }),
  component: HostDashboard,
});

function HostDashboard() {
  const { bookings } = useAppState();
  const mine = properties.slice(0, 4);
  const mineIds = mine.map((p) => p.id);
  const myBookings = bookings.filter((b) => mineIds.includes(b.propertyId));
  const earnings = myBookings
    .filter((b) => b.status === "approved" || b.status === "completed")
    .reduce((s, b) => s + b.total, 0);
  const pending = myBookings.filter((b) => b.status === "pending").length;
  const avgRating =
    mine.reduce((s, p) => s + p.rating, 0) / (mine.length || 1);
  const max = Math.max(...revenueSeries.map((r) => r.revenue));

  return (
    <WorkspaceShell items={hostNav} workspace="Host workspace" role="Elena Marinos · Superhost">
      <PageHeading
        title="Dashboard"
        description="A snapshot of your portfolio for the last six months."
        action={
          <Button asChild>
            <Link to="/host/properties/new">
              <PlusCircle className="size-4" /> Add property
            </Link>
          </Button>
        }
      />

      <div className="grid gap-4 sm:grid-cols-2 xl:grid-cols-4">
        <StatCard label="Net earnings" value={formatMoney(earnings)} hint="Approved + completed" Icon={DollarSign} />
        <StatCard label="Pending requests" value={String(pending)} hint="Awaiting your reply" Icon={CalendarCheck} />
        <StatCard label="Active listings" value={String(mine.length)} hint="Published on Havenly" Icon={BarChart3} />
        <StatCard label="Average rating" value={avgRating.toFixed(2)} hint="Across all homes" Icon={Star} />
      </div>

      <section className="mt-8 rounded-2xl border border-border bg-card p-6">
        <h2 className="text-xl">Revenue</h2>
        <p className="text-sm text-muted-foreground">Monthly payouts after service fees.</p>
        <div className="mt-6 flex h-48 items-end gap-3">
          {revenueSeries.map((r) => (
            <div key={r.month} className="flex flex-1 flex-col items-center gap-2">
              <span className="text-xs font-medium text-muted-foreground">{formatMoney(r.revenue)}</span>
              <div
                className="w-full rounded-t-lg bg-primary/85"
                style={{ height: `${(r.revenue / max) * 100}%` }}
                role="img"
                aria-label={`${r.month}: ${formatMoney(r.revenue)}`}
              />
              <span className="text-xs text-muted-foreground">{r.month}</span>
            </div>
          ))}
        </div>
      </section>

      <section className="mt-8">
        <h2 className="text-xl">Your listings</h2>
        <div className="mt-4 overflow-x-auto rounded-2xl border border-border bg-card">
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Property</TableHead>
                <TableHead>Location</TableHead>
                <TableHead>Nightly</TableHead>
                <TableHead>Rating</TableHead>
                <TableHead>Status</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {mine.map((p) => (
                <TableRow key={p.id}>
                  <TableCell className="font-medium">
                    <Link to="/property/$propertyId" params={{ propertyId: p.id }} className="hover:underline">
                      {p.title}
                    </Link>
                  </TableCell>
                  <TableCell className="text-muted-foreground">{p.city}, {p.country}</TableCell>
                  <TableCell>{formatMoney(p.price)}</TableCell>
                  <TableCell>{p.rating.toFixed(2)}</TableCell>
                  <TableCell><StatusBadge status={p.status} /></TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </div>
      </section>

      <section className="mt-8">
        <h2 className="text-xl">Latest bookings</h2>
        <div className="mt-4 overflow-x-auto rounded-2xl border border-border bg-card">
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Ref</TableHead>
                <TableHead>Guest</TableHead>
                <TableHead>Property</TableHead>
                <TableHead>Dates</TableHead>
                <TableHead>Total</TableHead>
                <TableHead>Status</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {myBookings.slice(0, 6).map((b) => (
                <TableRow key={b.id}>
                  <TableCell className="font-mono text-xs">{b.id}</TableCell>
                  <TableCell className="font-medium">{b.guest}</TableCell>
                  <TableCell className="text-muted-foreground">{getProperty(b.propertyId)?.title}</TableCell>
                  <TableCell className="whitespace-nowrap text-muted-foreground">
                    {formatDate(b.checkIn)} – {formatDate(b.checkOut)}
                  </TableCell>
                  <TableCell>{formatMoney(b.total)}</TableCell>
                  <TableCell><StatusBadge status={b.status} /></TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </div>
      </section>
    </WorkspaceShell>
  );
}
