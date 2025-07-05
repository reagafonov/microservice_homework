using Domain.Entities;
using Services.Contracts;

namespace Services.Implementations.Validation;

public class AccountValidate : ValidationBase<AccountDto>
{
    protected override void CheckErrors(AccountDto dto)
    {
        CheckRequired(dto.ClientID, nameof(dto.ClientID));
        CheckRequired(dto.Amount,nameof(dto.Amount));
        CheckStringLength(dto.ClientID, DomainConstraints.ClientIDMaxLength, nameof(dto.ClientID));
    }
}