﻿using BlazorTextEditor.RazorLib.Model;
using BlazorTextEditor.RazorLib.Store.Model;
using BlazorTextEditor.RazorLib.Store.Options;
using BlazorTextEditor.RazorLib.Store.ViewModel;
using BlazorTextEditor.RazorLib.ViewModel;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib;

/// <summary>
/// <see cref="TextEditorView"/> is the
/// message broker abstraction between a
/// Blazor component and a <see cref="TextEditorModel"/>
/// </summary>
public class TextEditorView : FluxorComponent
{
    // TODO: Do not rerender so much too many things are touched by the [Inject] in this file
    //
    // [Inject]
    // protected IStateSelection<TextEditorModelsCollection, TextEditorModel?> TextEditorModelsCollectionSelection { get; set; } = null!;
    [Inject]
    protected IState<TextEditorModelsCollection> TextEditorModelsCollectionWrap { get; set; } = null!;
    [Inject]
    protected IState<TextEditorViewModelsCollection> TextEditorViewModelsCollectionWrap { get; set; } = null!;
    [Inject]
    protected IState<TextEditorOptionsState> TextEditorGlobalOptionsWrap { get; set; } = null!;
    [Inject]
    protected ITextEditorService TextEditorService { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public TextEditorViewModelKey TextEditorViewModelKey { get; set; } = null!;
    
    public TextEditorModel? MutableReferenceToModel => TextEditorService.ViewModel
        .FindBackingModelOrDefault(TextEditorViewModelKey);
    
    public TextEditorViewModel? MutableReferenceToViewModel => TextEditorViewModelsCollectionWrap.Value.ViewModelsList
        .FirstOrDefault(x => 
            x.ViewModelKey == TextEditorViewModelKey);
}