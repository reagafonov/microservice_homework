using System;

namespace Services.Repositories.Abstractions.Exceptions;

public class CrudUpdateException(string message) : Exception($"Ошибка записи данных{message}");