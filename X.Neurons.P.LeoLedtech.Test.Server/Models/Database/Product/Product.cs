namespace X.Neurons.P.LeoLedtech.Test.Server.Models.Database.Product
{
    public class Product
    {
        public int Id { get; set; }
        /// <summary>
        /// 是否已經刪除
        /// </summary>
        public bool IsDeleted { get; set; }
        /// <summary>
        /// 產品型號
        /// </summary>
        public string Model { get; set; }
        /// <summary>
        /// 適用的測試步驟
        /// </summary>
        public int TestStepId { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
    }
}
