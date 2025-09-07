using Microsoft.AspNetCore.Mvc;
using X.Neurons.P.LeoLedtech.Test.Server.Models;
using X.Neurons.P.LeoLedtech.Test.Server.Models.Test;

namespace X.Neurons.P.LeoLedtech.Test.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestStepController : ControllerBase
    {
        private readonly ILogger<TestStepController> _logger;

        public TestStepController(ILogger<TestStepController> logger)
        {
            _logger = logger;
        }

        [HttpGet("{id}")]
        public TestStep Get(string id)
        {
            var ch1 = new Cable
            {
                ID = 1,
                Name = "行車燈",
                CableColor = "blue"
            };
            var ch2 = new Cable
            {
                ID = 2,
                Name = "煞車燈",
                CableColor = "green_yellow"
            };
            var ch3 = new Cable
            {
                ID = 3,
                Name = "右方向燈",
                CableColor = "green"
            };
            var ch4 = new Cable
            {
                ID = 4,
                Name = "左方向燈",
                CableColor = "brown"
            };
            var step1 = new Models.TestStep.Step
            {
                ID = 1,
                Voltage = 9,
                Current = 2,
                Channel = new List<Models.TestStep.Channel>
                    {
                        new Models.TestStep.Channel
                        {
                            ID = 1,
                            H = 350,
                            L = 250
                        },
                        new Models.TestStep.Channel
                        {
                            ID = 2,
                            H = 200,
                            L = 150
                        },
                        new Models.TestStep.Channel
                        {
                            ID = 3,
                            H = 190,
                            L = 140
                        },
                        new Models.TestStep.Channel
                        {
                            ID = 4,
                            H = 190,
                            L = 140
                        }
                    }
            };
            var step2 = new Models.TestStep.Step
            {
                ID = 2,
                Voltage = 24,
                Current = 2,
                Channel = new List<Models.TestStep.Channel>
                    {
                        new Models.TestStep.Channel
                        {
                            ID = 1,
                            H = 360,
                            L = 300
                        },
                        new Models.TestStep.Channel
                        {
                            ID = 2,
                            H = 250,
                            L = 200
                        },
                        new Models.TestStep.Channel
                        {
                            ID = 3,
                            H = 200,
                            L = 150
                        },
                        new Models.TestStep.Channel
                        {
                            ID = 4,
                            H = 200,
                            L = 150
                        }
                    }
            };
            var step3 = new Models.TestStep.Step
            {
                ID = 3,
                Voltage = 13.5,
                Current = 2,
                Channel = new List<Models.TestStep.Channel>
                    {
                        new Models.TestStep.Channel
                        {
                            ID = 1,
                            H = 350,
                            L = 300
                        },
                        new Models.TestStep.Channel
                        {
                            ID = 2,
                            H = 250,
                            L = 200
                        },
                        new Models.TestStep.Channel
                        {
                            ID = 3,
                            H = 200,
                            L = 150
                        },
                        new Models.TestStep.Channel
                        {
                            ID = 4,
                            H = 200,
                            L = 150
                        }
                    }
            };

            var r = new TestStep
            {
                Guid = Guid.NewGuid().ToString(),
                Name = "AT001測試步驟",
                Description = "測試步驟",
                Cable = new List<Cable>(),
                Step = new List<Models.TestStep.Step>()
            };
            r.Cable.Add(ch1);
            r.Cable.Add(ch2);
            r.Cable.Add(ch3);
            r.Cable.Add(ch4);
            r.Step.Add(step1);
            r.Step.Add(step2);
            r.Step.Add(step3);
            return r;
        }
    }
}
