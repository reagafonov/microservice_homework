# ONVIF PTZ: гексагональная структура и соглашения по коду

## Назначение

Решение **VParkingOvnif** выставляет угол PTZ камеры (в т.ч. XM530) по протоколу **ONVIF** через SOAP. Сборка следует упрощённой **гексагональной** схеме, отражённой в структуре solution: **Core**, **Services**, **Infrastructure/SecondaryAdapters**, **MoveCamera** (первичный адаптер).

## Слои и зависимости

| Слой | Проекты / папки | Роль |
|------|------------------|------|
| Домен | `Core/Entities` | Сущности и value objects: `Position`, `OnvifCredentials`, `OnvifServiceEndpoints`, `Xm530PtzConstants`. Не зависят от WCF/ONVIF SDK. |
| Входящие порты | `Core/Interfaces` | Контракты, которые вызывает приложение: `IOnvif`, `IOnvifRunService`. |
| Прикладные сценарии | `Services` | Use case: `OnvifService`, `Services/Ptz/Xm530OnvifPtzService`. Зависят от портов `Ovnif.Ports.Ptz`, не от WCF-классов. |
| Исходящие порты (вторичные) | `Infrastructure/.../PTZ/Ports` | Интерфейсы `Ovnif.Ports.Ptz` и DTO `PtzAbsoluteMovePayload` — «что нужно от ONVIF», без привязки к конкретному транспорту в имени класса. |
| Вторичные адаптеры | `Infrastructure/.../PTZ/Adapters/OnvifSoap` | Реализации портов поверх сгенерированных WCF-клиентов (`ONVIFDevice`, `ONVIFMedia`, `ONVIFPTZ`). |
| Первичный адаптер | `MoveCamera` | CLI, `Program.cs`: composition root, регистрация реализаций в `Microsoft.Extensions.DependencyInjection`. |

Зависимости направлены **внутрь**: домен не знает об инфраструктуре; `Services` не ссылается на конкретные классы из `Adapters`, только на `Ovnif.Ports.Ptz` (интерфейсы собираются в сборке PTZ вместе с адаптерами — типичный компромисс для небольшого репозитория).

## Соглашение: один тип на файл

В области ONVIF PTZ и связанного домена:

- Каждый **`class`**, **`interface`**, **`record`**, статический класс констант — **отдельный `.cs` файл** с именем, совпадающим с именем типа.
- Примеры: `OnvifCredentials.cs`, `OnvifServiceEndpoints.cs`, `IOnvifEndpointsResolver.cs`, `Xm530OnvifPtzService.cs`.

Сгенерированные файлы `ServiceReference/**/Reference.cs` не трогаются.

## Поток выполнения (XM530, текущая сборка)

1. `MoveCamera/Program` создаёт `CustomBinding`, регистрирует реализации портов и `IOnvif` → `Xm530OnvifPtzService`.
2. `Xm530OnvifPtzService` запрашивает `IOnvifEndpointsResolver` → получает `OnvifServiceEndpoints`.
3. `IOnvifMediaPtzSoapSessionFactory` открывает сессию Media+PTZ.
4. `IMediaProfileSelector` выбирает профиль с PTZ.
5. `IPtzAbsoluteMoveComposer` строит `PtzAbsoluteMovePayload`.
6. `IPtzAbsoluteMoveExecutor` вызывает `AbsoluteMove` на контракте `PTZ`.

## Смена камеры или политики

- Другая логика выбора профиля: новая реализация `IMediaProfileSelector`.
- Другая шкала углов / пространство координат: новая реализация `IPtzAbsoluteMoveComposer` (константы при необходимости вынести в `Core` рядом с доменом).
- Регистрация в `MoveCamera/Program.cs`: заменить соответствующую строку `AddSingleton<..., ...>`.

## Документация в коде

Публичные типы снабжены **XML-комментариями** (`/// summary`, `param` для записей и методов), чтобы IDE и при включённой генерации `DocumentationFile` можно было получить справочник сборки.

В сценарии PTZ/ONVIF (`MoveCamera/Program`, `Services/Ptz`, `Adapters/OnvifSoap`) добавлены **внутренние комментарии** (`//`) блоками примерно каждые 3–4 строки: поясняют шаг алгоритма, а не дублируют очевидный синтаксис.

## Сборка и запуск

Сборка solution или проекта `MoveCamera`; параметры CLI задаются в `MoveCamera/InputOptions.cs` (`--ip`, `--username`, `--password`, `--pan`, `--tilt`).
