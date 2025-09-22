using System.Text.Json.Serialization;

namespace X.Neurons.P.LeoLedtech.Test.Server.Models.Database.TestOrder.AddOrder
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
