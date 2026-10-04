using System.Diagnostics.CodeAnalysis;

namespace Flare.Components;

/// <summary>
/// A button with a menu of the grid's columns, where each column is shown or hidden with a check.
/// </summary>
/// <typeparam name="TItem">Row type of the grid whose columns are picked.</typeparam>
public partial class DataGridColumnPicker<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.PublicFields)] TItem>;
