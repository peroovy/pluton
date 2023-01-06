namespace Core.Utils.Text
{
    public class Location
    {
        public Location(SourceText sourceText, int start, int length)
        {
            SourceText = sourceText;
            Start = start;
            Length = length;
        }
        
        public SourceText SourceText { get; }
        
        public int Start { get; }
        
        public int Length { get; }
    }
}