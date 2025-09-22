namespace X.Neurons.P.LeoLedtech.Test.Server.Models.Database.TestOrder
{
    public class TestOrderHead
    {
        public int Id { get; set; }
        /// <summary>
        /// 建立測試時間
        /// </summary>
        public string CreateDateTime { get; set; }
        /// <summary>
        /// 工站
        /// </summary>
        public string Station { get; set; }
        /// <summary>
        /// 測試者
        /// </summary>
        public int TestUser { get; set; }
        /// <summary>
        /// 產品序號
        /// </summary>
        public int ProductNumber { get; set; }
        /// <summary>
        /// 產品測試步驟
        /// </summary>
        public int TestModel { get; set; }
        /// <summary>
        /// 產品測試結果
        /// </summary>
        public bool IsPass { get; set; }
    }
}
