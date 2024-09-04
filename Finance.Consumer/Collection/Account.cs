using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Finance.Consumer.Collection;

public class Account
{
    public int AccountId { get; set; }
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal TotalBalance { get; set; }
}