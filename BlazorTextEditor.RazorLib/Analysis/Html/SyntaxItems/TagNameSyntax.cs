﻿using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Analysis.Html.SyntaxItems;

public class TagNameSyntax : IHtmlSyntax
{
    public TagNameSyntax(
        string value,
        TextEditorTextSpan textEditorTextSpan)
    {
        Value = value;
        TextEditorTextSpan = textEditorTextSpan;
    }

    public string Value { get; }
    public TextEditorTextSpan TextEditorTextSpan { get; }
    public HtmlSyntaxKind HtmlSyntaxKind => HtmlSyntaxKind.TagName;
    public ImmutableArray<IHtmlSyntax> ChildHtmlSyntaxes { get; } = ImmutableArray<IHtmlSyntax>.Empty;
}