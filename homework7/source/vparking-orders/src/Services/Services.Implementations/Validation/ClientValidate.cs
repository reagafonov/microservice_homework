using Domain.Entities;
using Services.Contracts;

namespace Services.Implementations.Validation;

public class ClientValidate : ValidationBase<OrderDto>
{
    protected override void CheckErrors(OrderDto dto)
    {
        CheckRequired(dto.Data, nameof(dto.Data));
        CheckRequired(dto.ClientID, nameof(dto.ClientID));
        CheckRequired(dto.Amount,nameof(dto.Amount));
        CheckStringLength(dto.Data, DomainConstraints.OrderDataMaxLength, nameof(dto.Data));
        CheckStringLength(dto.ClientID, DomainConstraints.ClientIDMaxLength, nameof(dto.ClientID));
        CheckStringLength(dto.Email, DomainConstraints.EmailMaxLength, nameof(dto.Email));
        CheckEmail(dto.Email, nameof(dto.Email));
    }
}