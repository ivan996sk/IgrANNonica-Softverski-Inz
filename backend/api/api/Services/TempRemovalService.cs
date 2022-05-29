using api.Interfaces;
using api.Models;
using MongoDB.Driver;

namespace api.Services
{
    public class TempRemovalService
    {
        private readonly IMongoCollection<FileModel> _file;
        private readonly IMongoCollection<Model> _model;
        private readonly IMongoCollection<Dataset> _dataset;
        private readonly IMongoCollection<Experiment> _experiment;
        private readonly IMongoCollection<User> _user;
        private readonly IMongoCollection<Predictor> _predictor;

        public TempRemovalService(IUserStoreDatabaseSettings settings, IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase(settings.DatabaseName);
            _file = database.GetCollection<FileModel>(settings.FilesCollectionName);
            _model= database.GetCollection<Model>(settings.ModelCollectionName);
            _dataset = database.GetCollection<Dataset>(settings.DatasetCollectionName);
            _experiment= database.GetCollection<Experiment>(settings.ExperimentCollectionName);
            _user = database.GetCollection<User>(settings.CollectionName);
            _predictor = database.GetCollection<Predictor>(settings.PredictorCollectionName);

        }
        public void DeleteTemps()                
        {
            List<User> tempUsers=_user.Find(u=>u.isPermament==false).ToList();
            foreach (User user in tempUsers)
            {
                if ((DateTime.Now.ToUniversalTime() - user.dateCreated).TotalDays < 1)
                    continue;
                List<Predictor> tempPredictors=_predictor.Find(p=>p.uploaderId==user._id).ToList();
                List<Model> tempModels=_model.Find(m=>m.uploaderId==user._id).ToList();
                List<Experiment> tempExperiment = _experiment.Find(e => e.uploaderId == user._id).ToList();
                List<Dataset> tempDatasets = _dataset.Find(d => d.uploaderId == user._id).ToList();
                List<FileModel> tempFiles = _file.Find(f => f.uploaderId == user._id).ToList();


                foreach (Predictor predictor in tempPredictors)
                    DeletePredictor(predictor._id);
                foreach(Model model in tempModels)
                    DeleteModel(model._id);
                foreach(Experiment experiment in tempExperiment)
                    DeleteExperiment(experiment._id);
                foreach(Dataset dataset in tempDatasets)
                    DeleteDataset(dataset._id);
                foreach(FileModel file in tempFiles)
                {
                    DeleteFile(file._id);
                    if(File.Exists(file.path))
                        File.Delete(file.path);
                }
                DeleteUser(user._id);



            }
            

        }




        public void DeleteFile(string id)
        {
            _file.DeleteOne(file => file._id == id);
        }
        public void DeleteModel(string id)
        {
            _model.DeleteOne(model=>model._id==id);
        }
        public void DeleteDataset(string id)
        {
            _dataset.DeleteOne(dataset => dataset._id == id);
        }
        public void DeleteExperiment(string id)
        {
            _experiment.DeleteOne(experiment => experiment._id == id);
        }
        public void DeletePredictor(string id)
        {
            _predictor.DeleteOne(predictor=> predictor._id == id);
        }
        public void DeleteUser(string id)
        {
            _user.DeleteOne(user=>user._id == id);
        }


    }
}
