import { createFileRoute, useNavigate } from "@tanstack/react-router";
import { SlidersHorizontal, MapPinOff } from "lucide-react";
import { useMemo, useState } from "react";
import { z } from "zod";
import { GuestShell } from "@/components/havenly/shells";
import { SearchBar } from "@/components/havenly/search-bar";
import { PropertyCard } from "@/components/havenly/property-card";
import { EmptyState } from "@/components/havenly/empty-state";
import { Button } from "@/components/ui/button";
import { Checkbox } from "@/components/ui/checkbox";
import { Label } from "@/components/ui/label";
import { Slider } from "@/components/ui/slider";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Sheet, SheetContent, SheetHeader, SheetTitle, SheetTrigger } from "@/components/ui/sheet";
import {
  Pagination,
  PaginationContent,
  PaginationItem,
  PaginationLink,
} from "@/components/ui/pagination";
import { amenityList, categories, properties } from "@/lib/data";

const searchSchema = z.object({
  city: z.string().optional(),
  checkIn: z.string().optional(),
  checkOut: z.string().optional(),
  guests: z.number().optional(),
  category: z.string().optional(),
});

export const Route = createFileRoute("/search")({
  validateSearch: searchSchema,
  head: () => ({
    meta: [
      { title: "Search stays — Havenly" },
      {
        name: "description",
        content:
          "Filter Havenly stays by city, price, guests, rooms, rating and amenities to find the right home for your dates.",
      },
      { property: "og:title", content: "Search stays — Havenly" },
      { property: "og:description", content: "Filter homes by city, price, guests and amenities." },
    ],
  }),
  component: SearchPage,
});

const PAGE_SIZE = 6;

function SearchPage() {
  const search = Route.useSearch();
  const navigate = useNavigate({ from: "/search" });

  const [maxPrice, setMaxPrice] = useState(450);
  const [minRooms, setMinRooms] = useState("any");
  const [minRating, setMinRating] = useState("any");
  const [amenities, setAmenities] = useState<string[]>([]);
  const [sort, setSort] = useState("recommended");
  const [page, setPage] = useState(1);

  const results = useMemo(() => {
    let list = properties.filter((p) => p.status === "approved");
    if (search.city) {
      const q = search.city.toLowerCase();
      list = list.filter(
        (p) =>
          p.city.toLowerCase().includes(q) ||
          p.country.toLowerCase().includes(q) ||
          p.title.toLowerCase().includes(q),
      );
    }
    if (search.category) list = list.filter((p) => p.category === search.category);
    if (search.guests) list = list.filter((p) => p.guests >= search.guests!);
    list = list.filter((p) => p.price <= maxPrice);
    if (minRooms !== "any") list = list.filter((p) => p.bedrooms >= Number(minRooms));
    if (minRating !== "any") list = list.filter((p) => p.rating >= Number(minRating));
    if (amenities.length) list = list.filter((p) => amenities.every((a) => p.amenities.includes(a)));

    if (sort === "price-asc") list = [...list].sort((a, b) => a.price - b.price);
    if (sort === "price-desc") list = [...list].sort((a, b) => b.price - a.price);
    if (sort === "rating") list = [...list].sort((a, b) => b.rating - a.rating);
    return list;
  }, [search.city, search.category, search.guests, maxPrice, minRooms, minRating, amenities, sort]);

  const pages = Math.max(1, Math.ceil(results.length / PAGE_SIZE));
  const current = Math.min(page, pages);
  const visible = results.slice((current - 1) * PAGE_SIZE, current * PAGE_SIZE);

  function reset() {
    setMaxPrice(450);
    setMinRooms("any");
    setMinRating("any");
    setAmenities([]);
    navigate({ search: { guests: search.guests ?? 2 } });
  }

  const filters = (
    <FilterPanel
      city={search.city ?? ""}
      onCity={(city) => navigate({ search: (prev) => ({ ...prev, city: city || undefined }) })}
      category={search.category ?? "any"}
      onCategory={(category) =>
        navigate({ search: (prev) => ({ ...prev, category: category === "any" ? undefined : category }) })
      }
      guests={search.guests ?? 1}
      onGuests={(guests) => navigate({ search: (prev) => ({ ...prev, guests }) })}
      maxPrice={maxPrice}
      onMaxPrice={setMaxPrice}
      minRooms={minRooms}
      onMinRooms={setMinRooms}
      minRating={minRating}
      onMinRating={setMinRating}
      amenities={amenities}
      onAmenities={setAmenities}
      onReset={reset}
    />
  );

  return (
    <GuestShell>
      <div className="container-page pt-6">
        <SearchBar
          variant="compact"
          initial={{
            city: search.city ?? "",
            checkIn: search.checkIn ?? "",
            checkOut: search.checkOut ?? "",
            guests: search.guests ?? 2,
          }}
        />
      </div>

      <div className="container-page mt-8 grid gap-8 lg:grid-cols-[17rem_1fr]">
        <aside className="hidden lg:block">
          <div className="sticky top-24 rounded-2xl border border-border bg-card p-5">{filters}</div>
        </aside>

        <div>
          <div className="mb-5 flex flex-wrap items-center justify-between gap-3">
            <div>
              <h1 className="text-2xl">
                {search.city ? `Stays in ${search.city}` : "All stays"}
              </h1>
              <p className="text-sm text-muted-foreground">
                {results.length} {results.length === 1 ? "home" : "homes"} match your filters
              </p>
            </div>
            <div className="flex items-center gap-2">
              <Sheet>
                <SheetTrigger asChild>
                  <Button variant="outline" className="h-11 gap-2 lg:hidden">
                    <SlidersHorizontal className="size-4" /> Filters
                  </Button>
                </SheetTrigger>
                <SheetContent side="bottom" className="max-h-[85vh] overflow-y-auto">
                  <SheetHeader>
                    <SheetTitle>Filters</SheetTitle>
                  </SheetHeader>
                  <div className="px-4 pb-8">{filters}</div>
                </SheetContent>
              </Sheet>

              <Select value={sort} onValueChange={setSort}>
                <SelectTrigger className="h-11 w-48" aria-label="Sort results">
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="recommended">Recommended</SelectItem>
                  <SelectItem value="price-asc">Price: low to high</SelectItem>
                  <SelectItem value="price-desc">Price: high to low</SelectItem>
                  <SelectItem value="rating">Highest rated</SelectItem>
                </SelectContent>
              </Select>
            </div>
          </div>

          {visible.length === 0 ? (
            <EmptyState
              icon={<MapPinOff className="size-5" />}
              title="No stays match these filters"
              description="Try widening your price range, lowering the minimum rating, or removing an amenity."
              action={
                <Button onClick={reset} variant="outline">
                  Clear all filters
                </Button>
              }
            />
          ) : (
            <>
              <div className="grid gap-6 sm:grid-cols-2 xl:grid-cols-3">
                {visible.map((p) => (
                  <PropertyCard key={p.id} property={p} />
                ))}
              </div>

              {pages > 1 && (
                <Pagination className="mt-10">
                  <PaginationContent>
                    {Array.from({ length: pages }, (_, i) => i + 1).map((n) => (
                      <PaginationItem key={n}>
                        <PaginationLink
                          href="#"
                          isActive={n === current}
                          onClick={(e) => {
                            e.preventDefault();
                            setPage(n);
                          }}
                        >
                          {n}
                        </PaginationLink>
                      </PaginationItem>
                    ))}
                  </PaginationContent>
                </Pagination>
              )}
            </>
          )}
        </div>
      </div>
    </GuestShell>
  );
}

function FilterPanel(props: {
  city: string;
  onCity: (v: string) => void;
  category: string;
  onCategory: (v: string) => void;
  guests: number;
  onGuests: (v: number) => void;
  maxPrice: number;
  onMaxPrice: (v: number) => void;
  minRooms: string;
  onMinRooms: (v: string) => void;
  minRating: string;
  onMinRating: (v: string) => void;
  amenities: string[];
  onAmenities: (v: string[]) => void;
  onReset: () => void;
}) {
  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <h2 className="font-sans text-sm font-semibold uppercase tracking-wide">Filters</h2>
        <Button variant="ghost" size="sm" onClick={props.onReset}>
          Reset
        </Button>
      </div>

      <div className="space-y-2">
        <Label htmlFor="f-category">Category</Label>
        <Select value={props.category} onValueChange={props.onCategory}>
          <SelectTrigger id="f-category" className="h-11">
            <SelectValue placeholder="Any category" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="any">Any category</SelectItem>
            {categories.map((c) => (
              <SelectItem key={c} value={c}>
                {c}
              </SelectItem>
            ))}
          </SelectContent>
        </Select>
      </div>

      <div className="space-y-3">
        <div className="flex items-center justify-between">
          <Label htmlFor="f-price">Max price per night</Label>
          <span className="text-sm font-semibold">${props.maxPrice}</span>
        </div>
        <Slider
          id="f-price"
          value={[props.maxPrice]}
          min={80}
          max={600}
          step={10}
          onValueChange={([v]) => props.onMaxPrice(v ?? 600)}
        />
      </div>

      <div className="space-y-2">
        <Label htmlFor="f-guests">Guests</Label>
        <Select value={String(props.guests)} onValueChange={(v) => props.onGuests(Number(v))}>
          <SelectTrigger id="f-guests" className="h-11">
            <SelectValue />
          </SelectTrigger>
          <SelectContent>
            {[1, 2, 4, 6, 8].map((n) => (
              <SelectItem key={n} value={String(n)}>
                {n}+ guests
              </SelectItem>
            ))}
          </SelectContent>
        </Select>
      </div>

      <div className="space-y-2">
        <Label htmlFor="f-rooms">Bedrooms</Label>
        <Select value={props.minRooms} onValueChange={props.onMinRooms}>
          <SelectTrigger id="f-rooms" className="h-11">
            <SelectValue />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="any">Any</SelectItem>
            {[1, 2, 3, 4].map((n) => (
              <SelectItem key={n} value={String(n)}>
                {n}+ bedrooms
              </SelectItem>
            ))}
          </SelectContent>
        </Select>
      </div>

      <div className="space-y-2">
        <Label htmlFor="f-rating">Guest rating</Label>
        <Select value={props.minRating} onValueChange={props.onMinRating}>
          <SelectTrigger id="f-rating" className="h-11">
            <SelectValue />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="any">Any rating</SelectItem>
            <SelectItem value="4.5">4.5+</SelectItem>
            <SelectItem value="4.8">4.8+</SelectItem>
            <SelectItem value="4.9">4.9+</SelectItem>
          </SelectContent>
        </Select>
      </div>

      <fieldset className="space-y-3">
        <legend className="text-sm font-medium">Amenities</legend>
        <div className="grid grid-cols-1 gap-2.5">
          {amenityList.slice(0, 8).map((a) => {
            const id = `am-${a.replace(/\s+/g, "-").toLowerCase()}`;
            const checked = props.amenities.includes(a);
            return (
              <div key={a} className="flex items-center gap-3">
                <Checkbox
                  id={id}
                  checked={checked}
                  onCheckedChange={() =>
                    props.onAmenities(
                      checked ? props.amenities.filter((x) => x !== a) : [...props.amenities, a],
                    )
                  }
                />
                <Label htmlFor={id} className="text-sm font-normal">
                  {a}
                </Label>
              </div>
            );
          })}
        </div>
      </fieldset>
    </div>
  );
}