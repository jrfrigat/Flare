namespace Flare.ApiDocGen;

/// <summary>Documentation extracted for a single component type.</summary>
internal sealed record ComponentDoc(
    string Name,
    string FullName,
    string Namespace,
    string? Summary,
    string? Remarks,
    IReadOnlyList<ParameterDoc> Parameters,
    IReadOnlyList<MethodDoc> Methods,
    IReadOnlyList<string> Inherits,
    IReadOnlyList<string> DerivedBy);

/// <summary>Documentation for one [Parameter] / [CascadingParameter] member.</summary>
internal sealed record ParameterDoc(
    string Name,
    string Type,
    string? Default,
    string? Summary,
    string? Remarks,
    bool IsCascading,
    bool IsEventCallback,
    bool IsRequired,
    string DeclaringType);

/// <summary>Documentation for a public instance method.</summary>
internal sealed record MethodDoc(
    string Name,
    string Signature,
    string ReturnType,
    string? ReturnSummary,
    string? Summary,
    IReadOnlyList<MethodParamDoc> Parameters);

/// <summary>Documentation for a single method parameter.</summary>
internal sealed record MethodParamDoc(
    string Name,
    string Type,
    string? Summary);

/// <summary>Documentation extracted for a public enum type.</summary>
internal sealed record EnumDoc(
    string Name,
    string FullName,
    string Namespace,
    string? Summary,
    string? Remarks,
    IReadOnlyList<EnumMemberDoc> Members,
    IReadOnlyList<string> UsedBy);

/// <summary>Documentation for a single enum member.</summary>
internal sealed record EnumMemberDoc(
    string Name,
    string Value,
    string? Summary);

/// <summary>Raw XML doc comment content for one member, with inline markup normalized to text.</summary>
internal sealed record XmlMemberDoc(
    string? Summary,
    string? Remarks,
    string? Returns,
    string? Value,
    IReadOnlyDictionary<string, string> Parameters);
