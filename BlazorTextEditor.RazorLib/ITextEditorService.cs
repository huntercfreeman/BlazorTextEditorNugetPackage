﻿using BlazorTextEditor.RazorLib.Row;
using BlazorTextEditor.RazorLib.Store.TextEditorCase;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.Actions;
using BlazorTextEditor.RazorLib.Store.ThemeCase;
using BlazorTextEditor.RazorLib.TextEditor;

namespace BlazorTextEditor.RazorLib;

public interface ITextEditorService : IDisposable
{
    public static string LocalStorageGlobalTextEditorOptionsKey { get; } = "bte_text-editor-options";
    
    public TextEditorStates TextEditorStates { get; }

    public string GlobalThemeCssClassString { get; }
    public Theme? GlobalThemeValue { get; }
    public string GlobalFontSizeInPixelsStyling { get; }
    public int GlobalFontSizeInPixelsValue { get; }
    public bool GlobalShowNewlines { get; }
    public bool GlobalShowWhitespace { get; }

    public event Action? OnTextEditorStatesChanged;

    public void RegisterTextEditor(TextEditorBase textEditorBase);
    public void InsertText(InsertTextTextEditorBaseAction insertTextTextEditorBaseAction);
    public void HandleKeyboardEvent(KeyboardEventTextEditorBaseAction keyboardEventTextEditorBaseAction);
    public void DeleteTextByMotion(DeleteTextByMotionTextEditorBaseAction deleteTextByMotionTextEditorBaseAction);
    public void DeleteTextByRange(DeleteTextByRangeTextEditorBaseAction deleteTextByRangeTextEditorBaseAction);
    public void DisposeTextEditor(TextEditorKey textEditorKey);
    public void SetFontSize(int fontSizeInPixels);
    public void SetTheme(Theme theme);
    public void SetShowWhitespace(bool showWhitespace);
    public void SetShowNewlines(bool showNewlines);
    public void SetUsingRowEndingKind(TextEditorKey textEditorKey, RowEndingKind rowEndingKind);
    public void ShowSettingsDialog();
    
    public Task SetTextEditorOptionsFromLocalStorageAsync();
    public void WriteGlobalTextEditorOptionsToLocalStorage();
}