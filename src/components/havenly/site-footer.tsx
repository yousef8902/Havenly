import { Link } from "@tanstack/react-router";
import { Globe, Instagram, Twitter, Facebook } from "lucide-react";

const columns = [
  {
    title: "Company",
    links: [
      { label: "About Us", to: "/search" as const },
      { label: "Careers", to: "/search" as const },
      { label: "Press", to: "/search" as const },
      { label: "Blog", to: "/search" as const },
    ],
  },
  {
    title: "Support",
    links: [
      { label: "Help Center", to: "/search" as const },
      { label: "My bookings", to: "/bookings" as const },
      { label: "Cancellation", to: "/bookings" as const },
      { label: "Reviews", to: "/reviews" as const },
    ],
  },
  {
    title: "Hosting",
    links: [
      { label: "Become a Host", to: "/host" as const },
      { label: "List your place", to: "/host/properties/new" as const },
      { label: "Booking requests", to: "/host/bookings" as const },
      { label: "Responsible Hosting", to: "/host" as const },
    ],
  },
  {
    title: "Platform",
    links: [
      { label: "Admin console", to: "/admin" as const },
      { label: "Activity log", to: "/admin/activity" as const },
      { label: "Saved stays", to: "/favorites" as const },
      { label: "Explore", to: "/search" as const },
    ],
  },
];

export function SiteFooter() {
  return (
    <footer className="mt-24 bg-foreground text-background">
      <div className="container-page py-16">
        <div className="flex flex-col gap-12 lg:flex-row lg:items-start lg:justify-between">
          <div className="max-w-[260px]">
            <div className="flex items-center gap-2">
              <span className="grid size-8 place-items-center rounded-[10px] bg-accent text-foreground">
                <svg viewBox="0 0 24 24" className="size-4" fill="none" aria-hidden="true">
                  <path
                    d="M4 11.2 12 4.5l8 6.7V19a1 1 0 0 1-1 1h-4.5v-5h-5v5H5a1 1 0 0 1-1-1z"
                    stroke="currentColor"
                    strokeWidth="1.8"
                    strokeLinejoin="round"
                  />
                </svg>
              </span>
              <span className="font-display text-xl font-bold tracking-tight">havenly</span>
            </div>
            <p className="mt-4 text-sm text-background/50">
              A curated marketplace of independent homes and the hosts who care for them.
            </p>
            <div className="mt-5 flex gap-2">
              {[Instagram, Twitter, Facebook].map((Icon, i) => (
                <span
                  key={i}
                  className="grid size-8 place-items-center rounded-lg bg-background/10 text-background/80"
                >
                  <Icon className="size-4" />
                </span>
              ))}
            </div>
          </div>

          <div className="grid grid-cols-2 gap-10 sm:grid-cols-4 lg:gap-16">
            {columns.map((col) => (
              <div key={col.title}>
                <h2 className="font-sans text-sm font-semibold text-background/90">{col.title}</h2>
                <ul className="mt-4 flex flex-col gap-2.5">
                  {col.links.map((l) => (
                    <li key={l.label}>
                      <Link
                        to={l.to}
                        className="text-sm text-background/50 transition-colors hover:text-background"
                      >
                        {l.label}
                      </Link>
                    </li>
                  ))}
                </ul>
              </div>
            ))}
          </div>
        </div>

        <div className="mt-14 flex flex-col gap-3 border-t border-background/10 pt-8 text-sm text-background/40 sm:flex-row sm:items-center sm:justify-between">
          <p>© 2026 Havenly, Inc. All rights reserved.</p>
          <p className="flex items-center gap-2">
            <Globe className="size-4" /> English (US)
          </p>
        </div>
      </div>
    </footer>
  );
}
