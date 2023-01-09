namespace Core.Execution.Operations
{
    public enum OperationPrecedence
    {
        Unary = 1000,
        Multiplicative = 900,
        Additive = 800,
        Shift = 700,
        Relational = 600,
        Equality = 500,
        BitwiseAnd = 400,
        BitwiseOr = 300,
        ConditionalAnd = 200,
        ConditionalOr = 100,
        None = 0
    }
}