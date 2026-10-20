namespace Flare.Guards.Tests;

public sealed class JsCatchGuardScannerTests
{
    private static void Protected(string source) => Assert.Empty(JsCatchGuardScanner.FindOffenders(source));

    private static void Offender(string source, int line = 1)
        => Assert.Equal([line], JsCatchGuardScanner.FindOffenders(source));

    [Fact]
    public void A_following_js_catch_protects_the_disconnected_one()
    {
        Protected("try { Run(); } catch (JSDisconnectedException) { } catch (JSException) { }");
        Protected("""
            try { Run(); }
            catch (JSDisconnectedException) { }
            catch (JSException) { }
            """);
        Protected("try { Run(); } catch (JSDisconnectedException error) { } catch (JSException problem) { }");
        Protected("try { Run(); } catch (Microsoft.JSInterop.JSDisconnectedException) { } catch (JSException) { }");
    }

    [Fact]
    public void A_commented_out_catch_does_not_protect()
    {
        Offender("""
            try { Run(); } catch (JSDisconnectedException) { }
            // catch (JSException) { }
            """);
        Offender("try { Run(); } catch (JSDisconnectedException) { } // catch (JSException) { }");
        Offender("""
            try { Run(); } catch (JSDisconnectedException) { }
            /* catch (JSException) { } */
            """);
        Offender("""
            try { Run(); } catch (JSDisconnectedException) { }
            /*
            catch (JSException) { }
            */ try { Run(); } catch (JSException) { }
            """);
    }

    [Fact]
    public void A_string_holding_catch_text_does_not_protect()
    {
        Offender("""try { Run(); } catch (JSDisconnectedException) { var text = "catch (JSException)"; }""");
        Offender("""try { Run(); } catch (JSDisconnectedException) { var text = @"catch (JSException)"; }""");
        Offender(""""try { Run(); } catch (JSDisconnectedException) { var text = """catch (JSException)"""; }"""");
    }

    [Fact]
    public void A_nested_catch_inside_the_body_does_not_protect_the_outer_one()
    {
        Offender("""
            try { Run(); } catch (JSDisconnectedException)
            {
                try { Run(); } catch (JSException) { }
            }
            """);
    }

    [Fact]
    public void Only_a_js_catch_of_the_same_try_protects()
    {
        Offender("try { Run(); } catch (JSDisconnectedException) { } catch (Exception) { }");
        Offender("try { Run(); } catch (JSDisconnectedException) { } finally { Stop(); }");
        Offender("""
            try { Run(); } catch (JSDisconnectedException) { }
            try { Run(); } catch (JSException) { }
            """);
    }

    [Fact]
    public void Braces_in_characters_and_escaped_quotes_do_not_break_the_body()
    {
        Protected("""try { Run(); } catch (JSDisconnectedException) { var c = '}'; } catch (JSException) { }""");
        Protected("""try { Run(); } catch (JSDisconnectedException) { var s = @"a""}"; } catch (JSException) { }""");
    }

    [Fact]
    public void An_apostrophe_in_razor_prose_does_not_hide_the_code_after_it()
    {
        Offender("""
            <p>Don't stop</p>
            @code { void Run() { try { Work(); } catch (JSDisconnectedException) { } } }
            """, 2);
    }

    [Fact]
    public void A_filter_and_a_body_with_braces_still_find_the_next_catch()
    {
        Protected("""
            try { Run(); } catch (JSDisconnectedException) when (IsAlive()) { if (true) { Stop(); } }
            catch (JSException) { }
            """);
    }
}
