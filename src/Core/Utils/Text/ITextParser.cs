using System.Collections.Immutable;

namespace Core.Utils.Text
{
    public interface ITextParser
    {
        public ImmutableArray<Line> ParseLines(string text);
    }
}