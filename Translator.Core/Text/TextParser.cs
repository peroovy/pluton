using System.Collections.Immutable;
using System.Linq;

namespace Translator.Core.Text
{
    public class TextParser : ITextParser
    {
        public ImmutableArray<Line> ParseLines(string text)
        {
            var lines = text.Split('\n');

            return lines
                .Select((value, num) => new Line(num, num == lines.Length - 1 ? value + '\0' : value))
                .ToImmutableArray();
        }
    }
}