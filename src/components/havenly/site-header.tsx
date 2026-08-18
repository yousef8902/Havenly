import { Link } from "@tanstack/react-router";
import { CalendarDays, Heart, LayoutDashboard, Menu, Search, Shield, UserRound } from "lucide-react";
import { useState } from "react";
import { Logo } from "./brand";
import { Button } from "@/components/ui/button";
import { Sheet, SheetContent, SheetHeader, SheetTitle, SheetTrigger } from "@/components/ui/sheet";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { Avatar, AvatarFallback } from "@/components/ui/avatar";

const guestLinks = [
  { to: "/search", label: "Explore" },
  { to: "/bookings", label: "My bookings" },
  { to: "/favorites", label: "Favorites" },
] as const;

export function SiteHeader() {
  const [open, setOpen] = useState(false);

  return (
    <header className="sticky top-0 z-40 border-b border-border bg-background/85 backdrop-blur">
      <div className="container-page flex h-16 items-center justify-between gap-4">
        <Logo />

        <nav aria-label="Main" className="hidden items-center gap-1 md:flex">
          {guestLinks.map((l) => (
            <Link
              key={l.to}
              to={l.to}
              activeProps={{ className: "bg-secondary text-foreground" }}
              className="rounded-full px-4 py-2 text-sm font-medium text-muted-foreground transition-colors hover:text-foreground"
            >
              {l.label}
            </Link>
          ))}
        </nav>

        <div className="flex items-center gap-2">
          <Button asChild variant="ghost" className="hidden lg:inline-flex">
            <Link to="/host">Become a host</Link>
          </Button>

          <DropdownMenu>
            <DropdownMenuTrigger asChild>
              <Button variant="outline" className="hidden h-11 gap-2 rounded-full pl-3 pr-2 md:inline-flex">
                <UserRound className="size-4" />
                <Avatar className="size-7">
                  <AvatarFallback className="bg-primary text-xs text-primary-foreground">NR</AvatarFallback>
                </Avatar>
              </Button>
            </DropdownMenuTrigger>
            <DropdownMenuContent align="end" className="w-56">
              <DropdownMenuLabel>Nadia Rahman · Guest</DropdownMenuLabel>
              <DropdownMenuSeparator />
              <DropdownMenuItem asChild>
                <Link to="/bookings">My bookings</Link>
              </DropdownMenuItem>
              <DropdownMenuItem asChild>
                <Link to="/favorites">Favorites</Link>
              </DropdownMenuItem>
              <DropdownMenuItem asChild>
                <Link to="/reviews">Reviews</Link>
              </DropdownMenuItem>
              <DropdownMenuSeparator />
              <DropdownMenuLabel className="text-xs uppercase tracking-wide text-muted-foreground">
                Switch workspace
              </DropdownMenuLabel>
              <DropdownMenuItem asChild>
                <Link to="/host">Host dashboard</Link>
              </DropdownMenuItem>
              <DropdownMenuItem asChild>
                <Link to="/admin">Admin console</Link>
              </DropdownMenuItem>
            </DropdownMenuContent>
          </DropdownMenu>

          <Sheet open={open} onOpenChange={setOpen}>
            <SheetTrigger asChild>
              <Button variant="outline" size="icon" className="size-11 md:hidden" aria-label="Open menu">
                <Menu className="size-5" />
              </Button>
            </SheetTrigger>
            <SheetContent side="right" className="w-80">
              <SheetHeader>
                <SheetTitle>Menu</SheetTitle>
              </SheetHeader>
              <nav className="flex flex-col gap-1 px-4" aria-label="Mobile">
                {[
                  { to: "/search", label: "Explore stays", Icon: Search },
                  { to: "/bookings", label: "My bookings", Icon: CalendarDays },
                  { to: "/favorites", label: "Favorites", Icon: Heart },
                  { to: "/reviews", label: "Reviews", Icon: UserRound },
                  { to: "/host", label: "Host dashboard", Icon: LayoutDashboard },
                  { to: "/admin", label: "Admin console", Icon: Shield },
                ].map(({ to, label, Icon }) => (
                  <Link
                    key={to}
                    to={to}
                    onClick={() => setOpen(false)}
                    className="flex min-h-12 items-center gap-3 rounded-xl px-3 text-sm font-medium hover:bg-secondary"
                  >
                    <Icon className="size-4 text-muted-foreground" />
                    {label}
                  </Link>
                ))}
              </nav>
            </SheetContent>
          </Sheet>
        </div>
      </div>
    </header>
  );
}

export function MobileTabBar() {
  const items = [
    { to: "/search", label: "Explore", Icon: Search },
    { to: "/favorites", label: "Saved", Icon: Heart },
    { to: "/bookings", label: "Trips", Icon: CalendarDays },
    { to: "/host", label: "Host", Icon: LayoutDashboard },
  ] as const;

  return (
    <nav
      aria-label="Quick navigation"
      className="fixed inset-x-0 bottom-0 z-40 border-t border-border bg-card/95 backdrop-blur md:hidden"
    >
      <ul className="grid grid-cols-4">
        {items.map(({ to, label, Icon }) => (
          <li key={to}>
            <Link
              to={to}
              activeProps={{ className: "text-primary" }}
              className="flex min-h-14 flex-col items-center justify-center gap-1 text-xs font-medium text-muted-foreground"
            >
              <Icon className="size-5" />
              {label}
            </Link>
          </li>
        ))}
      </ul>
    </nav>
  );
}