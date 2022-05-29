using api.Controllers;
using api.Interfaces;
using api.Models;
using MongoDB.Driver;

namespace api.Services
{
    public class DatasetService : IDatasetService
    {
        private readonly IMongoCollection<Dataset> _dataset;
        private readonly IMongoCollection<Experiment> _experiment;
        private readonly IExperimentService _experimentService;

        public DatasetService(IUserStoreDatabaseSettings settings, IMongoClient mongoClient, IExperimentService experimentService)
        {
            var database = mongoClient.GetDatabase(settings.DatabaseName);
            _dataset = database.GetCollection<Dataset>(settings.DatasetCollectionName);
            _experiment = database.GetCollection<Experiment>(settings.ExperimentCollectionName);
            _experimentService = experimentService;
        }

        public List<Dataset> SearchDatasets(string name)
        {
            return _dataset.Find(dataset => dataset.name == name && dataset.isPublic == true && dataset.isPreProcess).ToList();
        }

        //kreiranje dataseta
        public Dataset Create(Dataset dataset)
        {
            _dataset.InsertOne(dataset);
            return dataset;
        }

        //brisanje odredjenog name-a
        public void Delete(string userId, string id)
        {
            List<Experiment> experiment = null;
            _dataset.DeleteOne(dataset => (dataset.uploaderId == userId && dataset._id == id));

            experiment = _experiment.Find(experiment => (experiment.datasetId == id && experiment.uploaderId == userId)).ToList();

            foreach (Experiment experimentItem in experiment)
                _experimentService.Delete(userId, experimentItem._id);
        }

        public List<Dataset> GetMyDatasets(string userId)
        {
            return _dataset.Find(dataset => dataset.uploaderId == userId && dataset.isPreProcess).ToList();
        }
        public List<Dataset> GetGuestDatasets()
        {
            //Join Igranonica public datasetove sa svim temp uploadanim datasetovima
            List<Dataset> datasets = _dataset.Find(dataset => dataset.uploaderId == "000000000000000000000000" && dataset.isPublic == true && dataset.isPreProcess).ToList();
            datasets.AddRange(_dataset.Find(dataset => dataset.uploaderId == "" && dataset.isPreProcess).ToList());
            return datasets;
        }

        //poslednji datasetovi
        public List<Dataset> SortDatasets(string userId, bool ascdsc, int latest)
        {
            List<Dataset> list = _dataset.Find(dataset => dataset.uploaderId == userId && dataset.isPreProcess).ToList();

            if (ascdsc)
                list = list.OrderBy(dataset => dataset.lastUpdated).ToList();
            else
                list = list.OrderByDescending(dataset => dataset.lastUpdated).ToList();

            return list;
        }

        public List<Dataset> GetPublicDatasets()
        {
            return _dataset.Find(dataset => dataset.isPublic == true && dataset.isPreProcess).ToList();
        }

        public Dataset GetOneDataset(string userId, string id)
        {
            return _dataset.Find(dataset => dataset.uploaderId == userId && dataset._id == id && dataset.isPreProcess).FirstOrDefault();
        }
        public Dataset GetOneDatasetN(string userId, string name)
        {
            return _dataset.Find(dataset => dataset.uploaderId == userId && dataset.name == name && dataset.isPreProcess).FirstOrDefault();
        }
        //odraditi za pretragu getOne

        public Dataset GetOneDataset(string id)
        {
            return _dataset.Find(dataset => dataset._id == id && dataset.isPreProcess).FirstOrDefault();
        }

        //ako je potrebno da se zameni name  ili ekstenzija
        public void Update(string userId, string id, Dataset dataset)
        {
            _dataset.ReplaceOne(dataset => dataset.uploaderId == userId && dataset._id == id, dataset);
        }
        public void Update(Dataset dataset)
        {
            _dataset.ReplaceOne(x => x._id == dataset._id, dataset);
        }

        public string GetDatasetId(string fileId)
        {
            Dataset dataset = _dataset.Find(dataset => dataset.fileId == fileId && dataset.uploaderId == "000000000000000000000000").FirstOrDefault();

            return dataset._id;
        }
        
    }
}
