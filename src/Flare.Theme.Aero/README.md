# Flare.Theme.Aero

Aero (glassy, translucent) light and dark theme for the
[Flare](https://github.com/jrfrigat/Flare) Blazor component library.

```sh
dotnet add package Flare.Theme.Aero
```

```csharp
// default theme...
builder.Services.AddFlare(opts => opts.DefaultTheme = new AeroTheme());
// ...or register and switch at runtime:
builder.Services.AddFlareTheme(new AeroTheme());
// await ThemeService.SetThemeAsync("aero");
```

## Windows Aero -> Flare

Aero is a Windows visual style rather than a published component library, so this table maps the
Win32 common controls it dressed onto the Flare components that stand in for them. If you are porting
a desktop-looking app - a 1C or Office 2010 style line-of-business UI - this is the column to read.

| Aero / Win32 | Flare | Notes |
| :-- | :-- | :-- |
| Command button | `FlareButton` | `Variant="ButtonVariant.Tonal"` is the classic grey command button |
| Default (accent) button | `FlareButton` | `Variant="ButtonVariant.Filled"` |
| Link label | `FlareLink` | |
| Edit box / masked edit | `FlareField`, `FlareMaskedField` | |
| Spin control (up-down) | `FlareNumericField` | |
| Combo box (dropdown / dropdown list) | `FlareCombobox`, `FlareSelect` | editable vs. list-only |
| Check box / Option button (radio) | `FlareCheckbox`, `FlareRadioGroup` | |
| Trackbar | `FlareSlider` | |
| Progress bar | `FlareProgress` | |
| Group box | `FlareCard` | `Variant="CardVariant.Outlined"` |
| Tab control | `FlareTabs` | |
| List view (details) | `FlareTable`, `FlareDataGrid` | |
| Tree view | `FlareTreeView` | |
| Toolbar / Rebar | `FlareToolbar` | |
| Status bar | `FlareStatusBar` | |
| Menu bar / context menu | `FlareMenuBar`, `FlareMenu` | |
| Balloon tip | `FlareTooltip` | |
| Task dialog | `FlareDialog`, `IMessageBoxService` | |
| Date/time picker, month calendar | `FlareDatePicker`, `FlareCalendar` | |
| Splitter | `FlareSplitter` | |

## What this theme changes beyond colour

- **Glossy vertical gradients** on every raised surface, with an inset top highlight and a 1px border.
- **The gradient swap IS the hover** - the theme sets its state-layer tokens to `transparent` rather
  than letting a translucent wash sit on top of the gloss.
- **A light-blue glow on hover** and a sunken inset on press, both from the era's chrome.
- All of it is built from theme tokens resolved through `color-mix`, so the gloss follows the palette
  and works in dark mode rather than being a fixed set of blues.

Requires `Flare.Components`. Repository & docs: https://github.com/jrfrigat/Flare  -  MIT licensed.
