using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace X.Neurons.P.LeoLedtech.Test.TotalPower.Models.AddOrder
{
    public class Request_TestOrderChannel
    {
        /// <summary>
        /// 通道
        /// </summary>
        [JsonPropertyName("channel")]
        public int Channel { get; set; }
        /// <summary>
        /// 電壓紀錄
        /// </summary>
        [JsonPropertyName("voltage")]
        public double Voltage { get; set; }
        /// <summary>
        /// 電流紀錄
        /// </summary>
        [JsonPropertyName("current")]
        public double Current { get; set; }
        /// <summary>
        /// 功率
        /// </summary>
        [JsonPropertyName("power")]
        public double Power { get; set; }
    }
}
