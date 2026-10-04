using System.Globalization;

namespace Flare.Components.Tests;

/// <summary>
/// TASK-188: the dial writes its number positions and hand from literal tables; they must match the geometry they
/// stand for - 50% +/- radius * sin/cos(30 degrees * position), outer radius 40, inner 26.
/// </summary>
public class ClockDialGeometryTests : FlareTestContext
{
    private static string Expected(int p, double r)
    {
        var x = Math.Round(50 + r * Math.Sin(p * Math.PI / 6), 2).ToString(CultureInfo.InvariantCulture);
        var y = Math.Round(50 - r * Math.Cos(p * Math.PI / 6), 2).ToString(CultureInfo.InvariantCulture);
        return $"left:{x}%;top:{y}%";
    }

    [Fact]
    public void HourRings_SitWhereTheGeometryPutsThem()
    {
        var cut = Render<FlareClockDial>(p => p.Add(x => x.Is24Hour, true).Add(x => x.Hour, 0));
        var nums = cut.FindAll($".{Css.Classes.ClockDial.Num}");
        Assert.Equal(24, nums.Count);
        for (var n = 1; n <= 12; n++)
        {
            Assert.Equal(n.ToString(CultureInfo.InvariantCulture), nums[n - 1].TextContent);
            Assert.Equal(Expected(n % 12, 40), nums[n - 1].GetAttribute("style"));
        }
        for (var p = 0; p < 12; p++)
        {
            Assert.Equal(p == 0 ? "00" : (p + 12).ToString(CultureInfo.InvariantCulture), nums[12 + p].TextContent);
            Assert.Equal(Expected(p, 26), nums[12 + p].GetAttribute("style"));
        }
        Assert.Contains(Css.Classes.ClockDial.NumSelected, nums[12].ClassName);
    }

    [Fact]
    public void Hand_PointsAtTheValue()
    {
        var cut = Render<FlareClockDial>(p => p.Add(x => x.Is24Hour, true).Add(x => x.Hour, 15));
        Assert.Equal($"{Css.Tokens.LocalVars.DialAngle}:90deg;{Css.Tokens.LocalVars.DialLength}:26",
            cut.Find($".{Css.Classes.ClockDial.Hand}").GetAttribute("style"));
    }
}
