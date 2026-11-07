import { afterEach, expect, it, vi } from 'vitest';
import { cssLengthResolver } from '../../src/Flare.Components/wwwroot/js/flare-css-length.js';

afterEach(() => { vi.restoreAllMocks(); document.body.replaceChildren(); });

it('measures element-local CSS expressions once per interaction and restores the declaration', () => {
    const element = document.createElement('div');
    element.style.height = '80px';
    document.body.append(element);
    let usedHeight = 160;
    // jsdom has no layout engine; the real var/calc path is also exercised by SchedulerInteractionTests.
    const computed = vi.spyOn(globalThis, 'getComputedStyle').mockImplementation(() => ({ height: usedHeight + 'px' }));
    const resolver = cssLengthResolver(element, false);
    expect(resolver.resolve('calc(var(--unit) * 4)', 0)).toBe(160);
    expect(element.style.height).toBe('80px');
    usedHeight = 200;
    expect(resolver.resolve('calc(var(--unit) * 4)', 0)).toBe(160);
    expect(computed).toHaveBeenCalledTimes(1);
    resolver.refresh();
    expect(resolver.resolve('calc(var(--unit) * 4)', 0)).toBe(200);
    expect(element.style.height).toBe('80px');
});

it('resolves percentages against the parent and ignores unsupported or negative lengths', () => {
    const parent = document.createElement('div');
    const child = document.createElement('div');
    parent.append(child); document.body.append(parent);
    parent.getBoundingClientRect = () => ({ width: 400, height: 300 });
    const resolver = cssLengthResolver(child, true);
    expect(resolver.resolve('50%', 0)).toBe(200);
    expect(resolver.resolve('-10px', 7)).toBe(7);
    expect(resolver.resolve('12oops', 7)).toBe(7);
});
