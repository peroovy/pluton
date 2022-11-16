using System.Collections.Immutable;

namespace Translator.Core.Text
{
    public interface ITextParser
    {
        public ImmutableArray<Line> ParseLines(string text);
    }
}