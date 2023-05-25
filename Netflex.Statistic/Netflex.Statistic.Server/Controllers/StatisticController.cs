using Kafka.Consumer.Configuration;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Netflex.Statistic.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticController : ControllerBase
    {
        private IMongoCollection<Models.Statistic> _statistics;

        public StatisticController(IMongoDBSettings mongoDBSettings, IMongoClient mongoClient)
        {
            var mongoDB = mongoClient.GetDatabase(mongoDBSettings.DatabaseName);
            _statistics = mongoDB.GetCollection<Models.Statistic>(mongoDBSettings.CollectionName);
        }

        [HttpGet]
        public async Task<IActionResult> Get() 
        {
            var filter = Builders<Models.Statistic>.Filter.Empty;
            return Ok(await _statistics.FindAsync(filter));
        }

    }
}
