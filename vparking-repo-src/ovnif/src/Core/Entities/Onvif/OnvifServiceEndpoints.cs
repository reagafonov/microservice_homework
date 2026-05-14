namespace Core;

// Результат GetCapabilities: куда звонить за Media и PTZ отдельно

/// <summary>
/// Абсолютные SOAP URL сервисов Media и PTZ, полученные из <c>Device.GetCapabilities</c>.
/// </summary>
/// <param name="MediaServiceUrl">Базовый адрес ONVIF Media (например <c>http://192.168.1.10/onvif/media</c>).</param>
/// <param name="PtzServiceUrl">Базовый адрес ONVIF PTZ.</param>
public sealed record OnvifServiceEndpoints(string MediaServiceUrl, string PtzServiceUrl);
