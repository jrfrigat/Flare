# Flare.Theme.MaterialDesign3Expressive

Material Design 3 (Expressive) light and dark theme for the
[Flare](https://github.com/jrfrigat/Flare) Blazor component library, including the built-in MD3
palettes.

```sh
dotnet add package Flare.Theme.MaterialDesign3Expressive
```

```csharp
// as the default theme...
builder.Services.AddFlare(opts =>
{
    opts.DefaultTheme   = new MaterialDesign3ExpressiveTheme();
    opts.DefaultPalette = Md3Palettes.Violet;
});
// ...or register alongside others, then switch at runtime:
builder.Services.AddFlareTheme(new MaterialDesign3ExpressiveTheme());
// await ThemeService.SetThemeAsync("md3-expressive");
```

## Material 3 Expressive -> Flare

Expressive is not a separate component set: it is M3 with a wider size axis, a shape that reacts to
interaction, and springier motion. So the mapping below is M3's, and the Expressive column says what
this theme does that the baseline one does not.

| Material 3 | Flare | How you select it |
| :-- | :-- | :-- |
| Common buttons (elevated, filled, tonal, outlined, text) | `FlareButton` | `Variant="ButtonVariant.Elevated\|Filled\|Tonal\|Outlined\|Text"` |
| Button sizes (XS-XL) | `FlareButton` | `Size` - the full 5-rung ramp is Expressive's; baseline M3 ships one button |
| Toggle button | `FlareToggleButton` | `@bind-Toggled`; selection swaps the shape round<->square |
| Button groups (standard, connected) | `FlareButtonGroup` | `Connected`, `Vertical` |
| Segmented buttons | `FlareButtonGroup` of `FlareToggleButton` | `Connected="true"` |
| FAB, small/large FAB | `FlareFloatingActionButton` | `Size="FabSize.Sm\|Md\|Lg"` |
| Extended FAB | `FlareFloatingActionButton` | a labelled FAB IS the extended one: set `Label` |
| FAB menu | `FlareFloatingActionMenu` + `FlareFloatingActionMenuItem` | |
| Icon buttons | `FlareIconButton` | `Variant` mirrors the button variants |
| Split button | `FlareSplitButton` | |
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
| Progress indicators (linear, circular) | `FlareProgress` | `Variant="ProgressVariant.Linear\|Circular"`; `Wavy` is Expressive's |
| Navigation bar | `FlareBottomNav` | |
| Navigation rail | `FlareNavMenu` | `Mode="NavMenuMode.Rail"` |
| Navigation drawer (standard, modal) | `FlareLayoutDrawer` | `Variant="DrawerVariant.Persistent\|Temporary\|Responsive"` |
| Top app bar | `FlareLayoutAppBar` | |
| Tabs (primary, secondary) | `FlareTabs` | `Variant="TabsVariant.Primary\|Underline"` |
| Search | `FlareCombobox` | |
| Date pickers | `FlareDatePicker`, `FlareDateRangePicker` | |
| Time pickers | `FlareTimePicker` | |
| Carousel | `FlareCarousel` | `Flare.Components.Carousel` package |
| Divider | `FlareDivider` | |
| Bottom sheet / side sheet | `FlareDialog` | `Size="DialogSize.FullWidth"` plus placement |
| Toolbar | `FlareToolbar` | |

Material components Flare has no equivalent for: none of the above is missing, but Flare adds many
that M3 does not specify (data grid, tree, kanban, ribbon, query builder). Those take their colour and
shape from the same tokens and simply have no Material counterpart to compare against.

## What this theme changes beyond colour

- **The size ramp is the spec's**, 32/40/56/96/136dp for buttons, not a gentler five steps.
- **Shape reacts to interaction**: a pressed button squares off, and a selected toggle swaps to the
  opposite shape, both on a spring rather than a linear ease.
- **The button group trades width on press** - the pressed segment grows and its neighbours give up
  exactly as much, so the group's own width never changes.
- **Wavy progress indicators** and the spring motion primitives (`MotionTokens.EasingSpring*`).

Requires `Flare.Components`. Repository & docs: https://github.com/jrfrigat/Flare  -  MIT licensed.
