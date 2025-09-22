namespace X.Neurons.P.LeoLedtech.Test.Server.Models.Database.TestStep
{
    public class TestStepBodyLimit
    {
        public int Id { get; set; }
        /// <summary>
        /// TestStepBody的ID
        /// </summary>
        public int TestStepBodyId { get; set; }
        /// <summary>
        /// 通道
        /// </summary>
        public int Channel { get; set; }
        /// <summary>
        /// 高高限
        /// </summary>
        public double HH { get; set; }
        /// <summary>
        /// 高限
        /// </summary>
        public double H { get; set; }
        /// <summary>
        /// 低限
        /// </summary>
        public double L { get; set; }
        /// <summary>
        /// 低低限
        /// </summary>
        public double LL { get; set; }
    }
}
