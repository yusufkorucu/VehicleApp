using MongoDB.Driver;
using Vehicle.Api.Collection;
using Vehicle.Api.Events;
using Vehicle.Api.Messaging;
using Vehicle.Api.Models;

namespace Vehicle.Api.Services;

public class VehicleService : IVehicleService
{
    private readonly IEventBus _eventBus;
    private readonly IMongoCollection<Collection.Vehicle> _vehicleCollection;
    private readonly IMongoCollection<Collection.Account> _accountCollection;

    public VehicleService(IEventBus eventBus)
    {
        var client = new MongoClient("mongodb://localhost:27017");
        var database = client.GetDatabase("vehicleApp");
        _vehicleCollection = database.GetCollection<Collection.Vehicle>("vehicles");
        _accountCollection = database.GetCollection<Account>("accounts");
        _eventBus = eventBus;
    }

    public async Task<ApiResponseModel> GetVehicleByPlateAsync(PlateQueryRequestModel requestModel)
    {
        
        var accountFilter = Builders<Account>.Filter.Eq(account => account.AccountId, requestModel.AccountId);
        var account = await _accountCollection.Find(accountFilter).FirstOrDefaultAsync();

        if (account?.TotalBalance < 5 || account == null)
        {
            return new ApiResponseModel() { IsSuccess = false, Description = "Bakiye Yetersiz" };
        }

        var filter = Builders<Collection.Vehicle>.Filter.Eq(vehicle => vehicle.Plate, requestModel.Plate);
        var vehicle = await _vehicleCollection.Find(filter).FirstOrDefaultAsync();

        if (vehicle != null)
        {
            var response = new ApiResponseModel
            {
                VehicleHistory = vehicle.Histories.ToList(),
                IsSuccess = true,
                Description = "Vehicle history"
            };

            await _eventBus.PublishAsync(new SpendEvent()
            {
                AccountId = requestModel.AccountId
            }, "spend-events", Guid.NewGuid().ToString());

            return response;
        }

        return new ApiResponseModel() { IsSuccess = false, Description = "Vehicle Not Found" };
    }
}