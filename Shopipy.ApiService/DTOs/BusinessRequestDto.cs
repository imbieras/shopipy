using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Shopipy.ApiService.Models;

namespace Shopipy.ApiService.DTOs;

public class BusinessRequestDto
{
    [Required]
    [StringLength(255)]
    public string BusinessName { get; set; }

    [Required]
    [StringLength(500)]
    public string BusinessAddress { get; set; }

    [Required]
    [StringLength(50)]
    public string BusinessVatNumber { get; set; }

    [Required]
    [EmailAddress]
    [StringLength(255)]
    public string BusinessEmail { get; set; }

    [Required]
    [Phone]
    [StringLength(20)]
    public string BusinessPhone { get; set; }
    
    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter))]    
    public BusinessType BusinessType { get; set; }
}