// Flare color extractor: derive a representative seed color from an image, suitable for
// FlareColor.Dynamic. Loads the image to a small offscreen canvas, quantizes the pixels into
// coarse buckets, then scores buckets by population x chroma (skipping near-grey / very dark /
// very light) - a lightweight take on the MD3 Score step. Cross-origin images need CORS
// (crossorigin=anonymous + an allowing host) or getImageData throws (returns null here).

function rgbToHsl(r, g, b) {
    r /= 255; g /= 255; b /= 255;
    const max = Math.max(r, g, b), min = Math.min(r, g, b), d = max - min;
    const l = (max + min) / 2;
    let s = 0, h = 0;
    if (d !== 0) {
        s = d / (1 - Math.abs(2 * l - 1));
        if (max === r) h = ((g - b) / d) % 6;
        else if (max === g) h = (b - r) / d + 2;
        else h = (r - g) / d + 4;
        h *= 60; if (h < 0) h += 360;
    }
    return [h, s, l];
}

const hex = (r, g, b) => '#' + [r, g, b].map(x => Math.round(x).toString(16).padStart(2, '0')).join('');

async function loadPixels(src, size) {
    const img = new Image();
    img.crossOrigin = 'anonymous';
    img.src = src;
    await img.decode();
    const c = (typeof OffscreenCanvas !== 'undefined')
        ? new OffscreenCanvas(size, size)
        : Object.assign(document.createElement('canvas'), { width: size, height: size });
    const ctx = c.getContext('2d', { willReadFrequently: true });
    ctx.drawImage(img, 0, 0, size, size);
    return ctx.getImageData(0, 0, size, size).data;
}

// Returns the most suitable seed color as #RRGGBB, or null if it cannot be read.
export async function dominantColor(src, size) {
    try {
        const data = await loadPixels(src, size || 32);
        const buckets = new Map();   // key -> { count, r, g, b, score }
        for (let i = 0; i < data.length; i += 4) {
            if (data[i + 3] < 128) continue;                 // transparent
            const r = data[i], g = data[i + 1], b = data[i + 2];
            const [, s, l] = rgbToHsl(r, g, b);
            if (l < 0.10 || l > 0.92 || s < 0.15) continue;  // skip near-black/white/grey
            const key = (r >> 4) << 8 | (g >> 4) << 4 | (b >> 4);   // 4 bits/channel
            const e = buckets.get(key) || { count: 0, r: 0, g: 0, b: 0 };
            e.count++; e.r += r; e.g += g; e.b += b;
            buckets.set(key, e);
        }
        if (buckets.size === 0) return null;
        let best = null, bestScore = -1;
        for (const e of buckets.values()) {
            const r = e.r / e.count, g = e.g / e.count, b = e.b / e.count;
            const [, s] = rgbToHsl(r, g, b);
            const score = e.count * (0.4 + s);               // population weighted by chroma
            if (score > bestScore) { bestScore = score; best = [r, g, b]; }
        }
        return best ? hex(best[0], best[1], best[2]) : null;
    } catch {
        return null;
    }
}

// Returns up to `count` representative colors (#RRGGBB), most prominent first.
export async function palette(src, count, size) {
    try {
        const data = await loadPixels(src, size || 32);
        const buckets = new Map();
        for (let i = 0; i < data.length; i += 4) {
            if (data[i + 3] < 128) continue;
            const r = data[i], g = data[i + 1], b = data[i + 2];
            const [, , l] = rgbToHsl(r, g, b);
            if (l < 0.06 || l > 0.96) continue;
            const key = (r >> 4) << 8 | (g >> 4) << 4 | (b >> 4);
            const e = buckets.get(key) || { count: 0, r: 0, g: 0, b: 0 };
            e.count++; e.r += r; e.g += g; e.b += b;
            buckets.set(key, e);
        }
        return [...buckets.values()]
            .sort((a, b) => b.count - a.count)
            .slice(0, count || 5)
            .map(e => hex(e.r / e.count, e.g / e.count, e.b / e.count));
    } catch {
        return [];
    }
}
