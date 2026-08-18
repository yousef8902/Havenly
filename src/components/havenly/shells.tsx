import { Link, type LinkProps } from "@tanstack/react-router";
import { Menu } from "lucide-react";
import type { ComponentType, ReactNode } from "react";
import { useState } from "react";
import { Logo } from "./brand";
import { MobileTabBar, SiteHeader } from "./site-header";
import { SiteFooter } from "./site-footer";
import { Button } from "@/components/ui/button";
import { Sheet, SheetContent, SheetHeader, SheetTitle, SheetTrigger } from "@/components/ui/sheet";

export function GuestShell({
  children,
  transparentHeader = false,
}: {
  children: ReactNode;
  transparentHeader?: boolean;
}) {
  return (
    <div className="flex min-h-screen flex-col">
      <SiteHeader transparent={transparentHeader} />
      <main className="flex-1 pb-20 md:pb-0">{children}</main>
      <SiteFooter />
      <MobileTabBar />
    </div>
  );
}

export type NavItem = {
  to: NonNullable<LinkProps["to"]>;
  label: string;
  Icon: ComponentType<{ className?: string }>;
  exact?: boolean;
};

function NavList({ items, onNavigate }: { items: NavItem[]; onNavigate?: () => void }) {
  return (
    <nav className="flex flex-col gap-1" aria-label="Workspace">
      {items.map((item) => (
        <Link
          key={item.to}
          to={item.to}
          activeOptions={{ exact: item.exact ?? false }}
          activeProps={{ className: "bg-sidebar-accent text-sidebar-accent-foreground font-semibold" }}
          onClick={onNavigate}
          className="flex min-h-11 items-center gap-3 rounded-xl px-3 text-sm text-muted-foreground transition-colors hover:bg-sidebar-accent hover:text-foreground"
        >
          <item.Icon className="size-4" />
          {item.label}
        </Link>
      ))}
    </nav>
  );
}

export function WorkspaceShell({
  items,
  workspace,
  role,
  children,
}: {
  items: NavItem[];
  workspace: string;
  role: string;
  children: ReactNode;
}) {
  const [open, setOpen] = useState(false);

  return (
    <div className="min-h-screen bg-background lg:grid lg:grid-cols-[16rem_1fr]">
      <aside className="sticky top-0 hidden h-screen flex-col border-r border-sidebar-border bg-sidebar p-4 lg:flex">
        <Logo />
        <p className="mt-6 px-3 text-xs font-semibold uppercase tracking-wide text-muted-foreground">
          {workspace}
        </p>
        <div className="mt-2">
          <NavList items={items} />
        </div>
        <div className="mt-auto rounded-xl border border-sidebar-border bg-card p-3">
          <p className="text-sm font-medium">{role}</p>
          <Link to="/" className="text-xs text-primary underline-offset-2 hover:underline">
            Back to Havenly
          </Link>
        </div>
      </aside>

      <div className="flex min-h-screen flex-col">
        <header className="sticky top-0 z-30 flex h-16 items-center justify-between gap-3 border-b border-border bg-background/90 px-5 backdrop-blur lg:hidden">
          <Logo />
          <Sheet open={open} onOpenChange={setOpen}>
            <SheetTrigger asChild>
              <Button variant="outline" size="icon" className="size-11" aria-label="Open workspace menu">
                <Menu className="size-5" />
              </Button>
            </SheetTrigger>
            <SheetContent side="left" className="w-72">
              <SheetHeader>
                <SheetTitle>{workspace}</SheetTitle>
              </SheetHeader>
              <div className="px-4">
                <NavList items={items} onNavigate={() => setOpen(false)} />
              </div>
            </SheetContent>
          </Sheet>
        </header>
        <main className="flex-1 px-5 py-8 lg:px-10 lg:py-10">{children}</main>
      </div>
    </div>
  );
}

export function PageHeading({
  title,
  description,
  action,
}: {
  title: string;
  description?: string;
  action?: ReactNode;
}) {
  return (
    <div className="mb-8 flex flex-wrap items-end justify-between gap-4">
      <div>
        <h1 className="text-3xl">{title}</h1>
        {description && <p className="mt-1 text-muted-foreground">{description}</p>}
      </div>
      {action}
    </div>
  );
}

export function StatCard({
  label,
  value,
  hint,
  Icon,
}: {
  label: string;
  value: string;
  hint?: string;
  Icon: ComponentType<{ className?: string }>;
}) {
  return (
    <div className="rounded-2xl border border-border bg-card p-5">
      <div className="flex items-center justify-between">
        <p className="text-sm text-muted-foreground">{label}</p>
        <Icon className="size-4 text-muted-foreground" />
      </div>
      <p className="mt-3 font-display text-3xl font-semibold">{value}</p>
      {hint && <p className="mt-1 text-xs text-muted-foreground">{hint}</p>}
    </div>
  );
}