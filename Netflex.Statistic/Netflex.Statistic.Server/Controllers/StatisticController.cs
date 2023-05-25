using Confluent.Kafka;
using Kafka.Consumer.Configuration;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Netflex.Statistic.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticController : ControllerBase
    {
        private IMongoCollection<Models.Statistic> _statistics;
        IMongoClient _mongoClient;
        public StatisticController(IMongoDBSettings mongoDBSettings, IMongoClient mongoClient)
        {
            var mongoDB = mongoClient.GetDatabase("JustChat");

            _mongoClient = mongoClient;
            _statistics = mongoDB.GetCollection<Models.Statistic>("Statistics");
        }

        [HttpGet]
        public async Task<IActionResult> Get() 
        {
            return Ok((await _statistics.FindAsync(statistic => true)).ToList());
        }

    }
}
