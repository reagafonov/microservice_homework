using System.Threading.Tasks;
using Common.Users;
using Domain.Entities;
using keycloak_userEditor.Queue;

namespace Services.Repositories.Abstractions;

public interface INewUserMessageProducer
{
    Task<bool> ProduceAsync(NewUserEventMessage notification);
}