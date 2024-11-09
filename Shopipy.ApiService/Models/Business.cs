using System.ComponentModel.DataAnnotations;

namespace Shopipy.ApiService.Models;

public class Business
{
    [Key]
    public int BusinessId { get; set; }

    [Required]
    [StringLength(255)]
    public string Name { get; set; }

    [StringLength(500)]
    public string Address { get; set; }

    [Phone]
    [StringLength(20)]
    public string Phone { get; set; }

    [Required]
    [EmailAddress]
    [StringLength(255)]
    public string Email { get; set; }

    [StringLength(50)]
    public string VATNumber { get; set; }

    [Required]
    public BusinessType BusinessType { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public enum BusinessType
{
    Retail,
    Service,
    Manufacturing,
    Wholesale,
    Other
}
