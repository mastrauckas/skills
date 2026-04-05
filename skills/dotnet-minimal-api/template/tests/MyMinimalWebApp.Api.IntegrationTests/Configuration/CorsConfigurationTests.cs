namespace MyMinimalWebApp.Api.IntegrationTests.Configuration;

public class CorsConfigurationTests(NoCorsFactory factory)
    : IClassFixture<NoCorsFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task App_StartsSuccessfully_WithoutCorsOrigins()
    {
        var response = await _client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
