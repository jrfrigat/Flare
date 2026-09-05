// Flare drag-and-drop transfer layer.
//
// The gesture is startDrag (flare-drag.js); this module is what rides on top of it: who is being
// dragged, which targets will take them, where the drop lands. It replaces native HTML5 drag-and-drop,
// which does not fire a single event on a touch screen - the reason reordering a data-grid column or a
// tree node was impossible on a phone.
//
// THE INTEROP BUDGET IS TWO CALLS PER DRAG. One at the start, to ask .NET which targets accept this
// item; one at the drop, to report where it landed. Everything in between - hit-testing, the preview
// that follows the pointer, the insertion line, the hover classes - happens here, because a call per
// pointermove is a network round trip per pointermove on Blazor Server. (The tree's HTML5 handler had
// to grow a "one measurement in flight at a time" coalescer to survive exactly that.)
//
// The whole hit test is elementFromPoint. It is scroll-safe by construction: no rect is cached, so a
// list that scrolls under the pointer mid-drag cannot desynchronise.

import { registry } from './flare-dom.js';
import { startDrag } from './flare-drag.js';

const ITEM = '[data-flare-drag]';
const ZONE = '[data-flare-drop]';
const CONTEXT = '.flare-drag-context';

const _contexts = registry();

// Which way a zone lays its items out, so "before/after" means the right edge. MEASURED, not declared:
// where the first two items actually sit answers for every layout there is, including the ones a
// declaration would get wrong - a row of table headers is `display: table-row`, which says nothing
// about direction, and a grid's rows are `table-row-group`. Falling back to the computed style covers
// a zone holding fewer than two items, where there is no arrangement to read.
function _isRow(zoneEl, items) {
    if (items && items.length >= 2) {
        const a = items[0].getBoundingClientRect();
        const b = items[1].getBoundingClientRect();
        if (b.top >= a.bottom - 1) return false;
        if (b.left >= a.right - 1) return true;
    }
    const s = getComputedStyle(zoneEl);
    if (s.display === 'flex' || s.display === 'inline-flex')
        return s.flexDirection === 'row' || s.flexDirection === 'row-reverse';
    if (s.display === 'grid' || s.display === 'inline-grid')
        return s.gridAutoFlow.startsWith('column');
    return false;
}

// The items a zone owns directly. A nested zone (a tree branch inside a tree branch) keeps its own.
function _ownItems(zoneEl) {
    return [...zoneEl.querySelectorAll(ITEM)].filter(el => el.closest(ZONE) === zoneEl);
}

// Which part of the item the pointer is over. `both` splits it into thirds (a tree row: land before it,
// inside it, or after it); everything else splits it in half, because there is no "inside" to hit.
function _edgeOf(itemEl, placement, x, y, row) {
    const r = itemEl.getBoundingClientRect();
    const size = row ? r.width : r.height;
    if (size <= 0) return 'into';
    const offset = row ? x - r.left : y - r.top;
    if (placement === 'both') {
        if (offset < size / 3) return 'before';
        if (offset > (size * 2) / 3) return 'after';
        return 'into';
    }
    return offset < size / 2 ? 'before' : 'after';
}

function _hitTest(drag, x, y) {
    const el = document.elementFromPoint(x, y);
    if (!el) return null;

    const zoneEl = el.closest(ZONE);
    if (!zoneEl || !drag.zones.has(zoneEl)) return null;

    const zone = drag.zones.get(zoneEl);
    const placement = zone.placement;
    const itemEl = el.closest(ITEM);

    // Over the item being dragged: there is nowhere for it to go, so nothing is indicated and a drop
    // here is a no-op rather than a move to a position it already occupies.
    if (itemEl === drag.sourceEl) return null;

    // The index is reported in the list WITHOUT the dragged item, so it is the index the item ends up
    // at - which is what a reorder callback wants, and what an index counted in the current DOM (where
    // the source is still sitting somewhere earlier) would not be.
    const items = _ownItems(zoneEl).filter(i => i !== drag.sourceEl);

    // Over the zone but not over any item: an ordered zone appends, a container zone just takes it.
    if (!itemEl || !zoneEl.contains(itemEl)) {
        if (placement === 'into') return { zone, edge: 'into', index: -1, overEl: null };
        return { zone, edge: 'into', index: items.length, overEl: null };
    }

    const row = _isRow(zoneEl, items);
    const edge = placement === 'into' ? 'into' : _edgeOf(itemEl, placement, x, y, row);
    if (edge === 'into') return { zone, edge, index: -1, overEl: itemEl, row };

    let index = items.indexOf(itemEl);
    if (index < 0) index = items.length;
    else if (edge === 'after') index += 1;
    return { zone, edge, index, overEl: itemEl, row };
}

function _makePreview(sourceEl) {
    const r = sourceEl.getBoundingClientRect();
    const node = sourceEl.cloneNode(true);
    node.removeAttribute('id');
    node.classList.add('flare-drag-preview');
    node.classList.remove('flare-draggable--dragging');
    node.style.width = r.width + 'px';
    node.style.height = r.height + 'px';
    document.body.appendChild(node);
    return { node, grabX: 0, grabY: 0, left: r.left, top: r.top };
}

function _makeIndicator() {
    const node = document.createElement('div');
    node.className = 'flare-drag-indicator';
    node.hidden = true;
    document.body.appendChild(node);
    return node;
}

function _placeIndicator(node, hit) {
    if (!hit || hit.edge === 'into' || !hit.overEl) { node.hidden = true; return; }
    const r = hit.overEl.getBoundingClientRect();
    node.classList.toggle('flare-drag-indicator--horizontal', !hit.row);
    if (hit.row) {
        node.style.left = (hit.edge === 'before' ? r.left : r.right) + 'px';
        node.style.top = r.top + 'px';
        node.style.width = '';
        node.style.height = r.height + 'px';
    } else {
        node.style.left = r.left + 'px';
        node.style.top = (hit.edge === 'before' ? r.top : r.bottom) + 'px';
        node.style.width = r.width + 'px';
        node.style.height = '';
    }
    node.hidden = false;
}

// Register a context root. Every draggable and every drop zone under it is found through the DOM, so
// adding a thousand rows costs nothing here - there is one gesture for the whole subtree, not one per
// item.
//   dotNetRef: OnDragStartAsync(sourceId) -> string[] | null   (null = every zone in the group)
//              OnDropAsync(sourceId, targetId, index, edge, overId)
export function registerDragContext(root, dotNetRef) {
    if (!root) return;
    let drag = null;

    function cleanup() {
        if (!drag) return;
        drag.preview?.node.remove();
        drag.indicator?.remove();
        drag.sourceEl.classList.remove('flare-draggable--dragging');
        for (const zoneEl of drag.zones.keys())
            zoneEl.classList.remove('flare-drop-zone--candidate', 'flare-drop-zone--over');
        document.removeEventListener('keydown', drag.onKey, true);
        drag = null;
    }

    const off = startDrag(root, {
        threshold: 4,
        touchAction: null,   // .flare-draggable declares its own; the context may be a scroll container
        // Contexts nest - a data grid declares one for its rows and another for its columns - and the
        // gesture of an outer context sees the presses of an inner one bubble through it. The INNERMOST
        // context owns the item, so exactly one of them starts a drag.
        filter(e) {
            const item = e.target instanceof Element ? e.target.closest(ITEM) : null;
            if (!item || item.dataset.flareDragDisabled === 'true') return false;
            return item.closest(CONTEXT) === root;
        },
        onStart(e) {
            const sourceEl = e.target.closest(ITEM);
            if (!sourceEl) return;

            // A drag only sees the zones of its own group, and the group is the ITEM's - so one context
            // can hold a board and a tree without a card ever becoming a node.
            const group = sourceEl.dataset.flareDragGroup || '';
            const zones = new Map();
            for (const zoneEl of root.querySelectorAll(ZONE)) {
                if ((zoneEl.dataset.flareDragGroup || '') !== group) continue;
                if (zoneEl.closest(CONTEXT) !== root) continue;   // belongs to a nested context
                zones.set(zoneEl, {
                    el: zoneEl,
                    id: zoneEl.dataset.flareDrop,
                    placement: zoneEl.dataset.flareDropPlacement || 'into',
                });
                zoneEl.classList.add('flare-drop-zone--candidate');
            }

            const r = sourceEl.getBoundingClientRect();
            const preview = _makePreview(sourceEl);
            preview.grabX = e.clientX - r.left;
            preview.grabY = e.clientY - r.top;
            preview.node.style.transform = `translate(${r.left}px, ${r.top}px)`;

            // Escape abandons the drag. Nothing has to be unwound on the .NET side: it holds no state
            // between the two calls, so a drag that never reaches OnDropAsync simply never happened.
            const onKey = ev => {
                if (ev.key !== 'Escape') return;
                ev.preventDefault();
                cleanup();
            };

            sourceEl.classList.add('flare-draggable--dragging');
            document.addEventListener('keydown', onKey, true);

            drag = {
                sourceEl,
                sourceId: sourceEl.dataset.flareDrag,
                zones,
                preview,
                indicator: _makeIndicator(),
                hit: null,
                onKey,
            };

            // The one call at the start. Until it answers, every zone in the group is a candidate; the
            // answer narrows them. A drag that is over before the answer lands simply used the wider
            // set, which is the same set HTML5 drag-and-drop would have offered.
            dotNetRef.invokeMethodAsync('OnDragStartAsync', drag.sourceId).then(allowed => {
                if (!drag || allowed == null) return;
                const keep = new Set(allowed);
                for (const [zoneEl, zone] of [...drag.zones]) {
                    if (keep.has(zone.id)) continue;
                    drag.zones.delete(zoneEl);
                    zoneEl.classList.remove('flare-drop-zone--candidate', 'flare-drop-zone--over');
                }
            }).catch(() => { });
        },
        onMove(dx, dy, e) {
            if (!drag) return;
            const p = drag.preview;
            p.node.style.transform =
                `translate(${e.clientX - p.grabX}px, ${e.clientY - p.grabY}px)`;

            const hit = _hitTest(drag, e.clientX, e.clientY);
            if (drag.hit?.zone.el !== hit?.zone.el) {
                drag.hit?.zone.el.classList.remove('flare-drop-zone--over');
                hit?.zone.el.classList.add('flare-drop-zone--over');
            }
            drag.hit = hit;
            _placeIndicator(drag.indicator, hit);
        },
        onEnd() {
            if (!drag) return;
            const hit = drag.hit;
            const sourceId = drag.sourceId;
            cleanup();
            if (!hit) return;   // dropped on nothing, which is a cancel
            dotNetRef.invokeMethodAsync(
                'OnDropAsync', sourceId, hit.zone.id, hit.index, hit.edge,
                hit.overEl ? hit.overEl.dataset.flareDrag : null).catch(() => { });
        },
    });

    _contexts.keep(root, () => { cleanup(); off(); });
}

export function removeDragContext(root) {
    _contexts.drop(root);
}

// The draggable ids a group holds, in the order the DOM has them, grouped by zone. The keyboard
// reorder path asks for this once when an item is picked up: registration order in C# is not render
// order after a list has been reordered, and the DOM is the only thing that knows.
export function dragItemOrder(root, group) {
    if (!root) return [];
    const out = [];
    for (const zoneEl of root.querySelectorAll(ZONE)) {
        if ((zoneEl.dataset.flareDragGroup || '') !== (group || '')) continue;
        out.push({
            target: zoneEl.dataset.flareDrop,
            items: _ownItems(zoneEl).map(el => el.dataset.flareDrag),
        });
    }
    return out;
}
