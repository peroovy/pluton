using System;
using System.Collections.Immutable;

namespace Core.Utils.Text
{
    public class SourceText
    {
        public SourceText(string value)
        {
            Value = value + '\0';
            Lines = ParseLines(value);
        }
        
        public string Value { get; }

        public int Length => Value.Length;

        public ImmutableArray<Line> Lines { get; }

        public char this[int index] => Value[index];

        public int GetLineIndex(int position)
        {
            var left = 0;
            var right = Lines.Length - 1;

            if (position < 0 || position >= Length)
                throw new ArgumentOutOfRangeException();

            while (left <= right)
            {
                var center = left + (right - left) / 2;
                var start = Lines[center].Start;

                if (start > position) right = center - 1;
                else left = center + 1;
            }

            return left - 1;
        }

        public string ToString(int start) => Value.Substring(start);

        public string ToString(int start, int length) => Value.Substring(start, length);

        private ImmutableArray<Line> ParseLines(string text)
        {
            var lineStart = 0;
            var position = 0;
            var lines = ImmutableArray.CreateBuilder<Line>();

            while (position < text.Length)
            {
                var lineBreakLength = GetLineBreakLength(text, position);
                if (lineBreakLength == 0)
                {
                    position++;
                    continue;
                }

                var line = new Line(this, lineStart, position - lineStart, lineBreakLength);
                lines.Add(line);

                lineStart = position += lineBreakLength;
            }

            if (position >= lineStart)
            {
                var line = new Line(this, lineStart, position - lineStart, 0);
                lines.Add(line);
            }

            return lines.ToImmutable();
        }

        private static int GetLineBreakLength(string text, int position)
        {
            var c = text[position];
            var l = position + 1 < text.Length ? text[position + 1] : '\0';

            return c switch
            {
                '\r' when l == '\n' => 2,
                '\n' => 1,
                _ => 0
            };
        }
    }
}