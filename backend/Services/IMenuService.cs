using CampusEats.Api.Dtos;

namespace CampusEats.Api.Services;

public interface IMenuService
{
    Task<PagedResult<MenuItemDto>> GetAsync(MenuQuery query);
    Task<MenuItemDto?> GetByIdAsync(int id);
    Task<MenuItemDto> CreateAsync(CreateMenuItemDto dto);
    Task<MenuItemDto?> PatchAsync(int id, PatchMenuItemDto dto);
    Task<bool> DeleteAsync(int id);
}

// The search / filter / sort / page options the menu list supports.
public record MenuQuery(
    string? Search, string? Category, string? Sort,
    string? Order, int? Page, int? Limit);
