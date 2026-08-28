# Flare owns only part of the application setup

**Status: OPEN. Tier 0. Reported items 10 and 11.**

Two unrelated symptoms with one root cause: Flare presents itself as owning theming and services, and
then leaves two holes the application has to patch by hand. Both were found by the same app.

## 1. No document-level reset - `body` keeps the UA `margin: 8px`

> in the fui2 theme body has `margin: 8`; I had to write `body { margin: 0 }` in my app CSS. That is not
> acceptable if we say Flare controls the themes.

The theme is not the culprit: no Flare stylesheet or theme sets a body margin at all. `8px` is the
browser's own UA default, and Flare never overrides it. Every app therefore starts with an 8px gutter
around a full-bleed layout, and each one is expected to discover and fix that itself. The Gallery does -
`samples/Flare.Gallery/wwwroot/css/app.css:4` is `margin: 0; padding: 0;` - which is why this was never
noticed in-house.

The reporter is right, and the fix belongs in Flare. A component library that ships a *theme engine* and
paints surfaces from tokens has to normalize the document those surfaces sit in, the way every other
design-system library does.

**Design.** A `reset.css` in the global bundle, wrapped in `:where()` so its specificity is zero and any
app rule beats it without `!important`. It carries only what the theme cannot express otherwise:

- `box-sizing: border-box` inherited from the root;
- `body { margin: 0 }` - the UA gutter, the actual complaint;
- body background / text color / font from `--flare-color-surface`, `--flare-color-on-surface` and the
  body typescale tokens, so an unstyled document already agrees with the theme;
- `-webkit-text-size-adjust: 100%` and `text-rendering` defaults for mobile.

Nothing with a visual opinion beyond that, and no literal colors or sizes - the mandate applies to the
reset as much as to any component: it references semantic tokens only.

Because a reset is a global side effect it is a separate stylesheet (`_content/Flare.Components/css/reset.css`)
that `flare-components.css` imports, so an app that genuinely wants the UA defaults can link the bundle
without it. That is documented, not hidden.

## 2. `AddFlare` does not register `TimeProvider`

> Flare injected services: AddFlare does NOT register TimeProvider (the Gallery and Weir.Admin register
> it themselves), and FlareDatePicker fails to activate without it.

Confirmed: `grep -rn TimeProvider src/` returns nothing outside the test helper.
`ServiceCollectionExtensions.AddFlare` registers twenty-odd services and not this one, so a component
that injects `TimeProvider` throws `InvalidOperationException: Cannot provide a value for property ...`
the first time a date picker is rendered. The Gallery works because it registers it itself, so again the
in-house sample hid the defect.

**Design.** `AddFlare` registers `TimeProvider.System` with `TryAddSingleton`, so an app that already
registered its own (or a test that registered a fake) wins. That is the whole fix, but the *rule* it
implies is the point of this issue:

> **`AddFlare()` must be sufficient.** Any service a Flare component injects is registered by `AddFlare`.
> A component may not depend on a registration the application is expected to guess.

**Guard.** A test that reflects over every public component in `Flare.Components` (and the satellite
packages), collects every `[Inject]` property and constructor dependency, and asserts each one is
resolvable from a service collection built by `AddFlare()` alone. That turns the rule into something the
build enforces, and would have caught `TimeProvider` on the day it was introduced.
