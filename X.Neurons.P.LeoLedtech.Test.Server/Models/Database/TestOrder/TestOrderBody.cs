namespace X.Neurons.P.LeoLedtech.Test.Server.Models.Database.TestOrder
{
    public class TestOrderBody
    {
        public int Id { get; set; }
        /// <summary>
        /// 頭部ID
        /// </summary>
        public int TestOrderHeadId { get; set; }
        /// <summary>
        /// 步驟測試時間(yyyy-MM-dd HH:mm:ss.fff)
        /// </summary>
        public string DateTime { get; set; }
        /// <summary>
        /// 測試步驟
        /// </summary>
        public int Step { get; set; }
        /// <summary>
        /// 輸出電壓
        /// </summary>
        public double Voltage { get; set; }
        /// <summary>
        /// 輸出電流
        /// </summary>
        public double Current { get; set; }
        /// <summary>
        /// 步驟是否通過
        /// </summary>
        public bool IsPass { get; set; }
        /// <summary>
        /// 內容
        /// </summary>
        public string Content { get; set; }
    }
}
