import { defineConfig } from 'vitest/config';

export default defineConfig({
  test: {
    // jsdom gives the DOM the modules are written against: DOMParser, importNode, replaceChildren.
    // What it does NOT give is Element.setHTML, and that is stated rather than worked around - see the
    // note at the top of sanitize.test.js about which path this suite therefore covers.
    environment: 'jsdom',
    include: ['**/*.test.js'],
    // The modules under test live in the packages, not beside the tests.
    root: '.',
  },
});
