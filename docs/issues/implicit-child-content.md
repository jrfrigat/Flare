# Implicit child content reported to throw at runtime

**Status: OPEN, NEEDS THE EXCEPTION TEXT. Tier 0. Reported item 7.**

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

## Work

1. **Get the exception text** for the reported case. If it is not a name collision, this issue is
   re-scoped to whatever it actually is.
2. **Audit the slot names** across every public Flare component: list every `RenderFragment` parameter
   whose name is a plausible application component name, and decide per component whether to keep it.
   The mandate allows renaming: a slot named `HeaderContent` (the pattern `FlareCollapse` already uses)
   cannot collide the way `Header` can, and reads no worse.
3. **Document the rule** in the component conventions: a named slot is `XxxContent`, never a bare noun
   that an application would plausibly name a component.
4. Add a test that renders every component with a plain implicit-child-content body, so a component that
   cannot take implicit children is caught at build time rather than in an app.
