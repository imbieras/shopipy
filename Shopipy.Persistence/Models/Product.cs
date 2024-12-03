using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Shopipy.Persistence.Models;

public class Product
{
    [Key]
    public int ProductId { get; set; }

    [ForeignKey("Business")]
    public required int BusinessId { get; set; }

    [ForeignKey("Category")]
    public required int CategoryId { get; set; }

    [MaxLength(255)]
    public required string Name { get; set; }

    [MaxLength(255)]
    public required string Description { get; set; }

    public required decimal BasePrice { get; set; }

    public required ProductState ProductState { get; set; }

    public required DateTime CreatedAt { get; set; }

    public required DateTime UpdatedAt { get; set; }

}
