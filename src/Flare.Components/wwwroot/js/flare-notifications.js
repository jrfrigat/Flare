// Page notifications behind IFlareNotifications (Flare.Infrastructure FlareNotificationsService).
//
// The click handler lives on each notification, so nothing is left listening on the window. Nothing here
// prompts the user except requestPermission: show reports a missing permission instead.

export function isSupported() {
    return 'Notification' in window;
}

export function getPermission() {
    return 'Notification' in window ? Notification.permission : 'unsupported';
}

export function requestPermission() {
    if (!('Notification' in window)) return Promise.resolve('unsupported');
    // Older Safari takes a callback and returns nothing; the others return a promise.
    return new Promise(resolve => {
        const pending = Notification.requestPermission(resolve);
        if (pending) pending.then(resolve, () => resolve(Notification.permission));
    });
}

// An address in the application is opened the way a link in it would be - without reloading the app.
function open(url) {
    const target = new URL(url, location.href);
    if (target.origin === location.origin && window.Blazor && typeof window.Blazor.navigateTo === 'function')
        window.Blazor.navigateTo(target.pathname + target.search + target.hash);
    else
        location.assign(target.href);
}

export async function show(title, body, tag, icon, url) {
    if (!('Notification' in window)) return 'unsupported';
    if (Notification.permission !== 'granted') return 'permission';

    const options = {};
    if (body) options.body = body;
    if (tag) options.tag = tag;
    if (icon) options.icon = icon;

    try {
        const n = new Notification(title, options);
        n.onclick = e => {
            e.preventDefault();
            window.focus();
            n.close();
            if (url) open(url);
        };
        return 'shown';
    } catch (err) {
        // Chrome on Android allows notifications only through a service worker registration.
        if (!(err instanceof TypeError)) return 'failed';
        const reg = 'serviceWorker' in navigator ? await navigator.serviceWorker.getRegistration() : null;
        if (!reg) return 'unsupported';
        try {
            await reg.showNotification(title, { ...options, data: { url: url ?? null } });
            return 'shown';
        } catch {
            return 'failed';
        }
    }
}
