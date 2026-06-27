# ── Stage 1: Build ───────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /repo

# Restore dependencies first (layer-cached unless .csproj files change)
COPY ["src/Flare.Abstractions/Flare.Abstractions.csproj",                           "src/Flare.Abstractions/"]
COPY ["src/Flare.Theming/Flare.Theming.csproj",                                     "src/Flare.Theming/"]
COPY ["src/Flare.Infrastructure/Flare.Infrastructure.csproj",                       "src/Flare.Infrastructure/"]
COPY ["src/Flare.Components/Flare.Components.csproj",                               "src/Flare.Components/"]
COPY ["src/Flare.Components.IDE/Flare.Components.IDE.csproj",                       "src/Flare.Components.IDE/"]
COPY ["src/Flare.Components.Media/Flare.Components.Media.csproj",                   "src/Flare.Components.Media/"]
COPY ["src/Flare.Components.Carousel/Flare.Components.Carousel.csproj",             "src/Flare.Components.Carousel/"]
COPY ["src/Flare.Components.Kanban/Flare.Components.Kanban.csproj",                 "src/Flare.Components.Kanban/"]
COPY ["src/Flare.Components.QrCode/Flare.Components.QrCode.csproj",                 "src/Flare.Components.QrCode/"]
COPY ["src/Flare.Components.RichTextEditor/Flare.Components.RichTextEditor.csproj", "src/Flare.Components.RichTextEditor/"]
COPY ["src/Flare.Components.Transfer/Flare.Components.Transfer.csproj",             "src/Flare.Components.Transfer/"]
COPY ["src/Flare.Theme.MaterialDesign3Expressive/Flare.Theme.MaterialDesign3Expressive.csproj", "src/Flare.Theme.MaterialDesign3Expressive/"]
COPY ["src/Flare.Theme.MaterialDesign3/Flare.Theme.MaterialDesign3.csproj",         "src/Flare.Theme.MaterialDesign3/"]
COPY ["src/Flare.Theme.MaterialDesign2/Flare.Theme.MaterialDesign2.csproj",         "src/Flare.Theme.MaterialDesign2/"]
COPY ["src/Flare.Theme.FluentUI2/Flare.Theme.FluentUI2.csproj",                     "src/Flare.Theme.FluentUI2/"]
COPY ["src/Flare.Theme.Aero/Flare.Theme.Aero.csproj",                               "src/Flare.Theme.Aero/"]
COPY ["src/Flare.Theme.LiquidGlass/Flare.Theme.LiquidGlass.csproj",                 "src/Flare.Theme.LiquidGlass/"]
COPY ["src/Flare.Theme.VisualStudio/Flare.Theme.VisualStudio.csproj",               "src/Flare.Theme.VisualStudio/"]
COPY ["src/Flare/Flare.csproj",                                                     "src/Flare/"]
COPY ["samples/Flare.Gallery.Generators/Flare.Gallery.Generators.csproj",           "samples/Flare.Gallery.Generators/"]
COPY ["samples/Flare.Gallery/Flare.Gallery.csproj",                                 "samples/Flare.Gallery/"]
# Directory.Build.props must be present so referenced projects get <TargetFramework>
COPY Directory.Build.props .
RUN dotnet restore "samples/Flare.Gallery/Flare.Gallery.csproj"

# Copy full source and publish
COPY . .
RUN dotnet publish "samples/Flare.Gallery/Flare.Gallery.csproj" -c Release -o /publish --no-restore

# ── Stage 2: Serve ────────────────────────────────────────────
FROM nginx:alpine AS final
COPY --from=build /publish/wwwroot /usr/share/nginx/html
COPY nginx.conf /etc/nginx/nginx.conf
EXPOSE 80
