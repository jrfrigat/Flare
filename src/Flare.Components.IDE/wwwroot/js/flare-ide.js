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
