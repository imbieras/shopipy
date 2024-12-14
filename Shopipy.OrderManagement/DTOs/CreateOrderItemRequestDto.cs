using System.Text.Json.Serialization;
using Shopipy.OrderManagement.Mappings;

namespace Shopipy.OrderManagement.DTOs;

[JsonConverter(typeof(CreateOrderItemRequestDtoJsonConverter))]
public abstract class CreateOrderItemRequestDto
{
    public int? TaxRateId { get; set; }
}