// Resolve resize constraints in the element's own CSS context. Expressions are measured once per
// interaction, so pointer moves do not repeatedly mutate layout just to resolve the same token.
export function cssLengthResolver(element, horizontal) {
    const dimension = horizontal ? 'width' : 'height';
    const expressions = new Map();
    const resolve = (raw, fallback) => {
        const text = String(raw ?? '').trim();
        if (!text) return fallback;
        if (text === '0') return 0;
        const literal = /^(\d+(?:\.\d*)?|\.\d+)(px|rem|em|vw|vh|%)$/.exec(text);
        if (literal) {
            const n = Number(literal[1]);
            const unit = literal[2];
            if (unit === 'px') return n;
            let scale;
            if (unit === 'rem') scale = parseFloat(getComputedStyle(document.documentElement).fontSize);
            else if (unit === 'em') scale = parseFloat(getComputedStyle(element).fontSize);
            else if (unit === 'vw') scale = window.innerWidth / 100;
            else if (unit === 'vh') scale = window.innerHeight / 100;
            else {
                const parent = element.parentElement?.getBoundingClientRect();
                scale = parent ? (horizontal ? parent.width : parent.height) / 100 : NaN;
            }
            return Number.isFinite(scale) ? n * scale : fallback;
        }
        if (!/^(?:var|calc|min|max|clamp)\(/.test(text)) return fallback;
        if (globalThis.CSS?.supports && !CSS.supports(dimension, text)) return fallback;
        if (!expressions.has(text)) {
            const previous = element.style.getPropertyValue(dimension);
            const priority = element.style.getPropertyPriority(dimension);
            let value;
            try {
                // A sibling probe would lose element-local variables; a child probe would resolve
                // percentages against the wrong box. Restore synchronously before the browser paints.
                element.style.setProperty(dimension, text);
                value = parseFloat(getComputedStyle(element)[dimension]);
            } finally {
                if (previous) element.style.setProperty(dimension, previous, priority);
                else element.style.removeProperty(dimension);
            }
            expressions.set(text, Number.isFinite(value) && value >= 0 ? value : NaN);
        }
        const value = expressions.get(text);
        return Number.isFinite(value) ? value : fallback;
    };
    return { resolve, refresh: () => expressions.clear() };
}
