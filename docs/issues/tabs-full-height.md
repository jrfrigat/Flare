# Tabs and layout: no supported way to fill the viewport height

**Status: OPEN. Layout gap. Found in a real app (OrderingPlatform) on 0.26.2 building a
screen-fit page: two equal halves, each with a scrolling grid.**

Two structural gaps force every "fits the screen" page to re-implement library internals
in app CSS.

## 1. Horizontal Tabs panels do not stretch

`.flare-tabs__panels` has no `flex: 1` / `min-height: 0` in the horizontal case (only the
vertical variant gets `flex: 1`), and `.flare-tabs__panel` is a plain block with no height.
So even with `Style="height: 100%"` on a definite parent, a child that says `height: 100%`
or `flex: 1` resolves against `auto` and collapses to content height.

The failure is nastier than it looks: a DataGrid inside such a tab either grows the page
(no scroll of its own), or - once the page gives it `overflow` - disappears entirely,
because a zero-height parent plus `overflow: auto` clips everything away. In the virtual
variant the same zero-height parent makes the `max-height: 100%` cap drop out and the table
paints over whatever sits below, which LOOKS fine until it does not.

## 2. FlareLayoutContent frame has no height

`.flare-layout-content` (the scrolling `main`, a definite `1fr` grid row) wraps page
content in `.flare-layout__content-frame`, which has no height. Every `height: 100%` on a
page resolves against `auto` right there, one level above the tabs. Screen-fit pages
currently patch this themselves:

```css
.flare-layout-content > .flare-layout__content-frame { height: 100%; }
```

Ordinary scrolling pages are unaffected by that rule (the frame still grows, `main` still
scrolls), which suggests it could simply live in the library.

## Ask

- Either a supported switch on `FlareTabs` (e.g. `FillHeight="true"`) that stretches the
  panels container AND makes the visible panel a flex column, or a documented snippet.
- Consider giving `.flare-layout__content-frame` a height so page-level `height: 100%`
  works out of the box, or documenting the intended screen-fit pattern for `FlareLayout`.
