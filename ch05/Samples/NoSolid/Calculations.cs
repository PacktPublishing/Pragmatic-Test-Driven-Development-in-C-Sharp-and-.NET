public interface IArithmeticOperation {}

public class Addition : IArithmeticOperation
{
    public double Add(double left, double right) => left + right;
}

public class Subtraction : IArithmeticOperation
{
    public double Subtract(double left, double right) => left - right;
}

public class Calculation
{
    public double Calculate(IArithmeticOperation op, double left, double right) => 
        op switch
        {
            Addition addition => addition.Add(left, right),
            Subtraction subtraction => subtraction.Subtract(left, right),
            // Multiplication multiplication => multiplication.Multiply(left, right),
            _ => throw new NotImplementedException()
        };
}