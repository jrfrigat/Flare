# Flare.CssAudit

Internal CLI that keeps `Flare.Core/CssClasses.cs` in sync with the component CSS.

## What it does

- Collects every `flare-*` class from `src/Flare.Components/wwwroot/css/*.css`.
- Parses the constants in `src/Flare.Core/CssClasses.cs` (value + declaring nested class).
- Compares both directions and reports:
  - `[+]` classes that exist in CSS but are **missing from CssClasses** (with the CSS file).
  - `[-]` constants in CssClasses that have **no matching CSS rule** (with the `CssClasses.X.Y` location).
- Generates C# constants for the `[+]` classes, **grouped by CSS file** (one nested class per file),
  with PascalCase field names derived from the class (`flare-datepicker__day--today` -> `DayToday`).

## Usage

Run from the repo root:

```
dotnet run --project tools/Flare.CssAudit            # interactive menu
dotnet run --project tools/Flare.CssAudit -- check   # report (exit 1 on mismatch)
dotnet run --project tools/Flare.CssAudit -- generate
```

`generate` prints the suggested nested classes and can write them to
`tools/Flare.CssAudit/generated-cssclasses.txt` for review before merging into `CssClasses.cs`.

> Note: a `[-]` constant may legitimately reference CSS that lives in `Flare.Core` (not scanned),
> or a base class that only has modifier rules (e.g. `__display-part` styled only via `--active`).

## Roadmap

- Collect CSS custom properties (`--flare-*` tokens) and generate/verify a `CssTokens` registry,
  mirroring the class workflow.
