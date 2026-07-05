using Flare.Abstractions;
using Flare.Theming;
using Flare.Abstractions.Tokens;
using Flare.Abstractions.Tokens.Components;

namespace Flare.Theming;

/// <summary>
/// Fluent API for building custom themes at runtime. Use this to create themes
/// without implementing <see cref="ITheme"/> directly.
/// </summary>
/// <example>
/// // Start from a theme package's published reference and override only what you need:
/// var theme = new FlareThemeBuilder("my-theme", "My Custom Theme", baseReference)
///     .WithShape(s => s with { Medium = "8px" })
///     .WithButton(b => b with { HeightMd = "48px" })
///     .WithStyleAsset("_content/MyApp/css/my-theme.css")
///     .Build();
/// </example>
public sealed class FlareThemeBuilder
{
    private readonly string _id;
    private readonly string _displayName;
    private DesignTokens _design;
    private readonly List<string> _styleAssets = [];
    private string _defaultPaletteId = "default";
    private IReadOnlyDictionary<string, string>? _extendedDarkOverride;
    private IPaletteGenerator? _paletteGenerator;

    /// <summary>
    /// Creates a theme builder starting from <paramref name="baseDesign"/>, then override only what you
    /// need via the <c>With*</c> methods. Pass a theme (or reference) package's published
    /// <c>DesignReference</c> as the base: Flare's core ships no
    /// built-in design baseline (design values live in theme packages, not the theme-agnostic core).
    /// </summary>
    public FlareThemeBuilder(string id, string displayName, DesignTokens baseDesign)
    {
        _id = id;
        _displayName = displayName;
        _design = baseDesign;
    }

    /// <summary>Sets the typography tokens.</summary>
    public FlareThemeBuilder WithTypography(TypographyTokens typography)
    {
        _design = _design with { Typography = typography };
        return this;
    }

    /// <summary>Sets the shape tokens.</summary>
    public FlareThemeBuilder WithShape(ShapeTokens shape)
    {
        _design = _design with { Shape = shape };
        return this;
    }

    /// <summary>Sets the elevation tokens.</summary>
    public FlareThemeBuilder WithElevation(ElevationTokens elevation)
    {
        _design = _design with { Elevation = elevation };
        return this;
    }

    /// <summary>Sets the motion tokens.</summary>
    public FlareThemeBuilder WithMotion(MotionTokens motion)
    {
        _design = _design with { Motion = motion };
        return this;
    }

    /// <summary>Sets the state tokens.</summary>
    public FlareThemeBuilder WithState(StateTokens state)
    {
        _design = _design with { State = state };
        return this;
    }

    /// <summary>Sets the button tokens.</summary>
    public FlareThemeBuilder WithButton(ButtonTokens button)
    {
        _design = _design with { Button = button };
        return this;
    }

    /// <summary>Sets the input tokens.</summary>
    public FlareThemeBuilder WithInput(InputTokens input)
    {
        _design = _design with { Input = input };
        return this;
    }

    /// <summary>Sets the dialog tokens.</summary>
    public FlareThemeBuilder WithDialog(DialogTokens dialog)
    {
        _design = _design with { Dialog = dialog };
        return this;
    }

    /// <summary>Sets the drawer tokens.</summary>
    public FlareThemeBuilder WithDrawer(DrawerTokens drawer)
    {
        _design = _design with { Drawer = drawer };
        return this;
    }

    /// <summary>Sets the snackbar tokens.</summary>
    public FlareThemeBuilder WithSnackbar(SnackbarTokens snackbar)
    {
        _design = _design with { Snackbar = snackbar };
        return this;
    }

    /// <summary>Sets the complete design tokens.</summary>
    public FlareThemeBuilder WithDesign(DesignTokens design)
    {
        _design = design;
        return this;
    }

    /// <summary>Adds a static CSS stylesheet asset.</summary>
    public FlareThemeBuilder WithStyleAsset(string href)
    {
        _styleAssets.Add(href);
        return this;
    }

    /// <summary>Adds multiple static CSS stylesheet assets.</summary>
    public FlareThemeBuilder WithStyleAssets(IEnumerable<string> hrefs)
    {
        _styleAssets.AddRange(hrefs);
        return this;
    }

    /// <summary>Sets the default palette ID.</summary>
    public FlareThemeBuilder WithDefaultPalette(string paletteId)
    {
        _defaultPaletteId = paletteId;
        return this;
    }

    /// <summary>Sets the dark mode extended overrides.</summary>
    public FlareThemeBuilder WithExtendedDarkOverride(IReadOnlyDictionary<string, string> overrides)
    {
        _extendedDarkOverride = overrides;
        return this;
    }

    /// <summary>Sets a custom palette generator.</summary>
    public FlareThemeBuilder WithPaletteGenerator(IPaletteGenerator generator)
    {
        _paletteGenerator = generator;
        return this;
    }

    /// <summary>Builds the theme. Validates the theme and throws if invalid.</summary>
    public ITheme Build()
    {
        var validator = new ThemeValidator();
        var theme = new BuiltTheme(
            _id, _displayName, _design,
            _styleAssets.ToArray(),
            _defaultPaletteId,
            _extendedDarkOverride,
            _paletteGenerator);

        var errors = validator.Validate(theme);
        if (errors.Count > 0)
            throw new InvalidOperationException(
                $"Theme validation failed:\n{string.Join("\n", errors)}");

        return theme;
    }

    /// <summary>Builds the theme without validation.</summary>
    public ITheme BuildUnsafe()
    {
        return new BuiltTheme(
            _id, _displayName, _design,
            _styleAssets.ToArray(),
            _defaultPaletteId,
            _extendedDarkOverride,
            _paletteGenerator);
    }

    private sealed record BuiltTheme : ITheme
    {
        public string Id { get; }
        public string DisplayName { get; }
        public DesignTokens Design { get; }
        public string DefaultPaletteId { get; }
        public IReadOnlyList<string> StyleAssets { get; }
        public IReadOnlyDictionary<string, string>? ExtendedDarkOverride { get; }
        public IPaletteGenerator? PaletteGenerator { get; }

        public BuiltTheme(string id, string displayName, DesignTokens design,
            string[] styleAssets, string defaultPaletteId,
            IReadOnlyDictionary<string, string>? extendedDarkOverride,
            IPaletteGenerator? paletteGenerator)
        {
            Id = id;
            DisplayName = displayName;
            Design = design;
            StyleAssets = styleAssets;
            DefaultPaletteId = defaultPaletteId;
            ExtendedDarkOverride = extendedDarkOverride;
            PaletteGenerator = paletteGenerator;
        }
    }
}
