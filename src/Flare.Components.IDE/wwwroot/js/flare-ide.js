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
