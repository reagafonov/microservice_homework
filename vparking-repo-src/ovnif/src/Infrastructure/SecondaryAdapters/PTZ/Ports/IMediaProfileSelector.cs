using ONVIFMedia;

namespace Ovnif.Ports.Ptz;

/// <summary>
/// Вторичный порт: политика выбора ONVIF Media profile, к которому привязана PTZ-конфигурация.
/// </summary>
public interface IMediaProfileSelector
{
    /// <summary>Возвращает выбранный профиль или <c>null</c>, если ни один не подходит.</summary>
    /// <param name="profiles">Список профилей из <c>GetProfiles</c>.</param>
    Profile? Select(IReadOnlyList<Profile> profiles);
}
