using System.ComponentModel.DataAnnotations;

namespace Shopipy.Persistence.Models;

public class BaseCategory
{
    [Key]
    public required int CategoryId { get; set; }
    [StringLength(255)]
    public required string Name { get; set; }
    [StringLength(1000)]
    public required string Description { get; set; }
    public required decimal BasePrice { get; set; }
    public required decimal Amount { get; set; }
}