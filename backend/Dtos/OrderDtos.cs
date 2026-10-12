using System.ComponentModel.DataAnnotations;

namespace CampusEats.Api.Dtos;

public record OrderDto(
    int Id, string StudentId, string StudentName, string Items,
    decimal Total, string Status, DateTime PlacedAt);

public record CreateOrderDto(
    [Required, StringLength(20)] string StudentId,
    [Required, StringLength(80)] string StudentName,
    [Required, StringLength(400)] string Items,
    [Range(0, 1000000)] decimal Total);

// PATCH /api/orders/{id} -- the frontend sends { "status": "Cancelled" }
public record PatchOrderDto([StringLength(20)] string? Status);
