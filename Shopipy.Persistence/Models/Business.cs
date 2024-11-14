using System.ComponentModel.DataAnnotations;

namespace Shopipy.Persistence.Models;

public class Business
{
    [Key]
    public int BusinessId { get; set; }

    [StringLength(255)]
    public required string Name { get; set; }

    [StringLength(500)]
    public required string Address { get; set; }

    [Phone]
    [StringLength(20)]
    public string? Phone { get; set; }

    [EmailAddress]
    [StringLength(255)]
    public required string Email { get; set; }

    [StringLength(50)]
    public string? VatNumber { get; set; }

    public required BusinessType BusinessType { get; set; }

    public required DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public required DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}