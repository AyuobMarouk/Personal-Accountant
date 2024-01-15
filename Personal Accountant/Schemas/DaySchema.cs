using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace Personal_Accountant.Schemas
{
    public class DaySchema
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public int Date { get; set; }

        public List<TransSchema> Transactions { get; set; }

        public double Balance { get; set; }
    }
}
