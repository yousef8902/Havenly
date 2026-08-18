import { createFileRoute, Link } from "@tanstack/react-router";
import { useState } from "react";
import { Building2, Check, CircleDollarSign, Users, X } from "lucide-react";
import { toast } from "sonner";
import { PageHeading, StatCard, WorkspaceShell } from "@/components/havenly/shells";
import { adminNav } from "@/components/havenly/workspace-nav";
import { StatusBadge } from "@/components/havenly/status-badge";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { useAppState } from "@/lib/app-state";
import { formatDate, formatMoney, platformSeries, properties, users, type ListingStatus } from "@/lib/data";

export const Route = createFileRoute("/admin/")({
  head: () => ({
    meta: [
      { title: "Admin console — Havenly" },
      { name: "description", content: "Moderate listings, manage member accounts and monitor platform growth across Havenly." },
      { property: "og:title", content: "Admin console — Havenly" },
      { property: "og:description", content: "Moderate listings and manage members on Havenly." },
      { property: "og:type", content: "website" },
      { name: "twitter:card", content: "summary_large_image" },
    ],
  }),
  component: AdminOverview,
});

function AdminOverview() {
  const { bookings } = useAppState();
  const [query, setQuery] = useState("");
  const [listingStatus, setListingStatus] = useState<Record<string, ListingStatus>>({});
  const [userStatus, setUserStatus] = useState<Record<string, string>>({});

  const gmv = bookings
    .filter((b) => b.status !== "cancelled" && b.status !== "rejected")
    .reduce((s, b) => s + b.total, 0);
  const max = Math.max(...platformSeries.map((p) => p.bookings));

  const filteredUsers = users.filter((u) =>
    (u.name + u.email + u.role).toLowerCase().includes(query.toLowerCase()),
  );

  return (
    <WorkspaceShell items={adminNav} workspace="Admin console" role="Omar Khalil · Administrator">
      <PageHeading title="Overview" description="Platform health, moderation queue and member management." />

      <div className="grid gap-4 sm:grid-cols-2 xl:grid-cols-4">
        <StatCard label="Members" value={String(users.length)} hint="Guests, hosts and staff" Icon={Users} />
        <StatCard label="Listings" value={String(properties.length)} hint="Live and in review" Icon={Building2} />
        <StatCard label="Gross bookings" value={formatMoney(gmv)} hint="Excludes cancellations" Icon={CircleDollarSign} />
        <StatCard
          label="Pending review"
          value={String(properties.filter((p) => (listingStatus[p.id] ?? p.status) === "pending").length)}
          hint="Awaiting moderation"
          Icon={Check}
        />
      </div>

      <section className="mt-8 rounded-2xl border border-border bg-card p-6">
        <h2 className="text-xl">Bookings vs. signups</h2>
        <div className="mt-6 flex h-44 items-end gap-4">
          {platformSeries.map((p) => (
            <div key={p.month} className="flex flex-1 flex-col items-center gap-2">
              <div className="flex h-full w-full items-end justify-center gap-1">
                <div className="w-1/3 rounded-t bg-primary/85" style={{ height: `${(p.bookings / max) * 100}%` }} />
                <div className="w-1/3 rounded-t bg-accent/70" style={{ height: `${(p.signups / max) * 100}%` }} />
              </div>
              <span className="text-xs text-muted-foreground">{p.month}</span>
            </div>
          ))}
        </div>
        <div className="mt-4 flex gap-4 text-xs text-muted-foreground">
          <span className="flex items-center gap-2"><span className="size-3 rounded bg-primary/85" /> Bookings</span>
          <span className="flex items-center gap-2"><span className="size-3 rounded bg-accent/70" /> Signups</span>
        </div>
      </section>

      <Tabs defaultValue="listings" className="mt-8">
        <TabsList>
          <TabsTrigger value="listings">Listings</TabsTrigger>
          <TabsTrigger value="users">Members</TabsTrigger>
        </TabsList>

        <TabsContent value="listings" className="mt-6">
          <div className="overflow-x-auto rounded-2xl border border-border bg-card">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Property</TableHead>
                  <TableHead>Host</TableHead>
                  <TableHead>Submitted</TableHead>
                  <TableHead>Status</TableHead>
                  <TableHead className="text-right">Moderation</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {properties.map((p) => {
                  const status = listingStatus[p.id] ?? p.status;
                  return (
                    <TableRow key={p.id}>
                      <TableCell className="font-medium">
                        <Link to="/property/$propertyId" params={{ propertyId: p.id }} className="hover:underline">
                          {p.title}
                        </Link>
                      </TableCell>
                      <TableCell className="text-muted-foreground">{p.host.name}</TableCell>
                      <TableCell className="text-muted-foreground">{formatDate(p.submitted)}</TableCell>
                      <TableCell><StatusBadge status={status} /></TableCell>
                      <TableCell className="text-right">
                        <div className="flex justify-end gap-2">
                          <Button
                            size="sm"
                            variant="outline"
                            disabled={status === "approved"}
                            onClick={() => {
                              setListingStatus((s) => ({ ...s, [p.id]: "approved" }));
                              toast.success(`Approved “${p.title}”`);
                            }}
                          >
                            <Check className="size-4" /> Approve
                          </Button>
                          <Button
                            size="sm"
                            variant="ghost"
                            disabled={status === "rejected"}
                            onClick={() => {
                              setListingStatus((s) => ({ ...s, [p.id]: "rejected" }));
                              toast(`Rejected “${p.title}”`);
                            }}
                          >
                            <X className="size-4" /> Reject
                          </Button>
                        </div>
                      </TableCell>
                    </TableRow>
                  );
                })}
              </TableBody>
            </Table>
          </div>
        </TabsContent>

        <TabsContent value="users" className="mt-6">
          <Input
            value={query}
            onChange={(e) => setQuery(e.target.value)}
            placeholder="Search members by name, email or role"
            className="mb-4 h-11 max-w-sm"
            aria-label="Search members"
          />
          <div className="overflow-x-auto rounded-2xl border border-border bg-card">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Name</TableHead>
                  <TableHead>Email</TableHead>
                  <TableHead>Role</TableHead>
                  <TableHead>Joined</TableHead>
                  <TableHead>Bookings</TableHead>
                  <TableHead>Status</TableHead>
                  <TableHead className="text-right">Action</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {filteredUsers.map((u) => {
                  const status = userStatus[u.id] ?? u.status;
                  const suspended = status === "Suspended";
                  return (
                    <TableRow key={u.id}>
                      <TableCell className="font-medium">{u.name}</TableCell>
                      <TableCell className="text-muted-foreground">{u.email}</TableCell>
                      <TableCell>{u.role}</TableCell>
                      <TableCell className="text-muted-foreground">{formatDate(u.joined)}</TableCell>
                      <TableCell>{u.bookings}</TableCell>
                      <TableCell><StatusBadge status={status} /></TableCell>
                      <TableCell className="text-right">
                        <Button
                          size="sm"
                          variant={suspended ? "outline" : "ghost"}
                          onClick={() => {
                            setUserStatus((s) => ({ ...s, [u.id]: suspended ? "Active" : "Suspended" }));
                            toast.success(`${u.name} ${suspended ? "reinstated" : "suspended"}`);
                          }}
                        >
                          {suspended ? "Reinstate" : "Suspend"}
                        </Button>
                      </TableCell>
                    </TableRow>
                  );
                })}
              </TableBody>
            </Table>
          </div>
        </TabsContent>
      </Tabs>
    </WorkspaceShell>
  );
}
