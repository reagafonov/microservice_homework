using AutoMapper;
using Services.Abstractions;
using Services.Contracts;
using VParkingBilling.Models;

namespace VParkingBilling.Mapping;

/// <summary>
/// Профиль автомаппера для сущности урока.
/// </summary>
public class AccountMappingsProfile : Profile
{
    public AccountMappingsProfile()
    {
        CreateMap<AccountDto, AccountOutputModel>();
        CreateMap<AccountInputModel, AccountDto>()
            .ForMember(dto => dto.Id, expression => expression.Ignore())
            .ForMember(order => order.ClientID, expression => expression.Ignore());

        CreateMap<AccountFilterModel, AccountFilterDto>();
    }
}