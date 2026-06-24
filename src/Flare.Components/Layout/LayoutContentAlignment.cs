namespace Flare.Components;

/// <summary>
/// Horizontal placement of a width-capped content frame within the available area of
/// <c>FlareLayout</c> / <c>FlareLayoutContent</c>. Only has a visible effect when the content is
/// narrower than the available width (i.e. a <see cref="ContainerMaxWidth"/> cap is applied);
/// at full width there is no free space to distribute.
/// </summary>
public enum LayoutContentAlignment
{
    /// <summary>Pin the content to the start (left in LTR, right in RTL).</summary>
    Start,

    /// <summary>Center the content horizontally (the default).</summary>
    Center,

    /// <summary>Pin the content to the end (right in LTR, left in RTL).</summary>
    End,
}
