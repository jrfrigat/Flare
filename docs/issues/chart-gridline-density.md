# Chart: the horizontal grid is always five lines

**Status: OPEN. Tier 2. Reported after 0.19.1.**

## The report

> In FlareChart, add a way to specify the number of "rows" in the chart. Right now, going by the
> examples, it is always 5 rows.

## What is actually there

`FlareChart.Overlays.cs`, `GridLines(min, max)`:

```csharp
int steps = 4;
```

Four steps, five lines, for every cartesian chart. Not a parameter, not derived from anything. The Y
axis labels are drawn by the same loop, so the count of labels is welded to the count of gridlines.

The X axis does not have this problem any more - 0.19.1 replaced its fixed step with a width budget
(`_labelSlot`, one label per 56 viewBox units) precisely because a fluid chart is no longer 400 units
wide. The Y axis never got the same treatment, and it has the same defect for the same reason: a 120px
chart and a 600px chart both get five lines, so one is cluttered and the other is empty.

## Where I disagree with the literal request

A raw count parameter alone is the wrong primitive to stop at, for the reason Excel does not stop
there either. With `min=0, max=470` and 7 lines you get labels at 0, 67.1, 134.3, 201.4 ... - which is
worse than the five-line default it replaced. The count is only useful together with axis rounding.

So the fix is three things, and the count is the smallest of them.

## The fix

### a. `YAxisTickCount`

`[Parameter] public int? YAxisTickCount` - number of horizontal gridlines / Y labels. Null (the
default) means derive it from the plot height on the same budget principle as the X axis: one line per
`_tickSlot` viewBox units, clamped to a sane range. That makes the current five-line result the outcome
of a rule instead of a constant, and makes a short chart draw three lines and a tall one draw eight.

### b. Nice-number axis bounds

The scale currently ends at the exact data min/max, so any tick count above the default produces
unreadable labels. Add `NiceScale`-style rounding (the standard 1/2/2.5/5/10 progression) that expands
`[min,max]` to the nearest round step for the requested tick count. This is what makes the axis read
0, 100, 200, 300, 400, 500 instead of 0, 94, 188, 282, 376, 470.

Guarded by the existing `YAxisMin`/`YAxisMax` bounds: an explicitly bounded axis is not rounded.

### c. Minor gridlines and vertical gridlines

`GridLines` only ever draws horizontal lines. Excel draws both, and offers a lighter minor line between
majors. Add:

- `ShowVerticalGrid` - a line at each category slot, using the X projection already in
  `XOfIndex` so it stays correct under zoom.
- `YAxisMinorTicks` - count of minor divisions between two majors, drawn with a dedicated
  `--flare-chart-grid-minor-*` token so a theme can make it near-invisible.

## Tokens

Two new component tokens (`ChartTokens`), values in both theme packages, mapped in `CssVarMap`:
`GridMinorColor`, `GridMinorWidth`. The vertical grid reuses the existing major grid tokens - it is the
same line at a different angle, and per the token mandate similar things share tokens.

## Gallery

The axis demo gains a control for tick count so the effect is visible, and one demo shows a chart with
vertical gridlines - which is the layout most people recognise from a spreadsheet.
