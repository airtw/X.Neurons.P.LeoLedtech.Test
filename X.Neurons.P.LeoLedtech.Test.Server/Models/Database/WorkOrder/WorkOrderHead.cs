namespace X.Neurons.P.LeoLedtech.Test.Server.Models.Database.WorkOrder
{
    public class WorkOrderHead
    {
        public int Id { get; set; }
        /// <summary>
        /// 是否已經刪除
        /// </summary>
        public bool IsDeleted { get; set; }
        /// <summary>
        /// 建立時間
        /// </summary>
        public string CreateDateTime { get; set; }
        /// <summary>
        /// 建立人
        /// </summary>
        public int Create { get; set; }
        /// <summary>
        /// 工單號
        /// </summary>
        public string Number { get; set; }
        /// <summary>
        /// 產品型號
        /// </summary>
        public int ProductModel { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
    }
}
