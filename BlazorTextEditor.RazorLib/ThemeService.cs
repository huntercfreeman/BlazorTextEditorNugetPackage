﻿using BlazorTextEditor.RazorLib.Store.TextEditorCase;
using BlazorTextEditor.RazorLib.Store.ThemeCase;
using Fluxor;

namespace BlazorTextEditor.RazorLib;

public class ThemeService : IThemeService
{
    private readonly IState<ThemeStates> _themeStates;
    private readonly IDispatcher _dispatcher;

    public ThemeService(
        IState<ThemeStates> themeStates, 
        IDispatcher dispatcher)
    {
        _themeStates = themeStates;
        _dispatcher = dispatcher;

        _themeStates.StateChanged += ThemeStatesOnStateChanged;
    }

    public ThemeStates ThemeStates => _themeStates.Value;
    
    public event EventHandler? OnThemeStatesChanged;

    public void RegisterTheme(Theme theme)
    {
        _dispatcher.Dispatch(new RegisterThemeAction(theme));
    }
    
    public void DisposeTheme(ThemeKey themeKey)
    {
        _dispatcher.Dispatch(new DisposeThemeAction(themeKey));
    }

    private void ThemeStatesOnStateChanged(object? sender, EventArgs e)
    {
        OnThemeStatesChanged?.Invoke(sender, e);
    }
    
    public void Dispose()
    {
        _themeStates.StateChanged -= ThemeStatesOnStateChanged;
    }
}