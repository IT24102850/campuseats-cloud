using CampusEats.Api.Dtos;
using CampusEats.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusEats.Api.Controllers;

// Route matches the URL shape the React frontend already calls:
// GET /api/menuItems?_page=1&_limit=6&_sort=name&q=kottu&category=Drinks
[ApiController] // auto 400 on invalid body
[Route("api/menuItems")]
public class MenuItemsController : ControllerBase
{
    private readonly IMenuService _svc;

    public MenuItemsController(IMenuService svc)
        => _svc = svc; // injected by DI

    [HttpGet] // GET /api/menuItems
    public async Task<ActionResult<IEnumerable<MenuItemDto>>> Get(
        [FromQuery(Name = "q")] string? q,
        [FromQuery(Name = "category")] string? category,
        [FromQuery(Name = "_sort")] string? sort,
        [FromQuery(Name = "_order")] string? order,
        [FromQuery(Name = "_page")] int? page,
        [FromQuery(Name = "_limit")] int? limit)
    {
        var result = await _svc.GetAsync(
            new MenuQuery(q, category, sort, order, page, limit));

        // The frontend reads the total row count from this header to
        // build its pager. CORS must expose it or the browser hides it.
        Response.Headers["X-Total-Count"] = result.TotalCount.ToString();

        return Ok(result.Items);
    }

    [HttpGet("{id:int}")] // GET /api/menuItems/1
    public async Task<ActionResult<MenuItemDto>> GetById(int id)
    {
        var item = await _svc.GetByIdAsync(id);
        return item is null ? NotFound() : Ok(item);
    }

    // PATCH is how the frontend saves a favourite: { "isFavourite": true }
    [HttpPatch("{id:int}")] // PATCH /api/menuItems/1
    public async Task<ActionResult<MenuItemDto>> Patch(
        int id, [FromBody] PatchMenuItemDto dto)
    {
        var updated = await _svc.PatchAsync(id, dto);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpPost, Authorize] // POST /api/menuItems  (staff only -> needs a JWT)
    public async Task<ActionResult<MenuItemDto>> Create(
        [FromBody] CreateMenuItemDto dto)
    {
        var created = await _svc.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById),
            new { id = created.Id }, created); // 201
    }

    [HttpDelete("{id:int}"), Authorize] // DELETE /api/menuItems/1
    public async Task<IActionResult> Delete(int id)
        => await _svc.DeleteAsync(id) ? NoContent() : NotFound(); // 204/404
}
