# Chart: partial series (nullable values) - gaps in line series

**Status: OPEN. Feature request from a real app (OrderingPlatform, 0.26.2).**

`ChartSeries.Values` is `IReadOnlyList<double>` - no nulls. A series that exists only on
part of the x-range cannot be expressed: it must be padded with zeros, and zeros are data
(a line crashing to the axis reads as "no sales that month", not "not sold that month").

Real case: a sales chart over all months of the year. Most products sell every month; a
seasonal product sells only in summer. The chart must show all twelve months (shared
`Labels`), with the seasonal line present on its months and absent (a gap, not a zero)
elsewhere.

## Ask

- Accept `IReadOnlyList<double?>` (or a parallel "known range", or `double.NaN` with
  defined skip semantics) for line and area series: consecutive present points connect,
  a null point breaks the line (gap), markers only on present points.
- Bar series with nulls simply draw nothing for that category.
- Document how nulls interact with stacking, if stacking arrives later (a null in a
  stacked band most likely means "treat the whole stack slot as absent").
