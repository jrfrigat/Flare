# Scrollbars are the browser's, not the theme's

**Status: OPEN. Tier 2. From the app user's review.**

## The report

> The scrollbars on the pages - horizontal and vertical alike - stand out visually too much. They
> should probably be dark or light to match the palette.

Correct, and it is the one surface in a Flare application that ignores the design language entirely. A
dark theme currently gets a light scrollbar wherever the browser's default wins, which is the single
loudest thing on a dark page.

Nothing in `src/Flare.Components/wwwroot/css` sets `scrollbar-color`, `scrollbar-width` or any
`::-webkit-scrollbar` rule today. Every scroll container - the app shell's content panel, a DataGrid
body, a listbox, a tab bar, a code block, a dialog body - paints whatever the user agent decides.

## Why it is not simply "add a CSS rule"

Three things make this a token decision rather than a stylesheet one:

1. **Two mechanisms, and they do not overlap cleanly.** `scrollbar-color` / `scrollbar-width` are the
   standard, supported in Firefox and in Chromium 121+; `::-webkit-scrollbar` is the older pseudo-element
   family that Safari still needs and that allows far more (track radius, button removal, per-state
   thumb). A theme should express the intent once and have both emitted, rather than authoring the same
   colours twice.
2. **`scrollbar-width` is not a length.** It takes `auto | thin | none` - so a "thin scrollbar" token
   cannot be a pixel value on the standard path even though the webkit path wants one. The token has to
   carry the keyword and the pseudo-element rule derive its width from a separate one, or the two paths
   disagree.
3. **It has to be a theme decision, not a Flare one.** Under the mandate Flare ships no default look, so
   the tokens are `required` and each theme answers: M3 would take the thumb from `outline-variant` and
   the track from the surrounding container tone; Fluent has its own scrollbar treatment; a Visual
   Studio theme wants the IDE's overlay style. Core CSS applies whatever it is given, including "leave
   the browser alone".

## Shape

Roughly four tokens, in a scale beside the border one (which was the same argument in a different
place - a rule the browser drew that no theme could reach):

- `--flare-scrollbar-width` - the `scrollbar-width` keyword.
- `--flare-scrollbar-size` - the pixel size for the webkit path.
- `--flare-scrollbar-thumb` - thumb colour, with a hover/active variant or a state-layer composition.
- `--flare-scrollbar-track` - track colour, `transparent` for an overlay look.

Core emits both mechanisms from those, applied to `html` and to a `.flare-scrollable` utility that the
components with their own scroll containers already need. Whether it lands on `html` globally or only
inside Flare surfaces is worth deciding deliberately: taking over the whole document's scrollbar is a
strong default for a component library, and probably belongs behind an opt-in on `FlareThemeProvider`
rather than being unconditional.

## Where it shows up

Worth checking every one of these when it lands, because they scroll independently and some sit on a
different surface tone than the page: the shell content panel, `FlareLayoutDrawer`, `FlareDataGrid`
body, `FlareListbox` / `FlareSelect` panels, `FlareTabs` bar, `FlareCodeBlock`, `FlareDialog` body,
`FlareTable`, `FlareTree`, and the horizontal strip on the scroll service page.
