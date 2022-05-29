using api.Interfaces;
using api.Models;
using MongoDB.Driver;

namespace api.Services
{
    public class UserService : IUserService 
    {
        private readonly IMongoCollection<User> _users;
        private readonly IMongoClient _client;
        private readonly IMongoCollection<Model> _models;
        private readonly IMongoCollection<Dataset> _datasets;
        private readonly IMongoCollection<FileModel> _fileModels;
        private readonly IMongoCollection<Predictor> _predictors;


        public UserService(IUserStoreDatabaseSettings settings, IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase(settings.DatabaseName);
             _users = database.GetCollection<User>(settings.CollectionName);
            _models = database.GetCollection<Model>(settings.ModelCollectionName);
            _datasets= database.GetCollection<Dataset>(settings.DatasetCollectionName);
            _fileModels = database.GetCollection<FileModel>(settings.FilesCollectionName);
            _predictors= database.GetCollection<Predictor>(settings.PredictorCollectionName);
            _client = mongoClient;
        }
        public User Create(User user)
        {
            _users.InsertOne(user);
            return user;
        }
        public List<User> Get()
        {
            return _users.Find(user => true).ToList();
        }
        public User GetUserByUsername(string username)
        {
            return _users.Find(user=>user.Username == username).FirstOrDefault();
        }
        public User GetUserById(string id)
        {
            return _users.Find(user => user._id == id).FirstOrDefault();
        }
        public User GetUserUsername(string username)
        {
            return _users.Find(user => user.Username == username).FirstOrDefault();
        }
        public bool Update(string username, User user)
        {
            //username koji postoji u bazi
            using (var session =  _client.StartSession())
            {
                if(username!=user.Username)
                    if(_users.Find(u => u.Username == user.Username).FirstOrDefault()!=null)
                    {
                        return false;
                    }

                //Trenutan MongoDB Server ne podrzava transakcije.Omoguciti Podrsku
                //session.StartTransaction();
                try
                {
                    _users.ReplaceOne(user => user.Username == username, user);
                    /*if (username != user.Username)
                    {
                        var builderModel = Builders<Model>.Update;
                        var builderDataset = Builders<Dataset>.Update;
                        var builderPredictor = Builders<Predictor>.Update;
                        _models.UpdateMany(x => x.username == username, builderModel.Set(x => x.username, user.Username));
                        _datasets.UpdateMany(x => x.username == username, builderDataset.Set(x => x.username, user.Username));
                        _predictors.UpdateMany(x => x.username == username, builderPredictor.Set(x => x.username, user.Username));
                    }
                    */
                    //session.AbortTransaction();


                    //session.CommitTransaction();
                }
                catch (Exception e)
                {
                    //session.AbortTransaction();
                    return false;
                }
                return true;
            }
        }
        public void Delete(string username)
        {
            _users.DeleteOne(user => user.Username == username);

        }
    }
}
