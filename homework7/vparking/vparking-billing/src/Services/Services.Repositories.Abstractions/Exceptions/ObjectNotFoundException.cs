using System;

namespace Services.Repositories.Abstractions.Exceptions;

public class ObjectNotFoundException(string message):Exception($"Объект не найден:{message}");