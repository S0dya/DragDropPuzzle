namespace PT.Tools.Other
{
    public enum ValueChangeType
    {
        Add,
        Subtract
    }

    public struct ValueChange
    {
        public readonly int Value;
        public readonly ValueChangeType Type;

        public ValueChange(int value, ValueChangeType type)
        {
            Value = value;
            Type = type;
        }
    }
}