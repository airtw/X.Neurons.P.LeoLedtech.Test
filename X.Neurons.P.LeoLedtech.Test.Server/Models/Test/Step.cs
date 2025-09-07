using System.Text.Json.Serialization;

namespace X.Neurons.P.LeoLedtech.Test.Server.Models.TestStep
{
    public class Step
    {
        [JsonPropertyName("id")]
        public int ID { get; set; }
        [JsonPropertyName("voltage")]
        public double Voltage { get; set; }
        [JsonPropertyName("current")]
        public double Current { get; set; }
        [JsonPropertyName("channel")]
        public List<Channel> Channel { get; set; }
    }
}
