namespace Flare.Css.Classes;

/// <summary>CSS classes for codeblock.</summary>
public static class Codeblock
{
    /// <summary>The <c>flare-codeblock</c> CSS class.</summary>
    public const string Root = "flare-codeblock";
    /// <summary>The <c>flare-codeblock--line-numbers</c> CSS class.</summary>
    public const string LineNumbers = "flare-codeblock--line-numbers";
    /// <summary>The <c>flare-codeblock--readonly</c> CSS class.</summary>
    public const string Readonly = "flare-codeblock--readonly";
    /// <summary>The <c>flare-codeblock__highlight</c> CSS class.</summary>
    public const string Highlight = "flare-codeblock__highlight";
    /// <summary>The <c>flare-codeblock__linenumbers</c> CSS class.</summary>
    public const string Linenumbers = "flare-codeblock__linenumbers";
    /// <summary>The <c>flare-codeblock__textarea</c> CSS class.</summary>
    public const string Textarea = "flare-codeblock__textarea";

    /// <summary>
    /// Syntax-highlight token classes emitted by the built-in tokenizer (flare-highlight.js
    /// <c>getHighlightedHtml</c>) inside <see cref="Highlight"/> and colored by codeblock.css.
    /// They are a JS-to-CSS contract; these constants exist so the classes are discoverable
    /// and tracked (not referenced from C#).
    /// </summary>
    public static class Token
    {
        /// <summary>The <c>flare-hl-keyword</c> token class (language keywords).</summary>
        public const string Keyword = "flare-hl-keyword";
        /// <summary>The <c>flare-hl-type</c> token class (type names / capitalized identifiers).</summary>
        public const string Type = "flare-hl-type";
        /// <summary>The <c>flare-hl-string</c> token class (string literals).</summary>
        public const string String = "flare-hl-string";
        /// <summary>The <c>flare-hl-number</c> token class (numeric literals).</summary>
        public const string Number = "flare-hl-number";
        /// <summary>The <c>flare-hl-literal</c> token class (true/false/null and similar).</summary>
        public const string Literal = "flare-hl-literal";
        /// <summary>The <c>flare-hl-property</c> token class (object/member properties).</summary>
        public const string Property = "flare-hl-property";
        /// <summary>The <c>flare-hl-tag</c> token class (HTML/XML tag names).</summary>
        public const string Tag = "flare-hl-tag";
        /// <summary>The <c>flare-hl-attr</c> token class (HTML/Razor/XML attribute names).</summary>
        public const string Attr = "flare-hl-attr";
        /// <summary>The <c>flare-hl-meta</c> token class (directives, preprocessor, doctype).</summary>
        public const string Meta = "flare-hl-meta";
        /// <summary>The <c>flare-hl-comment</c> token class (comments).</summary>
        public const string Comment = "flare-hl-comment";
    }
}
