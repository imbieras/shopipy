using System.Text.Json.Serialization;
using Shopipy.Persistence.Models;

namespace Shopipy.BusinessManagement.DTOs;

public class BusinessResponseDto
{
    public int BusinessId { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string VATNumber { get; set; }
    public BusinessType BusinessType { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }   
}