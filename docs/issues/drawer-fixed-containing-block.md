# Every overlay inside an open FlareDrawer is offset by the drawer's origin

**Status: FIXED in 0.26.1. Tier 1 - it makes a select, menu, tooltip, autocomplete or date picker inside a drawer
unusable, and it looks like the component has no options rather than like a layout bug. Found in Weir's
admin panel on 0.26.0; the same markup misbehaves back to whenever the open state became a transform.
One-line fix, verified in the browser.**

An anchored panel inside an open drawer renders off-screen. The panel is `position: fixed` and Flare
gives it viewport coordinates, which is correct - but the drawer establishes a containing block for
fixed descendants, so the browser measures those coordinates from the drawer's own top-left instead of
from the viewport's.

The cause is one declaration in `drawer.css`:

```css
.flare-drawer--open  { transform: translate(0, 0) !important; }
```

`translate(0, 0)` and `none` paint identically. They do not behave identically: any `transform` value
other than `none` makes the element a containing block for `position: fixed` descendants (CSS Transforms
Level 2, "the transformed element becomes the containing block for all fixed-position descendants"). The
closed states legitimately need a transform to slide the drawer; the open state sets one it does not
need, and pays for it with every overlay inside.

## Reproduction

A right-anchored 704px drawer on a 1600px viewport, containing any `FlareSelect`:

```razor
<FlareDrawer Open="true" Anchor="DrawerAnchor.Right" Width="44rem" Variant="DrawerVariant.Temporary">
    <ChildContent>
        <FlareSelect TValue="ParameterDirection" Items="Enum.GetValues<ParameterDirection>()" />
    </ChildContent>
</FlareDrawer>
```

Measured with the dropdown open:

| | |
| :-- | :-- |
| control `getBoundingClientRect().x` | 1188 |
| dropdown computed `left` | 1188.09px (correct) |
| dropdown `getBoundingClientRect().x` | **2084** |
| delta | **896** = 1600 - 704, the drawer's left edge |

The listbox is fully populated the whole time - four `[role="option"]` children - so nothing about the
data path is wrong. It is drawn 896px past the right edge of the window, which reads to a user as "the
dropdown opens empty".

The containing block is easy to confirm directly. Appending
`<div style="position:fixed;left:0;top:0">` to the drawer puts it at `x: 896`; the identical probe
appended to `<body>` sits at `x: 0`.

Every anchored overlay in the drawer is affected, not one component. Six selects measured in the same
drawer - method, object type, result mode, and three inside a repeated parameter row - were each off by
exactly 896.

## The fix

```css
.flare-drawer--open  { transform: none !important; }
```

The slide still animates. Interpolating a transform against `none` is defined: the `none` side is
treated as the identity matrix, so `translateX(100%) -> none` is the same animation
`translateX(100%) -> translate(0, 0)` was. Verified over a real open/close/open cycle, not just by
forcing the end state: after the drawer settles, computed `transform` is `none`,
`element.getAnimations()` is empty, the fixed probe reads `x: 0`, and the dropdown lands exactly on its
control (delta 0).

One caveat worth stating rather than discovering later: while the open transition is *running*, the
transform is a real matrix and the containing block is back. That is the correct behaviour for a
panel opened mid-animation and it is not observable here, because an overlay can only be opened by
interacting with a drawer that has already settled.

Changing this from the outside does not work, which is why it needs to be fixed in the package. An
application stylesheet that overrides `.flare-drawer--open` with `transform: none !important` while the
drawer is already open only retargets the running transition; Chrome leaves that transition in
`running` forever, holding `matrix(1, 0, 0, 1, 0, 0)`, and a running transition outranks an author
`!important` rule. The override works only if it is in force before the drawer opens.

## Worth a test

The regression is invisible to a rendering test - the markup and classes are all correct, and only the
resolved geometry is wrong. A test that asserts no drawer variant leaves a non-`none` transform on its
open state would hold the line, as would a check that no component under `Flare.Components` styles an
open/visible state with an identity transform. The dialog, popover and menu surfaces are worth the same
look: any of them that animates in with a transform and hosts anchored content has this bug latent.

## Resolution, 0.26.1

Fixed as proposed, plus four more places with the same defect that the report did not reach. A guard
test - `SettledTransformTests` - now fails on any rule whose selector names a settled state
(`--open`, `--visible`, `--shown`, `--expanded`) and whose body sets an identity transform. Keyframes
are excluded: an animation without a fill mode reverts to the base style when it ends, so it leaves
nothing holding a containing block.

| Rule | Was | Reach |
| :-- | :-- | :-- |
| `.flare-drawer--open` | `translate(0, 0) !important` | the reported case |
| `[dir="rtl"] .flare-drawer--left/right.--open` | `translateX(0)` | the same drawer in RTL, where the fix would otherwise not apply |
| `.flare-layout-drawer--floating/--temporary.--open` | `translateX(0)` | `FlareLayoutDrawer`, which is the navigation drawer of a whole application |
| `.flare-fab-menu__list--open .flare-fab-menu__item` | `scale(1)` | a tooltip anchored to a FAB action |
| `.flare-scroll-top--visible` | `translateY(0)` | hosts nothing anchored today; changed so the rule is uniform |

The guard found the FAB menu one on its first run, which is the argument for having written it.

Verified in the browser on the running Gallery, right-anchored drawer at `left: 672` in a 1032px
viewport, measured in a single pass so the two readings are directly comparable:

| | fixed probe appended to the drawer |
| :-- | --: |
| with `transform: translate(0, 0) !important` (the old rule, reinstated inline) | **x = 672** |
| with `transform: none` (shipped) | **x = 0** |

The slide is intact: sampled 60ms into the open transition the computed transform is
`matrix(1, 0, 0, 1, 125.938, 0)` with one running animation; once settled it is `none` with
`getAnimations()` empty.

The dialog, listbox, menu, snackbar and tab surfaces were checked for the same latent bug and are
clean - each animates in with a keyframe and no fill mode, so a settled panel carries no transform.
