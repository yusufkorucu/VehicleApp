using Vehicle.Api.Models;

namespace Vehicle.Api.Services;

public interface IVehicleService
{
    Task<ApiResponseModel> GetVehicleByPlateAsync(PlateQueryRequestModel requestModel);
}