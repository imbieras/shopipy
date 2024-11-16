using System.Text.Json.Serialization;
using Shopipy.Persistence.Models;

namespace Shopipy.BusinessManagement.DTOs;

public class BusinessResponseDto
{
    public int BusinessId { get; set; }
    public required string Name { get; set; }
    public required string Address { get; set; }
    public string? Phone { get; set; }
    public required string Email { get; set; }
    public string? VatNumber { get; set; }
    public required BusinessType BusinessType { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime UpdatedAt { get; set; }   
}