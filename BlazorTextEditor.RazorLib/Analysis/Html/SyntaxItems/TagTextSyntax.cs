﻿using System.Collections.Immutable;

namespace BlazorTextEditor.RazorLib.Analysis.Html.SyntaxItems;

public class TagTextSyntax : TagSyntax
{
    public TagTextSyntax(
        ImmutableArray<AttributeTupleSyntax> attributeTupleSyntaxes,
        ImmutableArray<TagSyntax> childTagSyntaxes,
        string value,
        bool hasSpecialHtmlCharacter = false)
        : base(
            null,
            null,
            attributeTupleSyntaxes,
            childTagSyntaxes,
            TagKind.Text,
            hasSpecialHtmlCharacter)
    {
        Value = value;
    }

    public string Value { get; }
}