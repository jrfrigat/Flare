using System.Collections;

namespace Flare.Abstractions;

/// <summary>
/// A bag of values bound to the parameters of a component rendered as a dialog body. Keys are the
/// target component's <c>[Parameter]</c> property names; values are the arguments bound to them.
/// </summary>
/// <remarks>
/// Build one fluently and pass it to <see cref="IDialogService.ShowAsync{TComponent}"/>:
/// <code>
/// var parameters = new DialogParameters()
///     .Add(nameof(MyDialog.Title), "Edit profile")
///     .Add(nameof(MyDialog.Model), model);
/// </code>
/// </remarks>
public sealed class DialogParameters : IEnumerable<KeyValuePair<string, object?>>
{
    private readonly Dictionary<string, object?> _values = new(StringComparer.Ordinal);

    /// <summary>Creates an empty parameter bag.</summary>
    public DialogParameters() { }

    /// <summary>Creates a parameter bag pre-populated from the given name/value pairs.</summary>
    /// <param name="values">The initial parameter name/value pairs.</param>
    public DialogParameters(IEnumerable<KeyValuePair<string, object?>> values)
    {
        ArgumentNullException.ThrowIfNull(values);
        foreach (var pair in values)
            _values[pair.Key] = pair.Value;
    }

    /// <summary>Gets or sets the value bound to the component parameter named <paramref name="name"/>.</summary>
    /// <param name="name">The target component parameter name.</param>
    public object? this[string name]
    {
        get => _values.TryGetValue(name, out var value) ? value : null;
        set => _values[name] = value;
    }

    /// <summary>The number of parameters in the bag.</summary>
    public int Count => _values.Count;

    /// <summary>Adds or replaces the value bound to the component parameter <paramref name="name"/>.</summary>
    /// <param name="name">The target component parameter name.</param>
    /// <param name="value">The value to bind, or null.</param>
    /// <returns>The same bag, so calls can be chained.</returns>
    public DialogParameters Add(string name, object? value)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        _values[name] = value;
        return this;
    }

    /// <summary>Adds or replaces a strongly-typed value bound to the component parameter <paramref name="name"/>.</summary>
    /// <typeparam name="T">The value type, matching the target parameter's type.</typeparam>
    /// <param name="name">The target component parameter name.</param>
    /// <param name="value">The value to bind.</param>
    /// <returns>The same bag, so calls can be chained.</returns>
    public DialogParameters Add<T>(string name, T value) => Add(name, (object?)value);

    /// <summary>Removes the parameter <paramref name="name"/>.</summary>
    /// <param name="name">The parameter name to remove.</param>
    /// <returns><see langword="true"/> when the parameter was present and removed.</returns>
    public bool Remove(string name) => _values.Remove(name);

    /// <summary>Determines whether the bag contains a parameter named <paramref name="name"/>.</summary>
    /// <param name="name">The parameter name to look for.</param>
    public bool Contains(string name) => _values.ContainsKey(name);

    /// <summary>Tries to get the value bound to <paramref name="name"/>.</summary>
    /// <param name="name">The parameter name to look up.</param>
    /// <param name="value">The bound value when present, otherwise null.</param>
    /// <returns><see langword="true"/> when the parameter was present.</returns>
    public bool TryGetValue(string name, out object? value) => _values.TryGetValue(name, out value);

    // Snapshot in the shape DynamicComponent.Parameters requires (IDictionary&lt;string, object&gt;).
    // Internal: consumed only by the dialog provider host, not part of the public surface.
    internal Dictionary<string, object> ToComponentParameters()
    {
        var snapshot = new Dictionary<string, object>(_values.Count, StringComparer.Ordinal);
        foreach (var pair in _values)
            snapshot[pair.Key] = pair.Value!;
        return snapshot;
    }

    /// <summary>Returns an enumerator over the parameter name/value pairs.</summary>
    public IEnumerator<KeyValuePair<string, object?>> GetEnumerator() => _values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
