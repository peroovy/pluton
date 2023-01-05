using System.Collections.Immutable;
using System.Linq;

namespace Core.Utils.Text
{
    public class TextParser : ITextParser
    {
        public ImmutableArray<Line> ParseLines(string text)
        {
            var lines = text.Split('\n');

            return lines
                .Select((line, i) =>
                {
                    var endLine = i == lines.Length - 1 ? '\0' : '\n';

                    return new Line(i, line + endLine);
                })
                .ToImmutableArray();
        }
    }
}