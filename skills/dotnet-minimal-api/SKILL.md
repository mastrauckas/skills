---
name: dotnet-minimal-api
description:
  Guide for creating .NET Minimal API projects following best practices. Use this when asked to
  create, scaffold, or extend a .NET Minimal API application.
user-invocable: true
argument-hint: "create a new .NET Minimal API project"
metadata:
  version: 1.0.6
  author: Michael Astrauckas
  tags: dotnet, minimal-api, csharp
  created: "2026-02-28"
  dotnet-version: "10.0"
---

## Quick Start

1. Ask for the **solution name** and **project name** (see [Scaffolding](#scaffolding-a-new-project))
2. Copy `template/` into the output directory
3. Rename all src projects, all test projects, and all namespace references from `MyMinimalWebApp.Api` → `<ProjectName>` (see [Scaffolding](#scaffolding-a-new-project))
4. Replace `Item`/`Items` with your domain entity name
5. Run `dotnet test` to verify everything passes

## Scaffolding a New Project

When asked to create a new project, **ask the user the following questions before doing any work**:

1. **Solution name** — what should the solution be called? (required)
2. **Project name** — what should the API project be called? (required; conventionally
   `<SolutionName>.Api`)
3. **Output directory** — where should the project be created? (defaults to the current working
   directory)

Once you have the answers:

- Copy the contents of the `template/` directory directly into the output directory (do **not**
  create an extra subdirectory — the output directory itself is the project root)
- Rename `MyMinimalWebApp.slnx` → `<SolutionName>.slnx`
- Rename `src/MyMinimalWebApp.Api/` → `src/<ProjectName>/`
- Rename `src/MyMinimalWebApp.Api/MyMinimalWebApp.Api.csproj` →
  `src/<ProjectName>/<ProjectName>.csproj`
- Update all namespace references from `MyMinimalWebApp.Api` → `<ProjectName>` throughout all `.cs`
  files
- Update all project references in the `.slnx` file
- Update the `InternalsVisibleTo` in `src/<ProjectName>/<ProjectName>.csproj` from
  `MyMinimalWebApp.Api.IntegrationTests` → `<ProjectName>.IntegrationTests`
- Rename `tests/MyMinimalWebApp.Api.IntegrationTests/` → `tests/<ProjectName>.IntegrationTests/`
- Rename `tests/MyMinimalWebApp.Api.UnitTests/` → `tests/<ProjectName>.UnitTests/`
- Update all namespace references in test projects from `MyMinimalWebApp.Api` → `<ProjectName>`
- Replace `Item`/`Items` with the appropriate domain entity name if provided

## Template

A complete working reference solution is included in the `template/` directory alongside this file.
When scaffolding a new project, use this template as the starting point — copy and rename it,
replacing `Item`/`Items` with the appropriate domain entity name.

```
template/
  MyMinimalWebApp.slnx                        ← solution with src/, tests/, http-files/ folders
  global.json                               ← pins .NET SDK version
  Directory.Build.props                     ← shared build properties (TreatWarningsAsErrors, EnforceCodeStyleInBuild)
  Directory.Packages.props                  ← Central Package Management (all NuGet versions here)
  Directory.Build.targets                   ← placeholder for post-build targets
  .editorconfig                             ← C# coding style rules at :warning severity
  .gitignore                                ← .NET gitignore (bin, obj, logs, TestResults)
  src/
    MyMinimalWebApp.Api/
      Program.cs                            ← minimal: UseSerilog + ConfigureBuilder + ConfigureApp
      GlobalUsings.cs                       ← all global usings centralized here
      Configuration/
        BuilderConfiguration.cs             ← all service registrations (RegisterX methods)
        AppConfiguration.cs                 ← all middleware and endpoint mapping
      Endpoints/
        HttpRoutes.cs                       ← creates api root group, calls MapItemEndpoints
        ItemEndpoints.cs                    ← 5 CRUD endpoints as private static handlers
      Dtos/
        ItemDto.cs                          ← response DTO
        ItemRequests.cs                     ← CreateItemRequest, UpdateItemRequest
      Logging/
        Log.cs                              ← [LoggerMessage] source-generated log methods
      Middleware/
        ExceptionMiddleware.cs              ← catches unhandled exceptions, returns ProblemDetails
      Services/
        IItemService.cs
        ItemService.cs
      Properties/
        launchSettings.json                 ← Kestrel profiles, launchBrowser: false
      appsettings.json                      ← Serilog, Cors, ConnectionStrings, Auth, KeyVault
      appsettings.Development.json          ← Debug log level override
      appsettings.Production.json           ← Warning log level override
  tests/
    MyMinimalWebApp.Api.IntegrationTests/
      Endpoints/ItemEndpointsTests.cs       ← WebApplicationFactory<Program> integration tests
      HealthChecks/HealthCheckTests.cs      ← /health, /health/live, /health/ready tests
      Middleware/ExceptionMiddlewareTests.cs ← 500 + ProblemDetails test
      Middleware/ThrowingAppFactory.cs      ← custom WebApplicationFactory for exception tests
      GlobalUsings.cs                       ← all global usings centralized here
    MyMinimalWebApp.Api.UnitTests/
      Services/ItemServiceTests.cs          ← unit tests with Bogus test data
      GlobalUsings.cs                       ← all global usings centralized here
  http-files/
    items.http                              ← all CRUD requests
    health.http                             ← health check requests
```

## Coding Conventions

- All `using` statements → `GlobalUsings.cs` only
- Use `var` when type is apparent from the right-hand side (constructor calls, `.ToList()`)
- Use explicit type when type is not apparent (method return values, async results)
- 2+ parameters → each on its own line
- Method chains with 2+ dots → each `.` on its own line
- `TreatWarningsAsErrors = true` + `EnforceCodeStyleInBuild = true` — all style rules enforced at
  build time
- ILogger aliases in GlobalUsings: `ILogger` = `Microsoft.Extensions.Logging.ILogger`,
  `SerilogLogger` = `Serilog.ILogger`

## Key Patterns

### Program.cs

```csharp
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((ctx, config) =>
    config.ReadFrom.Configuration(ctx.Configuration));
builder.ConfigureBuilder();

WebApplication app = builder.Build();
app.ConfigureApp();
app.Run();

public partial class Program { }
```

### BuilderConfiguration.cs

Extension block on `WebApplicationBuilder` — one `RegisterX()` method per concern:

```csharp
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
            builder.RegisterLogging();
            builder.RegisterDatabase();
            builder.RegisterValidation();
            builder.RegisterServices();
        }

        public void RegisterCors()
        {
            string[] allowedOrigins = builder
                .Configuration
                .GetSection("Cors:AllowedOrigins")
                .Get<string[]>() ?? [];

            builder.Services.AddCors(options => { ... });
        }

        public void RegisterHealthChecks()
        {
            builder.Services
                .AddHealthChecks()
                .AddCheck("live", () => HealthCheckResult.Healthy(), tags: ["live"]);
                // Add dependency checks tagged "ready" for readiness probe
        }

        public void RegisterProblemDetails() => builder.Services.AddProblemDetails();
        public void RegisterValidation() => builder.Services.AddValidation();

#pragma warning disable IDE0022
        public void RegisterServices()
        {
            builder.Services.AddSingleton<IItemService, ItemService>();
        }
#pragma warning restore IDE0022
    }
}
```

### AppConfiguration.cs

Extension block on `WebApplication` — middleware pipeline and endpoint mapping:

```csharp
public static class AppConfigurationExtensions
{
    extension(WebApplication app)
    {
        public void ConfigureApp()
        {
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseSerilogRequestLogging();

            if (app.Environment.IsDevelopment())
                app.UseCors("AllowLocalAngularDevelopment");

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseRateLimiter();
            app.MapOpenApi();

            app.MapHealthChecks("/health");
            app.MapHealthChecks("/health/live", new HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("live")
            });
            app.MapHealthChecks("/health/ready", new HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("ready")
            });

            app.ConfigureHttpRoutes();
        }
    }
}
```

### HttpRoutes.cs + Endpoints

`HttpRoutes.cs` creates the `api` root group and delegates to feature endpoint mappers:

```csharp
extension(WebApplication app)
{
    public void ConfigureHttpRoutes()
    {
        RouteGroupBuilder root = app.MapGroup("api");
        app.MapItemEndpoints(root);
    }
}
```

`ItemEndpoints.cs` extends `WebApplication`, receives the root group:

```csharp
extension(WebApplication app)
{
    public void MapItemEndpoints(RouteGroupBuilder root)
    {
        RouteGroupBuilder group = root
            .MapGroup("/items")
            .WithTags("Items");

        group.MapGet("/", GetAllItems).WithName("ListItems")...;
        group.MapGet("/{id:int}", GetItemById).WithName("GetItem")...;
        group.MapPost("/", CreateItem).WithName("CreateItem")...;
        group.MapPut("/{id:int}", UpdateItem).WithName("UpdateItem")...;
        group.MapDelete("/{id:int}", DeleteItem).WithName("DeleteItem")...;
    }
}

// Handlers are private static methods — TypedResults infers OpenAPI responses automatically
// Do NOT add .Produces<T>() — TypedResults + Results<T1,T2> return types handle it
private static async Task<Ok<IEnumerable<ItemDto>>> GetAllItems(IItemService service)
{
    IEnumerable<ItemDto> items = await service.GetAllAsync();
    return TypedResults.Ok(items);
}
```

### Serilog (appsettings.json)

Configured entirely via `appsettings.json` — no code changes needed to add sinks:

```json
{
  "Serilog": {
    "Using": ["Serilog.Sinks.Console", "Serilog.Sinks.File"],
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.AspNetCore": "Warning"
      }
    },
    "WriteTo": [
      { "Name": "Console" },
      {
        "Name": "File",
        "Args": { "path": "logs/api-.log", "rollingInterval": "Day", "retainedFileCountLimit": 10 }
      }
    ],
    "Enrich": ["FromLogContext", "WithMachineName", "WithThreadId"]
  }
}
```

Additional sinks (Seq, App Insights, Blob Storage, Cosmos DB, Elasticsearch, Loki, Datadog, SQL
Server, MongoDB) are documented as comments in `RegisterLogging()` inside `BuilderConfiguration.cs`.

### Testing

Two test projects:

- **`MyMinimalWebApp.Api.IntegrationTests`** — `WebApplicationFactory<Program>`, tests HTTP endpoints and middleware
- **`MyMinimalWebApp.Api.UnitTests`** — tests service layer directly with Bogus for test data, NSubstitute for mocks

`public partial class Program {}` at the bottom of `Program.cs` is required for
`WebApplicationFactory<Program>`.

```csharp
public class ItemEndpointsTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task GetAllItems_ReturnsOk()
    {
        HttpResponseMessage response = await factory.CreateClient().GetAsync("/api/items");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
```
