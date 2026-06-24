namespace Flare.Gallery.Api;

/// <summary>API documentation for a single Flare component.</summary>
/// <param name="Name">Short type name, e.g. <c>FlareButton</c>.</param>
/// <param name="FullName">Fully-qualified type name.</param>
/// <param name="Namespace">Declaring namespace.</param>
/// <param name="Summary">XML summary text, if any.</param>
/// <param name="Remarks">XML remarks text, if any.</param>
/// <param name="Parameters">Documented parameters (own first, inherited last).</param>
/// <param name="Methods">Documented public methods.</param>
/// <param name="Inherits">Base-type chain from the immediate base up to <c>object</c>.</param>
/// <param name="DerivedBy">Names of components that inherit from this type.</param>
public sealed record ApiComponentInfo(
    string Name,
    string FullName,
    string Namespace,
    string? Summary,
    string? Remarks,
    IReadOnlyList<ApiParameterInfo> Parameters,
    IReadOnlyList<ApiMethodInfo> Methods,
    IReadOnlyList<string> Inherits,
    IReadOnlyList<string> DerivedBy);

/// <summary>Documentation for one component parameter.</summary>
/// <param name="Name">Parameter property name.</param>
/// <param name="Type">Friendly C# type, e.g. <c>EventCallback&lt;MouseEventArgs&gt;</c>.</param>
/// <param name="Default">Default value text (e.g. <c>false</c>), or null when none.</param>
/// <param name="Summary">XML summary text, if any.</param>
/// <param name="Remarks">XML remarks text, if any.</param>
/// <param name="IsCascading">True for <c>[CascadingParameter]</c>.</param>
/// <param name="IsEventCallback">True when the type is an <c>EventCallback</c>.</param>
/// <param name="IsRequired">True for <c>[EditorRequired]</c> parameters.</param>
/// <param name="DeclaringType">Declaring type name; differs from the component when inherited.</param>
public sealed record ApiParameterInfo(
    string Name,
    string Type,
    string? Default,
    string? Summary,
    string? Remarks,
    bool IsCascading,
    bool IsEventCallback,
    bool IsRequired,
    string DeclaringType);

/// <summary>Documentation for one public component method.</summary>
/// <param name="Name">Method name.</param>
/// <param name="Signature">Friendly signature, e.g. <c>FocusAsync(bool preventScroll)</c>.</param>
/// <param name="ReturnType">Friendly return type.</param>
/// <param name="ReturnSummary">XML returns text, if any.</param>
/// <param name="Summary">XML summary text, if any.</param>
/// <param name="Parameters">Method parameters.</param>
public sealed record ApiMethodInfo(
    string Name,
    string Signature,
    string ReturnType,
    string? ReturnSummary,
    string? Summary,
    IReadOnlyList<ApiMethodParameter> Parameters);

/// <summary>Documentation for one method parameter.</summary>
/// <param name="Name">Parameter name.</param>
/// <param name="Type">Friendly C# type.</param>
/// <param name="Summary">XML parameter description, if any.</param>
public sealed record ApiMethodParameter(
    string Name,
    string Type,
    string? Summary);

/// <summary>API documentation for a public enum type.</summary>
/// <param name="Name">Short type name, e.g. <c>FlareColor</c>.</param>
/// <param name="FullName">Fully-qualified type name.</param>
/// <param name="Namespace">Declaring namespace.</param>
/// <param name="Summary">XML summary text, if any.</param>
/// <param name="Remarks">XML remarks text, if any.</param>
/// <param name="Members">Enum members in declaration order.</param>
/// <param name="UsedBy">Names of components that reference this enum as a parameter or return type.</param>
public sealed record ApiEnumInfo(
    string Name,
    string FullName,
    string Namespace,
    string? Summary,
    string? Remarks,
    IReadOnlyList<ApiEnumMember> Members,
    IReadOnlyList<string> UsedBy);

/// <summary>Documentation for one enum member.</summary>
/// <param name="Name">Member name.</param>
/// <param name="Value">Underlying integer value.</param>
/// <param name="Summary">XML summary text, if any.</param>
public sealed record ApiEnumMember(
    string Name,
    string Value,
    string? Summary);
