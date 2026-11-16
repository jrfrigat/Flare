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
