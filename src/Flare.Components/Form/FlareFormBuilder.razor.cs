using System.Diagnostics.CodeAnalysis;

namespace Flare.Components;

/// <summary>
/// Builds an edit form from the public properties of <typeparamref name="TModel"/>: one field per property, picked
/// by its type and described by <see cref="FlareFormFieldAttribute"/>, validated with data annotations.
/// </summary>
/// <typeparam name="TModel">The model type whose public properties become the form's fields.</typeparam>
// The fields are read from the model by reflection, so a trimmed app keeps its public properties.
public partial class FlareFormBuilder<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] TModel>;
