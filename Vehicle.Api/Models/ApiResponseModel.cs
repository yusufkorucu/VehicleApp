using System.Text.Json;
using System.Text.Json.Serialization;
using Confluent.Kafka;
using Vehicle.Api.Collection;

namespace Vehicle.Api.Models;

public class ApiResponseModel
{
    public bool IsSuccess { get; set; }
    public string Description { get; set; }
    public List<VehicleHistory> VehicleHistory { get; set; }
    
    public bool ShouldSerializeVehicleHistory()
    {
        return VehicleHistory != null;
        
    }
}