namespace Flare.Components;

/// <summary>Which of the linear track's three shapes <see cref="FlareProgressLinear"/> draws.</summary>
public enum LinearProgressVariant
{
    /// <summary>A single active indicator on a track: the ordinary progress bar.</summary>
    Bar,
    /// <summary>The bar plus a muted secondary fill behind it, for buffered-ahead work.</summary>
    Buffer,
    /// <summary>A pulse sweeping backwards along the track, for work whose size is not known yet.</summary>
    Query,
}
