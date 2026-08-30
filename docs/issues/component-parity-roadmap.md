# Component parity roadmap: what Flare still has to build

**Status: OPEN - this is the plan of record for closing the component-inventory gap against the four
reference frameworks.** Every child issue linked below is written to be built *in Flare's terms* - token
records with no defaults, CSS in the global bundle, satellite packages for the heavy widgets, JS only
behind a port. None of them is a port of a competitor's code.

## How the gap was measured

On 2026-08-23 the component inventory of five repositories was read from source - not from documentation
sites, which overstate and understate in different directions:

| Framework | Repository read | Counted |
| :-- | :-- | :-- |
| Flare | `src/Flare.Components*` (9 packages) | ~150 public components |
| MudBlazor | `src/MudBlazor/Components` | ~150 |
| Radzen | `Radzen.Blazor/*.razor` | ~230 (about 60 are chart series / gauge scales) |
| Blazorise | `Source/Blazorise/Components` + 45 extension packages | ~120 core + extensions |
| Fluent UI Blazor | `src/Core/Components` | ~70 |

**Caveat that matters:** this was an inventory of *what exists*, by component type, not an audit of how
deep each one is. A row that reads "+ / +" can still hide a large capability difference; that comparison
is what the `component-audit` skill does per component. Nothing below is justified by "the competitor has
one" alone - each issue states what the component is *for*.

Flare already leads on primitives. It matches MudBlazor's breadth, exceeds Fluent UI's, and owns a whole
class nobody else has (the IDE shell: Ribbon, Backstage, QuickAccessToolbar, DocumentTabs, SheetTabs,
StatusBar, ToolPanel, MenuBar, FormulaBar). The remaining gap is concentrated in **heavy data widgets and
in the chart engine's breadth**, plus a short tail of small components and integrations.

## Rules every item on this roadmap follows

These are the difference between "Flare has a Scheduler" and "Flare copied a Scheduler". A child issue
that cannot satisfy all eight does not get built as specified - it gets re-specified.

1. **Tokens first, no defaults.** Every new component ships a record in
   `Flare.Abstractions/Tokens/Components/*Tokens.cs` whose properties are `required` or a neutral
   sentinel. No literal `16px`, no `Vars.Var(Color.Primary)` fallback. `ThemeIndependenceTests` is the
   guard. An unthemed new component must render completely unstyled - that is the plugin model working.
2. **Maximum token surface.** The rule is "a theme can repaint and re-shape the whole component". For the
   data widgets this means the timeline, the grid lines, the band fills and the drag affordances are all
   token-driven, not just the text color.
3. **Shared tokens across similar components.** Scheduler / Gantt / TileLayout are all timeline-or-cell
   surfaces; they reuse one family of surface, grid and selection tokens rather than inventing three.
   Same rule that already applies to nav-like and tab-like components.
4. **CSS in the global bundle**, `wwwroot/css/<name>.css`, never scoped; class names registered in
   `Flare.Abstractions/Css/Classes`. `FlareButton` remains the reference implementation.
5. **Minimum JS, and only behind a port.** Layout math belongs in C# and SVG. Where the browser is
   genuinely required (file transfer, canvas, PDF), the contract is an interface in `Flare.Abstractions`
   and the implementation is in `Flare.Infrastructure`. `Flare.Components` never gains a service
   implementation and never takes a third-party SDK reference.
6. **Heavy widgets ship as satellite packages** - `Flare.Components.Pivot`, `.Scheduler`, `.Gantt` -
   exactly as Kanban, Query, Media, IDE and RichTextEditor already do. The core package does not grow a
   Scheduler.
7. **Reuse before invent.** The DataGrid column/filter/export engine, the chart SVG renderer, the Popover
   collision engine, `FlareFieldChrome`, `FlareResizable`, the `FlareCodeBlock` two-layer editing contract
   and the Query package's condition model are all existing assets. Each issue names what it reuses.
8. **Definition of done is the same everywhere:** XML docs on every public type, `[Parameter]` and method;
   user-visible strings localized through the EN and RU resx pair; a Gallery demo page plus regenerated
   API reference; bUnit tests; and a visual check in the Gallery, because the Gallery is how this library
   is reviewed.

## The plan

Ordered by dependency first, then by cost-to-value. The three foundation items unblock the rest.

### Phase 1 - foundation and cheap parity

| # | Issue | Size | Unblocks |
| :-- | :-- | :-- | :-- |
| 1 | ~~Chart tokenization~~ - DONE, `ChartTokens` ships 58 tokens | M | gauges, all new series, treemap/sankey |
| 2 | ~~Upload transfer port~~ - DONE. `Uploader` takes a delegate, not a URL: Flare owns the queue, concurrency, cancel, retry, remove and the row template, the application owns the wire. `ChunkSize` was dropped - chunking is the wire, and the wire is the caller's | M | the most visible hole in an existing component |
| 3 | ~~Gauge family~~ - DONE. `FlareGauge` radial / arc / linear; bands are `FlareZone` children rather than a gauge-only range type, and the arc math moved to a shared `ArcGeometry` the chart now uses too | M | dashboards |
| 4 | ~~Barcode~~ - DONE. `Flare.Components.Barcode` ships seven symbologies as pure encoders tested against their published vectors. The QR version cap the same file carried was already lifted (versions 1-40) | S | - |
| 5 | [Markdown editor](markdown-editor.md) - edit mode for `FlareMarkdown` on the two-layer contract | S | - |
| 6 | ~~Small parity batch~~ - DONE. All five: `FlareBusy`, `FlareTimeSpanPicker`, `ShowStrength` on the password field, `FlareNavigationGuard`, `FlarePullToRefresh` | S each | - |

### Phase 2 - the heavy data widgets

| # | Issue | Size | Notes |
| :-- | :-- | :-- | :-- |
| 7 | [PivotGrid](pivot-grid.md) | L | Reuses the DataGrid rendering and export path and the Query condition model. Radzen and Blazorise both have one; Mud and Fluent do not. |
| 8 | [Scheduler](scheduler.md) | XL | The single largest gap. Today `FlareCalendar` is a month grid of day-level chips. Radzen has 7 views, Blazorise has a recurrence editor. |
| 9 | [Gantt](gantt.md) | L | Built on the Scheduler's timeline engine, not beside it. Blocked by #8. |

### Phase 3 - surfaces and the rest of the chart engine

| # | Issue | Size | Notes |
| :-- | :-- | :-- | :-- |
| 10 | [TileLayout](tile-layout.md) | M | Draggable, resizable dashboard grid. Reuses `FlareResizable` and the Kanban drag model. Radzen-only today. |
| 11 | [Chart series expansion](chart-series-expansion.md) | L | Financial (candlestick / OHLC / high-low / box plot), waterfall / funnel / pyramid, range series, trendlines and moving averages, treemap and sankey. Blocked by #1. |
| 12 | [Spreadsheet surface](spreadsheet-surface.md) | L | Flare already owns `FlareFormulaBar` and `FlareSheetTabs` with nothing behind them. Finishing this completes the IDE story rather than chasing Radzen. |

### Phase 4 - integration satellites

| # | Issue | Size | Notes |
| :-- | :-- | :-- | :-- |
| 13 | [Integration satellites](integration-satellites.md) | M each | Map, PDF viewer, image cropper, chat/assistant surface. All four are provider-shaped, so all four are a port in `Abstractions` plus a satellite package - never a vendor SDK inside `Flare.Components`. |

## Deliberately not planned

Recorded so the decision is not re-litigated every time somebody re-reads a competitor's component list.

- **Login form, Gravatar, SSRS viewer, reporting designer** (Radzen, Blazorise). These are application
  features and server integrations wearing a component's clothes. Flare ships the primitives they would be
  assembled from. If a report *viewer* is ever wanted, it enters through Phase 4 as a port.
- **Lottie, Captcha, Animate** (Blazorise). Thin wrappers over third-party JS. They would put a vendor
  dependency in the dependency graph for something an application can add in ten lines. Revisit only on a
  concrete request.
- **On-screen / virtual keyboard** (Radzen, Blazorise). Genuinely niche - kiosk builds. Cheap to add later
  on the existing overlay and token machinery; not worth a slot ahead of the Scheduler.
- **DockLayout** (Blazorise). `FlareIdeLayout` plus `FlareSplitter` plus `FlareToolPanel` already covers
  the docking story, and covers it in the IDE idiom Flare owns.
- **RouterTabs** (Blazorise). `FlareDocumentTabs` plus `FlareLinkTabs` cover it; an auto-route-tracking
  variant is a parameter on the existing components if it is ever asked for, not a new component.
