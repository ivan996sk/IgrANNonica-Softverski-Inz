using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api.Models
{
	public class Dataset
	{
        public Dataset() { }
        public string uploaderId { get; set; }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]//mongo data type to .net
        public string _id { get; set; }

        public string description { get; set; }
        public string name { get; set; }
        public string fileId { get; set; }
        public string extension { get; set; }
        public bool isPublic { get; set; }
        public bool accessibleByLink { get; set; }
        public DateTime dateCreated { get; set; }
        public DateTime lastUpdated { get; set; }
        public string delimiter { get; set; }
        public ColumnInfo[] columnInfo { get; set; }
        public int rowCount { get; set; }
        public int nullCols { get; set; }
        public int nullRows { get; set; }
        public bool isPreProcess { get; set; }

        public float[][] cMatrix { get; set; }
    }
}

