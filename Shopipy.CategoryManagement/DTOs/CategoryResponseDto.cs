namespace Shopipy.CategoryManagement.DTOs;

public class CategoryResponseDto
{
    public required int BusinessId { get; init; }

    public required int CategoryId { get; init; }

    public required string Name { get; init; }
}