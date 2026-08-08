# FlareProgress: the wavy RING does not flow (the linear bar does)

**Status: the visual defect is fixed; the animation is not implemented.** The ring renders a correct,
still wavy arc. Making its crests travel is what remains.

## What was wrong (0.14.0)

The ring's flow was attempted with a CSS keyframe on the indicator:

```css
from { rotate: 0deg;   stroke-dashoffset: 0;   }
to   { rotate: 360deg; stroke-dashoffset: 100; }
```

The idea was to rotate the wavy path a full turn while sliding the dash window by the same amount, so
the two cancel and the visible arc stays put while the crests travel through it.

It cannot work, for a reason that is structural rather than a tuning error. The indicator's dash array
is **a single window**, not a repeating pattern:

```
stroke-dasharray = "0 <lead> <len> 100"      (pathLength=100)
```

Sliding `stroke-dashoffset` across that walks the window onto the trailing 100-unit gap, so the
visible arc changes length and position every frame and periodically collapses to a fragment. Rotation
on its own is no better: the window is defined *along the path*, so it rides around with it.

A second, smaller flaw sat underneath: `transform-box: fill-box` puts the rotation origin at the
bounding box centre of the wavy path, which is not exactly the ring's centre.

## What to do instead

Animate the **phase of the wave in the path data**, leaving the element untransformed and the dash
window alone. `BuildWavyCirclePath(waves, amp, phase)` already takes a phase argument and is only ever
called with `0`, which is the shape the original design had in mind - the razor still carries a comment
about a SMIL `d`-morph.

Two routes, both without JS:

- **SMIL** `<animate attributeName="d" values="…;…;…" dur="…" repeatCount="indefinite" />` with one
  path per phase step through a single wave period. Works in every evergreen browser. Cost is markup
  size: each path is ~120 segments, so keep the step count low and consider fewer segments while
  animating.
- **CSS `d`** on `path()` values. Cleaner, but support is narrower than SMIL's.

Either way: honour `prefers-reduced-motion`, and remember that the linear bar's flow is unaffected -
it works because it translates a wide clipped sine by exactly one wavelength, which IS periodic.
