using System.Text.Json.Serialization;

namespace X.Neurons.P.LeoLedtech.Test.Server.Models.TestStep
{
    public class Channel
    {
        [JsonPropertyName("id")]
        public int ID { get; set; }
        [JsonPropertyName("hh")]
        public double HH { get; set; }
        [JsonPropertyName("h")]
        public double H { get; set; }
        [JsonPropertyName("l")]
        public double L { get; set; }
        [JsonPropertyName("ll")]
        public double LL { get; set; }
    }
}
