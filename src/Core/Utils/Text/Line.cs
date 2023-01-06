namespace Core.Utils.Text
{
    public class Line
    {
        public Line(SourceText text, int start, int length)
        {
            Text = text;
            Start = start;
            Length = length;
        }

        public SourceText Text { get; }
        
        public int Start { get; }
        
        public int Length { get; }

        public string ToString(int start) => ToString().Substring(start);

        public string ToString(int start, int length) => ToString().Substring(start, length);

        public override string ToString() => Text.Value.Substring(Start, Length);
    }
}