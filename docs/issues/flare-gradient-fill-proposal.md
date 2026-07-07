# Proposal: first-class gradient FILL for Flare (background + text)

**Status:** planned, not started. Separate task from the Deka gap fixes
([deka-design-flare-gaps.md](deka-design-flare-gaps.md) #2).

**Goal:** let a theme or app define a brand gradient and apply it as a *fill* to component backgrounds
(logo mark, avatar, primary CTA, card cover) and to *text* (gradient headings / wordmark), opt-in and
theme-agnostic - without hand-writing `background-clip: text` in every app.

---

## Why this is NOT a `FlareColor` / `ColorScheme` addition

A CSS gradient (`linear-gradient(...)`, `radial-gradient(...)`, `conic-gradient(...)`) is an
`<image>`, not a `<color>`. Everything in Flare's color pipeline is `<color>`-only and silently drops a
gradient:

- Role/custom colors flow through the `--fc-main / --fc-on / --fc-container / --fc-on-container` tokens,
  consumed as `color:`, `background-color:`, `border-color:`, `fill:` - none accept an `<image>`.
- State layers use `color-mix(in srgb, var(--fc-main) 8%, transparent)` - `color-mix` requires colors.
- The on-color / contrast is derived from the main color (`FlareColorResolver.OnColor`) - a gradient has
  no single tone to derive from.

So overloading `FlareColor` would compile-and-silently-fail in most components. A gradient is a
**surface/background** concept and must be a distinct API from `Color`, carrying its own explicit on-color.

---

## Proposed shape

### 1. A `FlareGradient` value type (Flare.Theming)
A small immutable value holding the gradient CSS plus an explicit foreground (on) color, because contrast
cannot be derived from a gradient:

- factory presets: `FlareGradient.Linear(angle, stops...)`, `.Radial(...)`, `.Conic(...)`
- `FlareGradient.Custom(cssImage, on: "...")` (sanitized; see security)
- `.From(role)` helpers that build from two `--flare-color-*` roles (e.g. primary -> a second stop)
- emits inline vars `--fg-image` (the gradient) and `--fg-on` (text/icon color on it)

### 2. A distinct `Fill` / `Background` parameter (NOT `Color`)
Only on components where a decorative fill is meaningful: `FlareAvatar`, `FlareCard` (cover), `FlareButton`
(filled), and a brand-mark. Renders `background-image: var(--fg-image)` with `color: var(--fg-on)`; the
hover/pressed state layer becomes a separate translucent overlay (`::after`) instead of a `color-mix` on
the fill.

### 3. Gradient TEXT
`background-image: var(--fg-image); background-clip: text; -webkit-background-clip: text;
-webkit-text-fill-color: transparent; color: transparent;` exposed as either:
- a `FlareText` `GradientFill="..."` parameter, or
- a utility class (`Css.Classes...GradientText`) apps can drop on any element.

### 4. (Optional) themeable brand gradient role
A `GradientTokens` family (from / to / angle) or a `BrandGradient` palette role so a theme can ship a
default brand gradient, mapped to `--flare-gradient-brand`. Kept optional and default-free per the token
mandate (no baked theme opinion).

---

## Security
The current `CssValidator.SanitizeColor` is a char-whitelist (`^[#a-zA-Z0-9(),.%\s\-]+$`) that already
permits gradient syntax but does not validate it is a color. A gradient fill needs its own sanitizer that
allows the gradient functions + `<color>`/`<length>`/`<angle>` tokens while still blocking `url(...)`,
`expression(...)`, `image-set(...)` remote fetches, `;`, `{`, `}`, `<`, `>` (reuse `StripDangerous`).

## Accessibility / open questions
- Text-on-gradient and text-on-gradient-fill contrast: no automatic guarantee; document that the author
  owns legibility (WCAG). Consider a dev-time contrast warning against the gradient's darkest/lightest stop.
- `prefers-reduced-transparency` / print: fall back to a solid stop (`--fg-on` background or the first stop).
- Should `Fill` and `Color` be mutually exclusive per component, or does `Fill` win? (Proposed: `Fill` wins,
  `Color` ignored when a `Fill` is set.)

## Suggested phasing
1. Gradient **text** (utility + `FlareText.GradientFill`) + a gradient **background** utility class - covers
   the Deka logo mark / wordmark / avatar-initials cases immediately, minimal surface.
2. `FlareGradient` type + `Fill` parameter on `Avatar` / `Card` / `Button`.
3. Optional themeable `BrandGradient` / `GradientTokens`.
