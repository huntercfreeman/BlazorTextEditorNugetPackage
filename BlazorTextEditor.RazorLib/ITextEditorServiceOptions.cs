﻿using BlazorTextEditor.RazorLib.Store.ThemeCase;

namespace BlazorTextEditor.RazorLib;

public interface ITextEditorServiceOptions
{
    /// <summary>
    ///     If the consumer of the Nuget Package is
    ///     registering Fluxor themselves they can include
    ///     typeof(TextEditorOptions).Assembly when invoking
    ///     AddFluxor to add it as a service.
    ///     <br /><br />
    ///     As well the Fluxor.Blazor.Web.StoreInitializer will
    ///     not be rendered from within the Nuget Package
    /// </summary>
    public bool InitializeFluxor { get; }
    /// <summary>
    /// <see cref="ThemeFacts"/> contains themes to choose from.
    /// <br/><br/>
    /// The default theme is <see cref="ThemeFacts.Unset"/>.
    /// The <see cref="ThemeFacts.Unset"/> theme is equivalent to <see cref="ThemeFacts.VisualStudioDarkClone"/>
    /// </summary>
    public Theme InitialTheme { get; set; }
}