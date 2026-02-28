namespace Api.Configuration;

public static class BuilderConfigurationExtensions
{
    extension(WebApplicationBuilder builder)
    {
        public void RegisterOpenApi()
        {
            builder.Services.AddOpenApi();
            builder.Services.AddEndpointsApiExplorer();
        }

        public void RegisterAuthentication()
        {
            builder.Services.AddAuthentication();
            builder.Services.AddAuthorization();
        }

        public void RegisterCors()
        {
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
        }

        public void RegisterServices()
        {
            // Feature services
            builder.Services.AddItemServices();
        }
    }
}
