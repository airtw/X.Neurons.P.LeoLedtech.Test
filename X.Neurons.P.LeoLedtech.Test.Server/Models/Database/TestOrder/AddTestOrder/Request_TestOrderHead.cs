using System.Text.Json.Serialization;

namespace X.Neurons.P.LeoLedtech.Test.Server.Models.Database.TestOrder.AddOrder
{
    public class Request_TestOrderHead
    {
        /// <summary>
        /// 建立測試時間
        /// </summary>
        [JsonPropertyName("createDateTime")]
        public string CreateDateTime { get; set; }
        /// <summary>
        /// 工站
        /// </summary>
        [JsonPropertyName("station")]
        public string Station { get; set; }
        /// <summary>
        /// 測試者
        /// </summary>
        [JsonPropertyName("testUser")]
        public int TestUser { get; set; }
        /// <summary>
        /// 產品序號
        /// </summary>
        [JsonPropertyName("productNumber")]
        public int ProductNumber { get; set; }
        /// <summary>
        /// 產品測試步驟
        /// </summary>
        [JsonPropertyName("testModel")]
        public int TestModel { get; set; }
        /// <summary>
        /// 產品測試結果
        /// </summary>
        [JsonPropertyName("isPass")]
        public bool IsPass { get; set; }
    }
}
