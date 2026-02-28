namespace Api.Configuration;

public static class AppConfigurationExtensions
{
    extension(WebApplication app)
    {
        public void ConfigureApp()
        {
            app.UseMiddleware<ExceptionMiddleware>();

            // ORDER MATTERS: UseCors must come before UseAuthentication/UseAuthorization
            // so CORS preflight requests are handled before auth middleware runs.
            if (app.Environment.IsDevelopment())
            {
                app.UseCors("AllowLocalAngularDevelopment");
            }

            // ORDER MATTERS: UseAuthentication must come before UseAuthorization.
            // Authentication establishes who the user is; authorization uses that identity.
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapOpenApi();

            // Feature endpoints
            app.MapItemEndpoints();
        }
    }
}
