import { createFileRoute, Link } from "@tanstack/react-router";
import { HeartOff } from "lucide-react";
import { GuestShell } from "@/components/havenly/shells";
import { PropertyCard } from "@/components/havenly/property-card";
import { EmptyState } from "@/components/havenly/empty-state";
import { Button } from "@/components/ui/button";
import { useAppState } from "@/lib/app-state";
import { properties } from "@/lib/data";

export const Route = createFileRoute("/favorites")({
  head: () => ({
    meta: [
      { title: "Saved stays — Havenly" },
      { name: "description", content: "Every Havenly home you've saved, ready to compare when your dates are set." },
      { property: "og:title", content: "Saved stays — Havenly" },
      { property: "og:description", content: "Compare the homes you've saved on Havenly." },
    ],
  }),
  component: FavoritesPage,
});

function FavoritesPage() {
  const { favorites } = useAppState();
  const saved = properties.filter((p) => favorites.includes(p.id));

  return (
    <GuestShell>
      <div className="container-page py-12">
        <h1 className="text-3xl md:text-4xl">Saved stays</h1>
        <p className="mt-1 text-muted-foreground">
          {saved.length} {saved.length === 1 ? "home" : "homes"} saved. Tap the heart on any card to
          remove it.
        </p>

        <div className="mt-8">
          {saved.length === 0 ? (
            <EmptyState
              icon={<HeartOff className="size-5" />}
              title="Nothing saved yet"
              description="Tap the heart on any listing and it will wait for you here while you make up your mind."
              action={
                <Button asChild>
                  <Link to="/search" search={{ guests: 2 }}>
                    Browse stays
                  </Link>
                </Button>
              }
            />
          ) : (
            <div className="grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
              {saved.map((p) => (
                <PropertyCard key={p.id} property={p} />
              ))}
            </div>
          )}
        </div>
      </div>
    </GuestShell>
  );
}