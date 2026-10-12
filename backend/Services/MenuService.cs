using CampusEats.Api.Data;
using CampusEats.Api.Dtos;
using CampusEats.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CampusEats.Api.Services;

// Lab 03 kept the menu in a List<MenuItem>. From Lab 04 onwards the
// data lives in PostgreSQL, so the store is the DbContext and the
// service is registered AddScoped (one context per request).
public class MenuService : IMenuService
{
    private const int MaxPageSize = 100;

    private readonly AppDbContext _db;

    public MenuService(AppDbContext db) => _db = db;

    private static MenuItemDto ToDto(MenuItem m) => new(
        m.Id, m.Name, m.Category, m.Price, m.Outlet, m.IsFavourite, m.Available);

    public async Task<PagedResult<MenuItemDto>> GetAsync(MenuQuery query)
    {
        IQueryable<MenuItem> items = _db.MenuItems;

        // Free-text search across the fields a student would search by.
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim();
            items = items.Where(m =>
                EF.Functions.ILike(m.Name, $"%{term}%") ||
                EF.Functions.ILike(m.Category, $"%{term}%") ||
                EF.Functions.ILike(m.Outlet, $"%{term}%"));
        }

        if (!string.IsNullOrWhiteSpace(query.Category) &&
            !query.Category.Equals("All", StringComparison.OrdinalIgnoreCase))
        {
            items = items.Where(m => m.Category == query.Category);
        }

        // The total is counted BEFORE paging, because the client needs
        // the full row count to render the pager.
        var totalCount = await items.CountAsync();

        var descending = string.Equals(query.Order, "desc",
            StringComparison.OrdinalIgnoreCase);

        items = query.Sort?.ToLowerInvariant() switch
        {
            "name" => descending
                ? items.OrderByDescending(m => m.Name)
                : items.OrderBy(m => m.Name),
            "price" => descending
                ? items.OrderByDescending(m => m.Price)
                : items.OrderBy(m => m.Price),
            "category" => descending
                ? items.OrderByDescending(m => m.Category)
                : items.OrderBy(m => m.Category),
            _ => descending
                ? items.OrderByDescending(m => m.Id)
                : items.OrderBy(m => m.Id)
        };

        if (query.Limit is > 0)
        {
            var pageSize = Math.Min(query.Limit.Value, MaxPageSize);
            var page = Math.Max(query.Page ?? 1, 1); // _page is 1-based
            items = items.Skip((page - 1) * pageSize).Take(pageSize);
        }

        var results = await items
            .Select(m => new MenuItemDto(
                m.Id, m.Name, m.Category, m.Price,
                m.Outlet, m.IsFavourite, m.Available))
            .ToListAsync();

        return new PagedResult<MenuItemDto>(results, totalCount);
    }

    public async Task<MenuItemDto?> GetByIdAsync(int id)
    {
        var m = await _db.MenuItems.FindAsync(id);
        return m is null ? null : ToDto(m);
    }

    public async Task<MenuItemDto> CreateAsync(CreateMenuItemDto dto)
    {
        var item = new MenuItem
        {
            Name = dto.Name,              // Id is assigned by the database
            Category = dto.Category,
            Price = dto.Price,
            Outlet = dto.Outlet ?? "",
            Available = dto.Available ?? true
        };
        _db.MenuItems.Add(item);
        await _db.SaveChangesAsync();
        return ToDto(item);
    }

    // PATCH semantics: only the fields present in the body are changed.
    public async Task<MenuItemDto?> PatchAsync(int id, PatchMenuItemDto dto)
    {
        var m = await _db.MenuItems.FindAsync(id);
        if (m is null) return null; // guard clause

        if (dto.Name is not null) m.Name = dto.Name;
        if (dto.Category is not null) m.Category = dto.Category;
        if (dto.Price is not null) m.Price = dto.Price.Value;
        if (dto.Outlet is not null) m.Outlet = dto.Outlet;
        if (dto.IsFavourite is not null) m.IsFavourite = dto.IsFavourite.Value;
        if (dto.Available is not null) m.Available = dto.Available.Value;

        await _db.SaveChangesAsync();
        return ToDto(m);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var m = await _db.MenuItems.FindAsync(id);
        if (m is null) return false;

        _db.MenuItems.Remove(m);
        await _db.SaveChangesAsync();
        return true;
    }
}
