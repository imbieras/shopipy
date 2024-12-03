using Shopipy.Persistence.Models;

namespace Shopipy.ProductManagement.DTOs
{
    public class ProductResponseDTO
    {
        public required int CategoryId { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required decimal BasePrice { get; set; }
        public required ProductState ProductState { get; set; }
        public int ProductId { get; set; }
        public int BusinessId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
