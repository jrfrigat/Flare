// Flare drag-and-drop transfer layer.
//
// The gesture is startDrag (flare-drag.js); this module is what rides on top of it: who is being
// dragged, which targets will take them, where the drop lands. It replaces native HTML5 drag-and-drop,
// which does not fire a single event on a touch screen - the reason reordering a data-grid column or a
// tree node was impossible on a phone.
//
// THE INTEROP BUDGET IS THREE CALLS PER DRAG, AND IT IS A CONSTANT. One at the start, to ask .NET
// which targets accept this item; one at the drop, to report where it landed; one at the end, which is
// the only one a cancelled drag makes. Everything in between - hit-testing, the preview that follows
// the pointer, the insertion line, the hover classes - happens here, because a call per pointermove is
// a network round trip per pointermove on Blazor Server. (The tree's HTML5 handler had to grow a "one
// measurement in flight at a time" coalescer to survive exactly that.)
//
// The whole hit test is elementFromPoint. It is scroll-safe by construction: no rect is cached, so a
// list that scrolls under the pointer mid-drag cannot desynchronise.

import { registry } from './flare-dom.js';
import { startDrag } from './flare-drag.js';

const ITEM = '[data-flare-drag]';
const ZONE = '[data-flare-drop]';
const CONTEXT = '.flare-drag-context';
const HIT = ':scope > [data-flare-drag-hit]';

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

// The box that decides where the pointer is inside an item, which is not always the item. An expanded
// tree node is as tall as its whole subtree, so measured against the li every point in a 300px branch
// lands in its top third; the node marks its own ROW with data-flare-drag-hit and that is measured
// instead. Scoped to direct children so a nested item's row is not mistaken for this one's.
function _hitBox(itemEl) {
    return itemEl.querySelector(HIT) ?? itemEl;
}

// Which part of the item the pointer is over. `both` splits it into thirds (a tree row: land before it,
// inside it, or after it); everything else splits it in half, because there is no "inside" to hit.
function _edgeOf(itemEl, placement, x, y, row) {
    const r = _hitBox(itemEl).getBoundingClientRect();
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
    const hitEl = _hitBox(itemEl);
    if (edge === 'into') return { zone, edge, index: -1, overEl: itemEl, hitEl, row };

    let index = items.indexOf(itemEl);
    if (index < 0) index = items.length;
    else if (edge === 'after') index += 1;
    return { zone, edge, index, overEl: itemEl, hitEl, row };
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
    const r = hit.hitEl.getBoundingClientRect();
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

// -- Auto-scroll at a container's edge ---------------------------------------
// A board that scrolls sideways keeps columns off-screen, and `elementFromPoint` outside the viewport
// returns null - so without this a card cannot be moved to a column you cannot see, which on a phone is
// most of them. It runs on a FRAME loop rather than on pointermove: a pointer held still at the edge
// stops firing moves, and that is exactly when the scrolling has to keep going.
const SCROLL_EDGE = 56;   // px band at a container's edge where scrolling starts
const SCROLL_MAX = 20;    // px per frame at the very edge, tapering to 0 at the band's inner rim

function _scrollsOn(el, axis) {
    const s = getComputedStyle(el);
    const overflow = axis === 'x' ? s.overflowX : s.overflowY;
    if (overflow !== 'auto' && overflow !== 'scroll') return false;
    return axis === 'x' ? el.scrollWidth > el.clientWidth + 1 : el.scrollHeight > el.clientHeight + 1;
}

function _scroller(from, axis) {
    for (let n = from; n && n !== document.body; n = n.parentElement)
        if (_scrollsOn(n, axis)) return n;
    return null;
}

function _velocity(pos, min, max) {
    if (pos < min + SCROLL_EDGE) return -Math.ceil(SCROLL_MAX * Math.min(1, (min + SCROLL_EDGE - pos) / SCROLL_EDGE));
    if (pos > max - SCROLL_EDGE) return Math.ceil(SCROLL_MAX * Math.min(1, (pos - (max - SCROLL_EDGE)) / SCROLL_EDGE));
    return 0;
}

// Returns whether anything actually moved, which is the signal to re-run the hit test: the pointer has
// not moved, but what is under it has.
function _autoScroll(x, y) {
    const cx = Math.max(0, Math.min(x, innerWidth - 1));
    const cy = Math.max(0, Math.min(y, innerHeight - 1));
    const under = document.elementFromPoint(cx, cy);
    if (!under) return false;
    let moved = false;

    const sx = _scroller(under, 'x');
    if (sx) {
        const b = sx.getBoundingClientRect();
        const v = _velocity(cx, b.left, b.right);
        if (v) { const was = sx.scrollLeft; sx.scrollLeft += v; moved = moved || sx.scrollLeft !== was; }
    }

    const sy = _scroller(under, 'y');
    if (sy) {
        const b = sy.getBoundingClientRect();
        const v = _velocity(cy, b.top, b.bottom);
        if (v) { const was = sy.scrollTop; sy.scrollTop += v; moved = moved || sy.scrollTop !== was; }
    }
    else {
        // The page itself is the outermost container: a long list scrolls the document, not a box.
        const v = _velocity(cy, 0, innerHeight);
        if (v) { const was = scrollY; scrollBy(0, v); moved = moved || scrollY !== was; }
    }

    return moved;
}

// Register a context root. Every draggable and every drop zone under it is found through the DOM, so
// adding a thousand rows costs nothing here - there is one gesture for the whole subtree, not one per
// item.
//   dotNetRef: OnDragStartAsync(sourceId) -> { allow?: string[], deny?: string[] } | null
//              (null = every zone in the group; allow is the .NET side's list and can only name zones
//               registered there, deny names zones it knows of but only this side can enumerate)
//              OnDropAsync(sourceId, targetId, index, edge, overId)
//              OnDragEndAsync()   - once at the end of every drag, dropped or not
export function registerDragContext(root, dotNetRef) {
    if (!root) return;
    let drag = null;

    // Space on a focused control is the control's, never the page's - so it never scrolls under a
    // draggable. The arrows stay the page's until an item is actually picked up, which the model says
    // by putting --picked on it; a card you have merely tabbed to must not trap the reader.
    function onRootKey(e) {
        const item = e.target instanceof Element ? e.target.closest(ITEM) : null;
        if (!item || item.closest(CONTEXT) !== root || item.dataset.flareDragDisabled === 'true') return;
        const space = e.key === ' ' || e.key === 'Spacebar';
        const held = item.classList.contains('flare-draggable--picked');
        if (space || (held && (e.key === 'Escape' || e.key.startsWith('Arrow')))) e.preventDefault();
    }
    root.addEventListener('keydown', onRootKey);

    function cleanup() {
        if (!drag) return;
        cancelAnimationFrame(drag.raf);
        drag.preview?.node.remove();
        drag.indicator?.remove();
        drag.hit?.hitEl?.classList.remove('flare-draggable--drop-into');
        drag.sourceEl.classList.remove('flare-draggable--dragging');
        for (const zoneEl of drag.zones.keys())
            zoneEl.classList.remove('flare-drop-zone--candidate', 'flare-drop-zone--over');
        document.removeEventListener('keydown', drag.onKey, true);
        drag = null;
    }

    // Everything the drag shows for a given pointer position. Called from pointermove AND from the
    // frame loop after an auto-scroll, where the pointer has not moved but the page under it has.
    function update() {
        if (!drag) return;
        const { x, y } = drag.pointer;
        const p = drag.preview;
        p.node.style.transform = `translate(${x - p.grabX}px, ${y - p.grabY}px)`;

        const hit = _hitTest(drag, x, y);
        if (drag.hit?.zone.el !== hit?.zone.el) {
            drag.hit?.zone.el.classList.remove('flare-drop-zone--over');
            hit?.zone.el.classList.add('flare-drop-zone--over');
        }
        // "Into" an item is the one state a zone highlight and an insertion line cannot express.
        if (drag.hit?.overEl !== hit?.overEl || drag.hit?.edge !== hit?.edge) {
            drag.hit?.hitEl?.classList.remove('flare-draggable--drop-into');
            if (hit?.edge === 'into' && hit.hitEl) hit.hitEl.classList.add('flare-draggable--drop-into');
        }
        drag.hit = hit;
        _placeIndicator(drag.indicator, hit);
    }

    function tick() {
        if (!drag) return;
        if (_autoScroll(drag.pointer.x, drag.pointer.y)) update();
        drag.raf = requestAnimationFrame(tick);
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

            // Escape abandons the drag: no drop is reported, only the end.
            const onKey = ev => {
                if (ev.key !== 'Escape') return;
                ev.preventDefault();
                cleanup();
                dotNetRef.invokeMethodAsync('OnDragEndAsync').catch(() => { });
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
                pointer: { x: e.clientX, y: e.clientY },
                raf: requestAnimationFrame(tick),
            };

            // The one call at the start. Until it answers, every zone in the group is a candidate; the
            // answer narrows them. A drag that is over before the answer lands simply used the wider
            // set, which is the same set HTML5 drag-and-drop would have offered.
            dotNetRef.invokeMethodAsync('OnDragStartAsync', drag.sourceId).then(ruling => {
                if (!drag || ruling == null) return;
                const allow = ruling.allow ? new Set(ruling.allow) : null;
                const deny = ruling.deny ? new Set(ruling.deny) : null;
                if (!allow && !deny) return;
                for (const [zoneEl, zone] of [...drag.zones]) {
                    if ((!allow || allow.has(zone.id)) && !(deny && deny.has(zone.id))) continue;
                    drag.zones.delete(zoneEl);
                    zoneEl.classList.remove('flare-drop-zone--candidate', 'flare-drop-zone--over');
                }
            }).catch(() => { });
        },
        onMove(dx, dy, e) {
            if (!drag) return;
            drag.pointer.x = e.clientX;
            drag.pointer.y = e.clientY;
            update();
        },
        onEnd() {
            if (!drag) return;
            const hit = drag.hit;
            const sourceId = drag.sourceId;
            cleanup();
            if (hit) {
                dotNetRef.invokeMethodAsync(
                    'OnDropAsync', sourceId, hit.zone.id, hit.index, hit.edge,
                    hit.overEl ? hit.overEl.dataset.flareDrag : null).catch(() => { });
            }
            dotNetRef.invokeMethodAsync('OnDragEndAsync').catch(() => { });
        },
    });

    _contexts.keep(root, () => { cleanup(); off(); root.removeEventListener('keydown', onRootKey); hideDropHint(root); });
}

export function removeDragContext(root) {
    _contexts.drop(root);
}

// -- Keyboard reorder ---------------------------------------------------------
// There is no pointer, so there is no gesture: .NET owns the whole interaction and asks for two things.
// This draws the same insertion line the pointer path draws, at a position named as (zone, index)
// rather than found under a cursor.
const _hints = new Map();   // context root -> indicator element

export function showDropHint(root, targetId, index, sourceId) {
    if (!root) return;
    const zoneEl = root.querySelector(`[data-flare-drop="${CSS.escape(targetId)}"]`);
    if (!zoneEl) return hideDropHint(root);

    const items = _ownItems(zoneEl).filter(el => el.dataset.flareDrag !== sourceId);
    const row = _isRow(zoneEl, items);
    const at = Math.max(0, Math.min(index, items.length));

    // Past the last item there is no item to sit beside, so the line goes after the last one.
    const anchorEl = at < items.length ? items[at] : items[items.length - 1];
    if (!anchorEl) return hideDropHint(root);

    let node = _hints.get(root);
    if (!node) { node = _makeIndicator(); _hints.set(root, node); }
    _placeIndicator(node, {
        edge: at < items.length ? 'before' : 'after',
        overEl: anchorEl,
        hitEl: _hitBox(anchorEl),
        row,
    });
    anchorEl.scrollIntoView({ block: 'nearest', inline: 'nearest' });
}

export function hideDropHint(root) {
    const node = _hints.get(root);
    if (!node) return;
    node.remove();
    _hints.delete(root);
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
