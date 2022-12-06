using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Analysis.FSharp;
using BlazorTextEditor.RazorLib.Analysis.FSharp.Decoration;
using BlazorTextEditor.RazorLib.Analysis.FSharp.SyntaxActors;
using BlazorTextEditor.RazorLib.Lexing;
using BlazorTextEditor.Tests.TestDataFolder;

namespace BlazorTextEditor.Tests.Lexers;

public class LexFSharpTests
{
    [Fact]
    public async Task LexKeywords()
    {
        var text = TestData.FSharp.EXAMPLE_TEXT_21_LINES
            .ReplaceLineEndings("\n");

        var expectedKeywordTextEditorTextSpans = new[]
        {
            new TextEditorTextSpan(0, 3, (byte)FSharpDecorationKind.Keyword),
            new TextEditorTextSpan(26, 30, (byte)FSharpDecorationKind.Keyword),
            new TextEditorTextSpan(65, 68, (byte)FSharpDecorationKind.Keyword),
            new TextEditorTextSpan(94, 97, (byte)FSharpDecorationKind.Keyword),
            new TextEditorTextSpan(123, 126, (byte)FSharpDecorationKind.Keyword),
            new TextEditorTextSpan(129, 131, (byte)FSharpDecorationKind.Keyword),
            new TextEditorTextSpan(145, 147, (byte)FSharpDecorationKind.Keyword),
            new TextEditorTextSpan(160, 163, (byte)FSharpDecorationKind.Keyword),
            new TextEditorTextSpan(247, 250, (byte)FSharpDecorationKind.Keyword),
        };
        
        var fSharpLexer = new TextEditorFSharpLexer();

        var textEditorTextSpans = 
            await fSharpLexer.Lex(text);

        textEditorTextSpans = textEditorTextSpans
            .Where(x => x.DecorationByte == (byte)FSharpDecorationKind.Keyword)
            .OrderBy(x => x.StartingIndexInclusive)
            .ToImmutableArray();

        Assert.Equal(expectedKeywordTextEditorTextSpans, textEditorTextSpans);
    }
}