namespace Flare.Components;

/// <summary>
/// One part of a <c>FlareMeter</c>, weighted rather than positioned: a meter has no external scale, so
/// each segment carries a raw measurement and is sized in proportion to the sum of them all. That lets
/// figures be declared as measured, with no cumulative boundaries to work out by hand.
/// </summary>
/// <remarks>
/// For an absolute range on a scale the host owns - a slider or progress bar - use <see cref="FlareZone"/>.
/// </remarks>
public partial class FlareMeterSegment;
