using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests.Component;

// ------------------------------------------------------------------------------
// Render-smoke + key-behavior coverage for standalone components that previously
// had no dedicated tests.
// ------------------------------------------------------------------------------

public class C_FlareStackTests : FlareTestContext
{
    [Fact]
    public void RendersRootAndChildContent()
    {
        var cut = Render<FlareStack>(p => p
            .AddChildContent("<span class=\"kid\">x</span>"));
        Assert.NotEmpty(cut.FindAll(".flare-stack"));
        Assert.NotEmpty(cut.FindAll(".kid"));
    }

    [Fact]
    public void Row_AddsRowModifier()
    {
        var cut = Render<FlareStack>(p => p.Add(x => x.Row, true));
        Assert.Contains("flare-stack--row", cut.Find(".flare-stack").ClassName);
    }
}

public class C_FlareGridTests : FlareTestContext
{
    [Fact]
    public void RendersRootAndChildContent()
    {
        var cut = Render<FlareGrid>(p => p
            .Add(x => x.Columns, 3)
            .AddChildContent("<div class=\"cell\">c</div>"));
        Assert.NotEmpty(cut.FindAll(".flare-grid"));
        Assert.NotEmpty(cut.FindAll(".cell"));
    }

    [Fact]
    public void MinColumnWidth_EmitsAutoFillTemplate()
    {
        var cut = Render<FlareGrid>(p => p
            .Add(x => x.MinColumnWidth, "15rem")
            .Add(x => x.Columns, 4));
        var style = cut.Find(".flare-grid").GetAttribute("style") ?? "";
        Assert.Contains("repeat(auto-fill,minmax(15rem,1fr))", style);
        // MinColumnWidth overrides the fixed Columns track set.
        Assert.DoesNotContain("repeat(4,1fr)", style);
    }

    [Fact]
    public void WithoutMinColumnWidth_EmitsFixedColumnTemplate()
    {
        var cut = Render<FlareGrid>(p => p.Add(x => x.Columns, 4));
        var style = cut.Find(".flare-grid").GetAttribute("style") ?? "";
        Assert.Contains("repeat(4,1fr)", style);
    }
}

public class C_FlareLinkTests : FlareTestContext
{
    [Fact]
    public void WithHref_RendersAnchor()
    {
        var cut = Render<FlareLink>(p => p
            .Add(x => x.Href, "/docs")
            .AddChildContent("Docs"));
        var a = cut.Find("a.flare-link");
        Assert.Equal("/docs", a.GetAttribute("href"));
        Assert.Contains("Docs", a.TextContent);
    }

    [Fact]
    public void Disabled_AddsDisabledClass()
    {
        var cut = Render<FlareLink>(p => p
            .Add(x => x.Href, "/x")
            .Add(x => x.Disabled, true));
        Assert.Contains("flare-link--disabled", cut.Find(".flare-link").ClassName);
    }
}

public class C_FlareImageTests : FlareTestContext
{
    [Fact]
    public void RendersImgWithSrcAndAlt()
    {
        var cut = Render<FlareImage>(p => p
            .Add(x => x.Src, "/logo.png")
            .Add(x => x.Alt, "Logo"));
        Assert.NotEmpty(cut.FindAll(".flare-image"));
        var img = cut.Find("img");
        Assert.Equal("/logo.png", img.GetAttribute("src"));
        Assert.Equal("Logo", img.GetAttribute("alt"));
    }
}

public class C_FlareAppBarTests : FlareTestContext
{
    [Fact]
    public void RendersTitle()
    {
        var cut = Render<FlareAppBar>(p => p.Add(x => x.Title, "My App"));
        Assert.NotEmpty(cut.FindAll(".flare-appbar"));
        Assert.Contains("My App", cut.Markup);
    }

    [Fact]
    public void Sticky_AddsStickyModifier()
    {
        var cut = Render<FlareAppBar>(p => p
            .Add(x => x.Title, "T")
            .Add(x => x.Sticky, true));
        Assert.Contains("flare-appbar--sticky", cut.Find(".flare-appbar").ClassName);
    }
}

public class C_FlareHiddenTests : FlareTestContext
{
    [Fact]
    public void NoBreakpoint_RendersChildContent()
    {
        var cut = Render<FlareHidden>(p => p
            .AddChildContent("<span class=\"kid\">visible</span>"));
        Assert.NotEmpty(cut.FindAll(".kid"));
    }

    [Fact]
    public void NoBreakpoint_DoesNotCarryUnconditionalBaseClass()
    {
        // With no breakpoint set the component is a plain pass-through wrapper:
        // it must never emit the bare `flare-hidden` utility (which would hide
        // its content in every viewport).
        var cut = Render<FlareHidden>(p => p
            .AddChildContent("<span class=\"kid\">visible</span>"));
        var div = cut.Find("div");
        Assert.DoesNotContain("flare-hidden", (div.GetAttribute("class") ?? string.Empty).Split(' '));
    }

    [Fact]
    public void Below_EmitsModifierClassWithoutUnconditionalBaseClass()
    {
        var cut = Render<FlareHidden>(p => p
            .Add(x => x.Below, Breakpoint.Sm)
            .AddChildContent("<span class=\"kid\">conditional</span>"));

        // Child content is always rendered; the breakpoint decides visibility via CSS.
        Assert.NotEmpty(cut.FindAll(".kid"));

        var classes = (cut.Find("div").GetAttribute("class") ?? string.Empty).Split(' ');
        Assert.Contains("flare-hidden--below-sm", classes);
        Assert.DoesNotContain("flare-hidden", classes);
    }
}

public class C_FlareMenuGroupTests : FlareTestContext
{
    [Fact]
    public void RendersLabelAndChildren()
    {
        var cut = Render<FlareMenuGroup>(p => p
            .Add(x => x.Label, "Section")
            .AddChildContent("<li class=\"item\">a</li>"));
        Assert.NotEmpty(cut.FindAll(".flare-menu-group"));
        Assert.Contains("Section", cut.Markup);
        Assert.NotEmpty(cut.FindAll(".item"));
    }
}

public class C_FlareColorModeToggleTests : FlareTestContext
{
    [Fact]
    public void RendersRoot()
    {
        var cut = Render<FlareColorModeToggle>();
        Assert.NotEmpty(cut.FindAll(".flare-color-mode-toggle"));
    }
}

public class C_FlareOtpFieldTests : FlareTestContext
{
    [Fact]
    public void RendersOneInputPerDigit()
    {
        var cut = Render<FlareOtpField>(p => p.Add(x => x.Length, 5));
        Assert.NotEmpty(cut.FindAll(".flare-otp"));
        Assert.Equal(5, cut.FindAll("input").Count);
    }
}

public class C_FlareColorPickerTests : FlareTestContext
{
    [Fact]
    public void RendersRootAndTrigger()
    {
        var cut = Render<FlareColorPicker>(p => p.Add(x => x.Value, "#ff0000"));
        Assert.NotEmpty(cut.FindAll(".flare-colorpicker"));
        Assert.NotEmpty(cut.FindAll(".flare-colorpicker__trigger"));
    }

    [Fact]
    public void Inline_RendersInlinePanel()
    {
        var cut = Render<FlareColorPicker>(p => p
            .Add(x => x.Value, "#00ff00")
            .Add(x => x.Inline, true));
        Assert.NotEmpty(cut.FindAll(".flare-colorpicker"));
    }

    [Fact]
    public void Label_RendersAboveTrigger()
    {
        var cut = Render<FlareColorPicker>(p => p
            .Add(x => x.Value, "#ff0000")
            .Add(x => x.Label, "Brand color"));
        var label = cut.Find("label.flare-colorpicker__label");
        Assert.Equal("Brand color", label.TextContent);
        // for/id wiring points the label at the trigger button
        var trigger = cut.Find(".flare-colorpicker__trigger");
        Assert.Equal(trigger.Id, label.GetAttribute("for"));
    }

    [Fact]
    public void HexValue_DefaultFormat_ShowsHexOnTrigger()
    {
        var cut = Render<FlareColorPicker>(p => p.Add(x => x.Value, "#1188ee"));
        Assert.Equal("#1188EE", cut.Find(".flare-colorpicker__trigger-label").TextContent);
    }

    [Fact]
    public void RgbaValue_WithRgbFormat_RoundTripsOnTrigger()
    {
        var cut = Render<FlareColorPicker>(p => p
            .Add(x => x.Value, "rgba(255, 0, 0, 0.5)")
            .Add(x => x.Format, ColorFormat.Rgb));
        Assert.Equal("rgba(255, 0, 0, 0.5)", cut.Find(".flare-colorpicker__trigger-label").TextContent);
    }

    [Fact]
    public void RgbValue_ParsesAndShowsRgb()
    {
        var cut = Render<FlareColorPicker>(p => p
            .Add(x => x.Value, "rgb(0, 128, 255)")
            .Add(x => x.Format, ColorFormat.Rgb));
        Assert.Equal("rgb(0, 128, 255)", cut.Find(".flare-colorpicker__trigger-label").TextContent);
    }

    [Fact]
    public void RgbaValue_DefaultFormat_ShowsHex8OnTrigger()
    {
        // Parsed as a color, emitted as hex (the default format) -- alpha 0.5 -> 80.
        var cut = Render<FlareColorPicker>(p => p.Add(x => x.Value, "rgba(255, 0, 0, 0.5)"));
        Assert.Equal("#FF000080", cut.Find(".flare-colorpicker__trigger-label").TextContent);
    }

    [Fact]
    public void HslValue_WithHslFormat_RoundTripsOnTrigger()
    {
        var cut = Render<FlareColorPicker>(p => p
            .Add(x => x.Value, "hsl(210, 100%, 50%)")
            .Add(x => x.Format, ColorFormat.Hsl));
        Assert.Equal("hsl(210, 100%, 50%)", cut.Find(".flare-colorpicker__trigger-label").TextContent);
    }
}

public class C_FlareTimePickerTests : FlareTestContext
{
    [Fact]
    public void RendersRoot()
    {
        var cut = Render<FlareTimePicker>(p => p
            .Add(x => x.Value, new TimeOnly(14, 30))
            .Add(x => x.Label, "Time"));
        Assert.NotEmpty(cut.FindAll(".flare-timepicker"));
        Assert.Contains("Time", cut.Markup);
    }
}

public class C_FlareDateRangePickerTests : FlareTestContext
{
    [Fact]
    public void RendersRoot()
    {
        var cut = Render<FlareDateRangePicker>(p => p
            .Add(x => x.StartDate, new DateOnly(2026, 1, 1))
            .Add(x => x.EndDate, new DateOnly(2026, 1, 31)));
        Assert.NotEmpty(cut.FindAll(".flare-daterangepicker"));
    }

    [Fact]
    public void ShowPresets_RendersDefaultPresetChips()
    {
        var cut = Render<FlareDateRangePicker>(p => p.Add(x => x.ShowPresets, true));
        // 7 default presets: Today, Yesterday, Last 7/30 days, This/Last month, This year
        Assert.Equal(7, cut.FindAll(".flare-daterangepicker__preset").Count);
    }

    [Fact]
    public void NoPresets_ByDefault()
    {
        var cut = Render<FlareDateRangePicker>();
        Assert.Empty(cut.FindAll(".flare-daterangepicker__preset"));
    }

    [Fact]
    public void ClickingTodayPreset_SetsBothDatesToToday()
    {
        var today = DateOnly.FromDateTime(TimeProvider.System.GetLocalNow().DateTime);
        DateOnly? start = null, end = null;
        var cut = Render<FlareDateRangePicker>(p => p
            .Add(x => x.ShowPresets, true)
            .Add(x => x.StartDateChanged, d => start = d)
            .Add(x => x.EndDateChanged, d => end = d));

        cut.FindAll(".flare-daterangepicker__preset")[0].Click(); // Today
        Assert.Equal(today, start);
        Assert.Equal(today, end);
    }

    [Fact]
    public void CustomPresets_OverrideDefaults()
    {
        var cut = Render<FlareDateRangePicker>(p => p
            .Add(x => x.ShowPresets, true)
            .Add(x => x.Presets, new List<DateRangePreset>
            {
                new("Q1", t => (new DateOnly(t.Year, 1, 1), new DateOnly(t.Year, 3, 31))),
            }));

        var chips = cut.FindAll(".flare-daterangepicker__preset");
        Assert.Single(chips);
        Assert.Equal("Q1", chips[0].TextContent);
    }
}

public class C_FlareBreadcrumbCollapseTests : FlareTestContext
{
    private static readonly BreadcrumbItem[] Five =
    [
        new("Home", "/", false),
        new("A", "/a", false),
        new("B", "/b", false),
        new("C", "/c", false),
        new("D", null, false),
    ];

    [Fact]
    public void UnderMaxItems_NoExpander()
    {
        var cut = Render<FlareBreadcrumb>(p => p
            .Add(x => x.Items, Five)
            .Add(x => x.MaxItems, 10));
        Assert.Empty(cut.FindAll(".flare-breadcrumb__expander"));
        Assert.Equal(5, cut.FindAll(".flare-breadcrumb__item").Count);
    }

    [Fact]
    public void OverMaxItems_CollapsesMiddleWithExpander()
    {
        var cut = Render<FlareBreadcrumb>(p => p
            .Add(x => x.Items, Five)
            .Add(x => x.MaxItems, 3));
        // first(1) + expander item + last(1) = 3 list items
        Assert.Single(cut.FindAll(".flare-breadcrumb__expander"));
        Assert.Contains("Home", cut.Markup);
        Assert.Contains("D", cut.Markup);
        Assert.DoesNotContain(">B<", cut.Markup);
    }

    [Fact]
    public void ClickingExpander_RevealsAllItems()
    {
        var cut = Render<FlareBreadcrumb>(p => p
            .Add(x => x.Items, Five)
            .Add(x => x.MaxItems, 3));
        cut.Find(".flare-breadcrumb__expander").Click();
        Assert.Empty(cut.FindAll(".flare-breadcrumb__expander"));
        Assert.Equal(5, cut.FindAll(".flare-breadcrumb__item").Count);
    }
}

public class C_FlareTabsScrollTests : FlareTestContext
{
    [Fact]
    public void RendersBarWrapAroundBar()
    {
        var cut = Render<FlareTabs>();
        var wrap = cut.Find(".flare-tabs__bar-wrap");
        Assert.NotNull(wrap.QuerySelector(".flare-tabs__bar"));
    }

    [Fact]
    public void OnTabScrollState_ShowsArrowsWhenOverflowing()
    {
        var cut = Render<FlareTabs>();
        cut.InvokeAsync(() => cut.Instance.OnTabScrollState(overflowing: true, atStart: true, atEnd: false));
        Assert.NotEmpty(cut.FindAll(".flare-tabs__scroll"));
        Assert.NotNull(cut.Find(".flare-tabs__scroll--prev").GetAttribute("disabled")); // atStart
    }
}

public class C_FlareTabsPlacementTests : FlareTestContext
{
    [Theory]
    [InlineData(TabsPlacement.Top, "", false)]
    [InlineData(TabsPlacement.Bottom, "flare-tabs--bottom", false)]
    [InlineData(TabsPlacement.Left, "flare-tabs--vertical", true)]
    [InlineData(TabsPlacement.Right, "flare-tabs--right", true)]
    public void Placement_AppliesClasses(TabsPlacement placement, string expected, bool vertical)
    {
        var cut = Render<FlareTabs>(p => p.Add(x => x.Placement, placement));
        var cls = cut.Find(".flare-tabs").ClassName ?? "";
        if (!string.IsNullOrEmpty(expected)) Assert.Contains(expected, cls);
        Assert.Equal(vertical, cls.Contains("flare-tabs--vertical"));
    }

    [Theory]
    [InlineData(TabLabelRotation.None, "0deg")]
    [InlineData(TabLabelRotation.Rotate90, "90deg")]
    [InlineData(TabLabelRotation.Rotate180, "180deg")]
    [InlineData(TabLabelRotation.Rotate270, "270deg")]
    public void LabelRotation_SetsCssVariable(TabLabelRotation rot, string expected)
    {
        var cut = Render<FlareTabs>(p => p.Add(x => x.LabelRotation, rot));
        Assert.Contains($"--flare-tab-label-rotation:{expected}", cut.Find(".flare-tabs").GetAttribute("style"));
    }

    [Fact]
    public void Rotation90_AddsRotatedClass()
    {
        var cut = Render<FlareTabs>(p => p.Add(x => x.LabelRotation, TabLabelRotation.Rotate90));
        Assert.Contains("flare-tabs--rotated", cut.Find(".flare-tabs").ClassName);
    }
}

public class C_XlsxWriterTests
{
    private static string ReadEntry(byte[] xlsx, string path)
    {
        using var ms = new MemoryStream(xlsx);
        using var zip = new System.IO.Compression.ZipArchive(ms, System.IO.Compression.ZipArchiveMode.Read);
        var entry = zip.GetEntry(path);
        Assert.NotNull(entry);
        using var r = new StreamReader(entry!.Open());
        return r.ReadToEnd();
    }

    [Fact]
    public void Write_ProducesValidZipWithRequiredParts()
    {
        var bytes = XlsxWriter.Write(["A", "B"], [new string?[] { "1", "x" }]);
        using var ms = new MemoryStream(bytes);
        using var zip = new System.IO.Compression.ZipArchive(ms, System.IO.Compression.ZipArchiveMode.Read);
        var names = zip.Entries.Select(e => e.FullName).ToHashSet();
        Assert.Contains("[Content_Types].xml", names);
        Assert.Contains("xl/workbook.xml", names);
        Assert.Contains("xl/worksheets/sheet1.xml", names);
        // xlsx magic bytes "PK"
        Assert.Equal(0x50, bytes[0]);
        Assert.Equal(0x4B, bytes[1]);
    }

    [Fact]
    public void Write_HeadersAndCellsAppearInSheet()
    {
        var bytes = XlsxWriter.Write(["Name", "Score"],
            [new string?[] { "Alice", "92" }, new string?[] { "Bob", "78" }]);
        var sheet = ReadEntry(bytes, "xl/worksheets/sheet1.xml");
        Assert.Contains("Name", sheet);
        Assert.Contains("Alice", sheet);
        // canonical number -> numeric <v> cell (no inlineStr)
        Assert.Contains("<v>92</v>", sheet);
    }

    [Fact]
    public void Write_NonCanonicalNumbersStayText()
    {
        // leading zero must not become a numeric cell
        var bytes = XlsxWriter.Write(["Code"], [new string?[] { "007" }]);
        var sheet = ReadEntry(bytes, "xl/worksheets/sheet1.xml");
        Assert.Contains("inlineStr", sheet);
        Assert.Contains("007", sheet);
        Assert.DoesNotContain("<v>007</v>", sheet);
    }

    [Fact]
    public void Write_EscapesXmlSpecialChars()
    {
        var bytes = XlsxWriter.Write(["H"], [new string?[] { "a<b>&\"c" }]);
        var sheet = ReadEntry(bytes, "xl/worksheets/sheet1.xml");
        Assert.Contains("a&lt;b&gt;&amp;&quot;c", sheet);
    }
}

public class C_DataGridExporterTests
{
    private sealed record Row(string Name, int Score);

    // Captures whatever an exporter sends to the download service.
    private sealed class CaptureDownload : IFlareDownload
    {
        public string? FileName; public string? Text; public byte[]? Bytes; public string? Mime;
        public ValueTask DownloadAsync(string filename, string content, string? mimeType = null, bool withBom = false)
        { FileName = filename; Text = content; Mime = mimeType; return ValueTask.CompletedTask; }
        public ValueTask DownloadCsvAsync(string filename, string csv)
        { FileName = filename; Text = csv; Mime = "text/csv"; return ValueTask.CompletedTask; }
        public ValueTask DownloadBytesAsync(string filename, byte[] bytes, string? mimeType = null)
        { FileName = filename; Bytes = bytes; Mime = mimeType; return ValueTask.CompletedTask; }
    }

    private static DataGridExportData<Row> SampleData() => new()
    {
        Columns =
        [
            new("Name", r => r.Name),
            new("Score", r => r.Score),
        ],
        Rows = [new Row("Alice, A", 92), new Row("Bob", 78)],
        FileName = "people",
    };

    [Fact]
    public async Task Csv_WritesHeaderRowsAndQuotesCommas()
    {
        var dl = new CaptureDownload();
        await DataGridExporters.Csv<Row>().ExportAsync(SampleData(), dl);
        Assert.Equal("people.csv", dl.FileName);
        Assert.Contains("Name,Score", dl.Text);
        Assert.Contains("\"Alice, A\"", dl.Text); // comma forces quoting
        Assert.Contains("92", dl.Text);
    }

    [Fact]
    public async Task Tsv_And_Json_UseProperExtensions()
    {
        var tsv = new CaptureDownload();
        await DataGridExporters.Tsv<Row>().ExportAsync(SampleData(), tsv);
        Assert.Equal("people.tsv", tsv.FileName);
        Assert.Contains("Name\tScore", tsv.Text);

        var json = new CaptureDownload();
        await DataGridExporters.Json<Row>().ExportAsync(SampleData(), json);
        Assert.Equal("people.json", json.FileName);
        Assert.Contains("\"Name\"", json.Text);
    }

    [Fact]
    public async Task Excel_DownloadsValidXlsxBytes()
    {
        var dl = new CaptureDownload();
        await DataGridExporters.Excel<Row>().ExportAsync(SampleData(), dl);
        Assert.Equal("people.xlsx", dl.FileName);
        Assert.NotNull(dl.Bytes);
        Assert.Equal(0x50, dl.Bytes![0]); // "PK" zip magic
        Assert.Equal(0x4B, dl.Bytes![1]);
    }

    [Fact]
    public async Task CustomExporter_IsInvokedWithGridData()
    {
        var custom = new MarkdownExporter();
        var dl = new CaptureDownload();
        await custom.ExportAsync(SampleData(), dl);
        Assert.Equal("people.md", dl.FileName);
        Assert.Contains("| Name | Score |", dl.Text);
    }

    // A minimal third-party-style exporter to prove the contract is open.
    private sealed class MarkdownExporter : IDataGridExporter<Row>
    {
        public string Id => "MD";
        public string Label => "Markdown";
        public string? Icon => null;
        public Task ExportAsync(DataGridExportData<Row> data, IFlareDownload download)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("| " + string.Join(" | ", data.Columns.Select(c => c.Title)) + " |");
            foreach (var r in data.Rows)
                sb.AppendLine("| " + string.Join(" | ", data.Columns.Select(c => c.Value(r))) + " |");
            return download.DownloadAsync(System.IO.Path.ChangeExtension(data.FileName, ".md"), sb.ToString(), "text/markdown").AsTask();
        }
    }
}

public class C_FlareSplitterTests : FlareTestContext
{
    [Fact]
    public void RendersAsAStandaloneHandle()
    {
        var cut = Render<FlareSplitter>();

        // The component IS the handle (a single separator element with a grip bar) - no panes.
        Assert.Single(cut.FindAll(".flare-splitter"));
        Assert.Single(cut.FindAll(".flare-splitter__gutter-bar"));
        Assert.Empty(cut.FindAll(".flare-splitter__pane--first"));
        Assert.Equal("separator", cut.Find(".flare-splitter").GetAttribute("role"));
    }

    [Fact]
    public void DefaultOrientation_IsAuto_NoForcedAxisClass()
    {
        // Auto: the axis is detected from the parent flex direction at runtime (in JS), so no
        // orientation class is forced up front; aria defaults to a vertical separator (horizontal split).
        var cut = Render<FlareSplitter>();
        var cls = cut.Find(".flare-splitter").ClassName;
        Assert.DoesNotContain("flare-splitter--vertical", cls);
        Assert.DoesNotContain("flare-splitter--horizontal", cls);
        Assert.Equal("vertical", cut.Find(".flare-splitter").GetAttribute("aria-orientation"));
    }

    [Fact]
    public void ExplicitOrientation_AppliesClass()
    {
        var horizontal = Render<FlareSplitter>(p => p
            .Add(x => x.Orientation, FlareSplitter.SplitterOrientation.Horizontal));
        Assert.Contains("flare-splitter--horizontal", horizontal.Find(".flare-splitter").ClassName);
        Assert.Equal("vertical", horizontal.Find(".flare-splitter").GetAttribute("aria-orientation"));

        var vertical = Render<FlareSplitter>(p => p
            .Add(x => x.Orientation, FlareSplitter.SplitterOrientation.Vertical));
        Assert.Contains("flare-splitter--vertical", vertical.Find(".flare-splitter").ClassName);
        Assert.Equal("horizontal", vertical.Find(".flare-splitter").GetAttribute("aria-orientation"));
    }

    [Fact]
    public void Handle_IsKeyboardFocusable()
    {
        var cut = Render<FlareSplitter>();
        Assert.Equal("0", cut.Find(".flare-splitter").GetAttribute("tabindex"));
    }

    [Fact]
    public void Icon_ReplacesGripBar_AndHoverIconRenders()
    {
        var cut = Render<FlareSplitter>(p => p
            .Add(x => x.Icon, "drag_indicator")
            .Add(x => x.HoverIcon, "open_with"));

        Assert.Empty(cut.FindAll(".flare-splitter__gutter-bar"));
        Assert.Equal("drag_indicator", cut.Find(".flare-splitter__icon--base").TextContent);
        Assert.Equal("open_with", cut.Find(".flare-splitter__icon--hover").TextContent);
    }

    [Fact]
    public void ChildContent_OverridesIconAndGrip()
    {
        var cut = Render<FlareSplitter>(p => p
            .Add(x => x.Icon, "drag_indicator")
            .AddChildContent("<b id=\"custom\">grip</b>"));

        Assert.Single(cut.FindAll("#custom"));
        Assert.Empty(cut.FindAll(".flare-splitter__icon--base"));
        Assert.Empty(cut.FindAll(".flare-splitter__gutter-bar"));
    }

    [Fact]
    public void SizeAndColors_SetCssVariables()
    {
        var cut = Render<FlareSplitter>(p => p
            .Add(x => x.Size, "14px")
            .Add(x => x.Color, "var(--flare-color-surface-container-high)")
            .Add(x => x.HoverColor, "var(--flare-color-primary-container)"));

        var style = cut.Find(".flare-splitter").GetAttribute("style");
        Assert.Contains("--flare-splitter-gutter-size:14px", style);
        Assert.Contains("--flare-splitter-color:var(--flare-color-surface-container-high)", style);
        Assert.Contains("--flare-splitter-hover-color:var(--flare-color-primary-container)", style);
    }
}

public class C_FlareCalendarTests : FlareTestContext
{
    [Fact]
    public void RendersSevenDayLabels()
    {
        var cut = Render<FlareCalendar>(p => p
            .Add(x => x.InitialDate, new DateOnly(2026, 6, 1)));
        Assert.NotEmpty(cut.FindAll(".flare-calendar"));
        Assert.Equal(7, cut.FindAll(".flare-calendar__day-label").Count);
    }

    [Fact]
    public void ClickingDay_RaisesSelectedDateChanged()
    {
        DateOnly? picked = null;
        var cut = Render<FlareCalendar>(p => p
            .Add(x => x.InitialDate, new DateOnly(2026, 6, 1))
            .Add(x => x.SelectedDateChanged, d => picked = d));

        cut.FindAll(".flare-calendar__cell")
            .First(c => !(c.ClassName ?? "").Contains("--other"))
            .Click();

        Assert.NotNull(picked);
    }
}

public class C_FlareFormBuilderTests : FlareTestContext
{
    private sealed class Person
    {
        public string Name { get; set; } = "";
        public int Age { get; set; }
    }

    [Fact]
    public void RendersFieldsAndSubmitButton()
    {
        var cut = Render<FlareFormBuilder<Person>>(p => p
            .Add(x => x.Model, new Person()));
        Assert.NotEmpty(cut.FindAll(".flare-formbuilder"));
        Assert.NotEmpty(cut.FindAll("input"));        // a field per model property
        Assert.NotEmpty(cut.FindAll("button[type=submit]"));
    }
}

public class C_FlareDropZoneTests : FlareTestContext
{
    [Fact]
    public void RendersRootAndFileInput()
    {
        var cut = Render<FlareDropZone>(p => p
            .AddChildContent("<span>Drop here</span>"));
        Assert.NotEmpty(cut.FindAll(".flare-dropzone"));
        Assert.NotEmpty(cut.FindAll("input[type=file]"));
    }
}

public class C_FlareInfiniteScrollTests : FlareTestContext
{
    [Fact]
    public void RendersChildContentAndSentinel()
    {
        var cut = Render<FlareInfiniteScroll>(p => p
            .AddChildContent("<div class=\"row\">item</div>"));
        Assert.NotEmpty(cut.FindAll(".flare-infinite-scroll"));
        Assert.NotEmpty(cut.FindAll(".flare-infinite-scroll__sentinel"));
        Assert.NotEmpty(cut.FindAll(".row"));
    }

    [Fact]
    public void NoMore_ShowsEndContent()
    {
        var cut = Render<FlareInfiniteScroll>(p => p
            .Add(x => x.HasMore, false)
            .Add(x => x.EndContent, (RenderFragment)(b => b.AddMarkupContent(0, "<span class=\"end\">No more</span>"))));
        Assert.NotEmpty(cut.FindAll(".end"));
    }
}

public class C_FlareNavMenuTests : FlareTestContext
{
    [Fact]
    public void RendersNavWithChildContent()
    {
        var cut = Render<FlareNavMenu>(p => p
            .AddChildContent("<a class=\"link\">Home</a>"));
        Assert.NotEmpty(cut.FindAll("nav.flare-nav-menu"));
        Assert.NotEmpty(cut.FindAll(".link"));
    }

    [Fact]
    public void HideScrollbar_AddsModifier()
    {
        var cut = Render<FlareNavMenu>(p => p.Add(x => x.HideScrollbar, true));
        Assert.Contains("flare-nav-menu--no-scrollbar", cut.Find("nav.flare-nav-menu").ClassName);
    }

    [Fact]
    public void Mode_Rail_AddsRailModifier()
    {
        var cut = Render<FlareNavMenu>(p => p.Add(x => x.Mode, NavMenuMode.Rail));
        Assert.Contains("flare-nav-menu--rail", cut.Find("nav.flare-nav-menu").ClassName);
    }

    [Fact]
    public void Mode_Full_OverridesRailFlag()
    {
        // An explicit Full mode wins over the legacy Rail flag.
        var cut = Render<FlareNavMenu>(p => p.Add(x => x.Mode, NavMenuMode.Full).Add(x => x.Rail, true));
        Assert.DoesNotContain("flare-nav-menu--rail", cut.Find("nav.flare-nav-menu").ClassName);
    }

    [Fact]
    public void Mode_RailLabeled_AddsBothRailModifiers()
    {
        var cut = Render<FlareNavMenu>(p => p.Add(x => x.Mode, NavMenuMode.RailLabeled));
        var cls = cut.Find("nav.flare-nav-menu").ClassName;
        Assert.Contains("flare-nav-menu--rail", cls);
        Assert.Contains("flare-nav-menu--rail-labeled", cls);
    }
}

// FlareLayoutDrawer owns its own open state and reports a grid track to the layout per variant:
// Mini collapses to an icon rail, Persistent reserves a zero-width track when closed, Temporary floats.
public class C_FlareLayoutDrawerTests : FlareTestContext
{
    private IRenderedComponent<FlareLayoutDrawer> RenderDrawer(DrawerVariant variant, bool open)
        => Render<FlareLayoutDrawer>(p => p
            .Add(d => d.Variant, variant)
            .Add(d => d.Open, open)
            .Add(d => d.Width, "16rem")
            .Add(d => d.RailWidth, "5rem"));

    [Fact]
    public void MiniCollapsed_IsIconRail_ReservesRailTrack()
    {
        var d = RenderDrawer(DrawerVariant.Mini, open: false).Instance;
        Assert.True(d.IsCollapsedRail);
        Assert.False(d.IsOpen);
        Assert.True(d.ReservesTrack(false));
        Assert.Equal("5rem", d.TrackWidth(false));
    }

    [Fact]
    public void MiniOpen_IsFullWidth_NotCollapsed()
    {
        var d = RenderDrawer(DrawerVariant.Mini, open: true).Instance;
        Assert.False(d.IsCollapsedRail);
        Assert.Equal("16rem", d.TrackWidth(false));
    }

    [Fact]
    public void PersistentClosed_ReservesZeroWidthTrack()
    {
        var d = RenderDrawer(DrawerVariant.Persistent, open: false).Instance;
        Assert.True(d.ReservesTrack(false));   // keeps its track so the width can animate
        Assert.Equal("0", d.TrackWidth(false));
    }

    [Fact]
    public void PersistentOpen_ReservesFullTrack()
        => Assert.Equal("16rem", RenderDrawer(DrawerVariant.Persistent, open: true).Instance.TrackWidth(false));

    [Fact]
    public void Temporary_Floats_NeverReservesTrack()
    {
        var d = RenderDrawer(DrawerVariant.Temporary, open: true).Instance;
        Assert.False(d.ReservesTrack(false));
        Assert.True(d.IsOverlayOpen(false));
    }

    [Fact]
    public void TemporaryOpen_IsModalDialog_WithAriaLabel()
    {
        var cut = Render<FlareLayoutDrawer>(p => p
            .Add(d => d.Variant, DrawerVariant.Temporary)
            .Add(d => d.Open, true)
            .Add(d => d.AriaLabel, "Navigation"));

        var nav = cut.Find("nav");
        Assert.Equal("dialog", nav.GetAttribute("role"));
        Assert.Equal("true", nav.GetAttribute("aria-modal"));
        Assert.Equal("Navigation", nav.GetAttribute("aria-label"));
    }

    [Fact]
    public void PersistentOpen_IsNotModal_AndNotInert()
    {
        var cut = Render<FlareLayoutDrawer>(p => p
            .Add(d => d.Variant, DrawerVariant.Persistent)
            .Add(d => d.Open, true)
            .Add(d => d.Width, "16rem"));

        var nav = cut.Find("nav");
        Assert.False(nav.HasAttribute("role"));
        Assert.False(nav.HasAttribute("aria-modal"));
        Assert.False(nav.HasAttribute("inert"));
    }

    [Fact]
    public void ClosedPushDrawer_IsInert_NotKeyboardReachable()
    {
        var cut = Render<FlareLayoutDrawer>(p => p
            .Add(d => d.Variant, DrawerVariant.Persistent)
            .Add(d => d.Open, false)
            .Add(d => d.Width, "16rem"));

        // A closed push drawer collapses to a 0-width track but keeps its links in the DOM; inert
        // removes them from the tab order + a11y tree instead of leaving focusable links under aria-hidden.
        Assert.True(cut.Find("nav").HasAttribute("inert"));
    }

    [Fact]
    public void Mobile_PushDrawerGoesOffCanvas()
    {
        var d = RenderDrawer(DrawerVariant.Mini, open: false).Instance;
        Assert.False(d.ReservesTrack(true));   // off-canvas on mobile
        Assert.Equal("0", d.TrackWidth(true));
    }

    [Fact]
    public async Task Toggle_FlipsOpen_AndRaisesChanged()
    {
        var changed = false;
        var cut = Render<FlareLayoutDrawer>(p => p
            .Add(d => d.Variant, DrawerVariant.Persistent)
            .Add(d => d.Open, false)
            .Add(d => d.OpenChanged, EventCallback.Factory.Create<bool>(this, v => changed = v)));
        await cut.InvokeAsync(() => cut.Instance.ToggleAsync());
        Assert.True(cut.Instance.IsOpen);
        Assert.True(changed);
    }
}

// FlareLayout reserves a grid track for each registered in-flow drawer and exposes the column template
// via the --flare-layout-cols custom property, so the content is pushed aside rather than covered.
public class C_FlareLayoutGridTests : FlareTestContext
{
    [Fact]
    public void Layout_ReservesRailTrackForMiniDrawer()
    {
        var cut = Render<FlareLayout>(p => p
            .Add(x => x.Responsive, false)
            .AddChildContent<FlareLayoutDrawer>(d => d
                .Add(x => x.Variant, DrawerVariant.Mini)
                .Add(x => x.RailWidth, "5rem")
                .Add(x => x.Open, false)));

        var style = cut.Find(".flare-layout").GetAttribute("style") ?? string.Empty;
        Assert.Contains("--flare-layout-cols", style);
        Assert.Contains("5rem", style);          // the collapsed rail reserves a 5rem track
        Assert.Contains("minmax(0, 1fr)", style); // the content fills the rest
    }
}

// FlareNavGroup renders an inline accordion: a header button that toggles a child-items region.
// It stays an inline accordion regardless of the layout's collapsed/mini-rail state.
public class C_FlareNavGroupInlineTests : FlareTestContext
{
    [Fact]
    public void RendersInlineGroup_HeaderTogglesItems()
    {
        var cut = Render<FlareNavGroup>(p => p
            .Add(g => g.Label, "Components")
            .Add(g => g.Icon, "category")
            .AddChildContent("<a class=\"flare-nav-link\">Buttons</a>"));

        Assert.NotEmpty(cut.FindAll(".flare-nav-group__items"));
        var header = cut.Find("button.flare-nav-group__header");
        Assert.Equal("false", header.GetAttribute("aria-expanded"));
        Assert.Contains("Buttons", cut.Find(".flare-nav-group__items").TextContent);

        header.Click();
        Assert.Equal("true", cut.Find("button.flare-nav-group__header").GetAttribute("aria-expanded"));
    }

    [Fact]
    public void RendersInlineGroup_NoFlyoutMarkup()
    {
        // The group is always an inline accordion: no flyout panel. (The two-pane secondary column
        // replaced the old per-group flyout; FlareNavGroup no longer reads the layout context.)
        var cut = Render<FlareNavGroup>(p => p
            .Add(g => g.Label, "Components")
            .AddChildContent("<a>x</a>"));

        Assert.Empty(cut.FindAll(".flare-nav-group--flyout"));
        Assert.NotEmpty(cut.FindAll(".flare-nav-group__items"));
    }
}

public class C_FlareMaskedFieldTests : FlareTestContext
{
    [Fact]
    public void RendersInput()
    {
        var cut = Render<FlareMaskedField>(p => p
            .Add(x => x.Mask, "000-000")
            .Add(x => x.Value, "123456"));
        Assert.NotEmpty(cut.FindAll(".flare-input"));
        Assert.NotEmpty(cut.FindAll("input"));
    }

    [Fact]
    public void LeadingLiteralMask_FirstDigit_AppearsWithLiteral()
    {
        // Regression: a mask that starts with a literal ('+') dropped the first
        // typed digit, so nothing appeared. Typing "7" must render "+7 (".
        string? bound = null;
        var cut = Render<FlareMaskedField>(p => p
            .Add(x => x.Mask, "+# (###) ###-##-##")
            .Add(x => x.ValueChanged, (string? v) => bound = v));

        cut.Find("input").Input("7");

        Assert.Equal("+7 (", cut.Find("input").GetAttribute("value"));
        Assert.Equal("+7 (", bound);
    }

    [Fact]
    public void LeadingLiteralMask_TypedGroups_ExtractAndReapply()
    {
        // The masked string round-trips: re-feeding it keeps only the digits and
        // re-applies the literals in place.
        var cut = Render<FlareMaskedField>(p => p
            .Add(x => x.Mask, "+# (###) ###-##-##"));

        cut.Find("input").Input("+7 (912");

        Assert.Equal("+7 (912) ", cut.Find("input").GetAttribute("value"));
    }

    [Fact]
    public void ParenLeadingMask_FirstDigit_AppearsWithLiteral()
    {
        // The same class of bug affected any leading literal, e.g. the classic
        // "(###) ###-####" phone shape.
        var cut = Render<FlareMaskedField>(p => p
            .Add(x => x.Mask, "(###) ###-####"));

        cut.Find("input").Input("5");

        Assert.Equal("(5", cut.Find("input").GetAttribute("value"));
    }
}

public class C_FlareCodeBlockTests : FlareTestContext
{
    [Fact]
    public void RendersRootWithValue()
    {
        var cut = Render<FlareCodeBlock>(p => p
            .Add(x => x.Value, "var x = 1;")
            .Add(x => x.Language, "csharp")
            .Add(x => x.ReadOnly, false));
        Assert.NotEmpty(cut.FindAll(".flare-codeblock"));
        Assert.Equal("var x = 1;", cut.Find("textarea").GetAttribute("value"));
    }
}

public class C_FlareMediaQueryTests : FlareTestContext
{
    [Fact]
    public void RendersChildContentWithInitialBreakpoint()
    {
        var cut = Render<FlareMediaQuery>(p => p
            .Add(x => x.InitialBreakpoint, Breakpoint.Md)
            .Add(x => x.ChildContent, (RenderFragment<Breakpoint>)(bp => b => b.AddContent(0, $"bp:{bp}"))));
        Assert.Contains("bp:Md", cut.Markup);
    }
}

public class C_FlareSelectVariantTests : FlareTestContext
{
    [Fact]
    public void OutlinedVariant_ReusesInputVariantClass()
    {
        var cut = Render<FlareSelect<string>>(p => p.Add(x => x.Variant, InputVariant.Outlined));
        Assert.Contains("flare-input-variant--outlined", cut.Find(".flare-select").ClassName);
    }

    [Fact]
    public void MultiSelect_FilledVariant_ReusesInputVariantClass()
    {
        var cut = Render<FlareMultiSelect<string>>(p => p.Add(x => x.Variant, InputVariant.Filled));
        Assert.Contains("flare-input-variant--filled", cut.Find(".flare-multiselect").ClassName);
    }
}

public class C_SizeGridUnificationTests : FlareTestContext
{
    [Theory]
    [InlineData(ChipSize.Xs, "flare-chip--xs")]
    [InlineData(ChipSize.Sm, "flare-chip--sm")]
    [InlineData(ChipSize.Lg, "flare-chip--lg")]
    [InlineData(ChipSize.Xl, "flare-chip--xl")]
    public void Chip_Size_AppliesModifier(ChipSize size, string expected)
    {
        var cut = Render<FlareChip>(p => p.Add(x => x.Label, "x").Add(x => x.Size, size));
        Assert.Contains(expected, cut.Find(".flare-chip").ClassName);
    }

    [Fact]
    public void Avatar_Xs_AppliesModifier()
    {
        var cut = Render<FlareAvatar>(p => p.Add(x => x.Text, "AB").Add(x => x.Size, AvatarSize.Xs));
        Assert.Contains("flare-avatar--xs", cut.Find(".flare-avatar").ClassName);
    }

    [Fact]
    public void Slider_Xl_AppliesModifier()
    {
        var cut = Render<FlareSlider>(p => p.Add(x => x.Size, SliderSize.Xl));
        Assert.Contains("flare-slider--xl", cut.Find(".flare-slider").ClassName);
    }
}

public class C_FieldVariantReuseTests : FlareTestContext
{
    [Fact]
    public void TagInput_Outlined_ReusesInputVariantClass()
    {
        var cut = Render<FlareTagField<string>>(p => p.Add(x => x.Variant, InputVariant.Outlined));
        Assert.Contains("flare-input-variant--outlined", cut.Find(".flare-tag-input").ClassName);
    }

    [Fact]
    public void DatePicker_Filled_ReusesInputVariantClass()
    {
        var cut = Render<FlareDatePicker>(p => p.Add(x => x.Variant, InputVariant.Filled));
        Assert.Contains("flare-input-variant--filled", cut.Find(".flare-datepicker").ClassName);
    }
}

public class C_FlarePaginationSizeTests : FlareTestContext
{
    [Theory]
    [InlineData(PaginationSize.Xs, "flare-pagination--xs")]
    [InlineData(PaginationSize.Sm, "flare-pagination--sm")]
    [InlineData(PaginationSize.Lg, "flare-pagination--lg")]
    [InlineData(PaginationSize.Xl, "flare-pagination--xl")]
    public void Size_AppliesModifierClass(PaginationSize size, string expected)
    {
        var cut = Render<FlarePagination>(p => p
            .Add(x => x.TotalPages, 5)
            .Add(x => x.Size, size));
        Assert.Contains(expected, cut.Find(".flare-pagination").ClassName);
    }

    [Fact]
    public void Medium_HasNoSizeModifier()
    {
        var cut = Render<FlarePagination>(p => p
            .Add(x => x.TotalPages, 5)
            .Add(x => x.Size, PaginationSize.Md));
        var cls = cut.Find(".flare-pagination").ClassName;
        Assert.DoesNotContain("flare-pagination--", cls);
    }
}

public class C_FlareEmptyStateCompactTests : FlareTestContext
{
    [Fact]
    public void Compact_AddsModifier()
    {
        var cut = Render<FlareEmptyState>(p => p.Add(x => x.Compact, true).Add(x => x.Title, "Empty"));
        Assert.Contains("flare-empty-state--compact", cut.Find(".flare-empty-state").ClassName);
    }

    [Fact]
    public void Default_HasNoCompactModifier()
    {
        var cut = Render<FlareEmptyState>(p => p.Add(x => x.Title, "Empty"));
        Assert.DoesNotContain("flare-empty-state--compact", cut.Find(".flare-empty-state").ClassName);
    }
}

public class C_FlareDividerInsetTests : FlareTestContext
{
    [Theory]
    [InlineData(DividerInset.Inset, "flare-divider--inset")]
    [InlineData(DividerInset.MiddleInset, "flare-divider--middle-inset")]
    public void Inset_AppliesModifier(DividerInset inset, string expected)
    {
        var cut = Render<FlareDivider>(p => p.Add(x => x.Inset, inset));
        Assert.Contains(expected, cut.Find("hr").ClassName);
    }

    [Fact]
    public void None_HasNoInsetModifier()
    {
        var cut = Render<FlareDivider>();
        Assert.DoesNotContain("inset", cut.Find("hr").ClassName);
    }
}

public class C_FlareCardKeyboardActivationTests : FlareTestContext
{
    [Theory]
    [InlineData("Enter")]
    [InlineData(" ")]
    public void ClickableCard_ActivatesOnKey(string key)
    {
        var clicks = 0;
        var cut = Render<FlareCard>(p => p
            .Add(x => x.OnClick, () => clicks++)
            .AddChildContent("Card"));
        cut.Find(".flare-card").KeyDown(new Microsoft.AspNetCore.Components.Web.KeyboardEventArgs { Key = key });
        Assert.Equal(1, clicks);
    }

    [Fact]
    public void NonInteractiveCard_HasNoButtonRole()
    {
        var cut = Render<FlareCard>(p => p.AddChildContent("Card"));
        Assert.Null(cut.Find(".flare-card").GetAttribute("role"));
    }
}

public class C_FlareMenuDenseTests : FlareTestContext
{
    [Fact]
    public void Dense_AddsDenseModifier()
    {
        var cut = Render<FlareMenu>(p => p.Add(x => x.Dense, true));
        Assert.Contains("flare-menu--dense", cut.Find(".flare-menu").ClassName);
    }

    [Fact]
    public void Default_HasNoDenseModifier()
    {
        var cut = Render<FlareMenu>();
        Assert.DoesNotContain("flare-menu--dense", cut.Find(".flare-menu").ClassName);
    }
}

public class C_FlareTabsVariantTests : FlareTestContext
{
    [Theory]
    [InlineData(TabsVariant.Underline, "flare-tabs--underline")]
    [InlineData(TabsVariant.Primary, "flare-tabs--primary")]
    [InlineData(TabsVariant.Text, "flare-tabs--text")]
    [InlineData(TabsVariant.Tonal, "flare-tabs--tonal")]
    [InlineData(TabsVariant.Filled, "flare-tabs--filled")]
    [InlineData(TabsVariant.Outlined, "flare-tabs--outlined")]
    public void Variant_AddsModifierClass(TabsVariant variant, string expected)
    {
        var cut = Render<FlareTabs>(p => p
            .Add(x => x.Variant, variant)
            .AddChildContent<FlareTab>(t => t.Add(x => x.Label, "A")));
        Assert.Contains(expected, cut.Find(".flare-tabs").ClassName);
    }

    [Fact]
    public void Default_AddsNoVariantModifier()
    {
        var cut = Render<FlareTabs>(p => p
            .AddChildContent<FlareTab>(t => t.Add(x => x.Label, "A")));
        var cls = cut.Find(".flare-tabs").ClassName;
        Assert.DoesNotContain("flare-tabs--underline", cls);
        Assert.DoesNotContain("flare-tabs--primary", cls);
        Assert.DoesNotContain("flare-tabs--filled", cls);
    }
}

public class C_FlareThemeScopeTests : FlareTestContext
{
    [Fact]
    public void Mode_Dark_AppliesThemePaletteAndDarkModeClasses()
    {
        var cut = Render<FlareThemeScope>(p => p
            .AddCascadingValue<IThemeService>(new StubThemeService())
            .Add(x => x.Mode, ThemeMode.Dark)
            .AddChildContent("<span class=\"kid\">x</span>"));
        var cls = cut.Find("div").ClassName;
        Assert.Contains("flare-root", cls);
        Assert.Contains("flare-theme-stub", cls);
        Assert.Contains("flare-palette-stub", cls);
        Assert.Contains("flare-mode-dark", cls);
        Assert.NotEmpty(cut.FindAll(".kid"));
    }

    [Fact]
    public void UnsetMode_InheritsOuterMode()
    {
        // Stub outer is light -> an unset Mode inherits light (explicit light class re-asserts it).
        var cut = Render<FlareThemeScope>(p => p
            .AddCascadingValue<IThemeService>(new StubThemeService())
            .AddChildContent("<span>x</span>"));
        var cls = cut.Find("div").ClassName;
        Assert.Contains("flare-mode-light", cls);
        Assert.DoesNotContain("flare-mode-dark", cls);
    }
}

public class C_FlareProgressWavyTests : FlareTestContext
{
    // Theme that opts into wavy progress (mirrors the MD3 theme tokens).
    private static TokenThemeService WavyTheme() => new(new Dictionary<string, string>
    {
        ["--flare-progress-wavy-enabled"] = "1",
        ["--flare-progress-wave-length"] = "40px",
        ["--flare-progress-wave-amplitude"] = "3px",
        ["--flare-progress-circular-gap"] = "4px",
    });

    [Fact]
    public void Wavy_NoThemeOptIn_RendersPlain()
    {
        // Point 2: without --flare-progress-wavy-enabled (e.g. Fluent), Wavy="true" stays plain.
        var cut = Render<FlareProgress>(p => p.Add(x => x.Value, 60d).Add(x => x.Wavy, true));
        Assert.DoesNotContain("flare-progress--wavy", cut.Find(".flare-progress").ClassName);
    }

    [Fact]
    public void Wavy_Linear_OptedIn_UsesSplitTrackWithWaveSvg()
    {
        var cut = Render<FlareProgress>(p => p
            .AddCascadingValue<IThemeService>(WavyTheme())
            .Add(x => x.Value, 60d).Add(x => x.Wavy, true));
        var root = cut.Find(".flare-progress").ClassName;
        Assert.Contains("flare-progress--wavy", root);
        Assert.Contains("flare-progress--split", root);          // inherits gap + stop indicator
        Assert.NotEmpty(cut.FindAll(".flare-progress__bar svg.flare-progress__wave path"));
        Assert.NotEmpty(cut.FindAll(".flare-progress__remain"));
    }

    [Fact]
    public void Wavy_Circular_OptedIn_TrackIsSmoothCircle_IndicatorIsWavyPath()
    {
        var cut = Render<FlareProgress>(p => p
            .AddCascadingValue<IThemeService>(WavyTheme())
            .Add(x => x.Variant, ProgressVariant.Circular)
            .Add(x => x.Value, 60d).Add(x => x.Wavy, true));
        Assert.Contains("flare-progress--wavy", cut.Find(".flare-progress").ClassName);
        // Point 1: track stays a smooth <circle>, only the active indicator is a wavy <path>.
        Assert.NotEmpty(cut.FindAll("circle.flare-progress__track"));
        var ind = cut.FindAll("path.flare-progress__indicator");
        Assert.NotEmpty(ind);
        Assert.Equal("100", ind[0].GetAttribute("pathLength"));
        // Point 4: the wavy indicator flows via the ring-wave CSS animation (rotate + dashoffset).
    }

    [Fact]
    public void Wavy_Indeterminate_FallsBackToFlat()
    {
        var cut = Render<FlareProgress>(p => p
            .AddCascadingValue<IThemeService>(WavyTheme())
            .Add(x => x.Wavy, true));
        Assert.DoesNotContain("flare-progress--wavy", cut.Find(".flare-progress").ClassName);
    }
}

public class C_FlareProgressThicknessTests : FlareTestContext
{
    [Fact]
    public void Thickness_SetsLinearHeightToken()
    {
        var cut = Render<FlareProgress>(p => p.Add(x => x.Value, 50d).Add(x => x.Thickness, 10));
        Assert.Contains("--flare-progress-linear-height:10px", cut.Find(".flare-progress").GetAttribute("style"));
    }

    [Fact]
    public void NoThicknessToken_ByDefault()
    {
        var cut = Render<FlareProgress>(p => p.Add(x => x.Value, 50d));
        var style = cut.Find(".flare-progress").GetAttribute("style") ?? "";
        Assert.DoesNotContain("--flare-progress-linear-height", style);
    }
}

public class C_FlareBadgeSizeTests : FlareTestContext
{
    [Theory]
    [InlineData(FieldSize.Xs, "flare-badge--xs")]
    [InlineData(FieldSize.Xl, "flare-badge--xl")]
    public void Size_AppliesModifier(FieldSize size, string expected)
    {
        var cut = Render<FlareBadge>(p => p.Add(x => x.Count, 3).Add(x => x.Size, size));
        Assert.Contains(expected, cut.Find(".flare-badge").ClassName);
    }
}

public class C_FlareRatingSizeTests : FlareTestContext
{
    [Theory]
    [InlineData(FieldSize.Xs, "flare-rating--xs")]
    [InlineData(FieldSize.Xl, "flare-rating--xl")]
    public void Size_AppliesModifier(FieldSize size, string expected)
    {
        var cut = Render<FlareRating>(p => p.Add(x => x.Size, size));
        Assert.Contains(expected, cut.Find(".flare-rating").ClassName);
    }
}

public class C_FlareCheckboxRadioSizeTests : FlareTestContext
{
    [Theory]
    [InlineData(FieldSize.Xs, "flare-checkbox--xs")]
    [InlineData(FieldSize.Xl, "flare-checkbox--xl")]
    public void Checkbox_Size_AppliesModifier(FieldSize size, string expected)
    {
        var cut = Render<FlareCheckbox>(p => p.Add(x => x.Size, size));
        Assert.Contains(expected, cut.Find(".flare-checkbox").ClassName);
    }

    [Fact]
    public void RadioGroup_Size_CascadesToRadios()
    {
        var cut = Render<FlareRadioGroup<string>>(p => p
            .Add(g => g.Size, FieldSize.Lg)
            .AddChildContent<FlareRadio<string>>(r => r.Add(x => x.Value, "a")));
        Assert.Contains("flare-radio--lg", cut.Find(".flare-radio").ClassName);
    }
}

public class C_FlareFormFieldSizeParityTests : FlareTestContext
{
    [Theory]
    [InlineData(FieldSize.Xs, "flare-input--xs")]
    [InlineData(FieldSize.Lg, "flare-input--lg")]
    public void Autocomplete_Size_AppliesModifier(FieldSize size, string expected)
    {
        var cut = Render<FlareCombobox<string>>(p => p.Add(x => x.Size, size));
        Assert.Contains(expected, cut.Find(".flare-autocomplete").ClassName);
    }

    [Theory]
    [InlineData(FieldSize.Xs, "flare-input--xs")]
    [InlineData(FieldSize.Lg, "flare-input--lg")]
    public void TagInput_Size_AppliesModifier(FieldSize size, string expected)
    {
        var cut = Render<FlareTagField<string>>(p => p.Add(x => x.Size, size));
        Assert.Contains(expected, cut.Find(".flare-tag-input").ClassName);
    }
}

public class C_FlareNumericFieldSizeTests : FlareTestContext
{
    [Theory]
    [InlineData(FieldSize.Xs, "flare-input--xs")]
    [InlineData(FieldSize.Sm, "flare-input--sm")]
    [InlineData(FieldSize.Lg, "flare-input--lg")]
    [InlineData(FieldSize.Xl, "flare-input--xl")]
    public void Size_AppliesSharedInputSizeClass(FieldSize size, string expected)
    {
        var cut = Render<FlareNumericField<int>>(p => p.Add(x => x.Size, size));
        Assert.Contains(expected, cut.Find(".flare-input").ClassName);
    }
}

// C1 enterprise gaps closed: numeric clamp/format, slider marks, tag suggestions.
public class C_FlareNumericFieldClampFormatTests : FlareTestContext
{
    [Fact]
    public void Change_AboveMax_ClampsToMax()
    {
        var value = 0;
        var cut = Render<FlareNumericField<int>>(p => p
            .Add(x => x.Min, 0.0)
            .Add(x => x.Max, 10.0)
            .Add(x => x.Value, 0)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<int>(this, v => value = v)));

        cut.Find("input").Change("999");
        Assert.Equal(10, value);
    }

    [Fact]
    public void Change_BelowMin_ClampsToMin()
    {
        var value = 0;
        var cut = Render<FlareNumericField<int>>(p => p
            .Add(x => x.Min, 5.0)
            .Add(x => x.Max, 10.0)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<int>(this, v => value = v)));

        cut.Find("input").Change("-3");
        Assert.Equal(5, value);
    }

    [Fact]
    public void Format_SwitchesToTextModeInput()
    {
        var cut = Render<FlareNumericField<decimal>>(p => p
            .Add(x => x.Format, "N0")
            .Add(x => x.Value, 1234567m));

        var input = cut.Find("input");
        Assert.Equal("text", input.GetAttribute("type"));
        // Blurred display shows group separators (invariant culture uses comma).
        Assert.Contains(",", input.GetAttribute("value")!);
    }

    [Fact]
    public void NoFormat_StaysNumberInput()
    {
        var cut = Render<FlareNumericField<int>>(p => p.Add(x => x.Value, 42));
        Assert.Equal("number", cut.Find("input").GetAttribute("type"));
    }
}

public class C_FlareSliderMarksTests : FlareTestContext
{
    [Fact]
    public void Marks_RenderLabeledMarks()
    {
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.Min, 0)
            .Add(x => x.Max, 100)
            .Add(x => x.Value, 50)
            .Add(x => x.Marks, new Dictionary<double, string> { [0] = "Low", [50] = "Mid", [100] = "High" }));

        var marks = cut.FindAll(".flare-slider__mark");
        Assert.Equal(3, marks.Count);
        Assert.Contains(marks, m => m.TextContent == "Mid");
        Assert.Contains("flare-slider--with-marks", cut.Find(".flare-slider").ClassName);
    }

    [Fact]
    public void Marks_OutOfRange_AreIgnored()
    {
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.Min, 0)
            .Add(x => x.Max, 100)
            .Add(x => x.Marks, new Dictionary<double, string> { [0] = "A", [200] = "B" }));

        Assert.Single(cut.FindAll(".flare-slider__mark"));
    }
}

public class C_FlareSliderZonesTests : FlareTestContext
{
    private static RenderFragment Zones(RenderFragment body) => body;

    [Fact]
    public void Zones_RenderBands_WithRoleColorClassAndPercents()
    {
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.Min, 0)
            .Add(x => x.Max, 100)
            .Add(x => x.Value, 40)
            .Add(x => x.Zones, Zones(b =>
            {
                b.OpenComponent<FlareSliderZone>(0);
                b.AddAttribute(1, nameof(FlareSliderZone.Start), 0d);
                b.AddAttribute(2, nameof(FlareSliderZone.End), 60d);
                b.AddAttribute(3, nameof(FlareSliderZone.Color), FlareColor.Success);
                b.CloseComponent();
                b.OpenComponent<FlareSliderZone>(4);
                b.AddAttribute(5, nameof(FlareSliderZone.Start), 60d);
                b.AddAttribute(6, nameof(FlareSliderZone.End), 100d);
                b.AddAttribute(7, nameof(FlareSliderZone.Color), FlareColor.Error);
                b.CloseComponent();
            })));

        var bands = cut.FindAll(".flare-slider__zone");
        Assert.Equal(2, bands.Count);
        Assert.Contains(bands, z => z.ClassName.Contains("flare-color-success"));
        Assert.Contains(bands, z => z.ClassName.Contains("flare-color-error"));
        // First zone spans 0% -> 60% of the scale.
        Assert.Contains(bands, z => z.GetAttribute("style")!.Contains("--_z1:60.00%"));
    }

    [Fact]
    public void Zone_CustomColor_InlinesLocalToken()
    {
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.Min, 0)
            .Add(x => x.Max, 100)
            .Add(x => x.Zones, Zones(b =>
            {
                b.OpenComponent<FlareSliderZone>(0);
                b.AddAttribute(1, nameof(FlareSliderZone.Start), 10d);
                b.AddAttribute(2, nameof(FlareSliderZone.End), 50d);
                b.AddAttribute(3, nameof(FlareSliderZone.Color), FlareColor.Custom("#ff0000"));
                b.CloseComponent();
            })));

        var band = cut.Find(".flare-slider__zone");
        Assert.DoesNotContain("flare-color-", band.ClassName);
        Assert.Contains("--fc-main", band.GetAttribute("style"));
    }

    [Fact]
    public void Zone_ZeroWidth_IsDropped()
    {
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.Min, 0)
            .Add(x => x.Max, 100)
            .Add(x => x.Zones, Zones(b =>
            {
                b.OpenComponent<FlareSliderZone>(0);
                b.AddAttribute(1, nameof(FlareSliderZone.Start), 50d);
                b.AddAttribute(2, nameof(FlareSliderZone.End), 50d);
                b.CloseComponent();
            })));

        Assert.Empty(cut.FindAll(".flare-slider__zone"));
    }
}

public class C_FlareTagFieldSuggestionsTests : FlareTestContext
{
    [Fact]
    public void StaticSuggestions_FilterOnInput()
    {
        var cut = Render<FlareTagField<string>>(p => p
            .Add(x => x.Suggestions, new[] { "apple", "apricot", "banana" })
            .Add(x => x.MinChars, 1));

        cut.Find(".flare-tag-input__input").Input("ap");
        var options = cut.FindAll(".flare-listbox__option");
        Assert.Equal(2, options.Count);
        Assert.Contains(options, o => o.TextContent.Trim() == "apple");
        Assert.Contains(options, o => o.TextContent.Trim() == "apricot");
    }

    [Fact]
    public void SelectingSuggestion_AddsTag()
    {
        IReadOnlyList<string> tags = [];
        var cut = Render<FlareTagField<string>>(p => p
            .Add(x => x.Suggestions, new[] { "apple", "apricot" })
            .Add(x => x.ValuesChanged, EventCallback.Factory.Create<IReadOnlyList<string>>(this, v => tags = v)));

        cut.Find(".flare-tag-input__input").Input("ap");
        cut.FindAll(".flare-listbox__option")[0].Click();
        Assert.Contains("apple", tags);
    }

    [Fact]
    public void AlreadyAddedTag_ExcludedFromSuggestions()
    {
        var cut = Render<FlareTagField<string>>(p => p
            .Add(x => x.Values, new[] { "apple" })
            .Add(x => x.Suggestions, new[] { "apple", "apricot" }));

        cut.Find(".flare-tag-input__input").Input("ap");
        var options = cut.FindAll(".flare-listbox__option");
        Assert.Single(options);
        Assert.Equal("apricot", options[0].TextContent.Trim());
    }
}

public class C_FlareFieldCharacterCountTests : FlareTestContext
{
    [Fact]
    public void ShowCharacterCount_WithMaxLength_RendersCurrentOverMax()
    {
        var cut = Render<FlareField<string>>(p => p
            .Add(x => x.Value, "abc")
            .Add(x => x.MaxLength, 10)
            .Add(x => x.ShowCharacterCount, true));
        Assert.Equal("3/10", cut.Find(".flare-input__counter").TextContent);
    }

    [Fact]
    public void NoCounter_ByDefault()
    {
        var cut = Render<FlareField<string>>(p => p.Add(x => x.Value, "abc"));
        Assert.Empty(cut.FindAll(".flare-input__counter"));
    }
}

public class C_FlareButtonShapeTests : FlareTestContext
{
    [Fact]
    public void Square_AddsShapeModifier()
    {
        var cut = Render<FlareButton>(p => p
            .Add(x => x.Shape, ButtonShape.Square)
            .AddChildContent("Go"));
        Assert.Contains("flare-btn--square", cut.Find("button").ClassName);
    }

    [Fact]
    public void Rounded_AddsRoundedModifier()
    {
        var cut = Render<FlareButton>(p => p
            .Add(x => x.Shape, ButtonShape.Rounded)
            .AddChildContent("Go"));
        Assert.Contains("flare-btn--rounded", cut.Find("button").ClassName);
    }

    [Fact]
    public void Circular_AddsCircularModifier()
    {
        var cut = Render<FlareButton>(p => p
            .Add(x => x.Shape, ButtonShape.Circular)
            .AddChildContent("Go"));
        Assert.Contains("flare-btn--circular", cut.Find("button").ClassName);
    }

    [Fact]
    public void Default_IsDefaultWithoutShapeModifier()
    {
        var cut = Render<FlareButton>(p => p.AddChildContent("Go"));
        var cls = cut.Find("button").ClassName;
        Assert.DoesNotContain("flare-btn--square", cls);
        Assert.DoesNotContain("flare-btn--rounded", cls);
        Assert.DoesNotContain("flare-btn--circular", cls);
    }

    [Fact]
    public void PressMorph_AddsMorphModifier()
    {
        var cut = Render<FlareButton>(p => p.Add(x => x.PressMorph, true).AddChildContent("Go"));
        Assert.Contains("flare-btn--morph", cut.Find("button").ClassName);
    }

    [Fact]
    public void NoPressMorph_ByDefault()
    {
        var cut = Render<FlareButton>(p => p.AddChildContent("Go"));
        Assert.DoesNotContain("flare-btn--morph", cut.Find("button").ClassName);
    }
}

public class C_FlareMultiSelectDisabledA11yTests : FlareTestContext
{
    [Fact]
    public void Disabled_SetsAriaDisabledAndRemovesFromTabOrder()
    {
        var cut = Render<FlareMultiSelect<string>>(p => p.Add(x => x.Disabled, true));
        var combobox = cut.Find("[role=combobox]");
        Assert.Equal("true", combobox.GetAttribute("aria-disabled"));
        Assert.Equal("-1", combobox.GetAttribute("tabindex"));
    }

    [Fact]
    public void Enabled_IsTabbableWithoutAriaDisabled()
    {
        var cut = Render<FlareMultiSelect<string>>();
        var combobox = cut.Find("[role=combobox]");
        Assert.Null(combobox.GetAttribute("aria-disabled"));
        Assert.Equal("0", combobox.GetAttribute("tabindex"));
    }
}

public class C_FlareTableDenseTests : FlareTestContext
{
    [Fact]
    public void Dense_AppliesDenseModifier()
    {
        var cut = Render<FlareTable<int>>(p => p.Add(x => x.Dense, true));
        Assert.Contains("flare-table--dense", cut.Find(".flare-table").ClassName);
    }

    [Fact]
    public void Default_HasNoDenseModifier()
    {
        var cut = Render<FlareTable<int>>();
        Assert.DoesNotContain("flare-table--dense", cut.Find(".flare-table").ClassName);
    }
}

// WCAG contrast math used by the ColorCustomizer's accessibility preview.
public class C_ColorMathContrastTests
{
    [Fact]
    public void BlackOnWhite_IsMaxRatio()
        => Assert.Equal(21.0, Flare.Theming.ColorMath.ContrastRatio("#000000", "#FFFFFF"), 1);

    [Fact]
    public void SameColor_IsOne()
        => Assert.Equal(1.0, Flare.Theming.ColorMath.ContrastRatio("#6750A4", "#6750A4"), 2);

    [Fact]
    public void IsSymmetric()
        => Assert.Equal(
            Flare.Theming.ColorMath.ContrastRatio("#6750A4", "#FFFFFF"),
            Flare.Theming.ColorMath.ContrastRatio("#FFFFFF", "#6750A4"), 4);

    [Fact]
    public void WhiteOnDarkPrimary_PassesAa()
        => Assert.True(Flare.Theming.ColorMath.ContrastRatio("#FFFFFF", "#6750A4") >= 4.5);
}

// FlareColorCustomizer shows a WCAG contrast preview once a primary color is chosen.
public class C_FlareColorCustomizerTests : FlareTestContext
{
    [Fact]
    public void SelectingPreset_ShowsContrastVerdict()
    {
        var cut = Render<FlareColorCustomizer>();
        Assert.Empty(cut.FindAll(".flare-color-customizer__contrast")); // nothing chosen yet

        cut.Find("button.flare-color-customizer__swatch").Click();      // pick the first preset

        Assert.NotEmpty(cut.FindAll(".flare-color-customizer__contrast"));
        var verdict = cut.Find(".flare-color-customizer__contrast-badge").TextContent.Trim();
        Assert.Contains(verdict, new[] { "AAA", "AA", "AA Large", "Fail" });
    }

    [Fact]
    public void ShowContrastFalse_HidesPreview()
    {
        var cut = Render<FlareColorCustomizer>(p => p.Add(c => c.ShowContrast, false));
        cut.Find("button.flare-color-customizer__swatch").Click();
        Assert.Empty(cut.FindAll(".flare-color-customizer__contrast"));
    }
}

// Relevance scoring used by the Fuzzy search option on Autocomplete / MultiSelect.
public class C_FlareSearchTests
{
    [Fact]
    public void Exact_BeatsPrefix()
        => Assert.True(FlareSearch.Score("lo", "lo") > FlareSearch.Score("london", "lo"));

    [Fact]
    public void Prefix_BeatsContains()
        => Assert.True(FlareSearch.Score("london", "lon") > FlareSearch.Score("clone", "lon"));

    [Fact]
    public void ShorterPrefix_RanksHigher()
        => Assert.True(FlareSearch.Score("London", "lo") > FlareSearch.Score("Los Angeles", "lo"));

    [Fact]
    public void Subsequence_Matches_ButRanksBelowSubstring()
    {
        Assert.True(FlareSearch.Score("a-b-c", "abc") > 0);               // subsequence (not contiguous)
        Assert.True(FlareSearch.Score("xabcy", "abc") > FlareSearch.Score("a-b-c", "abc")); // substring wins
    }

    [Fact]
    public void NoMatch_IsZero() => Assert.Equal(0, FlareSearch.Score("apple", "xyz"));

    [Fact]
    public void EmptyQuery_MatchesAll() => Assert.Equal(1, FlareSearch.Score("anything", ""));

    [Fact]
    public void Rank_FiltersOutNonMatches_AndOrdersBestFirst()
    {
        var ranked = FlareSearch.Rank(
            new[] { "Los Angeles", "London", "Paris" },
            s => FlareSearch.Score(s, "lo")).ToList();
        Assert.Equal(new[] { "London", "Los Angeles" }, ranked);          // Paris dropped, London first
    }
}

// FlareStepper.OnStepChanging is an async guard that can veto a step change (e.g. validate first).
public class C_FlareStepperGuardTests : FlareTestContext
{
    private static RenderFragment ThreeSteps() => b =>
    {
        for (var i = 0; i < 3; i++)
        {
            b.OpenComponent<FlareStep>(i * 2);
            b.AddAttribute(i * 2 + 1, nameof(FlareStep.Label), (object)$"Step {i}");
            b.CloseComponent();
        }
    };

    [Fact]
    public async Task OnStepChanging_ReturningFalse_BlocksAdvance()
    {
        var cut = Render<FlareStepper>(p => p
            .Add(s => s.OnStepChanging, _ => Task.FromResult(false))
            .AddChildContent(ThreeSteps()));
        await cut.InvokeAsync(() => cut.Instance.Next());
        Assert.Equal(0, cut.Instance.ActiveIndex);
    }

    [Fact]
    public async Task OnStepChanging_ReturningTrue_Advances()
    {
        var cut = Render<FlareStepper>(p => p
            .Add(s => s.OnStepChanging, _ => Task.FromResult(true))
            .AddChildContent(ThreeSteps()));
        await cut.InvokeAsync(() => cut.Instance.Next());
        Assert.Equal(1, cut.Instance.ActiveIndex);
    }

    [Fact]
    public async Task OnStepChanging_ReceivesFromAndTo()
    {
        StepperChange seen = default;
        var cut = Render<FlareStepper>(p => p
            .Add(s => s.OnStepChanging, c => { seen = c; return Task.FromResult(true); })
            .AddChildContent(ThreeSteps()));
        await cut.InvokeAsync(() => cut.Instance.Next());
        Assert.Equal(0, seen.From);
        Assert.Equal(1, seen.To);
    }
}

// FlareDateRangePicker Calendar mode: click a start day then an end day to select a range (swapping
// when the second click precedes the first). Fields mode (default) renders the two-input layout.
public class C_FlareDateRangeCalendarTests : FlareTestContext
{
    [Fact]
    public void Default_IsFieldsMode_NoInlineCalendar()
    {
        var cut = Render<FlareDateRangePicker>();
        Assert.Empty(cut.FindAll(".flare-daterangepicker__calendar"));
        Assert.NotEmpty(cut.FindAll(".flare-daterangepicker__fields"));
    }

    [Fact]
    public async Task Calendar_TwoClicks_SelectOrderedRange()
    {
        var cut = Render<FlareDateRangePicker>(p => p.Add(c => c.Mode, DateRangePickerMode.Calendar));
        await cut.InvokeAsync(() => cut.FindAll(".flare-picker__day")[10].Click());
        Assert.NotNull(cut.Instance.StartDate);
        Assert.Null(cut.Instance.EndDate);                       // first click sets only the start

        await cut.InvokeAsync(() => cut.FindAll(".flare-picker__day")[24].Click());
        Assert.NotNull(cut.Instance.EndDate);
        Assert.True(cut.Instance.StartDate <= cut.Instance.EndDate);
    }

    [Fact]
    public async Task Calendar_SecondClickEarlier_Swaps()
    {
        var cut = Render<FlareDateRangePicker>(p => p.Add(c => c.Mode, DateRangePickerMode.Calendar));
        await cut.InvokeAsync(() => cut.FindAll(".flare-picker__day")[24].Click());   // later day first
        await cut.InvokeAsync(() => cut.FindAll(".flare-picker__day")[10].Click());   // earlier second
        Assert.True(cut.Instance.StartDate <= cut.Instance.EndDate);                       // swapped into order
    }

    [Fact]
    public void DefaultPresets_ArePublic_AndCombineWithCustom()
    {
        Assert.NotEmpty(FlareDateRangePicker.DefaultPresets);
        var combined = new List<DateRangePreset>(FlareDateRangePicker.DefaultPresets)
        {
            new("Sprint", t => (t.AddDays(-13), t)),
        };
        var cut = Render<FlareDateRangePicker>(p => p
            .Add(c => c.ShowPresets, true)
            .Add(c => c.Presets, combined));
        // every built-in preset plus the custom one renders a chip
        Assert.Equal(combined.Count, cut.FindAll(".flare-daterangepicker__preset").Count);
    }
}

// FlareAccordion: auto-collapsing a sibling now notifies (two-way bind stays in sync), and a panel's
// OnBeforeToggle can veto a toggle.
public class C_FlareAccordionToggleTests : FlareTestContext
{
    [Fact]
    public async Task SingleExpand_AutoCollapsesSibling_AndNotifies()
    {
        var p0States = new List<bool>();
        var cut = Render<FlareAccordion>(p => p.AddChildContent(b =>
        {
            b.OpenComponent<FlareAccordionPanel>(0);
            b.AddAttribute(1, nameof(FlareAccordionPanel.Header), (object)"P0");
            b.AddAttribute(2, nameof(FlareAccordionPanel.ExpandedChanged),
                EventCallback.Factory.Create<bool>(this, v => p0States.Add(v)));
            b.CloseComponent();
            b.OpenComponent<FlareAccordionPanel>(3);
            b.AddAttribute(4, nameof(FlareAccordionPanel.Header), (object)"P1");
            b.CloseComponent();
        }));

        await cut.InvokeAsync(() => cut.FindAll("button[aria-expanded]")[0].Click()); // expand P0
        await cut.InvokeAsync(() => cut.FindAll("button[aria-expanded]")[1].Click()); // expand P1 -> P0 collapses

        Assert.Contains(true, p0States);
        Assert.Contains(false, p0States);  // the auto-collapse fired ExpandedChanged(false) -- the bug fix
    }

    [Fact]
    public async Task OnBeforeToggle_ReturningFalse_BlocksExpand()
    {
        var cut = Render<FlareAccordionPanel>(p => p
            .Add(x => x.Header, "P")
            .Add(x => x.OnBeforeToggle, _ => Task.FromResult(false)));
        await cut.InvokeAsync(() => cut.Find("button[aria-expanded]").Click());
        Assert.Equal("false", cut.Find("button[aria-expanded]").GetAttribute("aria-expanded"));
    }
}

// FlareMenu keyboard a11y: opening sets aria-activedescendant to the first item and arrow keys move it.
public class C_FlareMenuKeyboardTests : FlareTestContext
{
    private static RenderFragment ThreeItems() => b =>
    {
        for (var i = 0; i < 3; i++)
        {
            b.OpenComponent<FlareMenuItem>(i * 2);
            b.AddAttribute(i * 2 + 1, nameof(FlareMenuItem.ChildContent),
                (RenderFragment)(cb => cb.AddContent(0, $"Item {i}")));
            b.CloseComponent();
        }
    };

    [Fact]
    public async Task Open_SetsActiveDescendant_AndArrowMovesIt()
    {
        var cut = Render<FlareMenu>(p => p
            .Add(m => m.Activator, "<span>open</span>")
            .Add(m => m.ChildContent, ThreeItems()));

        await cut.InvokeAsync(() => cut.Find("[aria-haspopup=menu]").Click());   // open -> focuses first item

        var ad1 = cut.Find("[role=menu]").GetAttribute("aria-activedescendant");
        Assert.False(string.IsNullOrEmpty(ad1));                                  // points at the first item

        await cut.InvokeAsync(() => cut.Find("[role=menu]").KeyDown(
            new Microsoft.AspNetCore.Components.Web.KeyboardEventArgs { Key = "ArrowDown" }));
        var ad2 = cut.Find("[role=menu]").GetAttribute("aria-activedescendant");
        Assert.NotEqual(ad1, ad2);                                               // moved to the next item
    }
}

// FlarePopover Trigger="Click" toggles open from the anchor without external wiring.
public class C_FlarePopoverTriggerTests : FlareTestContext
{
    [Fact]
    public async Task ClickTrigger_TogglesOpenFromAnchor()
    {
        var states = new List<bool>();
        var cut = Render<FlarePopover>(p => p
            .Add(x => x.Trigger, PopoverTrigger.Click)
            .Add(x => x.AnchorContent, "<button>open</button>")
            .Add(x => x.OpenChanged, EventCallback.Factory.Create<bool>(this, v => states.Add(v))));

        await cut.InvokeAsync(() => cut.Find("span[style*=cursor]").Click());
        Assert.Equal(new[] { true }, states);   // anchor click requested open
    }

    [Fact]
    public void ManualTrigger_DoesNotWrapAnchor()
    {
        var cut = Render<FlarePopover>(p => p.Add(x => x.AnchorContent, "<button>x</button>"));
        Assert.Empty(cut.FindAll("span[style*=cursor]"));   // default Manual: no built-in handler
    }
}
