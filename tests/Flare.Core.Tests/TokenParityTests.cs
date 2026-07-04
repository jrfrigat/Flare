using Flare.Theming;
using Flare.Abstractions.Tokens;
using Flare.Abstractions.Tokens.Components;
using Flare.Theme.MaterialDesign3.Tokens;
using System.Reflection;

namespace Flare.Core.Tests;

/// <summary>
/// Verifies that DesignTokens, ColorScheme, and CssVarMap stay in sync.
/// When a new token field is added, the matching flatten mapping must be updated too.
/// </summary>
public class TokenParityTests
{
    private static Dictionary<string, string> Flat() =>
        CreateDefaultDesignTokens().Flatten(CreateDefaultColorScheme());

    [Fact]
    public void CssVarMap_Flatten_Should_Produce_AtLeast_200_Vars()
    {
        var flat = Flat();

        // At least 200 CSS variables (colors + typography + shape + elevation + motion + state + components)
        Assert.True(flat.Count >= 200,
            $"Expected at least 200 CSS variables, got {flat.Count}");
    }

    [Fact]
    public void CssVarMap_Flatten_Should_Contain_All_Color_Tokens()
    {
        var flat = Flat();

        Assert.Contains(Css.Tokens.Color.Primary, flat.Keys);
        Assert.Contains(Css.Tokens.Color.OnPrimary, flat.Keys);
        Assert.Contains(Css.Tokens.Color.Surface, flat.Keys);
        Assert.Contains(Css.Tokens.Color.Background, flat.Keys);
        Assert.Contains(Css.Tokens.Color.Error, flat.Keys);
        Assert.Contains(Css.Tokens.Color.ShadowUmbra, flat.Keys);
    }

    [Fact]
    public void CssVarMap_Flatten_Should_Contain_All_Design_Tokens()
    {
        var flat = Flat();

        Assert.Contains(Css.Tokens.Shape.Small, flat.Keys);
        Assert.Contains(Css.Tokens.Shape.Medium, flat.Keys);
        Assert.Contains(Css.Tokens.Elevation.Level1, flat.Keys);
        Assert.Contains(Css.Tokens.Motion.DurationShort1, flat.Keys);
        Assert.Contains(Css.Tokens.Motion.EasingStandard, flat.Keys);
        Assert.Contains(Css.Tokens.State.HoverOpacity, flat.Keys);
        Assert.Contains(Css.Tokens.Vars.FocusRing, flat.Keys);
    }

    [Fact]
    public void CssVarMap_Flatten_Should_Contain_All_Button_Tokens()
    {
        var flat = Flat();

        Assert.Contains(Css.Tokens.Button.Gap.Md, flat.Keys);
        Assert.Contains(Css.Tokens.Button.Height.Md, flat.Keys);
        Assert.Contains(Css.Tokens.Button.PaddingInline.Md, flat.Keys);
        Assert.Contains(Css.Tokens.Button.Radius.MdTopLeft, flat.Keys);
        Assert.Contains(Css.Tokens.Button.FocusOutline, flat.Keys);
        Assert.Contains(Css.Tokens.Button.IconSize.Md, flat.Keys);
    }

    [Fact]
    public void CssTokens_Constants_Should_Match_Hardcoded_Pattern()
    {
        // Verify that all Css.Tokens constants that look like CSS variables start with "--flare-"
        // Exclude helpers (Size, Side) which contain suffixes, the base "Vars.Flare" constant, and the
        // local per-instance color vars (LocalColor.*) which use the short "--fc-" prefix.
        var constants = GetAllCssTokenConstants()
            .Where(c => !c.Name.StartsWith("Size.") && !c.Name.StartsWith("Side.")
                        && !c.Name.StartsWith("LocalColor.") && c.Name != "Vars.Flare")
            .ToList();

        var badConstants = constants
            .Where(c => !c.Value.StartsWith("--flare-"))
            .ToList();

        Assert.Empty(badConstants);
    }

    [Fact]
    public void Flatten_Should_Merge_Both_Axes()
    {
        var design = CreateDefaultDesignTokens();
        var colors = CreateDefaultColorScheme();

        var flat = design.Flatten(colors);

        // The merged map carries values from both the color and the design axis
        Assert.Equal(colors.Primary, flat[Css.Tokens.Color.Primary]);
        Assert.Equal(design.Shape.Small, flat[Css.Tokens.Shape.Small]);
        Assert.Equal(design.Typography.BodyLarge.FontFamily, flat[Css.Tokens.Typography.Font("body-large")]);
    }

    [Fact]
    public void FlattenColors_Should_Produce_All_Color_Roles()
    {
        var colors = CreateDefaultColorScheme();
        var flat = colors.FlattenColors();

        // Should have all color roles (47 roles × 1 + 2 shadow extras = ~49)
        Assert.True(flat.Count >= 47,
            $"Expected at least 47 color roles, got {flat.Count}");
    }

    #region Helpers

    internal static DesignTokens CreateDefaultDesignTokens()
    {
        var typeStyle = new TypeStyle
        {
            FontFamily = "Roboto",
            FontWeight = "400",
            FontSize = "1rem",
            LineHeight = "1.5",
            LetterSpacing = "0em"
        };

        return MaterialDesignTokens.Design with
        {
            FocusRing = "2px solid blue",
            Typography = new TypographyTokens
            {
                DisplayLarge = typeStyle,
                DisplayMedium = typeStyle,
                DisplaySmall = typeStyle,
                HeadlineLarge = typeStyle,
                HeadlineMedium = typeStyle,
                HeadlineSmall = typeStyle,
                TitleLarge = typeStyle,
                TitleMedium = typeStyle,
                TitleSmall = typeStyle,
                BodyLarge = typeStyle,
                BodyMedium = typeStyle,
                BodySmall = typeStyle,
                LabelLarge = typeStyle,
                LabelMedium = typeStyle,
                LabelSmall = typeStyle
            },
            Shape = new ShapeTokens
            {
                None = "0",
                ExtraSmall = "2px",
                Small = "4px",
                Medium = "8px",
                Large = "12px",
                ExtraLarge = "16px",
                Full = "9999px"
            },
            Elevation = new ElevationTokens
            {
                Level0 = "none",
                Level1 = "0 1px 2px",
                Level2 = "0 2px 4px",
                Level3 = "0 4px 8px",
                Level4 = "0 8px 16px",
                Level5 = "0 16px 32px"
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
        };
    }

    private static ColorScheme CreateDefaultColorScheme()
    {
        return new ColorScheme
        {
            Primary = "#6750A4",
            OnPrimary = "#FFFFFF",
            PrimaryContainer = "#EADDFF",
            OnPrimaryContainer = "#21005D",
            Secondary = "#625B71",
            OnSecondary = "#FFFFFF",
            SecondaryContainer = "#E8DEF8",
            OnSecondaryContainer = "#1D192B",
            Tertiary = "#7D5260",
            OnTertiary = "#FFFFFF",
            TertiaryContainer = "#FFD8E4",
            OnTertiaryContainer = "#31111D",
            Error = "#B3261E",
            OnError = "#FFFFFF",
            ErrorContainer = "#F9DEDC",
            OnErrorContainer = "#410E0B",
            Success = "#2E6C47",
            OnSuccess = "#FFFFFF",
            SuccessContainer = "#B2F1C5",
            OnSuccessContainer = "#002111",
            Warning = "#7D5700",
            OnWarning = "#FFFFFF",
            WarningContainer = "#FFDEA6",
            OnWarningContainer = "#271900",
            Info = "#6750A4",
            OnInfo = "#FFFFFF",
            InfoContainer = "#EADDFF",
            OnInfoContainer = "#21005D",
            Surface = "#FFFBFE",
            OnSurface = "#1C1B1F",
            SurfaceVariant = "#E7E0EC",
            OnSurfaceVariant = "#49454F",
            SurfaceContainer = "#F3EDF7",
            SurfaceContainerLow = "#F7F2FA",
            SurfaceContainerHigh = "#ECE6F0",
            SurfaceContainerHighest = "#E6E0E9",
            Background = "#FFFBFE",
            OnBackground = "#1C1B1F",
            Outline = "#79747E",
            OutlineVariant = "#CAC4D0",
            InverseSurface = "#313033",
            InverseOnSurface = "#F4EFF4",
            InversePrimary = "#D0BCFF",
            Scrim = "#000000",
            Shadow = "#000000",
            ShadowUmbra = "rgba(0,0,0,0.3)",
            ShadowPenumbra = "rgba(0,0,0,0.15)"
        };
    }

    private static List<(string Name, string Value)> GetAllCssTokenConstants()
    {
        var result = new List<(string, string)>();

        // The token holder classes are top-level types in the Flare.Css.Tokens namespace
        // (Size, Side, Button, ..., Vars) -- the former nested types of the CssTokens class.
        var holders = typeof(Css.Tokens.Vars).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested && t.Namespace == "Flare.Css.Tokens");

        foreach (var holder in holders)
        {
            foreach (var field in holder.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy))
            {
                if (field.FieldType == typeof(string))
                {
                    var value = (string?)field.GetValue(null);
                    if (value is not null)
                        result.Add(($"{holder.Name}.{field.Name}", value));
                }
            }
        }

        return result;
    }

    #endregion
}
