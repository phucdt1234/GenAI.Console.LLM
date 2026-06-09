using DemoUnitTest_ConsoleApp;
using Xunit;

namespace UnitTest;

public class UnitTest_Generated
{
    [Theory]
    [InlineData(1, 2, 3)]
    [InlineData(-4, 9, 5)]
    [InlineData(0, 0, 0)]
    public void Add_ReturnsExpectedSum(int left, int right, int expected)
    {
        var calculator = new Calculator();

        int actual = calculator.Add(left, right);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(8, 2, 4)]
    [InlineData(9, 3, 3)]
    [InlineData(-12, 4, -3)]
    public void Divide_ReturnsExpectedQuotient(int left, int right, int expected)
    {
        var calculator = new Calculator();

        int actual = calculator.Divide(left, right);

        Assert.Equal(expected, actual);
    }
}
