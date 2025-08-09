namespace Services.Abstractions;

public interface IValidateDto<in TDto>
{
    void Validate(TDto dto);
}