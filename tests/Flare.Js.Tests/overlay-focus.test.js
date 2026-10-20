import { afterEach, expect, it, vi } from 'vitest';
import {
    trapFocus, releaseFocusTrap, registerDialogEscHandler, removeDialogEscHandler,
    pushPopupLayer, removePopupLayer, restoreFocusOutside,
} from '../../src/Flare.Components/wwwroot/js/flare-overlay.js';

const ids = [];
function visible(el) {
    el.getClientRects = () => [{}];
    return el;
}
function dialog(id, parent = document.body) {
    const el = visible(document.createElement('div'));
    el.tabIndex = -1;
    const first = visible(document.createElement('input'));
    const last = visible(document.createElement('button'));
    el.append(first, last);
    parent.append(el);
    ids.push(id);
    return { el, first, last };
}
function key(name, shiftKey = false) {
    const e = new KeyboardEvent('keydown', { key: name, shiftKey, bubbles: true, cancelable: true });
    document.activeElement.dispatchEvent(e);
    return e;
}
afterEach(() => {
    for (const id of ids.reverse()) { releaseFocusTrap(id); removeDialogEscHandler(id); }
    ids.length = 0;
    for (const id of ['popup-1', 'popup-2', 'select', 'menu']) removePopupLayer(id);
    document.body.replaceChildren();
});

it('cycles both Tab boundaries and restores the initiator', () => {
    const trigger = visible(document.createElement('button'));
    document.body.append(trigger); trigger.focus();
    const d = dialog('prompt'); trapFocus('prompt', d.el);
    expect(document.activeElement).toBe(d.first);
    expect(key('Tab', true).defaultPrevented).toBe(true);
    expect(document.activeElement).toBe(d.last);
    key('Tab'); expect(document.activeElement).toBe(d.first);
    releaseFocusTrap('prompt'); expect(document.activeElement).toBe(trigger);
    trigger.focus(); expect(document.activeElement).toBe(trigger);
});

it('focuses an empty panel and keeps both Tab directions inside it', () => {
    const trigger = visible(document.createElement('button'));
    document.body.append(trigger); trigger.focus();
    const d = dialog('empty'); d.first.remove(); d.last.remove();
    trapFocus('empty', d.el);
    expect(document.activeElement).toBe(d.el);
    expect(key('Tab').defaultPrevented).toBe(true);
    expect(key('Tab', true).defaultPrevented).toBe(true);
    trigger.focus(); expect(document.activeElement).toBe(d.el);
    releaseFocusTrap('empty'); expect(document.activeElement).toBe(trigger);
});

it('recovers panel focus on Tab after every focused control is removed', () => {
    const d = dialog('removed-controls'); trapFocus('removed-controls', d.el);
    d.first.remove(); d.last.remove();
    expect(document.activeElement).toBe(document.body);
    expect(key('Tab').defaultPrevented).toBe(true);
    expect(document.activeElement).toBe(d.el);
});

it('returns escaped focus to the dialog and handles an alert with one button', () => {
    const trigger = visible(document.createElement('button'));
    document.body.append(trigger);
    const d = dialog('alert'); d.first.remove(); trapFocus('alert', d.el);
    trigger.focus(); expect(document.activeElement).toBe(d.last);
    key('Tab'); expect(document.activeElement).toBe(d.last);
    key('Tab', true); expect(document.activeElement).toBe(d.last);
});

it('restores a surviving ancestor when the initiator is removed', () => {
    const region = visible(document.createElement('main'));
    const trigger = visible(document.createElement('button'));
    region.append(trigger); document.body.append(region); trigger.focus();
    const d = dialog('removed'); trapFocus('removed', d.el); trigger.remove();
    releaseFocusTrap('removed');
    expect(document.activeElement).toBe(region);
    // The temporary tabindex stays while the fallback holds focus: a browser would otherwise move focus to the body.
    expect(region.getAttribute('tabindex')).toBe('-1');
    region.blur();
    expect(region.hasAttribute('tabindex')).toBe(false);
});

it('uses the fallback when the initiator becomes disabled', () => {
    const region = visible(document.createElement('main'));
    region.tabIndex = 0;
    const trigger = visible(document.createElement('button'));
    region.append(trigger); document.body.append(region); trigger.focus();
    const d = dialog('disabled'); trapFocus('disabled', d.el); trigger.disabled = true;
    releaseFocusTrap('disabled');
    expect(document.activeElement).toBe(region);
    region.blur();
    expect(region.tabIndex).toBe(0);
});

it('returns focus to the opener when a field in the dialog took autofocus first', () => {
    const opener = visible(document.createElement('button'));
    document.body.append(opener); opener.focus();
    const d = dialog('autofocus'); d.first.focus();
    trapFocus('autofocus', d.el);
    releaseFocusTrap('autofocus'); d.el.remove();
    expect(document.activeElement).toBe(opener);
});

it('keeps a nested trap active and resumes the underlying trap on close', () => {
    const outer = dialog('outer'); trapFocus('outer', outer.el);
    const inner = dialog('inner', outer.el); trapFocus('inner', inner.el);
    key('Tab', true); expect(document.activeElement).toBe(inner.last);
    releaseFocusTrap('inner'); inner.el.remove();
    expect(document.activeElement).toBe(outer.first);
    key('Tab', true); expect(document.activeElement).toBe(outer.last);
});

it('does not steal focus when an underlying trap is disposed first', () => {
    const outer = dialog('outer'); trapFocus('outer', outer.el);
    const inner = dialog('inner'); trapFocus('inner', inner.el);
    releaseFocusTrap('outer'); outer.el.remove();
    expect(document.activeElement).toBe(inner.first);
    key('Tab', true); expect(document.activeElement).toBe(inner.last);
});

it('does not send Escape to a dialog underneath a MessageBox', () => {
    const receiver = { invokeMethodAsync: vi.fn(() => Promise.resolve()) };
    const outer = dialog('outer'); registerDialogEscHandler('outer', receiver); trapFocus('outer', outer.el);
    const inner = dialog('messagebox'); trapFocus('messagebox', inner.el);
    key('Escape'); expect(receiver.invokeMethodAsync).not.toHaveBeenCalled();
    releaseFocusTrap('messagebox'); inner.el.remove();
    key('Escape'); expect(receiver.invokeMethodAsync).toHaveBeenCalledOnce();
});

it('sends focus back to where it came from when a subtree closes', () => {
    const toggle = visible(document.createElement('button'));
    document.body.append(toggle);
    const drawer = visible(document.createElement('div'));
    const inner = visible(document.createElement('button'));
    drawer.append(inner);
    document.body.append(drawer);

    toggle.focus();
    inner.focus();                       // the history now holds the toggle, then the drawer's button
    restoreFocusOutside(drawer);

    expect(document.activeElement).toBe(toggle);
});

it('does not move focus that is already outside the closing subtree', () => {
    const toggle = visible(document.createElement('button'));
    const drawer = visible(document.createElement('div'));
    document.body.append(toggle, drawer);

    toggle.focus();
    restoreFocusOutside(drawer);

    expect(document.activeElement).toBe(toggle);
});

it('keeps an open dialog above a drawer that becomes modal underneath it', () => {
    const receiver = { invokeMethodAsync: vi.fn(() => Promise.resolve()) };
    const confirm = dialog('confirm');
    registerDialogEscHandler('confirm', receiver);
    trapFocus('confirm', confirm.el);
    expect(document.activeElement).toBe(confirm.first);

    // The drawer was open as a sidebar and the viewport narrowed: it is now a modal layer BELOW the
    // modal dialog that was opened on top of it, so it registers as a background trap.
    const drawer = dialog('drawer');
    trapFocus('drawer', drawer.el, true);

    expect(document.activeElement).toBe(confirm.first);          // focus stays with the dialog
    key('Escape');
    expect(receiver.invokeMethodAsync).toHaveBeenCalledOnce();    // and so does Escape
    key('Tab', true);
    expect(document.activeElement).toBe(confirm.last);            // and Tab

    releaseFocusTrap('confirm'); confirm.el.remove();
    // The dialog is gone; the drawer is topmost and takes the keyboard over.
    expect(document.activeElement).toBe(drawer.first);
    key('Tab', true);
    expect(document.activeElement).toBe(drawer.last);
});

it('leaves Escape to a registered popup inside the dialog, and not to an expanded disclosure', () => {
    const receiver = { invokeMethodAsync: vi.fn(() => Promise.resolve()) };
    const d = dialog('select'); registerDialogEscHandler('select', receiver); trapFocus('select', d.el);
    const popup = { invokeMethodAsync: vi.fn(() => Promise.resolve()) };
    pushPopupLayer('select', popup, 'CloseFromEscape');
    const select = visible(document.createElement('div'));
    select.tabIndex = 0; select.setAttribute('role', 'combobox'); select.setAttribute('aria-expanded', 'true');
    const disclosure = visible(document.createElement('button'));
    disclosure.setAttribute('aria-expanded', 'true');
    d.el.append(select, disclosure);

    select.focus(); key('Escape');
    expect(popup.invokeMethodAsync).toHaveBeenCalledWith('CloseFromEscape');
    expect(receiver.invokeMethodAsync).not.toHaveBeenCalled();

    removePopupLayer('select');
    disclosure.focus(); key('Escape');
    expect(receiver.invokeMethodAsync).toHaveBeenCalledOnce();
});

it('keeps the dialog open when the component closes its popup earlier in the same keydown', () => {
    const receiver = { invokeMethodAsync: vi.fn(() => Promise.resolve()) };
    const d = dialog('wasm'); trapFocus('wasm', d.el);
    const select = visible(document.createElement('div'));
    select.tabIndex = 0; select.setAttribute('role', 'combobox'); select.setAttribute('aria-expanded', 'true');
    d.el.append(select);
    const popup = { invokeMethodAsync: vi.fn(() => Promise.resolve()) };
    // Blazor WebAssembly runs the component's keydown handler synchronously inside the dispatch, so a
    // Release build has already closed the select - layer gone, aria-expanded=false - when the module's
    // document listeners run. A listener registered first reproduces that order.
    const componentHandler = (e) => {
        if (e.key !== 'Escape') return;
        removePopupLayer('select');
        select.setAttribute('aria-expanded', 'false');
    };
    document.addEventListener('keydown', componentHandler);
    pushPopupLayer('select', popup, 'CloseFromEscape');
    registerDialogEscHandler('wasm', receiver);
    select.focus();
    key('Escape');
    document.removeEventListener('keydown', componentHandler);

    // The C# handler closed the list; the dialog that owned the key before that must stay open, and the
    // removed layer must not ask the component to close a second time.
    expect(popup.invokeMethodAsync).not.toHaveBeenCalled();
    expect(receiver.invokeMethodAsync).not.toHaveBeenCalled();
});

it('closes a menu focused inside its role=menu panel without closing the dialog', () => {
    const receiver = { invokeMethodAsync: vi.fn(() => Promise.resolve()) };
    const d = dialog('dialog'); registerDialogEscHandler('dialog', receiver); trapFocus('dialog', d.el);
    const menuLayer = { invokeMethodAsync: vi.fn(() => Promise.resolve()) };
    pushPopupLayer('menu', menuLayer, 'CloseFromEscape');

    // FlareMenu moves the focus into its panel, which is a sibling of the activator and carries role=menu but
    // no aria-expanded or aria-haspopup - the exact element the old ARIA heuristic failed to recognise.
    const panel = visible(document.createElement('div'));
    panel.setAttribute('role', 'menu');
    panel.tabIndex = -1;
    d.el.append(panel);
    panel.focus();
    expect(document.activeElement).toBe(panel);

    key('Escape');
    expect(menuLayer.invokeMethodAsync).toHaveBeenCalledWith('CloseFromEscape');
    expect(receiver.invokeMethodAsync).not.toHaveBeenCalled();
});

it('closes only the top of two open popups', () => {
    const first = { invokeMethodAsync: vi.fn(() => Promise.resolve()) };
    const second = { invokeMethodAsync: vi.fn(() => Promise.resolve()) };
    pushPopupLayer('popup-1', first, 'CloseFromEscape');
    pushPopupLayer('popup-2', second, 'CloseFromEscape');

    key('Escape');
    expect(second.invokeMethodAsync).toHaveBeenCalledWith('CloseFromEscape');
    expect(first.invokeMethodAsync).not.toHaveBeenCalled();

    removePopupLayer('popup-2');
    key('Escape');
    expect(first.invokeMethodAsync).toHaveBeenCalledWith('CloseFromEscape');
});

// The menu and its submenu are two popup layers. A real document keydown must reach the submenu alone;
// the C# key handler's stopPropagation cannot do that, which is why each submenu owns a layer.
it('closes the innermost submenu layer with Escape, then the menu underneath', () => {
    const menu = { invokeMethodAsync: vi.fn(() => Promise.resolve()) };
    const submenu = { invokeMethodAsync: vi.fn(() => Promise.resolve()) };
    pushPopupLayer('menu', menu, 'CloseFromEscape');
    pushPopupLayer('submenu', submenu, 'CloseFromEscape');

    key('Escape');
    expect(submenu.invokeMethodAsync).toHaveBeenCalledWith('CloseFromEscape');
    expect(menu.invokeMethodAsync).not.toHaveBeenCalled();

    removePopupLayer('submenu');
    key('Escape');
    expect(menu.invokeMethodAsync).toHaveBeenCalledWith('CloseFromEscape');
});

it('lets Escape reach the dialog again once the popup layer is removed', () => {
    const receiver = { invokeMethodAsync: vi.fn(() => Promise.resolve()) };
    const d = dialog('dialog'); registerDialogEscHandler('dialog', receiver); trapFocus('dialog', d.el);
    const popup = { invokeMethodAsync: vi.fn(() => Promise.resolve()) };
    pushPopupLayer('popup-1', popup, 'CloseFromEscape');

    key('Escape');
    expect(receiver.invokeMethodAsync).not.toHaveBeenCalled();

    removePopupLayer('popup-1');
    key('Escape');
    expect(receiver.invokeMethodAsync).toHaveBeenCalledOnce();
});

it('sends Escape only to the newest registration of a popup closed and opened again', () => {
    const first = { invokeMethodAsync: vi.fn(() => Promise.resolve()) };
    const second = { invokeMethodAsync: vi.fn(() => Promise.resolve()) };
    pushPopupLayer('popup-1', first, 'CloseFromEscape');
    removePopupLayer('popup-1');
    pushPopupLayer('popup-1', second, 'CloseFromEscape');

    key('Escape');
    expect(second.invokeMethodAsync).toHaveBeenCalledOnce();
    expect(first.invokeMethodAsync).not.toHaveBeenCalled();
});

it('drops a layer disposed while its popup was still open', () => {
    const receiver = { invokeMethodAsync: vi.fn(() => Promise.resolve()) };
    const d = dialog('dialog'); registerDialogEscHandler('dialog', receiver); trapFocus('dialog', d.el);
    const popup = { invokeMethodAsync: vi.fn(() => Promise.resolve()) };
    pushPopupLayer('popup-1', popup, 'CloseFromEscape');

    removePopupLayer('popup-1');
    key('Escape');

    expect(popup.invokeMethodAsync).not.toHaveBeenCalled();
    expect(receiver.invokeMethodAsync).toHaveBeenCalledOnce();
});
