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

var myFluent = new Fluent2Theme().Derive(
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

```csharp
using Flare.Abstractions;
using Flare.Abstractions.Tokens;

public sealed class MyTheme : ITheme
{
    public string Id => "my-theme";
    public string DisplayName => "My Custom Theme";
    
    public DesignTokens Design => new()
    {
        FocusRing = "2px solid var(--flare-color-primary)",
        Typography = MyTypography.Tokens,
        Shape = MyShape.Tokens,
        Elevation = MyElevation.Tokens,
        Motion = MyMotion.Tokens,
        State = MyState.Tokens,
        Button = MyButton.Tokens,
        Input = MyInput.Tokens,
        Dialog = MyDialog.Tokens,
        // ... all component tokens
    };
    
    public string DefaultPaletteId => "my-brand";
    public IReadOnlyList<string> StyleAssets => [
        "_content/MyApp/css/my-theme.css"
    ];
}
```

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
    opts.DefaultTheme = new Md3Theme();
    opts.UseDynamicPalette = true;               // registers the "dynamic" palette
    opts.DynamicPaletteFallbackSeed = "#6750A4"; // seed used when the OS accent is unavailable
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

**Browser support / fallback.** The accent comes from the CSS `AccentColor` system color (modern
Edge/Chrome/Safari/Firefox; on Android Chrome it reflects Material You). Engines that don't expose it
fall back to `DynamicPaletteFallbackSeed`. The web exposes no deeper "wallpaper palette" API, so
`AccentColor` is the accent source.

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
| `ButtonTokens` | Button geometry | ~30 fields |
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
