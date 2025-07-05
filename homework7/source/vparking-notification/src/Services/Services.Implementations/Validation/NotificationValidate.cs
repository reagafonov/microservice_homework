using Domain.Entities;
using Services.Contracts;

namespace Services.Implementations.Validation;

public class NotificationValidate : ValidationBase<NotificationDto>
{
    protected override void CheckErrors(NotificationDto dto)
    {
        CheckRequired(dto.ClientID, nameof(dto.ClientID));
        CheckRequired(dto.MessageType,nameof(dto.MessageType));
        CheckRequired(dto.ClientID, nameof(dto.ClientID));
        CheckStringLength(dto.ClientID, DomainConstraints.ClientIDLength, nameof(dto.ClientID));
    }
}