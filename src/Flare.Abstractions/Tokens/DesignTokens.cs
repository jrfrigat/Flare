using Flare.Abstractions.Tokens.Components;

namespace Flare.Abstractions.Tokens;

/// <summary>
/// The non-color half of a theme: typography, shape, motion, state, elevation GEOMETRY,
/// component tokens, focus ring and extended overrides. Mode-independent -- the same
/// <see cref="DesignTokens"/> serves both light and dark; only the <see cref="ColorScheme"/>
/// changes between modes. Elevation shadow COLOR lives in <see cref="ColorScheme"/>
/// (ShadowUmbra/ShadowPenumbra); here Elevation holds only the geometry referencing them.
/// Every field is overridable via <c>with</c> from a published reference instance.
/// </summary>
public sealed record DesignTokens
{
    /// <summary>CSS value for the focus ring outline.</summary>
    public required string FocusRing { get; init; }

    /// <summary>Typography token.</summary>
    public required TypographyTokens Typography { get; init; }
    /// <summary>Shape token.</summary>
    public required ShapeTokens Shape { get; init; }
    /// <summary>Elevation token.</summary>
    public required ElevationTokens Elevation { get; init; }
    /// <summary>Motion token.</summary>
    public required MotionTokens Motion { get; init; }
    /// <summary>State token.</summary>
    public required StateTokens State { get; init; }

    /// <summary>Spacing scale (padding/margin/gap). A theme may tighten or loosen the whole scale
    /// via <c>with</c> to make the UI denser or roomier.</summary>
    public required SpacingTokens Spacing { get; init; }

    /// <summary>Badge token.</summary>
    public required BadgeTokens Badge { get; init; }
    /// <summary>Alert token.</summary>
    public required AlertTokens Alert { get; init; }
    /// <summary>Button token.</summary>
    public required ButtonTokens Button { get; init; }
    /// <summary>Split button token.</summary>
    public required SplitButtonTokens SplitButton { get; init; }
    /// <summary>Toggle button token.</summary>
    public required ToggleButtonTokens ToggleButton { get; init; }
    /// <summary>Fab token.</summary>
    public required FabTokens Fab { get; init; }
    /// <summary>Menu token.</summary>
    public required MenuTokens Menu { get; init; }
    /// <summary>Checkbox token.</summary>
    public required CheckboxTokens Checkbox { get; init; }
    /// <summary>Radio token.</summary>
    public required RadioTokens Radio { get; init; }
    /// <summary>Chip token.</summary>
    public required ChipTokens Chip { get; init; }
    /// <summary>Tabs token.</summary>
    public required TabsTokens Tabs { get; init; }

    /// <summary>Slider geometry/colors. Defaults to MD3 Expressive; Fluent overrides to a thin rail.</summary>
    public required SliderTokens Slider { get; init; }

    /// <summary>Input/Select/TextArea form field tokens.</summary>
    public InputTokens Input { get; init; } = new();

    /// <summary>Dialog modal tokens.</summary>
    public required DialogTokens Dialog { get; init; }

    /// <summary>Drawer navigation tokens.</summary>
    public required DrawerTokens Drawer { get; init; }

    /// <summary>Snackbar notification tokens.</summary>
    public required SnackbarTokens Snackbar { get; init; }

    /// <summary>Tooltip tokens.</summary>
    public required TooltipTokens Tooltip { get; init; }

    /// <summary>Popover tokens.</summary>
    public required PopoverTokens Popover { get; init; }

    /// <summary>DataGrid tokens.</summary>
    public DataGridTokens DataGrid { get; init; } = new();

    /// <summary>Card tokens.</summary>
    public CardTokens Card { get; init; } = new();

    /// <summary>Avatar tokens.</summary>
    public required AvatarTokens Avatar { get; init; }

    /// <summary>Progress tokens.</summary>
    public ProgressTokens Progress { get; init; } = new();

    /// <summary>Switch tokens.</summary>
    public SwitchTokens Switch { get; init; } = new();

    /// <summary>Navigation (nav item + active indicator) tokens.</summary>
    public required NavTokens Nav { get; init; }

    /// <summary>Mobile bottom-navigation bar tokens.</summary>
    public required BottomNavTokens BottomNav { get; init; }

    /// <summary>FlareTableOfContents / FlareOnThisPage tokens.</summary>
    public required TableOfContentsTokens TableOfContents { get; init; }

    /// <summary>FlareColorPicker checkerboard + native range-thumb tokens.</summary>
    public required ColorPickerTokens ColorPicker { get; init; }

    /// <summary>Theme-specific extras not in the core schema (e.g. Fluent focus ring vars).</summary>
    public IReadOnlyDictionary<string, string> Extended { get; init; }
        = new Dictionary<string, string>();
}
