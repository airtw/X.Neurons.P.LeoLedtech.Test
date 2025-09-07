using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace X.Neurons.P.LeoLedtech.Test.TotalPower.Models.Test
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
