
using api.Models;

namespace api.Services
{
    public interface IDatasetService
    {
        Dataset GetOneDataset(string userId, string id);
        Dataset GetOneDatasetN(string userId, string name);
        Dataset GetOneDataset(string id);
        List<Dataset> SearchDatasets(string name);
        List<Dataset> GetMyDatasets(string userId);
        List<Dataset> SortDatasets(string userId, bool ascdsc, int latest);
        List<Dataset> GetPublicDatasets();
        Dataset Create(Dataset dataset);
        void Update(string userId, string id, Dataset dataset);
        void Delete(string userId, string id);
        public List<Dataset> GetGuestDatasets();
        public void Update(Dataset dataset);
        string GetDatasetId(string fileId);
        //bool CheckDb();
    }
}
