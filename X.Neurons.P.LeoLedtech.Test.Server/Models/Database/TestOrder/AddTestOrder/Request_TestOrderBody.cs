using System.Text.Json.Serialization;

namespace X.Neurons.P.LeoLedtech.Test.Server.Models.Database.TestOrder.AddOrder
{
    public class Request_TestOrderBody
    {
        /// <summary>
        /// 步驟測試時間(yyyy-MM-dd HH:mm:ss.fff)
        /// </summary>
        [JsonPropertyName("datetime")]
        public string DateTime { get; set; }
        /// <summary>
        /// 測試步驟
        /// </summary>
        [JsonPropertyName("step")]
        public int Step { get; set; }
        /// <summary>
        /// 輸出電壓
        /// </summary>
        [JsonPropertyName("voltage")]
        public double Voltage { get; set; }
        /// <summary>
        /// 輸出電流
        /// </summary>
        [JsonPropertyName("current")]
        public double Current { get; set; }
        /// <summary>
        /// 步驟是否通過
        /// </summary>
        [JsonPropertyName("isPass")]
        public bool IsPass { get; set; }
        /// <summary>
        /// 內容
        /// </summary>
        [JsonPropertyName("content")]
        public string Content { get; set; }
        /// <summary>
        /// 通道內容
        /// </summary>
        [JsonPropertyName("channels")]
        public List<Request_TestOrderChannel> Channels { get; set; }
    }
}
