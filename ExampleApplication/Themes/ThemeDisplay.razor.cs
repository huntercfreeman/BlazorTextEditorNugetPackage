﻿using BlazorTextEditor.RazorLib.Store.ThemeCase;
using Microsoft.AspNetCore.Components;

namespace ExampleApplication.Themes;

public partial class ThemeDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public Theme Theme { get; set; } = null!;
}