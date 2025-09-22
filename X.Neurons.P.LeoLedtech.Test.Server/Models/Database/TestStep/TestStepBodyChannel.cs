namespace X.Neurons.P.LeoLedtech.Test.Server.Models.Database.TestStep
{
    public class TestStepBodyChannel
    {
        public int Id { get; set; }
        /// <summary>
        /// TestStepBody的ID
        /// </summary>
        public int TestStepHeadId { get; set; }
        /// <summary>
        /// 通道
        /// </summary>
        public int Channel { get; set; }
        /// <summary>
        /// 通道名稱
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 線色
        /// </summary>
        public string CableColor { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
    }
}
