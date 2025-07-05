using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Services.Contracts;

namespace Services.Abstractions;

/// <summary>
/// Сервис работы с клиентами (интерфейс)
/// </summary>
public interface INotificationService
{

    Task<bool> SendNotification(NotificationDto notificationDto);
    Task<IEnumerable<NotificationDto>> GetNotifications();
}