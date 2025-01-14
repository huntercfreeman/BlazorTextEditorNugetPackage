﻿using System.Collections.Immutable;
using BlazorCommon.RazorLib.Clipboard;
using BlazorCommon.RazorLib.Dialog;
using BlazorCommon.RazorLib.Misc;
using BlazorCommon.RazorLib.WatchWindow;
using BlazorTextEditor.RazorLib.Commands;
using BlazorTextEditor.RazorLib.Commands.Default;
using BlazorTextEditor.RazorLib.Cursor;
using BlazorTextEditor.RazorLib.Lexing;
using BlazorTextEditor.RazorLib.Model;
using BlazorTextEditor.RazorLib.Row;
using BlazorTextEditor.RazorLib.Semantics;
using BlazorTextEditor.RazorLib.ViewModel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorTextEditor.RazorLib.HelperComponents;

public partial class TextEditorHeader : TextEditorView
{
    [Inject]
    private IClipboardService ClipboardService { get; set; } = null!;
    [Inject]
    private IDialogService DialogService { get; set; } = null!;

    [Parameter]
    public ImmutableArray<TextEditorHeaderButtonKind>? HeaderButtonKinds { get; set; }

    private TextEditorCommandParameter ConstructTextEditorCommandParameter(
        TextEditorModel textEditorModel,
        TextEditorViewModel textEditorViewModel)
    {
        return new TextEditorCommandParameter(
            textEditorModel,
            TextEditorCursorSnapshot.TakeSnapshots(textEditorViewModel.PrimaryCursor),
            ClipboardService,
            TextEditorService,
            textEditorViewModel);
    }

    private void SelectRowEndingKindOnChange(ChangeEventArgs changeEventArgs)
    {
        var textEditor = MutableReferenceToModel;
        var localTextEditorViewModel = MutableReferenceToViewModel;

        if (textEditor is null ||
            localTextEditorViewModel is null)
        {
            return;
        }

        var rowEndingKindString = (string)(changeEventArgs.Value ?? string.Empty);

        if (Enum.TryParse<RowEndingKind>(rowEndingKindString, out var rowEndingKind))
            TextEditorService.Model.SetUsingRowEndingKind(
                localTextEditorViewModel.ModelKey,
                rowEndingKind);
    }

    private async Task DoCopyOnClick(MouseEventArgs arg)
    {
        var textEditor = MutableReferenceToModel;
        var textEditorViewModel = MutableReferenceToViewModel;
        
        if (textEditor is null || 
            textEditorViewModel is null)
        {
            return;
        }

        var textEditorCommandParameter = ConstructTextEditorCommandParameter(
            textEditor,
            textEditorViewModel);

        var command = TextEditorCommandDefaultFacts.Copy;
        
        await command.DoAsyncFunc.Invoke(
            textEditorCommandParameter);
    }

    private async Task DoCutOnClick(MouseEventArgs arg)
    {
        var textEditor = MutableReferenceToModel;
        var textEditorViewModel = MutableReferenceToViewModel;
        
        if (textEditor is null || 
            textEditorViewModel is null)
        {
            return;
        }

        var textEditorCommandParameter = ConstructTextEditorCommandParameter(
            textEditor,
            textEditorViewModel);

        var command = TextEditorCommandDefaultFacts.Cut;
        
        await command.DoAsyncFunc.Invoke(
            textEditorCommandParameter);
    }

    private async Task DoPasteOnClick(MouseEventArgs arg)
    {
        var textEditor = MutableReferenceToModel;
                var textEditorViewModel = MutableReferenceToViewModel;
                
        if (textEditor is null || 
            textEditorViewModel is null)
        {
            return;
        }

        var textEditorCommandParameter = ConstructTextEditorCommandParameter(
            textEditor,
            textEditorViewModel);

        var command = TextEditorCommandDefaultFacts.Paste;
        
        await command.DoAsyncFunc.Invoke(
            textEditorCommandParameter);
    }

    private async Task DoRedoOnClick(MouseEventArgs arg)
    {
        var textEditor = MutableReferenceToModel;
                var textEditorViewModel = MutableReferenceToViewModel;
                
        if (textEditor is null || 
            textEditorViewModel is null)
        {
            return;
        }

        var textEditorCommandParameter = ConstructTextEditorCommandParameter(
            textEditor,
            textEditorViewModel);

        var command = TextEditorCommandDefaultFacts.Redo;
        
        await command.DoAsyncFunc.Invoke(
            textEditorCommandParameter);
    }

    private async Task DoSaveOnClick(MouseEventArgs arg)
    {
        var textEditor = MutableReferenceToModel;
                var textEditorViewModel = MutableReferenceToViewModel;
                
        if (textEditor is null || 
            textEditorViewModel is null)
        {
            return;
        }

        var textEditorCommandParameter = ConstructTextEditorCommandParameter(
            textEditor,
            textEditorViewModel);

        var command = TextEditorCommandDefaultFacts.Save;
        
        await command.DoAsyncFunc.Invoke(
            textEditorCommandParameter);
    }

    private async Task DoUndoOnClick(MouseEventArgs arg)
    {
        var textEditor = MutableReferenceToModel;
        var textEditorViewModel = MutableReferenceToViewModel;
        
        if (textEditor is null || 
            textEditorViewModel is null)
        {
            return;
        }

        var textEditorCommandParameter = ConstructTextEditorCommandParameter(
            textEditor,
            textEditorViewModel);

        var command = TextEditorCommandDefaultFacts.Undo;
        
        await command.DoAsyncFunc.Invoke(
            textEditorCommandParameter);
    }

    private async Task DoSelectAllOnClick(MouseEventArgs arg)
    {
        var textEditor = MutableReferenceToModel;
        var textEditorViewModel = MutableReferenceToViewModel;
        
        if (textEditor is null || 
            textEditorViewModel is null)
        {
            return;
        }

        var textEditorCommandParameter = ConstructTextEditorCommandParameter(
            textEditor,
            textEditorViewModel);

        var command = TextEditorCommandDefaultFacts.SelectAll;
        
        await command.DoAsyncFunc.Invoke(
            textEditorCommandParameter);
    }

    private async Task DoRemeasureOnClick(MouseEventArgs arg)
    {
        var textEditor = MutableReferenceToModel;
        var textEditorViewModel = MutableReferenceToViewModel;
        
        if (textEditor is null || 
            textEditorViewModel is null)
        {
            return;
        }

        var textEditorCommandParameter = ConstructTextEditorCommandParameter(
            textEditor,
            textEditorViewModel);

        var command = TextEditorCommandDefaultFacts.Remeasure;
        
        await command.DoAsyncFunc.Invoke(
            textEditorCommandParameter);
    }
    
    private void ShowWatchWindowDisplayDialogOnClick()
    {
        var model = MutableReferenceToModel;
        
        if (model is null)
            return;

        if (model.SemanticModel is not null)
            model.SemanticModel.Parse(model);

        var watchWindowObjectWrap = new WatchWindowObjectWrap(
            model,
            typeof(TextEditorModel),
            "TextEditorModel",
            true);
        
        var dialogRecord = new DialogRecord(
            DialogKey.NewDialogKey(), 
            $"WatchWindow: {model.ResourceUri}",
            typeof(WatchWindowDisplay),
            new Dictionary<string, object?>
            {
                {
                    nameof(WatchWindowDisplay.WatchWindowObjectWrap),
                    watchWindowObjectWrap
                }
            },
            null)
        {
            IsResizable = true
        };
        
        DialogService.RegisterDialogRecord(dialogRecord);

        ChangeLastPresentationLayer();
    }
    
    private async Task DoRefreshOnClick()
    {
        var textEditor = MutableReferenceToModel;
        var textEditorViewModel = MutableReferenceToViewModel;
        
        if (textEditor is null || 
            textEditorViewModel is null)
        {
            return;
        }

        var textEditorCommandParameter = ConstructTextEditorCommandParameter(
            textEditor,
            textEditorViewModel);

        var command = TextEditorCommandDefaultFacts.Remeasure;
        
        await command.DoAsyncFunc.Invoke(
            textEditorCommandParameter);
    }

    /// <summary>
    /// disabled=@GetUndoDisabledAttribute()
    /// will toggle the attribute
    /// <br/><br/>
    /// disabled="@GetUndoDisabledAttribute()"
    /// will toggle the value of the attribute
    /// </summary>
    private bool GetUndoDisabledAttribute()
    {
        var textEditor = MutableReferenceToModel;
        var textEditorViewModel = MutableReferenceToViewModel;
        
        if (textEditor is null || 
            textEditorViewModel is null)
        {
            return true;
        }
        
        return !textEditor.CanUndoEdit();
    }
    
    /// <summary>
    /// disabled=@GetRedoDisabledAttribute()
    /// will toggle the attribute
    /// <br/><br/>
    /// disabled="@GetRedoDisabledAttribute()"
    /// will toggle the value of the attribute
    /// </summary>
    private bool GetRedoDisabledAttribute()
    {
        var textEditor = MutableReferenceToModel;
        var textEditorViewModel = MutableReferenceToViewModel;
        
        if (textEditor is null || 
            textEditorViewModel is null)
        {
            return true;
        }
        
        return !textEditor.CanRedoEdit();
    }

    private void ChangeLastPresentationLayer()
    {
        var viewModel = MutableReferenceToViewModel;

        if (viewModel is null)
            return;

        TextEditorService.ViewModel.With(
            viewModel.ViewModelKey,
            inViewModel =>
            {
                var outPresentationLayer = inViewModel.FirstPresentationLayer;

                var inPresentationModel = outPresentationLayer
                    .FirstOrDefault(x =>
                        x.TextEditorPresentationKey == SemanticFacts.PresentationKey);

                if (inPresentationModel is null)
                {
                    inPresentationModel = SemanticFacts.EmptyPresentationModel;

                    outPresentationLayer = outPresentationLayer.Add(
                        inPresentationModel);
                }

                var model = TextEditorService.ViewModel
                    .FindBackingModelOrDefault(viewModel.ViewModelKey);

                var outPresentationModel = inPresentationModel with
                {
                    TextEditorTextSpans = model?.SemanticModel?.DiagnosticTextSpans 
                        ?? ImmutableList<TextEditorTextSpan>.Empty
                };

                outPresentationLayer = outPresentationLayer.Replace(
                    inPresentationModel,
                    outPresentationModel);

                return inViewModel with
                {
                    FirstPresentationLayer = outPresentationLayer,
                    RenderStateKey = RenderStateKey.NewRenderStateKey()
                };
            });
    }
}