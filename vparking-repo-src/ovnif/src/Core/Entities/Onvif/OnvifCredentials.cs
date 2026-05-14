namespace Core;

// --- Доменные value objects для ONVIF (без зависимости от WCF) ---

/// <summary>
/// Учётные данные и сетевой хост для доступа к ONVIF-устройству.
/// Используется на границе прикладного слоя и вторичных адаптеров (SOAP).
/// </summary>
/// <param name="Host">IP или DNS камеры (без схемы и порта для построения URL вида <c>http://{Host}:8899</c>).</param>
/// <param name="Username">Имя пользователя WS-Security / HTTP.</param>
/// <param name="Password">Пароль.</param>
public sealed record OnvifCredentials(string Host, string Username, string Password);
