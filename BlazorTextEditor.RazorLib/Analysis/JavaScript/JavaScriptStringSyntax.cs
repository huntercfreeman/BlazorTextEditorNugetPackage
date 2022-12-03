﻿using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Analysis.JavaScript;

public class JavaScriptStringSyntax : IJavaScriptSyntax
{
    public TextEditorTextSpan TextEditorTextSpan { get; }
    public ImmutableArray<IJavaScriptSyntax> Children => ImmutableArray<IJavaScriptSyntax>.Empty;
    public JavaScriptSyntaxKind JavaScriptSyntaxKind => JavaScriptSyntaxKind.String;
}