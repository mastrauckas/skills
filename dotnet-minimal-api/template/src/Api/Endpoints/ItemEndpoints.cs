namespace Api.Endpoints;

public static class ItemEndpointExtensions
{
    extension(WebApplication app)
    {
        public void MapItemEndpoints(RouteGroupBuilder root)
        {
            var group = root.MapGroup("/items").WithTags("Items");

            group.MapGet("/", GetAllItems)
                .WithName("ListItems")
                .WithSummary("List all items");

            group.MapGet("/{id:int}", GetItemById)
                .WithName("GetItem")
                .WithSummary("Get an item by ID");

            group.MapPost("/", CreateItem)
                .WithName("CreateItem")
                .WithSummary("Create a new item");

            group.MapPut("/{id:int}", UpdateItem)
                .WithName("UpdateItem")
                .WithSummary("Update an item");

            group.MapDelete("/{id:int}", DeleteItem)
                .WithName("DeleteItem")
                .WithSummary("Delete an item");
        }
    }

    private static async Task<Ok<IEnumerable<ItemDto>>> GetAllItems(IItemService service)
    {
        var items = await service.GetAllAsync();
        return TypedResults.Ok(items);
    }

    private static async Task<Results<Ok<ItemDto>, NotFound>> GetItemById(
        int id,
        IItemService service)
    {
        var item = await service.GetByIdAsync(id);
        return item is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(item);
    }

    private static async Task<Created<ItemDto>> CreateItem(
        CreateItemRequest request,
        IItemService service)
    {
        var item = new ItemDto(0, request.Name, request.Description);
        var created = await service.CreateAsync(item);
        return TypedResults.Created($"/items/{created.Id}", created);
    }

    private static async Task<Results<Ok<ItemDto>, NotFound>> UpdateItem(
        int id,
        UpdateItemRequest request,
        IItemService service)
    {
        var item = new ItemDto(0, request.Name, request.Description);
        var updated = await service.UpdateAsync(id, item);
        return updated is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(updated);
    }

    private static async Task<Results<NoContent, NotFound>> DeleteItem(
        int id,
        IItemService service)
    {
        var deleted = await service.DeleteAsync(id);
        return deleted
            ? TypedResults.NoContent()
            : TypedResults.NotFound();
    }
}

