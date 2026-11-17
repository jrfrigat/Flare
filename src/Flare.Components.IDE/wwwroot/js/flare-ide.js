export function registerShortcutCapture(root) {
    const keydown = event => {
        if (event.isComposing) {
            event.stopPropagation();
        } else if (event.key !== 'Tab') {
            event.preventDefault();
        }
    };
    root.addEventListener('keydown', keydown);
    return { dispose: () => root.removeEventListener('keydown', keydown) };
}

// A Blazor drop event cannot read DataTransfer.getData(), so the document key is carried in module
// state as well: dragstart records it, the capture-phase drop listener hands it to the layout before
// Blazor's own drop handler runs, and takeDroppedDocument clears it after one read.
const documentType = 'application/x-flare-ide-document';
let dragSourceRegistered = false;
let draggedKey = null;
let droppedKey = null;

export function registerDocumentDragSource() {
    if (dragSourceRegistered) return;
    dragSourceRegistered = true;

    document.addEventListener('dragstart', event => {
        const target = event.target;
        const source = target instanceof Element ? target.closest('[data-flare-doc-key]') : null;
        const key = source ? source.dataset.flareDocKey : null;
        draggedKey = key ?? null;
        droppedKey = null;
        if (!key || !event.dataTransfer) return;

        try {
            event.dataTransfer.effectAllowed = 'move';
            event.dataTransfer.setData(documentType, key);
            event.dataTransfer.setData('text/plain', key);
        } catch {
            // A synthetic or protected drag can reject setData; module state still carries the key.
        }
    }, true);

    document.addEventListener('drop', () => {
        if (draggedKey !== null) droppedKey = draggedKey;
        draggedKey = null;
    }, true);

    document.addEventListener('dragend', () => {
        draggedKey = null;
    }, true);
}

export function takeDroppedDocument() {
    const key = droppedKey;
    droppedKey = null;
    return key;
}

// Auto-hide tool flyout dismissal. The registry is module-level and keyed by layout and side; the
// document listeners exist only while at least one flyout is open, so a page with no open flyout
// pays nothing. A press inside the floating body or its rail stays, and a Flare overlay layer (a
// menu or dialog backdrop, a dialog, an open popover) owns its own press and Escape.
const flyouts = new Map();
let flyoutPointerDown = null;
let flyoutKeyDown = null;

const containsTarget = (element, target) =>
    element instanceof Node && target instanceof Node && element.contains(target);

const anyFlyoutContains = target => {
    for (const regions of flyouts.values())
        for (const entry of regions.values())
            if (containsTarget(entry.body, target) || containsTarget(entry.rail, target)) return true;
    return false;
};

const layerSelector = '[data-flare-backdrop], [role="dialog"]';
const isOverlayTarget = target => target instanceof Element &&
    target.closest(`${layerSelector}, [popover]`) !== null;
const hasOpenLayer = () => document.querySelector(`${layerSelector}, [popover]:popover-open`) !== null;

const onFlyoutPointerDown = event => {
    if (flyouts.size === 0) return;
    const target = event.target;
    if (anyFlyoutContains(target) || isOverlayTarget(target)) return;
    for (const regions of flyouts.values())
        for (const [position, entry] of regions)
            entry.dotNetRef.invokeMethodAsync('OnFlyoutOutsidePressAsync', position).catch(() => { });
};

const onFlyoutKeyDown = event => {
    if (event.key !== 'Escape' || event.isComposing || flyouts.size === 0) return;
    if (hasOpenLayer()) return;
    const active = document.activeElement;
    let chosen = null;
    for (const regions of flyouts.values()) {
        for (const [position, entry] of regions) {
            if (containsTarget(entry.body, active) || containsTarget(entry.rail, active)) {
                chosen = { position, entry };
                break;
            }
        }
        if (chosen) break;
    }
    if (!chosen) {
        const first = flyouts.values().next().value;
        const firstEntry = first ? first.entries().next().value : null;
        if (firstEntry) chosen = { position: firstEntry[0], entry: firstEntry[1] };
    }
    if (chosen)
        chosen.entry.dotNetRef.invokeMethodAsync('OnFlyoutEscapeAsync', chosen.position).catch(() => { });
};

const listenForFlyouts = () => {
    if (flyoutPointerDown) return;
    flyoutPointerDown = onFlyoutPointerDown;
    flyoutKeyDown = onFlyoutKeyDown;
    document.addEventListener('pointerdown', flyoutPointerDown, true);
    document.addEventListener('keydown', flyoutKeyDown, true);
};

const unlistenForFlyouts = () => {
    if (!flyoutPointerDown) return;
    document.removeEventListener('pointerdown', flyoutPointerDown, true);
    document.removeEventListener('keydown', flyoutKeyDown, true);
    flyoutPointerDown = null;
    flyoutKeyDown = null;
};

const syncFlyoutListeners = () =>
    flyouts.size === 0 ? unlistenForFlyouts() : listenForFlyouts();

export function setFlyout(layoutId, dotNetRef, position, body, rail) {
    let regions = flyouts.get(layoutId);
    if (!regions) {
        regions = new Map();
        flyouts.set(layoutId, regions);
    }
    regions.set(position, { body, rail, dotNetRef });
    listenForFlyouts();
}

export function clearFlyout(layoutId, position) {
    const regions = flyouts.get(layoutId);
    if (!regions) return;
    regions.delete(position);
    if (regions.size === 0) flyouts.delete(layoutId);
    syncFlyoutListeners();
}

export function disposeFlyoutDismissal(layoutId) {
    if (!flyouts.delete(layoutId)) return;
    syncFlyoutListeners();
}

// Tool panel tab fitting. The strip never scrolls: tabs shrink and their labels ellipsize in CSS.
// The one state CSS cannot decide is whether a tab has become too narrow for text at all - that
// depends on the strip width and the tab count, not on any single element's content. When the
// per-tab share falls below this width, a tab that has an icon shows the icon alone; the label is
// clipped by CSS, not removed, so the accessible name survives.
// The thresholds are expressed in the tab's own font size, so a theme or a user text scale that
// enlarges the tab text moves them with it. At the default 12px body-small tab these are 72px and
// 96px per tab.
const compactEm = 6;
// Leaving the icon-only state needs a wider tab than entering it: clipping the labels changes the
// strip's max-content width, so a group inside a shrink-to-fit host could otherwise flip states
// forever. A layout region has a definite width and never reaches this branch.
const expandEm = 8;

const compactAttribute = 'data-flare-tab-compact';
const iconAttribute = 'data-flare-tab-icon';

export function registerTabFitting(bar) {
    if (!bar) return null;

    let compact = null;
    const apply = () => {
        const tabs = Array.from(bar.children).filter(child => child.matches('[role="tab"]'));
        if (tabs.length === 0 || bar.clientWidth === 0) return;
        const fontSize = parseFloat(getComputedStyle(tabs[0]).fontSize) || 12;
        const share = bar.clientWidth / tabs.length;
        compact = compact === true ? share > fontSize * expandEm : share < fontSize * compactEm;
        for (const tab of tabs)
            tab.toggleAttribute(compactAttribute, compact && tab.hasAttribute(iconAttribute));
    };

    // Both observers coalesce their own bursts, so the decision runs once per delivered batch;
    // no animation frame is involved and a page that never paints still gets the state right.
    const resizes = new ResizeObserver(apply);
    resizes.observe(bar);
    const mutations = new MutationObserver(apply);
    mutations.observe(bar, { childList: true });
    apply();

    return {
        dispose: () => {
            resizes.disconnect();
            mutations.disconnect();
        },
    };
}
