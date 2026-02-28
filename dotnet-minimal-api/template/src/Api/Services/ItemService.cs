namespace Api.Services;

public class ItemService: IItemService
{
    private static readonly List<Item> _items = new()
    {
        new Item(1, "Item 1", "First item"),
        new Item(2, "Item 2", "Second item"),
    };

    private static int _nextId = 3;

    public Task<IEnumerable<Item>> GetAllAsync()
    {
        return Task.FromResult(_items.AsEnumerable());
    }

    public Task<Item?> GetByIdAsync(int id)
    {
        var item = _items.FirstOrDefault(x => x.Id == id);
        return Task.FromResult(item);
    }

    public Task<Item> CreateAsync(Item item)
    {
        var newItem = new Item(_nextId++, item.Name, item.Description);
        _items.Add(newItem);
        return Task.FromResult(newItem);
    }

    public Task<Item?> UpdateAsync(
        int id,
        Item item)
    {
        var existingItem = _items.FirstOrDefault(x => x.Id == id);
        if (existingItem is null)
            return Task.FromResult<Item?>(null);

        var updatedItem = new Item(id, item.Name, item.Description);
        var index = _items.FindIndex(x => x.Id == id);
        _items[index] = updatedItem;
        return Task.FromResult<Item?>(updatedItem);
    }

    public Task<bool> DeleteAsync(int id)
    {
        var item = _items.FirstOrDefault(x => x.Id == id);
        if (item is null)
            return Task.FromResult(false);

        _items.Remove(item);
        return Task.FromResult(true);
    }
}

public static class ItemServiceExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddItemServices()
        {
            services.AddSingleton<IItemService, ItemService>();
            return services;
        }
    }
}
