namespace MyMinimalWebApp.Api.Configuration;

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
            builder.RegisterForwardedHeaders();
            builder.RegisterLogging();
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
                options.AddPolicy("AllowLocalDevelopment", policy =>
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

        public void RegisterProblemDetails() => builder.Services.AddProblemDetails();

        public void RegisterForwardedHeaders()
        {
            builder.Services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;

                // Uncomment the lines below when ALL of the following are true:
                //   - Running behind a reverse proxy (nginx, Kubernetes ingress, AWS ALB, Azure Front Door)
                //   - The proxy is the sole entry point (pods are not directly reachable from the internet)
                //   - Traffic arrives from a non-loopback IP (e.g., Kubernetes ClusterIP, internal VPC address)
                //
                // Common scenarios:
                //   - Kubernetes: ingress controller reaches pods via ClusterIP — not loopback, so headers
                //     are ignored by default. Clearing these lets any in-cluster proxy forward headers.
                //   - Docker Compose: nginx container forwards to app container via bridge network IP.
                //   - Cloud load balancers: AWS ALB, Azure App Gateway, Cloudflare — all use non-loopback IPs.
                //
                // Do NOT uncomment if pods/containers are directly reachable from the internet —
                // an attacker could spoof X-Forwarded-For to fake their IP.
                // options.KnownNetworks.Clear();
                // options.KnownProxies.Clear();
            });
        }

        public void RegisterValidation() => builder.Services.AddValidation();

        // ---- Customize these for your application ----

        public void RegisterLogging()
        {
            builder.Host.UseSerilog((ctx, config) =>
                config.ReadFrom.Configuration(ctx.Configuration));

            // Serilog is configured via the "Serilog" section in appsettings.json.
            // Active sinks: Console + File (rolling daily, 10-day retention).
            // To add more sinks, install the NuGet package and add to "Using" + "WriteTo" in appsettings.json:
            //
            // Seq (local structured log server — great for development):
            //   Package : Serilog.Sinks.Seq
            //   WriteTo : { "Name": "Seq", "Args": { "serverUrl": "http://localhost:5341" } }
            //
            // Azure Application Insights:
            //   Package : Serilog.Sinks.ApplicationInsights
            //   WriteTo : { "Name": "ApplicationInsights", "Args": { "connectionString": "<connection-string>", "telemetryConverter": "Serilog.Sinks.ApplicationInsights.TelemetryConverters.TraceTelemetryConverter, Serilog.Sinks.ApplicationInsights" } }
            //
            // Azure Blob Storage:
            //   Package : Serilog.Sinks.AzureBlobStorage
            //   WriteTo : { "Name": "AzureBlobStorage", "Args": { "connectionString": "<connection-string>", "storageContainerName": "logs", "storageFileName": "api-.log" } }
            //
            // Azure Cosmos DB:
            //   Package : Serilog.Sinks.AzureCosmosDB
            //   WriteTo : { "Name": "AzureCosmosDB", "Args": { "endpointUri": "<endpoint>", "authorizationKey": "<key>" } }
            //
            // Elasticsearch / Kibana (ELK stack):
            //   Package : Serilog.Sinks.Elasticsearch
            //   WriteTo : { "Name": "Elasticsearch", "Args": { "nodeUris": "http://localhost:9200", "indexFormat": "api-{0:yyyy.MM.dd}" } }
            //
            // Grafana Loki:
            //   Package : Serilog.Sinks.Grafana.Loki
            //   WriteTo : { "Name": "GrafanaLoki", "Args": { "uri": "http://localhost:3100" } }
            //
            // Datadog:
            //   Package : Serilog.Sinks.Datadog.Logs
            //   WriteTo : { "Name": "DatadogLogs", "Args": { "apiKey": "<api-key>" } }
            //
            // SQL Server:
            //   Package : Serilog.Sinks.MSSqlServer
            //   WriteTo : { "Name": "MSSqlServer", "Args": { "connectionString": "<connection-string>", "tableName": "Logs", "autoCreateSqlTable": true } }
            //
            // MongoDB:
            //   Package : Serilog.Sinks.MongoDB
            //   WriteTo : { "Name": "MongoDB", "Args": { "databaseUrl": "<connection-string>", "collectionName": "logs" } }
        }

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

        // Register feature services and repositories here
#pragma warning disable IDE0022
        public void RegisterServices()
        {
            builder.Services.AddSingleton<IItemService, ItemService>();
        }
#pragma warning restore IDE0022
    }
}
