using System.Net;
using System.Text;
using System.Text.Json;
using Moq.Protected;

namespace WeatherForecast.Cli.Tests.Clients;
internal class HttpClientFactory
{
    private const string _baseAddress = "https://test.base.address";

    private readonly string _request;

    /// <summary>
    /// The status code that will be returned when the configured endpoint is called
    /// </summary>
    public HttpStatusCode HttpStatusCode { get; set; } = HttpStatusCode.InternalServerError;

    /// <summary>
    /// The content that will be returned when the configured endpoint is called
    /// </summary>
    public object? Content { get; set; }

    /// <summary>
    /// Provides a mean to create a <see cref="HttpClient"/> instance with a configured endpoint
    /// </summary>
    public HttpClientFactory(string endpoint)
    {
        _request = endpoint;
    }

    public HttpClient CreateClient()
    {
        string jsonContent = JsonSerializer.Serialize(Content);
        HttpResponseMessage httpResponseMessage = new()
        {
            StatusCode = HttpStatusCode,
            Content = new StringContent(jsonContent, Encoding.UTF8, "application/json")
        };

        Mock<HttpMessageHandler> httpMessageHandlerMock = new();
        httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
            ItExpr.Is<HttpRequestMessage>(
                r => r.Method == HttpMethod.Get
                  && r.RequestUri == new Uri(_baseAddress + _request)),
            ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponseMessage);

        HttpClient httpClient = new(httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri(_baseAddress)
        };

        return httpClient;
    }
}
