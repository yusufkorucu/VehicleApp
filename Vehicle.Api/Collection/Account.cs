using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Vehicle.Api.Collection;

public class Account
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId Id { get; set; }
    public int AccountId { get; set; }
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal TotalBalance { get; set; }
}