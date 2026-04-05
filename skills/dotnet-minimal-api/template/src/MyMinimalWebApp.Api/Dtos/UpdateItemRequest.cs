namespace MyMinimalWebApp.Api.Dtos;

public record UpdateItemRequest(
    [Required] string Name,
    [Required] string Description);
