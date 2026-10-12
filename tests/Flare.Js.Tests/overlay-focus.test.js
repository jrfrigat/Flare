import { afterEach, expect, it, vi } from 'vitest';
import { trapFocus, releaseFocusTrap, registerDialogEscHandler, removeDialogEscHandler } from '../../src/Flare.Components/wwwroot/js/flare-overlay.js';

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
    expect(region.hasAttribute('tabindex')).toBe(false);
});

it('uses the fallback when the initiator becomes disabled', () => {
    const region = visible(document.createElement('main'));
    region.tabIndex = 0;
    const trigger = visible(document.createElement('button'));
    region.append(trigger); document.body.append(region); trigger.focus();
    const d = dialog('disabled'); trapFocus('disabled', d.el); trigger.disabled = true;
    releaseFocusTrap('disabled');
    expect(document.activeElement).toBe(region); expect(region.tabIndex).toBe(0);
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
