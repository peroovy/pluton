namespace Translator.Core.Text
{
    public class Line
    {
        public Line(int number, string value)
        {
            Number = number;
            Value = value;
        }
        
        public int Number { get; }
        
        public string Value { get; }

        public int Length => Value.Length;
    }
}