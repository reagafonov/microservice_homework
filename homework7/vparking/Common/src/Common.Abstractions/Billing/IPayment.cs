using Services.Contracts;

namespace Services.Abstractions;

public interface IPayment
{
    Task<bool> Pay(PaymentModel model);
}