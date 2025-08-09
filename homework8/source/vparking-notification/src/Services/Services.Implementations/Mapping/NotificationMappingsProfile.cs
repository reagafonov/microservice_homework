using AutoMapper;
using Domain.Entities;
using Services.Contracts;

namespace Services.Implementations.Mapping;

/// <summary>
/// Профиль автомаппера для сущности клиента.
/// </summary>
public class NotificationMappingsProfile : Profile
{
    public NotificationMappingsProfile()
    {
        CreateMap<NotificationMessage, NotificationDto>().ReverseMap();

        CreateMap<NotificationDto, NotificationMessage>();

        CreateMap<NotificationDto, Notification>()
            .ForMember(notification => notification.Id, expression => expression.Ignore())
            .ForMember(notification => notification.IsDeleted, expression => expression.Ignore())
            .ForMember(notification => notification.OrderId, expression => expression.MapFrom(notification=>notification.OrderId))
            .ReverseMap();

    }
}