# Flare - Theme Creation Guide

> [Русская версия ->](../ru/theme-creation-guide.md) - [README](https://github.com/jrfrigat/Flare/blob/main/README.md)

## Overview

A rendered theme in Flare is the composition of three independently switchable axes:
1. **Design System** (`ITheme`, non-color tokens): typography, shape, motion, state, elevation, component tokens
2. **Color Palette** (`Palette`, color tokens): ~47 semantic color roles, with light + dark (and optional high-contrast) variants
3. **Mode** (`ThemeMode`): Light / Dark / Auto - selects which `ColorScheme` of the palette is applied

This guide covers authoring the first two: themes provide design tokens, palettes provide colors, and
you can mix any theme with any palette. Mode is a runtime selection, not something you author.

## Quick Start

### Using FlareThemeBuilder (Recommended)

```csharp
using Flare.Theming;

var myTheme = new FlareThemeBuilder("my-theme", "My Custom Theme")
    .WithTypography(new TypographyTokens
    {
        BodyLarge = new TypeStyle
        {
            FontFamily = "Inter",
            FontWeight = "400",
            FontSize = "1rem",
            LineHeight = "1.5rem",
            LetterSpacing = "0em"
        },
        // ... other type styles
    })
    .WithShape(new ShapeTokens
    {
        None = "0px",
        ExtraSmall = "4px",
        Small = "8px",
        Medium = "12px",
        Large = "16px",
        ExtraLarge = "28px",
        Full = "9999px"
    })
    .WithStyleAsset("_content/MyApp/css/my-theme.css")
    .WithDefaultPalette("my-brand")
    .Build();
```

### Deriving from a built-in theme

To start from a built-in theme (MD3, Fluent UI 2, Aero, ...) and override only a few parameters, use
`Derive` - composition, not subclassing (the theme classes are intentionally `sealed`, which keeps the
theme auto-discovery and `with`-based override model clean):

```csharp
using Flare.Theming;
using Flare.Theme.FluentUI2;

var myFluent = new FluentUI2Theme().Derive(
    id: "my-fluent",                 // required: a distinct id
    displayName: "My Fluent",
    design: d => d with { Shape = d.Shape with { Medium = "6px" } });

services.AddFlareTheme(myFluent);
```

`Derive` forwards every member of the base theme (palettes, default palette, style assets, palette
generator, dark overrides) except the ones you pass; `design` receives the base `DesignTokens` so you
`with`-override just what you need.

Each theme package also exposes its reference tokens (`Md3`, `Fluent2`, `Aero`, `LiquidGlass`,
`VisualStudio`) for composing them directly when implementing `ITheme` from scratch:

```csharp
public DesignTokens Design => Fluent2.DesignReference with { /* overrides */ };
// palette colors:  Fluent2.LightColors with { Primary = "#0F6CBD" }
```

### Implementing ITheme Directly

Flare's core (`Flare.Abstractions`) ships **no default token values** - every group on `DesignTokens`
and every member of every `*Tokens` record is `required`, so the core carries no baked-in design
opinion (guarded by `ThemeIndependenceTests`). A bare `new DesignTokens { ... }` therefore has to set
*every* token, which is impractical. Instead, **derive from a published reference package** and
`with`-override only what differs - this is exactly what the built-in themes do:

- `Flare.Theme.MaterialDesign3.Tokens` -> `MaterialDesignTokens.Design` (Material lineage baseline)
- `Flare.Theme.FluentUI2.Tokens` -> `FluentUI2Tokens.Design` (Fluent lineage baseline)

```csharp
using Flare.Abstractions;
using Flare.Abstractions.Tokens;
using Flare.Theme.MaterialDesign3.Tokens;   // or Flare.Theme.FluentUI2.Tokens

public sealed class MyTheme : ITheme
{
    public string Id => "my-theme";
    public string DisplayName => "My Custom Theme";

    // Start from a fully-populated reference; override only the tokens you care about.
    public DesignTokens Design => MaterialDesignTokens.Design with
    {
        FocusRing = "2px solid var(--flare-color-primary)",
        Shape = MaterialDesignTokens.Design.Shape with { Medium = "6px" },
        Button = MaterialDesignTokens.Design.Button with { HeightMd = "2.5rem" },
        // ... only the tokens that differ from the base
    };

    public string DefaultPaletteId => "my-brand";
    public IReadOnlyList<string> StyleAssets => [
        "_content/MyApp/css/my-theme.css"
    ];
}
```

If you genuinely want a from-scratch design system with no Material/Fluent ancestry, construct a full
`DesignTokens` yourself (setting every `required` group) - the compiler (CS9035) will list any token
you miss.

## Registering a Theme

```csharp
// In Program.cs or Startup.cs
services.AddFlare(options =>
{
    options.DefaultTheme = new MyTheme();
    options.DefaultPalette = myBrandPalette;
    options.RegisterAllBuiltInThemes = false; // only register what you need
});

// Or register at runtime
public void ConfigureThemeService(IThemeService themeService)
{
    themeService.RegisterTheme(new MyTheme());
    themeService.RegisterPalette(myBrandPalette);
}
```

## Creating a Palette

### From Seed Colors

```csharp
var palette = PaletteFactory.FromColors(
    id: "my-brand",
    name: "My Brand Colors",
    main: "#6750A4",      // brand color
    background: "#FFFBFE" // optional background tint
);
```

### Manual Palette

```csharp
var palette = new Palette
{
    Id = "my-brand",
    Name = "My Brand",
    Source = "Custom",
    Light = new ColorScheme
    {
        Primary = "#6750A4",
        OnPrimary = "#FFFFFF",
        PrimaryContainer = "#EADDFF",
        OnPrimaryContainer = "#21005D",
        // ... all 45+ color roles
    },
    Dark = new ColorScheme
    {
        Primary = "#D0BCFF",
        OnPrimary = "#381E72",
        PrimaryContainer = "#4F378B",
        OnPrimaryContainer = "#EADDFF",
        // ... all 45+ color roles
    }
};
```

### Dynamic Color (palette from the OS accent)

Flare can derive a full light + dark palette at runtime from the **OS/browser accent color** - the
Windows/macOS accent, or Android Material You - read via the CSS `AccentColor` system color. The
palette is generated through the **active theme's** generator (MD3 tonal, Fluent ramp, ...), so it
adapts to whichever theme is selected and is regenerated when you switch themes.

Enable it once in `AddFlare`:

```csharp
builder.Services.AddFlare(opts =>
{
    opts.DefaultTheme = new MaterialDesign3ExpressiveTheme();
    opts.UseDynamicPalette = true;                  // registers the "dynamic" palette
    opts.DynamicFallbackPalette = Md3Palettes.Violet; // curated palette when the OS accent is unavailable
});
```

When no other default palette is set, the dynamic palette becomes the default. Otherwise it is just
selectable at runtime like any palette (e.g. from a palette picker):

```csharp
await ThemeService.SetPaletteAsync(Palette.DynamicId);   // "dynamic"
```

`FlareThemeProvider` reads the accent on startup, re-reads it when the window regains focus or the OS
light/dark setting changes, and regenerates with the new generator when the theme changes - no extra
wiring needed.

> **Important - Chromium does not expose the real OS accent.** The accent comes from the CSS
> `AccentColor` system color. To mitigate fingerprinting, **Chrome and Edge return a fixed placeholder**
> (`#0075FF`, identical for every user in light and dark, even in installed PWAs) instead of the user's
> real Windows/macOS accent. Only **Firefox** (and engines that expose the genuine accent) reflect the
> actual OS color; on Android Chrome the accent reflects Material You. Flare treats that Chromium
> placeholder as "no accent" and uses the fallback below, so the Dynamic palette never shows an
> arbitrary blue that is the same for everyone. The web exposes no deeper "wallpaper palette" API.

**Fallback palette.** When the OS accent is unavailable (Chrome/Edge, or older engines without
`AccentColor`), set `DynamicFallbackPalette` to a curated palette - the Dynamic palette adopts its
exact colors instead of an approximation. This is the recommended setup. If you prefer a generated
fallback, set `DynamicPaletteFallbackSeed` (a seed color) instead; the palette is then generated from
it with the active theme's rules. A genuine accent (Firefox) still overrides either fallback.

**From your own seed.** To drive the dynamic palette from any color (e.g. one extracted from an image
via `IFlareColorExtractor`), apply a seed directly - it is generated with the active theme's rules:

```csharp
await ThemeService.ApplyDynamicPaletteAsync(new PaletteSeed("#3F51B5"));
```

## Token System

### Available Token Records

| Token | Purpose | Fields |
|-------|---------|--------|
| `TypographyTokens` | Font families, sizes, weights | 15 type scales |
| `ShapeTokens` | Corner radii | 7 levels |
| `ElevationTokens` | Box shadows | 6 levels |
| `MotionTokens` | Durations + easings | 6 durations + 4 easings |
| `StateTokens` | Opacity levels | 6 states |
| `ButtonTokens` | Button geometry, selected state, toggle colour | 62 fields |
| `ButtonGroupTokens` | Both group models, per size | 20 fields |
| `ToggleButtonTokens` | The segmented container only | 4 fields |
| `InputTokens` | Form field geometry | 23 fields |
| `DialogTokens` | Modal dialog | 26 fields |
| `DrawerTokens` | Navigation drawer | 18 fields |
| `SnackbarTokens` | Notifications | 22 fields |
| `SelectTokens` | Dropdowns | 24 fields |
| `TooltipTokens` | Tooltips | 15 fields |
| `PopoverTokens` | Popovers | 12 fields |
| `DataGridTokens` | Data grids | 33 fields |
| `CardTokens` | Cards | 20 fields |
| `AvatarTokens` | Avatars | 17 fields |
| `ProgressTokens` | Progress indicators | 18 fields |
| `SwitchTokens` | Toggle switches | 28 fields |
| `NavTokens` | Nav item + active indicator | 4 fields |
| `RatingTokens` | Star rating | 4 fields |
| `PaginationTokens` | Pagination controls | 4 fields |
| `TimelineTokens` | Timeline dot + connector | 7 fields |
| `StepperTokens` | Stepper circle + connector | 8 fields |
| `TreeTokens` | Tree view rows | 6 fields |
| `CalendarTokens` | Calendar month/day grid | 9 fields |

This is a representative subset; the full set of component token records lives in
`Flare.Abstractions/Tokens/Components/`. Every record's members are `required`, so the compiler
lists any you miss when you build a `DesignTokens` from scratch.

### The button family: what your theme has to answer (0.16.0)

The button carries more decisions than any other component, and three of them are easy to miss because
they only appear in states you may not have thought about yet. If you derive from an in-box theme with
`with { }` you inherit workable answers for all of them; if you build `ButtonTokens` from scratch, the
compiler will name every one.

**1. A selected button is a shape change, not only a colour change.** A toggle button is a
`FlareButton` wearing `flare-btn--selected` - the same element, the same tokens - so selection is
described in `ButtonTokens`:

```csharp
// Material states it as a SWAP: round becomes square, square becomes round.
SelectedRadiusXs = "0.75rem",   // what a round button takes when selected, per size
SelectedRadiusSm = "0.75rem",
SelectedRadiusMd = "1rem",
SelectedRadiusLg = "1.75rem",
SelectedRadiusXl = "1.75rem",
// ...and the other direction, for a button whose rest shape is the explicit square.
// A language that does NOT reshape on selection points this back at its own square radius:
SelectedRadiusSquare = "var(--flare-shape-none)",
```

**2. Selection has a colour per variant, and one variant differs before anything is selected.**
Material keeps a separate colour table for toggles - "the default and toggle buttons use different
colors" - so each variant names where it lands:

```csharp
ElevatedSelectedBg = "var(--flare-color-primary)",
ElevatedSelectedColor = "var(--flare-color-on-primary)",
FilledSelectedBg = "var(--flare-color-primary)",
FilledSelectedColor = "var(--flare-color-on-primary)",
TonalSelectedBg = "var(--flare-color-secondary)",
TonalSelectedColor = "var(--flare-color-on-secondary)",
OutlinedSelectedBg = "var(--flare-color-inverse-surface)",
OutlinedSelectedColor = "var(--flare-color-inverse-on-surface)",
// The filled toggle is the one that differs while UNSELECTED: a filled toggle at rest is a neutral
// container, not the accent fill a filled BUTTON is, or a row of options reads as already chosen.
FilledUnselectedBg = "var(--flare-color-surface-container)",
FilledUnselectedColor = "var(--flare-color-on-surface-variant)",
// The fallback pair, for any variant with no entry of its own (in Material's table, only Text).
SelectedBg = "var(--flare-color-secondary-container)",
SelectedColor = "var(--flare-color-on-secondary-container)",
```

> **The trap worth naming.** Pointing all four variants at one colour is only safe if no variant
> already rests there. The tonal button rests on `secondary-container`, so a theme that answers every
> variant with `secondary-container` makes a selected tonal button pixel-identical to an unselected
> one. Check each variant's rest colour against the colour you chose for its selected state.

**3. Outline width ramps with the size.** A stroke that reads as a hairline beside a small label is a
thread beside a large one. It is reserved on every variant, not only the outlined one, so switching
variant never shifts layout:

```csharp
OutlineWidthXs = "1px", OutlineWidthSm = "1px", OutlineWidthMd = "1px",
OutlineWidthLg = "2px", OutlineWidthXl = "3px",
```

`ButtonGroupTokens` follows the same idea and describes **two models**. A standard group is separate
buttons standing together - it contributes a gap and nothing else, because the corners stay the
buttons' own. A connected group is one seamed control and needs the whole seam vocabulary. Which
families ramp per size follows from whether a height can answer the question: a capsule is half the
segment's own height, so the outer and selected radii are one token each, while interior corners,
pressed corners and standard gaps are ramps a design language picks freely:

```csharp
StandardGapXs = "1.125rem", StandardGapSm = "0.75rem", StandardGapMd = "0.5rem",
StandardGapLg  = "0.5rem",  StandardGapXl = "0.5rem",
ConnectedGap = "0.125rem",              // one value at every size
ConnectedOverlap = "0",                 // negative pulls segments onto a shared border
ConnectedOuterRadius = "calc(var(--_flare-btn-height, var(--flare-btn-height-md)) / 2)",
ConnectedSelectedRadius = "calc(var(--_flare-btn-height, var(--flare-btn-height-md)) / 2)",
ConnectedInnerRadiusXs = "0.5rem",  /* ...Sm, Md, Lg, Xl */
ConnectedPressedRadiusXs = "0.25rem", /* ...Sm, Md, Lg, Xl */
ZActive = "1",
```

`--_flare-btn-height` is a local the button's own size class sets, so a token referring to it resolves
per size even though it is written once.

Finally, `ToggleButtonTokens` is now **only** the segmented `FlareToggleGroup` container - its border,
its two corner radii and the rule between segments. The buttons inside it are buttons and read the
button family for everything else, so if you are migrating a theme from before 0.16.0, the heights,
paddings, gap, radii, rest colours and disabled opacity that used to live there are deleted rather
than moved: their answers are the ones your `ButtonTokens` already gives.

### Icon motion: four tokens, and one trap (0.18.0)

`IconTokens` is new and `required`, so a theme built from scratch will not compile until it answers it.
It describes what happens when an icon CHANGES - `FlareIconView.Morph` and `FlareMorphIcon` - and is
inert until an app asks for a transition, so a theme that has no opinion can park it and move on.

```csharp
internal static readonly IconTokens Icon = new()
{
    MorphDuration = "var(--flare-motion-duration-spring-fast)",
    MorphEasing = "var(--flare-motion-easing-spring-fast)",
    MorphScale = "0.6",     // how far the glyph travels in the Scale mode
    MorphRotate = "90deg",  // the angle it turns through in the Rotate mode
};
```

**The trap is `MorphEasing`: it times the MOVEMENT only, never the cross-fade.** That split exists
because the easing worth putting here is usually a spring, and a spring overshoots - an eased fraction
past 1 drives opacity past its endpoint, so the fade finishes about a third of the way in and the
hand-off between the two glyphs reads as a pop rather than a transition. The fade rides
`--flare-motion-easing-standard` instead, which your theme already answers.

Two ways to say "no":

```csharp
MorphDuration = "0s",    // icon swaps stay instant everywhere, whatever an app asks for
MorphScale = "1",        // Scale and Rotate become plain cross-fades, and the modes
MorphRotate = "0deg",    // stay available without asserting motion your language does not have
```

Fluent UI 2 does exactly the second - a short decelerated fade with both geometry axes parked, because
that language has no icon overshoot in it. Themes derived from Material with `with { }` inherit its
values, and since those reference the motion scale rather than literals, each one resolves through its
own springs.

### Using Tokens in CSS

```css
/* Use var() references to tokens */
.my-component {
    background: var(--flare-color-primary);
    color: var(--flare-color-on-primary);
    border-radius: var(--flare-shape-medium);
    padding: var(--flare-input-padding);
    font-family: var(--flare-typescale-body-large-font);
    transition: all var(--flare-motion-duration-short2) var(--flare-motion-easing-standard);
}
```

## Theme Validation

```csharp
var validator = new ThemeValidator();
var errors = validator.Validate(myTheme);

if (errors.Count > 0)
{
    foreach (var error in errors)
        Console.WriteLine(error);
}
```

## Theme Import/Export

```csharp
// Export to JSON
string json = ThemeJsonSerializer.ExportTheme(myTheme);

// Import from JSON
ITheme importedTheme = ThemeJsonSerializer.ImportTheme(json);

// Export palette
string paletteJson = ThemeJsonSerializer.ExportPalette(myPalette);

// Import palette
Palette importedPalette = ThemeJsonSerializer.ImportPalette(paletteJson);
```

## CSS Architecture

### File Structure

```
MyTheme/
+-- css/
|   +-- my-theme-base.css      # Base reset, typography imports
|   +-- components/
|       +-- button.css          # Button overrides
|       +-- input.css           # Input overrides
|       +-- dialog.css          # Dialog overrides
|       +-- ...                 # Other component overrides
```

### Base CSS

```css
/* my-theme-base.css */
@import url('https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700&display=swap');

/* Theme-specific overrides */
.flare-theme-my-theme {
    --flare-typescale-body-large-font: 'Inter', sans-serif;
}
```

### Component Overrides

```css
/* components/button.css */
.flare-theme-my-theme .flare-btn {
    border-radius: var(--flare-shape-medium);
    font-family: var(--flare-typescale-label-large-font);
}
```

## High Contrast Mode

```csharp
var palette = new Palette
{
    Id = "my-brand",
    Name = "My Brand",
    Light = lightScheme,
    Dark = darkScheme,
    HighContrast = new ColorScheme
    {
        // High contrast colors (WCAG AAA)
        Primary = "#000000",
        OnPrimary = "#FFFFFF",
        // ... all roles with >=7:1 contrast ratio
    }
};
```

## Best Practices

1. **Use tokens, not hardcoded values** - All colors, sizes, and spacing should reference CSS variables
2. **Follow BEM naming** - `flare-{component}__{element}--{modifier}`
3. **Test both modes** - Light and dark should both look good
4. **Test RTL** - Layout should work in right-to-left languages
5. **Validate your theme** - Use `ThemeValidator` before registration
6. **Keep themes minimal** - Only override what you need; inherit the rest from defaults
7. **Document your tokens** - Add XML docs to custom token records
