# No transient toast / snackbar for ephemeral confirmations

**Source:** Weir admin panel. Every mutating page (endpoints, keys, scopes, admins, settings) wants a
short "Saved" / "Deleted" confirmation after a successful action. There is no Flare component for a
transient, auto-dismissing, corner-anchored notification, so those confirmations are currently either
omitted or faked with a persistent inline element.

**Severity:** low-to-medium. Nothing is blocked - failures are already shown well (see below) - but the
positive path has no consistent affordance, and every consumer that wants one has to build its own,
which is exactly the bespoke-component situation Flare exists to avoid.

## What already works (do not treat this as missing)

- **`FlareAlert`** covers *persistent, in-context* messages perfectly: a validation or server error shown
  next to the form that caused it, staying put until the user fixes and retries. Weir uses it for exactly
  that, and it is the right tool there - an error a user must act on should not auto-dismiss.

The gap is the *opposite* case: a **success** that needs acknowledging but not persisting. An inline
`FlareAlert Success` after every save is visual clutter (it pushes layout, and it lingers with nothing to
do). A toast is the established pattern - appears in a corner, announces itself to a screen reader, fades
after a few seconds, stacks when several fire.

## What is missing

1. **A toast host + a service to raise them.** Something like an `IFlareToasts` service injected into a
   page, plus a `<FlareToastHost />` placed once in the layout:
   ```razor
   @inject IFlareToasts Toasts
   ...
   await Api.SaveEndpointAsync(def);
   Toasts.Show("Endpoint saved.", ToastSeverity.Success);
   ```
2. **Severity + auto-dismiss + manual dismiss.** Success / Info / Warning / Error (reuse
   `AlertSeverity` if it fits), a configurable timeout (with a sensible default, and a way to make an
   Error sticky), and a close affordance.
3. **Stacking and a max-visible cap.** Several actions in quick succession should stack and not cover the
   screen; oldest or excess ones drop.
4. **Accessibility.** The host should be an `aria-live` region (polite for Success/Info, assertive for
   Error) so the announcement reaches a screen reader without stealing focus.

## Anchoring notes

- Positioning should be theme-driven (a corner, offset by spacing tokens), not per-call pixel math, so
  it lands consistently in both the Command Center and Visual Studio themes.
- No bespoke CSS should be needed on the consumer side - that is the whole point of asking Flare for it
  rather than building it in Weir.

Until this exists, Weir shows failures with `FlareAlert` inline and simply omits success toasts (a silent
reload). That is acceptable but not polished, and it is the last admin-panel item that depends on Flare.
