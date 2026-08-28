(function () {
  const form = document.querySelector("[data-booking-form]");
  if (!form) return;

  const price = Number(form.getAttribute("data-price") || 0);
  const booked = (form.getAttribute("data-booked") || "").split(",").filter(Boolean);
  const maxGuests = Number(form.getAttribute("data-max-guests") || 16);
  const cin = form.querySelector("#ci");
  const cout = form.querySelector("#co");
  const guests = form.querySelector("#gs");
  const submit = form.querySelector("[type=submit]");
  const live = form.querySelector("[data-booking-live]");
  const breakdown = form.querySelector("[data-booking-breakdown]");

  function nightsBetween(a, b) {
    if (!a || !b) return 0;
    const diff = (new Date(b) - new Date(a)) / 86400000;
    return diff > 0 ? Math.round(diff) : 0;
  }

  function notice(tone, text) {
    const styles = {
      ok: "border-success/35 bg-success/10 text-success",
      error: "border-destructive/35 bg-destructive/10 text-destructive",
      muted: "border-border bg-secondary text-muted-foreground",
    };
    return `<p class="rounded-xl border px-3 py-2 text-sm font-medium ${styles[tone]}" ${tone === "error" ? 'role="alert"' : ""}>${text}</p>`;
  }

  function money(n) {
    return "$" + Math.round(n).toLocaleString("en-US");
  }

  function refresh() {
    const nights = nightsBetween(cin.value, cout.value);
    const invalidRange = Boolean(cin.value && cout.value && nights <= 0);
    const rangeDates = [];
    for (let i = 0; i < nights; i++) {
      rangeDates.push(new Date(new Date(cin.value).getTime() + i * 86400000).toISOString().slice(0, 10));
    }
    const conflict = rangeDates.some((d) => booked.includes(d));
    const overCapacity = Number(guests.value) > maxGuests;
    const canBook = nights > 0 && !conflict && !overCapacity && !invalidRange;

    if (invalidRange) live.innerHTML = notice("error", "Check-out must be at least one night after check-in.");
    else if (conflict) live.innerHTML = notice("error", "Those nights are already booked. Try shifting your dates by a few days.");
    else if (nights > 0) live.innerHTML = notice("ok", nights + (nights === 1 ? " night" : " nights") + " available for these dates.");
    else live.innerHTML = notice("muted", "Select dates to check availability.");

    if (nights > 0 && !conflict) {
      const subtotal = nights * price;
      const fee = Math.round(subtotal * 0.09);
      breakdown.hidden = false;
      breakdown.querySelector("[data-line-nights]").textContent = money(price) + " × " + nights + " nights";
      breakdown.querySelector("[data-line-sub]").textContent = money(subtotal);
      breakdown.querySelector("[data-line-fee]").textContent = money(fee);
      breakdown.querySelector("[data-line-total]").textContent = money(subtotal + fee);
    } else {
      breakdown.hidden = true;
    }

    submit.disabled = !canBook;
    if (cout) cout.min = cin.value || "";
  }

  [cin, cout, guests].forEach((el) => el?.addEventListener("change", refresh));
  [cin, cout].forEach((el) => el?.addEventListener("input", refresh));
  refresh();
})();
