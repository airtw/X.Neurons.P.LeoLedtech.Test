using Microsoft.AspNetCore.Mvc;
using X.Neurons.P.LeoLedtech.Test.Server.Models;

namespace X.Neurons.P.LeoLedtech.Test.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HeartbeatController : ControllerBase
    {
        private readonly ILogger<HeartbeatController> _logger;

        public HeartbeatController(ILogger<HeartbeatController> logger)
        {
            _logger = logger;
        }

        [HttpGet("{id}")]
        public Response_Heartbeat Get(string id)
        {
            var systemTime = DateTime.Now;
            return new Response_Heartbeat
            {
                Date = systemTime.ToString("yyyy-MM-dd"),
                Time = systemTime.ToString("HH:mm:ss"),
                Status = true
            };
        }
    }
}
