(function () {
  const $ = (sel, root = document) => root.querySelector(sel);
  const $$ = (sel, root = document) => Array.from(root.querySelectorAll(sel));

  function toast(message, isError) {
    let host = $("#toast-host");
    if (!host) {
      host = document.createElement("div");
      host.id = "toast-host";
      host.className = "fixed bottom-20 right-4 z-50 flex flex-col gap-2 md:bottom-6";
      document.body.appendChild(host);
    }
    const el = document.createElement("div");
    el.className =
      "rounded-xl border border-border bg-card px-4 py-3 text-sm shadow-lift " +
      (isError ? "text-destructive" : "text-foreground");
    el.textContent = message;
    host.appendChild(el);
    setTimeout(() => el.remove(), 3200);
  }
  window.havenlyToast = toast;

  $$("[data-dropdown]").forEach((root) => {
    const trigger = $("[data-dropdown-trigger]", root);
    const menu = $("[data-dropdown-menu]", root);
    if (!trigger || !menu) return;
    trigger.addEventListener("click", (e) => {
      e.stopPropagation();
      const open = menu.hasAttribute("hidden") === false;
      $$("[data-dropdown-menu]").forEach((m) => m.setAttribute("hidden", ""));
      if (!open) menu.removeAttribute("hidden");
    });
  });
  document.addEventListener("click", () => {
    $$("[data-dropdown-menu]").forEach((m) => m.setAttribute("hidden", ""));
  });

  $$("[data-sheet-open]").forEach((btn) => {
    btn.addEventListener("click", () => {
      const id = btn.getAttribute("data-sheet-open");
      const sheet = document.getElementById(id);
      if (sheet) {
        sheet.removeAttribute("hidden");
        sheet.classList.add("is-open");
        document.body.style.overflow = "hidden";
      }
    });
  });
  $$("[data-sheet-close]").forEach((btn) => {
    btn.addEventListener("click", () => {
      const sheet = btn.closest("[data-sheet]");
      if (sheet) {
        sheet.setAttribute("hidden", "");
        sheet.classList.remove("is-open");
        document.body.style.overflow = "";
      }
    });
  });

  $$("[data-tabs]").forEach((root) => {
    const triggers = $$("[data-tab]", root);
    const panels = $$("[data-tab-panel]", root);
    function activate(key) {
      triggers.forEach((t) => {
        const on = t.getAttribute("data-tab") === key;
        t.setAttribute("data-state", on ? "active" : "inactive");
      });
      panels.forEach((p) => {
        p.hidden = p.getAttribute("data-tab-panel") !== key;
      });
    }
    triggers.forEach((t) => t.addEventListener("click", () => activate(t.getAttribute("data-tab"))));
    const initial = root.getAttribute("data-tabs-default") || triggers[0]?.getAttribute("data-tab");
    if (initial) activate(initial);
  });

  $$("[data-popover]").forEach((root) => {
    const trigger = $("[data-popover-trigger]", root);
    const panel = $("[data-popover-panel]", root);
    if (!trigger || !panel) return;
    trigger.addEventListener("click", (e) => {
      e.stopPropagation();
      panel.toggleAttribute("hidden");
    });
    document.addEventListener("click", () => panel.setAttribute("hidden", ""));
  });

  $$("[data-guest-stepper]").forEach((root) => {
    const input = $("input[name='Guests']", root) || $("input[name='Filters.Guests']", root);
    const label = $("[data-guest-label]", root);
    const minus = $("[data-guest-minus]", root);
    const plus = $("[data-guest-plus]", root);
    function render() {
      const n = Number(input?.value || 2);
      if (label) label.textContent = n + " guests";
      const count = $("[data-guest-count]", root);
      if (count) count.textContent = String(n);
      if (minus) minus.disabled = n <= 1;
      if (plus) plus.disabled = n >= 16;
    }
    minus?.addEventListener("click", () => {
      input.value = Math.max(1, Number(input.value) - 1);
      render();
    });
    plus?.addEventListener("click", () => {
      input.value = Math.min(16, Number(input.value) + 1);
      render();
    });
    render();
  });

  $$("form[data-search-form]").forEach((form) => {
    form.addEventListener("submit", (e) => {
      const cin = form.querySelector("[name='CheckIn']")?.value;
      const cout = form.querySelector("[name='CheckOut']")?.value;
      const alert = form.querySelector("[data-date-alert]");
      if (cin && cout && new Date(cout) <= new Date(cin)) {
        e.preventDefault();
        if (alert) alert.hidden = false;
      }
    });
  });

  $$("[data-dialog-open]").forEach((btn) => {
    btn.addEventListener("click", () => {
      const dlg = document.getElementById(btn.getAttribute("data-dialog-open"));
      dlg?.removeAttribute("hidden");
    });
  });
  $$("[data-dialog-close]").forEach((btn) => {
    btn.addEventListener("click", () => {
      btn.closest("[data-dialog]")?.setAttribute("hidden", "");
    });
  });

  if (window.lucide) window.lucide.createIcons();
})();
