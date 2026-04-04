# Claude Instructions

## Language and Framework

This project uses C# 14 and .NET 10. Always prefer features from these versions over older
alternatives.

## Code Style

Always use extension blocks over extension methods. Lines must never exceed 80 characters. Each
class must be in its own file. Prefer primary constructors over traditional constructors. Always use
`init` over `set` for properties.

When declaring or calling with more than one parameter, each must be on its own line:

```csharp
public record ItemDto(
    int Id,
    string Name,
    string Description);
```

For HTTP handler return types, expand generic type arguments onto separate indented lines:

```csharp
private static async Task<
    Results<
        Ok<ItemDto>,
        NotFound>
    > UpdateItem(
```

Method chaining is fine when each call is short and reads naturally. When chains are long, break
each call onto its own line:

```csharp
var result = items
    .Where(x => x.IsActive)
    .OrderBy(x => x.Name)
    .Select(x => new ItemDto(x.Id, x.Name));
```

## Usings

Never add `using` directives at the top of individual C# files. Always add them to `GlobalUsings.cs`
instead.

## DTOs

DTOs must be records and placed in the `Dtos` directory. Never use default parameter values in
records. The multi-parameter rule applies — each property on its own line regardless of count:

```csharp
public record ItemDto(
    int Id,
    string Name,
    string Description);
```
