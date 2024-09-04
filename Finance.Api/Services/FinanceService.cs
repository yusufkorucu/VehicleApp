using Finance.Api.Collection;
using MongoDB.Driver;

namespace Finance.Api.Services;

public class FinanceService:IFinanceService
{
    private readonly IMongoCollection<Account> _accountCollection;

    public FinanceService()
    {
        var client = new MongoClient("mongodb://localhost:27017");
        var database = client.GetDatabase("vehicleApp");
        _accountCollection = database.GetCollection<Account>("accounts");
    }

    public async Task<decimal> GetTotalBalance(int accountId)
    {
        var filter = Builders<Account>.Filter.Eq(a => a.AccountId, accountId);
        
        var projection = Builders<Account>.Projection.Include(a=>a.TotalBalance);

        var result = await _accountCollection.Find(filter).FirstOrDefaultAsync();

        if (result != null)
        {
            decimal totalBalance = result.TotalBalance;
            return totalBalance;
        }

        return 0;
    }
}