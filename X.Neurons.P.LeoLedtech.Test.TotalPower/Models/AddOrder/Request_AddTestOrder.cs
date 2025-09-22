using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace X.Neurons.P.LeoLedtech.Test.TotalPower.Models.AddOrder
{
    public class Request_AddTestOrder
    {
        [JsonPropertyName("workOrderBodyId")]
        public string WorkOrderBodyId { get; set; }
        [JsonPropertyName("head")]
        public Request_TestOrderHead Head { get; set; }
        [JsonPropertyName("body")]
        public List<Request_TestOrderBody> Body { get; set; }
    }
}
