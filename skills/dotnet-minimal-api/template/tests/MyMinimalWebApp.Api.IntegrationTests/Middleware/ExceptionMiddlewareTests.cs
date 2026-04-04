namespace MyMinimalWebApp.Api.IntegrationTests.Middleware;

public class ExceptionMiddlewareTests(ThrowingAppFactory factory) : IClassFixture<ThrowingAppFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task UnhandledException_Returns500WithProblemDetails()
    {
        var response = await _client.GetAsync("/throw");

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);

        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("An error occurred while processing your request.", body);
        Assert.Contains("500", body);
    }
}
