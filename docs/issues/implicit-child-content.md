# Implicit child content reported to throw at runtime

**Status: DONE. Tier 0. Reported item 7. Every component that renders caller text takes a fragment for
it, every collision-prone slot name is renamed, and two guards keep both true.**

## The report

> `<FlareTab Label="test"><MyLocalComponent /></FlareTab>` throws at runtime, while
> `<FlareTab Label="test"><ChildContent><MyLocalComponent /></ChildContent></FlareTab>` works.

## What was tested

Both forms were compiled in the test project and the generated Razor was diffed:

```
diff ReproTabImplicit_razor.g.cs ReproTabExplicit_razor.g.cs
(no output)
```

The two forms produce **byte-identical** code - one `AddAttribute(seq, "ChildContent", RenderFragment)`
either way. Both render, and both were also rendered through `FlareTabs` in bUnit with a component
child. There is therefore no runtime difference between the two spellings *for a child whose tag name
does not collide with a parameter*, and the report as literally written cannot be reproduced.

## Checked against the reporting application, and it does not explain that one

The application was `techvill/OrderingPlatform` on Flare 0.18.1. Everything checkable there was checked:

- **Its own component names**: App, Config, Home, IssueList, MainLayout, NavMenu, ParamTable,
  RouteEditDialog, ScopeEditDialog, SzRcDaysTable, TaskCard, TaskCreateDialog, TaskDailyMetricsTable,
  TaskGraphGrid, TaskPicker, TaskResultGrid, TaskScheduleGrid, Tasks. **None collides** with any
  `FlareTab` parameter (`Label`, `ChildContent`, `LeadingIcon`, `Disabled`, `Closeable`, `Badge`,
  `BadgeColor`, `BadgeDot`, `Tooltip`, `OnClick`).
- **The implicit form compiles there**: a scratch page written into that project with
  `<FlareTab Label="..."><TaskDailyMetricsTable Rows="[]" /></FlareTab>` built with zero errors.
- **It also works at runtime**: the same shape rendered in the Gallery produces byte-identical panel
  content either way.
- **The file still uses the implicit form in two places** (`Tasks.razor`) and those work; the explicit
  form appears in `TaskCard.razor` and `Config.razor`.

So the reported symptom is not reproducible from that code, and the collision below - while real - is not
what happened there. What *was* wrong in that application was unrelated and is now fixed: it had no
`<script src=".../flare-components.js">` in its `index.html`, which is why `Frozen` on a DataGrid column
threw (see `js-layer-audit.md`, item A). It is possible the two reports were the same debugging session
and the `<ChildContent>` wrapping was a change that happened to coincide.

**To take this further, the exception text is still what is needed** - specifically whether it names a
component, a parameter or a null reference.

## The collision that IS real, and is a likely cause elsewhere

Razor decides "these children are named content slots" by matching the child *tag name* against the
component's `RenderFragment` parameters. So a user component whose name equals a slot name is silently
swallowed:

```razor
<FlareCard>
    <Header />          @* the app's own Header component *@
</FlareCard>
```

Razor binds this to the `Header` *parameter*, not to `ChildContent`. The app's component never renders,
and - depending on the slot - the surrounding component can then dereference something it expects the
child content to have provided. Wrapping in `<ChildContent>` fixes it, which is exactly the shape of the
report. `FlareTab`'s slots are `ChildContent` and `LeadingIcon`; a component named `LeadingIcon` is
unlikely, but `Header`, `Footer`, `Actions`, `Content`, `Title`, `Icon`, `Label` and `Body` are all slot
names somewhere in Flare and all are plausible app component names.

A component *name* is checked before a parameter name, so this only bites when the app component and the
slot share a name - but when it bites there is no diagnostic at all.

**Measured: 32 bare-noun slot names across the library**, any of which an application could plausibly
use as a component name. Ordered by how many components carry them:

| Slot | Components |
| :-- | :-- |
| `Icon` | Alert, FloatingActionButton, Dialog, EmptyState, FileUploadZone, BottomNavItem, NavLink, Step |
| `Header` | OptionList, Drawer, NavMenu |
| `Leading` / `Trailing` | AppBar, ListItem |
| `Avatar` | CardHeader, Chip |
| `Template` | ChipStrip, Column |
| `Zones` | Progress, Slider |

plus `Action`, `Actions`, `Badge`, `Counter`, `Empty`, `Fallback`, `Footer`, `Placeholder`, `Activator`,
`Composite`, `MenuItems`, `MenuButton` and the icon-state slots on Checkbox/Switch/ToggleButton.

`Icon` is the dangerous one: eight components expose it, and an application component called `Icon` is
about as likely as a name gets.

## Why nothing ever fails loudly

Found while writing a Gallery demo: `<FlareChip>42</FlareChip>` compiled with **zero errors and zero
warnings**, and rendered an empty chip.

Blazor's rule is that child content on a component with no `ChildContent` parameter is an error. Flare
never sees that error, because `FlareComponentBase` declares

```csharp
[Parameter(CaptureUnmatchedValues = true)]
public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }
```

and **every** component inherits it. The generated Razor confirms what happens - the fragment is emitted
as an untyped attribute rather than a component parameter:

```csharp
__builder.OpenComponent<FlareChip>(0);
__builder.AddComponentParameter(1, nameof(FlareChip.Label), "kept");
__builder.AddAttribute(2, "ChildContent", (RenderFragment)(__builder2 => {   // <- not a parameter
    __builder2.AddContent(3, "this text has nowhere to go");
}));
```

At runtime it lands in `AdditionalAttributes` as an unmatched attribute, gets splatted onto the root
element, and is dropped because a `RenderFragment` is not an attribute value. The catch-all turned a
compile-time error into silence, library-wide.

This is the same shape as `<FlareProgress Max="1" />`, where a parameter that does not exist became a
DOM attribute instead of an error. The catch-all is worth having - it is how `data-*`, `aria-*` and
`@onclick` reach the root element - but it means **Flare gets no compiler help at all** for a wrong
parameter name or an unaccepted child, and the library has to close that gap itself.

## Measured, corrected, and now enforced

A first pass grepped the `.razor` files and found 21 components. That was an **undercount**: the whole
field family declares `Label` on `FlareFieldBase`, not in its own file, so a text search cannot see it.
`CallerTextSlotTests` walks the assembly by reflection instead and found 15 more, the field family among
them.

The guard states the rule: *a component that renders a caller-supplied string must also accept a
`RenderFragment` for it.* The slot's name follows what the library already did before the rule was
written down - `ChildContent` when the caller's text is the whole of what the component renders,
`XxxContent` when it is one named part (`FlareAppBar.TitleContent` was the precedent).

**Given a slot:**

| Component | Slot | Why that name |
| :-- | :-- | :-- |
| `FlareChip`, `FlareCheckbox`, `FlareRadio`, `FlareSwitch`, `FlareDivider`, `FlareLinkTab` | `ChildContent` | the label is the whole of what it renders |
| `FlareSlider` | `LabelContent` | the label is one part among track, value and zones |
| The 13 fields (`FlareField`, `FlareTextField`, `FlareNumericField`, `FlareMaskedField`, `FlarePasswordField`, `FlareTagField`, `FlareTextArea`, `FlareOtpField`, `FlareSelect`, `FlareMultiSelect`, `FlareCombobox`, `FlareDatePicker`, `FlareTimePicker`, `FlareDateTimePicker`) | `LabelContent` | one parameter on `FlareFieldBase`, forwarded through `FlareFieldChrome` - a field's content is its control, not its label |

**Deliberately not given one**, each with its reason in the guard's `NotAMarkupSlot` table: `FlareRating`
(the string is an `aria-label`), `FlareHighlighter` (the string is the haystack it searches),
`FlareAvatar` (initials are derived, not rendered verbatim), `FlareFloatingActionMenuItem` (the same
string is both the visible label and the button's accessible name), and the components with two text
parameters where a single slot could not say which - `FlareEmptyState`, `FlareChart`,
`FlareDateRangePicker`.

**The DataGrid column header** was the last gap and is now closed: `FlareColumnBase.TitleContent` (so
bands get it too), carried through `DataGridColumn<TItem>` and painted by both the plain and the banded
header path. `Title` deliberately stays required and unchanged, because it is not only the heading - it
is the column's identity and its name in the export, the filter menu, the column picker, the aggregate
rows and the edit dictionaries, all of which need text. A fragment that replaced the string would have
silently emptied every one of those; one of the tests pins exactly that.

## Work

1. ~~Get the exception text for the reported case.~~ Superseded: the mechanism above explains both the
   reported symptom and the silent-swallow class it belongs to. A slot-name collision produces the
   reported "works only with explicit `<ChildContent>`" behaviour; the catch-all is why neither form
   ever produces a diagnostic.
2. ~~Give every component that renders caller text a fragment for it.~~ Done; see the table above.
3. ~~A guard test, because the compiler will never report this one.~~ Done: `CallerTextSlotTests`. It is
   static rather than render-based on purpose - rendering 181 components generically founders on their
   required parameters, while reflection sees inherited ones, which is exactly what the grep missed.
4. ~~The DataGrid column header slot.~~ Done - `FlareColumnBase.TitleContent`, on both the plain and
   the banded header path.
5. ~~Audit the slot names.~~ Done. 21 slots across 19 components renamed to `XxxContent`, guarded by
   `SlotNameTests`. `Icon` was the sharp one and it turned out to be two problems: eight components
   exposed it as a fragment while eight others exposed `Icon` as a `FlareIcon` VALUE, so the same
   parameter name meant different things depending on which component you were reading. `Icon` now
   always means an icon value. Deliberately kept: `Columns` / `Grouping` (collection slots every grid
   spells this way), `Leading` / `Trailing` / `Zones` / `Composite` / `Activator` (positional or domain
   terms nobody names a component after).
6. ~~Write the naming rule into the component conventions.~~ Done, EN and RU, section 4a.
