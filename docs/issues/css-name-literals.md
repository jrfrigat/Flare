# CSS names as string literals instead of registry constants

**Status: OPEN, and narrower than it looked. Tier 2. Found in review of `FlareAvatarGroup`. The one
silent corner - reading a token by name - is fixed and guarded; the rest is cosmetic and is NOT worth a
scripted rewrite (that was tried; see "What a blanket rewrite costs").**

`Flare.Abstractions` owns two name registries - `Flare.Css.Tokens.*` for custom properties and
`Css.Classes.*` for class names - and the point of both is that a name exists once, so a rename is a
compile error rather than a silent no-op. Component code does not use them consistently: it writes the
same names as string literals in markup and in C#, where nothing checks them.

The trigger:

```razor
<div style="--flare-avatar-group-spacing:@Spacing; @Style"  <!-- FlareAvatarGroup.razor -->
```

`AvatarField.GroupSpacing` already exists. Nothing connects the two.

## The part that was silent, and is now closed

Sorting the 105 by *what happens when the name is wrong* splits them unevenly:

- **A token read by name is silent when it is wrong.** `ReadTokenNum("--flare-x", fallback)` returns the
  fallback, the component behaves exactly as if no theme had set the token, and nothing appears on screen
  to say so. `FlareProgress` read **all eight** of its wave and ring tokens by literal, so renaming any
  of them in a theme would have quietly disabled the wavy progress bar. Now on constants, with
  `TokenLookupKeyTests` failing on any literal lookup key anywhere in `Flare.Components`. `FlareChart`
  already used constants for its twelve lookups, which is why only Progress had to move.
- **Everything else is visible when it is wrong.** A misspelled name in a style attribute or a CSS
  fragment produces an element that is obviously unstyled. Worth tidying; not worth risk.

## What a blanket rewrite costs

Rewriting all 105 by script was attempted and reverted. The regex that finds a `--flare-*` name inside a
double-quoted string cannot tell an opening quote from a closing one, so on a line like
`"a" + x + "--flare-b"` it matched from the wrong quote and produced **654 compile errors** across the
four chart partials. Doing this safely means reading each of the 105 sites, which is a session of its own
for a payoff of "a rename becomes a compile error instead of an obviously broken element".

If it is done, do it per file with the build run between files, not in one pass.

## Measured extent

`src/Flare.Components`, `.cs` + `.razor`:

- **105 occurrences of a `--flare-*` name across 30 files.** Highest counts:
  `FlareProgress.razor` (8 wavy/ring properties), `FlareChart.*` (7 across three partials),
  `FlareTypography.cs` (5), `FlareTabs.razor` (4), `FlareCard.razor` (3),
  `FlareLayoutDrawer.razor` (2), `FlareClockDial.razor`, `FlareTextArea.razor`,
  `FlareDataTree.razor`, `FlareColorCustomizer.razor`, `FlareSlider.razor`.
- **Class-name literals**, far fewer: `flare-visually-hidden` (`FlareDataGrid.razor`) and
  `flare-datagrid` as a storage-key prefix (`DataGridPersistence.cs`).

Not every hit is a defect. Three legitimate kinds have to survive the pass:

1. **Prefix construction** - `FlareComponentBase` and `FlareSpacingVars` build names from a `--flare-`
   stem plus a computed suffix. These are the registry's own mechanism, not a bypass of it.
2. **Element id prefixes** - `flare-menu-`, `flare-mi-`, `flare-submenu-`, `flare-popover-` are id
   seeds, not class names. They belong to no registry.
3. **Names a theme is not meant to set** - a per-instance channel like `--flare-dial-angle` or
   `--flare-slider-length` is written by the component and read by its own CSS in the same breath. It is
   still worth a constant so the two ends cannot drift, but it does not belong in a token record and
   must not gain a `[CssVar]` attribute, or the settable-token guard will demand a theme value for it.

## The work

1. ~~Classify all 105.~~ Done, and the answer is cleaner than expected: **54** name a token that already
   has a constant (just use it), **17** are per-instance channels that needed a registry of their own,
   and the remaining ~34 are prefix construction that must stay a literal by nature. Category (b) - a
   real theme token with no constant at all - turned out to be **empty**.
2. ~~The per-instance channels need somewhere to live.~~ Done: `Css.Tokens.LocalVars`, holding the 17,
   with the reason they must never gain a `[CssVar]` attribute written into the type doc - the
   settable-token guard would otherwise demand that every theme supply the angle of one clock hand.
3. **Swap the 54, per file, with a build between files.** Low value, non-zero risk; see above.
4. Same pass for class literals - only two exist, and neither is silent.
5. **Check for dead names in the other direction**: constants and CSS classes that no code or
   stylesheet reads any more. The registries have outlived several refactors, and nothing currently
   fails when a name is orphaned.
6. **Names that stopped being accurate.** A name is also wrong when it no longer says what the token is
   for, and the way that happens is a family growing suffixed siblings around an unsuffixed member that
   used to be the only one. `--flare-input-padding` was this: four `-xs/-sm/-lg/-xl` steps appeared
   beside it and it kept reading as "the padding" rather than "the medium step". Renamed to
   `--flare-input-padding-md`.

   Scanning all 1007 distinct `--flare-*` names for a base with two or more sized siblings turns up two
   candidates, and only one is a defect:

   - **`--flare-fab-radius`** - sits beside `-sm` and `-lg`, and the constant behind it is *already*
     called `Radius.Md`. The C# says medium, the CSS name does not. Rename to `--flare-fab-radius-md`.
   - **`--flare-col-span`** - leave it. The `-xs..-xxl` siblings are breakpoint overrides that fall back
     to it in a mobile-first cascade (`var(--flare-col-span-md, var(--flare-col-span-sm, ...))`), so the
     unsuffixed name genuinely is the base. Same shape, different meaning.

   The same question applies to modifier suffixes that are not sizes (`-dense`, `-compact`), but those
   read correctly today: `--flare-appbar-height` with `--flare-appbar-height-dense` is a base plus a
   variant, not one step of a ramp.

7. A guard test to keep it clean: no `--flare-` literal in `Flare.Components` sources outside the
   prefix-construction helpers. This is the part that makes the pass stick; without it the count grows
   back. It cannot catch item 6 - an inaccurate name is still a real name - so that one stays a
   judgement call at review time.

## Why it is worth doing

The mandate promises that a theme can repoint any token. A name written as a literal in one place and
as a constant in another breaks that promise silently: the theme sets the token, the component writes
its own spelling, and the override does nothing. There is no test today that would catch it, which is
also why the guards matter more than the tidying: `TokenLookupKeyTests` closes the half that is silent,
and item 3 is only cosmetics.
