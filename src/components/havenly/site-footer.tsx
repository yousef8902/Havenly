import { Link } from "@tanstack/react-router";
import { Logo } from "./brand";

const columns = [
  {
    title: "Discover",
    links: [
      { label: "Explore stays", to: "/search" },
      { label: "Favorites", to: "/favorites" },
      { label: "My bookings", to: "/bookings" },
    ],
  },
  {
    title: "Hosting",
    links: [
      { label: "Host dashboard", to: "/host" },
      { label: "List your place", to: "/host/properties/new" },
      { label: "Booking requests", to: "/host/bookings" },
    ],
  },
  {
    title: "Platform",
    links: [
      { label: "Admin console", to: "/admin" },
      { label: "Activity", to: "/admin/activity" },
      { label: "Reviews", to: "/reviews" },
    ],
  },
] as const;

export function SiteFooter() {
  return (
    <footer className="mt-24 border-t border-border bg-secondary/50">
      <div className="container-page grid gap-10 py-14 md:grid-cols-[1.4fr_repeat(3,1fr)]">
        <div className="space-y-3">
          <Logo />
          <p className="max-w-xs text-sm text-muted-foreground">
            Havenly is a short-term rental marketplace for considered homes and the people
            who care for them.
          </p>
        </div>
        {columns.map((col) => (
          <div key={col.title}>
            <h2 className="font-sans text-sm font-semibold">{col.title}</h2>
            <ul className="mt-3 space-y-2">
              {col.links.map((l) => (
                <li key={l.label}>
                  <Link
                    to={l.to}
                    className="text-sm text-muted-foreground transition-colors hover:text-foreground"
                  >
                    {l.label}
                  </Link>
                </li>
              ))}
            </ul>
          </div>
        ))}
      </div>
      <div className="container-page flex flex-col gap-2 border-t border-border py-6 text-sm text-muted-foreground sm:flex-row sm:items-center sm:justify-between">
        <p>© 2026 Havenly. A university capstone project.</p>
        <p>Privacy · Terms · Trust &amp; Safety</p>
      </div>
    </footer>
  );
}