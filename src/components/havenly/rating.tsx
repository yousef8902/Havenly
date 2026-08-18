import { Star } from "lucide-react";

export function Rating({
  value,
  reviews,
  className = "",
}: {
  value: number;
  reviews?: number;
  className?: string;
}) {
  return (
    <span className={`inline-flex items-center gap-1 text-sm ${className}`}>
      <Star className="size-4 fill-accent text-accent" aria-hidden="true" />
      <span className="font-semibold">{value.toFixed(2)}</span>
      {reviews !== undefined && (
        <span className="text-muted-foreground">({reviews})</span>
      )}
      <span className="sr-only">out of 5</span>
    </span>
  );
}

export function Stars({ value, size = 16 }: { value: number; size?: number }) {
  return (
    <span className="inline-flex items-center gap-0.5" aria-label={`${value} out of 5 stars`}>
      {[1, 2, 3, 4, 5].map((i) => (
        <Star
          key={i}
          style={{ width: size, height: size }}
          className={i <= Math.round(value) ? "fill-accent text-accent" : "text-border"}
          aria-hidden="true"
        />
      ))}
    </span>
  );
}