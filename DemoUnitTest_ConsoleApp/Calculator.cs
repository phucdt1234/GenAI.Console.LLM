namespace DemoUnitTest_ConsoleApp;

public class Calculator
{
    public int Add(int left, int right)
    {
        return left + right;
    }

    public int Subtract(int left, int right)
    {
        return left - right;
    }

    public int Multiply(int left, int right)
    {
        return left * right;
    }

    public int Divide(int left, int right)
    {
        if (right == 0)
        {
            throw new DivideByZeroException("Cannot divide by zero.");
        }

        return left / right;
    }
}
