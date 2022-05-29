using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api.Models
{
    public class Model
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]//mongo data type to .net
        public string _id { get; set; }
        public string uploaderId { get; set; }


        public string name { get; set; }
        public string description { get; set; }
        public DateTime dateCreated { get; set; }
        public DateTime lastUpdated { get; set; }


        //Neural net training
        public string type { get; set; }
        public string optimizer { get; set; }
        public string lossFunction { get; set; }
        public int hiddenLayers { get; set; }
        public string batchSize { get; set; }
        public string learningRate { get; set; }
        // na izlazu je moguce da bude vise neurona (klasifikacioni problem sa vise od 2 klase)
        public int outputNeurons { get; set; }
        public Layer[] layers { get; set; }
        public string outputLayerActivationFunction { get; set; }

        public string[] metrics { get; set; }
        public int epochs { get; set; }
        public bool randomOrder { get; set; }
        public bool randomTestSet { get; set; }
        public float randomTestSetDistribution { get; set; }
        public bool isPublic { get; set; }
        public bool accessibleByLink { get; set; }
        public float validationSize { get; set; }//0-1 ne ukljucuje 0.1-0.9 ogranici
    }

    public class Layer
    {
        

        public Layer(int layerNumber, string activationFunction, int neurons, string regularisation, string regularisationRate) 
        {
            this.layerNumber = layerNumber;
            this.activationFunction = activationFunction;
            this.neurons = neurons;
            this.regularisation = regularisation;
            this.regularisationRate = regularisationRate;
        }

        public int layerNumber { get; set; }
        public string activationFunction { get; set; }
        public int neurons { get; set; }
        public string regularisation { get; set; }
        public string regularisationRate  { get; set; }
    }
}


