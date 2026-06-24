# Contributing to Flare

Thanks for your interest in improving Flare! This guide covers everything you need to get a change
merged.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/)
- A recent IDE (Visual Studio 2022/2026, Rider, or VS Code with the C#/Razor extensions)

## Build & test

The repository builds from a single solution:

```sh
dotnet restore Flare.slnx
dotnet build   Flare.slnx -c Release
dotnet test    Flare.slnx -c Release
```

Run the component gallery locally to see your change:

```sh
dotnet run --project samples/Flare.Gallery
```

## Workflow

1. Fork the repo and create a short-lived branch off `main` (e.g. `feat/slider-ticks`,
   `fix/select-scroll`).
2. Make your change with tests.
3. Make sure `dotnet build` and `dotnet test` pass, and that CI is green on your PR.
4. Open a pull request against `main`. PRs are squash-merged, so the PR title becomes the commit -
   please use [Conventional Commits](https://www.conventionalcommits.org/) style
   (`feat(select): ...`, `fix(datagrid): ...`, `docs: ...`).

`main` is always releasable; releases are cut by tagging `vX.Y.Z` (see below).

## Conventions

Flare components follow a strict, unified style so the library stays consistent. Before adding or
changing a component, read **[docs/en/component-conventions.md](docs/en/component-conventions.md)**.
In short:

- **CSS** lives in the global bundle `src/Flare.Components/wwwroot/css/*.css` (not scoped), driven by
  semantic design tokens; minimal JavaScript.
- **Public API** (types, `[Parameter]`s, methods, enums) must carry meaningful XML documentation -
  it drives the generated API docs and is enforced as a build error.
- **ASCII only** in code, comments and XML docs (no smart quotes, arrows, em-dashes or emoji).
- **Gallery first**: every component has a demo page in `samples/Flare.Gallery`; extend it when you
  change a component so the behavior can be reviewed visually.
- **Localization**: user-facing strings in the Gallery are localized (EN + RU resources) - never
  hardcode them.

## Releases (maintainers)

Versioning is driven by [MinVer](https://github.com/adamralph/minver) from git tags:

- **Stable:** tag `vX.Y.Z` on `main` -> the release workflow packs and pushes all packages to NuGet.
- **Pre-release:** tag `vX.Y.Z-preview.N` (or `-rc.N`) -> published as a NuGet prerelease.
- **Gallery image:** tag `gallery-vX.Y.Z` -> builds and pushes the Docker image (versioned
  independently of the packages).

## Reporting bugs & requesting features

Use the [issue templates](https://github.com/jrfrigat/Flare/issues/new/choose). For security issues,
see [SECURITY.md](SECURITY.md) - please do not open a public issue.

By contributing, you agree that your contributions are licensed under the [MIT License](LICENSE).
