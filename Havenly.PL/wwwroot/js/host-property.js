(function () {
  const root = document.querySelector("[data-property-wizard]");
  if (!root) return;
  const steps = ["Basics", "Space", "Amenities", "Pricing"];
  let step = 0;
  const panels = Array.from(root.querySelectorAll("[data-step-panel]"));
  const pills = Array.from(root.querySelectorAll("[data-step-pill]"));
  const back = root.querySelector("[data-step-back]");
  const next = root.querySelector("[data-step-next]");
  const form = root.querySelector("form");

  function toast(msg) {
    if (window.havenlyToast) window.havenlyToast(msg, true);
    else alert(msg);
  }

  function show() {
    panels.forEach((p, i) => (p.hidden = i !== step));
    pills.forEach((p, i) => {
      p.className =
        "flex items-center gap-2 rounded-full border px-3 py-1.5 text-sm " +
        (i === step
          ? "border-primary bg-primary/10 font-semibold text-primary"
          : i < step
            ? "border-success/35 bg-success/10 text-success"
            : "border-border text-muted-foreground");
    });
    if (back) back.disabled = step === 0;
    if (next) next.innerHTML =
      step === steps.length - 1
        ? 'Submit for review <i data-lucide="arrow-right" class="size-4"></i>'
        : 'Continue <i data-lucide="arrow-right" class="size-4"></i>';
    if (window.lucide) window.lucide.createIcons();
  }

  function validate() {
    const title = form.querySelector("[name='Title']")?.value.trim();
    const city = form.querySelector("[name='City']")?.value.trim();
    const country = form.querySelector("[name='Country']")?.value.trim();
    const category = form.querySelector("[name='Category']")?.value;
    const description = form.querySelector("[name='Description']")?.value.trim() || "";
    const amenities = Array.from(form.querySelectorAll("[name='Amenities']")).filter((c) => c.checked);
    const price = Number(form.querySelector("[name='Price']")?.value || 0);
    if (step === 0 && (!title || !city || !country || !category)) {
      toast("Fill in the title, category and location.");
      return false;
    }
    if (step === 1 && description.length < 40) {
      toast("Descriptions need at least 40 characters.");
      return false;
    }
    if (step === 2 && amenities.length === 0) {
      toast("Pick at least one amenity.");
      return false;
    }
    if (step === 3 && price <= 0) {
      toast("Set a nightly price above zero.");
      return false;
    }
    return true;
  }

  back?.addEventListener("click", () => {
    step = Math.max(0, step - 1);
    show();
  });
  next?.addEventListener("click", () => {
    if (!validate()) return;
    if (step < steps.length - 1) {
      step += 1;
      show();
    } else {
      form.requestSubmit();
    }
  });

  const price = form.querySelector("[name='Price']");
  const cleaning = form.querySelector("[name='Cleaning']");
  const preview = root.querySelector("[data-price-preview]");
  function updatePreview() {
    if (!preview) return;
    const p = Number(price?.value || 0);
    const c = Number(cleaning?.value || 0);
    const guestPays = p * 5 + c;
    const keep = Math.round(guestPays * 0.97);
    preview.textContent =
      "$" + guestPays.toLocaleString("en-US") + " including cleaning · you keep $" + keep.toLocaleString("en-US") + " after fees.";
  }
  price?.addEventListener("input", updatePreview);
  cleaning?.addEventListener("input", updatePreview);
  updatePreview();
  show();
})();
