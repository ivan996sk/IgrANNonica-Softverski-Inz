namespace api.Interfaces
{
    public interface IUserStoreDatabaseSettings
    {
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
        string CollectionName { get; set; }
        string DatasetCollectionName { get; set; }
        string PredictorCollectionName { get; set; }
        string ModelCollectionName { get; set; }
        string FilesCollectionName { get; set; }
        string ExperimentCollectionName { get; set; }
    }
}
