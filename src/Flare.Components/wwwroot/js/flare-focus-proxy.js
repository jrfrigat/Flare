// A composite control can survive while its focused child is replaced. Modal focus restoration
// then reaches the surviving wrapper. Route only that direct entry to the declared child; ordinary
// focus inside the control remains untouched. One listener serves the entire application.
document.addEventListener('focusin', e => {
    const root = e.target;
    const selector = root?.getAttribute?.('data-flare-focus-target');
    if (!selector) return;
    let target;
    try { target = root.querySelector(selector); } catch { return; }
    if (target && target !== root && !target.matches(':disabled') && !target.closest('[inert], [hidden]'))
        target.focus({ preventScroll: true });
});
