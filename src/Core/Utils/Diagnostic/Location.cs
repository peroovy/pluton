namespace Core.Utils.Diagnostic
{
    public struct Location
    {
        public Location(Line line, int start, int length)
        {
            Line = line;
            Start = start;
            Length = length;
        }
        
        public Line Line { get; }
        
        public int Start { get; }
        
        public int Length { get; }
    }
}