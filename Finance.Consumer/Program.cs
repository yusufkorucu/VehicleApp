using Confluent.Kafka;
using MongoDB.Driver;
using System.Text.Json;
using Finance.Consumer.Collection;
using Finance.Consumer.Events;

public class Program
{
    private static readonly string _topic = "payment-events";
    private static readonly string _spendTopic = "spend-events";
    private static readonly string _bootstrapServers = "0.0.0.0:9092,0.0.0.0:19092";
    private static readonly string _mongoConnectionString = "mongodb://localhost:27017";
    private static readonly string _databaseName = "vehicleApp";
    private static readonly string _collectionName = "accounts";
    private static readonly int _batchSize = 10;

    private static IMongoCollection<Account> _accountCollection;

    public static void Main(string[] args)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _bootstrapServers,
            GroupId = "my-group",
            AutoOffsetReset = AutoOffsetReset.Latest
        };

        var client = new MongoClient(_mongoConnectionString);
        var database = client.GetDatabase(_databaseName);
        _accountCollection = database.GetCollection<Account>(_collectionName);

        

        var cancellationTokenSource = new CancellationTokenSource();
        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true;
            cancellationTokenSource.Cancel();
        };

        var program = new Program();
        program.StartConsumingAsync(config, cancellationTokenSource.Token).GetAwaiter().GetResult();
    }

    public async Task StartConsumingAsync(ConsumerConfig config, CancellationToken cancellationToken)
    {
        using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
        {
            consumer.Subscribe(_topic);
            consumer.Subscribe(_spendTopic);

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        bool success = false;
                        var consumeResult = consumer.Consume(cancellationToken);

                        var eventModel = JsonSerializer.Deserialize<EventModel>(consumeResult.Message.Value);

                        if (eventModel?.EventName == "spend-event")
                            success = await ProcessSpendEventAsync(eventModel);
                        else
                            success = await UpdateAccountBalanceAsync(eventModel.AccountId, eventModel.Price);


                        if (success)
                            consumer.Commit(consumeResult);
                        
                    }
                    catch (ConsumeException e)
                    {
                        Console.WriteLine($"Error occurred: {e.Error.Reason}");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                consumer.Close();
            }
        }
    }

    private async Task<bool> UpdateAccountBalanceAsync(int accountId, decimal balance)
    {
        try
        {
            var filter = Builders<Account>.Filter.Eq(a => a.AccountId, accountId);
            var update = Builders<Account>.Update.Inc(a => a.TotalBalance, balance);

            var result = await _accountCollection.UpdateOneAsync(filter, update);

            if (result.ModifiedCount > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating account balance: {ex.Message}");
            return false;
        }
    }


    private async Task<bool> ProcessSpendEventAsync(EventModel eventModel)
    {
        decimal spendAmount = eventModel.Price;

        var filter = Builders<Account>.Filter.Eq(a => a.AccountId, eventModel.AccountId);
        var update = Builders<Account>.Update.Inc(a => a.TotalBalance, -spendAmount);

        var result = await _accountCollection.UpdateOneAsync(filter, update);

        if (result.ModifiedCount > 0)
            return true;

        return false;
    }
}