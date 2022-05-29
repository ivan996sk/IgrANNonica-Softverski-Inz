using api.Models;
using RestSharp;
using System.Net.WebSockets;
using System.Text;
using Newtonsoft.Json;
using Microsoft.AspNetCore.SignalR;

namespace api.Services
{
    public class MlConnectionService : IMlConnectionService
    {
        private RestClient client;
        private readonly IDatasetService _datasetService;
        private readonly IModelService _modelService;
        private readonly IHubContext<ChatHub> _ichat;
        private readonly IConfiguration _configuration;
        private readonly IFileService _fileService;

        public MlConnectionService(IConfiguration configuration,IDatasetService datasetService,IHubContext<ChatHub> ichat, IFileService fileService)
        {
            _configuration = configuration;

            this.client = new RestClient(_configuration.GetValue<string>("AppSettings:MlApi"));
            _datasetService=datasetService;
            _ichat=ichat;

            _fileService = fileService;
        }

        public async Task<string> SendModelAsync(object model, object dataset)//Don't Use
        {
            var request = new RestRequest("train", Method.Post);
            request.AddJsonBody(new { model, dataset});
            var result = await this.client.ExecuteAsync(request);
            return result.Content; //Response od ML microservisa
        }
        public async Task TrainModel(Model model, Experiment experiment, string filePath,Dataset dataset,string id)
        {
            var request = new RestRequest("train", Method.Post);
            request.AddParameter("model", JsonConvert.SerializeObject(model));
            request.AddParameter("experiment", JsonConvert.SerializeObject(experiment));
            request.AddParameter("dataset", JsonConvert.SerializeObject(dataset));
            //request.AddFile("file", file,filename);
            request.AddFile("file", filePath);
            request.AddHeader("Content-Type", "multipart/form-data");
            this.client.ExecuteAsync(request);
            return;

        }
        public async Task PreProcess(Dataset dataset,string filePath,string id)//(Dataset dataset,byte[] file,string filename)
        {
            var request=new RestRequest("preprocess", Method.Post);
            request.AddParameter("dataset", JsonConvert.SerializeObject(dataset));
            //request.AddFile("file", file,filename);
            request.AddFile("file", filePath);
            request.AddHeader("Content-Type", "multipart/form-data");
            var result=await this.client.ExecuteAsync(request);

            Dataset newDataset = JsonConvert.DeserializeObject<Dataset>(result.Content);
            newDataset.isPreProcess = true;
            _datasetService.Update(newDataset);
            if (ChatHub.CheckUser(id))
                foreach (var connection in ChatHub.getAllConnectionsOfUser(id))
                    await _ichat.Clients.Client(connection).SendAsync("NotifyDataset",newDataset.name,newDataset._id);

            return;

        }

        public async Task<string> Predict(Predictor predictor, Experiment experiment, PredictorColumns[] inputs)
        {
            string filePath = _fileService.GetFilePath(predictor.h5FileId, predictor.uploaderId);

            var request = new RestRequest("predict", Method.Post);
            request.AddParameter("predictor", JsonConvert.SerializeObject(predictor));
            request.AddParameter("experiment", JsonConvert.SerializeObject(experiment));
            request.AddFile("file", filePath);
            request.AddHeader("Content-Type", "multipart/form-data");

            var result = await this.client.ExecuteAsync(request);

            return result.Content;

        }
    }
}
