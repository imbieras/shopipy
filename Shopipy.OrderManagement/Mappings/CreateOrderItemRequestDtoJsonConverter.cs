using System.Text.Json;
using System.Text.Json.Serialization;
using Shopipy.OrderManagement.DTOs;
using Shopipy.Persistence.Models;

namespace Shopipy.OrderManagement.Mappings;

public class CreateOrderItemRequestDtoJsonConverter : JsonConverter<CreateOrderItemRequestDto>
{
    public override CreateOrderItemRequestDto? Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var orderItem = doc.Deserialize<HelperCreateOrderItemRequestDto>();
        if (orderItem is null) throw new JsonException();
        if (orderItem.ProductId is null == orderItem.Serviceid is null)
            throw new JsonException("Either product id or service id must be set");
        if (orderItem.ProductId is not null)
        {
            return doc.Deserialize<CreateProductOrderItemRequestDto>(options);
        }
        return doc.Deserialize<CreateServiceOrderItemRequestDto>(options);
    }

    public override void Write(Utf8JsonWriter writer, CreateOrderItemRequestDto value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
    
    private class HelperCreateOrderItemRequestDto : CreateOrderItemRequestDto
    {
        public int? ProductId { get; set; }
        public int? Serviceid { get; set; }
    }
}