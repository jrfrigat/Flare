namespace Flare.Components.Tests;

public sealed class SignatureStyleTests : FlareTestContext
{
    [Fact]
    public void StrokeParametersApplyAgainAfterTheFirstRender()
    {
        var module = JSInterop.SetupModule("./_content/Flare.Components.Media/js/signature.js");
        module.Mode = JSRuntimeMode.Loose;
        var cut = Render<FlareSignaturePad>();
        cut.Render(p => p.Add(x => x.StrokeColor, FlareColor.Primary).Add(x => x.StrokeWidth, 4d));
        Assert.Equal(2, module.Invocations["setStrokeStyle"].Count);
        var last = module.Invocations["setStrokeStyle"].Last();
        Assert.Equal(Css.Tokens.Vars.Var(Css.Tokens.Color.Primary), last.Arguments[1]);
        Assert.Equal(4d, last.Arguments[2]);
    }
}
