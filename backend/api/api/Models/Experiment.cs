using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api.Models
{
    public class Experiment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string type { get; set; }
        public List<string> ModelIds { get; set; }
        public string datasetId { get; set; }
        public string uploaderId { get; set; }
        public string[] inputColumns { get; set; }
        public string outputColumn { get; set; }
        public string nullValues { get; set; }
        public DateTime dateCreated { get; set; }
        public DateTime lastUpdated { get; set; }
        public NullValues[] nullValuesReplacers { get; set; }
        public ColumnEncoding[] encodings { get; set; }
        public string[] columnTypes { get; set; }
    }
}
