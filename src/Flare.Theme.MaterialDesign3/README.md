# Flare.Theme.MaterialDesign3

Material Design 3 (baseline) light and dark theme for the
[Flare](https://github.com/jrfrigat/Flare) Blazor component library, including the built-in MD3
palettes.

```sh
dotnet add package Flare.Theme.MaterialDesign3
```

```csharp
// as the default theme...
builder.Services.AddFlare(opts =>
{
    opts.DefaultTheme   = new MaterialDesign3Theme();
    opts.DefaultPalette = Md3Palettes.Violet;
});
// ...or register alongside others, then switch at runtime:
builder.Services.AddFlareTheme(new MaterialDesign3Theme());
// await ThemeService.SetThemeAsync("md3");
```

## Material 3 -> Flare

| Material 3 | Flare | How you select it |
| :-- | :-- | :-- |
| Common buttons (elevated, filled, tonal, outlined, text) | `FlareButton` | `Variant="ButtonVariant.Elevated\|Filled\|Tonal\|Outlined\|Text"` |
| Toggle button | `FlareToggleButton` | `@bind-Toggled` |
| Segmented buttons | `FlareButtonGroup` of `FlareToggleButton` | `Connected="true"` |
| FAB, small/large FAB | `FlareFloatingActionButton` | `Size="FabSize.Sm\|Md\|Lg"` |
| Extended FAB | `FlareFloatingActionButton` | a labelled FAB IS the extended one: set `Label` |
| Icon buttons | `FlareIconButton` | `Variant` mirrors the button variants |
| Cards (elevated, filled, outlined) | `FlareCard` | `Variant="CardVariant.Elevated\|Filled\|Outlined"` |
| Chips (assist, filter, input, suggestion) | `FlareChip` | `Variant`, plus `@bind-Selected` for filter and `Closable` for input |
| Checkbox / Radio button / Switch | `FlareCheckbox`, `FlareRadioGroup`, `FlareSwitch` | |
| Sliders | `FlareSlider` | `Range="true"` for two handles |
| Text fields (filled, outlined) | `FlareField` and the typed fields | `Variant` |
| Menus | `FlareMenu` + `FlareMenuItem` | |
| Lists | `FlareList` + `FlareListItem` | |
| Dialogs (basic, full-screen) | `FlareDialog` | `Size="DialogSize.FullScreen"` |
| Snackbar | `ISnackbarService` | injected; no markup |
| Tooltips (plain, rich) | `FlareTooltip` | |
| Badges | `FlareBadge` | `Standalone` for the bare pill |
| Progress indicators (linear, circular) | `FlareProgress` | `Variant="ProgressVariant.Linear\|Circular"` |
| Navigation bar | `FlareBottomNav` | |
| Navigation rail | `FlareNavMenu` | `Mode="NavMenuMode.Rail"` |
| Navigation drawer (standard, modal) | `FlareLayoutDrawer` | `Variant="DrawerVariant.Persistent\|Temporary\|Responsive"` |
| Top app bar | `FlareLayoutAppBar` | |
| Tabs (primary, secondary) | `FlareTabs` | `Variant="TabsVariant.Primary\|Underline"` |
| Search | `FlareCombobox` | |
| Date / time pickers | `FlareDatePicker`, `FlareDateRangePicker`, `FlareTimePicker` | |
| Divider | `FlareDivider` | |
| Carousel | `FlareCarousel` | `Flare.Components.Carousel` package |

## Baseline, not Expressive

This package is M3 as specified before the Expressive update, and the difference is deliberate:

- **One button size.** Material's own M3-vs-Expressive table lists a single 40dp button; the XS/M/L/XL
  ramp arrives with Expressive. Flare's `Size` still works here - the steps are simply gentler.
- **No shape morph.** A pressed button does not square off and a selected toggle does not swap shape.
- **No wavy progress** and no spring motion.

If you want those, use `Flare.Theme.MaterialDesign3Expressive`; the palettes and the colour roles are
shared, so switching between them changes geometry and motion, not colour.

Requires `Flare.Components`. Repository & docs: https://github.com/jrfrigat/Flare  -  MIT licensed.
