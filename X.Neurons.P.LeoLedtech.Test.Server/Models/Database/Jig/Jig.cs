namespace X.Neurons.P.LeoLedtech.Test.Server.Models.Database.Jig
{
    public class Jig
    {
        public int Id { get; set; }
        /// <summary>
        /// 是否已經刪除
        /// </summary>
        public bool IsDeleted { get; set; }
        /// <summary>
        /// 適用產品
        /// </summary>
        public int ProductId { get; set; }
        /// <summary>
        /// 治具型號
        /// </summary>
        public string Number { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
    }
}
