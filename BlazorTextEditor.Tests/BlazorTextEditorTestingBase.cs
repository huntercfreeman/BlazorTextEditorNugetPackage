﻿using BlazorALaCarte.Shared.Clipboard;
using BlazorALaCarte.Shared.Storage;
using BlazorTextEditor.RazorLib;
using BlazorTextEditor.RazorLib.Model;
using Fluxor;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorTextEditor.Tests;

/// <summary>
/// Setup the dependency injection necessary
/// </summary>
public class BlazorTextEditorTestingBase
{
    protected readonly ServiceProvider ServiceProvider;
    protected readonly ITextEditorService TextEditorService;
    protected readonly TextEditorModelKey TextEditorModelKey = TextEditorModelKey.NewTextEditorModelKey();

    protected TextEditorModel TextEditorModel => TextEditorService
        .ModelFindOrDefault(TextEditorModelKey)
            ?? throw new ApplicationException(
                $"{nameof(TextEditorService)}" +
                $".{nameof(TextEditorService.ModelFindOrDefault)}" +
                " returned null.");

    public BlazorTextEditorTestingBase()
    {
        var services = new ServiceCollection();

        services.AddBlazorTextEditor(options =>
        {
            options.InitializeFluxor = false;
            
            options.ClipboardProviderFactory = _ => 
                new InMemoryClipboardProvider();
            
            options.StorageProviderFactory = _ =>
                new DoNothingStorageService();
        });
        
        services
            .AddFluxor(options => options
                .ScanAssemblies(
                    typeof(RazorLib.ServiceCollectionExtensions)
                        .Assembly));

        ServiceProvider = services.BuildServiceProvider();

        var store = ServiceProvider.GetRequiredService<IStore>();

        store.InitializeAsync().Wait();

        TextEditorService = ServiceProvider
            .GetRequiredService<ITextEditorService>();

        var textEditor = new TextEditorModel(
            nameof(BlazorTextEditorTestingBase),
            DateTime.UtcNow,
            "UnitTests",
            string.Empty,
            null,
            null,
            null,
            TextEditorModelKey);
        
        TextEditorService.ModelRegisterCustomModel(textEditor);
    }
}