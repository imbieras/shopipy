using System.ComponentModel.DataAnnotations;

namespace Shopipy.Persistence.Models;

public class Category
{
    [Key]
    public required int CategoryId { get; set; }
    [StringLength(255)]
    public required string Name { get; set; }
}