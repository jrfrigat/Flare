# CSS names as string literals instead of registry constants

**Status: OPEN. Tier 2. Found in review of `FlareAvatarGroup`.**

`Flare.Abstractions` owns two name registries - `Flare.Css.Tokens.*` for custom properties and
`Css.Classes.*` for class names - and the point of both is that a name exists once, so a rename is a
compile error rather than a silent no-op. Component code does not use them consistently: it writes the
same names as string literals in markup and in C#, where nothing checks them.

The trigger:

```razor
<div style="--flare-avatar-group-spacing:@Spacing; @Style"  <!-- FlareAvatarGroup.razor -->
```

`AvatarField.GroupSpacing` already exists. Nothing connects the two.

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

1. Classify all 105. Each is either (a) a real token that has a constant - use it; (b) a real token
   with no constant - add one, plus the record member and the `CssVarMap` line, and give both themes a
   value; or (c) a private per-instance channel - constant only, in a registry section marked as such.
2. Same pass for class literals.
3. **Check for dead names in the other direction**: constants and CSS classes that no code or
   stylesheet reads any more. The registries have outlived several refactors, and nothing currently
   fails when a name is orphaned.
4. **Names that stopped being accurate.** A name is also wrong when it no longer says what the token is
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

5. A guard test to keep it clean: no `--flare-` literal in `Flare.Components` sources outside the
   prefix-construction helpers. This is the part that makes the pass stick; without it the count grows
   back. It cannot catch item 4 - an inaccurate name is still a real name - so that one stays a
   judgement call at review time.

## Why it is worth doing

The mandate promises that a theme can repoint any token. A name written as a literal in one place and
as a constant in another breaks that promise silently: the theme sets the token, the component writes
its own spelling, and the override does nothing. There is no test today that would catch it, which is
also why step 5 matters more than steps 1-2.
