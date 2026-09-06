using System.ComponentModel.DataAnnotations;

namespace Flare.Integration.Tests.Screens;

/// <summary>
/// The model behind <c>NewOrderForm</c>. Every rule here is a DataAnnotations attribute, which is what
/// <c>FlareForm</c>'s validator reads - so what the screen shows on a failed submit is decided here and
/// nowhere in the markup.
/// </summary>
public sealed class NewOrder
{
    /// <summary>Who the order is for. Required, so an empty form fails on submit.</summary>
    [Required(ErrorMessage = "Customer is required")]
    public string Customer { get; set; } = "";

    /// <summary>Chosen shipping method. Required, and only settable from the select's own options.</summary>
    [Required(ErrorMessage = "Shipping is required")]
    public string Shipping { get; set; } = "";

    /// <summary>How many units. At least one, so the numeric field's Min has a rule behind it.</summary>
    [Range(1, 99, ErrorMessage = "Quantity must be at least 1")]
    public int Quantity { get; set; } = 1;

    /// <summary>Terms acceptance. A checkbox is only valid when checked, which Range expresses for a bool.</summary>
    [Range(typeof(bool), "true", "true", ErrorMessage = "The terms must be accepted")]
    public bool Accepted { get; set; }
}
