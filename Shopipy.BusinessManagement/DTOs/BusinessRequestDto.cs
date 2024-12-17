using System.ComponentModel.DataAnnotations;
using Shopipy.Persistence.Models;

namespace Shopipy.BusinessManagement.DTOs;

public class BusinessRequestDto
{
    [StringLength(255)]
    public required string BusinessName { get; set; }

    [StringLength(500)]
    public required string BusinessAddress { get; set; }

    [StringLength(50)]
    public string? BusinessVatNumber { get; set; }

    [EmailAddress]
    [StringLength(255)]
    public required string BusinessEmail { get; set; }

    [Phone]
    [StringLength(20)]
    public string? BusinessPhone { get; set; }

    public required BusinessType BusinessType { get; set; }
}