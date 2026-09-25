# Fluent1SpecParser

Generates the **Fluent UI v8 (Fluent 1) foundation + palette** specs under `docs/spec/` from the
official v8 theme packages: `@fluentui/theme` (the theme `@fluentui/react` v8 applies) and
`@fluentui/theme-samples` (the v8 dark theme).

```sh
cd tools/Fluent1SpecParser
npm install
npm run generate
```

Output (overwritten in place):

| File | Contents |
|------|----------|
| `docs/spec/_pallete/fluentui1-spec.md` | `theme.palette` (theme ramp, neutral ramp, accent, shared colors) and all `theme.semanticColors` roles, light **and** dark, plus the `NeutralColors` / `SharedColors` / `CommunicationColors` reference ramps. |
| `docs/spec/_foundation/fluentui1-spec.md` | Non-color values: the 13 `theme.fonts` steps, `FontSizes`, `FontWeights`, `IconFontSizes`, localized font families, `effects` (radius + elevation), `Depths`, `spacing`, `MotionDurations`, `MotionTimings`, `AnimationVariables` and the `MotionAnimations` names. |

## Scope

This tool only generates the **foundation** layer, which is machine-derivable because
`createTheme()` returns a fully resolved theme object.

The **per-component** specs (`docs/spec/<component>/fluentui1-spec.md`) are **authored**, not
generated: v8 keeps component styling in code (each component's `*.styles.ts` in
`@fluentui/react`, for example `BaseButton.styles.ts`), not in a declarative token table. Each
component spec transcribes the real slot/state -> semantic color bindings from that source and
resolves every value through this tool's theme dump. The per-component overrides that
`DarkCustomizations` carries (`scopedSettings`) belong there too, not here.

v8 has **no line height** in its type ramp: a `theme.fonts` step sets family, size and weight only.

## Where the dark scheme comes from

v8 core has no dark theme: `createTheme()` is light. The only dark theme Microsoft publishes for v8
is `DarkTheme` in `@fluentui/theme-samples` (the one the v8 documentation site switches to). The
generator takes it from `DARK_SOURCE` at the top of `generate.mjs`, and both output files name the
package and version it came from. To use a different dark theme, point `DARK_SOURCE` at another
package export that is a full inverted theme.

The generator fails instead of writing when:
- `@fluentui/react` (which `theme-samples` builds its dark theme with) resolves a second copy of
  `@fluentui/theme`, so the light and dark columns would come from different releases;
- the dark export is missing or not inverted;
- the light and dark themes differ in `fonts`, `effects` or `spacing` (the foundation file has one
  column);
- a palette or semantic color key matches no group in the generator.

## Updating the version

Versions are pinned exactly in `package.json`: `dependencies` pin the two theme packages, and
`overrides` pin `@fluentui/theme` and `@fluentui/react` for the whole tree. Bump all four
together, run `npm install` and `npm run generate`, and review the diff of both spec files.

Node tool (the theme source is a JS package); `node_modules/` and `package-lock.json` are
gitignored. The CommonJS build is loaded through `createRequire`, because the ESM build of
`@fluentui/theme` uses extensionless imports that Node cannot resolve.
