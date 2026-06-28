// Caution! Be sure you understand the caveats before publishing an application with
// offline support. See https://aka.ms/blazor-offline-considerations

self.importScripts('./service-worker-assets.js');
self.addEventListener('install', event => event.waitUntil(onInstall(event)));
self.addEventListener('activate', event => event.waitUntil(onActivate(event)));
self.addEventListener('fetch', event => event.respondWith(onFetch(event)));
// Allow the page to activate a waiting (updated) worker on demand (the "update" prompt).
self.addEventListener('message', event => {
    if (event.data === 'skipWaiting') self.skipWaiting();
});

const cacheNamePrefix = 'offline-cache-';
const cacheName = `${cacheNamePrefix}${self.assetsManifest.version}`;
const offlineAssetsInclude = [ /\.dll$/, /\.pdb$/, /\.wasm/, /\.html/, /\.js$/, /\.json$/, /\.css$/, /\.woff$/, /\.png$/, /\.jpe?g$/, /\.gif$/, /\.ico$/, /\.blat$/, /\.dat$/, /\.webmanifest$/ ];
// Never cache the worker itself or its assets manifest: the version-check probe reads
// service-worker-assets.js to learn the deployed version, so it must always hit the network.
const offlineAssetsExclude = [ /^service-worker\.js$/, /^service-worker-assets\.js$/ ];

// Requests the worker must always serve fresh from the network (the version-check manifest) rather
// than from the offline cache - otherwise the deployed-version probe reads a stale value and the
// "update available" prompt never fires.
const networkOnly = [ /service-worker-assets\.js(\?|$)/ ];

// Replace with your base path if you are hosting on a subfolder. Ensure there is a trailing '/'.
const base = "/";
const baseUrl = new URL(base, self.origin);
const manifestUrlList = self.assetsManifest.assets.map(asset => new URL(asset.url, baseUrl).href);

async function onInstall(event) {
    console.info('Service worker: Install');

    // Fetch and cache all matching items from the assets manifest
    const assetsRequests = self.assetsManifest.assets
        .filter(asset => offlineAssetsInclude.some(pattern => pattern.test(asset.url)))
        .filter(asset => !offlineAssetsExclude.some(pattern => pattern.test(asset.url)))
        .map(asset => new Request(asset.url, { integrity: asset.hash, cache: 'no-cache' }));
    await caches.open(cacheName).then(cache => cache.addAll(assetsRequests));
}

async function onActivate(event) {
    console.info('Service worker: Activate');

    // Delete unused caches
    const cacheKeys = await caches.keys();
    await Promise.all(cacheKeys
        .filter(key => key.startsWith(cacheNamePrefix) && key !== cacheName)
        .map(key => caches.delete(key)));

    // Take control of already-open pages immediately. This is the one deviation from the stock Blazor
    // worker, and it is what makes the on-demand update deterministic: when the page posts 'skipWaiting'
    // the new worker activates AND claims this client, firing a single 'controllerchange' that the
    // version-check script keys its reload on - so the reload is always served by the NEW worker and
    // never lands back on the old one (the cause of the stale "dev"/old version after an update).
    await self.clients.claim();
}

async function onFetch(event) {
    // Non-GET (and anything we don't cache) goes straight to the network, guarded so a failed
    // request never turns into an uncaught "Failed to fetch" rejection out of respondWith.
    if (event.request.method !== 'GET')
        return safeFetch(event.request);

    // The version-check manifest must always be read fresh; serve it network-first and only fall
    // back to any cached copy if the network is unavailable.
    if (networkOnly.some(pattern => pattern.test(event.request.url))) {
        try {
            return await fetch(event.request);
        } catch {
            const cache = await caches.open(cacheName);
            return (await cache.match(event.request)) || Response.error();
        }
    }

    // For navigation requests, serve the cached index.html shell (unless the URL is itself a cached
    // offline asset). If you need some URLs server-rendered, exclude them from this check.
    const shouldServeIndexHtml = event.request.mode === 'navigate'
        && !manifestUrlList.some(url => url === event.request.url);

    const request = shouldServeIndexHtml ? 'index.html' : event.request;
    const cache = await caches.open(cacheName);
    const cachedResponse = await cache.match(request);
    if (cachedResponse) return cachedResponse;

    // Not cached: go to the network, but degrade gracefully when offline instead of throwing.
    try {
        return await fetch(event.request);
    } catch (err) {
        if (shouldServeIndexHtml) {
            const shell = await cache.match('index.html');
            if (shell) return shell;
        }
        return Response.error();
    }
}

// fetch() that resolves to an error Response instead of rejecting, so respondWith never gets a
// rejected promise (the source of the noisy "Uncaught (in promise) TypeError: Failed to fetch").
async function safeFetch(request) {
    try {
        return await fetch(request);
    } catch {
        return Response.error();
    }
}
