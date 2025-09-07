using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace X.Neurons.P.LeoLedtech.Test.TotalPower.Models.Test
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
