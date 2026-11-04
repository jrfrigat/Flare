using Flare.Abstractions;

namespace Flare.Components;

/// <summary>A new cascade value signals a culture change without replacing component instances.</summary>
/// <param name="Context">The scoped translation and culture context.</param>
public sealed record FlareLocalizationState(IFlareLocalization Context)
{
    internal static readonly IFlareLocalization Ambient = new Flare.Theming.FlareLocalization();
}
