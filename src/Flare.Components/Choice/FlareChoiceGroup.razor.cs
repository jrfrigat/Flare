namespace Flare.Components;

/// <summary>
/// A single choice among several presented as a grid of <c>FlareChoiceCard</c> options - the
/// "selectable option card" composition, where each option carries a title and a hint rather than a
/// one-line label. The group owns the value; a card reports its title, its hint and the value it
/// contributes, and reads the selection and the disabled state from the enclosing group.
/// </summary>
/// <typeparam name="TValue">Type of the value the group holds and its cards contribute.</typeparam>
public partial class FlareChoiceGroup<TValue>;
