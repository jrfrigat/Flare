# The SDK tag is PINNED to the version global.json floors, and must not be floated back to `10.0`.
# That floating tag currently resolves to 10.0.301, which is below the floor, so `dotnet restore` exits
# 155 with "Install the [10.0.400] .NET SDK" before a single project is read - and because compose keeps
# the last good container when a build fails, the site simply went on serving a week-old image with no
# error in sight. The floor is not arbitrary: the Gallery's source generator references Roslyn 5.9.0, and
# a generator may not reference a Roslyn newer than the compiler loading it. On 10.0.301 the SDK would
# drop the analyzer with CS9057 - a warning - and the publish would fail far away with CS0103 on the
# types that quietly stopped being generated. Raise this tag and global.json together, never one alone.
#
# ── Stage 1: Gather every project file ───────────────────────
# This stage exists so restore stays layer-cached without anybody having to maintain a list of
# projects by hand. That list was maintained by hand, it fell behind as projects were added, and the
# build then failed at publish - long after restore had appeared to succeed - so the image silently
# stopped updating. Anything derived from the repo cannot fall behind it.
FROM mcr.microsoft.com/dotnet/sdk:10.0.400 AS projects
WORKDIR /src
COPY . .
# The pruned tree is byte-identical whenever no project file changed, so the layer that copies it
# into the build stage keeps its digest and the restore below stays cached.
RUN mkdir -p /projects && \
    find . -type f \( -name "*.csproj" -o -name "Directory.Build.props" -o -name "Directory.Packages.props" \
        -o -name "global.json" \) \
        -exec install -D {} /projects/{} \;

# ── Stage 2: Build ───────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0.400 AS build
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
