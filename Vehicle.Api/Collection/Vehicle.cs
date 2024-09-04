using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Vehicle.Api.Collection;

public class Vehicle
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string Plate { get; set; }

    [BsonElement("histories")]
    public List<VehicleHistory> Histories { get; set; }
}

public class VehicleHistory
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string Description { get; set; }
    public string Km { get; set; }
    public DateTime Date { get; set; }
    public string Company { get; set; }
}