using AutoMapper;
using Domain.Entities;
using Services.Abstractions;
using Services.Contracts;
using Services.Repositories.Abstractions;

namespace Services.Implementations.Mapping;

/// <summary>
/// Профиль автомаппера для сущности клиента.
/// </summary>
public class ClientMappingsProfile : Profile
{
    public ClientMappingsProfile()
    {
        CreateMap<Order, OrderDto>().ReverseMap();

        CreateMap<OrderDto, Order>()
            .ForMember(order => order.IsDeleted, expression => expression.Ignore());

        CreateMap<OrderFilterDto, OrderFilter>();

        CreateMap<OrderDto, PaymentModel>()
            .ForMember(paymentModel=>paymentModel.Price, expression=>expression.MapFrom(x=>x.Amount));
        
        CreateMap<OrderDto, Notification>()
            .ForMember(notificationMessage=>notificationMessage.MessageType, expression=>expression.Ignore())
            .ForMember(notificationMessage=>notificationMessage.OrderId, expression=>expression.MapFrom(order=>order.Id));
        
        CreateMap<Notification, NotificationMessage>();
    }
}