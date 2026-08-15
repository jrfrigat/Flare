# Flare.Theme.FluentUI2

Fluent UI 2 light and dark theme for the [Flare](https://github.com/jrfrigat/Flare) Blazor component
library.

```sh
dotnet add package Flare.Theme.FluentUI2
```

```csharp
// default theme...
builder.Services.AddFlare(opts => opts.DefaultTheme = new FluentUI2Theme());
// ...or register and switch at runtime:
builder.Services.AddFlareTheme(new FluentUI2Theme());
// await ThemeService.SetThemeAsync("fluent2");
```

## Fluent UI 2 -> Flare

Fluent names several controls differently from Material, and in two places it splits what Material
treats as one component. The Flare column is the component; the third column is what makes it read as
Fluent rather than as Material.

| Fluent UI 2 | Flare | How you select it |
| :-- | :-- | :-- |
| Button (primary, secondary, outline, subtle, transparent) | `FlareButton` | `Variant="ButtonVariant.Filled\|Tonal\|Outlined\|Text"` - "primary" is `Filled`, "subtle" is `Text` |
| Compound button | `FlareButton` | a second line of content inside `ChildContent` |
| Toggle button | `FlareToggleButton` | `@bind-Toggled` - Fluent changes colour rather than shape |
| Split button | `FlareSplitButton` | |
| Menu button | `FlareButton` + `FlareMenu` | |
| Card | `FlareCard` | |
| Badge / Counter badge / Presence badge | `FlareBadge`, `FlareAvatar` | presence is `FlareAvatar`'s status dot |
| Avatar / Avatar group | `FlareAvatar`, `FlareAvatarGroup` | |
| Checkbox / Radio group / Switch | `FlareCheckbox`, `FlareRadioGroup`, `FlareSwitch` | |
| Slider | `FlareSlider` | |
| Input / Textarea | `FlareField`, `FlareTextArea` | |
| Combobox / Dropdown | `FlareCombobox`, `FlareSelect` | Fluent's Combobox is editable, Dropdown is not |
| SpinButton | `FlareNumericField` | |
| Field (label + hint + validation around a control) | `FlareField` | the chrome is built in rather than a separate wrapper |
| Menu / MenuItem | `FlareMenu`, `FlareMenuItem` | |
| Toolbar | `FlareToolbar` | |
| Dialog / Drawer | `FlareDialog`, `FlareLayoutDrawer` | |
| Popover / Tooltip | `FlarePopover`, `FlareTooltip` | |
| Toast | `ISnackbarService` | injected; no markup |
| MessageBar | `FlareAlert` | |
| ProgressBar / Spinner | `FlareProgress` | `Variant="ProgressVariant.Linear\|Circular"` |
| TabList | `FlareTabs` | |
| Breadcrumb | `FlareBreadcrumb` | |
| Accordion | `FlareAccordion` | |
| DataGrid / Table | `FlareDataGrid`, `FlareTable` | |
| Tree | `FlareTreeView` | |
| Link | `FlareLink` | |
| Divider | `FlareDivider` | |
| Rating | `FlareRating` | |
| Persona | `FlareAvatar` + `FlareText` | no single component; compose it |

## What this theme changes beyond colour

- **Discrete state fills, not a translucent wash.** Where Material paints a `currentColor` overlay at a
  state opacity, Fluent assigns a flat fill per state through the same `--flare-state-*-layer` tokens.
- **Disabled repaints instead of dimming.** Every component's `DisabledOpacity` is `1` here, and the
  flat disabled palette does the work - Material fades the whole element to 38%.
- **Focus is a stroke that coexists with hover**, which is why the theme sets
  `--flare-state-focus-hover-layer` to its hover fill rather than to its focus one.
- **Squarer corners** and a tighter type ramp than either Material generation.

Requires `Flare.Components`. Repository & docs: https://github.com/jrfrigat/Flare  -  MIT licensed.
