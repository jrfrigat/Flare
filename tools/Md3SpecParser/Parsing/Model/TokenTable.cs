using System.Text.Json.Serialization;

namespace Flare.Tools.Md3SpecParser.Parsing.Model;

/// <summary>Root of a Material Design <c>TOKEN_TABLE</c> JSON document.</summary>
public sealed class TokenTableRoot
{
    /// <summary>The single <c>system</c> object holding all data.</summary>
    [JsonPropertyName("system")]
    public SystemModel? System { get; set; }
}

/// <summary>The design-system payload: components, token sets, tokens and values.</summary>
public sealed class SystemModel
{
    /// <summary>Components; each lists its token sets in display order.</summary>
    [JsonPropertyName("components")]
    public List<ComponentModel> Components { get; set; } = new();

    /// <summary>All token sets (a set ≈ one markdown section / heading).</summary>
    [JsonPropertyName("tokenSets")]
    public List<TokenSetModel> TokenSets { get; set; } = new();

    /// <summary>All tokens (a token ≈ one table row).</summary>
    [JsonPropertyName("tokens")]
    public List<TokenModel> Tokens { get; set; } = new();

    /// <summary>All values; a token may have several (per context).</summary>
    [JsonPropertyName("values")]
    public List<ValueModel> Values { get; set; } = new();

    /// <summary>Display groups used to order tokens within a set.</summary>
    [JsonPropertyName("displayGroups")]
    public List<DisplayGroupModel> DisplayGroups { get; set; } = new();

    /// <summary>Context tag groups; each declares a default tag.</summary>
    [JsonPropertyName("contextTagGroups")]
    public List<ContextTagGroupModel> ContextTagGroups { get; set; } = new();

    /// <summary>All context tags (each belongs to a group).</summary>
    [JsonPropertyName("tags")]
    public List<TagModel> Tags { get; set; } = new();
}

/// <summary>A single context tag (e.g. "Light", "3P").</summary>
public sealed class TagModel
{
    /// <summary>Resource name (<c>.../contextTagGroups/&lt;g&gt;/tags/&lt;id&gt;</c>).</summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Tag label, e.g. "Light", "Dark", "3P".</summary>
    [JsonPropertyName("displayName")]
    public string DisplayName { get; set; } = string.Empty;
}

/// <summary>A component definition with its ordered token-set references.</summary>
public sealed class ComponentModel
{
    /// <summary>Resource name of the component.</summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Human-readable component name.</summary>
    [JsonPropertyName("displayName")]
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>Token-set resource names, in the order they should be rendered.</summary>
    [JsonPropertyName("tokenSets")]
    public List<string> TokenSets { get; set; } = new();
}

/// <summary>A token set - rendered as one <c># Heading</c> + table.</summary>
public sealed class TokenSetModel
{
    /// <summary>Resource name (matched against <see cref="ComponentModel.TokenSets"/>).</summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Heading text, e.g. "Button group standard - Size - Xsmall".</summary>
    [JsonPropertyName("displayName")]
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>Dotted token-set name, e.g. <c>md.comp.button-group.standard.xsmall</c>.</summary>
    [JsonPropertyName("tokenSetName")]
    public string TokenSetName { get; set; } = string.Empty;

    /// <summary>Authoring order (often identical across sets; kept for fallback).</summary>
    [JsonPropertyName("order")]
    public int Order { get; set; }
}

/// <summary>A single token - one table row.</summary>
public sealed class TokenModel
{
    /// <summary>Resource name (<c>.../tokenSets/&lt;set&gt;/tokens/&lt;id&gt;</c>).</summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Dotted token name shown in the "Token Name" column.</summary>
    [JsonPropertyName("tokenName")]
    public string TokenName { get; set; } = string.Empty;

    /// <summary>Human-readable name shown in the "Display Name" column.</summary>
    [JsonPropertyName("displayName")]
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>Optional display-group resource the token belongs to.</summary>
    [JsonPropertyName("displayGroup")]
    public string? DisplayGroup { get; set; }

    /// <summary>Order within its display group (or ungrouped bucket).</summary>
    [JsonPropertyName("orderInDisplayGroup")]
    public int OrderInDisplayGroup { get; set; }

    /// <summary>Value type hint (LENGTH, SHAPE, DURATION, NUMERIC, ...).</summary>
    [JsonPropertyName("tokenValueType")]
    public string? TokenValueType { get; set; }
}

/// <summary>A display group within a token set.</summary>
public sealed class DisplayGroupModel
{
    /// <summary>Resource name referenced by <see cref="TokenModel.DisplayGroup"/>.</summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Group label (e.g. "Pressed", "Enabled").</summary>
    [JsonPropertyName("displayName")]
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>Order of this group within its parent token set.</summary>
    [JsonPropertyName("orderInParentTokenSet")]
    public int OrderInParentTokenSet { get; set; }
}

/// <summary>A context tag group; its <see cref="DefaultTag"/> defines the baseline.</summary>
public sealed class ContextTagGroupModel
{
    /// <summary>Resource name of the group.</summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Group label, e.g. "Theme", "Audience", "Contrast".</summary>
    [JsonPropertyName("displayName")]
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>Resource name of the default tag for this group.</summary>
    [JsonPropertyName("defaultTag")]
    public string? DefaultTag { get; set; }
}

/// <summary>A token value (possibly context-scoped or an alias to another token).</summary>
public sealed class ValueModel
{
    /// <summary>Resource name (<c>.../tokens/&lt;id&gt;/values/&lt;vid&gt;</c>).</summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Length value, when this is a dimension/percentage.</summary>
    [JsonPropertyName("length")]
    public LengthModel? Length { get; set; }

    /// <summary>Shape value, when this is a corner shape.</summary>
    [JsonPropertyName("shape")]
    public ShapeModel? Shape { get; set; }

    /// <summary>Numeric value (e.g. spring damping/stiffness).</summary>
    [JsonPropertyName("numeric")]
    public double? Numeric { get; set; }

    /// <summary>Color value (sRGB components 0..1).</summary>
    [JsonPropertyName("color")]
    public ColorModel? Color { get; set; }

    /// <summary>Opacity value (0..1).</summary>
    [JsonPropertyName("opacity")]
    public double? Opacity { get; set; }

    /// <summary>Elevation value ({value, unit}).</summary>
    [JsonPropertyName("elevation")]
    public LengthModel? Elevation { get; set; }

    /// <summary>Composite typography value referencing font sub-tokens.</summary>
    [JsonPropertyName("type")]
    public TypographyModel? Type { get; set; }

    /// <summary>Font family names.</summary>
    [JsonPropertyName("fontNames")]
    public FontNamesModel? FontNames { get; set; }

    /// <summary>Font weight (e.g. 500).</summary>
    [JsonPropertyName("fontWeight")]
    public double? FontWeight { get; set; }

    /// <summary>Font size ({value, unit}).</summary>
    [JsonPropertyName("fontSize")]
    public LengthModel? FontSize { get; set; }

    /// <summary>Line height ({value, unit}).</summary>
    [JsonPropertyName("lineHeight")]
    public LengthModel? LineHeight { get; set; }

    /// <summary>Letter tracking ({value, unit}).</summary>
    [JsonPropertyName("fontTracking")]
    public LengthModel? FontTracking { get; set; }

    /// <summary>Variable-font axis value.</summary>
    [JsonPropertyName("axisValue")]
    public AxisValueModel? AxisValue { get; set; }

    /// <summary>Alias: dotted name of another token whose value should be used.</summary>
    [JsonPropertyName("tokenName")]
    public string? TokenName { get; set; }

    /// <summary>Context tags scoping this value (tag resource names).</summary>
    [JsonPropertyName("contextTags")]
    public List<string>? ContextTags { get; set; }

    /// <summary>Specificity score (string in source JSON); higher wins ties.</summary>
    [JsonPropertyName("specificityScore")]
    public string? SpecificityScore { get; set; }

    /// <summary>True when the value is explicitly undefined for its context.</summary>
    [JsonPropertyName("undefined")]
    public bool Undefined { get; set; }
}

/// <summary>A length value with a unit (also used for elevation/font dimensions).</summary>
public sealed class LengthModel
{
    /// <summary>Numeric magnitude (absent for some tracking values).</summary>
    [JsonPropertyName("value")]
    public double? Value { get; set; }

    /// <summary>Unit (DIPS, PERCENT, POINTS, ...).</summary>
    [JsonPropertyName("unit")]
    public string? Unit { get; set; }
}

/// <summary>A shape value.</summary>
public sealed class ShapeModel
{
    /// <summary>Shape family, e.g. <c>SHAPE_FAMILY_CIRCULAR</c> or <c>SHAPE_FAMILY_ROUNDED_CORNERS</c>.</summary>
    [JsonPropertyName("family")]
    public string? Family { get; set; }

    /// <summary>Corner size for rounded-corner shapes.</summary>
    [JsonPropertyName("defaultSize")]
    public LengthModel? DefaultSize { get; set; }
}

/// <summary>An sRGB color, components in 0..1.</summary>
public sealed class ColorModel
{
    /// <summary>Red 0..1.</summary>
    [JsonPropertyName("red")]
    public double Red { get; set; }

    /// <summary>Green 0..1.</summary>
    [JsonPropertyName("green")]
    public double Green { get; set; }

    /// <summary>Blue 0..1.</summary>
    [JsonPropertyName("blue")]
    public double Blue { get; set; }

    /// <summary>Alpha 0..1.</summary>
    [JsonPropertyName("alpha")]
    public double Alpha { get; set; } = 1;
}

/// <summary>Font family name list.</summary>
public sealed class FontNamesModel
{
    /// <summary>Ordered font family names (first is primary).</summary>
    [JsonPropertyName("values")]
    public List<string> Values { get; set; } = new();
}

/// <summary>A variable-font axis value.</summary>
public sealed class AxisValueModel
{
    /// <summary>Axis tag (e.g. <c>wght</c>).</summary>
    [JsonPropertyName("tag")]
    public string? Tag { get; set; }

    /// <summary>Axis value.</summary>
    [JsonPropertyName("value")]
    public double? Value { get; set; }
}

/// <summary>Composite typography value: each field names a font sub-token.</summary>
public sealed class TypographyModel
{
    /// <summary>Token holding the font family names.</summary>
    [JsonPropertyName("fontNameTokenName")]
    public string? FontNameTokenName { get; set; }

    /// <summary>Token holding the font weight.</summary>
    [JsonPropertyName("fontWeightTokenName")]
    public string? FontWeightTokenName { get; set; }

    /// <summary>Token holding the font size.</summary>
    [JsonPropertyName("fontSizeTokenName")]
    public string? FontSizeTokenName { get; set; }

    /// <summary>Token holding the letter tracking.</summary>
    [JsonPropertyName("fontTrackingTokenName")]
    public string? FontTrackingTokenName { get; set; }

    /// <summary>Token holding the line height.</summary>
    [JsonPropertyName("lineHeightTokenName")]
    public string? LineHeightTokenName { get; set; }
}
