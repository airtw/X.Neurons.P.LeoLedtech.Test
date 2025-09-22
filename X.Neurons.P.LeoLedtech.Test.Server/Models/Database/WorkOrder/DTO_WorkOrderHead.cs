namespace X.Neurons.P.LeoLedtech.Test.Server.Models.Database.WorkOrder
{
    public class DTO_WorkOrderHead
    {
        public int Id { get; set; }
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
        /// 序號開頭
        /// </summary>
        public string ProdcutNumberHead { get; set; }
        /// <summary>
        /// 產品數量
        /// </summary>
        public int ProdcutQuantity { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
    }
}
