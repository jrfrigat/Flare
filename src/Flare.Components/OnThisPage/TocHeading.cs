namespace Flare.Components;

/// <summary>A single heading discovered by <see cref="FlareOnThisPage"/>.</summary>
public sealed class TocHeading
{
    /// <summary>The heading's <c>id</c> (auto-assigned by slugifying its text when absent).</summary>
    public string Id { get; set; } = "";

    /// <summary>The heading's visible text.</summary>
    public string Text { get; set; } = "";

    /// <summary>The heading level (1 for <c>h1</c> .. 6 for <c>h6</c>), used for indentation.</summary>
    public int Level { get; set; }
}
