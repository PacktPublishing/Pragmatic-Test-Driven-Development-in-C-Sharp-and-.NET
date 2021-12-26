//using System;
//using Xunit;

//namespace Uqs.Weather.Tests.Unit;

//public class SampleTests
//{
//    private int _instanceField = 0;
//    private static int _staticField = 0;

//    [Fact]
//    public void UnitTest1()
//    {
//        _instanceField++;
//        _staticField++;
//        Assert.Equal(1, _instanceField);
//        Assert.Equal(1, _staticField);
//    }

//    [Fact]
//    public void UnitTest2()
//    {
//        _instanceField++;
//        _staticField++;
//        Assert.Equal(1, _instanceField);
//        Assert.Equal(2, _staticField);
//    }

//    [Fact]
//    public void Load_InvalidJson_FormatException()
//    {
//        // Arrange
//        string input = "{not a valid JSON";

//        // Act
//        var exception = Record.Exception(
//            () => JsonParser.Load(input));
        
//        // Assert
//        Assert.IsType<FormatException>(exception);
//    }
//}

