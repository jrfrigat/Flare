# Flare for AI Agents

> [Русская версия ->](../ru/ai-agents.md) - [Getting Started](getting-started.md) - [API Reference](../../api/index.md)

A compact, high-signal reference for LLM coding agents that generate Blazor code with **Flare**.
Read this once and you can produce correct Flare markup without guessing. For exact parameter names
and types of any component, consult the generated [API Reference](../../api/index.md).

---

## 1. What Flare is

- A **.NET 8 / 9 / 10 Blazor** UI library (net10-first). Works in **both** Blazor WebAssembly and Blazor Server.
- **130+ components**, all prefixed `Flare*` (e.g. `FlareButton`, `FlareDataGrid`).
- **Token-based theming - you own the design system.** Components ship with zero baked-in styling; every
  color, shape, size and motion comes from a theme via one semantic token API. Seven preset design systems
  (Material Design 3 Expressive, MD3, MD2, Fluent UI 2, Aero, Liquid Glass, Visual Studio) ship as
  independent, optional packages - use one instantly or build a fully custom theme from scratch. You never
  write theme CSS.
- Component **variants/sizes/shapes are enums**, not strings (e.g. `Variant="ButtonVariant.Filled"`).

---

## 2. Hard rules (read first)

1. **Every component starts with `Flare`.** There is no `FlareIconButton`; an icon button is a
   `FlareButton` with an `Icon`. When unsure a component exists, check the catalog in section 9.
2. **Two-way binding uses `@bind-Value`** (Blazor standard). Read-only value uses `Value="..."`.
3. **Variants are enums.** Use `ButtonVariant.Filled`, `TypographyScale.HeadlineMedium`,
   `SnackbarSeverity.Success`, etc. Never pass a bare string where an enum is expected.
4. **Do not write custom CSS for layout/spacing/color.** Use Flare components, layout components
   (`FlareStack`, `FlareGrid`, `FlareSpacer`), and the token system. Inline `Style`/`Class` is allowed
   for one-off tweaks but prefer tokens.
5. **Wrap the app in `<FlareThemeProvider>`** and register at least one theme in DI, or nothing renders
   correctly.
6. **Overlay services need their provider** placed once in the layout: `<FlareDialogProvider />`,
   `<FlareSnackbarProvider />`, `<FlareMessageBoxProvider />`.
7. **Forms use Blazor's `EditForm`/`EditContext`** - Flare fields integrate with `DataAnnotationsValidator`
   out of the box.

---

## 3. Install

```sh
# Core (pulls in Flare.Components (+ the Abstractions/Theming/Infrastructure rings), 130+ components):
dotnet add package Flare.Blazor

# A theme - use a preset package below, or build your own (docs/en/theme-creation-guide.md):
dotnet add package Flare.Theme.MaterialDesign3Expressive
# others: Flare.Theme.FluentUI2, Flare.Theme.Aero, Flare.Theme.LiquidGlass, Flare.Theme.VisualStudio
```

Opt-in heavier components live in their own packages - add only what you use:

| Package | Component |
| :-- | :-- |
| `Flare.Components.Kanban` | `FlareKanban` |
| `Flare.Components.Media` | `FlareVideoPlayer` |
| `Flare.Components.RichTextEditor` | `FlareRichTextEditor` |
| `Flare.Components.Carousel` | `FlareCarousel` |
| `Flare.Components.Transfer` | `FlareTransfer` |
| `Flare.Components.QrCode` | `FlareQrCode` |
| `Flare.Components.IDE` | IDE shell (`FlareIdeLayout`, ribbon, docking, ...) |

---

## 4. Project setup (do all four)

**a) DI - `Program.cs`:**
```csharp
using Flare.Extensions;
using Flare.Theme.MaterialDesign3Expressive;
using Flare.Theme.FluentUI2;

builder.Services.AddFlare(opts =>
{
    opts.DefaultTheme   = new MaterialDesign3ExpressiveTheme();        // active design system
    opts.DefaultPalette = Md3Palettes.Violet;    // seed palette
    opts.DefaultMode    = ThemeMode.Auto;        // Light | Dark | Auto
});

// Register any additional themes you want available at runtime:
builder.Services.AddFlareTheme(new FluentUI2Theme());
```
`AddFlare` also registers `IDialogService`, `ISnackbarService` and `IMessageBoxService` - no extra
registration needed. Other entry points: `AddFlareTheme`, `AddFlarePalette`, `AddFlareIde`,
`AddFlareVersionCheck`.

**b) Styles - `index.html` (WASM) or `App.razor`/`_Host.cshtml` (Server), in `<head>`:**
```html
<script src="_content/Flare.Components/js/flare-bootstrap.js"></script>
<link rel="stylesheet" href="_content/Flare.Components/css/flare-components.css" />
<link rel="stylesheet" href="https://fonts.googleapis.com/css2?family=Material+Symbols+Rounded:opsz,wght,FILL,GRAD@20..48,100..700,0..1,-50..200" />
```
Theme CSS (fonts, base tokens) is wired up automatically by `FlareThemeProvider`; do not add it by hand.

**c) Imports - `_Imports.razor`:**
```razor
@using Flare.Components
@using Flare.Abstractions
@using Flare.Abstractions.Tokens
```

**d) Theme provider - wrap the router in `App.razor`:**
```razor
<FlareThemeProvider>
    <Router AppAssembly="@typeof(App).Assembly">
        <Found Context="routeData">
            <RouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)" />
        </Found>
    </Router>
</FlareThemeProvider>
```
Automatic dark mode follows the OS by default; disable with `RespectSystemColorScheme="false"`.

---

## 5. Core patterns

**Text & typography:**
```razor
<FlareText Typo="TypographyScale.HeadlineMedium">Title</FlareText>
<FlareText Typo="TypographyScale.BodyMedium">Body copy.</FlareText>
```

**Buttons:**
```razor
<FlareButton Variant="ButtonVariant.Filled" OnClick="Save">Save</FlareButton>
<FlareButton Variant="ButtonVariant.Outlined" Icon="delete">Delete</FlareButton>
<FlareButton Type="ButtonType.Submit" Variant="ButtonVariant.Filled">Submit</FlareButton>
```

**Icons** use Material Symbol names: `Icon="search"`, `Icon="delete"`, or the `<FlareIcon Name="..." />`
component.

**Two-way binding:**
```razor
<FlareTextField @bind-Value="_name" Label="Name" />
<FlareSelect @bind-Value="_role" Label="Role" Items="_roles" />
<FlareCheckbox @bind-Value="_agree">I agree</FlareCheckbox>
```

---

## 6. Services

```razor
@inject IDialogService Dialog
@inject ISnackbarService Snackbar
@inject IThemeService ThemeService

@code {
    private async Task Delete()
    {
        var ok = await Dialog.ConfirmAsync("Delete record", "This cannot be undone.", "Delete", "Cancel");
        if (ok == true)
            Snackbar.Show("Record deleted", SnackbarSeverity.Success);
    }

    // Theme switching:
    private Task UseNext() =>
        ThemeService.SetThemeAsync(ThemeService.Themes.First().Id);
}
```
`IThemeService` exposes `CurrentTheme`, `Themes`, and `SetThemeAsync(id)`. Remember the providers
(`<FlareDialogProvider/>`, `<FlareSnackbarProvider/>`, `<FlareMessageBoxProvider/>`) must exist once in
the layout.

---

## 7. Forms & validation

```razor
<EditForm Model="_model" OnValidSubmit="Submit">
    <DataAnnotationsValidator />

    <FlareTextField @bind-Value="_model.Name" Label="Name" />
    <FlareTextField @bind-Value="_model.Email" Label="Email" />
    <FlareSelect @bind-Value="_model.Role" Label="Role" Items="_roles" />

    <FlareButton Type="ButtonType.Submit" Variant="ButtonVariant.Filled">Submit</FlareButton>
</EditForm>
```
Validation messages render on the Flare field automatically via the shared `EditContext`.

---

## 8. Data grid (most-asked component)

```razor
<FlareDataGrid Items="_people" Filterable="true" Sortable="true" Pageable="true">
    <FlareColumn Field="@nameof(Person.Name)" Title="Name" />
    <FlareColumn Field="@nameof(Person.Age)" Title="Age" Align="ColumnAlign.End" />
    <FlareColumn Field="@nameof(Person.Email)" Title="Email" />
</FlareDataGrid>
```
The grid supports sorting, single/multi-column sort, filtering, paging, grouping, inline/batch editing,
selection, column reorder/resize/visibility, virtualization, and column bands. Check the
[API Reference](../../api/index.md) for the full `FlareDataGrid` / `FlareColumn` parameter set.

---

## 9. Component catalog

All real component names (consult the [API Reference](../../api/index.md) for each one's parameters):

**Inputs & forms:** `FlareTextField`, `FlareTextArea`, `FlareNumericField`, `FlarePasswordField`,
`FlareMaskedField`, `FlareOtpField`, `FlareTagField`, `FlareSelect`, `FlareMultiSelect`,
`FlareAutocomplete`, `FlareListbox`, `FlareCheckbox`, `FlareSwitch`, `FlareRadio`, `FlareRadioGroup`,
`FlareSlider`, `FlareRating`, `FlareDatePicker`, `FlareDateRangePicker`, `FlareTimePicker`,
`FlareDateTimePicker`, `FlareCalendar`, `FlareClockDial`, `FlareColorPicker`, `FlareFileUpload`,
`FlareDropZone`, `FlareSignaturePad`, `FlareField`, `FlareFormField`, `FlareForm`, `FlareFormBuilder`,
`FlareValidationSummary`.

**Buttons & actions:** `FlareButton`, `FlareButtonGroup`, `FlareSplitButton`, `FlareToggleButton`,
`FlareToggleGroup`, `FlareFloatingActionButton`, `FlareFloatingActionMenu`, `FlareFloatingActionMenuItem`,
`FlareShortcuts`, `FlareShortcutEntry`, `FlareClipboard`.

**Data display:** `FlareDataGrid` (+ `FlareColumn`, `FlareColumnBand`, `FlareColumnRow`,
`FlareDataGridPager`, `FlareDataGridQuickFilter`, `FlareDataGridFilterPresets`), `FlareTable`,
`FlareList`, `FlareListItem`, `FlareVirtualList`, `FlareTreeView`, `FlareDataTree`, `FlareTreeItem`,
`FlareTimeline`, `FlareTimelineItem`, `FlareChart`, `FlarePagination`, `FlareCard` (+ `FlareCardHeader`,
`FlareCardContent`, `FlareCardActions`, `FlareCardFooter`, `FlareCardMedia`), `FlarePaper`, `FlareBadge`,
`FlareChip`, `FlareChipGroup`, `FlareAvatar`, `FlareAvatarGroup`, `FlareSkeleton`, `FlareEmptyState`,
`FlareHighlighter`, `FlareImage`, `FlareIcon`, `FlarePropertyGrid`, `FlarePropertyGridItem`.

**Navigation:** `FlareAppBar`, `FlareNavMenu`, `FlareNavLink`, `FlareNavGroup`, `FlareTabs`, `FlareTab`,
`FlareStepper`, `FlareStep`, `FlareBreadcrumb`, `FlareMenu`, `FlareMenuItem`, `FlareMenuGroup`,
`FlareSubMenu`, `FlareMenuBar`, `FlareToolbar`, `FlareLink`, `FlareOnThisPage`, `FlareTableOfContents`,
`FlareTocLink`, `FlareBackstage`, `FlareBackstageItem`.

**Overlays & feedback:** `FlareDialog`, `FlareDialogProvider`, `FlareConfirmDialogProvider`,
`FlareSnackbarProvider`, `FlareMessageBoxProvider`, `FlareTooltip`, `FlarePopover`, `FlareOverlay`,
`FlareDrawer`, `FlareProgress`, `FlareAlert`.

**Layout & structure:** `FlareLayout` (+ `FlareLayoutAppBar`, `FlareLayoutContent`, `FlareLayoutDrawer`),
`FlareContainer`, `FlareGrid`, `FlareCol`, `FlareStack`, `FlareSpacer`, `FlareDivider`, `FlareSplitter`,
`FlareResizable`, `FlareAccordion`, `FlareAccordionPanel`, `FlareScrollTop`, `FlareInfiniteScroll`,
`FlareLazy`, `FlareMediaQuery`, `FlareHidden`.

**Content:** `FlareText`, `FlareMarkdown`, `FlareCodeBlock`.

**Theming:** `FlareThemeProvider`, `FlareThemeScope`, `FlareColorModeToggle`, `FlareColorCustomizer`.

**Office/IDE shell (`Flare.Components.IDE`):** `FlareIdeLayout`, `FlareDocumentTab`, `FlareDocumentTabs`,
`FlareToolPanel`, `FlareRibbon` (+ `FlareRibbonTab`, `FlareRibbonGroup`, `FlareRibbonButton`,
`FlareRibbonDropdown`, `FlareRibbonSeparator`), `FlareQuickAccessToolbar`, `FlareStatusBar`,
`FlareFormulaBar`, `FlareSheetTabs`.

**Separate packages:** `FlareKanban`, `FlareVideoPlayer`, `FlareRichTextEditor`, `FlareCarousel`,
`FlareTransfer`, `FlareQrCode`.

---

## 10. If you are editing Flare itself

When contributing to Flare's own source (not just consuming it), follow the project conventions:

- **ASCII-only** in code, XML-doc comments, and identifiers (no arrows, em-dashes, ellipses, or emoji).
  Localized resource (`.resx`) values are the only exception.
- **XML documentation is mandatory** on every public type, `[Parameter]`, method, and enum (it feeds
  this generated API site).
- **CSS lives in the global bundle** (`wwwroot/css/*.css`), not scoped; style via semantic tokens
  (`Css.Tokens.*`, `CssVarMap`, theme records). `FlareButton` is the canonical reference component.
- **Never hardcode user-visible strings** in the Gallery - add EN+RU resource entries.

See [Component Conventions](component-conventions.md) and [Architecture](architecture.md) for detail.

---

## 11. Where to look next

- **[API Reference](../../api/index.md)** - every public type, parameter, and enum (generated from XML docs).
- **[Getting Started](getting-started.md)** - the same setup with more prose.
- **[Architecture](architecture.md)** - modules, tokens, services, theming engine.
- **Gallery** - interactive live examples of every component (in `samples/Flare.Gallery`).
