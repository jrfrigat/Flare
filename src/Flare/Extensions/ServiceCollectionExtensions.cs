using Flare.Infrastructure;
using Flare.Abstractions;
using Flare.Theming;
using Flare.Abstractions.Tokens;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Flare.Extensions;

/// <summary>
/// DI entry points for Flare. Flare ships no themes of its own -- themes are independent packages
/// (<c>Flare.Theme.*</c>). Reference the theme packages you want and either let
/// <see cref="FlareOptions.RegisterAllBuiltInThemes"/> auto-discover them from the loaded assemblies,
/// or register them explicitly with <see cref="AddFlareTheme"/> / <see cref="AddFlarePalette"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all Flare services. Themes and palettes are registered directly into
    /// ThemeService - not via DI - so the correct selection is available from the first render.
    /// </summary>
    public static IServiceCollection AddFlare(
        this IServiceCollection services,
        Action<FlareOptions>? configure = null)
    {
        var opts = new FlareOptions();
        configure?.Invoke(opts);

        // Themes/palettes added via AddFlareTheme/AddFlarePalette accumulate here; the ThemeService
        // factory reads this registry lazily at resolve time, so those calls may come before or
        // after AddFlare.
        var registry = GetOrAddRegistry(services);

        services.AddScoped<ICssVariableInjector, Flare.Components.Services.CssVariableInjector>();
        services.AddScoped<IThemeService>(sp =>
        {
            var injector = sp.GetRequiredService<ICssVariableInjector>();

            // Collect every theme/palette to register: assembly auto-discovery (opt-in), the
            // explicitly added ones, and the configured default theme.
            var themes = new List<ITheme>();
            var palettes = new List<Palette>();

            if (opts.RegisterAllBuiltInThemes)
            {
                themes.AddRange(DiscoverThemes());
                palettes.AddRange(DiscoverPalettes());
            }

            themes.AddRange(registry.Themes);
            palettes.AddRange(registry.Palettes);

            if (opts.DefaultTheme is { } configuredDefault)
                themes.Insert(0, configuredDefault);

            // Palettes a theme ships with travel with it.
            foreach (var t in themes.ToList())
                palettes.AddRange(t.Palettes);

            var defaultTheme = ResolveDefaultTheme(opts, themes);
            var defaultPalette = ResolveDefaultPalette(opts, defaultTheme, palettes);

            var service = new ThemeService(injector, defaultTheme, defaultPalette, opts.DefaultMode, opts.Delivery);

            foreach (var t in themes) service.RegisterTheme(t);
            foreach (var p in palettes) service.RegisterPalette(p);

            return service;
        });

        services.AddScoped<IThemeStorageService, LocalStorageThemeStorage>();
        services.AddScoped<Flare.Components.IBrowserStorage, BrowserStorage>();
        services.AddScoped<ISnackbarService, SnackbarService>();
        services.AddScoped<IDialogService, DialogService>();
        services.AddScoped<IMessageBoxService, MessageBoxService>();

        // Typed JS-interop services (wrap Flare's JS so components inject a service, not IJSRuntime).
        services.AddScoped<Flare.Components.IFlareClipboard, Flare.Components.FlareClipboardService>();
        services.AddScoped<Flare.Components.IFlareDownload, Flare.Components.FlareDownloadService>();
        services.AddScoped<Flare.Components.IFlareColorExtractor, Flare.Components.FlareColorExtractor>();

        // Collision and Theme JS services.
        services.AddScoped<ICollisionService, Flare.Components.Services.CollisionService>();
        services.AddScoped<IThemeJsService, Flare.Components.Services.ThemeJsService>();
        services.AddScoped<Flare.Components.Services.ISplitterJsService, Flare.Components.Services.SplitterJsService>();
        services.AddScoped<Flare.Components.Services.ITreeJsService, Flare.Components.Services.TreeJsService>();
        services.AddScoped<Flare.Components.Services.IOverlayJsService, Flare.Components.Services.OverlayJsService>();

        return services;
    }

    /// <summary>
    /// Explicitly registers a theme (and the palettes it ships with). Use this instead of
    /// <see cref="FlareOptions.RegisterAllBuiltInThemes"/> when you want full control over which
    /// themes end up in the app. May be called before or after <see cref="AddFlare"/>.
    /// </summary>
    public static IServiceCollection AddFlareTheme(this IServiceCollection services, ITheme theme)
    {
        ArgumentNullException.ThrowIfNull(theme);
        var registry = GetOrAddRegistry(services);
        registry.Themes.Add(theme);
        registry.Palettes.AddRange(theme.Palettes);
        return services;
    }

    /// <summary>
    /// Explicitly registers a standalone palette so it is selectable at runtime regardless of the
    /// active theme. May be called before or after <see cref="AddFlare"/>.
    /// </summary>
    public static IServiceCollection AddFlarePalette(this IServiceCollection services, Palette palette)
    {
        ArgumentNullException.ThrowIfNull(palette);
        var registry = GetOrAddRegistry(services);
        registry.Palettes.Add(palette);
        return services;
    }

    /// <summary>
    /// Registers the Flare version-check service (<see cref="IVersionCheckService"/>). It polls for a
    /// newer app version on the configured interval and raises <see cref="IVersionCheckService.NewVersionAvailable"/>;
    /// it renders nothing - a layout subscribes and surfaces the update (e.g. a toast). Independent of
    /// <see cref="AddFlare"/>; can be called on its own.
    /// </summary>
    public static IServiceCollection AddFlareVersionCheck(
        this IServiceCollection services,
        Action<FlareVersionCheckOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        services.AddScoped<IVersionCheckService>(sp =>
        {
            var opts = new FlareVersionCheckOptions();
            configure(opts);
            // IJSRuntime is only needed for the built-in service-worker probe; resolved best-effort.
            return new Flare.Components.Services.VersionCheckService(opts, sp.GetService<Microsoft.JSInterop.IJSRuntime>());
        });
        return services;
    }

    private static FlareThemeRegistry GetOrAddRegistry(IServiceCollection services)
    {
        foreach (var d in services)
            if (d.ServiceType == typeof(FlareThemeRegistry) && d.ImplementationInstance is FlareThemeRegistry existing)
                return existing;

        var registry = new FlareThemeRegistry();
        services.AddSingleton(registry);
        return registry;
    }

    private static ITheme ResolveDefaultTheme(FlareOptions opts, IReadOnlyList<ITheme> themes)
    {
        if (opts.DefaultTheme is { } explicitTheme)
            return explicitTheme;

        if (!string.IsNullOrEmpty(opts.DefaultThemeId))
        {
            var byId = themes.FirstOrDefault(t => t.Id == opts.DefaultThemeId);
            if (byId is not null) return byId;
            throw new InvalidOperationException(
                $"Flare: DefaultThemeId '{opts.DefaultThemeId}' was not found among the registered themes. " +
                "Reference the theme package or register it via AddFlareTheme.");
        }

        if (themes.Count > 0) return themes[0];

        throw new InvalidOperationException(
            "Flare: no theme is registered. Flare ships no themes -- reference a theme package " +
            "(e.g. Flare.Theme.MaterialDesign3Expressive) and either keep RegisterAllBuiltInThemes " +
            "enabled or register one via AddFlareTheme / FlareOptions.DefaultTheme.");
    }

    private static Palette ResolveDefaultPalette(FlareOptions opts, ITheme defaultTheme, IReadOnlyList<Palette> palettes)
    {
        if (opts.DefaultPalette is { } explicitPalette)
            return explicitPalette;

        if (!string.IsNullOrEmpty(opts.DefaultPaletteId))
        {
            var byId = palettes.FirstOrDefault(p => p.Id == opts.DefaultPaletteId);
            if (byId is not null) return byId;
            throw new InvalidOperationException(
                $"Flare: DefaultPaletteId '{opts.DefaultPaletteId}' was not found among the registered palettes.");
        }

        // Prefer the chosen theme's own default palette, then any registered palette.
        var themeDefault = palettes.FirstOrDefault(p => p.Id == defaultTheme.DefaultPaletteId)
            ?? defaultTheme.Palettes.FirstOrDefault(p => p.Id == defaultTheme.DefaultPaletteId)
            ?? defaultTheme.Palettes.FirstOrDefault()
            ?? palettes.FirstOrDefault();

        return themeDefault ?? throw new InvalidOperationException(
            $"Flare: theme '{defaultTheme.Id}' has no palette registered. " +
            "Register one via AddFlarePalette or FlareOptions.DefaultPalette.");
    }

    /// <summary>
    /// Instantiates every concrete <see cref="ITheme"/> (public parameterless ctor) found in the
    /// currently loaded assemblies. This is how referenced theme packages are picked up without
    /// Flare referencing any of them.
    /// </summary>
    private static IEnumerable<ITheme> DiscoverThemes() =>
        DiscoverInstances<ITheme>();

    /// <summary>Instantiates every <see cref="IPaletteProvider"/> and flattens their palettes.</summary>
    private static IEnumerable<Palette> DiscoverPalettes() =>
        DiscoverInstances<IPaletteProvider>().SelectMany(p => p.Palettes);

    private static readonly string CoreAssemblyName = typeof(ITheme).Assembly.GetName().Name!;

    private static IEnumerable<T> DiscoverInstances<T>() where T : class
    {
        foreach (var assembly in CandidateAssemblies())
        {
            Type[] types;
            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                types = ex.Types.Where(t => t is not null).ToArray()!;
            }
            catch
            {
                continue;
            }

            foreach (var type in types)
            {
                if (type is null || type.IsAbstract || type.IsInterface || type.IsGenericTypeDefinition) continue;
                if (!typeof(T).IsAssignableFrom(type)) continue;
                if (type.GetConstructor(Type.EmptyTypes) is null) continue;

                T? instance = null;
                try { instance = (T)Activator.CreateInstance(type)!; }
                catch { /* skip themes/providers that fail to construct */ }
                if (instance is not null) yield return instance;
            }
        }
    }

    /// <summary>
    /// The assemblies that may contain a theme/palette provider: Flare.Core's own assembly plus any
    /// assembly that references it. Referenced theme packages whose types are never touched in code are
    /// not loaded into the (WASM) AppDomain on their own, so we first force-load the whole reference
    /// graph; otherwise auto-discovery would only see themes the app happens to use statically.
    /// </summary>
    private static IEnumerable<Assembly> CandidateAssemblies()
    {
        EnsureReferencedAssembliesLoaded();

        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            var name = asm.GetName().Name;
            if (name == CoreAssemblyName) { yield return asm; continue; }

            bool referencesCore = false;
            try { referencesCore = asm.GetReferencedAssemblies().Any(n => n.Name == CoreAssemblyName); }
            catch { /* dynamic/reflection-only assemblies */ }
            if (referencesCore) yield return asm;
        }
    }

    /// <summary>
    /// Walks the reference graph of the loaded assemblies and loads any not-yet-loaded referenced
    /// assembly, so a theme package that is referenced (but otherwise unused) becomes discoverable.
    /// Already-loaded assemblies (the bulk of the BCL) are skipped, so this loads only the few missing.
    /// </summary>
    private static void EnsureReferencedAssembliesLoaded()
    {
        var seen = new HashSet<string>(
            AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetName().Name!),
            StringComparer.OrdinalIgnoreCase);

        var queue = new Queue<Assembly>(AppDomain.CurrentDomain.GetAssemblies());
        while (queue.Count > 0)
        {
            AssemblyName[] referenced;
            try { referenced = queue.Dequeue().GetReferencedAssemblies(); }
            catch { continue; }

            foreach (var refName in referenced)
            {
                if (refName.Name is null || !seen.Add(refName.Name)) continue;
                try { queue.Enqueue(Assembly.Load(refName)); }
                catch { /* unresolved/lazy-load-only assembly */ }
            }
        }
    }
}

/// <summary>Mutable, DI-shared collection of themes/palettes added via AddFlareTheme / AddFlarePalette.</summary>
internal sealed class FlareThemeRegistry
{
    public List<ITheme> Themes { get; } = [];
    public List<Palette> Palettes { get; } = [];
}

/// <summary>Configuration for <see cref="ServiceCollectionExtensions.AddFlare"/>.</summary>
public sealed class FlareOptions
{
    /// <summary>
    /// The default design-system theme. When null, the default is resolved from
    /// <see cref="DefaultThemeId"/>, else the first registered theme. Flare ships no themes, so
    /// at least one theme must be referenced/registered.
    /// </summary>
    public ITheme? DefaultTheme { get; set; }

    /// <summary>
    /// Selects the default theme by id from the registered/auto-discovered themes, without taking a
    /// compile-time dependency on the theme type. Ignored when <see cref="DefaultTheme"/> is set.
    /// </summary>
    public string? DefaultThemeId { get; set; }

    /// <summary>The default color palette. When null, the chosen theme's default palette is used.</summary>
    public Palette? DefaultPalette { get; set; }

    /// <summary>Selects the default palette by id. Ignored when <see cref="DefaultPalette"/> is set.</summary>
    public string? DefaultPaletteId { get; set; }

    /// <summary>The default light/dark mode. Defaults to <see cref="ThemeMode.Auto"/>.</summary>
    public ThemeMode DefaultMode { get; set; } = ThemeMode.Auto;

    /// <summary>How theme CSS is delivered. Defaults to <see cref="ThemeDelivery.ClassToggle"/> (fastest).</summary>
    public ThemeDelivery Delivery { get; set; } = ThemeDelivery.ClassToggle;

    /// <summary>
    /// When true (default), every <see cref="ITheme"/> and <see cref="IPaletteProvider"/> found in the
    /// <em>loaded</em> assemblies is registered automatically (plus their <see cref="ITheme.Palettes"/>).
    /// <para>
    /// Note: a theme package that is referenced but never touched in code may not be loaded by a
    /// trimmed/WASM app (the runtime only loads assemblies it needs), so it would not be discovered.
    /// The reliable way to register such a theme is the explicit
    /// <see cref="ServiceCollectionExtensions.AddFlareTheme"/>, which also forces the theme assembly to
    /// load. Set this to false to opt out of scanning entirely.
    /// </para>
    /// <para>
    /// <b>Performance / trimming:</b> auto-discovery walks and force-loads the whole assembly reference
    /// graph at startup (<c>Assembly.Load</c> + <c>GetTypes()</c> on every assembly), which adds startup
    /// cost, defeats lazy-loading, and is not trim/AOT-safe. For the smallest, fastest, trim-friendly
    /// startup, register themes explicitly with <see cref="ServiceCollectionExtensions.AddFlareTheme"/>
    /// and set this to <c>false</c>.
    /// </para>
    /// </summary>
    public bool RegisterAllBuiltInThemes { get; set; } = true;
}
