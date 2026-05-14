using ONVIFMedia;
using Ovnif.Ports.Ptz;

namespace Ovnif.Adapters.OnvifSoap;

/// <summary>Адаптер политики выбора профиля для линейки XM530 (приоритет профиля с «Main» в имени).</summary>
public sealed class Xm530MediaProfileSelector : IMediaProfileSelector
{
    public Profile? Select(IReadOnlyList<Profile> profiles)
    {
        ArgumentNullException.ThrowIfNull(profiles);

        // Локальная функция: у профиля должна быть PTZ-конфигурация
        static bool HasPtz(Profile p) => p.PTZConfiguration != null;

        // Сначала ищем «основной» поток с PTZ — часто именно он управляемый
        var withMain = profiles.FirstOrDefault(p => HasPtz(p) && p.Name != null &&
            p.Name.Contains("Main", StringComparison.OrdinalIgnoreCase));
        if (withMain != null)
            return withMain;

        // Иначе берём любой профиль с ненулевой PTZConfiguration
        return profiles.FirstOrDefault(HasPtz);
    }
}
