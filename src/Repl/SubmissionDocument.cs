using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Repl;

public class SubmissionDocument : IEnumerable<string>
{
    private readonly List<string> lines = new();

    private int lineIndex = -1;
    private int characterLeftIndex = -1;

    public event Action<SubmissionDocument> OnChanged;

    public int LineIndex
    {
        get => lineIndex;
        set
        {
            if (value < 0 || value >= LinesCount)
                throw new ArgumentOutOfRangeException();
            
            lineIndex = value;
            CharacterLeftIndex = Math.Min(CharacterLeftIndex, lines[value].Length - 1);
        }
    }

    public int CharacterLeftIndex
    {
        get => characterLeftIndex;
        set
        {
            if (value < -1 || value >= CharactersCount)
                throw new ArgumentOutOfRangeException();
            
            characterLeftIndex = value;
        }
    }

    public int LinesCount => lines.Count;

    public int CharactersCount => lines[LineIndex].Length;

    public void InsertEmptyLine()
    {
        lines.Insert(LineIndex + 1, string.Empty);
        LineIndex++;
        CharacterLeftIndex = -1; 
        
        OnChanged?.Invoke(this);
    }

    public void Insert(char symbol)
    {
        lines[lineIndex] = lines[lineIndex].Insert(CharacterLeftIndex + 1, symbol.ToString());
        CharacterLeftIndex++;
        
        OnChanged?.Invoke(this);
    }

    public void Insert(string str)
    {
        lines[lineIndex] = lines[lineIndex].Insert(CharacterLeftIndex + 1, str);
        CharacterLeftIndex += str.Length;
        
        OnChanged?.Invoke(this);
    }

    public void DeleteCharacter()
    {
        if (CharacterLeftIndex == -1)
        {
            if (LineIndex == 0)
                return;

            var (current, previous) = (lines[LineIndex], lines[LineIndex - 1]);
            
            lines.RemoveAt(LineIndex);
            LineIndex--;

            lines[LineIndex] = previous + current;
            CharacterLeftIndex = previous.Length - 1;
        }
        else
        {
            lines[LineIndex] = lines[LineIndex].Remove(CharacterLeftIndex, 1);
            CharacterLeftIndex--;
        }
        
        OnChanged?.Invoke(this);
    }

    public IEnumerator<string> GetEnumerator() => ((IEnumerable<string>)lines).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}