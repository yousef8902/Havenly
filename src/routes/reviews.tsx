import { createFileRoute } from "@tanstack/react-router";
import { useState } from "react";
import { MessageSquareQuote, Send, Star } from "lucide-react";
import { toast } from "sonner";
import { GuestShell } from "@/components/havenly/shells";
import { Stars } from "@/components/havenly/rating";
import { Button } from "@/components/ui/button";
import { Textarea } from "@/components/ui/textarea";
import { Label } from "@/components/ui/label";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { useAppState } from "@/lib/app-state";
import { formatDate, getProperty, reviews as seedReviews } from "@/lib/data";

export const Route = createFileRoute("/reviews")({
  head: () => ({
    meta: [
      { title: "Reviews — Havenly" },
      { name: "description", content: "Rate your completed Havenly stays and read what other guests said about the homes you visited." },
      { property: "og:title", content: "Reviews — Havenly" },
      { property: "og:description", content: "Rate your completed stays and read guest reviews on Havenly." },
      { property: "og:type", content: "website" },
      { name: "twitter:card", content: "summary_large_image" },
    ],
  }),
  component: ReviewsPage,
});

function ReviewsPage() {
  const { bookings } = useAppState();
  const completed = bookings.filter((b) => b.status === "completed");
  const [bookingId, setBookingId] = useState(completed[0]?.id ?? "");
  const [rating, setRating] = useState(5);
  const [hover, setHover] = useState(0);
  const [body, setBody] = useState("");
  const [written, setWritten] = useState<string[]>([]);

  const submit = (e: React.FormEvent) => {
    e.preventDefault();
    if (!bookingId) {
      toast.error("Choose a stay to review.");
      return;
    }
    if (body.trim().length < 20) {
      toast.error("Please write at least 20 characters.");
      return;
    }
    setWritten((w) => [...w, bookingId]);
    setBody("");
    toast.success("Review published — thank you!");
  };

  const pending = completed.filter((b) => !written.includes(b.id));

  return (
    <GuestShell>
      <div className="container-page py-12">
        <h1 className="text-3xl md:text-4xl">Reviews</h1>
        <p className="mt-1 text-muted-foreground">
          Share how your stay went. Reviews go live 24 hours after you submit them.
        </p>

        <div className="mt-8 grid gap-8 lg:grid-cols-[1fr_1.1fr]">
          <form onSubmit={submit} className="h-fit rounded-2xl border border-border bg-card p-6">
            <h2 className="text-xl">Write a review</h2>
            {pending.length === 0 ? (
              <p className="mt-4 text-sm text-muted-foreground">
                You're all caught up — no completed stays are waiting for a review.
              </p>
            ) : (
              <div className="mt-5 space-y-5">
                <div className="space-y-2">
                  <Label htmlFor="stay">Stay</Label>
                  <Select value={bookingId} onValueChange={setBookingId}>
                    <SelectTrigger id="stay" className="h-11">
                      <SelectValue placeholder="Select a completed stay" />
                    </SelectTrigger>
                    <SelectContent>
                      {pending.map((b) => (
                        <SelectItem key={b.id} value={b.id}>
                          {getProperty(b.propertyId)?.title ?? b.propertyId} · {formatDate(b.checkIn)}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                </div>

                <div className="space-y-2">
                  <Label>Rating</Label>
                  <div className="flex items-center gap-1" onMouseLeave={() => setHover(0)}>
                    {[1, 2, 3, 4, 5].map((i) => (
                      <button
                        key={i}
                        type="button"
                        aria-label={`${i} star${i > 1 ? "s" : ""}`}
                        onMouseEnter={() => setHover(i)}
                        onClick={() => setRating(i)}
                        className="p-1"
                      >
                        <Star
                          className={
                            i <= (hover || rating)
                              ? "size-7 fill-accent text-accent"
                              : "size-7 text-border"
                          }
                        />
                      </button>
                    ))}
                    <span className="ml-2 text-sm text-muted-foreground">{rating}.0</span>
                  </div>
                </div>

                <div className="space-y-2">
                  <Label htmlFor="body">Your review</Label>
                  <Textarea
                    id="body"
                    rows={5}
                    value={body}
                    onChange={(e) => setBody(e.target.value)}
                    placeholder="What stood out — the space, the host, the neighbourhood?"
                  />
                  <p className="text-xs text-muted-foreground">{body.length} characters (20 minimum)</p>
                </div>

                <Button type="submit" size="lg" className="w-full">
                  <Send className="size-4" /> Publish review
                </Button>
              </div>
            )}
          </form>

          <section>
            <h2 className="text-xl">Recent guest reviews</h2>
            <ul className="mt-5 space-y-4">
              {seedReviews.map((r) => (
                <li key={r.id} className="rounded-2xl border border-border bg-card p-5">
                  <div className="flex flex-wrap items-center justify-between gap-2">
                    <p className="font-semibold">{r.author}</p>
                    <span className="text-xs text-muted-foreground">{r.date}</span>
                  </div>
                  <p className="text-sm text-muted-foreground">
                    {getProperty(r.propertyId)?.title}
                  </p>
                  <div className="mt-2">
                    <Stars value={r.rating} />
                  </div>
                  <p className="mt-3 text-sm leading-relaxed">{r.body}</p>
                  {r.hostResponse && (
                    <div className="mt-4 flex gap-3 rounded-xl bg-secondary p-4">
                      <MessageSquareQuote className="size-4 shrink-0 text-primary" />
                      <p className="text-sm text-muted-foreground">{r.hostResponse}</p>
                    </div>
                  )}
                </li>
              ))}
            </ul>
          </section>
        </div>
      </div>
    </GuestShell>
  );
}
