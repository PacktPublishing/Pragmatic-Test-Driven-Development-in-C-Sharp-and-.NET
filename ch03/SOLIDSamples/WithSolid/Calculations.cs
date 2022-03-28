public interface IArithmeticOperation 
{
    public double Operate(double left, double right);
}

public class Addition : IArithmeticOperation
{
    public double Operate(double left, double right) => left + right;
}

public class Subtraction : IArithmeticOperation
{
    public double Operate(double left, double right) => left - right;
}

public class Multiplication : IArithmeticOperation
{
    public double Operate(double left, double right) => left * right;
}

public class Calculation
{
    public double Calculate(IArithmeticOperation op, double left, double right) =>
        op.Operate(left, right);
        
}