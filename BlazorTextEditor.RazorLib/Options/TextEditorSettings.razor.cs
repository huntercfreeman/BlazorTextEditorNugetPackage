﻿using BlazorTextEditor.RazorLib.Autocomplete;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.Options;

public partial class TextEditorSettings : FluxorComponent
{
    [Inject]
    private IAutocompleteIndexer AutocompleteIndexer { get; set; } = null!;

    [Parameter]
    public string InputElementCssClass { get; set; } = string.Empty;
}