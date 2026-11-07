using Flare.Abstractions;
using Flare.Abstractions.Tokens;

namespace Flare.Components.Tests;

public class ThemeTokenCacheTests
{
    private const string Token = "--flare-test-token";

    [Fact]
    public void ReadersShareDesignByThemeInstance()
    {
        var first = new CountingTheme();
        var second = new CountingTheme();
        var firstService = new StubThemeService(first);
        var reader = new TokenReader();

        Assert.Null(reader.Read(firstService));
        Assert.Null(new TokenReader().Read(firstService));
        Assert.Null(reader.Read(new StubThemeService(second)));
        Assert.Null(reader.Read(firstService));

        Assert.Equal(1, first.Reads);
        Assert.Equal(1, second.Reads);
    }

    private sealed class TokenReader : FlareComponentBase
    {
        protected override string ComponentCssClass => "probe";

        public string? Read(IThemeService service)
        {
            ThemeService = service;
            return ReadTokenRaw(Token);
        }
    }

    private sealed class CountingTheme : ITheme
    {
        public int Reads { get; private set; }
        public string Id => "counting";
        public string DisplayName => Id;
        public string DefaultPaletteId => Id;
        public IReadOnlyList<string> StyleAssets => [];

        public DesignTokens Design
        {
            get
            {
                Reads++;
                return null!;
            }
        }
    }
}
