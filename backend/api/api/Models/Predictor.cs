using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api.Models
{
	public class Predictor
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]//mongo data type to .net
		public string _id { get; set; }
		public string uploaderId { get; set; }
		public string[] inputs { get; set; }
		public string output { get; set; }
		public bool isPublic { get; set; }
		public bool accessibleByLink { get; set; }
		public DateTime dateCreated { get; set; }
		public string experimentId { get; set; }
		public string modelId { get; set; }
		public string h5FileId { get; set; }

		//public Metric[] metrics { get; set; }
	
		public float[] metricsLoss { get; set; }
		public float[] metricsValLoss { get; set; }
		public float[] metricsAcc { get; set; }
		public float[] metricsValAcc { get; set; }
		public float[] metricsMae { get; set; }
		public float[] metricsValMae { get; set; }
		public float[] metricsMse { get; set; }
		public float[] metricsValMse { get; set; }
		//public Metric[] finalMetrics { get; set; }
	}
	
	/*public class Metric
    {
		string Name { get; set; }
		string JsonValue { get; set; }

    }*/
	
}