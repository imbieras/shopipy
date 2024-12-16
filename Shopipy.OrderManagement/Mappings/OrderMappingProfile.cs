using AutoMapper;
using Shopipy.OrderManagement.DTOs;
using Shopipy.Persistence.Models;

namespace Shopipy.OrderManagement.Mappings;

public class OrderMappingProfile : Profile
{
    public OrderMappingProfile()
    {
        CreateMap<CreateOrderItemRequestDto, OrderItem>()
            .ForMember(dest => dest.OrderItemId, opt => opt.Ignore())
            .ForMember(dest => dest.BusinessId, opt => opt.Ignore())
            .ForMember(dest => dest.OrderId, opt => opt.Ignore())
            .ForMember(dest => dest.UnitPrice, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.TaxRateId, opt => opt.Ignore());
        
        CreateMap<CreateProductOrderItemRequestDto, ProductOrderItem>()
            .IncludeBase<CreateOrderItemRequestDto, OrderItem>();
        
        CreateMap<CreateServiceOrderItemRequestDto, ServiceOrderItem>()
            .IncludeBase<CreateOrderItemRequestDto, OrderItem>();
        
    }
}