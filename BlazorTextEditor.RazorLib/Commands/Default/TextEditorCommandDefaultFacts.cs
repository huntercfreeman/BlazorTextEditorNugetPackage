﻿using System.Collections.Immutable;
using BlazorCommon.RazorLib.Keyboard;
using BlazorTextEditor.RazorLib.Character;
using BlazorTextEditor.RazorLib.Cursor;
using BlazorTextEditor.RazorLib.Editing;
using BlazorTextEditor.RazorLib.Model;
using BlazorTextEditor.RazorLib.Store.Misc;
using BlazorTextEditor.RazorLib.Store.Model;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorTextEditor.RazorLib.Commands.Default;

public static class TextEditorCommandDefaultFacts
{
    public static readonly TextEditorCommand DoNothingDiscard = new(
        _ => Task.CompletedTask,
        false,
        "DoNothingDiscard",
        "defaults_do-nothing-discard");

    public static readonly TextEditorCommand Copy = new(
        async textEditorCommandParameter =>
        {
            var selectedText = TextEditorSelectionHelper
                .GetSelectedText(
                    textEditorCommandParameter
                        .PrimaryCursorSnapshot
                        .ImmutableCursor
                        .ImmutableTextEditorSelection,
                    textEditorCommandParameter.TextEditorModel);

            if (selectedText is null)
            {
                selectedText = textEditorCommandParameter.TextEditorModel.GetLinesRange(
                    textEditorCommandParameter.PrimaryCursorSnapshot.ImmutableCursor.RowIndex,
                    1);
            }
            
            await textEditorCommandParameter
                .ClipboardService
                .SetClipboard(
                    selectedText);

            await textEditorCommandParameter.TextEditorViewModel.FocusTextEditorAsync();
        },
        false,
        "Copy",
        "defaults_copy");
    
    public static readonly TextEditorCommand Cut = new(
        async textEditorCommandParameter =>
        {
            var selectedText = TextEditorSelectionHelper
                .GetSelectedText(
                    textEditorCommandParameter
                        .PrimaryCursorSnapshot
                        .ImmutableCursor
                        .ImmutableTextEditorSelection,
                    textEditorCommandParameter.TextEditorModel);

            var textEditorCursorSnapshots = textEditorCommandParameter.CursorSnapshots;
            
            if (selectedText is null)
            {
                var textEditorCursor = TextEditorSelectionHelper.SelectLinesRange(
                    textEditorCommandParameter.TextEditorModel,
                    textEditorCommandParameter.PrimaryCursorSnapshot.ImmutableCursor.RowIndex,
                    1);

                textEditorCursorSnapshots = TextEditorCursorSnapshot
                    .TakeSnapshots(textEditorCursor);

                var primaryCursorSnapshot = textEditorCursorSnapshots.FirstOrDefault();

                if (primaryCursorSnapshot is null)
                    return; // Should never occur
                
                selectedText = TextEditorSelectionHelper
                    .GetSelectedText(
                        primaryCursorSnapshot.ImmutableCursor.ImmutableTextEditorSelection,
                        textEditorCommandParameter.TextEditorModel);
            }

            if (selectedText is null)
                return; // Should never occur
            
            await textEditorCommandParameter
                .ClipboardService
                .SetClipboard(
                    selectedText);

            await textEditorCommandParameter.TextEditorViewModel.FocusTextEditorAsync();
            
            textEditorCommandParameter.TextEditorService
                .ModelHandleKeyboardEvent(new TextEditorModelsCollection.KeyboardEventAction(
                    textEditorCommandParameter.TextEditorModel.ModelKey,
                    textEditorCursorSnapshots,
                    new KeyboardEventArgs
                    {
                        Key = KeyboardKeyFacts.MetaKeys.DELETE
                    },
                    CancellationToken.None));
        },
        true,
        "Cut",
        "defaults_cut");

    public static readonly TextEditorCommand Paste = new(
        async textEditorCommandParameter =>
        {
            var clipboard = await textEditorCommandParameter
                .ClipboardService
                .ReadClipboard();

            textEditorCommandParameter.TextEditorService.ModelInsertText(
                new TextEditorModelsCollection.InsertTextAction(
                    textEditorCommandParameter.TextEditorModel.ModelKey,
                    new[]
                    {
                        textEditorCommandParameter.PrimaryCursorSnapshot,
                    }.ToImmutableArray(),
                    clipboard,
                    CancellationToken.None));
        },
        true,
        "Paste",
        "defaults_paste",
        TextEditKind.Other,
        "defaults_paste");

    public static readonly TextEditorCommand Save = new(textEditorCommandParameter =>
        {
            var onSaveRequestedFunc = textEditorCommandParameter
                .TextEditorViewModel
                .OnSaveRequested; 
            
            if (onSaveRequestedFunc is not null)
            {
                onSaveRequestedFunc
                    .Invoke(textEditorCommandParameter.TextEditorModel);
                
                textEditorCommandParameter.TextEditorService.ViewModelWith(
                    textEditorCommandParameter.TextEditorViewModel.ViewModelKey,
                    previousViewModel => previousViewModel with
                    {
                        TextEditorRenderStateKey = TextEditorRenderStateKey.NewTextEditorRenderStateKey()
                    });
            }
            
            return Task.CompletedTask;
        },
        false,
        "Save",
        "defaults_save");

    public static readonly TextEditorCommand SelectAll = new(textEditorCommandParameter =>
        {
            var primaryCursor = textEditorCommandParameter
                .PrimaryCursorSnapshot.UserCursor;

            primaryCursor.TextEditorSelection.AnchorPositionIndex =
                0;

            primaryCursor.TextEditorSelection.EndingPositionIndex =
                textEditorCommandParameter.TextEditorModel.DocumentLength;
            
            textEditorCommandParameter.TextEditorService.ViewModelWith(
                textEditorCommandParameter.TextEditorViewModel.ViewModelKey,
                previousViewModel => previousViewModel with
                {
                    TextEditorRenderStateKey = TextEditorRenderStateKey.NewTextEditorRenderStateKey()
                });
            
            return Task.CompletedTask;
        },
        false,
        "Select All",
        "defaults_select-all");
    
    public static readonly TextEditorCommand Undo = new(textEditorCommandParameter =>
        {
            textEditorCommandParameter.TextEditorService
                .ModelUndoEdit(textEditorCommandParameter.TextEditorModel.ModelKey);
            
            return Task.CompletedTask;
        },
        true,
        "Undo",
        "defaults_undo");
    
    public static readonly TextEditorCommand Redo = new(textEditorCommandParameter =>
        {
            textEditorCommandParameter.TextEditorService
                .ModelRedoEdit(textEditorCommandParameter.TextEditorModel.ModelKey);

            return Task.CompletedTask;
        },
        true,
        "Redo",
        "defaults_redo");
    
    public static readonly TextEditorCommand Remeasure = new(textEditorCommandParameter =>
        {
            textEditorCommandParameter.TextEditorService.ViewModelWith(
                textEditorCommandParameter.TextEditorViewModel.ViewModelKey,
                previousViewModel => previousViewModel with
                {
                    ShouldMeasureDimensions = true,
                    TextEditorRenderStateKey = TextEditorRenderStateKey.NewTextEditorRenderStateKey()
                });
            
            textEditorCommandParameter.TextEditorService.ViewModelWith(
                textEditorCommandParameter.TextEditorViewModel.ViewModelKey,
                previousViewModel => previousViewModel with
                {
                    TextEditorRenderStateKey = TextEditorRenderStateKey.NewTextEditorRenderStateKey()
                });
            
            return Task.CompletedTask;
        },
        false,
        "Remeasure",
        "defaults_remeasure");
    
    public static readonly TextEditorCommand ScrollLineDown = new(async textEditorCommandParameter =>
        {
            await textEditorCommandParameter.TextEditorViewModel
                .MutateScrollVerticalPositionByLinesAsync(1);
        },
        false,
        "Scroll Line Down",
        "defaults_scroll-line-down");
    
    public static readonly TextEditorCommand ScrollLineUp = new(async textEditorCommandParameter =>
        {
            await textEditorCommandParameter.TextEditorViewModel
                .MutateScrollVerticalPositionByLinesAsync(-1);
        },
        false,
        "Scroll Line Up",
        "defaults_scroll-line-up");
    
    public static readonly TextEditorCommand ScrollPageDown = new(async textEditorCommandParameter =>
        {
            await textEditorCommandParameter.TextEditorViewModel
                .MutateScrollVerticalPositionByPagesAsync(1);
        },
        false,
        "Scroll Page Down",
        "defaults_scroll-page-down");
    
    public static readonly TextEditorCommand ScrollPageUp = new(async textEditorCommandParameter =>
        {
            await textEditorCommandParameter.TextEditorViewModel
                .MutateScrollVerticalPositionByPagesAsync(-1);
        },
        false,
        "Scroll Page Up",
        "defaults_scroll-page-up");
    
    public static readonly TextEditorCommand CursorMovePageBottom = new(textEditorCommandParameter =>
        {
            textEditorCommandParameter.TextEditorViewModel
                .CursorMovePageBottom();
            return Task.CompletedTask;
        },
        false,
        "Move Cursor to Bottom of the Page",
        "defaults_cursor-move-page-bottom");
    
    public static readonly TextEditorCommand CursorMovePageTop = new(textEditorCommandParameter =>
        {
            textEditorCommandParameter.TextEditorViewModel
                .CursorMovePageTop();
            return Task.CompletedTask;
        },
        false,
        "Move Cursor to Top of the Page",
        "defaults_cursor-move-page-top");
    
    public static readonly TextEditorCommand Duplicate = new(textEditorCommandParameter =>
        {
            var selectedText = TextEditorSelectionHelper
                .GetSelectedText(
                    textEditorCommandParameter
                        .PrimaryCursorSnapshot
                        .ImmutableCursor
                        .ImmutableTextEditorSelection,
                    textEditorCommandParameter.TextEditorModel);

            TextEditorCursor cursorForInsertion;

            if (selectedText is null)
            {
                selectedText = textEditorCommandParameter.TextEditorModel.GetLinesRange(
                        textEditorCommandParameter.PrimaryCursorSnapshot.ImmutableCursor.RowIndex,
                        1);
                
                cursorForInsertion = new TextEditorCursor(
                    (textEditorCommandParameter.PrimaryCursorSnapshot.ImmutableCursor.RowIndex,
                        0),
                    textEditorCommandParameter.PrimaryCursorSnapshot.UserCursor.IsPrimaryCursor);
            }
            else
            {
                // Clone the TextEditorCursor to remove the TextEditorSelection otherwise the
                // selected text to duplicate would be overwritten by itself and do nothing
                cursorForInsertion = new TextEditorCursor(
                    (textEditorCommandParameter.PrimaryCursorSnapshot.ImmutableCursor.RowIndex,
                        textEditorCommandParameter.PrimaryCursorSnapshot.ImmutableCursor.ColumnIndex),
                    textEditorCommandParameter.PrimaryCursorSnapshot.UserCursor.IsPrimaryCursor);
            }
            
            var insertTextTextEditorModelAction = new TextEditorModelsCollection.InsertTextAction(
                textEditorCommandParameter.TextEditorModel.ModelKey,
                TextEditorCursorSnapshot.TakeSnapshots(cursorForInsertion),
                selectedText,
                CancellationToken.None);
                
            textEditorCommandParameter
                .TextEditorService
                .ModelInsertText(insertTextTextEditorModelAction);
            return Task.CompletedTask;
        },
        false,
        "Duplicate",
        "defaults_duplicate");
    
    public static readonly TextEditorCommand IndentMore = new(textEditorCommandParameter =>
        {
            var selectionBoundsInPositionIndexUnits = TextEditorSelectionHelper
                .GetSelectionBounds(
                    textEditorCommandParameter
                        .PrimaryCursorSnapshot
                        .ImmutableCursor
                        .ImmutableTextEditorSelection);

            var selectionBoundsInRowIndexUnits = TextEditorSelectionHelper
                .ConvertSelectionOfPositionIndexUnitsToRowIndexUnits(
                    textEditorCommandParameter.TextEditorModel,
                    selectionBoundsInPositionIndexUnits);

            for (var i = selectionBoundsInRowIndexUnits.lowerRowIndexInclusive;
                 i < selectionBoundsInRowIndexUnits.upperRowIndexExclusive;
                 i++)
            {
                var cursorForInsertion = new TextEditorCursor(
                    (i, 0),
                    true);
                
                var insertTextTextEditorModelAction = new TextEditorModelsCollection.InsertTextAction(
                    textEditorCommandParameter.TextEditorModel.ModelKey,
                    TextEditorCursorSnapshot.TakeSnapshots(cursorForInsertion),
                    KeyboardKeyFacts.WhitespaceCharacters.TAB.ToString(),
                    CancellationToken.None);
                
                textEditorCommandParameter
                    .TextEditorService
                    .ModelInsertText(insertTextTextEditorModelAction);
            }

            var lowerBoundPositionIndexChange = 1;
            var upperBoundPositionIndexChange = selectionBoundsInRowIndexUnits.upperRowIndexExclusive -
                                                selectionBoundsInRowIndexUnits.lowerRowIndexInclusive;
            
            if (textEditorCommandParameter.PrimaryCursorSnapshot.UserCursor.TextEditorSelection.AnchorPositionIndex <
                textEditorCommandParameter.PrimaryCursorSnapshot.UserCursor.TextEditorSelection.EndingPositionIndex)
            {
                textEditorCommandParameter.PrimaryCursorSnapshot.UserCursor.TextEditorSelection.AnchorPositionIndex +=
                    lowerBoundPositionIndexChange;
                
                textEditorCommandParameter.PrimaryCursorSnapshot.UserCursor.TextEditorSelection.EndingPositionIndex +=
                    upperBoundPositionIndexChange;
            }
            else
            {
                textEditorCommandParameter.PrimaryCursorSnapshot.UserCursor.TextEditorSelection.AnchorPositionIndex +=
                    upperBoundPositionIndexChange;
                
                textEditorCommandParameter.PrimaryCursorSnapshot.UserCursor.TextEditorSelection.EndingPositionIndex +=
                    lowerBoundPositionIndexChange;
            }

            var userCursorIndexCoordinates =
                textEditorCommandParameter.PrimaryCursorSnapshot.UserCursor.IndexCoordinates;
            
            textEditorCommandParameter.PrimaryCursorSnapshot.UserCursor.IndexCoordinates =
                (userCursorIndexCoordinates.rowIndex, userCursorIndexCoordinates.columnIndex + 1);
            return Task.CompletedTask;
        },
        false,
        "Indent More",
        "defaults_indent-more");
    
    public static readonly TextEditorCommand IndentLess = new(textEditorCommandParameter =>
        {
            var selectionBoundsInPositionIndexUnits = TextEditorSelectionHelper
                .GetSelectionBounds(
                    textEditorCommandParameter
                        .PrimaryCursorSnapshot
                        .ImmutableCursor
                        .ImmutableTextEditorSelection);

            var selectionBoundsInRowIndexUnits = TextEditorSelectionHelper
                .ConvertSelectionOfPositionIndexUnitsToRowIndexUnits(
                    textEditorCommandParameter.TextEditorModel,
                    selectionBoundsInPositionIndexUnits);

            bool isFirstLoop = true;
            
            for (var i = selectionBoundsInRowIndexUnits.lowerRowIndexInclusive;
                 i < selectionBoundsInRowIndexUnits.upperRowIndexExclusive;
                 i++)
            {
                var rowPositionIndex = textEditorCommandParameter.TextEditorModel
                    .GetPositionIndex(
                        i,
                        0);

                var characterReadCount = TextEditorModel.TAB_WIDTH;

                var lengthOfRow = textEditorCommandParameter.TextEditorModel.GetLengthOfRow(i);

                characterReadCount = Math.Min(lengthOfRow, characterReadCount);

                var readResult =
                    textEditorCommandParameter.TextEditorModel
                        .GetTextRange(rowPositionIndex, characterReadCount);

                int removeCharacterCount = 0;
                
                if (readResult.StartsWith(KeyboardKeyFacts.WhitespaceCharacters.TAB))
                {
                    removeCharacterCount = 1;
                    
                    var cursorForDeletion = new TextEditorCursor(
                        (i, 0),
                        true);
                    
                    var deleteTextTextEditorModelAction = new TextEditorModelsCollection.DeleteTextByRangeAction(
                        textEditorCommandParameter.TextEditorModel.ModelKey,
                        TextEditorCursorSnapshot.TakeSnapshots(cursorForDeletion),
                        removeCharacterCount, // Delete a single "Tab" character
                        CancellationToken.None);
                
                    textEditorCommandParameter
                        .TextEditorService
                        .ModelDeleteTextByRange(deleteTextTextEditorModelAction);
                }
                else if (readResult.StartsWith(KeyboardKeyFacts.WhitespaceCharacters.SPACE))
                {
                    var cursorForDeletion = new TextEditorCursor(
                        (i, 0),
                        true);

                    var contiguousSpaceCount = 0;
                    
                    foreach (var character in readResult)
                    {
                        if (character == KeyboardKeyFacts.WhitespaceCharacters.SPACE)
                            contiguousSpaceCount++;
                    }

                    removeCharacterCount = contiguousSpaceCount;
                    
                    var deleteTextTextEditorModelAction = new TextEditorModelsCollection.DeleteTextByRangeAction(
                        textEditorCommandParameter.TextEditorModel.ModelKey,
                        TextEditorCursorSnapshot.TakeSnapshots(cursorForDeletion),
                        removeCharacterCount,
                        CancellationToken.None);
                
                    textEditorCommandParameter
                        .TextEditorService
                        .ModelDeleteTextByRange(deleteTextTextEditorModelAction);
                }

                // Modify the lower bound of user's text selection
                if (isFirstLoop)
                {
                    isFirstLoop = false;
                    
                    if (textEditorCommandParameter.PrimaryCursorSnapshot.UserCursor.TextEditorSelection.AnchorPositionIndex <
                        textEditorCommandParameter.PrimaryCursorSnapshot.UserCursor.TextEditorSelection.EndingPositionIndex)
                    {
                        textEditorCommandParameter.PrimaryCursorSnapshot.UserCursor.TextEditorSelection.AnchorPositionIndex -=
                            removeCharacterCount;
                    }
                    else
                    {
                        textEditorCommandParameter.PrimaryCursorSnapshot.UserCursor.TextEditorSelection.EndingPositionIndex -=
                            removeCharacterCount;
                    }
                }
                
                // Modify the upper bound of user's text selection
                if (textEditorCommandParameter.PrimaryCursorSnapshot.UserCursor.TextEditorSelection.AnchorPositionIndex <
                    textEditorCommandParameter.PrimaryCursorSnapshot.UserCursor.TextEditorSelection.EndingPositionIndex)
                {
                    textEditorCommandParameter.PrimaryCursorSnapshot.UserCursor.TextEditorSelection.EndingPositionIndex -=
                        removeCharacterCount;
                }
                else
                {
                    textEditorCommandParameter.PrimaryCursorSnapshot.UserCursor.TextEditorSelection.AnchorPositionIndex -=
                        removeCharacterCount;
                }
                
                // Modify the column index of user's cursor
                if (i == textEditorCommandParameter.PrimaryCursorSnapshot.ImmutableCursor.RowIndex)
                {
                    var nextColumnIndex = textEditorCommandParameter.PrimaryCursorSnapshot.ImmutableCursor.ColumnIndex -
                                          removeCharacterCount;

                    var userCursorIndexCoordinates = textEditorCommandParameter
                        .PrimaryCursorSnapshot.UserCursor.IndexCoordinates;
                    
                    textEditorCommandParameter.PrimaryCursorSnapshot.UserCursor.IndexCoordinates =
                        (userCursorIndexCoordinates.rowIndex, Math.Max(0, nextColumnIndex));
                }
            }

            return Task.CompletedTask;
        },
        false,
        "Indent Less",
        "defaults_indent-less");
    
    public static readonly TextEditorCommand ClearTextSelection = new(
        textEditorCommandParameter =>
        {
            textEditorCommandParameter
                .PrimaryCursorSnapshot.UserCursor.TextEditorSelection.AnchorPositionIndex = null;
            
            return Task.CompletedTask;
        },
        false,
        "ClearTextSelection",
        "defaults_clear-text-selection");
    
    public static readonly TextEditorCommand NewLineBelow = new(
        textEditorCommandParameter =>
        {
            textEditorCommandParameter
                .PrimaryCursorSnapshot.UserCursor.TextEditorSelection.AnchorPositionIndex = null;
            
            var lengthOfRow = textEditorCommandParameter.TextEditorModel.GetLengthOfRow(
                textEditorCommandParameter
                    .PrimaryCursorSnapshot.UserCursor.IndexCoordinates.rowIndex);

            var temporaryIndexCoordinates = textEditorCommandParameter
                .PrimaryCursorSnapshot.UserCursor.IndexCoordinates;
            
            textEditorCommandParameter
                    .PrimaryCursorSnapshot.UserCursor.IndexCoordinates =
                (temporaryIndexCoordinates.rowIndex, lengthOfRow);
            
            textEditorCommandParameter.TextEditorService.ModelInsertText(
                new TextEditorModelsCollection.InsertTextAction(
                    textEditorCommandParameter.TextEditorModel.ModelKey,
                    TextEditorCursorSnapshot.TakeSnapshots(
                        textEditorCommandParameter.PrimaryCursorSnapshot.UserCursor),
                    "\n",
                    CancellationToken.None));
            
            return Task.CompletedTask;
        },
        false,
        "NewLineBelow",
        "defaults_new-line-below");
    
    public static readonly TextEditorCommand NewLineAbove = new(
        textEditorCommandParameter =>
        {
            textEditorCommandParameter
                .PrimaryCursorSnapshot.UserCursor.TextEditorSelection.AnchorPositionIndex = null;
            
            var temporaryIndexCoordinates = textEditorCommandParameter
                .PrimaryCursorSnapshot.UserCursor.IndexCoordinates;
            
            textEditorCommandParameter
                    .PrimaryCursorSnapshot.UserCursor.IndexCoordinates =
                (temporaryIndexCoordinates.rowIndex, 0);
            
            textEditorCommandParameter.TextEditorService.ModelInsertText(
                new TextEditorModelsCollection.InsertTextAction(
                    textEditorCommandParameter.TextEditorModel.ModelKey,
                    TextEditorCursorSnapshot.TakeSnapshots(
                        textEditorCommandParameter.PrimaryCursorSnapshot.UserCursor),
                    "\n",
                    CancellationToken.None));
            
            temporaryIndexCoordinates = textEditorCommandParameter
                .PrimaryCursorSnapshot.UserCursor.IndexCoordinates;

            if (temporaryIndexCoordinates.rowIndex > 1)
            {
                textEditorCommandParameter
                        .PrimaryCursorSnapshot.UserCursor.IndexCoordinates =
                    (temporaryIndexCoordinates.rowIndex - 1, 0);
            }
            
            return Task.CompletedTask;
        },
        false,
        "NewLineBelow",
        "defaults_new-line-below");
    
    public static TextEditorCommand GoToMatchingCharacterFactory(bool shouldSelectText) => new(
        textEditorCommandParameter =>
        {
            var cursorPositionIndex = textEditorCommandParameter.TextEditorModel.GetCursorPositionIndex(
                textEditorCommandParameter.PrimaryCursorSnapshot.UserCursor);
            
            if (shouldSelectText)
            {
                if (!TextEditorSelectionHelper.HasSelectedText(
                        textEditorCommandParameter.PrimaryCursorSnapshot.UserCursor.TextEditorSelection))
                {
                    textEditorCommandParameter
                            .PrimaryCursorSnapshot.UserCursor.TextEditorSelection.AnchorPositionIndex =
                        cursorPositionIndex;
                }
            }
            else
            {
                textEditorCommandParameter
                    .PrimaryCursorSnapshot.UserCursor.TextEditorSelection.AnchorPositionIndex = null;
            }
            
            var previousCharacter = textEditorCommandParameter.TextEditorModel.GetTextAt(
                cursorPositionIndex - 1);
            
            var currentCharacter = textEditorCommandParameter.TextEditorModel.GetTextAt(
                cursorPositionIndex);

            char? characterToMatch = null;
            char? match = null;
            var fallbackToPreviousCharacter = false;

            if (CharacterKindHelper.CharToCharacterKind(currentCharacter) == CharacterKind.Punctuation)
            {
                // Prefer current character
                match = KeyboardKeyFacts
                    .MatchPunctuationCharacter(currentCharacter);

                if (match is not null)
                    characterToMatch = currentCharacter;
            }
            
            if (characterToMatch is null &&
                CharacterKindHelper.CharToCharacterKind(previousCharacter) == CharacterKind.Punctuation)
            {
                // Fallback to the previous current character
                match = KeyboardKeyFacts
                    .MatchPunctuationCharacter(previousCharacter);
                
                if (match is not null)
                {
                    characterToMatch = previousCharacter;
                    fallbackToPreviousCharacter = true;
                }
            } 

            if (characterToMatch is null)
                return Task.CompletedTask;
            
            if (match is null)
                return Task.CompletedTask;

            var directionToFindMatchMatchingPunctuationCharacter = KeyboardKeyFacts
                .DirectionToFindMatchMatchingPunctuationCharacter(characterToMatch.Value);

            if (directionToFindMatchMatchingPunctuationCharacter is null)
                return Task.CompletedTask;

            var temporaryCursor = new TextEditorCursor(
                (textEditorCommandParameter.PrimaryCursorSnapshot.UserCursor.IndexCoordinates.rowIndex,
                    textEditorCommandParameter.PrimaryCursorSnapshot.UserCursor.IndexCoordinates.columnIndex),
                textEditorCommandParameter.PrimaryCursorSnapshot.UserCursor.IsPrimaryCursor);

            var unmatchedCharacters =
                (fallbackToPreviousCharacter &&
                 directionToFindMatchMatchingPunctuationCharacter == -1)
                    ? 0
                    : 1;

            while (true)
            {
                KeyboardEventArgs keyboardEventArgs;

                if (directionToFindMatchMatchingPunctuationCharacter == -1)
                {
                    keyboardEventArgs = new KeyboardEventArgs
                    {
                        Key = KeyboardKeyFacts.MovementKeys.ARROW_LEFT,
                    };
                }
                else
                {
                    keyboardEventArgs = new KeyboardEventArgs
                    {
                        Key = KeyboardKeyFacts.MovementKeys.ARROW_RIGHT,
                    };
                }
                
                TextEditorCursor.MoveCursor(
                    keyboardEventArgs,
                    temporaryCursor,
                    textEditorCommandParameter.TextEditorModel);

                var temporaryCursorPositionIndex = textEditorCommandParameter.TextEditorModel
                    .GetCursorPositionIndex(
                        temporaryCursor);
                
                var characterAt = textEditorCommandParameter.TextEditorModel.GetTextAt(
                    temporaryCursorPositionIndex);
                
                if (characterAt == match)
                    unmatchedCharacters--;
                else if (characterAt == characterToMatch)
                    unmatchedCharacters++;

                if (unmatchedCharacters == 0)
                    break;

                if (temporaryCursorPositionIndex <= 0 ||
                    temporaryCursorPositionIndex >= textEditorCommandParameter.TextEditorModel.DocumentLength)
                    break;
            }
            
            if (shouldSelectText)
            {
                textEditorCommandParameter
                        .PrimaryCursorSnapshot.UserCursor.TextEditorSelection.EndingPositionIndex =
                    textEditorCommandParameter.TextEditorModel.GetCursorPositionIndex(temporaryCursor);
            }
 
            textEditorCommandParameter.PrimaryCursorSnapshot.UserCursor.IndexCoordinates =
                temporaryCursor.IndexCoordinates;
            
            return Task.CompletedTask;
        },
        true,
        "GoToMatchingCharacter",
        "defaults_go-to-matching-character");
    
    public static readonly TextEditorCommand GoToDefinition = new(
        textEditorCommandParameter =>
        {
            if (textEditorCommandParameter.TextEditorModel.SemanticModel is null)
                return Task.CompletedTask;

            var positionIndex = textEditorCommandParameter.TextEditorModel
                .GetCursorPositionIndex(
                    textEditorCommandParameter.PrimaryCursorSnapshot.ImmutableCursor);
            
            var textSpanOfWordAtPositionIndex = textEditorCommandParameter.TextEditorModel
                .GetWordAt(positionIndex);
            
            if (textSpanOfWordAtPositionIndex is null)
                return Task.CompletedTask;

            var symbolDefinition = textEditorCommandParameter.TextEditorModel.SemanticModel
                .GoToDefinition(
                    textEditorCommandParameter.TextEditorModel,
                    textSpanOfWordAtPositionIndex);

            return Task.CompletedTask;
        },
        false,
        "GoToDefinition",
        "defaults_go-to-definition");
}