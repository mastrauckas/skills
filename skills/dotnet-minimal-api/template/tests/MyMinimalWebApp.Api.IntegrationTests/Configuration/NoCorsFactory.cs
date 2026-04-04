namespace MyMinimalWebApp.Api.IntegrationTests.Configuration;

public class NoCorsFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
    }
}
