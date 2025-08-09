using System.Threading.Tasks;
using Domain.Entities;

namespace Services.Repositories.Abstractions;

public interface INotificationAsyncMessage
{
    Task<bool> ProduceAsync(Notification notification);
}