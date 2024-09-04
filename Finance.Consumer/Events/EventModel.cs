namespace Finance.Consumer.Events;

public class EventModel
{
    public string EventName { get; set; }
    public int AccountId { get; set; }
    public decimal Price { get; set; }
}