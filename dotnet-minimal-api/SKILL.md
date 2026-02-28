---
name: dotnet-minimal-api
description: Guide for creating .NET Minimal API projects following best practices. Use this when asked to create, scaffold, or extend a .NET Minimal API application.
---

## Template

A complete working reference solution is included in the `template/` directory alongside this file. When scaffolding a new project, use this template as the starting point — copy and rename it, replacing `Item`/`Items` with the appropriate domain entity name.

```
template/
  Api.slnx
  src/
    Api/
      Program.cs                          ← 4 lines, delegates to Configuration classes
      Configuration/
        BuilderConfiguration.cs           ← all service registrations
        AppConfiguration.cs               ← all middleware and endpoint mapping
      Endpoints/ItemEndpoints.cs
      Models/Item.cs
      Services/IItemService.cs
      Services/ItemService.cs
      appsettings.json                    ← includes Cors:AllowedOrigins
  tests/
    Api.Tests/
      Endpoints/ItemEndpointsTests.cs
```

## Key Patterns

### Program.cs

```csharp
var builder = WebApplication.CreateBuilder(args);
builder.ConfigureBuilder();

var app = builder.Build();
app.ConfigureApp();
app.Run();

public partial class Program { }
```

### BuilderConfiguration.cs

Extension method on `WebApplicationBuilder` for all service registrations:

```csharp
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
```

### AppConfiguration.cs

Extension method on `WebApplication` for middleware pipeline and endpoint mapping:

```csharp
public static class AppConfiguration
{
    public static void ConfigureApp(this WebApplication app)
    {
        app.UseExceptionHandler(...);   // JSON problem details
        app.UseCors("AllowLocalAngularDevelopment");
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapOpenApi();

        // Feature endpoints
        app.MapItemEndpoints();
    }
}
```

### Endpoints

- Static extension methods on `IEndpointRouteBuilder`
- Use `app.MapGroup("/items").WithTags("Items")`
- Always use `TypedResults` (not `Results`) for strongly-typed responses
- Each endpoint decorated with `.WithName()`, `.WithSummary()`, `.Produces<T>()`

### appsettings.json

```json
{
  "Cors": {
    "AllowedOrigins": ["http://localhost:4200"]
  }
}
```

### Testing

Use `WebApplicationFactory<Program>` for integration tests. The `public partial class Program {}` at the bottom of `Program.cs` makes it accessible.

```csharp
public class ItemEndpointsTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task GetAllItems_ReturnsOk()
    {
        var client = factory.CreateClient();
        var response = await client.GetAsync("/items");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
```

