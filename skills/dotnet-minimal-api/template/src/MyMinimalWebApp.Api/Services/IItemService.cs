namespace MyMinimalWebApp.Api.Services;

public interface IItemService
{
    public Task<IEnumerable<ItemDto>> GetAllAsync();
    public Task<ItemDto?> GetByIdAsync(int id);
    public Task<ItemDto> CreateAsync(ItemDto item);
    public Task<ItemDto?> UpdateAsync(int id,
        ItemDto item);
    public Task<bool> DeleteAsync(int id);
}
