# Small parity batch: five components, each under a day

**Status: OPEN. Phase 1. Independent items - ship them one commit each, not as one branch.**

Each of these exists in at least one reference framework, is genuinely useful, and is small enough that
the specification fits in a paragraph. They are grouped only so they do not each need their own file;
they are not a single unit of work.

---

## 1. `FlareTimeSpanPicker` - duration input

Radzen has `RadzenTimeSpanPicker`; nobody else does. Flare has four date/time pickers and no way to type
a *duration*, which is what task trackers, billing screens and media tools actually need.

Not a clock. A field with unit segments - days / hours / minutes / seconds - each spinnable, with
`Min`, `Max`, `Step`, `Units` (which segments are shown), `Format` and `AllowNegative`. Build it on
`FlareEditableFieldBase` and `FlareFieldChrome` like the rest of the field family, so it inherits label,
helper text, validation and clear behaviour for free. Reuse `PickerTokens`; do not add a token record for
a field that is chrome plus segments.

## 2. `FlarePasswordStrength` - strength meter

Blazorise ships one. `FlarePasswordField` exists and has no strength affordance.

A *component* is the wrong shape here. Ship it as an opt-in parameter on `FlarePasswordField`
(`ShowStrength`, `StrengthEvaluator`) rendering through `FlareMeter`, plus a public default evaluator so
the common case is one boolean. The evaluator is a delegate returning a score and a reason list, because
password policy is an application decision and hardcoding a rule set into a UI library is the same class
of mistake as hardcoding a color. Strings for the reasons come from resx.

## 3. Navigation guard - unsaved-changes prompt

MudBlazor has `MudExitPrompt`. `grep -rl beforeunload src/` returns nothing, so Flare cannot stop a user
leaving a half-filled form, in-app or out.

Two halves, and both are needed:
- **Browser unload** - `beforeunload` registration, through the existing `IUiJsService` port, not a new one.
- **In-app navigation** - Blazor's `NavigationManager.RegisterLocationChangingHandler`, which is C# and
  needs no JS at all.

Shape it as `FlareNavigationGuard` with `IsDirty`, `Message`, and `OnConfirm` - and route the in-app case
through `IDialogService` so the prompt is a themed Flare dialog rather than the browser's untouchable
one. That is the part MudBlazor does not do.

## 4. `FlarePullToRefresh` - touch refresh

Fluent UI is the only reference framework with this, and it is the only one with a serious mobile story
at all. Flare already has the breakpoint machinery (`FlareMediaQuery`, `IBrowserViewportService`,
`Hidden`) and an open mobile audit; this is the missing gesture.

Wrap content, listen with pointer events (not touch events - pointer covers pen and mouse-drag testing),
pull past a threshold, show a token-styled indicator, raise `OnRefresh` and await it before releasing.
Parameters: `Threshold`, `MaxPull`, `Disabled`, `IndicatorTemplate`. No JS if the scroll-position read
can come from the existing element port; one call if it cannot. Must no-op when the scroll container is
not at the top, and must respect reduced-motion.

## 5. Busy overlay - the loading-state primitive

Blazorise has `LoadingIndicator` and `SpinKit`. Flare has `FlareProgress`, `FlareOverlay` and
`FlareSkeleton`, and every application wires the same three together itself.

Add `FlareBusy` - a container that dims and blocks its own subtree while `Busy` is true, showing a
centred indicator after a `Delay` (so fast operations do not flash) and holding it for a `MinDuration`
(so it does not flicker). Both of those timings are the entire reason this is a component and not a
`<div>`. It must trap focus out of the busy region and set `aria-busy`. Composed from the existing
overlay and progress components; new tokens only for the dim color, blur and z-layer, if the existing
scrim tokens do not already cover it - check before adding.

---

## Also considered, deliberately deferred

**`DropDownDataGrid`** (Radzen) - a select whose panel is a full DataGrid with columns, sorting and
filtering. This is real and it is wanted, but it is not small, and it belongs to the Select-family
direction already sketched (a maximally configurable `FlareSelect`, possibly absorbing the autocomplete)
rather than arriving as a fifth sibling component. Specify it there, not here.

## Done when

Each item independently: XML docs, EN and RU strings, bUnit tests, a Gallery demo, no literals in CSS,
and a token record only where an existing one genuinely does not fit.
