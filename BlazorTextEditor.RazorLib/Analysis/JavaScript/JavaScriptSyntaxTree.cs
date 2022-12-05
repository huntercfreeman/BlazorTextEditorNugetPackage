﻿using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Analysis.Json.SyntaxItems;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Analysis.JavaScript;

public class JavaScriptSyntaxTree
{
    public static JavaScriptSyntaxUnit ParseText(string content)
    {
        var documentChildren = new List<IJavaScriptSyntax>();
        var diagnosticBag = new TextEditorDiagnosticBag();
        
        var stringWalker = new StringWalker(content);

        while (!stringWalker.IsEof)
        {
            if (stringWalker.CurrentCharacter == JavaScriptFacts.STRING_STARTING_CHARACTER)
            {
                var javaScriptStringSyntax = ReadString(stringWalker, diagnosticBag);
                
                documentChildren.Add(javaScriptStringSyntax);
            }
            else if (stringWalker.CheckForSubstring(JavaScriptFacts.COMMENT_SINGLE_LINE_START))
            {
                var javaScriptCommentSyntax = ReadCommentSingleLine(stringWalker, diagnosticBag);
                
                documentChildren.Add(javaScriptCommentSyntax);
            }
            else if (stringWalker.CheckForSubstring(JavaScriptFacts.COMMENT_MULTI_LINE_START))
            {
                var javaScriptCommentSyntax = ReadCommentMultiLine(stringWalker, diagnosticBag);
                
                documentChildren.Add(javaScriptCommentSyntax);
            }
            
            /*
             * comment:
             *     if (currentCharacter == )
             *         if (nextCharacter == '/')
             *             var javaScriptSingleLineCommentSyntax = readSingleLineComment();
             *             documentChildren.Add(javaScriptSingleLineCommentSyntax);
             *         else if (nextCharacter == '*')
             *            var javaScriptMultiLineCommentSyntax = readMultiLineComment();
             *            documentChildren.Add(javaScriptMultiLineCommentSyntax);
             * keyword:
             *     if (listOfKeywords.Contains(nextWord))
             *         var javaScriptKeywordSyntax = readKeyword();
             *         documentChildren.Add(javaScriptKeywordSyntax);
             */
            
            _ = stringWalker.Consume();
        }

        var javaScriptDocumentSyntax = new JavaScriptDocumentSyntax(
            new TextEditorTextSpan(
                0,
                stringWalker.PositionIndex,
                (byte)JavaScriptDecorationKind.None),
            documentChildren.ToImmutableArray());
        
        return new JavaScriptSyntaxUnit(
            javaScriptDocumentSyntax,
            diagnosticBag);
    }

    /// <summary>
    /// currentCharacterIn:<br/>
    /// -<see cref="JavaScriptFacts.STRING_STARTING_CHARACTER"/>
    /// </summary>
    private static JavaScriptStringSyntax ReadString(
        StringWalker stringWalker,
        TextEditorDiagnosticBag diagnosticBag)
    {
        var startingPositionIndex = stringWalker.PositionIndex;

        while (!stringWalker.IsEof)
        {
            _ = stringWalker.Consume();

            if (stringWalker.CurrentCharacter == JavaScriptFacts.STRING_ENDING_CHARACTER)
                break;
        }

        if (stringWalker.IsEof)
        {
            diagnosticBag.ReportEndOfFileUnexpected(
                new TextEditorTextSpan(
                    startingPositionIndex,
                    stringWalker.PositionIndex,
                    (byte)JavaScriptDecorationKind.Error));
        }

        var stringTextEditorTextSpan = new TextEditorTextSpan(
            startingPositionIndex,
            stringWalker.PositionIndex + 1,
            (byte)JavaScriptDecorationKind.String);
        
        return new JavaScriptStringSyntax(
            stringTextEditorTextSpan);
    }
    
    /// <summary>
    /// currentCharacterIn:<br/>
    /// -<see cref="JavaScriptFacts.COMMENT_SINGLE_LINE_START"/>
    /// </summary>
    private static JavaScriptStringSyntax ReadCommentSingleLine(
        StringWalker stringWalker,
        TextEditorDiagnosticBag diagnosticBag)
    {
        var startingPositionIndex = stringWalker.PositionIndex;

        while (!stringWalker.IsEof)
        {
            _ = stringWalker.Consume();

            if (JavaScriptFacts.COMMENT_SINGLE_LINE_ENDINGS.Contains(stringWalker.CurrentCharacter))
                break;
        }

        if (stringWalker.IsEof)
        {
            diagnosticBag.ReportEndOfFileUnexpected(
                new TextEditorTextSpan(
                    startingPositionIndex,
                    stringWalker.PositionIndex,
                    (byte)JavaScriptDecorationKind.Error));
        }

        var commentTextEditorTextSpan = new TextEditorTextSpan(
            startingPositionIndex,
            stringWalker.PositionIndex,
            (byte)JavaScriptDecorationKind.Comment);
        
        return new JavaScriptStringSyntax(
            commentTextEditorTextSpan);
    }
    
    /// <summary>
    /// currentCharacterIn:<br/>
    /// -<see cref="JavaScriptFacts.COMMENT_MULTI_LINE_START"/>
    /// </summary>
    private static JavaScriptStringSyntax ReadCommentMultiLine(
        StringWalker stringWalker,
        TextEditorDiagnosticBag diagnosticBag)
    {
        var startingPositionIndex = stringWalker.PositionIndex;

        while (!stringWalker.IsEof)
        {
            _ = stringWalker.Consume();

            if (stringWalker.CheckForSubstring(JavaScriptFacts.COMMENT_MULTI_LINE_END))
                break;
        }

        if (stringWalker.IsEof)
        {
            diagnosticBag.ReportEndOfFileUnexpected(
                new TextEditorTextSpan(
                    startingPositionIndex,
                    stringWalker.PositionIndex,
                    (byte)JavaScriptDecorationKind.Error));
        }

        var commentTextEditorTextSpan = new TextEditorTextSpan(
            startingPositionIndex,
            stringWalker.PositionIndex + JavaScriptFacts.COMMENT_MULTI_LINE_END.Length,
            (byte)JavaScriptDecorationKind.Comment);
        
        return new JavaScriptStringSyntax(
            commentTextEditorTextSpan);
    }
}