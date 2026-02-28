namespace Api.Services;

public class ItemService : IItemService
{
    private static readonly List<ItemDto> _items = new()
    {
        new ItemDto(1, "Item 1", "First item"),
        new ItemDto(2, "Item 2", "Second item"),
    };

    private static int _nextId = 3;

    public Task<IEnumerable<ItemDto>> GetAllAsync()
    {
        return Task.FromResult(_items.AsEnumerable());
    }

    public Task<ItemDto?> GetByIdAsync(int id)
    {
        var item = _items.FirstOrDefault(x => x.Id == id);
        return Task.FromResult(item);
    }

    public Task<ItemDto> CreateAsync(ItemDto item)
    {
        var newItem = new ItemDto(_nextId++, item.Name, item.Description);
        _items.Add(newItem);
        return Task.FromResult(newItem);
    }

    public Task<ItemDto?> UpdateAsync(
        int id,
        ItemDto item)
    {
        var existingItem = _items.FirstOrDefault(x => x.Id == id);
        if (existingItem is null)
            return Task.FromResult<ItemDto?>(null);

        var updatedItem = new ItemDto(id, item.Name, item.Description);
        var index = _items.FindIndex(x => x.Id == id);
        _items[index] = updatedItem;
        return Task.FromResult<ItemDto?>(updatedItem);
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
