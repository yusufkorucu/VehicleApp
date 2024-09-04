namespace Vehicle.Api.Events;

public class SpendEvent
{
    public Guid Id { get; set; }
    public string EventName { get; set; } = "spend-event";
    public int AccountId { get; set; }
    public decimal Price { get; set; } = 5;
}