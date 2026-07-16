# Token-record XML docs are tautological and quote a stale theme value

**Found:** 0.5.0, while stripping dead fallbacks - the doc said one thing, the theme did another.

**Severity:** low, but actively misleading: at least one doc states a value the shipping theme does not use.

## The problem

Component token records document per-size members like this:

```csharp
/// <summary>Gap xs token (<c>0.25rem</c>).</summary>
[CssVar(Button.Gap.Xs)] public required string GapXs { get; init; }
```

Two things are wrong:

1. **It quotes a value the core does not own.** `MaterialDesignTokens` actually sets `GapXs = "0.5rem"`
   (8dp, per spec) - so the doc is simply **wrong**, and a reader who trusts it is misled. Verified by
   measuring the rendered button: the xs gap is 8px, not 4px.
2. **Quoting any theme's number in a core doc is the token mandate broken in prose.** The core is supposed to
   hold no theme opinion; a doc that says "this token is 0.25rem" states one theme's choice as if it were the
   token's meaning. Different themes legitimately differ (FluentUI2 pins one slider geometry at every size;
   MD3 ramps).

The summaries are also tautological - "Gap xs token" restates the member name and says nothing about purpose,
which the repo's doc-quality rule forbids.

## Scope

The `Name + size + token (<value>)` pattern appears in **5 records**, ~24+ members:

```
src/Flare.Abstractions/Tokens/Components/ButtonTokens.cs        (Gap/Height/PaddingInline/IconSize x5, focus, shadows)
src/Flare.Abstractions/Tokens/Components/FabTokens.cs
src/Flare.Abstractions/Tokens/Components/MenuTokens.cs
src/Flare.Abstractions/Tokens/Components/SplitButtonTokens.cs
src/Flare.Abstractions/Tokens/Components/ToggleButtonTokens.cs
```

## Proposed fix

Rewrite each summary to say what the token *is for*, not what one theme sets it to:

```csharp
/// <summary>Gap between the icon and the label at the xs size.</summary>
```

Drop the value entirely - the value belongs to the theme, and the reference themes are the place to read one.
Where a value genuinely aids understanding (e.g. "0 disables the notch"), describe the *semantics* of the
special value rather than quoting a theme's number.

## Why not bundled into 0.5.0

0.5.0 is the dead-fallback sweep + guards; this is a documentation-quality pass over a different set of files.
Keeping them apart keeps each diff reviewable. No behaviour change either way.

## Guard idea (optional)

A test could fail any `[CssVar]` member whose XML summary contains a CSS-looking literal (`\d+(px|rem|%)`),
the doc analogue of `DeadFallbackTests`. Cheap, and it would stop the pattern coming back.
