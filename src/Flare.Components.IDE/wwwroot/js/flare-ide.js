// Flare.Components.IDE JS interop helpers

// Drag-resize a docked region (Left/Right/Bottom) of FlareIdeLayout. Computes the new region size
// in pixels from the pointer position relative to the layout, clamps it, and reports it to .NET.
export function startPanelResize(dotNetRef, layoutEl, position, minPx) {
    if (!layoutEl) return;
    const onMove = (e) => {
        const rect = layoutEl.getBoundingClientRect();
        let size;
        if (position === 'Left') {
            size = e.clientX - rect.left;
        } else if (position === 'Right') {
            size = rect.right - e.clientX;
        } else { // Bottom: measure to the bottom region's own lower edge (excludes the status bar)
            const bottomEl = layoutEl.querySelector('.flare-ide-layout__bottom');
            const bottomEdge = bottomEl ? bottomEl.getBoundingClientRect().bottom : rect.bottom;
            size = bottomEdge - e.clientY;
        }
        const axis = position === 'Bottom' ? rect.height : rect.width;
        const max = axis * 0.85;
        size = Math.max(minPx || 0, Math.min(size, max));
        dotNetRef.invokeMethodAsync('ResizePanel', position, Math.round(size));
    };

    const onUp = () => {
        document.removeEventListener('mousemove', onMove);
        document.removeEventListener('mouseup', onUp);
        document.body.style.cursor = '';
        document.body.style.userSelect = '';
    };

    document.body.style.cursor = position === 'Bottom' ? 'row-resize' : 'col-resize';
    document.body.style.userSelect = 'none';
    document.addEventListener('mousemove', onMove);
    document.addEventListener('mouseup', onUp);
}

