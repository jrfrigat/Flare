# Charts: the series types Flare does not have

**Status: OPEN. Phase 3, large. Blocked by [chart tokenization](chart-tokenization.md) - do not add
series types until the palette and ramp are theme-owned, or every new series bakes in more literals.**

`ChartType` currently has 13 members: `Line, Bar, Pie, Donut, Area, StackedBar, Scatter, Radar, HeatMap,
Bubble, Rose, PolarArea, Combo`. That is a strong set - ahead of Blazorise's Chart.js defaults and
comparable to MudBlazor. Radzen is far ahead of everyone with roughly 35 series components, and the gap
is concentrated in three families that share almost no code with each other.

Fluent UI has no charts at all, so nothing here is about catching Fluent.

## Family 1 - financial and statistical

| Series | Encodes | Notes |
| :-- | :-- | :-- |
| Candlestick | open / high / low / close | Radzen has it. The standard trading view. |
| OHLC | same, bar notation | Shares the model with candlestick; cheap once one exists. |
| HighLow | min / max per category | Also the base for the range family. |
| BoxPlot | quartiles, whiskers, outliers | Radzen has it. Statistical, not financial, but the same "one category, five numbers" shape. |
| ErrorBar | value plus uncertainty | Nobody has it. Scientific plotting, and trivial once high-low exists. |

These need a **multi-value point model**. `ChartPoint` today carries X, Y and R. Rather than adding four
more nullable doubles, introduce a values array or a dedicated point record per family, chosen so the
existing single-value series pay nothing for it.

## Family 2 - flow and part-to-whole

| Series | Notes |
| :-- | :-- |
| Waterfall (vertical and horizontal) | Radzen has both. The finance-deck standard. Needs running-total layout plus explicit total columns. |
| Funnel | Radzen has it. Conversion analysis. |
| Pyramid | Radzen has it. Same layout, inverted, different label rules. |
| Sankey | Radzen and MudBlazor both have it. A real graph layout - node ranking plus link routing. Budget it separately; it is not a series, it is a diagram. |
| Treemap | Radzen has it. Squarified-treemap layout, recursive. Also not a series. |

Sankey and Treemap should be **their own components** (`FlareSankey`, `FlareTreemap`) sharing the chart
token record and the SVG conventions, not `ChartType` members. Forcing a graph layout through a
categorical axis model would distort both.

## Family 3 - analytics over an existing series

| Feature | Notes |
| :-- | :-- |
| Trendline (linear, polynomial) | Radzen has both; Blazorise has a trendline extension. Pure math over an existing series. |
| Moving average | Radzen has it. Window parameter. |
| Value line / mean line | A horizontal reference at a constant or a computed statistic. |
| Range band | Shaded region between two values - targets, tolerances, confidence intervals. |
| Data labels with collision avoidance | Radzen has `RadzenSeriesDataLabels`. Flare shows values on some series; a general labeller that hides overlapping labels is the missing part. |

These are the cheapest of the three families and the most immediately useful, because they apply to
series Flare already renders. **Do this family first.**

## Also worth having, from the "nobody has it" column

- **Range series** (range bar, range column, range area) - Radzen has them; they fall out of the high-low
  model almost free.
- **A brushing range selector** under a chart (Radzen's `RangeNavigator`). This is the interaction that
  makes a long time series usable, and it composes with the existing zoom story rather than adding one.
- **Stacked and full-stacked variants** of area and line. Radzen has full-stacked for four series types;
  Flare has `StackedBar` only. Generalise stacking as a *modifier* on the series rather than as N new
  enum members - `StackMode.None / Stacked / FullStacked` on `ChartSeries`. That single decision removes
  eight would-be enum members and is the kind of thing that keeps Flare's API smaller than Radzen's while
  covering more ground.

## Constraints

- Everything stays native SVG with zero JS, which is the reason Flare's charts are worth having at all
  next to a Chart.js wrapper.
- Every new series must render into the visually-hidden data table for screen readers, like the existing
  ones.
- `FlareChart.razor` is already 1025 lines. This work must not push it to 3000. Split the renderers into
  per-family internal classes behind a common interface *before* adding the first new series - that
  refactor is step zero of this issue, and it is also what makes the file-size rule in CLAUDE.md hold.
- Series colors come from the theme palette. No new hardcoded roles - that is the whole point of the
  blocking issue.

## Suggested order

1. Renderer split plus `StackMode` generalisation (refactor, no new features).
2. Family 3 - trendlines, moving average, reference lines, range bands, smart data labels.
3. High-low model, then candlestick / OHLC / box plot / error bars / range series.
4. Waterfall, funnel, pyramid.
5. `FlareTreemap`, then `FlareSankey`.
6. Range navigator.

## Done when

Each step independently: token-driven, accessible table entry, unit tests on the layout math (a
squarified treemap and a sankey ranking are both exactly the kind of algorithm that needs a fixture), a
Gallery demo, and no growth in the JS surface.
