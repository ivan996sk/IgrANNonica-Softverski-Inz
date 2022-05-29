using api.Interfaces;
using api.Models;
using MongoDB.Driver;

namespace api.Services
{
    public class FileService : IFileService
    {

        private readonly IMongoCollection<FileModel> _file;
        private readonly IMongoCollection<Dataset> _dataset;

        public FileService(IUserStoreDatabaseSettings settings, IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase(settings.DatabaseName);
            _file = database.GetCollection<FileModel>(settings.FilesCollectionName);
            _dataset = database.GetCollection<Dataset>(settings.DatasetCollectionName);
        }

        public FileModel Create(FileModel file)
        {
            if (file == null)
                return null;
            _file.InsertOne(file);
            return file;
        }

        public string GetFilePath(string id, string uploaderId)
        {
            FileModel file;
            if (_dataset.Find(x=>x.fileId==id && x.isPublic==true).FirstOrDefault()!=null)
                file = _file.Find(x => x._id == id).FirstOrDefault();
            else
                file = _file.Find(x => x._id == id && x.uploaderId == uploaderId).FirstOrDefault();
            if (file == null)
                return null;
            return file.path;
        }

        public FileModel getFile(string id)
        {
            return _file.Find(x=>x._id==id).FirstOrDefault();
        }

        public bool CheckDb()
        {
            FileModel? file = null;
            file = _file.Find(file => file.uploaderId == "000000000000000000000000").FirstOrDefault();

            if (file != null)
                return false;
            else
                return true;
        }

        public string GetFileId(string fullPath)
        {
            FileModel file = _file.Find(file => file.path == fullPath).FirstOrDefault();

            return file._id;
        }

        public bool CopyFile(string sourceFile, string destinacionFile)
        {
            File.Copy(sourceFile, destinacionFile, true);
            return true;
        }
    }
}
