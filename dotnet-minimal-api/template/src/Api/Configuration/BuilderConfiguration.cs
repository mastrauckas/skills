using Api.Services;

namespace Api.Configuration;

public static class BuilderConfiguration
{
    public static void ConfigureBuilder(this WebApplicationBuilder builder)
    {
        builder.Services.AddOpenApi();
        builder.Services.AddAuthentication();
        builder.Services.AddAuthorization();
        builder.Services.AddEndpointsApiExplorer();

        var allowedOrigins = builder.Configuration
            .GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowLocalAngularDevelopment", policy =>
            {
                policy.WithOrigins(allowedOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        // Feature services
        builder.Services.AddItemServices();
    }
}
