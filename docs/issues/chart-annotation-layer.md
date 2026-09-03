# Chart: annotation draw layer - over or under the series

**Status: OPEN. Feature request from a real app (OrderingPlatform, 0.26.2).**

`ChartAnnotation` decides WHERE an overlay sits (data coordinates, `LabelPosition`
Start/End/Auto for the label) but not WHICH LAYER it draws on. Annotations always render
on top of the series. With a busy combo chart (six series of lines and bars plus two
`VerticalBand` bands for fact/plan periods) the bands wash out the values they are meant
to frame: a translucent band over the data reads as noise, while the same band under the
data would read as a background period marker - which is what a "fact" / "plan" band is.

## Ask

A layer option on the annotation, e.g. `ChartAnnotationLayer { Over, Under }`
(`Layer = ChartAnnotationLayer.Under` for bands), defaulting to the current behavior.
Under-layer annotations should still respect the plot-area clip and draw beneath markers
and lines but above the plot background/grid.
