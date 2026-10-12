namespace CampusEats.Api.Dtos;

// Output shape — the public contract returned to clients.
public record MenuItemDto(
    int Id, string Name, decimal Price,
    string Category, bool Available);
