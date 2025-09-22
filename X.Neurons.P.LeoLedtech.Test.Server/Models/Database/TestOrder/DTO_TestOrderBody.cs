namespace X.Neurons.P.LeoLedtech.Test.Server.Models.Database.TestOrder
{
    public class DTO_TestOrderBody
    {
        public int Id { get; set; }
        /// <summary>
        /// 步驟測試時間(yyyy-MM-dd HH:mm:ss.fff)
        /// </summary>
        public string DateTime { get; set; }
        /// <summary>
        /// 測試步驟及細節 (步驟1 輸出電壓:12V 輸出電流:2A 是否通過:通過)
        /// </summary>
        public string StepDetail { get; set; }
        public List<DTO_TestOrderChannel> TestOrderChannel { get; set; }
    }
}
