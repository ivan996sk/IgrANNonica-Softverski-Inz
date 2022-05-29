using System;
using api.Models;

namespace api.Services
{
    public interface IModelService
    {
        Model GetOneModel(string userId, string name);
        Model GetOneModelById(string userId, string name);
        Model GetOneModel(string id);
        List<Model> GetMyModels(string userId);
        List<Model> GetMyModelsByType(string userId, string problemType);
        List<Model> GetLatestModels(string userId);
        List<Model> GetPublicModels();
        Model Create(Model model);
        Model Replace(Model model);
        void Update(string userId, string name, Model model);
        public void Update(string id, Model model);
        void Delete(string userId, string name);
        bool CheckHyperparameters(int inputNeurons, int hiddenLayerNeurons, int hiddenLayers, int outputNeurons);
        bool CheckDb();
    }
}

