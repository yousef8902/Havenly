import { createFileRoute, Link } from "@tanstack/react-router";
import { ArrowRight, BadgeCheck, CalendarCheck, Headphones, Search, ShieldCheck } from "lucide-react";
import { GuestShell } from "@/components/havenly/shells";
import { SearchBar } from "@/components/havenly/search-bar";
import { PropertyCard } from "@/components/havenly/property-card";
import { Button } from "@/components/ui/button";
import { categories, destinations, images, properties } from "@/lib/data";

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
        content: "Search, compare and book stays in considered homes across Europe.",
      },
    ],
  }),
  component: Home,
});

const howItWorks = [
  { title: "Search with intent", body: "Filter by dates, guests, price and the amenities you actually use.", Icon: Search },
  { title: "Request your dates", body: "Availability is checked live, so you never request a booked night.", Icon: CalendarCheck },
  { title: "Host confirms", body: "Most hosts respond within a few hours. You are only charged once approved.", Icon: BadgeCheck },
];

function Home() {
  const featured = properties.filter((p) => p.status === "approved").slice(0, 3);
  const recommended = properties.filter((p) => p.status === "approved").slice(3, 7);

  return (
    <GuestShell>
      <section className="relative">
        <div className="relative h-[560px] w-full overflow-hidden md:h-[620px]">
          <img
            src={images.hero}
            alt="Cliffside villa terrace with an infinity pool overlooking the sea at sunset"
            width={1920}
            height={1280}
            className="size-full object-cover"
          />
          <div className="absolute inset-0 bg-foreground/40" />
          <div className="container-page absolute inset-x-0 top-24 md:top-28">
            <p className="text-sm font-semibold uppercase tracking-[0.2em] text-primary-foreground/85">
              Stays worth the journey
            </p>
            <h1 className="mt-3 max-w-2xl text-4xl leading-[1.05] text-primary-foreground md:text-6xl">
              Find a place that feels like home.
            </h1>
            <p className="mt-4 max-w-xl text-base text-primary-foreground/90 md:text-lg">
              A curated marketplace of independent homes — from Aegean villas to forest
              cabins — hosted by people who take pride in the details.
            </p>
          </div>
        </div>

        <div className="container-page relative -mt-24 md:-mt-20">
          <SearchBar />
        </div>
      </section>

      <Section title="Popular destinations" description="Where Havenly guests are heading this season.">
        <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
          {destinations.map((d) => (
            <Link
              key={d.city}
              to="/search"
              search={{ city: d.city, guests: 2 }}
              className="group relative overflow-hidden rounded-2xl border border-border"
            >
              <img
                src={d.image}
                alt={`${d.city}, ${d.country}`}
                loading="lazy"
                width={1200}
                height={900}
                className="h-48 w-full object-cover transition-transform duration-500 group-hover:scale-105"
              />
              <div className="absolute inset-0 bg-linear-to-t from-foreground/70 to-transparent" />
              <div className="absolute bottom-4 left-4 text-primary-foreground">
                <p className="font-display text-lg font-semibold">{d.city}</p>
                <p className="text-sm opacity-90">{d.stays} stays · {d.country}</p>
              </div>
            </Link>
          ))}
        </div>
      </Section>

      <Section
        title="Featured stays"
        description="Hand-reviewed homes with consistently excellent guest feedback."
        action={
          <Button asChild variant="ghost" className="gap-1">
            <Link to="/search" search={{ guests: 2 }}>
              See all <ArrowRight className="size-4" />
            </Link>
          </Button>
        }
      >
        <div className="grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
          {featured.map((p) => (
            <PropertyCard key={p.id} property={p} />
          ))}
        </div>
      </Section>

      <Section title="Browse by category">
        <div className="flex flex-wrap gap-2">
          {categories.map((c) => (
            <Link
              key={c}
              to="/search"
              search={{ category: c, guests: 2 }}
              className="rounded-full border border-border bg-card px-4 py-2.5 text-sm font-medium transition-colors hover:border-primary hover:text-primary"
            >
              {c}
            </Link>
          ))}
        </div>
      </Section>

      <Section title="Recommended for you" description="Based on stays you saved and viewed.">
        <div className="grid gap-6 sm:grid-cols-2 lg:grid-cols-4">
          {recommended.map((p) => (
            <PropertyCard key={p.id} property={p} />
          ))}
        </div>
      </Section>

      <Section title="How Havenly works">
        <div className="grid gap-6 md:grid-cols-3">
          {howItWorks.map(({ title, body, Icon }) => (
            <div key={title} className="rounded-2xl border border-border bg-card p-6">
              <span className="grid size-11 place-items-center rounded-xl bg-primary/10 text-primary">
                <Icon className="size-5" />
              </span>
              <h3 className="mt-4 text-lg">{title}</h3>
              <p className="mt-2 text-sm text-muted-foreground">{body}</p>
            </div>
          ))}
        </div>
      </Section>

      <Section title="Trust &amp; safety">
        <div className="grid gap-6 rounded-2xl border border-border bg-secondary/60 p-8 md:grid-cols-3">
          {[
            { Icon: ShieldCheck, t: "Verified hosts", b: "Every host completes ID and property verification before publishing." },
            { Icon: BadgeCheck, t: "Reviewed listings", b: "Our team reviews each new listing for accuracy before it goes live." },
            { Icon: Headphones, t: "Support that answers", b: "Real people, 24/7, for anything that comes up before or during a stay." },
          ].map(({ Icon, t, b }) => (
            <div key={t} className="flex gap-3">
              <Icon className="size-5 shrink-0 text-primary" />
              <div>
                <p className="font-semibold">{t}</p>
                <p className="mt-1 text-sm text-muted-foreground">{b}</p>
              </div>
            </div>
          ))}
        </div>
      </Section>

      <section className="container-page mt-20">
        <div className="grid items-center gap-8 overflow-hidden rounded-3xl border border-border bg-primary p-8 text-primary-foreground md:grid-cols-2 md:p-12">
          <div>
            <h2 className="text-3xl text-primary-foreground md:text-4xl">
              Your spare place could be someone's favourite stay.
            </h2>
            <p className="mt-3 max-w-md text-primary-foreground/85">
              List in five guided steps, set your own availability, and approve every
              request yourself. No listing fee while you're getting started.
            </p>
            <Button asChild variant="secondary" size="lg" className="mt-6">
              <Link to="/host/properties/new">Start hosting</Link>
            </Button>
          </div>
          <img
            src={images.p2}
            alt="Restored stone farmhouse in the Tuscan countryside"
            loading="lazy"
            width={1200}
            height={900}
            className="h-64 w-full rounded-2xl object-cover"
          />
        </div>
      </section>
    </GuestShell>
  );
}

function Section({
  title,
  description,
  action,
  children,
}: {
  title: string;
  description?: string;
  action?: React.ReactNode;
  children: React.ReactNode;
}) {
  return (
    <section className="container-page mt-20">
      <div className="mb-6 flex flex-wrap items-end justify-between gap-3">
        <div>
          <h2 className="text-2xl md:text-3xl">{title}</h2>
          {description && <p className="mt-1 text-muted-foreground">{description}</p>}
        </div>
        {action}
      </div>
      {children}
    </section>
  );
}
