namespace Flare.Components.IDE;

/// <summary>A destination a host offers for a document tab moved from the tab strip's move menu.</summary>
/// <param name="Key">Host-defined destination key, for example a region name. The tab strip never
/// interprets it.</param>
/// <param name="Label">Text shown for the destination in the tab's move menu.</param>
public sealed record FlareDocumentDockTarget(string Key, string Label);

/// <summary>A request to move a document tab to a host destination chosen from its move menu.</summary>
/// <param name="DocumentKey">Effective key of the document tab the user moved.</param>
/// <param name="Target">The chosen destination.</param>
public readonly record struct FlareDocumentDockRequest(string DocumentKey, FlareDocumentDockTarget Target);
