using System.Text.Json.Serialization;
using X.Neurons.P.LeoLedtech.Test.Server.Models.TestStep;

namespace X.Neurons.P.LeoLedtech.Test.Server.Models.Test
{
    public class TestStep
    {
        [JsonPropertyName("guid")]
        public string Guid { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("cable")]
        public List<Cable> Cable { get; set; }
        [JsonPropertyName("step")]
        public List<Step> Step { get; set; }
    }
}
