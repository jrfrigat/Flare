# Implicit child content reported to throw at runtime

**Status: OPEN. Tier 0. Reported item 7. The mechanism is now known - see "Why nothing ever fails
loudly" - and 21 components are measured as affected. `FlareChip` is fixed; the other 20 are not.**

## The report

> `<FlareTab Label="test"><MyLocalComponent /></FlareTab>` throws at runtime, while
> `<FlareTab Label="test"><ChildContent><MyLocalComponent /></ChildContent></FlareTab>` works.

## What was tested

Both forms were compiled in the test project and the generated Razor was diffed:

```
diff ReproTabImplicit_razor.g.cs ReproTabExplicit_razor.g.cs
(no output)
```

The two forms produce **byte-identical** code - one `AddAttribute(seq, "ChildContent", RenderFragment)`
either way. Both render, and both were also rendered through `FlareTabs` in bUnit with a component
child. There is therefore no runtime difference between the two spellings *for a child whose tag name
does not collide with a parameter*, and the report as literally written cannot be reproduced.

## The collision that IS real, and is the likely cause

Razor decides "these children are named content slots" by matching the child *tag name* against the
component's `RenderFragment` parameters. So a user component whose name equals a slot name is silently
swallowed:

```razor
<FlareCard>
    <Header />          @* the app's own Header component *@
</FlareCard>
```

Razor binds this to the `Header` *parameter*, not to `ChildContent`. The app's component never renders,
and - depending on the slot - the surrounding component can then dereference something it expects the
child content to have provided. Wrapping in `<ChildContent>` fixes it, which is exactly the shape of the
report. `FlareTab`'s slots are `ChildContent` and `LeadingIcon`; a component named `LeadingIcon` is
unlikely, but `Header`, `Footer`, `Actions`, `Content`, `Title`, `Icon`, `Label` and `Body` are all slot
names somewhere in Flare and all are plausible app component names.

A component *name* is checked before a parameter name, so this only bites when the app component and the
slot share a name - but when it bites there is no diagnostic at all.

## Why nothing ever fails loudly

Found while writing a Gallery demo: `<FlareChip>42</FlareChip>` compiled with **zero errors and zero
warnings**, and rendered an empty chip.

Blazor's rule is that child content on a component with no `ChildContent` parameter is an error. Flare
never sees that error, because `FlareComponentBase` declares

```csharp
[Parameter(CaptureUnmatchedValues = true)]
public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }
```

and **every** component inherits it. The generated Razor confirms what happens - the fragment is emitted
as an untyped attribute rather than a component parameter:

```csharp
__builder.OpenComponent<FlareChip>(0);
__builder.AddComponentParameter(1, nameof(FlareChip.Label), "kept");
__builder.AddAttribute(2, "ChildContent", (RenderFragment)(__builder2 => {   // <- not a parameter
    __builder2.AddContent(3, "this text has nowhere to go");
}));
```

At runtime it lands in `AdditionalAttributes` as an unmatched attribute, gets splatted onto the root
element, and is dropped because a `RenderFragment` is not an attribute value. The catch-all turned a
compile-time error into silence, library-wide.

This is the same shape as `<FlareProgress Max="1" />`, where a parameter that does not exist became a
DOM attribute instead of an error. The catch-all is worth having - it is how `data-*`, `aria-*` and
`@onclick` reach the root element - but it means **Flare gets no compiler help at all** for a wrong
parameter name or an unaccepted child, and the library has to close that gap itself.

## Measured: 21 components render caller text but accept no children

Every one of these silently swallows `<X>content</X>`:

`FlareChip` (fixed), `FlareCheckbox`, `FlareRadio`, `FlareSwitch`, `FlareSlider`, `FlareRating`,
`FlareAvatar`, `FlareDivider`, `FlareAppBar`, `FlareEmptyState`, `FlareHighlighter`, `FlareColorPicker`,
`FlareColorCustomizer`, `FlareDateRangePicker`, `FlareFieldChrome`, `FlareFloatingActionMenuItem`,
`FlareMeterSegment`, `FlareOnThisPage`, `FlareShortcutEntry`, `DataGridColumnPicker`,
`DataGridFilterBuilder`.

The label family - Checkbox, Radio, Switch, Slider, Rating - is where a caller most naturally writes
markup between the tags, because a label is so often more than a string (a link in a consent checkbox,
a unit in a slider label). Those should come next.

## Work

1. ~~Get the exception text for the reported case.~~ Superseded: the mechanism above explains both the
   reported symptom and the silent-swallow class it belongs to. A slot-name collision produces the
   reported "works only with explicit `<ChildContent>`" behaviour; the catch-all is why neither form
   ever produces a diagnostic.
2. **Give the remaining 20 a `ChildContent` that falls back to the string parameter**, as `FlareChip`
   now does: content between the tags wins, the string is the shorthand. This is additive and breaks
   nothing.
3. **Audit the slot names** across every public Flare component: list every `RenderFragment` parameter
   whose name is a plausible application component name, and decide per component whether to keep it.
   The mandate allows renaming: a slot named `HeaderContent` (the pattern `FlareCollapse` already uses)
   cannot collide the way `Header` can, and reads no worse.
4. **Document the rule** in the component conventions: a named slot is `XxxContent`, never a bare noun
   that an application would plausibly name a component.
5. **A guard test** is the part that makes it stick: render every public component with an implicit
   child body carrying a marker, and assert either that the marker appears or that the component is on
   an explicit list of genuinely void ones. Without it the count grows back, and the compiler will never
   report it - which is the whole finding.
