namespace Payment.Api.Domain;

public class PaymentRequestModel
{
    public int AccountId { get; set; }
    public decimal Price { get; set; }
}