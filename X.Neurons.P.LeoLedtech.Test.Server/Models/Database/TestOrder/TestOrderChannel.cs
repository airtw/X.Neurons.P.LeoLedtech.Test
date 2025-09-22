namespace X.Neurons.P.LeoLedtech.Test.Server.Models.Database.TestOrder
{
    public class TestOrderChannel
    {
        public string Id { get; set; }
        /// <summary>
        /// TestStepBody的ID
        /// </summary>
        public int TestOrderBodyId { get; set; }
        /// <summary>
        /// 通道
        /// </summary>
        public int Channel { get; set; }
        /// <summary>
        /// 電壓紀錄
        /// </summary>
        public double Voltage { get; set; }
        /// <summary>
        /// 電流紀錄
        /// </summary>
        public double Current { get; set; }
        /// <summary>
        /// 功率
        /// </summary>
        public double Power { get; set; }
    }
}
