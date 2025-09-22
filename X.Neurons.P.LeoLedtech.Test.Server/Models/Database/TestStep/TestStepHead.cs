namespace X.Neurons.P.LeoLedtech.Test.Server.Models.Database.TestStep
{
    public class TestStepHead
    {
        public int Id { get; set; }
        /// <summary>
        /// 是否已經刪除
        /// </summary>
        public bool IsDeleted { get; set; }
        /// <summary>
        /// 測試名稱
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 是否為PCB板測試
        /// </summary>
        public bool IsPCB { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
    }
}
