using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using api.Interfaces;

namespace api.Data
{
    public class UserStoreDatabaseSettings : IUserStoreDatabaseSettings
    {
        public string ConnectionString { get; set; } = String.Empty;
        public string DatabaseName { get; set; } = String.Empty;
        public string CollectionName { get; set; } = String.Empty;
        public string DatasetCollectionName { get; set; } = String.Empty;
        public string PredictorCollectionName { get; set; } = String.Empty;
        public string ModelCollectionName { get; set; } = String.Empty;
        public string FilesCollectionName { get; set; } = String.Empty;
        public string ExperimentCollectionName { get; set; } = String.Empty;
    }
}
