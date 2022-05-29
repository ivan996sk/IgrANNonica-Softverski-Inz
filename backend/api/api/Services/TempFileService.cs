using api.Interfaces;
using MongoDB.Driver;

namespace api.Services
{
    public class TempFileService : IHostedService
    {
        private readonly TempRemovalService _removalService;
        private Timer _timer;

        public TempFileService(IUserStoreDatabaseSettings settings, IMongoClient mongoClient)
        {
            _removalService = new TempRemovalService(settings, mongoClient);
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(RemoveTempFiles,null,TimeSpan.Zero,TimeSpan.FromHours(6));


            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
        private void RemoveTempFiles(object state)
        {
            _removalService.DeleteTemps();
        }
    }
}
