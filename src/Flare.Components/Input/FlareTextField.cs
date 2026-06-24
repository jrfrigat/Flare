namespace Flare.Components;

/// <summary>
/// A single-line text field. Convenience over <see cref="FlareField{TValue}"/> for the common
/// string case: it is <c>FlareField&lt;string&gt;</c> under a non-generic name, so you can write
/// <c>&lt;FlareTextField Label="Name" @bind-Value="name" /&gt;</c> without the <c>TValue="string"</c>
/// noise. For non-string values (numbers, dates, custom types) use the typed
/// <see cref="FlareField{TValue}"/> or a dedicated field (<c>FlareNumericField</c>, pickers, ...).
/// </summary>
public sealed class FlareTextField : FlareField<string>
{
}
