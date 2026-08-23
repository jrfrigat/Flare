namespace Flare.Components;

/// <summary>
/// A coloured band drawn across part of a slider's or progress bar's track, marking a region of the
/// host's own scale - "danger starts at 90". The range is absolute, so each zone reads as a pair of
/// boundaries and is independent of the others. It registers with the host and renders nothing itself.
/// </summary>
/// <remarks>
/// For a part-to-whole breakdown, where the parts define the scale instead of sitting on one, use
/// <see cref="FlareMeterSegment"/>.
/// </remarks>
public partial class FlareZone;
