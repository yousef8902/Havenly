import { createFileRoute, Link } from "@tanstack/react-router";
import { CalendarX2, MapPin, Users } from "lucide-react";
import { useState } from "react";
import { toast } from "sonner";
import { GuestShell } from "@/components/havenly/shells";
import { StatusBadge } from "@/components/havenly/status-badge";
import { EmptyState } from "@/components/havenly/empty-state";
import { Button } from "@/components/ui/button";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from "@/components/ui/alert-dialog";
import { useAppState } from "@/lib/app-state";
import { formatDate, formatMoney, getProperty, nightsBetween, type Booking } from "@/lib/data";

export const Route = createFileRoute("/bookings")({
  head: () => ({
    meta: [
      { title: "My bookings — Havenly" },
      { name: "description", content: "Track upcoming, pending, completed and cancelled Havenly stays in one place." },
      { property: "og:title", content: "My bookings — Havenly" },
      { property: "og:description", content: "Track every Havenly stay and its current status." },
    ],
  }),
  component: BookingsPage,
});

const tabs = [
  { key: "approved", label: "Upcoming" },
  { key: "pending", label: "Pending" },
  { key: "completed", label: "Completed" },
  { key: "cancelled", label: "Cancelled" },
] as const;

function BookingsPage() {
  const { bookings, setBookingStatus } = useAppState();
  const [cancelId, setCancelId] = useState<string | null>(null);

  return (
    <GuestShell>
      <div className="container-page py-12">
        <h1 className="text-3xl md:text-4xl">My bookings</h1>
        <p className="mt-1 text-muted-foreground">
          Every request you've made, grouped by where it is in the process.
        </p>

        <Tabs defaultValue="approved" className="mt-8">
          <TabsList className="h-11">
            {tabs.map((t) => (
              <TabsTrigger key={t.key} value={t.key} className="px-4">
                {t.label}
                <span className="ml-1.5 text-xs text-muted-foreground">
                  {bookings.filter((b) => b.status === t.key).length}
                </span>
              </TabsTrigger>
            ))}
          </TabsList>

          {tabs.map((t) => {
            const list = bookings.filter((b) => b.status === t.key);
            return (
              <TabsContent key={t.key} value={t.key} className="mt-6">
                {list.length === 0 ? (
                  <EmptyState
                    icon={<CalendarX2 className="size-5" />}
                    title={`No ${t.label.toLowerCase()} bookings`}
                    description="When a booking reaches this stage, you'll find it here with all its details."
                    action={
                      <Button asChild>
                        <Link to="/search" search={{ guests: 2 }}>
                          Find a stay
                        </Link>
                      </Button>
                    }
                  />
                ) : (
                  <div className="space-y-4">
                    {list.map((b) => (
                      <BookingRow key={b.id} booking={b} onCancel={() => setCancelId(b.id)} />
                    ))}
                  </div>
                )}
              </TabsContent>
            );
          })}
        </Tabs>
      </div>

      <AlertDialog open={cancelId !== null} onOpenChange={(o) => !o && setCancelId(null)}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Cancel this booking?</AlertDialogTitle>
            <AlertDialogDescription>
              The host will be notified and the dates will be released to other guests. This
              cannot be undone.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>Keep booking</AlertDialogCancel>
            <AlertDialogAction
              className="bg-destructive text-destructive-foreground hover:bg-destructive/90"
              onClick={() => {
                if (cancelId) {
                  setBookingStatus(cancelId, "cancelled");
                  toast.success(`Booking ${cancelId} cancelled`);
                }
                setCancelId(null);
              }}
            >
              Cancel booking
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </GuestShell>
  );
}

function BookingRow({ booking, onCancel }: { booking: Booking; onCancel: () => void }) {
  const property = getProperty(booking.propertyId);
  const nights = nightsBetween(booking.checkIn, booking.checkOut);

  return (
    <article className="grid gap-5 rounded-2xl border border-border bg-card p-4 sm:grid-cols-[12rem_1fr]">
      {property && (
        <img
          src={property.images[0]}
          alt={property.title}
          loading="lazy"
          width={1200}
          height={900}
          className="aspect-4/3 w-full rounded-xl object-cover"
        />
      )}
      <div className="flex flex-col gap-3">
        <div className="flex flex-wrap items-start justify-between gap-3">
          <div>
            <h2 className="text-lg font-semibold">{property?.title}</h2>
            <p className="flex items-center gap-1 text-sm text-muted-foreground">
              <MapPin className="size-4" /> {property?.city}, {property?.country}
            </p>
          </div>
          <StatusBadge status={booking.status} />
        </div>

        <dl className="flex flex-wrap gap-x-8 gap-y-2 text-sm">
          <div>
            <dt className="text-xs uppercase tracking-wide text-muted-foreground">Dates</dt>
            <dd className="font-medium">
              {formatDate(booking.checkIn)} → {formatDate(booking.checkOut)} · {nights} nights
            </dd>
          </div>
          <div>
            <dt className="text-xs uppercase tracking-wide text-muted-foreground">Guests</dt>
            <dd className="flex items-center gap-1 font-medium">
              <Users className="size-4 text-muted-foreground" /> {booking.guests}
            </dd>
          </div>
          <div>
            <dt className="text-xs uppercase tracking-wide text-muted-foreground">Total</dt>
            <dd className="font-semibold">{formatMoney(booking.total)}</dd>
          </div>
          <div>
            <dt className="text-xs uppercase tracking-wide text-muted-foreground">Reference</dt>
            <dd className="font-medium">{booking.id}</dd>
          </div>
        </dl>

        <div className="mt-auto flex flex-wrap gap-2">
          <Button asChild variant="outline" size="sm" className="min-h-11">
            <Link to="/property/$propertyId" params={{ propertyId: booking.propertyId }}>
              View listing
            </Link>
          </Button>
          {booking.status === "completed" && (
            <Button asChild size="sm" className="min-h-11">
              <Link to="/reviews">Write a review</Link>
            </Button>
          )}
          {(booking.status === "approved" || booking.status === "pending") && (
            <Button variant="ghost" size="sm" className="min-h-11 text-destructive" onClick={onCancel}>
              Cancel booking
            </Button>
          )}
        </div>
      </div>
    </article>
  );
}