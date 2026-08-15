# Flare.Theme.MaterialDesign2

Material Design 2 light and dark theme for the [Flare](https://github.com/jrfrigat/Flare) Blazor
component library, including the built-in MD2 palettes.

```sh
dotnet add package Flare.Theme.MaterialDesign2
```

```csharp
// as the default theme...
builder.Services.AddFlare(opts =>
{
    opts.DefaultTheme   = new MaterialDesign2Theme();
    opts.DefaultPalette = Md2Palettes.Purple;
});
// ...or register alongside others, then switch at runtime:
builder.Services.AddFlareTheme(new MaterialDesign2Theme());
// await ThemeService.SetThemeAsync("md2");
```

## Material 2 -> Flare

M2 has a smaller vocabulary than M3, so several rows map one M2 component onto a Flare component that
also serves an M3 concept. Where M2 and M3 disagree about a name, the M2 name is the one listed.

| Material 2 | Flare | How you select it |
| :-- | :-- | :-- |
| Buttons (contained, outlined, text) | `FlareButton` | `Variant="ButtonVariant.Filled\|Outlined\|Text"` - "contained" is `Filled` |
| Toggle buttons | `FlareToggleButton`, `FlareButtonGroup` | |
| Floating action button (regular, mini, extended) | `FlareFloatingActionButton` | `Size`; set `Label` for the extended one |
| Icon buttons | `FlareIconButton` | |
| Cards | `FlareCard` | M2 draws one card; `Variant` still offers M3's three |
| Chips (input, choice, filter, action) | `FlareChip` | `@bind-Selected`, `Closable` |
| Checkboxes / Radio buttons / Switches | `FlareCheckbox`, `FlareRadioGroup`, `FlareSwitch` | |
| Sliders (continuous, discrete, range) | `FlareSlider` | `Step` for discrete, `Range="true"` for two handles |
| Text fields (filled, outlined) | `FlareField` and the typed fields | `Variant` |
| Menus | `FlareMenu` + `FlareMenuItem` | |
| Lists | `FlareList` + `FlareListItem` | |
| Dialogs | `FlareDialog` | |
| Snackbars | `ISnackbarService` | injected; no markup |
| Tooltips | `FlareTooltip` | |
| Badges | `FlareBadge` | |
| Progress indicators (linear, circular) | `FlareProgress` | `Variant` |
| Bottom navigation | `FlareBottomNav` | |
| Navigation drawer (standard, modal, bottom) | `FlareLayoutDrawer` | `Variant` |
| App bars: top | `FlareLayoutAppBar` | |
| Tabs (fixed, scrollable) | `FlareTabs` | scrolling is automatic when the bar overflows |
| Banners | `FlareAlert` | `Variant="AlertVariant.Filled\|Outlined\|Text"` |
| Data tables | `FlareTable`, `FlareDataGrid` | the grid adds sorting, paging, grouping and editing |
| Backdrop | `FlareOverlay` | |
| Image lists | `FlareGrid` of `FlareImage` | |
| Pickers (date, time) | `FlareDatePicker`, `FlareTimePicker` | |
| Dividers | `FlareDivider` | |

## What this theme changes beyond colour

- **Flatter and squarer.** M2's shape scale tops out well below M3's, and the pill-shaped button is
  not the default - buttons are 4dp rounded rectangles.
- **Elevation carries the hierarchy** rather than tonal surface colour: M2 leans on shadows where M3
  leans on surface tints.
- **Uppercase button labels**, which M3 abandoned.

Requires `Flare.Components`. Repository & docs: https://github.com/jrfrigat/Flare  -  MIT licensed.
