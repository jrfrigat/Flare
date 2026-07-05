using Flare.Components.Combobox;

namespace Flare.Components.Tests.Combobox;

// Pure engine tests - no bUnit, no rendering. Drive intents on ComboboxState<T> and assert on state.
public class ComboboxEngineTests
{
    private static readonly string[] Fruits = { "Apple", "Apricot", "Banana", "Cherry" };

    private static ComboboxState<string> Single(
        IEnumerable<string>? items = null,
        Func<ComboboxPolicy<string>, ComboboxPolicy<string>>? tweak = null)
    {
        var policy = ComboboxPolicy<string>.ForSingleSelect() with { Label = s => s };
        if (tweak is not null) policy = tweak(policy);
        var state = new ComboboxState<string>(policy);
        state.SetSource((items ?? Fruits).ToList());
        return state;
    }

    private static ComboboxState<string> Multi(IEnumerable<string>? items = null, int max = 0)
    {
        var policy = ComboboxPolicy<string>.ForMultiSelect() with { Label = s => s, MaxSelections = max };
        var state = new ComboboxState<string>(policy);
        state.SetSource((items ?? Fruits).ToList());
        return state;
    }

    [Fact]
    public void Open_KeepSelected_highlights_first_enabled_when_nothing_selected()
    {
        var s = Single();
        s.Open();
        Assert.True(s.IsOpen);
        Assert.Equal(0, s.HighlightedIndex);
    }

    [Fact]
    public void Open_KeepSelected_highlights_the_selected_item()
    {
        var s = Single();
        s.SetSelection(new[] { "Banana" });
        s.Open();
        Assert.Equal(2, s.HighlightedIndex);
    }

    [Fact]
    public void ArrowDown_on_closed_opens_and_highlights_first()
    {
        var s = Single();
        s.MoveHighlight(1);
        Assert.True(s.IsOpen);
        Assert.Equal(0, s.HighlightedIndex);
    }

    [Fact]
    public void MoveHighlight_skips_disabled_options()
    {
        var s = Single(tweak: p => p with { ItemDisabled = x => x == "Apricot" });
        s.Open(FocusStrategy.First);          // Apple (0)
        s.MoveHighlight(1);                    // skips Apricot (1) -> Banana (2)
        Assert.Equal(2, s.HighlightedIndex);
    }

    [Fact]
    public void MoveHighlight_without_wrap_stops_at_end()
    {
        var s = Single();
        s.Open(FocusStrategy.Last);            // Cherry (3)
        s.MoveHighlight(1);                    // no wrap -> stays
        Assert.Equal(3, s.HighlightedIndex);
    }

    [Fact]
    public void MoveHighlight_with_wrap_cycles()
    {
        var s = Single(tweak: p => p with { WrapNavigation = true });
        s.Open(FocusStrategy.First);           // Apple (0)
        s.MoveHighlight(-1);                    // wraps to Cherry (3)
        Assert.Equal(3, s.HighlightedIndex);
    }

    [Fact]
    public void Single_commit_selects_and_closes()
    {
        var s = Single();
        s.Open();
        var r = s.Commit("Banana");
        Assert.Equal(CommitKind.Changed, r.Kind);
        Assert.True(r.MenuClosed);
        Assert.False(s.IsOpen);
        Assert.Equal(new[] { "Banana" }, s.Selection.Selected);
    }

    [Fact]
    public void Single_commit_replaces_previous()
    {
        var s = Single();
        s.Commit("Apple");
        s.Commit("Cherry");
        Assert.Equal(new[] { "Cherry" }, s.Selection.Selected);
    }

    [Fact]
    public void CommitHighlighted_with_no_highlight_is_noop()
    {
        var s = Single();
        var r = s.CommitHighlighted();
        Assert.Equal(CommitKind.NoOp, r.Kind);
        Assert.Empty(s.Selection.Selected);
    }

    [Fact]
    public void Multi_commit_toggles_and_stays_open()
    {
        var s = Multi();
        s.Open();
        s.Commit("Apple");
        s.Commit("Banana");
        Assert.True(s.IsOpen);
        Assert.Equal(new[] { "Apple", "Banana" }, s.Selection.Selected);
        s.Commit("Apple");                     // toggle off
        Assert.Equal(new[] { "Banana" }, s.Selection.Selected);
    }

    [Fact]
    public void Multi_commit_clears_filter()
    {
        // A searchable multi-select (opt-in): committing a pick clears the query so the next pick starts fresh.
        var policy = ComboboxPolicy<string>.ForMultiSelect() with { Label = s => s, Searchable = true };
        var s = new ComboboxState<string>(policy);
        s.SetSource(Fruits.ToList());
        s.SetInput("ban");
        Assert.Single(s.Collection.Filtered);
        s.Commit("Banana");
        Assert.Equal(string.Empty, s.Input);
        Assert.Equal(4, s.Collection.Count);   // filter cleared -> all visible again
    }

    [Fact]
    public void MaxSelections_rejects_beyond_cap()
    {
        var s = Multi(max: 2);
        s.Commit("Apple");
        s.Commit("Banana");
        var r = s.Commit("Cherry");
        Assert.Equal(CommitKind.RejectedMax, r.Kind);
        Assert.True(r.RejectedForMax);
        Assert.Equal(2, s.Selection.Count);
    }

    [Fact]
    public void DisallowEmpty_blocks_deselecting_the_last_item()
    {
        var policy = ComboboxPolicy<string>.ForMultiSelect() with { Label = s => s, DisallowEmpty = true };
        var s = new ComboboxState<string>(policy);
        s.SetSource(Fruits.ToList());
        s.Commit("Apple");
        var r = s.Commit("Apple");             // try to remove the last one
        Assert.Equal(CommitKind.RejectedEmpty, r.Kind);
        Assert.Single(s.Selection.Selected);
    }

    [Fact]
    public void Disabled_option_cannot_be_committed()
    {
        var s = Single(tweak: p => p with { ItemDisabled = x => x == "Banana" });
        var r = s.Commit("Banana");
        Assert.Equal(CommitKind.RejectedDisabled, r.Kind);
        Assert.Empty(s.Selection.Selected);
    }

    [Fact]
    public void SetInput_filters_and_autohighlights_first_match()
    {
        var s = new ComboboxState<string>(ComboboxPolicy<string>.ForCombobox() with { Label = x => x });
        s.SetSource(Fruits.ToList());
        s.SetInput("ap");
        Assert.Equal(new[] { "Apple", "Apricot" }, s.Collection.Filtered);
        Assert.Equal(0, s.HighlightedIndex);
        Assert.True(s.IsOpen);                 // ForCombobox opens on input
    }

    [Fact]
    public void Hover_highlight_does_not_touch_selection()
    {
        var s = Single();
        s.Open();
        s.Commit("Apple");                     // closes (single)
        s.Open();
        s.HighlightAt(2);
        Assert.Equal(2, s.HighlightedIndex);
        Assert.Equal(new[] { "Apple" }, s.Selection.Selected);   // selection unchanged by hover
    }

    [Fact]
    public void TypeAhead_matches_prefix_and_opens()
    {
        var s = Single();
        Assert.False(s.IsOpen);
        s.TypeAhead('b');
        Assert.True(s.IsOpen);
        Assert.Equal("Banana", s.HighlightedItem);
    }

    [Fact]
    public void TypeAhead_repeated_char_cycles_through_matches()
    {
        var s = Single();                      // Apple, Apricot both start with A
        s.TypeAhead('a');
        Assert.Equal("Apple", s.HighlightedItem);
        s.TypeAhead('a');
        Assert.Equal("Apricot", s.HighlightedItem);
        s.TypeAhead('a');                      // wraps back
        Assert.Equal("Apple", s.HighlightedItem);
    }

    [Fact]
    public void TypeAhead_multichar_prefix_refines()
    {
        var s = Single();
        s.TypeAhead('a');
        s.TypeAhead('p');
        s.TypeAhead('r');
        Assert.Equal("Apricot", s.HighlightedItem);
    }

    [Fact]
    public void ResetTypeahead_starts_a_fresh_buffer()
    {
        var s = Single();
        s.TypeAhead('a');                      // Apple
        s.ResetTypeahead();
        s.TypeAhead('b');                      // fresh -> Banana, not "ab"
        Assert.Equal("Banana", s.HighlightedItem);
    }

    [Fact]
    public void SelectAllState_is_tristate_over_enabled_visible()
    {
        var s = Multi();
        Assert.Equal(TriState.None, s.SelectAllState());
        s.Commit("Apple");
        Assert.Equal(TriState.Some, s.SelectAllState());
        s.ToggleSelectAll();
        Assert.Equal(TriState.All, s.SelectAllState());
        s.ToggleSelectAll();
        Assert.Equal(TriState.None, s.SelectAllState());
    }

    [Fact]
    public void SelectAll_excludes_disabled_options()
    {
        var policy = ComboboxPolicy<string>.ForMultiSelect() with { Label = x => x, ItemDisabled = x => x == "Cherry" };
        var s = new ComboboxState<string>(policy);
        s.SetSource(Fruits.ToList());
        s.ToggleSelectAll();
        Assert.DoesNotContain("Cherry", s.Selection.Selected);
        Assert.Equal(3, s.Selection.Count);
    }

    [Fact]
    public void RemoveLast_pops_the_most_recent_selection()
    {
        var s = Multi();
        s.Commit("Apple");
        s.Commit("Banana");
        Assert.True(s.RemoveLast());
        Assert.Equal(new[] { "Apple" }, s.Selection.Selected);
    }

    [Fact]
    public void Grouped_rows_interleave_headers_before_each_group()
    {
        var policy = ComboboxPolicy<string>.ForSingleSelect() with
        {
            Label = x => x,
            Group = x => x.StartsWith("A") ? "A" : "Other",
        };
        var s = new ComboboxState<string>(policy);
        s.SetSource(Fruits.ToList());
        var rows = s.Collection.BuildRows();
        Assert.True(rows[0].IsHeader);
        Assert.Equal("A", rows[0].Group);
        Assert.Equal("Apple", rows[1].Item);
        Assert.Equal("Apricot", rows[2].Item);
        Assert.True(rows[3].IsHeader);
        Assert.Equal("Other", rows[3].Group);
        // option rows carry their filtered index for a stable activedescendant id
        Assert.Equal(0, rows[1].OptionIndex);
        Assert.Equal(1, rows[2].OptionIndex);
    }

    [Fact]
    public void Changed_event_fires_on_intents()
    {
        var s = Single();
        var count = 0;
        s.Changed += () => count++;
        s.Open();
        s.MoveHighlight(1);
        s.Commit("Apple");
        Assert.True(count >= 3);
    }

    [Fact]
    public void ScrollRequested_fires_only_on_keyboard_highlight_moves()
    {
        var s = Single();
        var scrolls = new List<int>();
        s.ScrollRequested += i => scrolls.Add(i);
        s.Open();                 // no scroll on open
        s.HighlightAt(2);         // hover -> no scroll
        Assert.Empty(scrolls);
        s.MoveHighlight(1);       // keyboard -> scroll
        Assert.Single(scrolls);
    }

    [Fact]
    public void Fuzzy_ranks_best_match_first()
    {
        var policy = ComboboxPolicy<string>.ForCombobox() with { Label = x => x, Fuzzy = true };
        var s = new ComboboxState<string>(policy);
        s.SetSource(new List<string> { "Los Angeles", "London" });
        s.SetInput("lo");
        Assert.Equal("London", s.Collection.Filtered[0]);   // prefix beats word-start
    }
}
