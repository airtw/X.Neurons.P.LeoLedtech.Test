using System.Text.Json.Serialization;

namespace X.Neurons.P.LeoLedtech.Test.Server.Models.Test
{
    public class Cable
    {
        [JsonPropertyName("id")]
        public int ID { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("channel")]
        public int Channel { get; set; }
        [JsonPropertyName("cable_color")]
        public string CableColor { get; set; }
    }
}
