import { useNavigate } from "@tanstack/react-router";
import { Minus, Plus, Search } from "lucide-react";
import { useState } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Popover, PopoverContent, PopoverTrigger } from "@/components/ui/popover";

export function SearchBar({
  variant = "hero",
  initial,
}: {
  variant?: "hero" | "compact";
  initial?: { city?: string; checkIn?: string; checkOut?: string; guests?: number };
}) {
  const navigate = useNavigate();
  const [city, setCity] = useState(initial?.city ?? "");
  const [checkIn, setCheckIn] = useState(initial?.checkIn ?? "");
  const [checkOut, setCheckOut] = useState(initial?.checkOut ?? "");
  const [guests, setGuests] = useState(initial?.guests ?? 2);

  const invalidRange = Boolean(checkIn && checkOut && new Date(checkOut) <= new Date(checkIn));

  function submit(e: React.FormEvent) {
    e.preventDefault();
    if (invalidRange) return;
    navigate({
      to: "/search",
      search: {
        city: city || undefined,
        checkIn: checkIn || undefined,
        checkOut: checkOut || undefined,
        guests,
      },
    });
  }

  return (
    <form
      onSubmit={submit}
      className={
        variant === "hero"
          ? "rounded-2xl border border-border bg-card p-3 shadow-lift"
          : "rounded-2xl border border-border bg-card p-2.5"
      }
      aria-label="Search stays"
    >
      <div className="grid gap-2.5 md:grid-cols-[1.4fr_1fr_1fr_auto_auto] md:items-end">
        <Field id="dest" label="Destination">
          <Input
            id="dest"
            value={city}
            onChange={(e) => setCity(e.target.value)}
            placeholder="Where to? e.g. Paros"
            className="h-11 border-0 bg-transparent px-0 shadow-none focus-visible:ring-0"
          />
        </Field>
        <Field id="in" label="Check-in">
          <Input
            id="in"
            type="date"
            value={checkIn}
            onChange={(e) => setCheckIn(e.target.value)}
            className="h-11 border-0 bg-transparent px-0 shadow-none focus-visible:ring-0"
          />
        </Field>
        <Field id="out" label="Check-out">
          <Input
            id="out"
            type="date"
            value={checkOut}
            min={checkIn || undefined}
            onChange={(e) => setCheckOut(e.target.value)}
            aria-invalid={invalidRange}
            className="h-11 border-0 bg-transparent px-0 shadow-none focus-visible:ring-0"
          />
        </Field>

        <Popover>
          <PopoverTrigger asChild>
            <button
              type="button"
              className="rounded-xl px-3 py-2 text-left transition-colors hover:bg-secondary md:min-w-32"
            >
              <span className="block text-xs font-semibold uppercase tracking-wide text-muted-foreground">
                Guests
              </span>
              <span className="block h-11 pt-2.5 text-sm">{guests} guests</span>
            </button>
          </PopoverTrigger>
          <PopoverContent className="w-64" align="start">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm font-medium">Guests</p>
                <p className="text-xs text-muted-foreground">Ages 13 or above</p>
              </div>
              <div className="flex items-center gap-2">
                <Button
                  type="button"
                  variant="outline"
                  size="icon"
                  className="size-9 rounded-full"
                  aria-label="Decrease guests"
                  disabled={guests <= 1}
                  onClick={() => setGuests((g) => Math.max(1, g - 1))}
                >
                  <Minus className="size-4" />
                </Button>
                <span className="w-6 text-center text-sm font-semibold">{guests}</span>
                <Button
                  type="button"
                  variant="outline"
                  size="icon"
                  className="size-9 rounded-full"
                  aria-label="Increase guests"
                  disabled={guests >= 16}
                  onClick={() => setGuests((g) => Math.min(16, g + 1))}
                >
                  <Plus className="size-4" />
                </Button>
              </div>
            </div>
          </PopoverContent>
        </Popover>

        <Button type="submit" size="lg" className="h-14 gap-2 rounded-xl px-6">
          <Search className="size-4" />
          Search
        </Button>
      </div>
      {invalidRange && (
        <p role="alert" className="px-3 pt-2 text-sm font-medium text-destructive">
          Check-out must be after check-in.
        </p>
      )}
    </form>
  );
}

function Field({
  id,
  label,
  children,
}: {
  id: string;
  label: string;
  children: React.ReactNode;
}) {
  return (
    <div className="rounded-xl px-3 py-2 transition-colors hover:bg-secondary">
      <Label htmlFor={id} className="text-xs font-semibold uppercase tracking-wide text-muted-foreground">
        {label}
      </Label>
      {children}
    </div>
  );
}