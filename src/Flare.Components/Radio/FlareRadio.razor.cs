namespace Flare.Components;

/// <summary>
/// One option of a radio group: a single choice among several, checked when its value matches the
/// group's. It takes its name, selection and disabled state from the enclosing <c>FlareRadioGroup</c>,
/// so the group stays the one place the answer lives.
/// </summary>
/// <typeparam name="TValue">Type of the value this option contributes to the group.</typeparam>
public partial class FlareRadio<TValue>;
