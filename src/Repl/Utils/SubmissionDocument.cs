﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Repl.Utils;

public class SubmissionDocument : IEnumerable<string>
{
    private List<string> lines = new() { BlankLine };

    private const string BlankLine = "\0";

    public event Action<SubmissionDocument> OnChanged;
    
    public int LineIndex { get; private set; }
    
    public int CharacterIndex { get; private set; }

    public int LineCount => lines.Count;

    public bool IsEmpty => lines.Count == 1 && IsBlankLine(lines[0]);
    
    private string CurrentLine => lines[LineIndex];
    
    public override string ToString()
    {
        return string.Join(Environment.NewLine, lines.Select(line => line.Substring(0, line.Length - 1)));
    }

    public void AddNewLine()
    {
        lines.Insert(++LineIndex, "\0");
        CharacterIndex = 0;
        
        OnChanged?.Invoke(this);
    }

    public bool MoveNext()
    {
        if (CharacterIndex == CurrentLine.Length - 1 && LineIndex + 1 < lines.Count)
        {
            LineIndex++;
            CharacterIndex = 0;

            return true;
        }

        if (CharacterIndex + 1 < CurrentLine.Length)
        {
            CharacterIndex++;
            
            return true;
        }

        return false;
    }

    public bool MoveBack()
    {
        if (CharacterIndex == 0 && LineIndex - 1 >= 0)
        {
            LineIndex--;
            CharacterIndex = CurrentLine.Length - 1;

            return true;
        }

        if (CharacterIndex - 1 >= 0)
        {
            CharacterIndex--;
            
            return true;
        }

        return false;
    }

    public bool MoveToNextLine()
    {
        if (LineIndex + 1 == lines.Count)
            return false;
        
        LineIndex++;
        CharacterIndex = CurrentLine.Length - 1;
        return true;
    }

    public bool MoveToPreviousLine()
    {
        if (LineIndex == 0)
            return false;

        LineIndex--;
        CharacterIndex = CurrentLine.Length - 1;
        return true;
    }

    public void Insert(char character)
    {
        lines[LineIndex] = lines[LineIndex].Insert(CharacterIndex++, character.ToString());
        
        OnChanged?.Invoke(this);
    }

    public void Insert(string str)
    {
        lines[LineIndex] = lines[LineIndex].Insert(CharacterIndex, str);
        CharacterIndex += str.Length;
        
        OnChanged?.Invoke(this);
    }
    
    public void Insert(SubmissionDocument document)
    {
        if (document.IsEmpty)
            return;
        
        if (IsEmpty)
            lines.Clear();
        
        lines.InsertRange(LineIndex, document.lines);

        LineIndex += document.lines.Count - 1;
        CharacterIndex = document.lines.Last().Length - 1;
        
        OnChanged?.Invoke(this);
    }

    public void Remove()
    {
        if (CharacterIndex == 0)
        {
            if (LineIndex == 0)
                return;

            var line = CurrentLine.Substring(0, CurrentLine.Length - 1);
            lines.RemoveAt(LineIndex);

            LineIndex--;
            CharacterIndex = CurrentLine.Length - 1;
            Insert(line);
            
            return;
        }
        
        lines[LineIndex] = lines[LineIndex].Remove(--CharacterIndex, 1);
        
        OnChanged?.Invoke(this);
    }

    public void Clear()
    {
        lines = new List<string> { BlankLine };
        LineIndex = 0;
        CharacterIndex = 0;
        
        OnChanged?.Invoke(this);
    }

    public IEnumerator<string> GetEnumerator() => ((IEnumerable<string>)lines).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public static bool IsBlankLine(string line) => line == BlankLine;
}