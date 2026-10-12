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
    private readonly AppDbContext _db;

    public MenuService(AppDbContext db) => _db = db;

    private static MenuItemDto ToDto(MenuItem m) =>
        new(m.Id, m.Name, m.Price, m.Category, m.Available);

    public async Task<IEnumerable<MenuItemDto>> GetAllAsync() =>
        await _db.MenuItems.OrderBy(m => m.Id)
                 .Select(m => ToDto(m)).ToListAsync();

    public async Task<MenuItemDto?> GetByIdAsync(int id)
    {
        var m = await _db.MenuItems.FindAsync(id);
        return m is null ? null : ToDto(m);
    }

    public async Task<MenuItemDto> CreateAsync(CreateMenuItemDto dto)
    {
        var item = new MenuItem
        {
            Name = dto.Name,          // Id is assigned by the database
            Price = dto.Price,
            Category = dto.Category,
            Available = true
        };
        _db.MenuItems.Add(item);
        await _db.SaveChangesAsync();
        return ToDto(item);
    }

    public async Task<bool> UpdateAsync(int id, CreateMenuItemDto dto)
    {
        var m = await _db.MenuItems.FindAsync(id);
        if (m is null) return false; // guard clause

        m.Name = dto.Name; // replaces the editable fields
        m.Price = dto.Price;
        m.Category = dto.Category;
        await _db.SaveChangesAsync();
        return true;
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
