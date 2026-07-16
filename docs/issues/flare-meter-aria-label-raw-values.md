# FlareMeter: aria-label reads out unformatted raw values, ignoring ShowValues

Reported from Weir (0.4.0), where `FlareMeter` replaced a hand-rolled timing bar.

## What happens

`FlareMeter` builds its `aria-label` from every segment's `DisplayValue`, unconditionally:

```csharp
// FlareMeter.razor:114
var parts = _views.Select(v => string.IsNullOrEmpty(v.Label) ? v.DisplayValue : $"{v.Label} {v.DisplayValue}");
```

and `DisplayValue` falls back to the `"G"` format specifier:

```csharp
// FlareMeter.razor:85
var display = value.ToString(Format ?? "G", CultureInfo.CurrentCulture);
```

Two consequences, both only visible to a screen reader:

1. **`ShowValues` does not gate the values in the aria-label.** The parameter is documented as
   "Shows each segment's numeric value in the legend (and in the hover tooltip)", and it does gate
   both of those - but the aria-label carries the values whether or not it is set. So a meter that
   deliberately hides values still announces them.

2. **The default format is `"G"`**, which on a `double` renders full round-trip precision. A meter fed
   real measurements announces them like this:

   ```
   aria-label="DB 0.3ms 0,25, Other 0.1ms 0,06269999999999998"
   ```

   That is a real duration of 0.0627 ms. Sighted users see a clean bar; a screen-reader user gets
   seventeen significant digits.

## Reproduction

Weir's request-log drawer, on Flare 0.4.0. A meter with two segments carrying raw millisecond
`double`s, `ShowLegend="true"`, `ShowValues` left at its default of false:

```razor
<FlareMeter ShowLegend="true">
    <FlareMeterSegment Value="0.25"   Label="DB 0.3ms"   Color="FlareColor.Error" />
    <FlareMeterSegment Value="0.0627" Label="Other 0.1ms" Color="FlareColor.OnSurfaceVariant" />
</FlareMeter>
```

Rendered DOM (copied from the running app):

```html
<div class="flare-meter" role="img"
     aria-label="DB 0.3ms 0,25, Other 0.1ms 0,06269999999999998">
```

## Why the obvious workaround is not enough

Setting `Format="0.#"` cleans up the precision, which is what Weir does now. But it does not address
the first point: a caller whose `Label` already states the quantity ("DB 0.3ms") gets it announced
twice ("DB 0.3ms 0,3"), and there is no way to opt out. `Format` is also documented as applying to
"values shown in the legend / tooltip", so reaching for it to fix the aria-label is a side effect,
not the intended lever.

## Suggested fix

Two independent changes, either useful alone:

- Gate the aria-label's values on `ShowValues`, matching the parameter's documented meaning and the
  legend/tooltip behaviour. With `ShowValues="false"`, announce the labels only; with no labels
  either, the current value-only text is still the sensible fallback.
- Default `Format` to something bounded (e.g. `"0.##"`) rather than `"G"`. `"G"` is the right default
  for round-tripping a value, not for reading one out; no meter wants its scale announced to
  seventeen digits.

Worth noting the current text is also comma-joined and culture-formatted, so under `ru` it reads
`"0,25, Other"` - the value separator and the segment separator become the same character. Gating on
`ShowValues` would make that far rarer, but a different join (e.g. `"; "`) would settle it.
