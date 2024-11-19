namespace Shopipy.CategoryManagement.DTOs;

public class CategoryResponseDto
{
    public required int BusinessId { get; set; }
    public required int CategoryId { get; set; }
    public required string Name { get; set; }
}