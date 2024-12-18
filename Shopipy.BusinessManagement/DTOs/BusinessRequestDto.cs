using System.ComponentModel.DataAnnotations;
using Shopipy.Persistence.Models;

namespace Shopipy.BusinessManagement.DTOs;

public class BusinessRequestDto
{
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
}