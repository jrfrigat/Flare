# `FlareCardActions` has alignment and nothing else

**Status: DONE. Tier 1. From the app user's review.**

## The report

> FlareCardActions supports alignment but has no proper FullWidth, Wrap or vertical mode. Long buttons
> on a phone overflowed the card - I had to assemble the actions with FlareContainer and FlareStack.

Confirmed by reading it: the component is a `div` with one modifier class chosen from three alignments.
It is a flex row that neither wraps nor stacks, so on a narrow viewport the buttons run past the card
edge instead of reflowing. Falling back to `FlareStack` works but throws away the card's action padding,
spacing and the `Align` it did have.

## Design

`FlareCardActions` gains the layout parameters a row of actions actually needs, all token-driven:

- `Wrap` (bool) - allow the row to wrap. Should arguably be the default; the mandate allows changing it,
  and a row of actions that overflows its card is never the intent. Default `true`, with `Wrap="false"`
  for the deliberate single-line case.
- `Orientation` (Horizontal | Vertical) - the phone case: full-width buttons stacked top to bottom.
- `FullWidth` (bool) - the children share the width equally (`flex: 1 1 0`), which is the standard
  mobile dialog/card footer.
- `Stacked` (Breakpoint?) - switch to vertical + full width automatically below a breakpoint, so one
  parameter covers "row on desktop, stack on phone" without the caller writing a media query. This is
  the parameter that removes the workaround.
- `Spacing` - the gap, from the spacing scale, defaulting to the card's action gap token.
- `Reverse` (bool) - reverse the visual order, for the platforms that put the confirm button first.

`Align` stays and gains `Center` and `Stretch`, which were missing from `CardActionsAlign`.

All of it is CSS on the existing element - modifier classes and a `--flare-card-actions-*` token group -
so no new DOM and no measurement. `Stacked` is a container query on the card, not a viewport media
query, so a card in a narrow column stacks even on a wide screen. That is the behaviour the reporter
wanted and it is strictly better than what they could build by hand.

`FlareCardFooter` is deliberately NOT given the same parameters: it is a metadata strip, its own docs
already point button rows at `FlareCardActions`, and duplicating the layout API on both would be the
kind of divergence the token mandate exists to prevent.

`StackBelow` takes CARD-width tiers (`Narrow` / `Compact` / `Wide`), not the viewport breakpoint scale:
a container query against `Breakpoint.Sm` would call a 500px card "narrow" and stack almost every card
there is.
