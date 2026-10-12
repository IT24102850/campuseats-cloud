using CampusEats.Api.Dtos;
using CampusEats.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace CampusEats.Api.Controllers;

// GET /api/orders?studentId=IT22045678&_sort=placedAt&_order=desc
[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _svc;

    public OrdersController(IOrderService svc) => _svc = svc;

    [HttpGet] // GET /api/orders
    public async Task<ActionResult<IEnumerable<OrderDto>>> Get(
        [FromQuery(Name = "studentId")] string? studentId,
        [FromQuery(Name = "_sort")] string? sort,
        [FromQuery(Name = "_order")] string? order)
        => Ok(await _svc.GetAsync(studentId, sort, order));

    [HttpGet("{id:int}")] // GET /api/orders/101
    public async Task<ActionResult<OrderDto>> GetById(int id)
    {
        var order = await _svc.GetByIdAsync(id);
        return order is null ? NotFound() : Ok(order);
    }

    [HttpPost] // POST /api/orders
    public async Task<ActionResult<OrderDto>> Create([FromBody] CreateOrderDto dto)
    {
        var created = await _svc.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById),
            new { id = created.Id }, created); // 201
    }

    // PATCH is how the frontend cancels an order: { "status": "Cancelled" }
    [HttpPatch("{id:int}")] // PATCH /api/orders/101
    public async Task<ActionResult<OrderDto>> Patch(
        int id, [FromBody] PatchOrderDto dto)
    {
        try
        {
            var updated = await _svc.PatchAsync(id, dto);
            return updated is null ? NotFound() : Ok(updated);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
