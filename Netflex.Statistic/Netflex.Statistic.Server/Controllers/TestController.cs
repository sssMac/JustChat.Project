using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Netflex.Statistic.Server.Models;
using Netflex.Statistic.Server.RabitMQProducer;
using Newtonsoft.Json;

namespace Netflex.Statistic.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IRabitMQProducer _rabbitmqProducer;

        public TestController(IRabitMQProducer rabbitmqProducer)
        {
            _rabbitmqProducer = rabbitmqProducer;
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            var message = new StatisticRequest
            {
                Id = Guid.NewGuid(),
                Name = "Test Name"
            };

            await _rabbitmqProducer.SendStatistic(JsonConvert.SerializeObject(message));
            return Ok();
        }
    }
}
