using Flare.Components.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Flare.Components.Tests;

public class DataGridValueFormatterTests
{
    private static readonly System.Globalization.CultureInfo Inv = System.Globalization.CultureInfo.InvariantCulture;

    [Theory]
    [InlineData(typeof(bool), ColumnDataType.Boolean)]
    [InlineData(typeof(bool?), ColumnDataType.Boolean)]
    [InlineData(typeof(DateOnly), ColumnDataType.Date)]
    [InlineData(typeof(DateTime), ColumnDataType.DateTime)]
    [InlineData(typeof(DateTimeOffset), ColumnDataType.DateTime)]
    [InlineData(typeof(TimeOnly), ColumnDataType.Time)]
    [InlineData(typeof(TimeSpan), ColumnDataType.Time)]
    [InlineData(typeof(int), ColumnDataType.Number)]
    [InlineData(typeof(decimal?), ColumnDataType.Number)]
    [InlineData(typeof(double), ColumnDataType.Number)]
    [InlineData(typeof(DayOfWeek), ColumnDataType.Enum)]
    [InlineData(typeof(string), ColumnDataType.Text)]
    [InlineData(null, ColumnDataType.Text)]
    public void Infer_MapsClrTypeToDataType(Type? clr, ColumnDataType expected)
        => Assert.Equal(expected, DataGridValueFormatter.Infer(clr));

    [Fact]
    public void Resolve_Auto_UsesSampleRuntimeType()
        => Assert.Equal(ColumnDataType.Boolean, DataGridValueFormatter.Resolve(ColumnDataType.Auto, true));

    [Fact]
    public void Resolve_Explicit_WinsOverSample()
        => Assert.Equal(ColumnDataType.Text, DataGridValueFormatter.Resolve(ColumnDataType.Text, 42));

    [Fact]
    public void FormatText_Null_UsesNullText()
        => Assert.Equal("n/a", DataGridValueFormatter.FormatText(null, ColumnDataType.Text, null, "n/a", Inv));

    [Fact]
    public void FormatText_ExplicitFormat_WinsForFormattable()
        => Assert.Equal("12.50", DataGridValueFormatter.FormatText(12.5m, ColumnDataType.Number, "0.00", null, Inv));

    [Fact]
    public void FormatText_Date_DropsTime()
        => Assert.Equal("06/21/2026",
            DataGridValueFormatter.FormatText(new DateTime(2026, 6, 21, 13, 45, 0), ColumnDataType.Date, null, null, Inv));

    [Theory]
    [InlineData(ColumnDataType.Number, ColumnFilterType.Number)]
    [InlineData(ColumnDataType.Date, ColumnFilterType.Date)]
    [InlineData(ColumnDataType.DateTime, ColumnFilterType.Date)]
    [InlineData(ColumnDataType.Boolean, ColumnFilterType.Select)]
    [InlineData(ColumnDataType.Enum, ColumnFilterType.Select)]
    [InlineData(ColumnDataType.Text, ColumnFilterType.Text)]
    public void ToFilterType_MapsDataTypeToEditor(ColumnDataType type, ColumnFilterType expected)
        => Assert.Equal(expected, DataGridValueFormatter.ToFilterType(type));
}
