# FlareSlider: no media-scrubber affordances (hover-only handle, hairline track)

**Source:** the leftover from the original `flareslider-media-progress` report (PlaylistShare audio player).
That report asked for three things; two are now done, this file tracks the remaining third.

**Severity:** low. A seek bar is expressible today - it just does not *look* like a player scrubber.

## Already resolved (do not re-do)

- **Buffered / loaded band** - the essential part. Expressed as a plain zone, no dedicated parameter:
  ```razor
  <FlareSlider @bind-Value="_played" Min="0" Max="100" Size="SliderSize.Xs" MouseWheel="true">
      <Zones>
          <FlareZone Start="0" End="@_buffered" Color="FlareColor.OnSurfaceVariant" />
      </Zones>
  </FlareSlider>
  ```
  A `BufferValue` parameter was deliberately NOT added - `Zones` already covers it, and zones additionally
  express what a single value cannot: several buffered ranges (a real player's `video.buffered` is a
  `TimeRanges` list, not one number).
- **Wheel-to-seek** - `MouseWheel="true"` (shipped).

## What is still missing

1. **A hairline track.** The original report wants a ~4dp seek bar. The thinnest built-in size is
   `SliderSize.Xs` = **16dp** track / 44dp handle (`slider.css`, the `--flare-slider--xs` size class).
   A consumer *can* override the theme token per instance (e.g. `Style="--flare-slider-track-height:4px"`),
   but that is undocumented, and it does not shrink the handle or the 44dp hit area to match.
2. **A hover-only handle.** A player scrubber shows no thumb at rest and reveals it on hover/focus of the
   track. `FlareSlider` always renders its handle.

## Why it matters

Every audio/video player needs this, so today each app either hand-rolls a raw `<input type="range">` (what
PlaylistShare did - ~100 lines of component + CSS) or accepts a form-slider look for its seek bar.

## Proposed approach (design first - do not rush)

Prefer a token/opt-in solution over a new component, and reuse what exists:

- **Do NOT add a `FlareMediaSlider`.** The band, the wheel-seek and the geometry tokens already exist; a
  second slider would duplicate the whole component for two visual affordances.
- Options to weigh:
  - (a) An opt-in `HandleOnHover` (or `Scrubber`) bool that hides the handle at rest and reveals it on
    `:hover`/`:focus-within` of the track - a few CSS rules, honouring `prefers-reduced-motion`.
  - (b) Document the existing per-instance geometry-token override as the supported way to get a hairline
    track, and make the handle/hit-area follow the track height so a 4px track does not keep a 44dp thumb.
  - (c) A dedicated `SliderSize.Hairline` preset that sets a consistent thin track + small handle.
- **Accessibility caveat:** a hover-only handle must stay keyboard-reachable and must not shrink the touch
  target below the 44dp guidance on coarse pointers - gate the hiding on `(hover: hover)`.

**See also:** the per-size geometry was broken under MD3 (rail 0px) and is fixed in 0.4.0; any hairline-track
work builds on those restored geometry tokens. The fallback contract is documented in
[core-theme-decoupling-p3-p4.md](core-theme-decoupling-p3-p4.md).
