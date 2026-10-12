using System.ComponentModel.DataAnnotations;

namespace CampusEats.Api.Dtos;

// Output shape -- the public contract returned to clients. Serialised
// as camelCase, which is exactly what the React frontend expects
// (id, name, category, price, outlet, isFavourite, available).
public record MenuItemDto(
    int Id, string Name, string Category, decimal Price,
    string Outlet, bool IsFavourite, bool Available);

// Input shape for a create -- no Id (the database assigns it).
public record CreateMenuItemDto(
    [Required, StringLength(80)] string Name,
    [Required, StringLength(40)] string Category,
    [Range(0, 100000)] decimal Price,
    [StringLength(60)] string? Outlet,
    bool? Available);

// Input shape for a PATCH: every field is optional, so the client can
// send just { "isFavourite": true } the way the frontend does.
public record PatchMenuItemDto(
    [StringLength(80)] string? Name,
    [StringLength(40)] string? Category,
    [Range(0, 100000)] decimal? Price,
    [StringLength(60)] string? Outlet,
    bool? IsFavourite,
    bool? Available);

// A page of results plus the total row count, which the controller
// returns to the client in the X-Total-Count header.
public record PagedResult<T>(IReadOnlyList<T> Items, int TotalCount);
