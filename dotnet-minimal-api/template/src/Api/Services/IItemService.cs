namespace Api.Services;

public interface IItemService
{
    Task<IEnumerable<ItemDto>> GetAllAsync();
    Task<ItemDto?> GetByIdAsync(int id);
    Task<ItemDto> CreateAsync(ItemDto item);
    Task<ItemDto?> UpdateAsync(
        int id,
        ItemDto item);
    Task<bool> DeleteAsync(int id);
}
