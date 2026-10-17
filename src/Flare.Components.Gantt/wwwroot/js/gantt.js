// Flare Gantt: what the timeline cannot know from .NET alone - how wide it is on screen, the drag of the
// row-header column's edge, and the horizontal scroll position.

const MinSide = 120;

export function attach(root, dotnet, virtualize, endless) {
    const scroll = root.querySelector('.flare-gantt__scroll');
    const corner = root.querySelector('.flare-gantt__corner');
    let resizing = null;

    const side = () => corner ? corner.getBoundingClientRect().width : 0;
    let sent = [-1, -1];
    let pending = 0;
    // .NET hears only a settled width: a menu or window animation changes it every frame, and redrawing the axis is costly.
    const send = () => {
        pending = 0;
        if (resizing) return;
        const width = scroll.clientWidth, column = side();
        if (Math.abs(width - sent[0]) < 1 && Math.abs(column - sent[1]) < 1) return;
        sent = [width, column];
        dotnet.invokeMethodAsync('Viewport', width, column).catch(() => { });
    };
    const report = () => {
        clearTimeout(pending);
        pending = setTimeout(send, 150);
    };
    const observer = new ResizeObserver(report);
    observer.observe(scroll);
    // The first measurement may land on a page layout that is not finished yet; it is repeated once it is.
    send();
    const settle = setTimeout(send, 300);
    window.addEventListener('resize', report);

    // The column follows the pointer through the CSS variable alone; .NET hears only the final width.
    const down = e => {
        const handle = e.target.closest('.flare-gantt__side-handle');
        if (!handle || e.button !== 0) return;
        e.preventDefault();
        // Capture keeps the drag even when the pointer leaves the narrow handle; without it the root listeners suffice.
        try { handle.setPointerCapture(e.pointerId); } catch { }
        resizing = { handle, x: e.clientX, width: side(), current: side() };
    };
    const move = e => {
        if (!resizing) return;
        const max = Math.max(MinSide, scroll.clientWidth * 0.7);
        resizing.current = Math.round(Math.min(max, Math.max(MinSide, resizing.width + e.clientX - resizing.x)));
        root.style.setProperty('--_gantt-side', resizing.current + 'px');
    };
    const up = () => {
        if (!resizing) return;
        const width = resizing.current;
        resizing = null;
        dotnet.invokeMethodAsync('SideResized', width).catch(() => { });
        report();
    };
    // A row drop zone covers the whole row; the unit under the pointer is lit here and resolved by .NET from the
    // last pointer position, so a chart of hundreds of rows needs one zone per row, not one per row and unit.
    const scale = root.querySelector('.flare-gantt__scale');
    let pointer = null;
    let frame = 0;
    const unitAt = x => [...root.querySelectorAll('.flare-gantt__scale-row:not(.flare-gantt__scale-row--major) > .flare-gantt__tick')]
        .find(t => x >= t.offsetLeft && x < t.offsetLeft + t.offsetWidth);
    const light = () => {
        frame = 0;
        const zone = root.querySelector('.flare-gantt__drop.flare-drop-zone--over');
        const unit = zone?.firstElementChild;
        if (!unit || !pointer || !scale) return;
        const tick = unitAt(pointer.x - scale.getBoundingClientRect().left);
        if (!tick) return;
        unit.style.left = tick.offsetLeft + 'px';
        unit.style.width = tick.offsetWidth + 'px';
    };
    const track = e => {
        pointer = { x: e.clientX, y: e.clientY };
        if (!frame && root.querySelector('.flare-drop-zone--candidate')) frame = requestAnimationFrame(light);
        // A drag that scrolled the chart left the drawn rows behind; once it is over, report where it ended.
        if (e.type === 'pointerup' && virtualize) setTimeout(() => visible(), 120);
    };
    document.addEventListener('pointermove', track, { capture: true, passive: true });
    document.addEventListener('pointerup', track, { capture: true, passive: true });

    // Alt + wheel scrolls the axis sideways; Shift + wheel and touchpads already do.
    const wheel = e => {
        if (!e.altKey || e.ctrlKey) return;
        e.preventDefault();
        scroll.scrollLeft += e.deltaY || e.deltaX;
    };
    scroll.addEventListener('wheel', wheel, { passive: false });

    // Virtualized panels draw only the rows near what is on screen: report, per panel body, which part of it is
    // visible - inside the chart's own scroll box and the window. Not while a drag is on: the drag knows the drop
    // zones it started with, and rows drawn later would not be among them.
    let windowTimer = 0;
    let target = null, retry = 0;
    const visible = () => {
        windowTimer = 0;
        if (root.querySelector('.flare-drop-zone--candidate')) return;
        const box = scroll.getBoundingClientRect();
        const top = Math.max(box.top, 0), bottom = Math.min(box.bottom, innerHeight);
        const ids = [], from = [], to = [];
        for (const body of root.querySelectorAll('.flare-gantt__body[data-panel]')) {
            const r = body.getBoundingClientRect();
            ids.push(body.dataset.panel);
            from.push(top - r.top);
            to.push(bottom - r.top);
        }
        if (virtualize && ids.length) dotnet.invokeMethodAsync('Visible', ids, from, to).catch(() => { });
        // The visible part of the time area: columns far from it are not drawn, and an endless axis grows near its edges.
        dotnet.invokeMethodAsync('Horizontal', scroll.scrollLeft, Math.max(0, scroll.clientWidth - side())).catch(() => { });
    };
    const onScroll = () => { if (!windowTimer) windowTimer = setTimeout(visible, 60); };
    if (virtualize || endless) {
        scroll.addEventListener('scroll', onScroll, { passive: true });
        window.addEventListener('scroll', onScroll, { passive: true });
        window.addEventListener('resize', onScroll);
        visible();
    }

    root.addEventListener('pointerdown', down);
    root.addEventListener('pointermove', move);
    root.addEventListener('pointerup', up);
    root.addEventListener('pointercancel', up);

    return {
        // While the panels are still drawing, the axis is narrow and the browser clamps the scroll; it is repeated once the width is ready.
        // The retry aims at the same moment: a shift from growth on the left in between moves the target with it.
        scrollTo(x) {
            target = Math.max(0, x);
            scroll.scrollLeft = target;
            clearTimeout(retry);
            retry = setTimeout(() => {
                if (target !== null && Math.abs(scroll.scrollLeft - target) > 1) scroll.scrollLeft = target;
                target = null;
                if (virtualize || endless) visible();
            }, 300);
            if (virtualize || endless) visible();
        },
        // The axis grew on the left: keep the same moment under the eye, and tell .NET at once where the view is now.
        shift(px) {
            scroll.scrollLeft += px;
            if (target !== null) target += px;
            if (virtualize || endless) visible();
        },
        // Where the pointer was last seen, in pixels from the start of the time area; null before any movement.
        pointerX() { return pointer && scale ? pointer.x - scale.getBoundingClientRect().left : null; },
        dispose() {
            clearTimeout(windowTimer);
            clearTimeout(retry);
            scroll.removeEventListener('wheel', wheel);
            scroll.removeEventListener('scroll', onScroll);
            window.removeEventListener('scroll', onScroll);
            window.removeEventListener('resize', onScroll);
            document.removeEventListener('pointermove', track, { capture: true });
            document.removeEventListener('pointerup', track, { capture: true });
            cancelAnimationFrame(frame);
            observer.disconnect();
            clearTimeout(settle);
            clearTimeout(pending);
            window.removeEventListener('resize', report);
            root.removeEventListener('pointerdown', down);
            root.removeEventListener('pointermove', move);
            root.removeEventListener('pointerup', up);
            root.removeEventListener('pointercancel', up);
        }
    };
}
