using AutoMapper;
using Services.Abstractions;
using Services.Contracts;
using VParkingSettings.Models;

namespace VParkingSettings.Mapping;

/// <summary>
/// Профиль автомаппера для сущности урока.
/// </summary>
public class OrderMappingsProfile : Profile
{
    public OrderMappingsProfile()
    {
        CreateMap<OrderDto, OrderOutputModel>();
        CreateMap<OrderInputModel, OrderDto>()
            .ForMember(dto => dto.Id, expression => expression.Ignore())
            .ForMember(order => order.IsVerified, expression => expression.Ignore())
            .ForMember(order => order.IsPayed, expression => expression.Ignore())
            .ForMember(order => order.ClientID, expression => expression.Ignore())
            .ForMember(order=>order.Email, expression => expression.Ignore());

        CreateMap<OrderFilterModel, OrderFilterDto>();
    }
}