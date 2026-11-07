// OS file drops use the same InputFile change path as the native picker. Cancelling the browser
// default also protects the padding around the input from opening the dropped file as a document.
document.addEventListener('drop', event => {
    const target = event.target;
    if (!(target instanceof Element)) return;
    const zone = target.closest('[data-flare-file-drop]');
    const files = event.dataTransfer?.files;
    if (!zone || !files?.length) return;
    event.preventDefault();
    if (target.closest('[data-flare-file-drop-ignore]')) return;
    const input = document.getElementById(zone.getAttribute('data-flare-file-drop'));
    if (!(input instanceof HTMLInputElement) || input.type !== 'file' || input.disabled || !zone.contains(input)) return;
    if (!input.multiple && files.length > 1) {
        const selection = new DataTransfer();
        selection.items.add(files[0]);
        input.files = selection.files;
    } else input.files = files;
    input.dispatchEvent(new Event('change', { bubbles: true }));
});

// Reset the native selection before opening the picker, so choosing the same file again is a change.
// InputFile retains the File objects already handed to an active upload until the next change.
document.addEventListener('click', event => {
    const input = event.target;
    if (input instanceof HTMLInputElement && input.type === 'file'
        && input.hasAttribute('data-flare-file-input') && !input.disabled) input.value = '';
}, true);
