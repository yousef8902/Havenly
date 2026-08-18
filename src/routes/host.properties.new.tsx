import { createFileRoute, useNavigate } from "@tanstack/react-router";
import { useState } from "react";
import { ArrowLeft, ArrowRight, Check } from "lucide-react";
import { toast } from "sonner";
import { PageHeading, WorkspaceShell } from "@/components/havenly/shells";
import { hostNav } from "@/components/havenly/workspace-nav";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";
import { Checkbox } from "@/components/ui/checkbox";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { amenityList, categories, formatMoney } from "@/lib/data";

export const Route = createFileRoute("/host/properties/new")({
  head: () => ({
    meta: [
      { title: "Add a property — Havenly Host" },
      { name: "description", content: "List a new home on Havenly in four guided steps: basics, details, amenities and pricing." },
      { property: "og:title", content: "Add a property — Havenly Host" },
      { property: "og:description", content: "List a new home on Havenly in four guided steps." },
      { property: "og:type", content: "website" },
      { name: "twitter:card", content: "summary_large_image" },
    ],
  }),
  component: NewProperty,
});

const steps = ["Basics", "Space", "Amenities", "Pricing"];

function NewProperty() {
  const navigate = useNavigate();
  const [step, setStep] = useState(0);
  const [form, setForm] = useState({
    title: "",
    category: "",
    city: "",
    country: "",
    description: "",
    guests: 4,
    bedrooms: 2,
    beds: 2,
    baths: 1,
    amenities: [] as string[],
    price: 180,
    cleaning: 60,
  });

  const set = <K extends keyof typeof form>(k: K, v: (typeof form)[K]) =>
    setForm((f) => ({ ...f, [k]: v }));

  const validate = () => {
    if (step === 0) {
      if (!form.title.trim() || !form.city.trim() || !form.country.trim() || !form.category) {
        toast.error("Fill in the title, category and location.");
        return false;
      }
    }
    if (step === 1 && form.description.trim().length < 40) {
      toast.error("Descriptions need at least 40 characters.");
      return false;
    }
    if (step === 2 && form.amenities.length === 0) {
      toast.error("Pick at least one amenity.");
      return false;
    }
    if (step === 3 && form.price <= 0) {
      toast.error("Set a nightly price above zero.");
      return false;
    }
    return true;
  };

  const next = () => {
    if (!validate()) return;
    if (step < steps.length - 1) return setStep(step + 1);
    toast.success("Listing submitted for review — you'll hear back within 48 hours.");
    navigate({ to: "/host" });
  };

  return (
    <WorkspaceShell items={hostNav} workspace="Host workspace" role="Elena Marinos · Superhost">
      <PageHeading title="Add a property" description="Four short steps. You can edit everything later." />

      <ol className="mb-8 flex flex-wrap gap-2">
        {steps.map((s, i) => (
          <li
            key={s}
            className={`flex items-center gap-2 rounded-full border px-3 py-1.5 text-sm ${
              i === step
                ? "border-primary bg-primary/10 font-semibold text-primary"
                : i < step
                  ? "border-success/35 bg-success/10 text-success"
                  : "border-border text-muted-foreground"
            }`}
          >
            {i < step ? <Check className="size-4" /> : <span className="text-xs">{i + 1}</span>}
            {s}
          </li>
        ))}
      </ol>

      <div className="max-w-2xl rounded-2xl border border-border bg-card p-6">
        {step === 0 && (
          <div className="space-y-5">
            <div className="space-y-2">
              <Label htmlFor="title">Listing title</Label>
              <Input id="title" value={form.title} onChange={(e) => set("title", e.target.value)} placeholder="Sunlit loft with harbour views" />
            </div>
            <div className="space-y-2">
              <Label htmlFor="category">Category</Label>
              <Select value={form.category} onValueChange={(v) => set("category", v)}>
                <SelectTrigger id="category" className="h-11"><SelectValue placeholder="Choose a category" /></SelectTrigger>
                <SelectContent>
                  {categories.map((c) => <SelectItem key={c} value={c}>{c}</SelectItem>)}
                </SelectContent>
              </Select>
            </div>
            <div className="grid gap-4 sm:grid-cols-2">
              <div className="space-y-2">
                <Label htmlFor="city">City</Label>
                <Input id="city" value={form.city} onChange={(e) => set("city", e.target.value)} />
              </div>
              <div className="space-y-2">
                <Label htmlFor="country">Country</Label>
                <Input id="country" value={form.country} onChange={(e) => set("country", e.target.value)} />
              </div>
            </div>
          </div>
        )}

        {step === 1 && (
          <div className="space-y-5">
            <div className="space-y-2">
              <Label htmlFor="description">Description</Label>
              <Textarea id="description" rows={6} value={form.description} onChange={(e) => set("description", e.target.value)} placeholder="Describe the space, the light, the neighbourhood…" />
              <p className="text-xs text-muted-foreground">{form.description.length} characters (40 minimum)</p>
            </div>
            <div className="grid grid-cols-2 gap-4 sm:grid-cols-4">
              {(["guests", "bedrooms", "beds", "baths"] as const).map((k) => (
                <div key={k} className="space-y-2">
                  <Label htmlFor={k} className="capitalize">{k}</Label>
                  <Input id={k} type="number" min={1} value={form[k]} onChange={(e) => set(k, Number(e.target.value))} />
                </div>
              ))}
            </div>
          </div>
        )}

        {step === 2 && (
          <fieldset className="space-y-4">
            <legend className="text-sm font-medium">What does the place offer?</legend>
            <div className="grid gap-3 sm:grid-cols-2">
              {amenityList.map((a) => (
                <label key={a} className="flex min-h-11 items-center gap-3 rounded-xl border border-border px-3 text-sm">
                  <Checkbox
                    checked={form.amenities.includes(a)}
                    onCheckedChange={(c) =>
                      set("amenities", c ? [...form.amenities, a] : form.amenities.filter((x) => x !== a))
                    }
                  />
                  {a}
                </label>
              ))}
            </div>
          </fieldset>
        )}

        {step === 3 && (
          <div className="space-y-5">
            <div className="grid gap-4 sm:grid-cols-2">
              <div className="space-y-2">
                <Label htmlFor="price">Nightly price (USD)</Label>
                <Input id="price" type="number" min={1} value={form.price} onChange={(e) => set("price", Number(e.target.value))} />
              </div>
              <div className="space-y-2">
                <Label htmlFor="cleaning">Cleaning fee (USD)</Label>
                <Input id="cleaning" type="number" min={0} value={form.cleaning} onChange={(e) => set("cleaning", Number(e.target.value))} />
              </div>
            </div>
            <div className="rounded-xl bg-secondary p-4 text-sm">
              <p className="font-medium">Guest pays for 5 nights</p>
              <p className="mt-1 text-muted-foreground">
                {formatMoney(form.price * 5 + form.cleaning)} including cleaning · you keep{" "}
                {formatMoney(Math.round((form.price * 5 + form.cleaning) * 0.97))} after fees.
              </p>
            </div>
          </div>
        )}

        <div className="mt-8 flex justify-between gap-3">
          <Button variant="outline" onClick={() => setStep((s) => Math.max(0, s - 1))} disabled={step === 0}>
            <ArrowLeft className="size-4" /> Back
          </Button>
          <Button onClick={next}>
            {step === steps.length - 1 ? "Submit for review" : "Continue"} <ArrowRight className="size-4" />
          </Button>
        </div>
      </div>
    </WorkspaceShell>
  );
}
