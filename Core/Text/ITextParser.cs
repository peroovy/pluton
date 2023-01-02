using System.Collections.Immutable;

namespace Core.Text
{
    public interface ITextParser
    {
        public ImmutableArray<Line> ParseLines(string text);
    }
}