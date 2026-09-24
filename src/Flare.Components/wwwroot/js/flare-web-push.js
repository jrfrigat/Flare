// Web Push plumbing behind IFlareWebPush (Flare.Infrastructure FlareWebPushService).
//
// The service worker is the application's; this module only reads and changes the push subscription
// of whatever worker the page has registered. Nothing here prompts the user except requestPermission:
// pushManager.subscribe would show the permission prompt itself, so subscribe checks the permission
// first and reports it instead.

function supported() {
    return 'Notification' in window && 'serviceWorker' in navigator && 'PushManager' in window;
}

export function isSupported() {
    return supported();
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

// navigator.serviceWorker.ready never settles on a page without a registration, so it is only awaited
// once getRegistration has found one whose worker is still installing.
async function registration(waitForActive) {
    if (!('serviceWorker' in navigator)) return null;
    const reg = await navigator.serviceWorker.getRegistration();
    if (!reg) return null;
    return reg.active || !waitForActive ? reg : await navigator.serviceWorker.ready;
}

function decodeKey(key) {
    const base64 = key.replace(/-/g, '+').replace(/_/g, '/').padEnd(Math.ceil(key.length / 4) * 4, '=');
    const raw = atob(base64);
    const bytes = new Uint8Array(raw.length);
    for (let i = 0; i < raw.length; i++) bytes[i] = raw.charCodeAt(i);
    return bytes;
}

function sameBytes(a, b) {
    if (a.length !== b.length) return false;
    for (let i = 0; i < a.length; i++) if (a[i] !== b[i]) return false;
    return true;
}

export async function getSubscription() {
    if (!supported()) return null;
    const reg = await registration(false);
    if (!reg) return null;
    const sub = await reg.pushManager.getSubscription();
    return sub ? sub.toJSON() : null;
}

export async function subscribe(key) {
    if (!supported()) return { status: 'unsupported' };
    if (Notification.permission !== 'granted') return { status: 'permission' };
    const reg = await registration(true);
    if (!reg) return { status: 'no-worker' };

    let keyBytes;
    try { keyBytes = decodeKey(key); }
    catch { return { status: 'failed', error: 'The server key is not base64url.' }; }

    try {
        const existing = await reg.pushManager.getSubscription();
        if (existing) {
            // The browser throws InvalidStateError for a second key; say so instead, and leave the old
            // subscription alone - the server still has it on record.
            const existingKey = existing.options && existing.options.applicationServerKey;
            if (existingKey && !sameBytes(new Uint8Array(existingKey), keyBytes)) return { status: 'key-mismatch' };
            return { status: 'subscribed', subscription: existing.toJSON() };
        }
        const sub = await reg.pushManager.subscribe({ userVisibleOnly: true, applicationServerKey: keyBytes });
        return { status: 'subscribed', subscription: sub.toJSON() };
    } catch (e) {
        return { status: 'failed', error: e && e.name ? `${e.name}: ${e.message}` : String(e) };
    }
}

export async function unsubscribe() {
    if (!supported()) return false;
    const reg = await registration(false);
    if (!reg) return false;
    const sub = await reg.pushManager.getSubscription();
    if (!sub) return false;
    try { return await sub.unsubscribe(); } catch { return false; }
}
