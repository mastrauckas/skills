namespace MyMinimalWebApp.Api.IntegrationTests.Middleware;

public class ThrowingAppFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.Configure(app =>
        {
            app.UseMiddleware<MyMinimalWebApp.Api.Middleware.ExceptionMiddleware>();
            app.Map("/throw", throwApp =>
                throwApp.Run(_ => throw new InvalidOperationException("Test unhandled exception")));
        });
    }
}
