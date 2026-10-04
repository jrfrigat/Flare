using System.Diagnostics.CodeAnalysis;

namespace Flare.Components;

/// <summary>
/// An editor for the grid's advanced filter: a tree of conditions joined by And or Or, applied to the grid.
/// </summary>
/// <typeparam name="TItem">Row type of the grid being filtered.</typeparam>
public partial class DataGridFilterBuilder<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.PublicFields)] TItem>;
