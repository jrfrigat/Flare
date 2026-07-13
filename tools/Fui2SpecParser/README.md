# Fui2SpecParser

Generates the **Fluent UI 2 foundation + palette** specs under `docs/spec/` from the official
`@fluentui/tokens` package (the same design-token package `@fluentui/react-components` ships).

```sh
cd tools/Fui2SpecParser
npm install
npm run generate
```

Output (overwritten in place):

| File | Contents |
|------|----------|
| `docs/spec/_pallete/fluentui2-spec.md` | All color alias tokens (Neutral / Brand / Compound Brand / Status / Subtle-Transparent / shared palette) resolved for light **and** dark, plus the raw `grey` and `brandWeb` ramps. |
| `docs/spec/_foundation/fluentui2-spec.md` | Non-color global tokens: typography (17 composite styles + ramps), spacing, border-radius, stroke width, shadow, motion (curves + durations). |

## Scope

This tool only generates the **foundation** (global + alias) layer, which is machine-derivable
because `@fluentui/tokens` exposes fully-resolved `webLightTheme` / `webDarkTheme` objects.

The **per-component** specs (`docs/spec/<component>/fluentui2-spec.md`) are **authored**, not
generated: Fluent keeps per-component styling in code (`@fluentui/react-<pkg>`'s
`use*Styles.styles.ts` griffel `makeStyles`), not in a declarative token table. Each component
spec transcribes its real slot/state -> alias-token bindings from that source and resolves every
token to its literal via this package's theme dump.

## Provenance

`webLightTheme` / `webDarkTheme` here are byte-identical to the objects exported by
`@fluentui/react-theme` (which `@fluentui/react-components` depends on), so the values match
exactly what `<FluentProvider>` applies at runtime. Bump the pinned version in `package.json`
to track a newer Fluent release, then re-run `npm run generate`.

Node tool (the token source is a JS package); `node_modules/` is gitignored.
