﻿using System.Collections.Immutable;
using System.Text;
using BlazorTextEditor.RazorLib.Analysis.Css.SyntaxItems;
using BlazorTextEditor.RazorLib.Keyboard;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Analysis.Css.SyntaxActors;

public class CssSyntaxTree
{
    public static CssSyntaxUnit ParseText(string content)
    {
        // Items to return wrapped in a CssSyntaxUnit
        var cssDocumentChildren = new List<ICssSyntax>();
        var textEditorCssDiagnosticBag = new TextEditorCssDiagnosticBag();

        // Step through the string 'character by character'
        var stringWalker = new StringWalker(content);
        
        while (!stringWalker.IsEof)
        {
            if (stringWalker.CheckForSubstring(CssFacts.COMMENT_START))
                ConsumeComment(stringWalker, cssDocumentChildren, textEditorCssDiagnosticBag);

            if (stringWalker.CurrentCharacter == CssFacts.STYLE_BLOCK_START)
                ConsumeStyleBlock(stringWalker, cssDocumentChildren, textEditorCssDiagnosticBag);

            _ = stringWalker.Consume();
        }

        var cssDocumentSyntax = new CssDocumentSyntax(
            new TextEditorTextSpan(
                0,
                stringWalker.PositionIndex,
                (byte)CssDecorationKind.None),
            cssDocumentChildren.ToImmutableArray());

        var cssSyntaxUnit = new CssSyntaxUnit(
            cssDocumentSyntax,
            textEditorCssDiagnosticBag);

        return cssSyntaxUnit;
    }

    private static void ConsumeComment(
        StringWalker stringWalker, 
        List<ICssSyntax> cssDocumentChildren,
        TextEditorCssDiagnosticBag textEditorCssDiagnosticBag)
    {
        var commentStartingPositionIndex = stringWalker.PositionIndex;

        while (!stringWalker.IsEof)
        {
            var closingOfCommentTextFound = stringWalker
                .CheckForSubstring(CssFacts.COMMENT_END);

            if (closingOfCommentTextFound)
            {
                // Skip the rest of the comment closing text
                _ = stringWalker.ConsumeRange(CssFacts.COMMENT_END.Length - 1);

                var commentTextSpan = new TextEditorTextSpan(
                    commentStartingPositionIndex,
                    stringWalker.PositionIndex + 1,
                    (byte)CssDecorationKind.Comment);

                var commentToken = new CssCommentSyntax(
                    commentTextSpan,
                    ImmutableArray<ICssSyntax>.Empty);

                cssDocumentChildren.Add(commentToken);

                return;
            }
        }
    }
    
    /// <summary>
    /// Assumes invoker found <see cref="CssFacts.STYLE_BLOCK_START"/>
    /// and did not <see cref="StringWalker.Consume"/>
    /// to increment the position index.
    /// <br/><br/>
    /// In other words the first action this method takes is
    /// <see cref="StringWalker.Consume"/> to increment
    /// the position index.
    /// </summary>
    private static void ConsumeStyleBlock(
        StringWalker stringWalker, 
        List<ICssSyntax> cssDocumentChildren,
        TextEditorCssDiagnosticBag textEditorCssDiagnosticBag)
    {
        var expectedStyleBlockChild = CssSyntaxKind.PropertyName;
        
        // when pendingChildStartingPositionIndex == -1 it is to
        // mean that there is NOT a pending child
        var pendingChildStartingPositionIndex = -1;
        
        while (!stringWalker.IsEof)
        {
            _ = stringWalker.Consume();
            
            if (stringWalker.CheckForSubstring(CssFacts.COMMENT_START))
                ConsumeComment(stringWalker, cssDocumentChildren, textEditorCssDiagnosticBag);

            char childEndingCharacter;
            CssDecorationKind childDecorationKind;
            
            switch (expectedStyleBlockChild)
            {
                case CssSyntaxKind.PropertyName:
                    childEndingCharacter = CssFacts.PROPERTY_NAME_END;
                    childDecorationKind = CssDecorationKind.PropertyName;
                    break;
                case CssSyntaxKind.PropertyValue:
                    childEndingCharacter = CssFacts.PROPERTY_VALUE_END;
                    childDecorationKind = CssDecorationKind.PropertyValue;
                    break;
                default:
                    throw new ApplicationException($"The {nameof(CssSyntaxKind)} of" +
                                                   $" {expectedStyleBlockChild} was unexpected.");
            }
            
            // Skip preceding and trailing whitespace
            // relative to the child's text
            if (WhitespaceFacts.ALL.Contains(stringWalker.CurrentCharacter) &&
                pendingChildStartingPositionIndex == -1)
            {
                continue;
            }
            
            // Start of a child's text
            if (pendingChildStartingPositionIndex == -1)
            {
                pendingChildStartingPositionIndex = stringWalker.PositionIndex;
                continue;
            }
            
            // End of a child's text
            if (stringWalker.CurrentCharacter == childEndingCharacter ||
                WhitespaceFacts.ALL.Contains(stringWalker.CurrentCharacter))
            {   
                var childTextSpan = new TextEditorTextSpan(
                    pendingChildStartingPositionIndex,
                    stringWalker.PositionIndex,
                    (byte)childDecorationKind);

                ICssSyntax childSyntax;
                
                switch (expectedStyleBlockChild)
                {
                    case CssSyntaxKind.PropertyName:
                        childSyntax = new CssPropertyNameSyntax(
                            childTextSpan,
                            ImmutableArray<ICssSyntax>.Empty);
                        break;
                    case CssSyntaxKind.PropertyValue:
                        childSyntax = new CssPropertyValueSyntax(
                            childTextSpan,
                            ImmutableArray<ICssSyntax>.Empty);
                        break;
                    default:
                        throw new ApplicationException($"The {nameof(CssSyntaxKind)} of" +
                                                       $" {expectedStyleBlockChild} was unexpected.");
                }
                
                cssDocumentChildren.Add(childSyntax);
                
                if (stringWalker.CurrentCharacter == childEndingCharacter)
                {
                    switch (expectedStyleBlockChild)
                    {
                        case CssSyntaxKind.PropertyName:
                            expectedStyleBlockChild = CssSyntaxKind.PropertyValue;
                            break;
                        case CssSyntaxKind.PropertyValue:
                            expectedStyleBlockChild = CssSyntaxKind.PropertyName;
                            break;
                        default:
                            throw new ApplicationException($"The {nameof(CssSyntaxKind)} of" +
                                                           $" {expectedStyleBlockChild} was unexpected.");
                    }
                }

                continue;
            }
            
            // Relies on the if statement before this that ensures
            // the current character is not whitespace
            if (stringWalker.CurrentCharacter != childEndingCharacter)
            {
                var unexpectedTokenTextSpan = new TextEditorTextSpan(
                    pendingChildStartingPositionIndex,
                    stringWalker.PositionIndex,
                    (byte)CssDecorationKind.UnexpectedToken);
                
                textEditorCssDiagnosticBag.ReportUnexpectedToken(
                    unexpectedTokenTextSpan,
                    stringWalker.CurrentCharacter.ToString());

                continue;
            }
        }
    }
}