# Build the standalone "Legacy" enterprise demo (Blazor WASM) and serve it via nginx.
# ── Stage 1: Build ───────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /repo

# Restore dependencies first (layer-cached unless .csproj files change). Only the Flare.Legacy
# closure is needed: Flare -> Components + Infrastructure -> Abstractions + Theming, plus the themes.
COPY ["src/Flare.Abstractions/Flare.Abstractions.csproj",                                        "src/Flare.Abstractions/"]
COPY ["src/Flare.Theming/Flare.Theming.csproj",                                                 "src/Flare.Theming/"]
COPY ["src/Flare.Infrastructure/Flare.Infrastructure.csproj",                                    "src/Flare.Infrastructure/"]
COPY ["src/Flare.Components/Flare.Components.csproj",                                            "src/Flare.Components/"]
COPY ["src/Flare.Theme.MaterialDesign3.Tokens/Flare.Theme.MaterialDesign3.Tokens.csproj",        "src/Flare.Theme.MaterialDesign3.Tokens/"]
COPY ["src/Flare.Theme.FluentUI2.Tokens/Flare.Theme.FluentUI2.Tokens.csproj",                    "src/Flare.Theme.FluentUI2.Tokens/"]
COPY ["src/Flare.Theme.MaterialDesign3Expressive/Flare.Theme.MaterialDesign3Expressive.csproj", "src/Flare.Theme.MaterialDesign3Expressive/"]
COPY ["src/Flare.Theme.FluentUI2/Flare.Theme.FluentUI2.csproj",                                  "src/Flare.Theme.FluentUI2/"]
COPY ["src/Flare.Theme.Aero/Flare.Theme.Aero.csproj",                                            "src/Flare.Theme.Aero/"]
COPY ["src/Flare.Theme.LiquidGlass/Flare.Theme.LiquidGlass.csproj",                              "src/Flare.Theme.LiquidGlass/"]
COPY ["src/Flare.Theme.VisualStudio/Flare.Theme.VisualStudio.csproj",                            "src/Flare.Theme.VisualStudio/"]
COPY ["src/Flare/Flare.csproj",                                                                  "src/Flare/"]
COPY ["samples/Flare.Legacy/Flare.Legacy.csproj",                                                "samples/Flare.Legacy/"]
# Both repo-level props files must be present for restore: Directory.Build.props (shared
# <TargetFramework> etc.) and Directory.Packages.props (Central Package Management - the single
# source of every NuGet version; without it the version-less PackageReferences won't resolve).
COPY Directory.Build.props Directory.Packages.props ./
RUN dotnet restore "samples/Flare.Legacy/Flare.Legacy.csproj"

# Copy full source and publish
COPY . .
RUN dotnet publish "samples/Flare.Legacy/Flare.Legacy.csproj" -c Release -o /publish --no-restore

# ── Stage 2: Serve ────────────────────────────────────────────
FROM nginx:alpine AS final
COPY --from=build /publish/wwwroot /usr/share/nginx/html
COPY nginx.conf /etc/nginx/nginx.conf
EXPOSE 80
