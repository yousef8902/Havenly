import { createFileRoute, Link, notFound, useNavigate } from "@tanstack/react-router";
import {
  BedDouble,
  Bath,
  Check,
  Heart,
  MapPin,
  Share2,
  ShieldCheck,
  Users,
} from "lucide-react";
import { useState } from "react";
import { toast } from "sonner";
import { GuestShell } from "@/components/havenly/shells";
import { Rating, Stars } from "@/components/havenly/rating";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Separator } from "@/components/ui/separator";
import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { useAppState } from "@/lib/app-state";
import {
  formatMoney,
  getProperty,
  nightsBetween,
  properties,
  reviews as allReviews,
} from "@/lib/data";

export const Route = createFileRoute("/property/$propertyId")({
  loader: ({ params }) => {
    const property = getProperty(params.propertyId);
    if (!property) throw notFound();
    return { property };
  },
  head: ({ loaderData }) => {
    if (!loaderData) {
      return { meta: [{ title: "Stay unavailable — Havenly" }, { name: "robots", content: "noindex" }] };
    }
    const p = loaderData.property;
    return {
      meta: [
        { title: `${p.title} — Havenly` },
        { name: "description", content: p.description.slice(0, 155) },
        { property: "og:title", content: `${p.title} — Havenly` },
        { property: "og:description", content: p.description.slice(0, 155) },
      ],
    };
  },
  component: PropertyDetail,
});

function PropertyDetail() {
  const { property } = Route.useLoaderData();
  const navigate = useNavigate();
  const { isFavorite, toggleFavorite, addBooking } = useAppState();
  const [checkIn, setCheckIn] = useState("");
  const [checkOut, setCheckOut] = useState("");
  const [guests, setGuests] = useState(2);
  const [submitting, setSubmitting] = useState(false);

  const nights = nightsBetween(checkIn, checkOut);
  const subtotal = nights * property.price;
  const serviceFee = Math.round(subtotal * 0.09);
  const total = subtotal + serviceFee;

  const rangeDates: string[] = [];
  if (nights > 0) {
    for (let i = 0; i < nights; i++) {
      rangeDates.push(new Date(new Date(checkIn).getTime() + i * 86400000).toISOString().slice(0, 10));
    }
  }
  const conflict = rangeDates.some((d) => property.bookedDates.includes(d));
  const invalidRange = Boolean(checkIn && checkOut && nights <= 0);
  const overCapacity = guests > property.guests;
  const canBook = nights > 0 && !conflict && !overCapacity && !invalidRange;

  const propertyReviews = allReviews.filter((r) => r.propertyId === property.id);
  const saved = isFavorite(property.id);

  function requestBooking() {
    if (!canBook) return;
    setSubmitting(true);
    const id = `HV-${Math.floor(4900 + Math.random() * 90)}`;
    setTimeout(() => {
      addBooking({
        id,
        propertyId: property.id,
        guest: "Nadia Rahman",
        guestEmail: "nadia.rahman@mail.com",
        checkIn,
        checkOut,
        guests,
        total,
        status: "pending",
        createdAt: new Date().toISOString().slice(0, 10),
      });
      toast.success("Booking request sent", {
        description: `${property.host.name} usually responds within a few hours.`,
      });
      setSubmitting(false);
      navigate({
        to: "/booking-confirmed",
        search: { id, propertyId: property.id, checkIn, checkOut, guests, total },
      });
    }, 700);
  }

  const similar = properties.filter((p) => p.id !== property.id && p.status === "approved").slice(0, 3);

  return (
    <GuestShell>
      <div className="container-page pt-8">
        <nav aria-label="Breadcrumb" className="text-sm text-muted-foreground">
          <Link to="/search" search={{ guests: 2 }} className="hover:text-foreground">
            Stays
          </Link>
          <span className="px-2">/</span>
          <span className="text-foreground">{property.city}</span>
        </nav>

        <div className="mt-3 flex flex-wrap items-start justify-between gap-4">
          <div>
            <h1 className="max-w-3xl text-3xl md:text-4xl">{property.title}</h1>
            <div className="mt-2 flex flex-wrap items-center gap-x-4 gap-y-1 text-sm text-muted-foreground">
              <Rating value={property.rating} reviews={property.reviews} />
              <span className="inline-flex items-center gap-1">
                <MapPin className="size-4" /> {property.neighbourhood}, {property.city},{" "}
                {property.country}
              </span>
            </div>
          </div>
          <div className="flex gap-2">
            <Button
              variant="outline"
              className="h-11 gap-2"
              onClick={() => toast("Link copied to clipboard")}
            >
              <Share2 className="size-4" /> Share
            </Button>
            <Button
              variant="outline"
              className="h-11 gap-2"
              aria-pressed={saved}
              onClick={() => toggleFavorite(property.id)}
            >
              <Heart className={`size-4 ${saved ? "fill-destructive text-destructive" : ""}`} />
              {saved ? "Saved" : "Save"}
            </Button>
          </div>
        </div>

        <div className="mt-6 grid gap-3 overflow-hidden rounded-3xl md:grid-cols-[2fr_1fr] md:grid-rows-2">
          <img
            src={property.images[0]}
            alt={`${property.title} — main view`}
            width={1200}
            height={900}
            className="h-72 w-full object-cover md:col-start-1 md:row-span-2 md:h-full"
          />
          {property.images.slice(1, 3).map((img, i) => (
            <img
              key={i}
              src={img}
              alt={`${property.title} — view ${i + 2}`}
              loading="lazy"
              width={1200}
              height={900}
              className="hidden h-full w-full object-cover md:block"
            />
          ))}
        </div>

        <div className="mt-10 grid gap-10 lg:grid-cols-[1fr_23rem]">
          <div className="space-y-10">
            <section className="flex flex-wrap items-center justify-between gap-4 border-b border-border pb-8">
              <div>
                <h2 className="text-xl">Hosted by {property.host.name}</h2>
                <p className="mt-1 text-sm text-muted-foreground">
                  Hosting since {property.host.since} · {property.host.responseRate}% response rate
                  {property.host.superhost ? " · Verified superhost" : ""}
                </p>
              </div>
              <Avatar className="size-14">
                <AvatarFallback className="bg-primary text-primary-foreground">
                  {property.host.name
                    .split(" ")
                    .map((n) => n[0])
                    .join("")}
                </AvatarFallback>
              </Avatar>
            </section>

            <section className="grid gap-4 sm:grid-cols-3">
              <Facts Icon={Users} label={`${property.guests} guests`} sub="Maximum occupancy" />
              <Facts
                Icon={BedDouble}
                label={`${property.bedrooms} bedrooms · ${property.beds} beds`}
                sub="Sleeping arrangement"
              />
              <Facts Icon={Bath} label={`${property.baths} bathrooms`} sub="Private to your stay" />
            </section>

            <section>
              <h2 className="text-xl">About this home</h2>
              <p className="mt-3 leading-relaxed text-muted-foreground">{property.description}</p>
            </section>

            <section>
              <h2 className="text-xl">Amenities</h2>
              <ul className="mt-4 grid gap-3 sm:grid-cols-2">
                {property.amenities.map((a) => (
                  <li key={a} className="flex items-center gap-2 text-sm">
                    <Check className="size-4 text-primary" aria-hidden="true" />
                    {a}
                  </li>
                ))}
              </ul>
            </section>

            <section>
              <h2 className="text-xl">House rules</h2>
              <ul className="mt-4 space-y-2 text-sm text-muted-foreground">
                {property.rules.map((r) => (
                  <li key={r}>· {r}</li>
                ))}
              </ul>
            </section>

            <section>
              <h2 className="text-xl">Where you'll be</h2>
              <div
                role="img"
                aria-label={`Map placeholder for ${property.neighbourhood}, ${property.city}`}
                className="mt-4 grid h-64 place-items-center rounded-2xl border border-border bg-secondary text-sm text-muted-foreground"
              >
                <div className="text-center">
                  <MapPin className="mx-auto size-6" />
                  <p className="mt-2 font-medium text-foreground">
                    {property.neighbourhood}, {property.city}
                  </p>
                  <p>Exact address shared after your booking is approved</p>
                </div>
              </div>
            </section>

            <section>
              <div className="flex flex-wrap items-center justify-between gap-3">
                <h2 className="text-xl">
                  {property.rating.toFixed(2)} · {property.reviews} reviews
                </h2>
                <Button asChild variant="ghost">
                  <Link to="/reviews">All reviews</Link>
                </Button>
              </div>
              <div className="mt-5 grid gap-5 md:grid-cols-2">
                {propertyReviews.length === 0 && (
                  <p className="text-sm text-muted-foreground">No written reviews yet.</p>
                )}
                {propertyReviews.map((r) => (
                  <article key={r.id} className="rounded-2xl border border-border bg-card p-5">
                    <div className="flex items-center justify-between">
                      <p className="font-semibold">{r.author}</p>
                      <Stars value={r.rating} />
                    </div>
                    <p className="text-xs text-muted-foreground">{r.date}</p>
                    <p className="mt-3 text-sm leading-relaxed">{r.body}</p>
                    {r.hostResponse && (
                      <p className="mt-3 rounded-xl bg-secondary p-3 text-sm">
                        <span className="font-semibold">Response from {property.host.name}: </span>
                        {r.hostResponse}
                      </p>
                    )}
                  </article>
                ))}
              </div>
            </section>
          </div>

          <aside className="lg:relative">
            <div className="sticky top-24 rounded-2xl border border-border bg-card p-5 shadow-soft">
              <p className="flex items-baseline gap-1">
                <span className="font-display text-2xl font-semibold">
                  {formatMoney(property.price)}
                </span>
                <span className="text-sm text-muted-foreground">per night</span>
              </p>

              <div className="mt-4 grid grid-cols-2 gap-3">
                <div className="space-y-1.5">
                  <Label htmlFor="ci">Check-in</Label>
                  <Input id="ci" type="date" value={checkIn} onChange={(e) => setCheckIn(e.target.value)} className="h-11" />
                </div>
                <div className="space-y-1.5">
                  <Label htmlFor="co">Check-out</Label>
                  <Input
                    id="co"
                    type="date"
                    value={checkOut}
                    min={checkIn || undefined}
                    onChange={(e) => setCheckOut(e.target.value)}
                    aria-invalid={invalidRange}
                    className="h-11"
                  />
                </div>
              </div>

              <div className="mt-3 space-y-1.5">
                <Label htmlFor="gs">Guests</Label>
                <Select value={String(guests)} onValueChange={(v) => setGuests(Number(v))}>
                  <SelectTrigger id="gs" className="h-11">
                    <SelectValue />
                  </SelectTrigger>
                  <SelectContent>
                    {Array.from({ length: property.guests }, (_, i) => i + 1).map((n) => (
                      <SelectItem key={n} value={String(n)}>
                        {n} {n === 1 ? "guest" : "guests"}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>

              <div className="mt-4" aria-live="polite">
                {invalidRange && (
                  <Notice tone="error">Check-out must be at least one night after check-in.</Notice>
                )}
                {!invalidRange && conflict && (
                  <Notice tone="error">
                    Those nights are already booked. Try shifting your dates by a few days.
                  </Notice>
                )}
                {!invalidRange && !conflict && nights > 0 && (
                  <Notice tone="ok">
                    {nights} {nights === 1 ? "night" : "nights"} available for these dates.
                  </Notice>
                )}
                {nights === 0 && !invalidRange && (
                  <Notice tone="muted">Select dates to check availability.</Notice>
                )}
              </div>

              {nights > 0 && !conflict && (
                <div className="mt-4 space-y-2 text-sm">
                  <Row label={`${formatMoney(property.price)} × ${nights} nights`} value={formatMoney(subtotal)} />
                  <Row label="Service fee" value={formatMoney(serviceFee)} />
                  <Separator />
                  <Row label="Total" value={formatMoney(total)} strong />
                </div>
              )}

              <Button
                className="mt-5 h-12 w-full"
                disabled={!canBook || submitting}
                onClick={requestBooking}
              >
                {submitting ? "Sending request…" : "Request to book"}
              </Button>
              <p className="mt-2 flex items-center justify-center gap-1.5 text-xs text-muted-foreground">
                <ShieldCheck className="size-3.5" /> You won't be charged until the host approves.
              </p>

              <div className="mt-4 rounded-xl bg-secondary p-3 text-xs text-muted-foreground">
                <p className="font-semibold text-foreground">Unavailable nights</p>
                <p className="mt-1">
                  {property.bookedDates.length
                    ? `${property.bookedDates[0]} → ${property.bookedDates[property.bookedDates.length - 1]}`
                    : "None — every night is open right now."}
                </p>
              </div>
            </div>
          </aside>
        </div>

        <section className="mt-20">
          <h2 className="text-2xl">Similar stays</h2>
          <div className="mt-6 grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
            {similar.map((p) => (
              <Link
                key={p.id}
                to="/property/$propertyId"
                params={{ propertyId: p.id }}
                className="group overflow-hidden rounded-2xl border border-border bg-card"
              >
                <img
                  src={p.images[0]}
                  alt={p.title}
                  loading="lazy"
                  width={1200}
                  height={900}
                  className="aspect-4/3 w-full object-cover transition-transform duration-500 group-hover:scale-105"
                />
                <div className="p-4">
                  <p className="font-semibold">{p.title}</p>
                  <p className="text-sm text-muted-foreground">
                    {p.city} · {formatMoney(p.price)} / night
                  </p>
                </div>
              </Link>
            ))}
          </div>
        </section>
      </div>
    </GuestShell>
  );
}

function Facts({
  Icon,
  label,
  sub,
}: {
  Icon: React.ComponentType<{ className?: string }>;
  label: string;
  sub: string;
}) {
  return (
    <div className="rounded-2xl border border-border bg-card p-4">
      <Icon className="size-5 text-primary" />
      <p className="mt-2 font-semibold">{label}</p>
      <p className="text-sm text-muted-foreground">{sub}</p>
    </div>
  );
}

function Row({ label, value, strong }: { label: string; value: string; strong?: boolean }) {
  return (
    <div className={`flex items-center justify-between ${strong ? "font-semibold" : ""}`}>
      <span className={strong ? "" : "text-muted-foreground"}>{label}</span>
      <span>{value}</span>
    </div>
  );
}

function Notice({ tone, children }: { tone: "ok" | "error" | "muted"; children: React.ReactNode }) {
  const styles = {
    ok: "border-success/35 bg-success/10 text-success",
    error: "border-destructive/35 bg-destructive/10 text-destructive",
    muted: "border-border bg-secondary text-muted-foreground",
  }[tone];
  return (
    <p className={`rounded-xl border px-3 py-2 text-sm font-medium ${styles}`} role={tone === "error" ? "alert" : undefined}>
      {children}
    </p>
  );
}