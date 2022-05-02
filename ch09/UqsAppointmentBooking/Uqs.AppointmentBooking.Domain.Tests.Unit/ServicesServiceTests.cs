using NSubstitute;
using System;
using System.Linq;
using System.Threading.Tasks;
using Uqs.AppointmentBooking.Domain.Services;
using Uqs.AppointmentBooking.Domain.Tests.Unit.Fakes;
using Xunit;

namespace Uqs.AppointmentBooking.Domain.Tests.Unit;

public class ServicesServiceTests : IDisposable
{
    private readonly ApplicationContextFakeBuilder _ctxBldr = new();
    private ServicesService? _sut;

    public void Dispose()
    {
        _ctxBldr.Dispose();
    }

    [Fact]
    public async Task GetActiveServices_NoServiceInTheSystem_NoServices()
    {
        // Arrange
        var ctx = _ctxBldr.Build();
        _sut = new ServicesService(ctx);

        // Act
        var actual = await _sut.GetActiveServices();

        // Assert
        Assert.True(!actual.Any());
    }

    [Fact]
    public async Task GetActiveServices_TwoActiveOneInactiveService_TwoServices()
    {
        // Arrange
        var ctx = _ctxBldr
            .WithSingleService(true)
            .WithSingleService(true)
            .WithSingleService(false)
            .Build();
        _sut = new ServicesService(ctx);
        var expected = 2;

        // Act
        var actual = await _sut.GetActiveServices();

        // Assert
        Assert.Equal(expected, actual.Count());
    }
}