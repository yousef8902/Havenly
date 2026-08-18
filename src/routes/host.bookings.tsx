import { createFileRoute } from "@tanstack/react-router";
import { useState } from "react";
import { CalendarX, Check, X } from "lucide-react";
import { toast } from "sonner";
import { PageHeading, WorkspaceShell } from "@/components/havenly/shells";
import { hostNav } from "@/components/havenly/workspace-nav";
import { StatusBadge } from "@/components/havenly/status-badge";
import { EmptyState } from "@/components/havenly/empty-state";
import { Button } from "@/components/ui/button";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { useAppState } from "@/lib/app-state";
import { formatDate, formatMoney, getProperty, nightsBetween } from "@/lib/data";
import type { Booking, BookingStatus } from "@/lib/data";

export const Route = createFileRoute("/host/bookings")({
  head: () => ({
    meta: [
      { title: "Booking requests — Havenly Host" },
      { name: "description", content: "Approve or decline guest requests, and review upcoming and past stays at your Havenly homes." },
      { property: "og:title", content: "Booking requests — Havenly Host" },
      { property: "og:description", content: "Approve or decline guest booking requests on Havenly." },
      { property: "og:type", content: "website" },
      { name: "twitter:card", content: "summary_large_image" },
    ],
  }),
  component: HostBookings,
});

const tabs: { key: string; label: string; match: BookingStatus[] }[] = [
  { key: "pending", label: "Requests", match: ["pending"] },
  { key: "upcoming", label: "Upcoming", match: ["approved"] },
  { key: "past", label: "Past", match: ["completed"] },
  { key: "closed", label: "Cancelled", match: ["cancelled", "rejected"] },
];

function BookingRow({ b, onAction }: { b: Booking; onAction?: (s: BookingStatus) => void }) {
  const property = getProperty(b.propertyId);
  return (
    <li className="rounded-2xl border border-border bg-card p-5">
      <div className="flex flex-wrap items-start justify-between gap-3">
        <div>
          <p className="font-semibold">{b.guest}</p>
          <p className="text-sm text-muted-foreground">{property?.title}</p>
        </div>
        <StatusBadge status={b.status} />
      </div>
      <dl className="mt-4 grid grid-cols-2 gap-4 text-sm sm:grid-cols-4">
        <div>
          <dt className="text-muted-foreground">Dates</dt>
          <dd className="font-medium">{formatDate(b.checkIn)} – {formatDate(b.checkOut)}</dd>
        </div>
        <div>
          <dt className="text-muted-foreground">Nights</dt>
          <dd className="font-medium">{nightsBetween(b.checkIn, b.checkOut)}</dd>
        </div>
        <div>
          <dt className="text-muted-foreground">Guests</dt>
          <dd className="font-medium">{b.guests}</dd>
        </div>
        <div>
          <dt className="text-muted-foreground">Payout</dt>
          <dd className="font-medium">{formatMoney(b.total)}</dd>
        </div>
      </dl>
      {onAction && (
        <div className="mt-5 flex flex-wrap gap-2">
          <Button onClick={() => onAction("approved")}>
            <Check className="size-4" /> Approve
          </Button>
          <Button variant="outline" onClick={() => onAction("rejected")}>
            <X className="size-4" /> Decline
          </Button>
        </div>
      )}
    </li>
  );
}

function HostBookings() {
  const { bookings, setBookingStatus } = useAppState();
  const [tab, setTab] = useState("pending");

  return (
    <WorkspaceShell items={hostNav} workspace="Host workspace" role="Elena Marinos · Superhost">
      <PageHeading title="Booking requests" description="Respond within 24 hours to keep your response rate high." />

      <Tabs value={tab} onValueChange={setTab}>
        <TabsList className="flex-wrap">
          {tabs.map((t) => (
            <TabsTrigger key={t.key} value={t.key}>
              {t.label} ({bookings.filter((b) => t.match.includes(b.status)).length})
            </TabsTrigger>
          ))}
        </TabsList>
        {tabs.map((t) => {
          const rows = bookings.filter((b) => t.match.includes(b.status));
          return (
            <TabsContent key={t.key} value={t.key} className="mt-6">
              {rows.length === 0 ? (
                <EmptyState
                  icon={<CalendarX className="size-5" />}
                  title={`No ${t.label.toLowerCase()}`}
                  description="Nothing to show here right now. New guest requests appear the moment they're sent."
                />
              ) : (
                <ul className="space-y-4">
                  {rows.map((b) => (
                    <BookingRow
                      key={b.id}
                      b={b}
                      onAction={
                        b.status === "pending"
                          ? (s) => {
                              setBookingStatus(b.id, s);
                              toast.success(s === "approved" ? `Approved ${b.id}` : `Declined ${b.id}`);
                            }
                          : undefined
                      }
                    />
                  ))}
                </ul>
              )}
            </TabsContent>
          );
        })}
      </Tabs>
    </WorkspaceShell>
  );
}
