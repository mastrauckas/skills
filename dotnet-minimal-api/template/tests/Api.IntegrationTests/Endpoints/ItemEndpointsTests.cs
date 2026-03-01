using System.Net;
using System.Net.Http.Json;
using MyMinimalWebApp.Api.Dtos;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Api.IntegrationTests.Endpoints;

public class ItemEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ItemEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllItems_ReturnsOkWithItems()
    {
        // Act
        HttpResponseMessage response = await _client.GetAsync("/api/items");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        IEnumerable<ItemDto>? items = await response.Content.ReadFromJsonAsync<IEnumerable<ItemDto>>();
        Assert.NotNull(items);
        Assert.NotEmpty(items);
    }

    [Fact]
    public async Task GetItemById_WithValidId_ReturnsOkWithItem()
    {
        // First get all items to find a valid ID
        HttpResponseMessage getAllResponse = await _client.GetAsync("/api/items");
        IEnumerable<ItemDto>? items = await getAllResponse.Content.ReadFromJsonAsync<IEnumerable<ItemDto>>();
        int? itemId = items?.First().Id;
        
        if (itemId == null)
            return;

        // Act
        HttpResponseMessage response = await _client.GetAsync($"/api/items/{itemId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        ItemDto? item = await response.Content.ReadFromJsonAsync<ItemDto>();
        Assert.NotNull(item);
        Assert.Equal(itemId, item.Id);
    }

    [Fact]
    public async Task GetItemById_WithInvalidId_ReturnsNotFound()
    {
        // Act
        HttpResponseMessage response = await _client.GetAsync("/api/items/999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateItem_WithValidRequest_ReturnsCreatedWithItem()
    {
        // Arrange
        var request = new { name = "New item", description = "A new item" };

        // Act
        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/items", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        ItemDto? item = await response.Content.ReadFromJsonAsync<ItemDto>();
        Assert.NotNull(item);
        Assert.Equal("New item", item.Name);
        Assert.Equal("A new item", item.Description);
    }

    [Fact]
    public async Task CreateItem_WithoutName_ReturnsBadRequest()
    {
        // Arrange
        var request = new { name = "", description = "A new item" };

        // Act
        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/items", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateItem_WithoutDescription_ReturnsBadRequest()
    {
        // Arrange
        var request = new { name = "New item", description = "" };

        // Act
        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/items", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateItem_WithValidRequest_ReturnsOkWithUpdatedItem()
    {
        // First get all items to find a valid ID
        HttpResponseMessage getAllResponse = await _client.GetAsync("/api/items");
        IEnumerable<ItemDto>? items = await getAllResponse.Content.ReadFromJsonAsync<IEnumerable<ItemDto>>();
        int? itemId = items?.First().Id;
        
        if (itemId == null)
            return;

        // Arrange
        var request = new { name = "Updated item", description = "Updated description" };

        // Act
        HttpResponseMessage response = await _client.PutAsJsonAsync($"/api/items/{itemId}", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        ItemDto? item = await response.Content.ReadFromJsonAsync<ItemDto>();
        Assert.NotNull(item);
        Assert.Equal("Updated item", item.Name);
        Assert.Equal("Updated description", item.Description);
    }

    [Fact]
    public async Task UpdateItem_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var request = new { name = "Updated item", description = "Updated description" };

        // Act
        HttpResponseMessage response = await _client.PutAsJsonAsync("/api/items/999", request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateItem_WithoutName_ReturnsBadRequest()
    {
        // First get all items to find a valid ID
        HttpResponseMessage getAllResponse = await _client.GetAsync("/api/items");
        IEnumerable<ItemDto>? items = await getAllResponse.Content.ReadFromJsonAsync<IEnumerable<ItemDto>>();
        int? itemId = items?.First().Id;
        
        if (itemId == null)
            return;

        // Arrange
        var request = new { name = "", description = "Updated description" };

        // Act
        HttpResponseMessage response = await _client.PutAsJsonAsync($"/api/items/{itemId}", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DeleteItem_WithValidId_ReturnsNoContent()
    {
        // First, get all items to know what ID to delete
        HttpResponseMessage getResponse = await _client.GetAsync("/api/items");
        IEnumerable<ItemDto>? items = await getResponse.Content.ReadFromJsonAsync<IEnumerable<ItemDto>>();
        ItemDto? itemToDelete = items?.First();
        
        if (itemToDelete == null)
            return;

        // Act
        HttpResponseMessage response = await _client.DeleteAsync($"/api/items/{itemToDelete.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify it's deleted
        HttpResponseMessage getAfterDelete = await _client.GetAsync($"/api/items/{itemToDelete.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getAfterDelete.StatusCode);
    }

    [Fact]
    public async Task DeleteItem_WithInvalidId_ReturnsNotFound()
    {
        // Act
        HttpResponseMessage response = await _client.DeleteAsync("/api/items/999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
