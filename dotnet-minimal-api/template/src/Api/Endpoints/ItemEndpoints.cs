namespace Api.Endpoints;

public static class ItemEndpointExtensions
{
    extension(IEndpointRouteBuilder app)
    {
        public IEndpointRouteBuilder MapItemEndpoints()
        {
            var group = app.MapGroup("/items").WithTags("Items");

            group.MapGet("/", GetAllItems)
                .WithName("GetAllItems")
                .WithSummary("Get all items")
                .Produces<IEnumerable<Item>>(StatusCodes.Status200OK);

            group.MapGet("/{id:int}", GetItemById)
                .WithName("GetItemById")
                .WithSummary("Get an item by ID")
                .Produces<Item>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);

            group.MapPost("/", CreateItem)
                .WithName("CreateItem")
                .WithSummary("Create a new item")
                .Produces<Item>(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest);

            group.MapPut("/{id:int}", UpdateItem)
                .WithName("UpdateItem")
                .WithSummary("Update an item")
                .Produces<Item>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status400BadRequest);

            group.MapDelete("/{id:int}", DeleteItem)
                .WithName("DeleteItem")
                .WithSummary("Delete an item")
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound);

            return app;
        }
    }

    private static async Task<Ok<IEnumerable<Item>>> GetAllItems(IItemService service)
    {
        var items = await service.GetAllAsync();
        return TypedResults.Ok(items);
    }

    private static async Task<Results<Ok<Item>, NotFound>> GetItemById(int id, IItemService service)
    {
        var item = await service.GetByIdAsync(id);
        return item is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(item);
    }

    private static async Task<Results<Created<Item>, BadRequest<string>>> CreateItem(
        CreateItemRequest request, IItemService service)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return TypedResults.BadRequest("Name is required.");

        if (string.IsNullOrWhiteSpace(request.Description))
            return TypedResults.BadRequest("Description is required.");

        var item = new Item(0, request.Name, request.Description);
        var created = await service.CreateAsync(item);
        return TypedResults.Created($"/items/{created.Id}", created);
    }

    private static async Task<Results<Ok<Item>, NotFound, BadRequest<string>>> UpdateItem(
        int id, UpdateItemRequest request, IItemService service)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return TypedResults.BadRequest("Name is required.");

        if (string.IsNullOrWhiteSpace(request.Description))
            return TypedResults.BadRequest("Description is required.");

        var item = new Item(0, request.Name, request.Description);
        var updated = await service.UpdateAsync(id, item);
        return updated is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(updated);
    }

    private static async Task<Results<NoContent, NotFound>> DeleteItem(int id, IItemService service)
    {
        var deleted = await service.DeleteAsync(id);
        return deleted
            ? TypedResults.NoContent()
            : TypedResults.NotFound();
    }
}

public record CreateItemRequest(string Name, string Description);
public record UpdateItemRequest(string Name, string Description);

