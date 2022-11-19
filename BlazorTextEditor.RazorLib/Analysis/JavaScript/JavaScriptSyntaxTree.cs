﻿using System.Text;

namespace BlazorTextEditor.RazorLib.Analysis.JavaScript;

public class JavaScriptSyntaxTree
{
    /// <summary>
    /// First attempt at lexing JavaScript here will return
    /// List&lt;string&gt; as I want to start by parsing the Keywords
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    public static List<string> ParseText(string content)
    {
        // Will contain the final result which will be returned.
        var foundKeywords = new List<string>();
        
        // A single while loop will go character by character
        // until the end of the file for this method.
        var stringWalker = new StringWalker(content);

        // The wordBuilder is appended to everytime a
        // character is consumed.
        var wordBuilder = new StringBuilder();
        
        // When a keyword does not '.StartsWith()'
        // the 'wordBuilder.ToString()' then remove it
        // from the possibleKeywordsState List
        //
        // If one finds whitespace then check if any of the remaining
        // possibleKeywordsState entries are an exact match to the
        // 'wordBuilder.ToString()'. If there is a keyword
        // which is an exact match, then set that span of text
        // to have the keyword decoration byte.
        //
        // After finding a keyword, reset 'possibleKeywordsState' list
        // back to JavaScriptKeywords.All.ToList(); As well,
        // clear 'wordBuilder'
        var possibleKeywordsState = JavaScriptKeywords.All.ToList();
        
        stringWalker.WhileNotEndOfFile(() =>
        {
            if (JavaScriptWhitespace.WHITESPACE
                .Contains(stringWalker.CurrentCharacter.ToString()))
            {
                // Check if wordBuilder contains a keyword

                // Using .First as I am proceeding the presumption
                // that .First() will short circuit and not evaluate
                // the predicate on the remaining keywords.
                //
                // Whereas I presume .Single would have to Assert there
                // was only one match by iterating through all the keywords.
                //
                // Not sure however if the keyword string comparisons
                // would even be impactful enough on performance
                // for this to be useful if it is true.
                var foundKeyword = possibleKeywordsState
                    .FirstOrDefault(keyword => 
                        keyword == wordBuilder.ToString());
                
                if (foundKeyword is not null)
                {
                    FoundKeyword(foundKeyword);
                }
            }
            else
            {
                wordBuilder.Append(stringWalker.CurrentCharacter);
                
                possibleKeywordsState = possibleKeywordsState
                    .Where(keyword =>
                        keyword.StartsWith(wordBuilder.ToString()))
                    .ToList();
            }

            return false;
        });
        
        // When the end of the file is found
        // the final keyword, if there is no whitespace after it,
        // the keyword will not be added to the list without this code
        {
            var foundKeyword = possibleKeywordsState
                .FirstOrDefault(keyword =>
                    keyword == wordBuilder.ToString());
            
            if (foundKeyword is not null)
            {
                FoundKeyword(foundKeyword);
            }
        }

        return foundKeywords;

        void FoundKeyword(string keyword)
        {
            foundKeywords.Add(keyword);
            wordBuilder.Clear();
            possibleKeywordsState = JavaScriptKeywords.All.ToList();
        }
    }
}