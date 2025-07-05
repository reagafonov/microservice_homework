namespace Services.Repositories.Abstractions;

public class AccountFilter
{
    public string? ClientID { get; set; }

    public bool? IsDeleted { get; set; }
}