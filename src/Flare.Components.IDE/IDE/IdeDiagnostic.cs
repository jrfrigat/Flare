namespace Flare.Components.IDE;

/// <summary>A message tied to a position in a document.</summary>
/// <param name="Key">Stable identifier unique in the diagnostic list.</param>
/// <param name="DocumentKey">Identifier the host uses to open the document.</param>
/// <param name="DocumentTitle">Display name of the document.</param>
/// <param name="Line">One-based line, when available.</param>
/// <param name="Column">One-based column, when available.</param>
/// <param name="Severity">Level assigned by the source.</param>
/// <param name="Message">Display message.</param>
public sealed record IdeDiagnostic(
    string Key, string DocumentKey, string DocumentTitle, int? Line, int? Column,
    IdeDiagnosticSeverity Severity, string Message);

/// <summary>Severity of a document diagnostic.</summary>
public enum IdeDiagnosticSeverity
{
    /// <summary>Informational message.</summary>
    Information,
    /// <summary>Warning.</summary>
    Warning,
    /// <summary>Error.</summary>
    Error
}
