using api.Models;

namespace api.Services
{
    public interface IExperimentService
    {
        Experiment Create(Experiment experiment);
        public Experiment Get(string id);
        public List<Experiment> GetMyExperiments(string id);
        public Experiment Get(string uploaderId, string name);
        Experiment GetOneExperiment(string userId, string id);
        Experiment Update(string userId, string id, Experiment experiment);
        void Delete(string userId, string id);

    }
}