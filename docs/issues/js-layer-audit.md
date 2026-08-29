# The JS layer: what a full read of it found

**Status: PARTIALLY DONE. A, B, E and H are fixed; C, D, F and G remain.**

2151 lines across 12 modules in `src/Flare.Components/wwwroot/js`. Read end to end. The findings are
ordered by what they cost, not by how much code they touch.

## A. Two loading models, and the older one is a manual setup step - FIXED, and it had a real victim

**Confirmed in the field before it was fixed.** A production application on Flare 0.18.1
(`techvill/OrderingPlatform`) reported "setting `Frozen` on a DataGrid column throws at runtime; remove
`Frozen` and everything works". Its `index.html` carries `flare-bootstrap.js` and
`blazor.webassembly.js` and **not** `flare-components.js`. So `window.FlareDataGrid` was undefined, and
the only DataGrid feature that reaches for it is the frozen-offset sync:

```
Could not find 'FlareDataGrid.updateFrozenOffsets' ('updateFrozenOffsets' was undefined).
```

In 0.18.1 that call site caught `InvalidOperationException` and `JSDisconnectedException` but not
`JSException`, so the throw escaped `OnAfterRenderAsync`. `Sortable` had nothing to do with it - the
reporter suspected it because that was the column they happened to be editing. Removing `Frozen` skipped
the branch, and the application was otherwise fine only because it used none of the other seven features
this file backs.

Both halves are now closed: the `JSException` catch was added to all four DataGrid call sites in a later
release, and as of 0.23.0 the file is an ES module the services import, so the tag cannot be forgotten.

### What the state was

Eleven modules are ES modules imported lazily through `FlareJsModule` on first use. The twelfth,
`flare-components.js` (279 lines), is a classic script that assigns **nine `window.*` globals** -
`flareOtp`, `flareField`, `FlareClipboardFallback`, `FlareInfiniteScroll`, `FlareLazy`, `FlareDataGrid`,
`FlareToc`, `FlareDownload`, `flareGetBounds` - reached from C# by 17 `InvokeVoidAsync("FlareToc.init")`
style calls.

Three costs, and the first is the one that hurts an adopter:

1. **Every host application has to add `<script src="_content/Flare.Components/js/flare-components.js">`
   by hand.** Forget it and OTP focus, DataGrid frozen columns and resize, the table of contents, lazy
   rendering, infinite scroll and file download all fail at runtime with no compile-time signal. Nothing
   else in Flare has a setup step like this.
2. It is parsed on every page of every app, including apps with no DataGrid and no OTP field.
3. It is served from `_content` unfingerprinted, which is the exact PWA service-worker skew hazard that
   already bit `FlareDataGrid` once.

Converting it to an ES module behind the existing typed services (`IDataGridJsService`,
`ITocJsService`, `ILazyJsService`, `IInfiniteScrollJsService`, `IElementJsService`) removes the setup
step, makes the load lazy, and closes the skew hazard. It is breaking for host `index.html` files, which
is an argument for doing it in one deliberate release rather than never.

## B. Fourteen of twenty interop callbacks are unguarded

`dotNetRef.invokeMethodAsync(...)` rejects once the circuit is gone - a navigation, a disposal, a dropped
SignalR connection. Only `flare-scroll.js` and `flare-viewport.js`, the two newest modules, wrap it. The
other fourteen call sites produce unhandled promise rejections on teardown:

`flare-components.js` (4), `flare-overlay.js` (4), `flare-drag.js` (2), `flare-theme.js` (2),
`flare-ui.js` (1), plus one more in components.

The fix is one shared helper the modules call instead of `invokeMethodAsync` directly, so a new call site
cannot forget. Cheap, and it removes a class of console noise that currently makes real errors harder to
find.

## C. The same registry is written fifteen times

`const _x = new Map()`, a `registerX(id, ...)` that tears down any previous entry and stores handlers,
and a `removeX(id)` that pulls them back off. It appears in `_escHandlers`, `_focusTraps`,
`_outsideClick`, `_anchoredPanels`, `_dismiss`, `_resizeHandles`, `_dialogDrags`, `_dialogResizes`,
`_splitters`, `_tabScrollers`, `_groupCollapsers`, `_subs`, `_elMap`, `_accentListeners`,
`_schemeListeners`. Each is five to fifteen lines and each is a place to forget a `removeEventListener`
- which is how the dead `setupCollision` came to leak two window listeners per call.

A four-line helper (`keep(map, id, teardown)` / `drop(map, id)`) would collapse all of them and make the
teardown structural rather than remembered.

## D. Two throttles with different semantics under one name

`flare-scroll.js` throttles - the first event claims the window, the trailing fire carries the latest
position. `flare-viewport.js` debounces - each event restarts the timer, so nothing fires until movement
stops. Both are the right choice for their event (you want scroll positions *during* a drag and a
resize only *after* it), but both parameters are called `ThrottleMs` in C#, which says one of them is
lying to the caller. Rename the viewport one, or document the difference where the caller reads it.

## E. `registerOutsideClick` is a strict subset of `registerDismiss`

Same file, same `Map` shape, and an identical pointerdown handler:

```js
if (element && !element.contains(e.target)) dotNetRef.invokeMethodAsync(method);
```

`registerDismiss` adds a `focusout` that fires when focus leaves the widget by keyboard. One consumer
each: `FlareColorPicker` on the older one, `FlarePopup` on the newer. Moving the colour picker onto
`registerDismiss` deletes the older pair here and two members from `IOverlayJsService`, and closes the
picker on Tab-away, which it should already do.

## F. Popup dismissal adds one document listener per popup

Every `registerDismiss` / `registerOutsideClick` / `registerDialogEscHandler` call attaches its own
capture-phase listener to `document`. With several dismissible widgets mounted, one pointerdown walks
several independent handlers that each do the same `contains` test. One listener per *event type* over a
registry would do the same work once. Small, but it is the shape that scales badly with the number of
open overlays, and the collision engine's own listeners were exactly this pattern before they were
deleted.

## G. Element resolution is written three times

`resolve(element, selector)` in `flare-scroll.js`, `scrollParent(el)` in `flare-components.js`, and
`pageTarget()` in `flare-scroll.js` all answer "which object do I measure and listen on". The
`pageTarget` reasoning in particular - that the page's offsets and extents must come from
`document.scrollingElement` rather than from `window`, or `Progress` lands off by a viewport - is
knowledge that currently lives in one module and is re-derived incorrectly elsewhere.

## H. `IScrollService` reports both axes but only derives one

`ScrollPosition` already carries `Left`, `ScrollWidth` and `ClientWidth`, and the JS already reports
them. What is missing is the derived half: `Progress`, `AtStart` and `AtEnd` are vertical-only, and
`ScrollChange.Delta` / `Direction` are vertical-only. An application scrolling a horizontal timeline,
carousel or Gantt has the raw numbers and has to do the arithmetic Flare already does for the other
axis.

Note that this does **not** make the port right for `FlareTabs`. The tab bar needs a `ResizeObserver`
as well, and its local handler already crosses interop only when one of its three booleans flips - once
per gesture rather than once per event. The horizontal metrics are for application code.

## Order

A, B, E and H are done. C, D, F and G remain - cleanups that make the next reader's job easier and should ride along
with whatever touches those files.
