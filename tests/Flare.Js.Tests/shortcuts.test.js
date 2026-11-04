import { describe, it, expect, vi, afterEach } from 'vitest';
import { registerShortcutListener, removeShortcutListener } from '../../src/Flare.Components/wwwroot/js/flare-ui.js';

afterEach(() => { removeShortcutListener('first'); removeShortcutListener('second'); });

describe('shortcut owner lifecycle', () => {
    it('keeps siblings active when an owner is replaced or removed', () => {
        const first = { invokeMethodAsync: vi.fn().mockResolvedValue(null) };
        const second = { invokeMethodAsync: vi.fn().mockResolvedValue(null) };
        const replaced = { invokeMethodAsync: vi.fn().mockResolvedValue(null) };
        registerShortcutListener('first', replaced);
        registerShortcutListener('first', first);
        registerShortcutListener('second', second);
        const press = () => document.dispatchEvent(new KeyboardEvent('keydown', { key: 'K', ctrlKey: true, altKey: true }));
        press();
        expect(replaced.invokeMethodAsync).not.toHaveBeenCalled();
        expect(first.invokeMethodAsync).toHaveBeenCalledExactlyOnceWith('HandleKeyDown', 'ctrl+alt+k');
        expect(second.invokeMethodAsync).toHaveBeenCalledExactlyOnceWith('HandleKeyDown', 'ctrl+alt+k');
        removeShortcutListener('second');
        press();
        expect(first.invokeMethodAsync).toHaveBeenCalledTimes(2);
        expect(second.invokeMethodAsync).toHaveBeenCalledTimes(1);
        removeShortcutListener('first');
        press();
        expect(first.invokeMethodAsync).toHaveBeenCalledTimes(2);
    });
});
