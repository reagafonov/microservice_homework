using System;
using AutoMapper;
using Common.Billing;
using Domain.Entities;
using Services.Abstractions;
using Services.Contracts;
using Services.Repositories.Abstractions;

namespace Services.Implementations.Mapping;

/// <summary>
/// Профиль автомаппера для сущности клиента.
/// </summary>
public class BillingMappingsProfile : Profile
{
    public BillingMappingsProfile()
    {
        CreateMap<Account, BillingDto>().ReverseMap();

        CreateMap<BillingDto, Account>()
            .ForMember(order => order.IsDeleted, expression => expression.Ignore());

        CreateMap<AccountFilterDto, AccountFilter>();

        CreateMap<PaymentDto, AccountFilter>()
            .ForMember(order => order.IsDeleted, expression => expression.Ignore());
        
        CreateMap<ChangeBallanceDto, AccountFilter>()
            .ForMember(order => order.IsDeleted, expression => expression.Ignore());

        CreateMap<NewUserEventMessage, NewUserDto>();

        CreateMap<NewUserDto, Account>()
            .ForMember(account => account.Id, expression => expression.MapFrom(dto => Guid.NewGuid()))
            .ForMember(account => account.Amount, expression => expression.MapFrom(dto => 0))
            .ForMember(account => account.IsDeleted, expression => expression.MapFrom(dto => false));
    }
}