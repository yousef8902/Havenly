import { createFileRoute } from "@tanstack/react-router";
import { useState } from "react";
import { Ban, CalendarCheck, CheckCircle2, Home, Star, UserPlus } from "lucide-react";
import { PageHeading, WorkspaceShell } from "@/components/havenly/shells";
import { adminNav } from "@/components/havenly/workspace-nav";
import { Button } from "@/components/ui/button";
import { activity } from "@/lib/data";

export const Route = createFileRoute("/admin/activity")({
  head: () => ({
    meta: [
      { title: "Activity log — Havenly Admin" },
      { name: "description", content: "A chronological audit trail of listings, bookings, reviews and account changes across Havenly." },
      { property: "og:title", content: "Activity log — Havenly Admin" },
      { property: "og:description", content: "Audit trail of platform events across Havenly." },
      { property: "og:type", content: "website" },
      { name: "twitter:card", content: "summary_large_image" },
    ],
  }),
  component: AdminActivity,
});

const icons: Record<string, typeof Home> = {
  listing: Home,
  booking: CalendarCheck,
  user: UserPlus,
  review: Star,
  approved: CheckCircle2,
  cancelled: Ban,
};

const filters = ["all", "listing", "booking", "user", "review", "approved", "cancelled"];

function AdminActivity() {
  const [filter, setFilter] = useState("all");
  const rows = activity.filter((a) => filter === "all" || a.type === filter);

  return (
    <WorkspaceShell items={adminNav} workspace="Admin console" role="Omar Khalil · Administrator">
      <PageHeading title="Activity log" description="Everything that happened on the platform, newest first." />

      <div className="mb-6 flex flex-wrap gap-2">
        {filters.map((f) => (
          <Button
            key={f}
            size="sm"
            variant={filter === f ? "default" : "outline"}
            onClick={() => setFilter(f)}
            className="capitalize"
          >
            {f}
          </Button>
        ))}
      </div>

      <ol className="relative space-y-4 border-l border-border pl-6">
        {rows.map((a) => {
          const Icon = icons[a.type] ?? Home;
          return (
            <li key={a.id} className="relative rounded-2xl border border-border bg-card p-4">
              <span className="absolute -left-[2.15rem] top-5 grid size-6 place-items-center rounded-full border border-border bg-background">
                <Icon className="size-3.5 text-primary" />
              </span>
              <p className="text-sm">{a.text}</p>
              <p className="mt-1 text-xs text-muted-foreground">{a.time}</p>
            </li>
          );
        })}
      </ol>
    </WorkspaceShell>
  );
}
