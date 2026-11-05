using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Gantt;

/// <summary>Item and standard interaction commands for a custom interactive bar presentation.</summary>
/// <typeparam name="TItem">Application item type.</typeparam>
/// <param name="Item">Application item in this bar.</param>
/// <param name="SelectAsync">Standard bar activation and selection.</param>
/// <param name="HandleKeyAsync">Standard bar keyboard navigation and editing; bind to the primary action.</param>
public sealed record GanttItemContext<TItem>(TItem Item, Func<Task> SelectAsync, Func<KeyboardEventArgs, Task> HandleKeyAsync);
