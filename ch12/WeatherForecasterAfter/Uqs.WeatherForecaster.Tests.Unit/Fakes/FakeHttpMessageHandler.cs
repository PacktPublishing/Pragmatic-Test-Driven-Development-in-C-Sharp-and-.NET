namespace Uqs.WeatherForecaster.Tests.Unit.Fakes;

public class FakeHttpMessageHandler : HttpMessageHandler
{
    private HttpResponseMessage _fakeHttpResponseMessage;

    public FakeHttpMessageHandler(HttpResponseMessage responseMessage)
    {
        _fakeHttpResponseMessage = responseMessage;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return SendSpyAsync(request, cancellationToken);
    }

    public virtual Task<HttpResponseMessage> SendSpyAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return Task.FromResult(_fakeHttpResponseMessage);
    }
}