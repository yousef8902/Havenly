(function () {
  const form = document.querySelector("[data-review-form]");
  if (!form) return;
  const stars = Array.from(form.querySelectorAll("[data-star]"));
  const input = form.querySelector("[name='Rating']");
  const label = form.querySelector("[data-rating-label]");
  let rating = Number(input?.value || 5);
  let hover = 0;

  function paint() {
    const v = hover || rating;
    stars.forEach((btn) => {
      const i = Number(btn.getAttribute("data-star"));
      const icon = btn.querySelector("i, svg");
      if (!icon) return;
      icon.setAttribute("class", i <= v ? "size-7 fill-accent text-accent" : "size-7 text-border");
    });
    if (label) label.textContent = rating.toFixed(1);
    if (window.lucide) window.lucide.createIcons();
  }

  stars.forEach((btn) => {
    btn.addEventListener("mouseenter", () => {
      hover = Number(btn.getAttribute("data-star"));
      paint();
    });
    btn.addEventListener("click", () => {
      rating = Number(btn.getAttribute("data-star"));
      if (input) input.value = rating;
      paint();
    });
  });
  form.querySelector("[data-stars]")?.addEventListener("mouseleave", () => {
    hover = 0;
    paint();
  });

  const body = form.querySelector("[name='Body']");
  const count = form.querySelector("[data-char-count]");
  body?.addEventListener("input", () => {
    if (count) count.textContent = (body.value.length || 0) + " characters (20 minimum)";
  });
  paint();
})();
