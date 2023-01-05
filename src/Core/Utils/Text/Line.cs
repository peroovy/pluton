namespace Core.Utils.Text
{
    public class Line
    {
        public Line(int index, string value)
        {
            Index = index;
            Value = value;
        }

        public char this[int index] => Value[index];
        
        public int Index { get; }
        
        public string Value { get; }

        public int Length => Value.Length;
    }
}