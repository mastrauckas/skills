namespace MyMinimalWebApp.Api.Endpoints;

public static class HttpRoutesExtensions
{
    extension(WebApplication app)
    {
        public void ConfigureHttpRoutes()
        {
            RouteGroupBuilder root = app.MapGroup("api");

            app.MapItemEndpoints(root);
        }
    }
}
