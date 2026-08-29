namespace Flare.Css.Tokens;

/// <summary>
/// Per-instance CSS custom properties: a component writes one on its own element and its own stylesheet
/// reads it back. They are channels, not design tokens.
/// </summary>
/// <remarks>
/// The distinction matters and is enforced elsewhere: a name here must NOT gain a <c>[CssVar]</c>
/// attribute or a token-record member, because the settable-token guard would then demand a value from
/// every theme for something no theme can meaningfully set - the angle of one clock hand, the column
/// span of one grid cell, the indent depth of one tree row.
/// <para>
/// They still belong in a registry. The writer and the reader are in different files - C# and CSS - so a
/// rename that updates one and not the other produces no error at all, just a component that quietly
/// stops moving.
/// </para>
/// </remarks>
public static class LocalVars
{
    /// <summary>CSS custom-property name for a grid cell's default column span.</summary>
    public const string ColSpan = "--flare-col-span";
    /// <summary>CSS custom-property name for a grid cell's column span at the xs breakpoint.</summary>
    public const string ColSpanXs = "--flare-col-span-xs";
    /// <summary>CSS custom-property name for a grid cell's column span at the sm breakpoint.</summary>
    public const string ColSpanSm = "--flare-col-span-sm";
    /// <summary>CSS custom-property name for a grid cell's column span at the md breakpoint.</summary>
    public const string ColSpanMd = "--flare-col-span-md";
    /// <summary>CSS custom-property name for a grid cell's column span at the lg breakpoint.</summary>
    public const string ColSpanLg = "--flare-col-span-lg";
    /// <summary>CSS custom-property name for a grid cell's column span at the xl breakpoint.</summary>
    public const string ColSpanXl = "--flare-col-span-xl";
    /// <summary>CSS custom-property name for a grid cell's column span at the xxl breakpoint.</summary>
    public const string ColSpanXxl = "--flare-col-span-xxl";
    /// <summary>CSS custom-property name for a grid cell's explicit start line.</summary>
    public const string ColStart = "--flare-col-start";
    /// <summary>CSS custom-property name for the column count of one grid instance.</summary>
    public const string LayoutCols = "--flare-layout-cols";

    /// <summary>CSS custom-property name for the rotation of one clock-dial hand.</summary>
    public const string DialAngle = "--flare-dial-angle";
    /// <summary>CSS custom-property name for the length of one clock-dial hand.</summary>
    public const string DialLength = "--flare-dial-len";

    /// <summary>CSS custom-property name for the colour of one customizer swatch.</summary>
    public const string Swatch = "--flare-swatch";
    /// <summary>CSS custom-property name for the rotation applied to one tab's label.</summary>
    public const string TabLabelRotation = "--flare-tab-label-rotation";
    /// <summary>CSS custom-property name for the row cap of one auto-growing textarea.</summary>
    public const string TextAreaMaxLines = "--flare-textarea-max-lines";
    /// <summary>CSS custom-property name for the nesting depth of one table-of-contents entry.</summary>
    public const string TocDepth = "--flare-toc-depth";
    /// <summary>CSS custom-property name for the indent of one virtual-tree row.</summary>
    public const string TreeIndent = "--flare-vtree-indent";
    /// <summary>CSS custom-property name for the marker colour of one chart data point.</summary>
    public const string ChartDot = "--flare-chart-dot";
}
