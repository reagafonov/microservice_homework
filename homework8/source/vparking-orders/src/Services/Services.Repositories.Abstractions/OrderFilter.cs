namespace Services.Repositories.Abstractions;

public class OrderFilter
{
    public string? ClientID { get; init; }

    public bool? IsPayed { get; init; }
    
    public bool? IsVerified { get; init; }
}