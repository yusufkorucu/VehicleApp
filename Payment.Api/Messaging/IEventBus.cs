namespace Payment.Api.Messaging;

public interface IEventBus
{
    Task PublishAsync<T>(T @event,string topicName,string key=null) where T : class;
}