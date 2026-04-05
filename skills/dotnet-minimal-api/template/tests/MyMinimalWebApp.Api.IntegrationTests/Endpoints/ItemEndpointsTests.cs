namespace MyMinimalWebApp.Api.IntegrationTests.Endpoints;

public class ItemEndpointsTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetAllItems_ReturnsOkWithItems()
    {
        var response = await _client.GetAsync("/api/items");

        Assert.Equal(HttpStatusCode.OK,
            response.StatusCode);
        var items = await response.Content
            .ReadFromJsonAsync<IEnumerable<ItemDto>>();
        Assert.NotNull(items);
        Assert.NotEmpty(items);
    }

    [Fact]
    public async Task GetItemById_WithValidId_ReturnsOkWithItem()
    {
        // Arrange
        var createRequest = new
        {
            name = "Test item",
            description = "Test description"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/items",
            createRequest);
        var created = await createResponse.Content
            .ReadFromJsonAsync<ItemDto>();

        // Act
        var response = await _client.GetAsync($"/api/items/{created!.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK,
            response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Snapshot.Match(body,
            matchOptions => matchOptions.IgnoreField("id"));
    }

    [Fact]
    public async Task GetItemById_WithInvalidId_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/items/999");

        Assert.Equal(HttpStatusCode.NotFound,
            response.StatusCode);
    }

    [Fact]
    public async Task CreateItem_WithValidRequest_ReturnsCreatedWithItem()
    {
        // Arrange
        var request = new { name = "New item", description = "A new item" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/items",
            request);

        // Assert
        Assert.Equal(HttpStatusCode.Created,
            response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Snapshot.Match(body,
            matchOptions => matchOptions.IgnoreField("id"));
    }

    [Fact]
    public async Task CreateItem_WithEmptyName_ReturnsBadRequest()
    {
        // Arrange
        var request = new { name = "", description = "A new item" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/items",
            request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    [Fact]
    public async Task CreateItem_WithEmptyDescription_ReturnsBadRequest()
    {
        // Arrange
        var request = new { name = "New item", description = "" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/items",
            request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    [Fact]
    public async Task UpdateItem_WithValidRequest_ReturnsOkWithUpdatedItem()
    {
        // Arrange
        var createRequest = new
        {
            name = "Original item",
            description = "Original description"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/items",
            createRequest);
        var created = await createResponse.Content
            .ReadFromJsonAsync<ItemDto>();
        var updateRequest = new
        {
            name = "Updated item",
            description = "Updated description"
        };

        // Act
        var response = await _client.PutAsJsonAsync(
            $"/api/items/{created!.Id}",
            updateRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK,
            response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Snapshot.Match(body,
            matchOptions => matchOptions.IgnoreField("id"));
    }

    [Fact]
    public async Task UpdateItem_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var request = new
        {
            name = "Updated item",
            description = "Updated description"
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/items/999",
            request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound,
            response.StatusCode);
    }

    [Fact]
    public async Task UpdateItem_WithEmptyName_ReturnsBadRequest()
    {
        // Arrange
        var request = new { name = "", description = "Updated description" };

        // Act
        var response = await _client.PutAsJsonAsync("/api/items/1",
            request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    [Fact]
    public async Task DeleteItem_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var createRequest = new
        {
            name = "Item to delete",
            description = "Will be deleted"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/items",
            createRequest);
        var created = await createResponse.Content
            .ReadFromJsonAsync<ItemDto>();

        // Act
        var response = await _client.DeleteAsync($"/api/items/{created!.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent,
            response.StatusCode);
        var getAfterDelete = await _client
            .GetAsync($"/api/items/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound,
            getAfterDelete.StatusCode);
    }

    [Fact]
    public async Task DeleteItem_WithInvalidId_ReturnsNotFound()
    {
        // Act
        var response = await _client.DeleteAsync("/api/items/999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound,
            response.StatusCode);
    }
}
