# Flare.CssAudit

Internal CLI that keeps the `Flare.Css` registry, the component stylesheets and the themes in
agreement. Its whole value rests on one thing: **it can only verify a name it can read**, so a name
assembled at runtime is a name it cannot prove exists.

## What it checks

`check` - `src/Flare.Components/wwwroot/css/*.css` against `src/Flare.Abstractions/Css/Classes/*.cs`:

- `[+]` a class the CSS styles with no constant naming it.
- `[-]` a constant with no CSS rule (a runtime prefix and a theme-scoping marker are exempt: neither
  has a standalone rule).
- `[~]` a class a theme defines that the base CSS does not.
- `[!]` a dead literal fallback on an always-emitted semantic token.
- `[=]` one class name declared by more than one constant.
- `[L]` a name written as text in a component: one the registry already owns, or a bare `flare-*` in a
  class attribute, which is what a name built from a stem looks like. Comments are not code and are
  dropped first. A string that only reads like a class - a storage key, say - is exempted on its own
  line with `cssaudit:allow-literal`, followed by the reason.

`packages` - the same, once per optional package (`src/Flare.Components.*`), each against its own
`Css/Classes` registry and its own stylesheet. The main check cannot see these: it reads one
stylesheet folder and one registry, and every package has its own pair. One direction differs:

- `[h]` a class a component emits that no rule styles. Reported, never failed - on an element whose
  parent owns the layout, that class is the hook a theme needs to reach it, and a package stylesheet
  is thin on purpose. `[-]` is kept for a name nothing styles *and* nothing emits, which is dead.

`tokens` - `--flare-*` usage against `Css.Tokens` (`[T+]`, `[T-]`, `[T~]`).

`generate` / `merge` - emit constants for the `[+]` classes, grouped by CSS file, with field names
derived from the class (`flare-datepicker__day--today` -> `DayToday`).

## Usage

From the repo root:

```
dotnet run --project tools/Flare.CssAudit               # interactive menu
dotnet run --project tools/Flare.CssAudit -- check      # exit 1 on mismatch
dotnet run --project tools/Flare.CssAudit -- packages   # exit 1 on mismatch
dotnet run --project tools/Flare.CssAudit -- tokens     # report only, always exit 0
dotnet run --project tools/Flare.CssAudit -- generate
```

`check`, `packages` and `tokens` also run as tests (`CssAuditTests`), so CI fails on drift without
anyone remembering to run the CLI.
