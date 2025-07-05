using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Common.Abstractions;
using Domain.Entities;
using Infrastructure.Email;
using Services.Abstractions;
using Services.Contracts;
using Services.Repositories.Abstractions;

namespace Services.Implementations;

/// <summary>
/// Сервис работы с клиентами
/// </summary>
public class NotificationService(
    IEmailNotificationSender emailNotificationSender,
    INotificationRepository notificationRepository,
    IMapper mapper) : INotificationService
{
    private const string SuccessBody = "Thank you for your payment!";
    private const string FailedBody = "Payment failed!";
    private const string SuccessSubject = "Payment successful!";
    private const string FailedSubject = "Payment failed!";
    
    public async Task<bool> SendNotification(NotificationDto notificationDto)
    {
        var notification = mapper.Map<Notification>(notificationDto);
        notification.Id = Guid.NewGuid();
        await notificationRepository.AddAsync(notification);
        await notificationRepository.SaveChangesAsync();

        return notificationDto.MessageType switch
        {
            MessageTypeEnum.Success => await emailNotificationSender.SendAsync(notificationDto.Email, SuccessSubject,
                SuccessBody),
            MessageTypeEnum.Failure => await emailNotificationSender.SendAsync(notificationDto.Email, FailedSubject,
                FailedBody),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public async Task<IEnumerable<NotificationDto>> GetNotifications()
    {
        return (await notificationRepository.GetPagedAsync(1, itemsPerPage: int.MaxValue, filter: null))
            .Select(x => mapper.Map<NotificationDto>(x)).ToList();
    }

}