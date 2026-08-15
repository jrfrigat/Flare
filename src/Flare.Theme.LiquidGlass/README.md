# Flare.Theme.LiquidGlass

Liquid Glass (frosted, depth-layered) light and dark theme for the
[Flare](https://github.com/jrfrigat/Flare) Blazor component library.

```sh
dotnet add package Flare.Theme.LiquidGlass
```

```csharp
// default theme...
builder.Services.AddFlare(opts => opts.DefaultTheme = new LiquidGlassTheme());
// ...or register and switch at runtime:
builder.Services.AddFlareTheme(new LiquidGlassTheme());
// await ThemeService.SetThemeAsync("liquid-glass");
```

## Liquid Glass -> Flare

Liquid Glass is an aesthetic rather than a component catalogue: translucent, depth-layered surfaces
with a specular sheen, in the Apple-platform tradition. There is no separate component vocabulary to
map, so the table below is the platform-control vocabulary the look belongs to.

| Apple-platform control | Flare | Notes |
| :-- | :-- | :-- |
| Button (prominent, bordered, plain) | `FlareButton` | `Variant="ButtonVariant.Filled\|Outlined\|Text"` |
| Toggle (switch) | `FlareSwitch` | |
| Segmented control | `FlareButtonGroup` of `FlareToggleButton` | `Connected="true"` |
| Stepper | `FlareNumericField` | |
| Slider | `FlareSlider` | |
| Picker / Menu | `FlareSelect`, `FlareMenu` | |
| Search field | `FlareCombobox` | |
| Text field / Text editor | `FlareField`, `FlareTextArea` | |
| Sheet / Alert | `FlareDialog`, `IMessageBoxService` | |
| Popover | `FlarePopover` | |
| Toolbar | `FlareToolbar` | |
| Tab bar | `FlareBottomNav` | |
| Sidebar (NavigationSplitView) | `FlareLayoutDrawer` + `FlareNavMenu` | |
| List / Section | `FlareList`, `FlareCard` | |
| Disclosure group | `FlareAccordion`, `FlareCollapse` | |
| Progress view / Gauge | `FlareProgress`, `FlareMeter` | |
| Badge | `FlareBadge` | |
| Date picker | `FlareDatePicker` | |

## What this theme changes beyond colour

- **Translucent capsules with a lensing rim** - a light gradient sheen over a semi-transparent fill,
  plus an inset highlight and a soft coloured shadow.
- **No backdrop blur.** This is deliberate and performance-first: the depth reads from layering and
  the sheen, not from a `backdrop-filter` that costs a compositor pass per surface.
- **The sheen IS the feedback** - the theme sets its state-layer tokens to `transparent` and expresses
  hover and press with saturation and brightness instead of an overlay.
- **A liquid squash on press** (`scale(0.96)`) on every button variant.

Requires `Flare.Components`. Repository & docs: https://github.com/jrfrigat/Flare  -  MIT licensed.
