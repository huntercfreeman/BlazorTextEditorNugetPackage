﻿using BlazorTextEditor.RazorLib.Store.ThemeCase;

namespace BlazorTextEditor.RazorLib.TextEditor;

/// <summary>
/// Any property on <see cref="TextEditorServiceOptions"/> will be equal to
/// the 
/// </summary>
public record TextEditorOptions(
    int? FontSizeInPixels,
    Theme? Theme)
{
    public static TextEditorOptions UnsetTextEditorOptions()
    {
        return new(
            null, 
            null);
    }
}