import { afterEach, expect, it, vi } from 'vitest';
import '../../src/Flare.Components/wwwroot/js/flare-file-upload.js';

afterEach(() => { document.body.replaceChildren(); vi.unstubAllGlobals(); });

function fixture(disabled = false) {
    document.body.innerHTML = '<div data-flare-file-drop="upload"><span>Drop</span><input id="upload" type="file" data-flare-file-input><ul data-flare-file-drop-ignore><li>Existing</li></ul></div>';
    const input = document.querySelector('input');
    Object.defineProperty(input, 'files', { value: null, writable: true });
    input.disabled = disabled;
    return { input, zone: document.querySelector('div'), caption: document.querySelector('span'), list: document.querySelector('li') };
}

function drop(target, files) {
    const event = new Event('drop', { bubbles: true, cancelable: true });
    Object.defineProperty(event, 'dataTransfer', { value: { files } });
    target.dispatchEvent(event);
    return event;
}

it('forwards file drops on the input and zone padding once through a bubbling change', () => {
    const { input, zone, caption } = fixture();
    let changes = 0;
    zone.addEventListener('change', () => changes++);
    for (const target of [input, zone, caption]) {
        const files = [new File(['bytes'], 'file.txt')];
        expect(drop(target, files).defaultPrevented).toBe(true);
        expect(input.files).toBe(files);
    }
    expect(changes).toBe(3);
});

it('refuses disabled-zone and file-list drops while still preventing navigation', () => {
    const { input, caption, list } = fixture(true);
    const files = [new File(['bytes'], 'file.txt')];
    expect(drop(caption, files).defaultPrevented).toBe(true);
    expect(input.files).toBeNull();
    input.disabled = false;
    expect(drop(list, files).defaultPrevented).toBe(true);
    expect(input.files).toBeNull();
});

it('leaves non-file drops and unrelated targets alone', () => {
    const { input, caption } = fixture();
    expect(drop(caption, []).defaultPrevented).toBe(false);
    expect(drop(document.body, [new File(['bytes'], 'file.txt')]).defaultPrevented).toBe(false);
    expect(input.files).toBeNull();
});

it('resets only managed enabled file inputs before the picker opens', () => {
    const { input } = fixture();
    Object.defineProperty(input, 'value', { value: 'previous', writable: true });
    input.dispatchEvent(new MouseEvent('click', { bubbles: true }));
    expect(input.value).toBe('');
    input.value = 'previous';
    input.disabled = true;
    input.dispatchEvent(new MouseEvent('click', { bubbles: true }));
    expect(input.value).toBe('previous');
    input.disabled = false;
    input.removeAttribute('data-flare-file-input');
    input.dispatchEvent(new MouseEvent('click', { bubbles: true }));
    expect(input.value).toBe('previous');
});

it('preserves the native single-file contract while multiple inputs receive the whole drop', () => {
    vi.stubGlobal('DataTransfer', class {
        files = [];
        items = { add: file => this.files.push(file) };
    });
    const { input, caption } = fixture();
    const files = [new File(['one'], 'one.txt'), new File(['two'], 'two.txt')];
    drop(caption, files);
    expect(input.files).toEqual([files[0]]);
    input.multiple = true;
    drop(caption, files);
    expect(input.files).toBe(files);
});
