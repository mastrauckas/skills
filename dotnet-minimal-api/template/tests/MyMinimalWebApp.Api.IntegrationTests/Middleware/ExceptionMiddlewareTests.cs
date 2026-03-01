using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;

namespace MyMinimalWebApp.Api.IntegrationTests.Middleware;

public class ExceptionMiddlewareTests : IClassFixture<ThrowingAppFactory>
{
    private readonly HttpClient _client;

    public ExceptionMiddlewareTests(ThrowingAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task UnhandledException_Returns500WithProblemDetails()
    {
        HttpResponseMessage response = await _client.GetAsync("/throw");

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);

        string body = await response.Content.ReadAsStringAsync();
        Assert.Contains("An error occurred while processing your request.", body);
        Assert.Contains("500", body);
    }
}
