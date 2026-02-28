using Api.Dtos;
using Api.Services;
using Bogus;

namespace Api.UnitTests.Services;

public class ItemServiceTests
{
    private readonly ItemService _sut = new();
    private readonly Faker<ItemDto> _faker = new Faker<ItemDto>()
        .CustomInstantiator(f => new ItemDto(
            f.IndexFaker + 1,
            f.Commerce.ProductName(),
            f.Commerce.ProductDescription()));

    [Fact]
    public async Task GetAllAsync_ReturnsAllItems()
    {
        var items = await _sut.GetAllAsync();

        Assert.NotNull(items);
        Assert.NotEmpty(items);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsItem()
    {
        var all = (await _sut.GetAllAsync()).ToList();
        var target = all.First();

        var result = await _sut.GetByIdAsync(target.Id);

        Assert.NotNull(result);
        Assert.Equal(target.Id, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        var result = await _sut.GetByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_AddsAndReturnsItem()
    {
        var dto = _faker.Generate();

        var created = await _sut.CreateAsync(dto);

        Assert.NotNull(created);
        Assert.Equal(dto.Name, created.Name);
        Assert.Equal(dto.Description, created.Description);
        Assert.True(created.Id > 0);

        var fetched = await _sut.GetByIdAsync(created.Id);
        Assert.NotNull(fetched);
    }

    [Fact]
    public async Task UpdateAsync_WithValidId_UpdatesAndReturnsItem()
    {
        var all = (await _sut.GetAllAsync()).ToList();
        var target = all.First();
        var updated = _faker.Generate();

        var result = await _sut.UpdateAsync(target.Id, updated);

        Assert.NotNull(result);
        Assert.Equal(target.Id, result.Id);
        Assert.Equal(updated.Name, result.Name);
        Assert.Equal(updated.Description, result.Description);
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidId_ReturnsNull()
    {
        var dto = _faker.Generate();

        var result = await _sut.UpdateAsync(999, dto);

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_RemovesAndReturnsTrue()
    {
        var all = (await _sut.GetAllAsync()).ToList();
        var target = all.First();

        var deleted = await _sut.DeleteAsync(target.Id);

        Assert.True(deleted);
        var fetched = await _sut.GetByIdAsync(target.Id);
        Assert.Null(fetched);
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ReturnsFalse()
    {
        var deleted = await _sut.DeleteAsync(999);

        Assert.False(deleted);
    }
}
