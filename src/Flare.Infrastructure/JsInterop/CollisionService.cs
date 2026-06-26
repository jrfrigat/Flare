using Flare.Abstractions;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Flare.Components.Services;

/// <summary>
/// Implementation of ICollisionService using JS interop.
/// </summary>
public sealed class CollisionService : FlareJsModule, ICollisionService
{
    /// <summary>Initializes a new <see cref="CollisionService"/>.</summary>
    public CollisionService(IJSRuntime js)
        : base(js, "./_content/Flare.Components/js/flare-collision.js") { }

    /// <summary>Computes a collision-aware placement for a floating element relative to its anchor.</summary>
    public async ValueTask<CollisionResult> CalculatePlacementAsync(
        ElementReference anchor,
        ElementReference floating,
        string preferredPlacement,
        CollisionOptions? options = null,
        CancellationToken ct = default)
    {
        var opts = options ?? new CollisionOptions();
        return await InvokeAsync<CollisionResult>(
            "calculatePlacement",
            anchor,
            floating,
            preferredPlacement,
            new
            {
                opts.Offset,
                opts.Flip,
                opts.Shift,
                opts.ArrowSize,
                opts.BoundaryPadding
            });
    }
}
