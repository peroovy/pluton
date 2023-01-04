using System;
using System.Collections;
using System.Collections.Generic;

namespace Repl;

public class SubmissionDocument : IEnumerable<string>
{
    private readonly List<string> lines = new();

    private int lineIndex = -1;
    private int characterIndex = -1;

    public event Action<SubmissionDocument> OnChanged;

    public int LineIndex
    {
        get => lineIndex;
        set
        {
            if (value < -1 || value >= lines.Count)
                return;
            
            lineIndex = value;
            CharacterIndex = Math.Min(CharacterIndex, lines[value].Length - 1);
        }
    }

    public int CharacterIndex
    {
        get => characterIndex;
        set
        {
            if (value < -1 || value >= lines[LineIndex].Length)
                return;
            
            characterIndex = value;
        }
    }

    public void InsertEmptyLine()
    {
        lines.Insert(LineIndex + 1, string.Empty);
        LineIndex++;
        CharacterIndex = -1; 
        
        OnChanged?.Invoke(this);
    }

    public void Insert(char symbol)
    {
        lines[lineIndex] = lines[lineIndex].Insert(CharacterIndex + 1, symbol.ToString());
        CharacterIndex++;
        
        OnChanged?.Invoke(this);
    }

    public void DeleteCharacter()
    {
        if (CharacterIndex < 0)
            return;

        lines[LineIndex] = lines[LineIndex].Remove(CharacterIndex, 1);
        CharacterIndex--;
        
        OnChanged?.Invoke(this);
    }

    public IEnumerator<string> GetEnumerator() => lines.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}