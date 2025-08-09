namespace Services.Contracts;

public class ChangeBallanceDto
{
    public string ClientID { get; set; }

    public decimal Amount { get; set; }
}