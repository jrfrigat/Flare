# `FlareCollapse` resets itself, and the controlled-state contract is not written down

**Status: OPEN. Tier 0. From the app user's review.**

## The report

> FlareCollapse was unreliable without external state management. FlareAccordionPanel turned out to be
> more stable, but it is not always an equivalent replacement.

## The defect

`FlareCollapse` keeps a local `_expanded` and a `_lastExpanded` mirror meant to detect an *external*
change to the `Expanded` parameter. `Toggle()` writes the mirror:

```csharp
private async Task Toggle()
{
    _expanded = !_expanded;
    _lastExpanded = _expanded;      // <-- writes the PARAMETER mirror with the LOCAL value
    await ExpandedChanged.InvokeAsync(_expanded);
}
```

With no binding (`<FlareCollapse Header="...">`), `Expanded` stays `false` forever. After the user
opens the panel the mirror says `true` while the parameter says `false`, so the next `OnParametersSet` -
which fires on **any** parent re-render, for any unrelated reason - sees `false != true` and closes the
panel. The panel shuts itself the first time anything else on the page changes. That is precisely
"unreliable without external state management": adding a binding makes the parameter track the mirror
and the symptom disappears.

The comment above the code states the intended behaviour ("keep the local toggle state so an unbound
header collapse is not reset by parent re-renders") - the implementation does the opposite.

## The fix

The mirror exists to answer one question - *did the parameter change since I last looked* - so it may
only ever be written from the parameter, in `OnParametersSet`, and never from an event handler. Removing
the one line in `Toggle` fixes the reported bug.

But `FlareCollapse` is also missing the other half that `FlareToggleButton` gets right: telling
controlled from uncontrolled. The canonical shape, which becomes the documented contract:

```csharp
protected override void OnParametersSet()
{
    if (ExpandedChanged.HasDelegate)        // controlled: the parent owns the state, always follow it
    {
        _expanded = Expanded;
    }
    else if (Expanded != _lastExpanded)     // uncontrolled: adopt only a real external change
    {
        _expanded = Expanded;
    }
    _lastExpanded = Expanded;               // mirror the PARAMETER, unconditionally, and nowhere else
}
```

Controlled now means what it says: a parent that receives `ExpandedChanged` and declines to change
`Expanded` keeps the panel shut, which is what a controlled component must do and what the current code
cannot express.

## Audit

Every component with a local mirror of a two-way parameter is checked against that shape. The pattern
`_last<Something> =` outside `OnParametersSet` is the smell:

- `FlareCollapse` - broken, above.
- `FlareToggleButton` - the `OnParametersSet` logic is correct; `SetToggledAsync` writes `_lastToggled`
  and has the same latent defect for an uncontrolled toggle driven imperatively.
- `FlareColorPicker`, `FlareAvatar`, `FlareToggleGroup`, `FlareTabs`, `FlareDataGrid` - each read and
  confirmed or fixed.

The contract goes into `docs/ru/component-conventions.md` next to the CSS and token rules, because it is
the same kind of rule: a shape every component follows so a defect cannot be reintroduced one component
at a time. A test renders each two-way component, toggles it internally, forces an unrelated parent
re-render, and asserts the state survived.
