﻿using BlazorTextEditor.RazorLib.Group;
using BlazorTextEditor.RazorLib.ViewModel;

namespace BlazorTextEditor.RazorLib.Store.TextEditorCase.Group;

public partial class TextEditorGroupsCollection
{
    public record AddViewModelToGroupAction(
        TextEditorGroupKey TextEditorGroupKey,
        TextEditorViewModelKey TextEditorViewModelKey);

    public record RegisterGroupAction(
        TextEditorGroup TextEditorGroup);

    public record RemoveViewModelFromGroupAction(
        TextEditorGroupKey TextEditorGroupKey,
        TextEditorViewModelKey TextEditorViewModelKey);

    public record SetActiveViewModelOfGroupAction(
        TextEditorGroupKey TextEditorGroupKey,
        TextEditorViewModelKey TextEditorViewModelKey);
}