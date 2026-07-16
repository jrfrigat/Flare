# FlareMeter under-fills its track when the segment values sum to less than 1

Reported from Weir (0.4.0), where `FlareMeter` replaced a hand-rolled timing bar. This is the more
serious of two `FlareMeter` findings; the other is
[aria-label reads out unformatted raw values](flare-meter-aria-label-raw-values.md).

## What happens

`FlareMeter` puts each segment's raw value straight into `flex-grow`:

```csharp
// FlareMeter.razor
// Only positive segments contribute; flex-grow carries the raw value so the row fills the track
// in proportion to the sum, with no per-segment percentage rounding gaps.
```

The comment's premise does not hold. CSS Flexbox distributes free space by flex-grow factor **only
when those factors sum to at least 1**: if the sum is below 1, each item takes just its own fraction
of the free space and the rest is left unfilled (CSS Flexible Box Layout 1, section 7.1.1). So a
meter whose values happen to sum to less than 1 silently renders a partly empty track, in correct
proportions.

This is not an edge case for a meter fed raw measurements. It is the whole sub-1 range of any unit:
milliseconds under a millisecond, fractions, ratios, megabytes under a megabyte.

## Reproduction

Weir's request-log drawer, Flare 0.4.0, one endpoint call taking 0.3955 ms (0.3155 ms in the database,
0.0797 ms elsewhere) - an ordinary fast call:

```razor
<FlareMeter ShowLegend="true" Format="0.#">
    <FlareMeterSegment Value="0.3155" Label="DB 0.3ms"    Color="FlareColor.Error" />
    <FlareMeterSegment Value="0.0797" Label="Other 0.1ms" Color="FlareColor.OnSurfaceVariant" />
</FlareMeter>
```

Measured in the running app:

```
flex-grow factors : 0.3155, 0.0797   (sum 0.3952)
track width       : 591.0 px
segment widths    : 186.5 px + 47.1 px = 233.6 px   <-- 40% of the track
```

The remaining 357 px of track is empty. The proportions between the two segments are right; the bar
just stops early.

Confirmed as the root cause by scaling the same two factors up so they sum to 100, changing nothing
else:

```
flex-grow factors : 79.83, 20.17     (sum 100)
segment widths    : 471.8 px + 119.2 px = 591.0 px  <-- fills exactly
segment ratio     : 3.958 (before: 3.960)           <-- unchanged
```

The changelog's own example - "raw measurements (12.4 ms, 3.4 ms) declarable as-is" - sums to 15.8,
comfortably above 1, which is presumably why this did not surface.

## Suggested fix

Normalize the factors inside the component before emitting them: divide each value by the sum of all
positive values and multiply by a constant (100 works). That keeps the author-facing promise intact -
raw values still go in as they are - and preserves the stated benefit, since a single `flex-grow` per
segment still leaves the browser to do the division with no per-segment percentage rounding gap.

`flex-basis: 0` on the segments is doing the right thing already; only the growth factors need
scaling.

## Workaround in the meantime

The caller normalizes the values to sum to 100 itself, which is what Weir now does - and what its
hand-rolled bar did before adopting the meter, for exactly this reason. It costs the caller the thing
the component was meant to remove, and it makes `ShowValues` / the aria-label announce percentages
rather than the real measurements.
