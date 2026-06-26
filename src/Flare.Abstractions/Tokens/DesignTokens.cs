using Flare.Core.Tokens.Components;

namespace Flare.Core.Tokens;

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

    /// <summary>Spacing scale (padding/margin/gap). Defaults to a 2px-base rem scale; a theme may
    /// override it to make the whole UI denser or roomier.</summary>
    public SpacingTokens Spacing { get; init; } = new();

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
    public SliderTokens Slider { get; init; } = new();

    /// <summary>Input/Select/TextArea form field tokens.</summary>
    public InputTokens Input { get; init; } = new();

    /// <summary>Dialog modal tokens.</summary>
    public DialogTokens Dialog { get; init; } = new();

    /// <summary>Drawer navigation tokens.</summary>
    public DrawerTokens Drawer { get; init; } = new();

    /// <summary>Snackbar notification tokens.</summary>
    public SnackbarTokens Snackbar { get; init; } = new();

    /// <summary>Tooltip tokens.</summary>
    public TooltipTokens Tooltip { get; init; } = new();

    /// <summary>Popover tokens.</summary>
    public PopoverTokens Popover { get; init; } = new();

    /// <summary>DataGrid tokens.</summary>
    public DataGridTokens DataGrid { get; init; } = new();

    /// <summary>Card tokens.</summary>
    public CardTokens Card { get; init; } = new();

    /// <summary>Avatar tokens.</summary>
    public AvatarTokens Avatar { get; init; } = new();

    /// <summary>Progress tokens.</summary>
    public ProgressTokens Progress { get; init; } = new();

    /// <summary>Switch tokens.</summary>
    public SwitchTokens Switch { get; init; } = new();

    /// <summary>FlareTableOfContents / FlareOnThisPage tokens.</summary>
    public TableOfContentsTokens TableOfContents { get; init; } = new();

    /// <summary>Theme-specific extras not in the core schema (e.g. Fluent focus ring vars).</summary>
    public IReadOnlyDictionary<string, string> Extended { get; init; }
        = new Dictionary<string, string>();
}
