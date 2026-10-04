using System.Globalization;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;

namespace Flare.Components;

/// <summary>
/// Remembers the parameters a component received last and tells whether a new set can change its markup. Blazor
/// re-renders a child on every render of its parent as soon as one parameter is not of a type it knows to be
/// immutable, and a lambda passed as a callback is a new delegate every time - so a page of fields re-renders every
/// field whenever one of them commits a value.
/// </summary>
/// <remarks>
/// Two values are the same when:
/// <list type="bullet">
/// <item>both are immutable values (primitives, strings, enums, dates and times, decimals, Guids) and are equal;</item>
/// <item>both are event callbacks - they run on an event and never take part in rendering;</item>
/// <item>both are delegates other than a plain <see cref="RenderFragment"/> and the component's markup does not
/// currently read its delegates;</item>
/// <item>both are expressions with the same text (a <c>For</c> accessor rebuilt by the parent);</item>
/// <item>both are attribute dictionaries with the same keys and the same values;</item>
/// <item>both are the same read-only culture or calendar;</item>
/// <item>both are the same instance cascaded from above - a cascade announces its changes on its own.</item>
/// </list>
/// Anything else - a plain <see cref="RenderFragment"/>, a list, any other object - counts as changed, as it does
/// for Blazor itself, because it may have been mutated in place.
/// </remarks>
internal sealed class UnchangedParameters
{
    private (string Name, object? Value)[] _last = [];
    private int _count;

    /// <summary>Records <paramref name="parameters"/> and returns whether they match the previous set.</summary>
    /// <param name="parameters">The incoming parameters.</param>
    /// <param name="delegatesRender">Whether the component's markup currently reads its delegate parameters.</param>
    public bool Matches(ParameterView parameters, bool delegatesRender)
    {
        var same = true;
        var i = 0;
        foreach (var p in parameters)
        {
            if (same && (i >= _count || _last[i].Name != p.Name || !Same(_last[i].Value, p.Value, delegatesRender, p.Cascading)))
                same = false;
            if (i == _last.Length) Array.Resize(ref _last, Math.Max(8, _last.Length * 2));
            _last[i++] = (p.Name, p.Value);
        }
        if (i != _count) same = false;
        Array.Clear(_last, i, _count > i ? _count - i : 0);
        _count = i;
        return same;
    }

    private static bool Same(object? a, object? b, bool delegatesRender, bool cascading)
    {
        if (a is null || b is null) return a is null && b is null;
        var type = a.GetType();
        if (type != b.GetType()) return false;
        if (IsImmutable(type)) return a.Equals(b);
        if (IsEventCallback(type)) return true;
        if (a is RenderFragment) return false;
        if (a is Delegate) return !delegatesRender;
        if (a is LambdaExpression ea) return ea.ToString() == ((LambdaExpression)b).ToString();
        if (a is IReadOnlyDictionary<string, object> da) return SameAttributes(da, (IReadOnlyDictionary<string, object>)b);
        // A read-only culture or calendar (CultureInfo.GetCultureInfo, CultureInfo.ReadOnly) cannot change.
        if (a is CultureInfo ca) return ReferenceEquals(a, b) && ca.IsReadOnly;
        if (a is Calendar cal) return ReferenceEquals(a, b) && cal.IsReadOnly;
        return cascading && ReferenceEquals(a, b);
    }

    private static bool SameAttributes(IReadOnlyDictionary<string, object> a, IReadOnlyDictionary<string, object> b)
    {
        // The same instance may have been changed in place, so it proves nothing; the unmatched attributes Blazor
        // captures arrive in a new dictionary every time and are compared by content.
        if (ReferenceEquals(a, b) || a.Count != b.Count) return false;
        foreach (var (key, value) in a)
        {
            if (!b.TryGetValue(key, out var other)) return false;
            if (value is null || other is null) { if (value is not null || other is not null) return false; continue; }
            var type = value.GetType();
            if (type != other.GetType()) return false;
            if (IsEventCallback(type)) continue;
            if (!IsImmutable(type) || !value.Equals(other)) return false;
        }
        return true;
    }

    private static bool IsImmutable(Type type) =>
        type.IsPrimitive || type.IsEnum || type == typeof(string) || type == typeof(decimal) || type == typeof(Guid)
        || type == typeof(DateOnly) || type == typeof(TimeOnly) || type == typeof(DateTime)
        || type == typeof(DateTimeOffset) || type == typeof(TimeSpan);

    private static bool IsEventCallback(Type type) =>
        type == typeof(EventCallback) || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(EventCallback<>));
}
