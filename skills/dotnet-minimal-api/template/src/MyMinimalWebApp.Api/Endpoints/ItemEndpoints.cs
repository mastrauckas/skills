namespace MyMinimalWebApp.Api.Endpoints;

public static class ItemEndpointExtensions
{
    extension(WebApplication app)
    {
        public void MapItemEndpoints(RouteGroupBuilder root)
        {
            var group = root
                .MapGroup("/items")
                .WithTags("Items");
                // Uncomment to require authentication for all
                // endpoints in this group:
                // .RequireAuthorization();

            group.MapGet("/",
                    GetAllItems)
                .WithName("ListItems")
                .WithDisplayName("List Items")
                .WithSummary("List all items")
                .WithDescription("Returns all items in the system.");

            group.MapGet("/{id:int}",
                    GetItemById)
                .WithName("GetItem")
                .WithDisplayName("Get Item")
                .WithSummary("Get an item by ID")
                .WithDescription(
                    "Returns a single item matching the given ID, " +
                    "or 404 if not found.");

            group.MapPost("/",
                    CreateItem)
                .WithName("CreateItem")
                .WithDisplayName("Create Item")
                .WithSummary("Create a new item")
                .WithDescription(
                    "Creates a new item and returns the created resource.");

            group.MapPut("/{id:int}",
                    UpdateItem)
                .WithName("UpdateItem")
                .WithDisplayName("Update Item")
                .WithSummary("Update an item")
                .WithDescription(
                    "Updates an existing item by ID, " +
                    "or returns 404 if not found.");

            group.MapDelete("/{id:int}",
                    DeleteItem)
                .WithName("DeleteItem")
                .WithDisplayName("Delete Item")
                .WithSummary("Delete an item")
                .WithDescription(
                    "Deletes an item by ID, or returns 404 if not found.");
        }
    }

#pragma warning disable IDE0051
    private static async Task<
        Ok<IEnumerable<ItemDto>>
        > GetAllItems(IItemService service)
    {
        var items = await service.GetAllAsync();
        return TypedResults.Ok(items);
    }

    private static async Task<
        Results<
            Ok<ItemDto>,
            NotFound>
        > GetItemById(int id,
        IItemService service)
    {
        var item = await service.GetByIdAsync(id);
        return item is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(item);
    }

    private static async Task<
        Created<ItemDto>
        > CreateItem(CreateItemRequest request,
        IItemService service)
    {
        var item = new ItemDto(0,
            request.Name,
            request.Description);
        var created = await service.CreateAsync(item);
        return TypedResults.Created($"/api/items/{created.Id}",
            created);
    }

    private static async Task<
        Results<
            Ok<ItemDto>,
            NotFound>
        > UpdateItem(int id,
        UpdateItemRequest request,
        IItemService service)
    {
        var item = new ItemDto(0,
            request.Name,
            request.Description);
        var updated = await service.UpdateAsync(id,
            item);
        return updated is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(updated);
    }

    private static async Task<
        Results<
            NoContent,
            NotFound>
        > DeleteItem(int id,
        IItemService service)
    {
        var deleted = await service.DeleteAsync(id);
        return deleted
            ? TypedResults.NoContent()
            : TypedResults.NotFound();
    }
#pragma warning restore IDE0051
}

