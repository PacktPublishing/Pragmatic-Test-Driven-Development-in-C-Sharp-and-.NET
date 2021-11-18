using System;
using Xunit;
namespace Uqs.Arithmetic.Tests.Unit;

public class DivisionTests
{
    [Fact]
    public void Divide_DivisibleIntegers_WholeNumber()
    {
        // Arrange
        int dividend = 10;
        int divisor = 5;
        decimal expectedQuotient = 2;

        // Act
        decimal actualQuotient = Division.Divide(dividend, divisor);

        // Assert
        Assert.Equal(expectedQuotient, actualQuotient);
    }

    [Fact]
    public void Divide_IndivisibleIntegers_DecimalNumber()
    {
        // Arrange
        int dividend = 10;
        int divisor = 4;
        decimal expectedQuotient = 2.5m;

        // Act
        decimal actualQuotient = Division.Divide(dividend, divisor);

        // Assert
        Assert.Equal(expectedQuotient, actualQuotient);
    }

    [Fact]
    public void Divide_ZeroDivisor_DivideByZeroException()
    {
        // Arrange
        int dividend = 10;
        int divisor = 0;

        // Act
        Exception e = Record.Exception(() => Division.Divide(dividend, divisor));

        // Assert
        Assert.IsType<DivideByZeroException>(e);
    }

    [Theory]
    [InlineData(  int.MaxValue ,  int.MinValue , -0.999999999534 )]
    [InlineData( -int.MaxValue ,  int.MinValue ,  0.999999999534 )]
    [InlineData(  int.MinValue ,  int.MaxValue , -1.000000000466 )]
    [InlineData(  int.MinValue , -int.MaxValue ,  1.000000000466 )]
    public void Divide_ExtremeInput_CorrectCalculation(
               int dividend, int divisor, decimal expectedQuotient)
    {
        // Arrange

        // Act
        decimal actualQuotient = Division.Divide(dividend, divisor);

        // Assert
        Assert.Equal(expectedQuotient, actualQuotient, 12);
    }
}