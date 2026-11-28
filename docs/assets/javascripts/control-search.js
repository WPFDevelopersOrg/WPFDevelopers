(() => {
  const input = document.getElementById("wd-control-filter");
  const status = document.getElementById("wd-control-search-status");
  const results = document.getElementById("wd-control-search-results");
  const heading = document.getElementById("control-catalog");
  const searchBox = input && input.closest(".wd-control-search");
  const header = document.querySelector(".md-header__inner");
  const siteSearchButton = header && header.querySelector('label.md-header__button[for="__search"]');
  const isEnglishPage = /(?:^|\/)en(?:\/|$)/.test(window.location.pathname);

  const createIcon = (viewBox, data) => {
    const svg = document.createElementNS("http://www.w3.org/2000/svg", "svg");
    svg.setAttribute("viewBox", viewBox);
    svg.setAttribute("aria-hidden", "true");
    svg.setAttribute("focusable", "false");
    const path = document.createElementNS("http://www.w3.org/2000/svg", "path");
    path.setAttribute("d", data);
    svg.append(path);
    return svg;
  };

  for (const icon of document.querySelectorAll('.md-nav--primary .md-nav__link[for^="__nav_"] > .md-nav__icon')) {
    icon.append(createIcon("0 0 24 24", "M8 4 16 12 8 20"));
  }

  const backToTop = document.createElement("button");
  backToTop.type = "button";
  backToTop.className = "wd-back-to-top";
  backToTop.append(createIcon("170 0 684 684", "M854,346 512,0 170,346 232,404 470,166 470,684 554,684 554,166 794,404z"));
  backToTop.setAttribute("aria-label", isEnglishPage ? "Back to top" : "返回顶部");
  backToTop.title = isEnglishPage ? "Back to top" : "返回顶部";
  backToTop.hidden = true;
  backToTop.addEventListener("click", () => window.scrollTo({ top: 0, behavior: "smooth" }));
  document.body.append(backToTop);
  const updateBackToTop = () => { backToTop.hidden = window.scrollY <= 0; };
  window.addEventListener("scroll", updateBackToTop, { passive: true });
  updateBackToTop();

  const resources = document.querySelector("[data-wd-resources]");
  if (resources) {
    const toggle = resources.querySelector(".wd-resources__toggle");
    const panel = resources.querySelector(".wd-resources__panel");
    const closeButton = resources.querySelector(".wd-resources__close");
    const closePanel = restoreFocus => {
      panel.hidden = true;
      toggle.setAttribute("aria-expanded", "false");
      if (restoreFocus) toggle.focus();
    };

    toggle.addEventListener("click", () => {
      if (!panel.hidden) {
        closePanel(false);
        return;
      }
      panel.hidden = false;
      toggle.setAttribute("aria-expanded", "true");
      closeButton.focus();
    });
    closeButton.addEventListener("click", () => closePanel(true));
    document.addEventListener("click", event => {
      if (!panel.hidden && !resources.contains(event.target)) closePanel(false);
    });
    document.addEventListener("keydown", event => {
      if (event.key === "Escape" && !panel.hidden) {
        event.preventDefault();
        closePanel(true);
      }
    });
  }

  if (siteSearchButton) {
    const label = isEnglishPage ? "Search" : "搜索";
    siteSearchButton.dataset.label = label;
    siteSearchButton.setAttribute("aria-label", isEnglishPage ? "Search the site" : "全站搜索");
    siteSearchButton.title = isEnglishPage ? "Search the site" : "全站搜索";
  }

  if (window.location.protocol === "file:") {
    const searchOutput = document.querySelector(".md-search__output");
    if (searchOutput) {
      const warning = document.createElement("p");
      warning.className = "wd-search-file-warning";
      warning.setAttribute("role", "status");
      warning.textContent = isEnglishPage
        ? "Site search requires an HTTP server. From the repository root, run: python -m http.server 8768 --directory site --bind 127.0.0.1, then open http://127.0.0.1:8768/."
        : "全站搜索需要通过 HTTP 访问。请在仓库根目录运行 python -m http.server 8768 --directory site --bind 127.0.0.1，然后打开 http://127.0.0.1:8768/。";
      searchOutput.prepend(warning);
    }
  }

  if (!input || !status || !results || !heading || !searchBox) return;

  const headerAnchor = header && (
    header.querySelector('[data-md-component="palette"]') ||
    header.querySelector('label[for="__search"]')
  );
  if (header && headerAnchor) {
    header.insertBefore(searchBox, headerAnchor);
    document.body.classList.add("wd-control-catalog-page");
  }

  const nodes = [];
  for (let node = heading.nextElementSibling; node && node.tagName !== "H2"; node = node.nextElementSibling) {
    nodes.push(node);
  }

  const categories = [];
  let category = null;
  let index = 0;

  for (const node of nodes) {
    if (node.tagName === "H3") {
      category = {
        name: node.textContent.replace(/\s*¶\s*$/, "").trim(),
        elements: [node],
        entries: []
      };
      categories.push(category);
      continue;
    }

    if (!category) continue;

    category.elements.push(node);
    if (node.tagName !== "TABLE") continue;

    for (const row of node.querySelectorAll("tbody tr")) {
      row.id = `wd-control-entry-${index++}`;
      const cells = Array.from(row.cells, cell => cell.textContent.replace(/\s+/g, " ").trim());
      category.entries.push({
        row,
        name: cells[0] || "",
        description: cells[cells.length - 1] || "",
        searchText: `${category.name} ${cells.join(" ")}`.normalize("NFKC").toLocaleLowerCase()
      });
    }
  }

  const entries = categories.flatMap(item => item.entries);
  if (entries.length === 0) return;

  const isEnglish = searchBox.dataset.language === "en";
  const messages = isEnglish
    ? {
        empty: count => `${count} controls in the catalog. Enter a control name, category, or purpose to search.`,
        noResults: "No matches found. Try a control name or a feature keyword.",
        found: count => `${count} match${count === 1 ? "" : "es"} found. Select a result to jump to the catalog.`,
        foundLimited: (count, shown) => `${count} matches found. Showing the first ${shown}.`
      }
    : {
        empty: count => `目录共 ${count} 项。输入控件名称、分类或用途开始搜索。`,
        noResults: "没有找到匹配项，请尝试控件名称或功能关键词。",
        found: count => `找到 ${count} 项，点击结果定位到目录。`,
        foundLimited: (count, shown) => `找到 ${count} 项，显示前 ${shown} 项。`
      };

  const renderResults = query => {
    const matches = query
      ? entries.filter(entry => entry.searchText.includes(query))
      : [];
    const matchingEntries = new Set(matches);

    for (const item of categories) {
      const visible = !query || item.entries.some(entry => matchingEntries.has(entry));
      for (const element of item.elements) {
        element.style.display = visible ? "" : "none";
      }

      for (const entry of item.entries) {
        entry.row.style.display = !query || matchingEntries.has(entry) ? "" : "none";
      }
    }

    results.replaceChildren();

    if (!query) {
      status.textContent = messages.empty(entries.length);
      return;
    }

    if (matches.length === 0) {
      status.textContent = messages.noResults;
      return;
    }

    const visibleMatches = matches.slice(0, 12);
    const fragment = document.createDocumentFragment();
    for (const entry of visibleMatches) {
      const item = document.createElement("li");
      const link = document.createElement("a");
      link.href = `#${entry.row.id}`;
      const categoryName = categories.find(candidate => candidate.entries.includes(entry)).name;
      link.textContent = `${entry.name} — ${categoryName}${isEnglish ? ": " : "："}${entry.description}`;
      link.addEventListener("click", event => {
        event.preventDefault();
        window.location.hash = entry.row.id;
        searchBox.classList.remove("is-open");
        input.blur();
        requestAnimationFrame(() => entry.row.scrollIntoView({ behavior: "smooth", block: "center" }));
      });
      item.append(link);
      fragment.append(item);
    }
    results.append(fragment);

    status.textContent = matches.length > visibleMatches.length
      ? messages.foundLimited(matches.length, visibleMatches.length)
      : messages.found(matches.length);
  };

  input.addEventListener("input", () => {
    searchBox.classList.add("is-open");
    renderResults(input.value.normalize("NFKC").trim().toLocaleLowerCase());
  });

  input.addEventListener("focus", () => searchBox.classList.add("is-open"));

  input.addEventListener("blur", () => {
    window.setTimeout(() => {
      if (!searchBox.contains(document.activeElement)) searchBox.classList.remove("is-open");
    });
  });

  input.addEventListener("keydown", event => {
    if (event.key === "Enter") {
      results.querySelector("a")?.click();
    }
  });

  renderResults("");
})();
