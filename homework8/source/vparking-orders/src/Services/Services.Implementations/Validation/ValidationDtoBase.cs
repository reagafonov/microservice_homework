using System.Collections.Generic;
using System.Text.RegularExpressions;
using Services.Abstractions;
using Services.Abstractions.Exceptions;

namespace Services.Implementations.Validation;

public abstract class ValidationBase<TDto> : IValidateDto<TDto>
{
    private readonly ICollection<string> _errors = new HashSet<string>();

    private bool HasErrors => _errors.Count != 0;

    public void Validate(TDto dto)
    {
        CheckErrors(dto);
        if (HasErrors)
            throw new DtoValidationException(string.Join('\n', _errors));
    }

    private static string GetRequiredErrorString(string fieldName)
        => $"Поле {fieldName} не установлено";

    private static string GetMaxLengthErrorString(string fieldName, int maxLength)
        => $"Превышена максимальная длина {maxLength} поля {fieldName}";

    protected void AddError(string error) => _errors.Add(error);

    protected void CheckRequired<TData>(TData data, string fieldName)
    {
        if (data == null)
            AddError(GetRequiredErrorString(fieldName));
        if (data is string s && string.IsNullOrWhiteSpace(s))
            AddError(GetRequiredErrorString(fieldName));
    }

    protected void CheckStringLength(string? data, int maxLength, string fieldName)
    {
        if (data is null)
            return;
        if (data.Length > maxLength)
            AddError(GetMaxLengthErrorString(fieldName, maxLength));
    }

    protected void CheckEmail(string email, string fieldName)
    {
        if (Regex.IsMatch(email,"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$")) return;
        AddError($"{fieldName} не является почтовым адресом" );
    }

    protected abstract void CheckErrors(TDto dto);
}