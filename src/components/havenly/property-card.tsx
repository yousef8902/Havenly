import { Link } from "@tanstack/react-router";
import { Heart, Users } from "lucide-react";
import { Rating } from "./rating";
import { StatusBadge } from "./status-badge";
import { useAppState } from "@/lib/app-state";
import { formatMoney, type Property } from "@/lib/data";

export function PropertyCard({
  property,
  showStatus = false,
}: {
  property: Property;
  showStatus?: boolean;
}) {
  const { isFavorite, toggleFavorite } = useAppState();
  const saved = isFavorite(property.id);

  return (
    <article className="group relative overflow-hidden rounded-2xl border border-border bg-card transition-shadow hover:shadow-lift">
      <div className="relative aspect-4/3 overflow-hidden bg-secondary">
        <img
          src={property.images[0]}
          alt={property.title}
          loading="lazy"
          width={1200}
          height={900}
          className="size-full object-cover transition-transform duration-500 group-hover:scale-105"
        />
        <button
          type="button"
          onClick={() => toggleFavorite(property.id)}
          aria-pressed={saved}
          aria-label={saved ? `Remove ${property.title} from favorites` : `Save ${property.title} to favorites`}
          className="absolute right-3 top-3 grid size-11 place-items-center rounded-full bg-card/90 text-foreground backdrop-blur transition-colors hover:bg-card"
        >
          <Heart className={`size-5 ${saved ? "fill-destructive text-destructive" : ""}`} />
        </button>
        {showStatus && (
          <div className="absolute left-3 top-3">
            <StatusBadge status={property.status} className="bg-card/95 backdrop-blur" />
          </div>
        )}
      </div>

      <div className="space-y-2 p-4">
        <div className="flex items-start justify-between gap-3">
          <h3 className="text-base font-semibold leading-snug">
            <Link
              to="/property/$propertyId"
              params={{ propertyId: property.id }}
              className="after:absolute after:inset-0 after:content-['']"
            >
              {property.title}
            </Link>
          </h3>
          <Rating value={property.rating} className="shrink-0 pt-0.5" />
        </div>
        <p className="text-sm text-muted-foreground">
          {property.neighbourhood}, {property.city} · {property.country}
        </p>
        <div className="flex items-center justify-between pt-1">
          <span className="inline-flex items-center gap-1.5 text-sm text-muted-foreground">
            <Users className="size-4" aria-hidden="true" />
            {property.guests} guests · {property.bedrooms} bd
          </span>
          <span className="text-sm text-muted-foreground">
            <span className="text-base font-semibold text-foreground">
              {formatMoney(property.price)}
            </span>{" "}
            / night
          </span>
        </div>
        <p className="text-xs text-muted-foreground">{property.reviews} reviews</p>
      </div>
    </article>
  );
}

export function PropertyCardSkeleton() {
  return (
    <div className="overflow-hidden rounded-2xl border border-border bg-card">
      <div className="aspect-4/3 animate-pulse bg-secondary" />
      <div className="space-y-3 p-4">
        <div className="h-4 w-3/4 animate-pulse rounded bg-secondary" />
        <div className="h-3 w-1/2 animate-pulse rounded bg-secondary" />
        <div className="h-3 w-1/3 animate-pulse rounded bg-secondary" />
      </div>
    </div>
  );
}