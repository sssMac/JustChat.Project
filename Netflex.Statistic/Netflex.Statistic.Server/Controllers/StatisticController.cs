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
            var mongoDB = mongoClient.GetDatabase("JustChat");
            _statistics = mongoDB.GetCollection<Models.Statistic>("Statistics");
        }

        [HttpGet]
        public async Task<IActionResult> Get() 
        {
            var filter = Builders<Models.Statistic>.Filter.Empty;
            return Ok(await _statistics.FindAsync(filter));
        }

    }
}
