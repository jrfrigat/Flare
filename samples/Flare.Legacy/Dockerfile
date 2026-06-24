# Build the standalone "Legacy" enterprise demo (Blazor WASM) and serve it via nginx.
# ── Stage 1: Build ───────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /repo

# Restore dependencies first (layer-cached unless .csproj files change). Only the Flare.Legacy
# closure is needed: Flare -> Components -> Core, plus the five built-in themes.
COPY ["src/Flare.Core/Flare.Core.csproj",                                                       "src/Flare.Core/"]
COPY ["src/Flare.Components/Flare.Components.csproj",                                            "src/Flare.Components/"]
COPY ["src/Flare.Theme.MaterialDesign3Expressive/Flare.Theme.MaterialDesign3Expressive.csproj", "src/Flare.Theme.MaterialDesign3Expressive/"]
COPY ["src/Flare.Theme.FluentUI2/Flare.Theme.FluentUI2.csproj",                                  "src/Flare.Theme.FluentUI2/"]
COPY ["src/Flare.Theme.Aero/Flare.Theme.Aero.csproj",                                            "src/Flare.Theme.Aero/"]
COPY ["src/Flare.Theme.LiquidGlass/Flare.Theme.LiquidGlass.csproj",                              "src/Flare.Theme.LiquidGlass/"]
COPY ["src/Flare.Theme.VisualStudio/Flare.Theme.VisualStudio.csproj",                            "src/Flare.Theme.VisualStudio/"]
COPY ["src/Flare/Flare.csproj",                                                                  "src/Flare/"]
COPY ["samples/Flare.Legacy/Flare.Legacy.csproj",                                                "samples/Flare.Legacy/"]
# Directory.Build.props must be present so referenced projects get <TargetFramework>
COPY Directory.Build.props .
RUN dotnet restore "samples/Flare.Legacy/Flare.Legacy.csproj"

# Copy full source and publish
COPY . .
RUN dotnet publish "samples/Flare.Legacy/Flare.Legacy.csproj" -c Release -o /publish --no-restore

# ── Stage 2: Serve ────────────────────────────────────────────
FROM nginx:alpine AS final
COPY --from=build /publish/wwwroot /usr/share/nginx/html
COPY nginx.conf /etc/nginx/nginx.conf
EXPOSE 80
