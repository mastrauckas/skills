namespace MyMinimalWebApp.Api.IntegrationTests.Configuration;

public class SerilogEnrichmentTests
{
    [Fact]
    public void EnrichWithClientIp_SetsClientIpOnDiagnosticContext()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Connection.RemoteIpAddress = System.Net.IPAddress.Loopback;
        var context = new StubDiagnosticContext();

        AppConfigurationExtensions.EnrichWithClientIp(context, httpContext);

        Assert.True(context.WasCalled);
    }
}
