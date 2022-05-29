using api.Interfaces;
using api.Models;
using MongoDB.Driver;

namespace api.Services
{
    public class PredictorService : IPredictorService
    {
        private readonly IMongoCollection<Predictor> _predictor;

        public PredictorService(IUserStoreDatabaseSettings settings, IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase(settings.DatabaseName);
            _predictor = database.GetCollection<Predictor>(settings.PredictorCollectionName);
        }


        public Predictor Create(Predictor predictor)
        {
            _predictor.InsertOne(predictor);
            return predictor;
        }

        public void Delete(string id, string userId)
        {
            _predictor.DeleteOne(predictor => (predictor._id == id && predictor.uploaderId == userId));
        }

        public List<Predictor> GetMyPredictors(string userId)
        {
            return _predictor.Find(predictor => predictor.uploaderId == userId).ToList();
        }

        public Predictor GetOnePredictor(string id)
        {
            return _predictor.Find(predictor => predictor._id == id).FirstOrDefault();

        }
        public Predictor GetPredictor(string userId, string id)
        {
            return _predictor.Find(predictor => predictor._id == id && (predictor.uploaderId == userId || predictor.isPublic == true)).FirstOrDefault();

        }

        public List<Predictor> SortPredictors(string userId, bool ascdsc, int latest)
        {
            List<Predictor> list = _predictor.Find(predictor => predictor.uploaderId == userId).ToList();

            if (ascdsc)
                list = list.OrderBy(predictor => predictor.dateCreated).ToList();
            else
                list = list.OrderByDescending(predictor => predictor.dateCreated).ToList();
            return list;
        }

        public List<Predictor> GetPublicPredictors()
        {
            return _predictor.Find(predictor => predictor.isPublic == true).ToList();
        }

        public void Update(string id, Predictor predictor)
        {
            _predictor.ReplaceOne(predictor => predictor._id == id, predictor);
        }
        public Predictor Exists(string modelId, string experimentId)
        {
            Predictor p=_predictor.Find(pre=> pre.modelId == modelId && pre.experimentId==experimentId).FirstOrDefault();
            return p;
        }
    }
}
