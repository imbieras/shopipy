using Shopipy.Persistence.Models;

namespace Shopipy.ProductManagement.DTOs;

public class ProductResponseDto
{
    public required int CategoryId { get; init; }

    public required string Name { get; init; }

    public required string Description { get; init; }

    public required decimal BasePrice { get; init; }

    public required ProductState ProductState { get; init; }

    public int ProductId { get; init; }

    public int BusinessId { get; init; }

    public DateTime CreatedAt { get; init; }

    public DateTime UpdatedAt { get; init; }
}