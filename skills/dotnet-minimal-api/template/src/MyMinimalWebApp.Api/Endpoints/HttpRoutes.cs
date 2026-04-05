namespace MyMinimalWebApp.Api.Endpoints;

public static class HttpRoutesExtensions
{
    extension(WebApplication app)
    {
        public void ConfigureHttpRoutes()
        {
            var root = app.MapGroup("api");

            app.MapItemEndpoints(root);
        }
    }
}
