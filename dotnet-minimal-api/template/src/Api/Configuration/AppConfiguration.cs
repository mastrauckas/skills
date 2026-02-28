namespace Api.Configuration;

public static class AppConfigurationExtensions
{
    extension(WebApplication app)
    {
        public void ConfigureApp()
        {
            app.UseExceptionHandler(exceptionApp =>
                exceptionApp.Run(async context =>
                {
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                    var problemDetails = new
                    {
                        type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                        title = "An error occurred while processing your request.",
                        status = StatusCodes.Status500InternalServerError,
                        traceId = context.TraceIdentifier
                    };

                    await context.Response.WriteAsJsonAsync(problemDetails);
                }));

            app.UseCors("AllowLocalAngularDevelopment");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapOpenApi();

            // Feature endpoints
            app.MapItemEndpoints();
        }
    }
}
