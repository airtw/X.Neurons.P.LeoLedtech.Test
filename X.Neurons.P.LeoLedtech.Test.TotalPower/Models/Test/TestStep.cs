using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace X.Neurons.P.LeoLedtech.Test.TotalPower.Models.Test
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
