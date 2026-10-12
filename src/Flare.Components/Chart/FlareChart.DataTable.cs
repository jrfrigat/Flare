using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Flare.Components.Resources;

namespace Flare.Components;

public partial class FlareChart
{
    private RenderFragment RenderDataTable() => builder =>
    {
        builder.OpenElement(0, "div");
        builder.AddAttribute(1, "class", Css.Classes.Visually.Hidden);
        builder.OpenComponent<FlareTable<object>>(2);
        builder.AddAttribute(3, nameof(FlareTable<object>.Class), Css.Classes.Chart.Table);
        builder.AddAttribute(4, nameof(FlareTable<object>.Caption),
            string.IsNullOrWhiteSpace(Title) ? FlareStrings.Chart_TableCaption : Title);
        builder.AddAttribute(5, nameof(FlareTable<object>.ChildContent), RenderTableContent());
        builder.CloseComponent();
        builder.CloseElement();
    };

    private RenderFragment RenderTableContent() => builder =>
    {
        IReadOnlyList<ChartSeries> series = Data?.Series ?? [];
        bool points = Type is ChartType.Scatter or ChartType.Bubble;
        int seq = 0;
        builder.OpenElement(seq++, "thead");
        builder.OpenElement(seq++, "tr");
        if (points)
        {
            TableHeader(builder, ref seq, FlareStrings.Chart_TableSeries, "col");
            TableHeader(builder, ref seq, FlareStrings.Chart_TablePoint, "col");
            TableHeader(builder, ref seq, FlareStrings.Chart_TableX, "col");
            TableHeader(builder, ref seq, FlareStrings.Chart_TableY, "col");
            if (_bubble) TableHeader(builder, ref seq, FlareStrings.Chart_TableWeight, "col");
        }
        else
        {
            TableHeader(builder, ref seq, FlareStrings.Chart_TableCategory, "col");
            foreach (var s in series) TableHeader(builder, ref seq, s.Label, "col");
        }
        builder.CloseElement();
        builder.CloseElement();
        builder.OpenElement(seq++, "tbody");
        int rows = points ? PointTableRows(builder, ref seq, series) : CategoryTableRows(builder, ref seq, series);
        if (rows == 0)
        {
            builder.OpenElement(seq++, "tr");
            builder.OpenElement(seq++, "td");
            builder.AddAttribute(seq++, "colspan", points ? (_bubble ? 5 : 4) : series.Count + 1);
            builder.AddContent(seq++, FlareStrings.Chart_TableEmpty);
            builder.CloseElement();
            builder.CloseElement();
        }
        builder.CloseElement();
    };

    private int CategoryTableRows(RenderTreeBuilder builder, ref int seq, IReadOnlyList<ChartSeries> series)
    {
        var labels = Data?.Labels;
        int rows = series.Count == 0 ? 0 : Math.Max(labels?.Count ?? 0, series.Max(s => s.Values.Count));
        for (int r = 0; r < rows; r++)
        {
            builder.OpenElement(seq++, "tr");
            TableHeader(builder, ref seq, labels is { } l && r < l.Count ? l[r] : (r + 1).ToString(_inv), "row");
            foreach (var s in series)
                TableCell(builder, ref seq, r < s.Values.Count ? TableNumber(s.Values[r]) : FlareStrings.Chart_TableMissing);
            builder.CloseElement();
        }
        return rows;
    }

    private int PointTableRows(RenderTreeBuilder builder, ref int seq, IReadOnlyList<ChartSeries> series)
    {
        int rows = 0;
        foreach (var s in series)
        {
            if (s.Points is not { } points) continue;
            for (int i = 0; i < points.Count; i++)
            {
                var point = points[i];
                builder.OpenElement(seq++, "tr");
                TableHeader(builder, ref seq, s.Label, "row");
                TableCell(builder, ref seq, (i + 1).ToString(_inv));
                TableCell(builder, ref seq, TableNumber(point.X));
                TableCell(builder, ref seq, TableNumber(point.Y));
                if (_bubble) TableCell(builder, ref seq, TableNumber(point.R));
                builder.CloseElement();
                rows++;
            }
        }
        return rows;
    }

    private static string TableNumber(double value) =>
        double.IsFinite(value) ? value.ToString(_inv) : FlareStrings.Chart_TableMissing;

    private static void TableHeader(RenderTreeBuilder builder, ref int seq, string text, string scope)
    {
        builder.OpenElement(seq++, "th");
        builder.AddAttribute(seq++, "scope", scope);
        builder.AddContent(seq++, text);
        builder.CloseElement();
    }

    private static void TableCell(RenderTreeBuilder builder, ref int seq, string text)
    {
        builder.OpenElement(seq++, "td");
        builder.AddContent(seq++, text);
        builder.CloseElement();
    }
}
