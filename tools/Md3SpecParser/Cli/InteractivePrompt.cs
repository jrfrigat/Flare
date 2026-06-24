using Flare.Tools.Md3SpecParser.Generation;

namespace Flare.Tools.Md3SpecParser.Cli;

/// <summary>Console prompts for mode and type selection.</summary>
public static class InteractivePrompt
{
    /// <summary>Asks a yes/no question (default No) and returns the answer.</summary>
    public static bool Confirm(string question)
    {
        Console.Write($"{question} [y/N]: ");
        var line = Console.ReadLine()?.Trim().ToLowerInvariant();
        return line is "y" or "yes";
    }

    /// <summary>
    /// Lets the user pick component types. Uses an interactive checkbox list (arrow
    /// keys to move, Space to toggle, A to select/deselect all, Enter to confirm,
    /// Esc to cancel); falls back to a numbered prompt when input is redirected or the
    /// console does not support key reading.
    /// </summary>
    public static IReadOnlyList<TypeGroup> AskTypes(IReadOnlyList<TypeGroup> candidates, GenerationMode mode)
    {
        if (candidates.Count == 0) return candidates;
        if (Console.IsInputRedirected) return AskTypesNumbered(candidates);

        try
        {
            return AskTypesCheckbox(candidates, mode);
        }
        catch (IOException)
        {
            return AskTypesNumbered(candidates);
        }
        catch (InvalidOperationException)
        {
            return AskTypesNumbered(candidates);
        }
    }

    private static IReadOnlyList<TypeGroup> AskTypesCheckbox(IReadOnlyList<TypeGroup> candidates, GenerationMode mode)
    {
        var n = candidates.Count;
        var chosen = new bool[n];
        // In New mode pre-check only the missing files; in Full pre-check everything.
        for (var i = 0; i < n; i++)
            chosen[i] = mode == GenerationMode.Full || !SpecGenerator.Exists(candidates[i]);
        var cursor = 0;
        var cols = Columns.For(candidates);

        Console.WriteLine();
        Console.WriteLine("Select component types:");
        Console.WriteLine("  ↑/↓ move · Space toggle · A all/none · Enter confirm · Esc cancel");
        Console.WriteLine();
        Console.WriteLine(cols.Header());

        for (var i = 0; i < n; i++)
            Console.WriteLine(RenderItem(candidates[i], chosen[i], i == cursor, cols));

        var startTop = Math.Max(0, Console.CursorTop - n);

        while (true)
        {
            var key = Console.ReadKey(intercept: true);
            switch (key.Key)
            {
                case ConsoleKey.UpArrow or ConsoleKey.K:
                    cursor = (cursor - 1 + n) % n;
                    break;
                case ConsoleKey.DownArrow or ConsoleKey.J:
                    cursor = (cursor + 1) % n;
                    break;
                case ConsoleKey.Spacebar:
                    chosen[cursor] = !chosen[cursor];
                    break;
                case ConsoleKey.A:
                    var anyOff = Array.Exists(chosen, c => !c);
                    Array.Fill(chosen, anyOff);
                    break;
                case ConsoleKey.Enter:
                    MoveBelow(startTop, n);
                    return candidates.Where((_, i) => chosen[i]).ToList();
                case ConsoleKey.Escape or ConsoleKey.Q:
                    MoveBelow(startTop, n);
                    return Array.Empty<TypeGroup>();
            }

            Redraw(candidates, chosen, cursor, startTop, cols);
        }
    }

    private static void Redraw(
        IReadOnlyList<TypeGroup> candidates, bool[] chosen, int cursor, int startTop, Columns cols)
    {
        Console.SetCursorPosition(0, startTop);
        for (var i = 0; i < candidates.Count; i++)
            Console.WriteLine(RenderItem(candidates[i], chosen[i], i == cursor, cols));
    }

    private static void MoveBelow(int startTop, int n)
    {
        var row = Math.Min(startTop + n, Math.Max(0, Console.BufferHeight - 1));
        Console.SetCursorPosition(0, row);
    }

    private static string RenderItem(TypeGroup g, bool isChecked, bool isCursor, Columns cols)
    {
        var pointer = isCursor ? ">" : " ";
        var box = isChecked ? "[x]" : "[ ]";
        var state = SpecGenerator.Exists(g) ? "exists" : "new";
        var type = g.Type.PadRight(cols.Type);
        var src = g.Sources.Count.ToString().PadLeft(cols.Src);
        return Pad($" {pointer} {box} {type}  {src}  {state}");
    }

    /// <summary>Column widths for the aligned type table.</summary>
    private readonly record struct Columns(int Type, int Src)
    {
        private const string Prefix = "       "; // aligns under " > [x] "

        public static Columns For(IReadOnlyList<TypeGroup> candidates) => new(
            Math.Max(candidates.Max(g => g.Type.Length), "Component".Length),
            Math.Max(candidates.Max(g => g.Sources.Count.ToString().Length), "Src".Length));

        public string Header() =>
            $"{Prefix}{"Component".PadRight(Type)}  {"Src".PadLeft(Src)}  State";
    }

    private static string Pad(string text)
    {
        int width;
        try { width = Math.Max(Console.WindowWidth - 1, text.Length); }
        catch { width = text.Length; }
        return text.PadRight(width);
    }

    /// <summary>
    /// Numbered fallback: accepts "a"/"all", or comma/space separated indices.
    /// </summary>
    private static IReadOnlyList<TypeGroup> AskTypesNumbered(IReadOnlyList<TypeGroup> candidates)
    {
        Console.WriteLine();
        Console.WriteLine($"Component types ({candidates.Count}):");
        for (var i = 0; i < candidates.Count; i++)
        {
            var g = candidates[i];
            var note = SpecGenerator.Exists(g) ? "[exists]" : "[new]   ";
            Console.WriteLine($"  {i + 1,2}) {note} {g.Type}  ({g.Sources.Count} src)");
        }

        while (true)
        {
            Console.Write("Select types ('a' = all, e.g. '1,3,5'): ");
            var line = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(line)) return Array.Empty<TypeGroup>();

            if (line.Equals("a", StringComparison.OrdinalIgnoreCase) ||
                line.Equals("all", StringComparison.OrdinalIgnoreCase))
                return candidates;

            var picked = new List<TypeGroup>();
            var ok = true;
            foreach (var tok in line.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (int.TryParse(tok, out var num) && num >= 1 && num <= candidates.Count)
                    picked.Add(candidates[num - 1]);
                else { Console.WriteLine($"  Invalid selection: '{tok}'"); ok = false; break; }
            }

            if (ok && picked.Count > 0)
                return picked.Distinct().ToList();
        }
    }
}
