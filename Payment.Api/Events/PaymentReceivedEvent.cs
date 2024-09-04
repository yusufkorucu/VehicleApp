namespace Payment.Api.Events;

public class PaymentReceivedEvent
{
    public Guid Id { get; set; }
    public int AccountId { get; set; }
    public decimal Price { get; set; }
    public DateTime Date { get; set; }
}