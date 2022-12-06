using System.Collections.Immutable;
using System.Text;
using BlazorTextEditor.RazorLib.Analysis.CSharp.Decoration;
using BlazorTextEditor.RazorLib.Analysis.CSharp.SyntaxActors;
using BlazorTextEditor.RazorLib.Analysis.Html;
using BlazorTextEditor.RazorLib.Analysis.Html.Decoration;
using BlazorTextEditor.RazorLib.Analysis.Html.InjectedLanguage;
using BlazorTextEditor.RazorLib.Analysis.Html.SyntaxObjects;
using BlazorTextEditor.RazorLib.Analysis.Razor.Facts;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Analysis.Razor.SyntaxActors;

public class RazorSyntaxTree
{
    /// <summary>
    /// currentCharacterIn:<br/>
    /// - <see cref="RazorInjectedLanguageFacts.RazorInjectedLanguageDefinition.TransitionSubstring"/><br/>
    /// </summary>
    public static List<TagSyntax> ParseInjectedLanguageFragment(
        StringWalker stringWalker,
        TextEditorHtmlDiagnosticBag textEditorHtmlDiagnosticBag,
        InjectedLanguageDefinition injectedLanguageDefinition)
    {
        var injectedLanguageFragmentSyntaxes = new List<TagSyntax>();

        var codeBlockOrExpressionStartingPositionIndex = stringWalker.PositionIndex;

        var foundCodeBlock = false;

        while (!stringWalker.IsEof)
        {
            _ = stringWalker.Consume();

            string? matchedOn = null;

            if (stringWalker.CheckForSubstringRange(
                    RazorKeywords.ALL,
                    out matchedOn) &&
                matchedOn is not null)
            {
                // Razor Keyword
                //
                // @page "/counter"
                
                // Syntax highlight the keyword as a razor keyword specifically
                {
                    injectedLanguageFragmentSyntaxes.Add(
                        new InjectedLanguageFragmentSyntax(
                            ImmutableArray<IHtmlSyntax>.Empty,
                            string.Empty,
                            new TextEditorTextSpan(
                                codeBlockOrExpressionStartingPositionIndex,
                                stringWalker.PositionIndex +
                                matchedOn.Length,
                                (byte)HtmlDecorationKind.InjectedLanguageFragment)));

                    _ = stringWalker
                        .ConsumeRange(matchedOn.Length);
                }

                // TODO: See the "Perhaps a switch is needed here" comment. For now just return and revisit this.
                return injectedLanguageFragmentSyntaxes;

                // Perhaps a switch is needed here due to
                // the keywords having parameters which might vary in type?
                //
                // @page takes a string
                // what do the others take?
                // all others take a string or something else?
                //
                // switch (matchedOn)
                // {
                //     case RazorKeywords.PAGE_KEYWORD:
                //         break;
                //     case RazorKeywords.NAMESPACE_KEYWORD:
                //         break;
                //     case RazorKeywords.FUNCTIONS_KEYWORD:
                //         break;
                //     case RazorKeywords.INHERITS_KEYWORD:
                //         break;
                //     case RazorKeywords.MODEL_KEYWORD:
                //         break;
                //     case RazorKeywords.SECTION_KEYWORD:
                //         break;
                //     case RazorKeywords.HELPER_KEYWORD:
                //         break;
                // }
            }
            else if (stringWalker.CheckForSubstringRange(
                         CSharpRazorKeywords.ALL,
                         out matchedOn) &&
                     matchedOn is not null)
            {
                // C# Razor Keyword
                //
                // Example: if (true) { ... } else { ... }
                
                // Syntax highlight the keyword as a razor keyword specifically
                {
                    injectedLanguageFragmentSyntaxes.Add(
                        new InjectedLanguageFragmentSyntax(
                            ImmutableArray<IHtmlSyntax>.Empty,
                            string.Empty,
                            new TextEditorTextSpan(
                                codeBlockOrExpressionStartingPositionIndex,
                                stringWalker.PositionIndex +
                                codeBlock.CodeBlockOpening.Length,
                                (byte)HtmlDecorationKind.InjectedLanguageFragment)));

                    _ = stringWalker
                        .ConsumeRange(codeBlock.CodeBlockOpening.Length);
                }

                // I expect to use a switch here
                //
                // an if statement for example would require
                // a check for an (else if)/(else) block
                switch (matchedOn)
                {
                    case CSharpRazorKeywords.CASE_KEYWORD:
                        break;
                    case CSharpRazorKeywords.DO_KEYWORD:
                        break;
                    case CSharpRazorKeywords.DEFAULT_KEYWORD:
                        break;
                    case CSharpRazorKeywords.FOR_KEYWORD:
                        break;
                    case CSharpRazorKeywords.FOREACH_KEYWORD:
                        break;
                    case CSharpRazorKeywords.IF_KEYWORD:
                        break;
                    case CSharpRazorKeywords.ELSE_KEYWORD:
                        break;
                    case CSharpRazorKeywords.LOCK_KEYWORD:
                        break;
                    case CSharpRazorKeywords.SWITCH_KEYWORD:
                        break;
                    case CSharpRazorKeywords.TRY_KEYWORD:
                        break;
                    case CSharpRazorKeywords.CATCH_KEYWORD:
                        break;
                    case CSharpRazorKeywords.FINALLY_KEYWORD:
                        break;
                    case CSharpRazorKeywords.USING_KEYWORD:
                        break;
                    case CSharpRazorKeywords.WHILE_KEYWORD:
                        break;
                }
            }
            else if (true)
            {
                // Razor Comment
                //
                // Example: @* This is a comment*@
            }
            else if (true)
            {
                // Razor Inline Expression
                //
                // Example: @myVariable
                // Example: @(myVariable)

                if (true)
                {
                    // Razor Implicit Inline Expression
                    //
                    // Example: @myVariable
                }
                else
                {
                    // Razor Explicit Inline Expression
                    //
                    // Example: @(myVariable)
                }
            }
            else
            {
                // Razor Code Block
                //
                // NOTE: There might be a mixture
                // of C# and HTML in the Razor Code Blocks.
                //
                // NOTE:
                // -<text></text>
                //     Render multiple lines of text without rendering an HTML Element
                // -@:
                //     Render a single line of text without rendering an HTML Element
                // -ANY tag can be used within the razor code blocks
                //     Example: <div></div> or <MyBlazorComponent/>
            }
        }

        stringWalker.WhileNotEndOfFile(() =>
        {
            // Try find matching code block opening syntax
            foreach (var codeBlock in injectedLanguageDefinition.InjectedLanguageCodeBlocks)
            {
                if (stringWalker.CheckForSubstring(
                        codeBlock.CodeBlockOpening))
                {
                    foundCodeBlock = true;

                    

                    var injectedLanguageOffsetPositionIndex = stringWalker.PositionIndex;

                    var injectedLanguageBuilder = new StringBuilder();

                    // > 0 means more opening brackets than closings
                    // once 0 is met then we've found the closing bracket
                    // start findMatchCounter = 1 because we're starting
                    // at the opening of the injected language code block
                    // and want to find the closing bracket of that given code block.
                    var findMatchCounter = 1;

                    // While !EOF continue checking for the respective closing syntax
                    // for the previously matched code block opening syntax.
                    stringWalker.WhileNotEndOfFile(() =>
                    {
                        injectedLanguageBuilder.Append(stringWalker.CurrentCharacter);

                        if (stringWalker.CheckForSubstring(
                                codeBlock.CodeBlockOpening) ||
                            // this or is hacky but @code{ ... } is messing things up
                            // and I am going to do this short term and come back.
                            stringWalker.CheckForSubstring("{"))
                            findMatchCounter++;
                        else if (stringWalker.CheckForSubstring(
                                     codeBlock.CodeBlockClosing))
                            findMatchCounter--;

                        if (findMatchCounter == 0)
                        {
                            // Track text span of the "}" character (example in .razor files)
                            // also will track the ending ")" character given it is the
                            // end of a code block.
                            injectedLanguageFragmentSyntaxes.Add(
                                new InjectedLanguageFragmentSyntax(
                                    ImmutableArray<IHtmlSyntax>.Empty,
                                    string.Empty,
                                    new TextEditorTextSpan(
                                        stringWalker.PositionIndex,
                                        stringWalker.PositionIndex +
                                        codeBlock.CodeBlockClosing.Length,
                                        (byte)HtmlDecorationKind.InjectedLanguageFragment)));

                            return true;
                        }

                        return false;
                    });

                    var lexer = new TextEditorCSharpLexer();

                    var classTemplateOpening = "public class Aaa{";

                    var injectedLanguageString = classTemplateOpening +
                                                 injectedLanguageBuilder;

                    var lexedInjectedLanguage = lexer.Lex(
                            injectedLanguageString)
                        .Result;

                    foreach (var lexedTokenTextSpan in lexedInjectedLanguage)
                    {
                        var startingIndexInclusive = lexedTokenTextSpan.StartingIndexInclusive +
                                                     injectedLanguageOffsetPositionIndex -
                                                     classTemplateOpening.Length;

                        var endingIndexExclusive = lexedTokenTextSpan.EndingIndexExclusive +
                                                   injectedLanguageOffsetPositionIndex -
                                                   classTemplateOpening.Length;

                        // startingIndexInclusive < 0 means it was part of the class
                        // template that was prepended so roslyn would recognize methods
                        if (lexedTokenTextSpan.StartingIndexInclusive - classTemplateOpening.Length
                            < 0)
                            continue;

                        var cSharpDecorationKind = (CSharpDecorationKind)lexedTokenTextSpan.DecorationByte;

                        switch (cSharpDecorationKind)
                        {
                            case CSharpDecorationKind.None:
                                break;
                            case CSharpDecorationKind.Method:
                                var razorMethodTextSpan = lexedTokenTextSpan with
                                {
                                    DecorationByte = (byte)HtmlDecorationKind.InjectedLanguageMethod,
                                    StartingIndexInclusive = startingIndexInclusive,
                                    EndingIndexExclusive = endingIndexExclusive,
                                };

                                injectedLanguageFragmentSyntaxes.Add(new InjectedLanguageFragmentSyntax(
                                    ImmutableArray<IHtmlSyntax>.Empty,
                                    stringWalker.GetText(razorMethodTextSpan),
                                    razorMethodTextSpan));

                                break;
                            case CSharpDecorationKind.Type:
                                var razorTypeTextSpan = lexedTokenTextSpan with
                                {
                                    DecorationByte = (byte)HtmlDecorationKind.InjectedLanguageType,
                                    StartingIndexInclusive = startingIndexInclusive,
                                    EndingIndexExclusive = endingIndexExclusive,
                                };

                                injectedLanguageFragmentSyntaxes.Add(new InjectedLanguageFragmentSyntax(
                                    ImmutableArray<IHtmlSyntax>.Empty,
                                    stringWalker.GetText(razorTypeTextSpan),
                                    razorTypeTextSpan));

                                break;
                            case CSharpDecorationKind.Parameter:
                                var razorVariableTextSpan = lexedTokenTextSpan with
                                {
                                    DecorationByte = (byte)HtmlDecorationKind.InjectedLanguageVariable,
                                    StartingIndexInclusive = startingIndexInclusive,
                                    EndingIndexExclusive = endingIndexExclusive,
                                };

                                injectedLanguageFragmentSyntaxes.Add(new InjectedLanguageFragmentSyntax(
                                    ImmutableArray<IHtmlSyntax>.Empty,
                                    stringWalker.GetText(razorVariableTextSpan),
                                    razorVariableTextSpan));

                                break;
                            case CSharpDecorationKind.StringLiteral:
                                var razorStringLiteralTextSpan = lexedTokenTextSpan with
                                {
                                    DecorationByte = (byte)HtmlDecorationKind.InjectedLanguageStringLiteral,
                                    StartingIndexInclusive = startingIndexInclusive,
                                    EndingIndexExclusive = endingIndexExclusive,
                                };

                                injectedLanguageFragmentSyntaxes.Add(new InjectedLanguageFragmentSyntax(
                                    ImmutableArray<IHtmlSyntax>.Empty,
                                    stringWalker.GetText(razorStringLiteralTextSpan),
                                    razorStringLiteralTextSpan));

                                break;
                            case CSharpDecorationKind.Keyword:
                                var razorKeywordTextSpan = lexedTokenTextSpan with
                                {
                                    DecorationByte = (byte)HtmlDecorationKind.InjectedLanguageKeyword,
                                    StartingIndexInclusive = startingIndexInclusive,
                                    EndingIndexExclusive = endingIndexExclusive,
                                };

                                injectedLanguageFragmentSyntaxes.Add(new InjectedLanguageFragmentSyntax(
                                    ImmutableArray<IHtmlSyntax>.Empty,
                                    stringWalker.GetText(razorKeywordTextSpan),
                                    razorKeywordTextSpan));

                                break;
                            case CSharpDecorationKind.Comment:
                                var razorCommentTextSpan = lexedTokenTextSpan with
                                {
                                    DecorationByte = (byte)HtmlDecorationKind.Comment,
                                    StartingIndexInclusive = startingIndexInclusive,
                                    EndingIndexExclusive = endingIndexExclusive,
                                };

                                injectedLanguageFragmentSyntaxes.Add(new InjectedLanguageFragmentSyntax(
                                    ImmutableArray<IHtmlSyntax>.Empty,
                                    stringWalker.GetText(razorCommentTextSpan),
                                    razorCommentTextSpan));

                                break;
                        }
                    }

                    return true;
                }
            }

            return true;
        });

        if (!foundCodeBlock)
        {
            var expressionBuilder = new StringBuilder();

            stringWalker.WhileNotEndOfFile(() =>
            {
                // There was no matching code block opening syntax.
                // Therefore assume an expression syntax and allow the
                // InjectedLanguageDefinition access to a continually appended to
                // StringBuilder so the InjectedLanguageDefinition can determine
                // when expression ends.
                //
                // (Perhaps it matches a known variable name).
                //
                // (Perhaps there is an unmatched variable name however
                // there is a non valid character in the variable name therefore
                // match what was expected to allow for parsing the remainder of the file).

                expressionBuilder.Append(stringWalker.CurrentCharacter);

                return true;
            });
        }

        return injectedLanguageFragmentSyntaxes;
    }
}