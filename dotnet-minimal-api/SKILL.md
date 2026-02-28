---
name: dotnet-minimal-api
description: Guide for creating .NET Minimal API projects following best practices. Use this when asked to create, scaffold, or extend a .NET Minimal API application.
---

# .NET Minimal API Best Practices

When creating or extending a .NET Minimal API project, follow these guidelines:

## Project Structure

Organize code using a feature-based structure:

```
src/
  MyApi/
    Program.cs
    Endpoints/
      ProductEndpoints.cs
      OrderEndpoints.cs
    Models/
      Product.cs
    Services/
      IProductService.cs
      ProductService.cs
    Data/
      AppDbContext.cs
```

## Program.cs Conventions

- Register all services in `Program.cs` using extension methods from each feature folder.
- Add OpenAPI/Swagger support via `builder.Services.AddOpenApi()` and `app.MapOpenApi()`.
- Use `app.MapGroup()` to group related endpoints with a common prefix.

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddProductServices(); // feature extension methods

var app = builder.Build();

app.MapOpenApi();
app.MapProductEndpoints();

app.Run();
```

## Endpoint Conventions

- Define endpoints in static extension methods on `IEndpointRouteBuilder`.
- Return `TypedResults` (not `Results`) for strongly-typed responses.
- Use `RouteGroupBuilder` to share prefixes and tags.

```csharp
public static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/products").WithTags("Products");

        group.MapGet("/", GetAllProducts);
        group.MapGet("/{id:int}", GetProductById);
        group.MapPost("/", CreateProduct);
        group.MapPut("/{id:int}", UpdateProduct);
        group.MapDelete("/{id:int}", DeleteProduct);

        return app;
    }

    static async Task<Ok<IEnumerable<Product>>> GetAllProducts(IProductService service)
        => TypedResults.Ok(await service.GetAllAsync());

    static async Task<Results<Ok<Product>, NotFound>> GetProductById(int id, IProductService service)
    {
        var product = await service.GetByIdAsync(id);
        return product is null ? TypedResults.NotFound() : TypedResults.Ok(product);
    }

    static async Task<Results<Created<Product>, ValidationProblem>> CreateProduct(
        Product product, IProductService service)
    {
        if (string.IsNullOrWhiteSpace(product.Name))
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                { "Name", ["Name is required."] }
            });

        var created = await service.CreateAsync(product);
        return TypedResults.Created($"/products/{created.Id}", created);
    }
}
```

## Validation

- Use `FluentValidation` or data annotations with a validation filter.
- Register a global validation endpoint filter to auto-validate request bodies.

```csharp
// Register once in Program.cs
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Apply to endpoints
group.MapPost("/", CreateProduct).AddEndpointFilter<ValidationFilter<Product>>();
```

## Error Handling

- Use `app.UseExceptionHandler()` with a problem details response.
- Use `app.MapFallback()` to handle unknown routes.

```csharp
app.UseExceptionHandler(errorApp =>
    errorApp.Run(async ctx =>
    {
        ctx.Response.StatusCode = 500;
        await ctx.Response.WriteAsJsonAsync(new { error = "An unexpected error occurred." });
    }));
```

## OpenAPI

- Always add `.WithName()`, `.WithSummary()`, and `.Produces<T>()` to each endpoint for good OpenAPI docs.

```csharp
group.MapGet("/{id:int}", GetProductById)
    .WithName("GetProductById")
    .WithSummary("Get a product by ID")
    .Produces<Product>()
    .Produces(404);
```

## Testing

- Prefer `WebApplicationFactory<Program>` for integration tests.
- Use `HttpClient` from the factory to test actual endpoint behavior.

```csharp
public class ProductEndpointsTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task GetProducts_ReturnsOk()
    {
        var client = factory.CreateClient();
        var response = await client.GetAsync("/products");
        response.EnsureSuccessStatusCode();
    }
}
```
