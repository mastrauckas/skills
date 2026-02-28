namespace Api.Configuration;

public static class BuilderConfigurationExtensions
{
    extension(WebApplicationBuilder builder)
    {
        public void ConfigureBuilder()
        {
            builder.RegisterOpenApi();
            builder.RegisterAuthentication();
            builder.RegisterCors();
            builder.RegisterRateLimiting();
            builder.RegisterHealthChecks();
            builder.RegisterProblemDetails();
            builder.RegisterDatabase();
            builder.RegisterValidation();
            builder.RegisterServices();
        }

        public void RegisterOpenApi()
        {
            builder.Services.AddOpenApi();
            builder.Services.AddEndpointsApiExplorer();
        }

        public void RegisterAuthentication()
        {
            // Uncomment and configure a scheme (e.g. JWT Bearer) to enable authentication:
            // builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //     .AddJwtBearer(options =>
            //     {
            //         options.Authority = builder.Configuration["Auth:Authority"];
            //         options.Audience = builder.Configuration["Auth:Audience"];
            //     });
            builder.Services.AddAuthentication();
            builder.Services.AddAuthorization();
        }

        public void RegisterCors()
        {
            string[] allowedOrigins = builder
                .Configuration
                .GetSection("Cors:AllowedOrigins")
                .Get<string[]>() ?? [];

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

        public void RegisterRateLimiting()
        {
            builder.Services.AddRateLimiter(options =>
            {
                options.AddFixedWindowLimiter("fixed", limiter =>
                {
                    limiter.PermitLimit = 100;
                    limiter.Window = TimeSpan.FromSeconds(10);
                    limiter.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    limiter.QueueLimit = 10;
                });
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            });
        }

        public void RegisterHealthChecks()
        {
            builder.Services
                .AddHealthChecks()
                // Liveness: only checks if the process is running (used by Kubernetes liveness probe)
                .AddCheck("live", () => HealthCheckResult.Healthy(), tags: ["live"]);
            // Readiness: add dependency checks tagged "ready" for Kubernetes readiness probe. Examples:
            // .AddSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")!)  // SQL Server connectivity (requires AspNetCore.HealthChecks.SqlServer)
            // .AddNpgSql(builder.Configuration.GetConnectionString("DefaultConnection")!)     // PostgreSQL connectivity (requires AspNetCore.HealthChecks.NpgSql)
            // .AddRedis(builder.Configuration.GetConnectionString("Redis")!)                  // Redis cache connectivity (requires AspNetCore.HealthChecks.Redis)
            // .AddRabbitMQ(builder.Configuration.GetConnectionString("RabbitMQ")!)            // RabbitMQ message broker (requires AspNetCore.HealthChecks.RabbitMQ)
            // .AddMongoDb(builder.Configuration.GetConnectionString("MongoDB")!)              // MongoDB connectivity (requires AspNetCore.HealthChecks.MongoDb)
            // .AddCosmosDb(builder.Configuration.GetConnectionString("CosmosDb")!)            // Azure Cosmos DB connectivity (requires AspNetCore.HealthChecks.CosmosDb)
            // .AddAzureServiceBusQueue(builder.Configuration.GetConnectionString("ServiceBus")!, "queue-name")  // Azure Service Bus queue (requires AspNetCore.HealthChecks.AzureServiceBus)
            // .AddAzureBlobStorage(builder.Configuration.GetConnectionString("BlobStorage")!)   // Azure Blob Storage (requires AspNetCore.HealthChecks.AzureBlobStorage)
            // .AddAzureQueueStorage(builder.Configuration.GetConnectionString("QueueStorage")!) // Azure Queue Storage (requires AspNetCore.HealthChecks.AzureStorage)
            // .AddAzureKeyVault(new Uri(builder.Configuration["KeyVault:Uri"]!), new DefaultAzureCredential(), options => { }) // Azure Key Vault (requires AspNetCore.HealthChecks.AzureKeyVault)
            // .AddUrlGroup(new Uri("https://external-service/health"), "external-service")    // External HTTP dependency reachability
            // .AddProcessAllocatedMemoryHealthCheck(512);                                     // Fails if process exceeds 512 MB allocated memory
        }

        public void RegisterProblemDetails()
        {
            builder.Services.AddProblemDetails();
        }

        public void RegisterValidation()
        {
            builder.Services.AddValidation();
        }

        // ---- Customize these for your application ----

        public void RegisterDatabase()
        {
            // Register your database context here. Examples:
            //
            // Entity Framework Core (SQL Server):
            // builder.Services.AddDbContext<AppDbContext>(options =>
            //     options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            //
            // Entity Framework Core (PostgreSQL):
            // builder.Services.AddDbContext<AppDbContext>(options =>
            //     options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
            //
            // Dapper — just register your connection factory:
            // builder.Services.AddSingleton<IDbConnectionFactory, SqlConnectionFactory>();
        }

        public void RegisterServices()
        {
            // Register feature services and repositories here
            builder.Services.AddSingleton<IItemService, ItemService>();
        }
    }
}
