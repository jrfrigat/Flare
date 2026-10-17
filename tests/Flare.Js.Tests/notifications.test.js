import { afterEach, beforeEach, expect, it, vi } from 'vitest';
import { getPermission, show } from '../../src/Flare.Components/wwwroot/js/flare-notifications.js';

// A stand-in for the browser's Notification: records what was shown and lets a test click it.
class FakeNotification {
    static permission = 'granted';
    static shown = [];
    constructor(title, options) {
        this.title = title;
        this.options = options;
        this.closed = false;
        FakeNotification.shown.push(this);
    }
    close() { this.closed = true; }
}

const click = n => n.onclick({ preventDefault() { } });

beforeEach(() => {
    FakeNotification.permission = 'granted';
    FakeNotification.shown = [];
    window.Notification = FakeNotification;
    window.Blazor = { navigateTo: vi.fn() };
    window.focus = vi.fn();
});

afterEach(() => {
    delete window.Notification;
    delete window.Blazor;
});

it('shows the title with only the options that were given', async () => {
    expect(await show('Build finished', 'All green', 'build', null, null)).toBe('shown');
    expect(FakeNotification.shown[0].title).toBe('Build finished');
    expect(FakeNotification.shown[0].options).toEqual({ body: 'All green', tag: 'build' });
});

it('brings the tab forward, closes the notification and opens an app address without a reload', async () => {
    await show('Build finished', null, null, null, '/builds/42?tab=log');
    const n = FakeNotification.shown[0];

    click(n);

    expect(window.focus).toHaveBeenCalled();
    expect(n.closed).toBe(true);
    expect(window.Blazor.navigateTo).toHaveBeenCalledWith('/builds/42?tab=log');
});

it('only brings the tab forward when there is no address', async () => {
    await show('Build finished', null, null, null, null);

    click(FakeNotification.shown[0]);

    expect(window.focus).toHaveBeenCalled();
    expect(window.Blazor.navigateTo).not.toHaveBeenCalled();
});

it('reports a missing permission instead of prompting', async () => {
    FakeNotification.permission = 'default';
    FakeNotification.requestPermission = vi.fn();

    expect(await show('Build finished', null, null, null, null)).toBe('permission');
    expect(FakeNotification.shown).toHaveLength(0);
    expect(FakeNotification.requestPermission).not.toHaveBeenCalled();
});

it('degrades where the browser has no notifications', async () => {
    delete window.Notification;

    expect(getPermission()).toBe('unsupported');
    expect(await show('Build finished', null, null, null, null)).toBe('unsupported');
});

it('leaves nothing listening on the window', async () => {
    const add = vi.spyOn(window, 'addEventListener');

    await show('Build finished', null, null, null, '/x');

    expect(add).not.toHaveBeenCalled();
    add.mockRestore();
});
