using api.Interfaces;
using api.Models;
using MongoDB.Driver;

namespace api.Services
{
    public class ExperimentService : IExperimentService
    {
        private readonly IMongoCollection<Experiment> _experiment;
        private readonly IMongoCollection<Predictor> _predictor;

        public ExperimentService(IUserStoreDatabaseSettings settings, IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase(settings.DatabaseName);
            _experiment = database.GetCollection<Experiment>(settings.ExperimentCollectionName);
            _predictor = database.GetCollection<Predictor>(settings.PredictorCollectionName);
        }

        public Experiment Create(Experiment experiment)
        {
            _experiment.InsertOne(experiment);
            return experiment;
        }
        public Experiment Get(string id)
        {
            return _experiment.Find(exp => exp._id == id).FirstOrDefault();
        }
        public Experiment Get(string uploaderId, string name)
        {
            return _experiment.Find(exp => exp.uploaderId == uploaderId && exp.name == name).FirstOrDefault();
        }

        public void Update(string id, Experiment experiment)
        {
            _experiment.ReplaceOne(experiment => experiment._id == id, experiment);
        }
        public List<Experiment> GetMyExperiments(string userId)
        {
            return _experiment.Find(experiment => experiment.uploaderId == userId).ToList();

        }

        public Experiment GetOneExperiment(string userId, string id)
        {
            return _experiment.Find(experiment => experiment.uploaderId == userId && experiment._id == id).FirstOrDefault();
        }

        public Experiment Update(string userId, string id, Experiment experiment)
        {
            _experiment.ReplaceOne(experiment => experiment.uploaderId == userId && experiment._id == id, experiment);
            return _experiment.Find(experiment => experiment.uploaderId == userId && experiment._id == id).FirstOrDefault();
        }

        public void Delete(string userId, string id)
        {
            _experiment.DeleteOne(experiment => (experiment.uploaderId == userId && experiment._id == id));
            _predictor.DeleteMany(predictor => (predictor.uploaderId == userId && predictor.experimentId == id));
        }

    }
}
