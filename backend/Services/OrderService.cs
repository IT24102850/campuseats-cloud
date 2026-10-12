using CampusEats.Api.Data;
using CampusEats.Api.Dtos;
using CampusEats.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CampusEats.Api.Services;

public class OrderService : IOrderService
{
    private static readonly string[] AllowedStatuses =
        ["Pending", "Preparing", "Ready", "Collected", "Cancelled"];

    private readonly AppDbContext _db;

    public OrderService(AppDbContext db) => _db = db;

    private static OrderDto ToDto(Order o) => new(
        o.Id, o.StudentId, o.StudentName, o.Items,
        o.Total, o.Status, o.PlacedAt);

    public async Task<IReadOnlyList<OrderDto>> GetAsync(
        string? studentId, string? sort, string? order)
    {
        IQueryable<Order> orders = _db.Orders;

        // "My orders" filters by student; the admin view passes nothing.
        if (!string.IsNullOrWhiteSpace(studentId))
            orders = orders.Where(o => o.StudentId == studentId);

        var descending = string.Equals(order, "desc",
            StringComparison.OrdinalIgnoreCase);

        orders = sort?.ToLowerInvariant() switch
        {
            "placedat" => descending
                ? orders.OrderByDescending(o => o.PlacedAt)
                : orders.OrderBy(o => o.PlacedAt),
            "total" => descending
                ? orders.OrderByDescending(o => o.Total)
                : orders.OrderBy(o => o.Total),
            _ => descending
                ? orders.OrderByDescending(o => o.Id)
                : orders.OrderBy(o => o.Id)
        };

        return await orders
            .Select(o => new OrderDto(
                o.Id, o.StudentId, o.StudentName, o.Items,
                o.Total, o.Status, o.PlacedAt))
            .ToListAsync();
    }

    public async Task<OrderDto?> GetByIdAsync(int id)
    {
        var o = await _db.Orders.FindAsync(id);
        return o is null ? null : ToDto(o);
    }

    public async Task<OrderDto> CreateAsync(CreateOrderDto dto)
    {
        var order = new Order
        {
            StudentId = dto.StudentId,
            StudentName = dto.StudentName,
            Items = dto.Items,
            Total = dto.Total,
            Status = "Pending",
            PlacedAt = DateTime.UtcNow
        };
        _db.Orders.Add(order);
        await _db.SaveChangesAsync();
        return ToDto(order);
    }

    public async Task<OrderDto?> PatchAsync(int id, PatchOrderDto dto)
    {
        var o = await _db.Orders.FindAsync(id);
        if (o is null) return null;

        if (dto.Status is not null)
        {
            // Only a known status may be written to the database.
            var match = AllowedStatuses.FirstOrDefault(s =>
                s.Equals(dto.Status, StringComparison.OrdinalIgnoreCase));
            if (match is null)
                throw new ArgumentException(
                    $"Unknown status '{dto.Status}'. Allowed: {string.Join(", ", AllowedStatuses)}");

            o.Status = match;
        }

        await _db.SaveChangesAsync();
        return ToDto(o);
    }
}
