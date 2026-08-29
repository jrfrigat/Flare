// Flare.Components JS interop bundle.
// Consolidates the small per-component helpers (OTP, clipboard, infinite-scroll,
// data-grid resize, file download) into one module.
//
// These were `window.*` globals until 0.23.0, which made this the one file a host application had to
// add to its index.html by hand. Forgetting it was silent: the components compiled, rendered, and then
// threw a JSException the first time one of them reached for JS - a frozen DataGrid column, a lazy
// block, a table of contents, an OTP field, a download. Now the services import it like every other
// Flare module, so it loads on first use and cannot be left out. The exported names are unchanged, so
// the dotted call strings in the services still resolve.

import { all, listen, registry, scrollParent } from './flare-dom.js';

// -- OTP input ---------------------------------------------------------------
export const flareOtp = {
    focus: function (el) {
        if (el) { el.focus(); el.select(); }
    },
    getClipboardText: async function () {
        try { return await navigator.clipboard.readText(); } catch { return ''; }
    }
};

// -- Field imperative helpers (select/blur/caret-range) ----------------------
// Focus is done natively from C# via ElementReference.FocusAsync; only the
// operations with no built-in Blazor equivalent live here.
export const flareField = {
    select: function (el) { if (el && el.select) el.select(); },
    blur: function (el) { if (el && el.blur) el.blur(); },
    selectRange: function (el, start, end) {
        if (el && el.setSelectionRange) {
            try { el.focus(); el.setSelectionRange(start, end); } catch (_) { }
        }
    }
};

// -- Clipboard write fallback (no eval/innerHTML - GS-5 compliant) ------------
export const FlareClipboardFallback = {
    copy: function (text) {
        var ta = document.createElement('textarea');
        ta.value = text;
        ta.style.position = 'fixed';
        ta.style.opacity = '0';
        document.body.appendChild(ta);
        ta.focus();
        ta.select();
        try { document.execCommand('copy'); } catch (_) { }
        document.body.removeChild(ta);
    }
};

// -- Infinite scroll (IntersectionObserver) ----------------------------------
export const FlareInfiniteScroll = (() => {
    const observers = registry();
    return {
        init(sentinel, dotNetRef, rootMargin) {
            if (!sentinel) return;
            const obs = new IntersectionObserver(entries => {
                if (entries[0].isIntersecting) {
                    dotNetRef.invokeMethodAsync('TriggerLoad').catch(() => { });
                }
            }, { threshold: 0.1, rootMargin: rootMargin || '0px' });
            obs.observe(sentinel);
            observers.keep(sentinel, () => obs.disconnect());
        },
        dispose(sentinel) {
            observers.drop(sentinel);
        }
    };
})();

// -- Lazy render (defer a subtree until it scrolls near the viewport) --------
export const FlareLazy = (() => {
    const observers = registry();
    return {
        init(el, dotNetRef, rootMargin, once, rootSelector) {
            if (!el) return;
            // A rootSelector watches the nearest matching scroll ancestor instead of the viewport,
            // so deferral also works for content inside an overflow:auto panel.
            const root = rootSelector ? el.closest(rootSelector) : null;
            const obs = new IntersectionObserver(entries => {
                const visible = entries[entries.length - 1].isIntersecting;
                dotNetRef.invokeMethodAsync('OnVisibilityChanged', visible).catch(() => { });
                // In "once" mode we only ever reveal, so stop observing after the first hit.
                if (visible && once) observers.drop(el);
            }, { root, rootMargin: rootMargin || '0px' });
            obs.observe(el);
            observers.keep(el, () => obs.disconnect());
        },
        dispose(el) {
            observers.drop(el);
        }
    };
})();

// -- DataGrid column resize --------------------------------------------------
export const FlareDataGrid = {
    initResize(thEl) {
        let startX, startW;
        const handle = thEl.querySelector('.flare-datagrid__resize-handle');
        if (!handle || handle.dataset.flareResize) return; // idempotent: attach once
        handle.dataset.flareResize = '1';
        handle.addEventListener('mousedown', (e) => {
            startX = e.clientX;
            startW = thEl.offsetWidth;
            e.preventDefault();
            const table = thEl.closest('table');
            const onMove = (mv) => {
                thEl.style.width = Math.max(40, startW + mv.clientX - startX) + 'px';
                FlareDataGrid.updateFrozenOffsets(table); // keep sticky offsets correct while resizing
            };
            const onUp = () => { document.removeEventListener('mousemove', onMove); document.removeEventListener('mouseup', onUp); };
            document.addEventListener('mousemove', onMove);
            document.addEventListener('mouseup', onUp);
        });
    },
    initAllResizeHandles(tableEl) {
        tableEl?.querySelectorAll('th').forEach(th => FlareDataGrid.initResize(th));
    },
    // Cumulative sticky offsets for left-frozen columns: each frozen cell's `left` is
    // the summed width of the frozen cells before it in its row (so multiple frozen
    // columns stack correctly instead of all pinning at 0).
    updateFrozenOffsets(tableEl) {
        if (!tableEl) return;
        tableEl.querySelectorAll('tr').forEach(row => {
            let left = 0;
            row.querySelectorAll('.flare-datagrid__th--frozen, .flare-datagrid__td--frozen').forEach(cell => {
                cell.style.left = left + 'px';
                left += cell.getBoundingClientRect().width;
            });
            // Right-frozen columns stack from the right edge inward (process right-to-left).
            let right = 0;
            const rightCells = row.querySelectorAll('.flare-datagrid__th--frozen-right, .flare-datagrid__td--frozen-right');
            for (let i = rightCells.length - 1; i >= 0; i--) {
                rightCells[i].style.right = right + 'px';
                right += rightCells[i].getBoundingClientRect().width;
            }
        });
    },
    // Infinite scroll: observe a bottom sentinel within an optional scroll-root (the grid
    // wrapper) so it fires on inner scrolling, not just page scroll.
    _infObs: registry(),
    initInfinite(sentinel, root, dotNetRef, rootMargin) {
        if (!sentinel) return;
        const obs = new IntersectionObserver(entries => {
            if (entries[0].isIntersecting) dotNetRef.invokeMethodAsync('TriggerLoad').catch(() => { });
        }, { root: root || null, threshold: 0, rootMargin: rootMargin || '160px' });
        obs.observe(sentinel);
        FlareDataGrid._infObs.keep(sentinel, () => obs.disconnect());
    },
    disposeInfinite(sentinel) {
        FlareDataGrid._infObs.drop(sentinel);
    }
};

// -- On-this-page table of contents ------------------------------------------
export const FlareToc = (() => {
    const instances = registry();

    // Unicode-aware slug: keep letters/numbers (incl. cyrillic), collapse the rest to '-'.
    function slugify(text) {
        const s = (text || '')
            .toLowerCase()
            .trim()
            .replace(/[^\p{L}\p{N}\s-]/gu, '')
            .replace(/[\s-]+/g, '-')
            .replace(/^-+|-+$/g, '');
        return s || 'section';
    }

    // Visible text of a heading, excluding any FlareText "#" deep-link appended to it.
    function headingText(el) {
        const clone = el.cloneNode(true);
        clone.querySelectorAll('.flare-text__anchor').forEach(a => a.remove());
        return (clone.textContent || '').trim();
    }

    function collect(root, headingSelector) {
        const used = new Set();
        return Array.from(root.querySelectorAll(headingSelector)).map(el => {
            const text = headingText(el);
            // Add an anchor (id) when one is missing, de-duplicating against ids already in use.
            if (!el.id) {
                const base = slugify(text);
                let id = base, n = 2;
                while (used.has(id) || document.getElementById(id)) { id = `${base}-${n++}`; }
                el.id = id;
            }
            used.add(el.id);
            return { el, id: el.id, text, level: parseInt(el.tagName.substring(1), 10) || 2 };
        });
    }

    return {
        init(handle, dotNetRef, rootSelector, headingSelector, scrollRootSelector) {
            const root = (rootSelector && document.querySelector(rootSelector)) || document.body;
            const sel = headingSelector || 'h2, h3';
            const items = collect(root, sel);
            dotNetRef.invokeMethodAsync('SetHeadings', items.map(i => ({ id: i.id, text: i.text, level: i.level }))).catch(() => { });
            if (items.length === 0) return;

            // Scroll container: explicit selector wins, else auto-detect, else the page (window).
            const scroller = (scrollRootSelector && document.querySelector(scrollRootSelector)) || scrollParent(items[0].el);
            const target = scroller || window;

            let last = [], ticking = false;
            function update() {
                ticking = false;
                // Active = every heading currently within the scroll container's visible area, so all
                // anchors on screen right now are highlighted at once.
                const box = scroller ? scroller.getBoundingClientRect() : null;
                const top = box ? box.top : 0;
                const bottom = box ? box.bottom : window.innerHeight;
                const visible = items
                    .filter(i => { const r = i.el.getBoundingClientRect(); return r.bottom > top && r.top < bottom; })
                    .map(i => i.id);
                if (visible.length !== last.length || visible.some((v, k) => v !== last[k])) {
                    last = visible;
                    dotNetRef.invokeMethodAsync('SetActive', visible).catch(() => { });
                }
            }
            function onScroll() { if (!ticking) { ticking = true; requestAnimationFrame(update); } }

            instances.keep(handle, all(
                listen(target, 'scroll', onScroll, { passive: true }),
                listen(window, 'resize', onScroll, { passive: true }),
            ));
            update();
        },
        dispose(handle) {
            instances.drop(handle);
        }
    };
})();

// -- File download (CSV = same path with a UTF-8 BOM) -------------------------
export const FlareDownload = {
    download(filename, content, mimeType, withBom) {
        const parts = withBom ? ['﻿' + content] : [content];
        const blob = new Blob(parts, { type: (mimeType || 'application/octet-stream') + ';charset=utf-8;' });
        FlareDownload._save(blob, filename);
    },
    // Binary download from a base64 payload (e.g. .xlsx produced server/C#-side).
    downloadBase64(filename, base64, mimeType) {
        const bin = atob(base64);
        const bytes = new Uint8Array(bin.length);
        for (let i = 0; i < bin.length; i++) bytes[i] = bin.charCodeAt(i);
        FlareDownload._save(new Blob([bytes], { type: mimeType || 'application/octet-stream' }), filename);
    },
    _save(blob, filename) {
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = filename;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        URL.revokeObjectURL(url);
    }
};

// -- Element bounds (for fixed-position popups, e.g. FlareDateTimePicker) -----
// Lives in the component bundle so it works without any host glue (a host CSP
// that forbids inline scripts must not break positioning).
export const flareGetBounds = function (el) {
    if (!el) return { top: 0, bottom: 0, left: 0, width: 240, viewportHeight: 600, viewportWidth: 1200 };
    const r = el.getBoundingClientRect();
    return { top: r.top, bottom: r.bottom, left: r.left, width: r.width, viewportHeight: window.innerHeight, viewportWidth: window.innerWidth };
};
