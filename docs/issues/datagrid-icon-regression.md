# DataGrid icons render as literal text after the SVG icon migration

**Status: OPEN. Tier 0. Reported item 1.**

## What is wrong

Three call sites in `FlareDataGrid` still emit a Material Symbols *ligature span* - a `<span>` carrying
`class="material-symbols-rounded"` whose text content is an icon name that the font is supposed to
substitute with a glyph. The icon system moved to SVG descriptors (`FlareIcon` / `FlareIconView`, see
`src/Flare.Icons/FlareIcons.cs`); an app that does not also load the Material Symbols web font now sees
the raw words:

| Site | Renders | Should be |
| :-- | :-- | :-- |
| `FlareDataGrid.Editing.cs:32` - boolean cell | `check_box` / `check_box_outline_blank` | a checkbox glyph |
| `FlareDataGrid.Editing.cs:190` - row edit actions | `edit`, `check`, `close` | pencil / tick / cross |
| `FlareDataGrid.Composite.cs:168` - composite sub-header sort arrow | `arrow_upward` / `arrow_downward` | the sort arrow |

The rest of the grid was migrated (`FlareDataGrid.razor` uses `FlareIconView` throughout), which is why
this reads as "some icons broke": in one grid, the sort arrow on a normal header is a glyph and the sort
arrow inside a composite header is the word `arrow_upward`.

The Gallery does not catch it because the Gallery loads the Material Symbols font for its own chrome, so
the ligature still resolves there. Any app that does not is broken.

## Everywhere else with the same defect

`grep -rn "material-symbols" src/ --include=*.cs --include=*.razor` outside `Flare.Icons`:

- `Flare.Components.Carousel/Carousel/FlareCarousel.razor` - 2 spans (prev/next chevrons)
- `Flare.Components.RichTextEditor/RichTextEditor/FlareRichTextEditor.razor` - 7 spans (the toolbar)

These are the "~10 raw-span components" the icon-system work left pending. This issue closes the set for
the *components*; the CSS side (`icon.css` comment and any font-size rules keyed on the ligature class)
is checked in the same pass.

## Fix

Replace every ligature span with `<FlareIconView Value="..." />`. Two icons the catalog does not yet
have are needed for the boolean cell and have to be added to `FlareIcons`:

- `CheckBoxOutlineBlank` - the empty box
- `CheckBox` - the ticked box

Both are drawn from the same Material Symbols source as the rest of the catalog so the set stays
visually consistent. `FlareIcons.Check`, `Close`, `Edit`, `ArrowUpward`, `ArrowDownward`, `ChevronLeft`,
`ChevronRight` already exist; the rich-text toolbar needs `FormatBold`, `FormatItalic`,
`FormatUnderlined`, `FormatListBulleted`, `FormatListNumbered` and `Link` (only `Link` exists today).

The row-action buttons take `FlareIcon` instead of a `string icon` name, so a call site cannot pass a
ligature name again. The inline `style="font-size:1.125rem"` on the action buttons goes away with the
span - `FlareIconView` sizes from the icon token.

## Guard

A test asserting that no rendered component markup contains `material-symbols` unless the component is
explicitly a Material-Symbols icon pack. Cheap, and it is exactly the regression that happened: the
migration was done component by component and three sites were missed with nothing to notice.
