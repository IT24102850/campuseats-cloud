using CampusEats.Api.Dtos;

namespace CampusEats.Api.Services;

public interface IMenuService
{
    Task<IEnumerable<MenuItemDto>> GetAllAsync();
    Task<MenuItemDto?> GetByIdAsync(int id);
    Task<MenuItemDto> CreateAsync(CreateMenuItemDto dto);
    Task<bool> UpdateAsync(int id, CreateMenuItemDto dto);
    Task<bool> DeleteAsync(int id);
}
