using CampusEats.Api.Dtos;

namespace CampusEats.Api.Services;

public interface IOrderService
{
    Task<IReadOnlyList<OrderDto>> GetAsync(string? studentId, string? sort, string? order);
    Task<OrderDto?> GetByIdAsync(int id);
    Task<OrderDto> CreateAsync(CreateOrderDto dto);
    Task<OrderDto?> PatchAsync(int id, PatchOrderDto dto);
}
