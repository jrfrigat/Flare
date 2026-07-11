# Bug: icon-only buttons render the glyph ~2px off-center

**Source:** found while building the Deka theme on **Flare 0.1.7** (PlaylistShared PWA). Every
`FlareIconButton` in the app (header back, mini/full-player play·pause, track-row play, `more_vert`,
share/settings, …) rendered its single glyph a couple of pixels **left of the button's optical center**.
Consistent and visible enough that the app had accumulated two per-button workarounds before the root
cause was found.

## Symptom

In any icon-only button the sole glyph sits ~2px to the left of center. Measured on 0.1.7: the icon slot's
computed `margin-inline-start` is `-4px`, so the flex-centered content is shifted left by half that → the
glyph center is ~2px left of the button center. Uniform across every `.flare-btn--icon-only` instance.

## Root cause

`src/Flare.Components/wwwroot/css/button.css`:

```css
/* Optically tuck each icon toward the button edge on its side (the icon side gets
   slightly less padding than the label side). */
.flare-btn__icon--leading  { margin-inline-start: calc(-1 * var(--flare-spacing-2)); }   /* ~L280 */
.flare-btn__icon--trailing { margin-inline-end:   calc(-1 * var(--flare-spacing-2)); }

/* ICON-ONLY: icon without text -> square (width = height), no inline padding and no gap. */
.flare-btn--icon-only {                                                                   /* ~L292 */
    --_flare-btn-padding-inline: 0;
    padding-inline: 0;
    gap: 0;
    width: var(--_flare-btn-height, var(--flare-btn-height-md, 3rem));
    aspect-ratio: 1 / 1;
}
```

The leading/trailing negative margin is a deliberate optical tuck **for icons that sit next to a label**
(the icon side wants slightly less padding than the label side). `.flare-btn--icon-only` correctly zeroes
`padding-inline` and `gap`, but it **does not reset that icon margin** — and an icon-only button has no
label for the tuck to balance against, so the lone glyph just ends up shifted by `-1 * var(--flare-spacing-2)`
and reads as off-center.

## Proposed fix

Neutralize the optical tuck when there is no label, inside the icon-only rule:

```css
.flare-btn--icon-only .flare-btn__icon--leading,
.flare-btn--icon-only .flare-btn__icon--trailing {
    margin-inline: 0;
}
```

Scope note: this is orthogonal to the new **`ButtonEdge`** / `.flare-edge-start` · `.flare-edge-end` feature
(0.1.5), which applies a negative margin to the *whole button* for app-bar/toolbar edge alignment — that
outer offset is intentional and unaffected.

## Affected

Any `.flare-btn--icon-only` — primarily `FlareIconButton`, and icon-only `FlareToggleButton` if it shares
the same slot markup. Text buttons (icon + label) are unaffected: the tuck there is intentional and correct.

## Workaround applied downstream (can be dropped once fixed)

Deka added one global rule (`deka.css`, loaded after `flare-components.css`, wins on cascade order without
`!important`):

```css
.flare-btn--icon-only .flare-btn__icon--leading,
.flare-btn--icon-only .flare-btn__icon--trailing { margin-inline-start: 0; margin-inline-end: 0; }
```

This replaced two earlier point fixes (a `translateX(2px)` on the filled play/pause glyph and a scoped
`:only-child` margin reset on the playlist action buttons). Verified: all icon-only buttons then measure a
glyph offset of 0.
