import { createFileRoute, Link } from "@tanstack/react-router";
import { CalendarDays, CheckCircle2, Users } from "lucide-react";
import { z } from "zod";
import { GuestShell } from "@/components/havenly/shells";
import { StatusBadge } from "@/components/havenly/status-badge";
import { Button } from "@/components/ui/button";
import { Separator } from "@/components/ui/separator";
import { formatDate, formatMoney, getProperty, nightsBetween } from "@/lib/data";

export const Route = createFileRoute("/booking-confirmed")({
  validateSearch: z.object({
    id: z.string(),
    propertyId: z.string(),
    checkIn: z.string(),
    checkOut: z.string(),
    guests: z.number(),
    total: z.number(),
  }),
  head: () => ({
    meta: [
      { title: "Booking request sent — Havenly" },
      { name: "description", content: "Your Havenly booking request has been sent to the host for approval." },
      { property: "og:title", content: "Booking request sent — Havenly" },
      { property: "og:description", content: "Review your request summary and track its status." },
      { name: "robots", content: "noindex" },
    ],
  }),
  component: BookingConfirmed,
});

function BookingConfirmed() {
  const s = Route.useSearch();
  const property = getProperty(s.propertyId);
  const nights = nightsBetween(s.checkIn, s.checkOut);

  return (
    <GuestShell>
      <div className="container-page max-w-3xl py-16">
        <div className="flex flex-col items-center text-center">
          <span className="grid size-14 place-items-center rounded-full bg-success/12 text-success">
            <CheckCircle2 className="size-7" />
          </span>
          <h1 className="mt-5 text-3xl md:text-4xl">Your request is with the host</h1>
          <p className="mt-2 max-w-md text-muted-foreground">
            Booking <span className="font-semibold text-foreground">{s.id}</span> was sent
            successfully. You'll get an email as soon as {property?.host.name ?? "the host"} responds —
            usually within a few hours.
          </p>
        </div>

        <div className="mt-10 overflow-hidden rounded-2xl border border-border bg-card">
          {property && (
            <img
              src={property.images[0]}
              alt={property.title}
              loading="lazy"
              width={1200}
              height={900}
              className="h-56 w-full object-cover"
            />
          )}
          <div className="space-y-4 p-6">
            <div className="flex flex-wrap items-start justify-between gap-3">
              <div>
                <h2 className="text-xl">{property?.title ?? "Your stay"}</h2>
                <p className="text-sm text-muted-foreground">
                  {property?.neighbourhood}, {property?.city}
                </p>
              </div>
              <StatusBadge status="pending" />
            </div>

            <Separator />

            <dl className="grid gap-4 sm:grid-cols-3">
              <div>
                <dt className="text-xs uppercase tracking-wide text-muted-foreground">Dates</dt>
                <dd className="mt-1 flex items-center gap-1.5 text-sm font-medium">
                  <CalendarDays className="size-4 text-muted-foreground" />
                  {formatDate(s.checkIn)} → {formatDate(s.checkOut)}
                </dd>
                <dd className="text-xs text-muted-foreground">{nights} nights</dd>
              </div>
              <div>
                <dt className="text-xs uppercase tracking-wide text-muted-foreground">Guests</dt>
                <dd className="mt-1 flex items-center gap-1.5 text-sm font-medium">
                  <Users className="size-4 text-muted-foreground" />
                  {s.guests} guests
                </dd>
              </div>
              <div>
                <dt className="text-xs uppercase tracking-wide text-muted-foreground">Total</dt>
                <dd className="mt-1 font-display text-xl font-semibold">{formatMoney(s.total)}</dd>
                <dd className="text-xs text-muted-foreground">Charged after approval</dd>
              </div>
            </dl>
          </div>
        </div>

        <div className="mt-8 flex flex-wrap justify-center gap-3">
          <Button asChild size="lg">
            <Link to="/bookings">View my bookings</Link>
          </Button>
          <Button asChild size="lg" variant="outline">
            <Link to="/search" search={{ guests: 2 }}>
              Keep exploring
            </Link>
          </Button>
        </div>
      </div>
    </GuestShell>
  );
}