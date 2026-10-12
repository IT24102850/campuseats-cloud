using CampusEats.Api.Dtos;
using CampusEats.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusEats.Api.Controllers;

[ApiController] // auto 400 on invalid body
[Route("api/[controller]")] // -> /api/menu
public class MenuController : ControllerBase
{
    private readonly IMenuService _svc;

    public MenuController(IMenuService svc)
        => _svc = svc; // injected by DI

    [HttpGet, AllowAnonymous] // GET /api/menu  (public: the storefront)
    public async Task<ActionResult<IEnumerable<MenuItemDto>>> GetAll()
        => Ok(await _svc.GetAllAsync());

    [HttpGet("{id}"), AllowAnonymous] // GET /api/menu/1
    public async Task<ActionResult<MenuItemDto>> GetById(int id)
    {
        var item = await _svc.GetByIdAsync(id);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost, Authorize] // POST /api/menu  (staff only -> needs a JWT)
    public async Task<ActionResult<MenuItemDto>> Create([FromBody] CreateMenuItemDto dto)
    {
        var created = await _svc.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById),
            new { id = created.Id }, created); // 201
    }

    [HttpPut("{id}"), Authorize] // PUT /api/menu/1
    public async Task<IActionResult> Update(int id, [FromBody] CreateMenuItemDto dto)
        => await _svc.UpdateAsync(id, dto) ? NoContent() : NotFound(); // 204/404

    [HttpDelete("{id}"), Authorize] // DELETE /api/menu/1
    public async Task<IActionResult> Delete(int id)
        => await _svc.DeleteAsync(id) ? NoContent() : NotFound(); // 204/404
}
