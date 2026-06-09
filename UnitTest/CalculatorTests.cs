using DemoUnitTest_ConsoleApp;

namespace UnitTest;

public class CalculatorTests
{
    [Fact]
    public void Add_ReturnsSum()
    {
        var calculator = new Calculator();

        int result = calculator.Add(2, 3);

        Assert.Equal(5, result);
    }

    [Fact]
    public void Divide_WithZeroDivisor_ThrowsDivideByZeroException()
    {
        var calculator = new Calculator();

        DivideByZeroException exception = Assert.Throws<DivideByZeroException>(() => calculator.Divide(10, 0));

        Assert.Equal("Cannot divide by zero.", exception.Message);
    }
}
