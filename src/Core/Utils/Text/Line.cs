namespace Core.Utils.Text
{
    public class Line
    {
        public Line(SourceText text, int start, int length, int lineBreakLength)
        {
            Text = text;
            Start = start;
            Length = length;
            LineBreakLength = lineBreakLength;
        }

        public SourceText Text { get; }
        
        public int Start { get; }
        
        public int Length { get; }

        public int LengthWithLineBreak => Length + LineBreakLength;

        public int End => Start + Length;

        public int EndWithLineBreak => Start + LengthWithLineBreak;

        public int LineBreakLength { get; }
    }
}