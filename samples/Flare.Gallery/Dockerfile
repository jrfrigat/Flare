# ── Stage 1: Gather every project file ───────────────────────
# This stage exists so restore stays layer-cached without anybody having to maintain a list of
# projects by hand. That list was maintained by hand, it fell behind as projects were added, and the
# build then failed at publish - long after restore had appeared to succeed - so the image silently
# stopped updating. Anything derived from the repo cannot fall behind it.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS projects
WORKDIR /src
COPY . .
# The pruned tree is byte-identical whenever no project file changed, so the layer that copies it
# into the build stage keeps its digest and the restore below stays cached.
RUN mkdir -p /projects && \
    find . -type f \( -name "*.csproj" -o -name "Directory.Build.props" -o -name "Directory.Packages.props" \
        -o -name "global.json" \) \
        -exec install -D {} /projects/{} \;

# ── Stage 2: Build ───────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /repo

# The repo-level build-control files come across with the projects: Directory.Build.props (shared
# <TargetFramework> etc.), Directory.Packages.props (Central Package Management - the single source
# of every NuGet version; without it the version-less PackageReferences won't resolve) and
# global.json (the SDK floor, so restore picks the same compiler the publish below will use).
COPY --from=projects /projects .
RUN dotnet restore "samples/Flare.Gallery/Flare.Gallery.csproj"

# Copy full source and publish
COPY . .
RUN dotnet publish "samples/Flare.Gallery/Flare.Gallery.csproj" -c Release -o /publish --no-restore

# ── Stage 3: Serve ────────────────────────────────────────────
FROM nginx:alpine AS final
COPY --from=build /publish/wwwroot /usr/share/nginx/html
COPY nginx.conf /etc/nginx/nginx.conf
EXPOSE 80
