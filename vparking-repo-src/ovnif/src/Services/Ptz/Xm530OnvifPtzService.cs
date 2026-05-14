using Core;
using Core.Interfaces;
using Ovnif.Ports.Ptz;

namespace Services.Ptz;

/// <summary>
/// Прикладной сценарий (application / use case): установка угла PTZ для камер XM530 через ONVIF SOAP.
/// Реализует входящий порт <see cref="IOnvif"/> и оркестрирует вторичные порты из <see cref="Ovnif.Ports.Ptz"/>.
/// </summary>
/// <param name="endpointsResolver">Разрешение URL сервисов по Device.GetCapabilities.</param>
/// <param name="sessionFactory">Фабрика пары клиентов Media+PTZ на одну операцию.</param>
/// <param name="profileSelector">Выбор профиля (Main / первый с PTZ).</param>
/// <param name="moveComposer">Сборка вектора и скорости из Position + PTZConfiguration.</param>
/// <param name="moveExecutor">Вызов SOAP AbsoluteMove.</param>
public sealed class Xm530OnvifPtzService(
    IOnvifEndpointsResolver endpointsResolver,
    IOnvifMediaPtzSoapSessionFactory sessionFactory,
    IMediaProfileSelector profileSelector,
    IPtzAbsoluteMoveComposer moveComposer,
    IPtzAbsoluteMoveExecutor moveExecutor) : IOnvif
{
    /// <inheritdoc />
    public async Task SetPositionAsync(
        string ip,
        string username,
        string password,
        Position position,
        CancellationToken token)
    {
        // Без валидной позиции смысла в ONVIF-вызовах нет
        ArgumentNullException.ThrowIfNull(position);
        // Ранний выход, если отмена уже запрошена
        token.ThrowIfCancellationRequested();

        // Упаковываем строковые параметры CLI в value object домена
        var credentials = new OnvifCredentials(ip, username, password);
        // Узнаём реальные SOAP-адреса Media/PTZ на устройстве
        var endpoints = await endpointsResolver.ResolveAsync(credentials, token).ConfigureAwait(false);

        // Открываем каналы; await using гарантирует Dispose даже при исключении
        await using var session = await sessionFactory
            .CreateOpenedSessionAsync(endpoints, credentials, token)
            .ConfigureAwait(false);

        // После тяжёлых I/O снова проверяем отмену
        token.ThrowIfCancellationRequested();
        // Читаем список профилей с камеры
        var profiles = await session.GetMediaProfilesAsync(token).ConfigureAwait(false);
        // Оставляем один профиль с PTZ по политике XM530
        var profile = profileSelector.Select(profiles);
        // Без токена профиля AbsoluteMove некуда отправлять
        if (profile?.PTZConfiguration == null || string.IsNullOrEmpty(profile.token))
            throw new InvalidOperationException(
                "No suitable media profile with PTZ (token empty). Try another ONVIF profile on the device.");

        // Перед SOAP-командой ещё раз уважаем CancellationToken
        token.ThrowIfCancellationRequested();
        // Готовим тело AbsoluteMove (вектор + скорость)
        var payload = moveComposer.Compose(position, profile.PTZConfiguration);
        // Отправляем команду на PTZ-порт той же сессии
        await moveExecutor.ExecuteAsync(session.PtzPort, profile.token, payload, token).ConfigureAwait(false);
    }
}
