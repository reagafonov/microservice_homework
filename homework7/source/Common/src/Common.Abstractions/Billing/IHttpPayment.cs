using Services.Contracts;

namespace Services.Abstractions;

public interface IHttpPayment
{
    Task<bool> Pay(PaymentModel model);
}