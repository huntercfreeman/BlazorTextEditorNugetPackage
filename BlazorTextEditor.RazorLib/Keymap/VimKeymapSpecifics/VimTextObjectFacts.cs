﻿using System.Collections.Immutable;
using BlazorALaCarte.Shared.Keyboard;
using BlazorTextEditor.RazorLib.Commands;
using BlazorTextEditor.RazorLib.Cursor;
using BlazorTextEditor.RazorLib.Editing;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorTextEditor.RazorLib.Keymap.VimKeymapSpecifics;

public static class VimTextObjectFacts
{
    public static bool TryConstructTextObjectToken(
        KeyboardEventArgs keyboardEventArgs,
        bool hasTextSelection,
        out VimGrammarToken? vimGrammarToken)
    {
        switch (keyboardEventArgs.Key)
        {
            case "w":
            case "e":
            case "b":
            case "h":
            case "j":
            case "k":
            case "l":
            case "$":
            case "0":
            {
                vimGrammarToken = new VimGrammarToken(
                    VimGrammarKind.TextObject,
                    keyboardEventArgs.Key);

                return true;
            }
        }

        vimGrammarToken = null;
        return false;
    }

    public static bool TryParseVimSentence(
        ImmutableArray<VimGrammarToken> sentenceSnapshot,
        int indexInSentence,
        KeyboardEventArgs keyboardEventArgs,
        bool hasTextSelection,
        out TextEditorCommand textEditorCommand)
    {
        var currentToken = sentenceSnapshot[indexInSentence];
        
        switch (currentToken.TextValue)
        {
            case "w":
            {
                // Move to the start of the next word.
                
                textEditorCommand = new TextEditorCommand(
                    textEditorCommandParameter =>
                    {
                        VimTextEditorMotionFacts.Word(
                            textEditorCommandParameter.PrimaryCursorSnapshot.UserCursor,
                            textEditorCommandParameter.TextEditorBase);
                
                        return Task.CompletedTask;
                    },
                    true,
                    "Vim::w",
                    "vim_w");
                
                return true;
            }
            case "e":
            {
                // Move to the end of the next word.
                
                textEditorCommand = new TextEditorCommand(
                    textEditorCommandParameter =>
                    {
                        VimTextEditorMotionFacts.End(
                            textEditorCommandParameter.PrimaryCursorSnapshot.UserCursor,
                            textEditorCommandParameter.TextEditorBase);
                
                        return Task.CompletedTask;
                    },
                    true,
                    "Vim::e",
                    "vim_e");
                
                return true;
            }
            case "b":
            {
                // Move cursor to the beginning of the previous word.
                // As well, skip any whitespace that is along the way.
                
                textEditorCommand = new TextEditorCommand(
                    textEditorCommandParameter =>
                    {
                        VimTextEditorMotionFacts.Back(
                            textEditorCommandParameter.PrimaryCursorSnapshot.UserCursor,
                            textEditorCommandParameter.TextEditorBase);
                
                        return Task.CompletedTask;
                    },
                    true,
                    "Vim::b",
                    "vim_b");
                
                return true;
            }
            case "h":
            {
                // Move the cursor 1 column to the left
                
                textEditorCommand = new TextEditorCommand(
                    textEditorCommandParameter =>
                    {
                        TextEditorCursor.MoveCursor(
                            new KeyboardEventArgs
                            {
                                Key = KeyboardKeyFacts.MovementKeys.ARROW_LEFT
                            },
                            textEditorCommandParameter.PrimaryCursorSnapshot.UserCursor,
                            textEditorCommandParameter.TextEditorBase);
                
                        return Task.CompletedTask;
                    },
                    true,
                    "Vim::h",
                    "vim_h");
                
                return true;
            }
            case "j":
            {
                // Move the cursor 1 row down
                
                textEditorCommand = new TextEditorCommand(
                    textEditorCommandParameter =>
                    {
                        TextEditorCursor.MoveCursor(
                            new KeyboardEventArgs
                            {
                                Key = KeyboardKeyFacts.MovementKeys.ARROW_DOWN
                            },
                            textEditorCommandParameter.PrimaryCursorSnapshot.UserCursor,
                            textEditorCommandParameter.TextEditorBase);
                
                        return Task.CompletedTask;
                    },
                    true,
                    "Vim::j",
                    "vim_j");
                
                return true;
            }
            case "k":
            {
                // Move the cursor 1 row up
                
                textEditorCommand = new TextEditorCommand(
                    textEditorCommandParameter =>
                    {
                        TextEditorCursor.MoveCursor(
                            new KeyboardEventArgs
                            {
                                Key = KeyboardKeyFacts.MovementKeys.ARROW_UP
                            },
                            textEditorCommandParameter.PrimaryCursorSnapshot.UserCursor,
                            textEditorCommandParameter.TextEditorBase);
                
                        return Task.CompletedTask;
                    },
                    true,
                    "Vim::k",
                    "vim_k");
                
                return true;
            }
            case "l":
            {
                // Move the cursor 1 column to the right
                
                textEditorCommand = new TextEditorCommand(
                    textEditorCommandParameter =>
                    {
                        TextEditorCursor.MoveCursor(
                            new KeyboardEventArgs
                            {
                                Key = KeyboardKeyFacts.MovementKeys.ARROW_RIGHT
                            },
                            textEditorCommandParameter.PrimaryCursorSnapshot.UserCursor,
                            textEditorCommandParameter.TextEditorBase);
                
                        return Task.CompletedTask;
                    },
                    true,
                    "Vim::l",
                    "vim_l");
                
                return true;
            }
            case "$":
            {
                // Move the cursor to the end of the current line.
                
                textEditorCommand = new TextEditorCommand(
                    textEditorCommandParameter =>
                    {
                        TextEditorCursor.MoveCursor(
                            new KeyboardEventArgs
                            {
                                Key = KeyboardKeyFacts.MovementKeys.END
                            },
                            textEditorCommandParameter.PrimaryCursorSnapshot.UserCursor,
                            textEditorCommandParameter.TextEditorBase);
                
                        return Task.CompletedTask;
                    },
                    true,
                    "Vim::$",
                    "vim_$");
                
                return true;
            }
            case "0":
            {
                // Move the cursor to the start of the current line.
                
                textEditorCommand = new TextEditorCommand(
                    textEditorCommandParameter =>
                    {
                        TextEditorCursor.MoveCursor(
                            new KeyboardEventArgs
                            {
                                Key = KeyboardKeyFacts.MovementKeys.HOME
                            },
                            textEditorCommandParameter.PrimaryCursorSnapshot.UserCursor,
                            textEditorCommandParameter.TextEditorBase);
                
                        return Task.CompletedTask;
                    },
                    true,
                    "Vim::0",
                    "vim_0");
                
                return true;
            }
        }
        
        textEditorCommand = TextEditorCommandFacts.DoNothingDiscard;
        return true;
    }
}