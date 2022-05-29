
using api.Models;

namespace api.Services
{
    public interface IMlConnectionService
    {
        Task<string> SendModelAsync(object model, object dataset);
        Task PreProcess(Dataset dataset, string filePath,string id);
        Task TrainModel(Model model, Experiment experiment, string filePath, Dataset dataset, string id);

        Task<string> Predict(Predictor predictor, Experiment experiment, PredictorColumns[] inputs);

        //Task<Dataset> PreProcess(Dataset dataset, byte[] file, string filename);
    }
}