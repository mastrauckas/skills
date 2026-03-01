# .NET Minimal API Template

A production-ready .NET 10 Minimal API template with a clean, consistent structure for building REST
APIs.

## Project Structure

```
src/
  Api/
    Configuration/
      BuilderConfiguration.cs   # Service registration (builder.ConfigureBuilder())
      AppConfiguration.cs       # Middleware pipeline (app.ConfigureApp())
    Dtos/
      ItemDto.cs                 # Data transfer objects
      ItemRequests.cs            # Request records (CreateItemRequest, UpdateItemRequest)
    Endpoints/
      HttpRoutes.cs              # Route root group (api/)
      ItemEndpoints.cs           # Item CRUD endpoints
    Logging/
      Log.cs                     # Centralized LoggerMessage source generators
    Middleware/
      ExceptionMiddleware.cs     # Global unhandled exception handler
    Services/
      IItemService.cs            # Service interface
      ItemService.cs             # In-memory implementation (replace with real persistence)
    GlobalUsings.cs              # All global using statements
    Program.cs                   # Entry point (4 lines)

tests/
  Api.IntegrationTests/          # Full HTTP pipeline tests via WebApplicationFactory
  Api.UnitTests/                 # Unit tests for services using NSubstitute + Bogus
```

## Conventions

- All `using` statements live in `GlobalUsings.cs` only
- 2+ method parameters → each on its own line
- Method chains with 2+ dots → each `.` on its own line
- C# 14 extension blocks for all extension methods
- File-scoped namespaces throughout
- `TypedResults` (not `Results`) for strongly-typed endpoint responses

## Getting Started

1. Clone or copy this template
2. Rename `Api` to your project name throughout
3. Replace `ItemDto` / `ItemService` with your domain models and services
4. Configure `RegisterDatabase()` in `BuilderConfiguration.cs`
5. Update `appsettings.json` with your configuration

## Key Customization Points

| File                                                   | What to change                        |
| ------------------------------------------------------ | ------------------------------------- |
| `BuilderConfiguration.cs` → `RegisterDatabase()`       | Add EF Core / Dapper / other DB setup |
| `BuilderConfiguration.cs` → `RegisterServices()`       | Register your feature services        |
| `BuilderConfiguration.cs` → `RegisterAuthentication()` | Uncomment and configure JWT Bearer    |
| `BuilderConfiguration.cs` → `RegisterRateLimiting()`   | Uncomment and configure rate limits   |
| `Directory.Packages.props`                             | Update NuGet package versions         |
| `global.json`                                          | Update SDK version pin                |

## Running Tests

```bash
dotnet test
```

## Coverage

```bash
dotnet test --collect:"XPlat Code Coverage"
```
