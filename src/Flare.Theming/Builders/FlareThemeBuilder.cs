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
/// var theme = new FlareThemeBuilder("my-theme", "My Custom Theme")
///     .WithTypography(t => t with { BodyLarge = new TypeStyle { FontFamily = "Inter", ... } })
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

    /// <summary>Flare theme builder.</summary>
    public FlareThemeBuilder(string id, string displayName)
    {
        _id = id;
        _displayName = displayName;
        _design = DefaultDesignTokens();
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

    private static DesignTokens DefaultDesignTokens() => new()
    {
        FocusRing = "2px solid var(--flare-color-primary)",
        Typography = new TypographyTokens
        {
            DisplayLarge = DefaultTypeStyle("Roboto", "400", "2.25rem", "2.75rem", "-0.015625em"),
            DisplayMedium = DefaultTypeStyle("Roboto", "400", "1.75rem", "2.25rem", "0em"),
            DisplaySmall = DefaultTypeStyle("Roboto", "400", "1.375rem", "1.75rem", "0em"),
            HeadlineLarge = DefaultTypeStyle("Roboto", "400", "1.25rem", "1.75rem", "0em"),
            HeadlineMedium = DefaultTypeStyle("Roboto", "400", "1rem", "1.5rem", "0em"),
            HeadlineSmall = DefaultTypeStyle("Roboto", "400", "0.875rem", "1.25rem", "0em"),
            TitleLarge = DefaultTypeStyle("Roboto", "500", "1rem", "1.5rem", "0.009375em"),
            TitleMedium = DefaultTypeStyle("Roboto", "500", "0.875rem", "1.25rem", "0.00625em"),
            TitleSmall = DefaultTypeStyle("Roboto", "500", "0.75rem", "1rem", "0.00625em"),
            BodyLarge = DefaultTypeStyle("Roboto", "400", "1rem", "1.5rem", "0.03125em"),
            BodyMedium = DefaultTypeStyle("Roboto", "400", "0.875rem", "1.25rem", "0.015625em"),
            BodySmall = DefaultTypeStyle("Roboto", "400", "0.75rem", "1rem", "0.025em"),
            LabelLarge = DefaultTypeStyle("Roboto", "500", "0.875rem", "1.25rem", "0.00625em"),
            LabelMedium = DefaultTypeStyle("Roboto", "500", "0.75rem", "1rem", "0.03125em"),
            LabelSmall = DefaultTypeStyle("Roboto", "500", "0.6875rem", "1rem", "0.03125em"),
        },
        Shape = new ShapeTokens
        {
            None = "0px",
            ExtraSmall = "4px",
            Small = "8px",
            Medium = "12px",
            Large = "16px",
            ExtraLarge = "28px",
            Full = "9999px"
        },
        Elevation = new ElevationTokens
        {
            Level0 = "none",
            Level1 = "0 1px 2px rgba(0,0,0,0.3), 0 1px 3px 1px rgba(0,0,0,0.15)",
            Level2 = "0 1px 2px rgba(0,0,0,0.3), 0 2px 6px 2px rgba(0,0,0,0.15)",
            Level3 = "0 4px 8px 3px rgba(0,0,0,0.15), 0 1px 3px rgba(0,0,0,0.3)",
            Level4 = "0 6px 10px 4px rgba(0,0,0,0.15), 0 2px 3px rgba(0,0,0,0.3)",
            Level5 = "0 8px 12px 6px rgba(0,0,0,0.15), 0 4px 4px rgba(0,0,0,0.3)"
        },
        Motion = new MotionTokens
        {
            DurationShort1 = "50ms",
            DurationShort2 = "100ms",
            DurationMedium1 = "200ms",
            DurationMedium2 = "300ms",
            DurationLong1 = "400ms",
            DurationLong2 = "500ms",
            EasingStandard = "cubic-bezier(0.2, 0, 0, 1)",
            EasingDecelerate = "cubic-bezier(0, 0, 0, 1)",
            EasingAccelerate = "cubic-bezier(0.3, 0, 1, 1)",
            EasingEmphasized = "cubic-bezier(0.2, 0, 0, 1)"
        },
        State = new StateTokens
        {
            HoverOpacity = "0.08",
            FocusOpacity = "0.12",
            PressedOpacity = "0.12",
            DraggedOpacity = "0.16",
            DisabledOpacity = "0.38",
            DisabledContainerOpacity = "0.12"
        },
        Badge = new BadgeTokens(),
        Alert = new AlertTokens(),
        Button = new ButtonTokens(),
        SplitButton = new SplitButtonTokens(),
        ToggleButton = new ToggleButtonTokens(),
        Fab = new FabTokens(),
        Menu = new MenuTokens(),
        Checkbox = new CheckboxTokens(),
        Radio = new RadioTokens(),
        Chip = new ChipTokens(),
        Tabs = new TabsTokens(),
        Slider = new SliderTokens(),
    };

    private static TypeStyle DefaultTypeStyle(string family, string weight, string size, string height, string spacing) => new()
    {
        FontFamily = family,
        FontWeight = weight,
        FontSize = size,
        LineHeight = height,
        LetterSpacing = spacing
    };

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
