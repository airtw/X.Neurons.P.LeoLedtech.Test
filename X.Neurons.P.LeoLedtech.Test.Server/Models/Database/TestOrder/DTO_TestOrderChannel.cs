namespace X.Neurons.P.LeoLedtech.Test.Server.Models.Database.TestOrder
{
    public class DTO_TestOrderChannel
    {
        /// <summary>
        /// 通道
        /// </summary>
        public int Channel { get; set; }
        /// <summary>
        /// 電壓紀錄
        /// </summary>
        public string Voltage { get; set; }
        /// <summary>
        /// 電流紀錄
        /// </summary>
        public string Current { get; set; }
        /// <summary>
        /// 功率
        /// </summary>
        public string Power { get; set; }
    }
}
