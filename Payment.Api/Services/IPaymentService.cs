using Payment.Api.Domain;

namespace Payment.Api.Services;

public interface IPaymentService
{
    Task<bool> Pay(PaymentRequestModel requestModel);
}