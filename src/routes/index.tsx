import { createFileRoute, Link } from "@tanstack/react-router";
import type { ComponentType, ReactNode } from "react";
import {
  ArrowRight,
  Building2,
  CalendarCheck,
  CreditCard,
  Crown,
  Gem,
  Headphones,
  Leaf,
  Mountain,
  PartyPopper,
  Search,
  ShieldCheck,
  Sparkles,
  Star,
  TreePine,
  Waves,
} from "lucide-react";
import { GuestShell } from "@/components/havenly/shells";
import { SearchBar } from "@/components/havenly/search-bar";
import { PropertyCard } from "@/components/havenly/property-card";
import { Button } from "@/components/ui/button";
import { destinations, images, properties } from "@/lib/data";

export const Route = createFileRoute("/")({
  head: () => ({
    meta: [
      { title: "Havenly — Find a place that feels like home" },
      {
        name: "description",
        content:
          "Havenly is a short-term rental marketplace for considered homes. Search cliffside villas, forest cabins and city lofts, then book with confidence.",
      },
      { property: "og:title", content: "Havenly — Find a place that feels like home" },
      {
        property: "og:description",
        content: "Search, compare and book stays in considered homes across the world.",
      },
      { property: "og:type", content: "website" },
      { name: "twitter:card", content: "summary_large_image" },
    ],
  }),
  component: Home,
});

const pills: { label: string; category: string; Icon: ComponentType<{ className?: string }> }[] = [
  { label: "City lofts", category: "City lofts", Icon: Building2 },
  { label: "Countryside", category: "Countryside", Icon: TreePine },
  { label: "Beachfront", category: "Beachfront", Icon: Waves },
  { label: "Cabins", category: "Cabins", Icon: Mountain },
  { label: "Islands", category: "Islands", Icon: PartyPopper },
  { label: "Design homes", category: "Design homes", Icon: Gem },
  { label: "Eco stays", category: "Countryside", Icon: Leaf },
  { label: "Luxury", category: "Islands", Icon: Crown },
];

const steps = [
  { n: "01", title: "Search your destination", body: "Filter by dates, guests, price and the amenities you actually use.", Icon: Search },
  { n: "02", title: "Choose your dates", body: "Availability is checked live, so you never request a booked night.", Icon: CalendarCheck },
  { n: "03", title: "Book securely", body: "You're only charged once the host approves your request.", Icon: CreditCard },
  { n: "04", title: "Enjoy your stay", body: "Message your host any time and get 24/7 support during the trip.", Icon: Sparkles },
];

const confidence = [
  { t: "Verified Properties", b: "Every host completes ID and property verification before publishing.", Icon: ShieldCheck },
  { t: "Secure Payments", b: "Payments are held until check-in and refunded per the listing's policy.", Icon: CreditCard },
  { t: "24/7 Support", b: "Real people, day or night, for anything that comes up before or during a stay.", Icon: Headphones },
];

const stories = [
  { name: "Sophie Laurent", place: "Paris, France", body: "Booked a Tuscan farmhouse in ten minutes. The host called ahead about our late arrival — the whole thing felt personal." },
  { name: "Daniel Osei", place: "Singapore", body: "The listings are honest. What we saw in the photos was exactly the space we walked into, down to the coffee grinder." },
  { name: "Marta Ruiz", place: "Valencia, Spain", body: "Cancellation terms were clear up front and support answered in under two minutes when our plans changed." },
];

function Home() {
  const approved = properties.filter((p) => p.status === "approved");
  const featured = approved.slice(0, 4);
  const recommended = approved.slice(4, 8);

  return (
    <GuestShell transparentHeader>
      <section className="relative flex min-h-[700px] flex-col">
        <div className="absolute inset-0 -z-10">
          <img
            src={images.hero}
            alt="Cliffside villa terrace with an infinity pool overlooking the sea at sunset"
            width={1920}
            height={1280}
            className="size-full object-cover"
          />
          <div className="absolute inset-0 bg-linear-to-b from-foreground/50 via-foreground/25 to-foreground/60" />
        </div>

        <div className="flex flex-1 flex-col items-center justify-center px-5 pb-16 pt-32 text-center lg:px-8">
          <div className="mb-6 inline-flex items-center gap-2 rounded-full border border-background/30 bg-background/20 px-4 py-1.5 backdrop-blur-sm">
            <span className="size-2 rounded-full bg-accent" />
            <span className="text-sm font-medium text-primary-foreground">
              Over 50,000 unique properties worldwide
            </span>
          </div>
          <h1 className="max-w-3xl text-4xl leading-tight text-primary-foreground md:text-6xl">
            Find a place that feels <span className="font-light italic">like home.</span>
          </h1>
          <p className="mt-5 max-w-xl text-lg text-primary-foreground/80 md:text-xl">
            Discover handpicked stays for every style, budget, and destination.
          </p>
          <div className="mt-10 w-full max-w-[880px]">
            <SearchBar />
          </div>
        </div>
      </section>

      <section className="container-page pb-8 pt-12">
        <div className="-mx-5 flex gap-3 overflow-x-auto px-5 pb-2 lg:mx-0 lg:px-0">
          {pills.map(({ label, category, Icon }) => (
            <Link
              key={label}
              to="/search"
              search={{ category, guests: 2 }}
              className="flex shrink-0 flex-col items-center gap-2 rounded-xl border border-border bg-surface px-5 py-3 text-muted-foreground transition-colors hover:border-primary hover:bg-primary-light hover:text-primary"
            >
              <Icon className="size-[22px]" />
              <span className="whitespace-nowrap text-xs font-medium">{label}</span>
            </Link>
          ))}
        </div>
      </section>

      <Section
        eyebrow="Popular destinations"
        title="Trending places to explore"
        action={<ViewAll label="View all" />}
      >
        <div className="grid grid-cols-2 gap-4 md:grid-cols-4">
          {destinations.map((d) => (
            <Link
              key={d.city}
              to="/search"
              search={{ city: d.city, guests: 2 }}
              className="group relative overflow-hidden rounded-xl"
            >
              <img
                src={d.image}
                alt={`${d.city}, ${d.country}`}
                loading="lazy"
                width={800}
                height={1000}
                className="h-56 w-full object-cover transition-transform duration-500 group-hover:scale-105"
              />
              <div className="absolute inset-0 bg-linear-to-t from-foreground/60 to-transparent" />
              <div className="absolute inset-x-0 bottom-0 p-4">
                <p className="font-display text-lg font-semibold leading-tight text-primary-foreground">
                  {d.city}
                </p>
                <p className="text-sm text-primary-foreground/80">{d.country}</p>
              </div>
            </Link>
          ))}
        </div>
      </Section>

      <section className="mt-20 bg-muted/40 py-16">
        <div className="container-page">
          <SectionHead
            eyebrow="Featured stays"
            title="Handpicked by our team"
            action={<ViewAll label="View all" />}
          />
          <div className="grid gap-5 sm:grid-cols-2 lg:grid-cols-4">
            {featured.map((p, i) => (
              <PropertyCard
                key={p.id}
                property={p}
                {...(i === 0 ? { badge: "Guest Favourite" } : i === 3 ? { badge: "New" } : {})}
              />
            ))}
          </div>
        </div>
      </section>

      <section className="container-page py-20">
        <div className="mb-12 text-center">
          <p className="mb-3 text-sm font-semibold uppercase tracking-wide text-primary">
            Simple process
          </p>
          <h2 className="text-3xl">How Havenly works</h2>
          <p className="mx-auto mt-4 max-w-lg text-lg text-muted-foreground">
            Book your dream stay in just a few steps.
          </p>
        </div>
        <div className="grid gap-5 sm:grid-cols-2 lg:grid-cols-4">
          {steps.map(({ n, title, body, Icon }) => (
            <div key={n} className="rounded-2xl border border-border bg-surface p-8">
              <p className="mb-6 text-xs font-bold tracking-widest text-muted-foreground">{n}</p>
              <span className="mb-5 grid size-12 place-items-center rounded-xl bg-primary-light text-primary">
                <Icon className="size-5" />
              </span>
              <h3 className="text-lg">{title}</h3>
              <p className="mt-2 text-sm text-muted-foreground">{body}</p>
            </div>
          ))}
        </div>
      </section>

      <section className="container-page pb-4">
        <SectionHead
          eyebrow="Recommended for you"
          title="Stays you might love"
          action={<ViewAll label="Explore more" />}
        />
        <div className="grid gap-5 sm:grid-cols-2 lg:grid-cols-4">
          {recommended.map((p, i) => (
            <PropertyCard key={p.id} property={p} {...(i === 1 ? { badge: "Top Rated" } : {})} />
          ))}
        </div>
      </section>

      <section className="container-page py-20">
        <div className="mb-12 text-center">
          <p className="mb-3 text-sm font-semibold uppercase tracking-wide text-primary">
            Why Havenly
          </p>
          <h2 className="text-3xl">Travel with confidence</h2>
        </div>
        <div className="grid gap-6 md:grid-cols-3">
          {confidence.map(({ t, b, Icon }) => (
            <div key={t} className="flex gap-5 rounded-2xl border border-border bg-surface p-8">
              <span className="grid size-12 shrink-0 place-items-center rounded-xl bg-primary-light text-primary">
                <Icon className="size-5" />
              </span>
              <div>
                <h3 className="text-lg">{t}</h3>
                <p className="mt-2 text-sm text-muted-foreground">{b}</p>
              </div>
            </div>
          ))}
        </div>
      </section>

      <section className="container-page pb-4">
        <div className="mb-8 flex flex-wrap items-end justify-between gap-4">
          <div>
            <p className="mb-2 text-sm font-semibold uppercase tracking-wide text-primary">
              Guest stories
            </p>
            <h2 className="text-3xl">Loved by travellers worldwide</h2>
          </div>
          <div className="flex items-center gap-2 rounded-full bg-foreground px-4 py-2 text-primary-foreground">
            <Star className="size-4 fill-accent text-accent" />
            <span className="font-semibold">4.96</span>
            <span className="text-primary-foreground/60 text-sm">from 12,400+ reviews</span>
          </div>
        </div>
        <div className="grid gap-5 md:grid-cols-3">
          {stories.map((s) => (
            <figure key={s.name} className="rounded-xl border border-border bg-surface p-6">
              <div className="flex items-center gap-3">
                <span className="grid size-10 place-items-center rounded-full bg-primary-light font-semibold text-primary">
                  {s.name.charAt(0)}
                </span>
                <div>
                  <figcaption className="font-semibold">{s.name}</figcaption>
                  <p className="text-sm text-muted-foreground">{s.place}</p>
                </div>
                <span className="ml-auto flex items-center gap-1 text-sm font-semibold">
                  <Star className="size-4 fill-accent text-accent" /> 5.0
                </span>
              </div>
              <blockquote className="mt-4 text-sm leading-relaxed text-muted-foreground">
                “{s.body}”
              </blockquote>
            </figure>
          ))}
        </div>
      </section>

      <section className="container-page py-20">
        <div className="relative overflow-hidden rounded-3xl">
          <img
            src={images.p2}
            alt="Restored stone farmhouse in the Tuscan countryside"
            loading="lazy"
            width={1600}
            height={900}
            className="absolute inset-0 size-full object-cover"
          />
          <div className="absolute inset-0 bg-linear-to-r from-foreground/85 to-foreground/40" />
          <div className="relative px-8 py-16 lg:px-14 lg:py-20">
            <span className="inline-flex items-center rounded-full border border-background/30 bg-background/20 px-3 py-1 text-sm font-medium text-primary-foreground backdrop-blur-sm">
              For hosts
            </span>
            <h2 className="mt-4 max-w-xl text-3xl leading-tight text-primary-foreground md:text-4xl">
              Turn your space into income
            </h2>
            <p className="mt-4 max-w-lg text-lg text-primary-foreground/80">
              Join thousands of hosts earning extra income on Havenly. It's free to get started.
            </p>
            <div className="mt-8 flex flex-wrap gap-3">
              <Button asChild size="lg" variant="secondary" className="rounded-xl">
                <Link to="/host/properties/new">Start hosting</Link>
              </Button>
              <Button
                asChild
                size="lg"
                variant="outline"
                className="rounded-xl border-background/40 bg-transparent text-primary-foreground hover:bg-background/15 hover:text-primary-foreground"
              >
                <Link to="/host">Learn more</Link>
              </Button>
            </div>
          </div>
        </div>
      </section>
    </GuestShell>
  );
}

function ViewAll({ label }: { label: string }) {
  return (
    <Link
      to="/search"
      search={{ guests: 2 }}
      className="flex items-center gap-1 text-base font-semibold text-primary hover:underline"
    >
      {label} <ArrowRight className="size-4" />
    </Link>
  );
}

function SectionHead({
  eyebrow,
  title,
  action,
}: {
  eyebrow: string;
  title: string;
  action?: ReactNode;
}) {
  return (
    <div className="mb-8 flex flex-wrap items-end justify-between gap-4">
      <div>
        <p className="mb-2 text-sm font-semibold uppercase tracking-wide text-primary">{eyebrow}</p>
        <h2 className="text-3xl">{title}</h2>
      </div>
      {action}
    </div>
  );
}

function Section({
  eyebrow,
  title,
  action,
  children,
}: {
  eyebrow: string;
  title: string;
  action?: ReactNode;
  children: ReactNode;
}) {
  return (
    <section className="container-page py-8">
      <SectionHead eyebrow={eyebrow} title={title} {...(action ? { action } : {})} />
      {children}
    </section>
  );
}
