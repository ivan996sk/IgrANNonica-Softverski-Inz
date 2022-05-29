using api.Models;

namespace api.Services
{
    public interface IPredictorService
    {
        Predictor Create(Predictor predictor);
        void Delete(string id, string userId);
        List<Predictor> GetMyPredictors(string userId);
        Predictor GetOnePredictor(string id);
        Predictor GetPredictor(string userId, string id);
        List<Predictor> GetPublicPredictors();
        List<Predictor> SortPredictors(string userId, bool ascdsc, int latest);
        void Update(string id, Predictor predictor);
        public Predictor Exists(string modelId, string experimentId);
    }
}