namespace MyMinimalWebApp.Api.Dtos;

public record CreateItemRequest(
    [Required] string Name,
    [Required] string Description);
