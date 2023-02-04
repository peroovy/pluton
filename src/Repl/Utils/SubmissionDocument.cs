using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Repl.Utils;

public class SubmissionDocument : IEnumerable<string>
{
    private List<string> lines = new() { BlankLine };

    private const string BlankLine = "\0";

    public int LineIndex { get; private set; }
    
    public int CharacterIndex { get; private set; }

    public int LineCount => lines.Count;

    public bool IsEmpty => lines.Count == 1 && IsBlankLine(lines[0]);

    public bool IsEnd => LineIndex == LineCount - 1 && CharacterIndex == CurrentLine.Length - 1;
    
    private string CurrentLine => lines[LineIndex];
    
    public override string ToString()
    {
        return string.Join(Environment.NewLine, lines.Select(line => line.Substring(0, line.Length - 1)));
    }

    public void AddNewLine(bool withHyphenation)
    {
        var hyphenation = BlankLine;

        if (withHyphenation)
        {
            hyphenation = CurrentLine.Substring(CharacterIndex);
            lines[LineIndex] = lines[LineIndex].Substring(0, CharacterIndex) + '\0';
        }
        
        lines.Insert(++LineIndex, hyphenation);
        CharacterIndex = 0;
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
    }

    public void Insert(string str)
    {
        lines[LineIndex] = lines[LineIndex].Insert(CharacterIndex, str);
        CharacterIndex += str.Length;
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
    }

    public void RemoveLeft()
    {
        if (CharacterIndex == 0)
        {
            if (LineIndex == 0)
                return;

            var line = CurrentLine.Substring(0, CurrentLine.Length - 1);
            lines.RemoveAt(LineIndex--);

            CharacterIndex = CurrentLine.Length - 1;
            lines[LineIndex] = CurrentLine.Insert(CharacterIndex, line);
        }
        else
        {
            lines[LineIndex] = lines[LineIndex].Remove(--CharacterIndex, 1);
        }
    }

    public void RemoveRight()
    {
        if (CharacterIndex == CurrentLine.Length - 1)
        {
            if (LineIndex == lines.Count - 1)
                return;

            var nextIndex = LineIndex + 1;
            var nextLine = lines[nextIndex].Substring(0, lines[nextIndex].Length - 1);
            lines.RemoveAt(nextIndex);

            lines[LineIndex] = CurrentLine.Insert(CharacterIndex, nextLine);
        }
        else
        {
            lines[LineIndex] = lines[LineIndex].Remove(CharacterIndex, 1);
        }
    }

    public void Clear()
    {
        lines = new List<string> { BlankLine };
        LineIndex = 0;
        CharacterIndex = 0;
    }

    public IEnumerator<string> GetEnumerator() => ((IEnumerable<string>)lines).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public static bool IsBlankLine(string line) => line == BlankLine;
}