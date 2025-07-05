using AutoMapper;
using Services.Abstractions;
using Services.Contracts;
using VParkingNotification.Models;

namespace VParkingNotification.Mapping;

/// <summary>
/// Профиль автомаппера для сущности урока.
/// </summary>
public class AccountMappingsProfile : Profile
{
    public AccountMappingsProfile()
    {
        CreateMap<NotificationDto, NotificationOutputModel>();

        CreateMap<AccountFilterModel, AccountFilterDto>();
    }
}