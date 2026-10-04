using System.Diagnostics.CodeAnalysis;

namespace Flare.Components;

/// <summary>
/// Toolbar buttons that export the grid's filtered and sorted rows, one per exporter, or a split button with the
/// first exporter as its main action.
/// </summary>
/// <typeparam name="TItem">Row type of the grid being exported.</typeparam>
public partial class DataGridExport<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.PublicFields)] TItem>;
