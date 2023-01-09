namespace Core.Utils.Text
{
    public class Location
    {
        public Location(SourceText sourceText, int start, int length)
        {
            SourceText = sourceText;
            Span = new TextSpan(start, length);
        }

        public Location(SourceText sourceText, TextSpan span)
        {
            SourceText = sourceText;
            Span = span;
        }
        
        public SourceText SourceText { get; }
        
        public TextSpan Span { get; }
    }
}