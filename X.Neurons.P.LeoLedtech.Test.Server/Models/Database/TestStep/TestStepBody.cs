namespace X.Neurons.P.LeoLedtech.Test.Server.Models.Database.TestStep
{
    public class TestStepBody
    {
        public int Id { get; set; }
        /// <summary>
        /// TestStepHead的ID
        /// </summary>
        public int TestStepHeadId { get; set; }
        /// <summary>
        /// 測試名稱
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 步驟
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
        /// 採集延遲時間
        /// </summary>
        public double CollectionDelayTime { get; set; }
        /// <summary>
        /// 延遲時間
        /// </summary>
        public double DleayTime { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
    }
}
