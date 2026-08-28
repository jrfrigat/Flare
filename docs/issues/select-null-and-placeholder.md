# `FlareSelect<T>` cannot express "no value"

**Status: OPEN. Tier 1. From the app user's review.**

## The report

> FlareSelect<T> is awkward with null: the closed list shows an empty value, and there is no separate
> Placeholder. For "All ingredients" and "All cake" I had to use typed sentinel values.

## What is actually missing

`Placeholder` does exist (inherited from the field base, rendered by `_showPlaceholder` when
`Current is null`). What does not exist is a **selectable option that means "no value"**. `Clearable`
offers a clear button, which is not the same affordance: a filter select needs "All" to be a row in the
list, at the top, selectable with the keyboard, and rendered in the closed field as the words "All
ingredients" rather than as emptiness.

Without it there are two bad choices, and the reporter hit both:

- put a sentinel item of type `T` in `Items` - which leaks a fake domain value into the model, forces
  every consumer to filter it out, and is what they ended up doing;
- bind to `T?` and let `null` be selectable - which renders as a blank row with no label, because
  nothing tells the select what a null option should say.

## Design

Three parameters on `FlareSelect<T>` (and `FlareCombobox<T>` and `FlareAutocomplete<T>`, which share the
engine, so the option list gets it once):

- `NullOption` (string?) - when set, an option row with this text is prepended to the list and selecting
  it sets `Value` to `default(T)`. This is the "All ingredients" case, and it is one parameter.
- `NullOptionTemplate` (RenderFragment?) - the rich form, for an italic or icon-led "All" row.
- `ShowNullOption` is not needed: `NullOption != null` is the switch.

Rules that make it behave:

- the null row renders as *selected* when `Value` is `default(T)`, so the closed field shows "All
  ingredients" instead of the placeholder;
- `Placeholder` keeps its meaning - "nothing has been chosen yet" - and only shows when there is no null
  option, so the two never fight;
- the null row participates in keyboard navigation and type-ahead like any other row, and is exempt from
  filtering when the user types (an "All" row that disappears when you search is useless);
- `Clearable` and `NullOption` compose: the clear button selects the null option.

For a non-nullable `T` (`int`, an enum, a struct) `default(T)` is a real value and is therefore *not* a
safe "none". Those cases bind `T?` - which the engine already supports - and the null option means
`null`. This is stated in the XML docs, because it is the one part a caller can get wrong.

The empty row the reporter saw in the closed list is the second half of this issue and is fixed
independently: an item whose display text resolves to empty renders a zero-height row today. It should
render the `NullText` fallback, and a `null` entry in `Items` should be either rejected or routed to the
null option rather than silently drawn as a blank.
