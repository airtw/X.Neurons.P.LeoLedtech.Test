namespace X.Neurons.P.LeoLedtech.Test.Server.Models.Database.WorkOrder
{
    public class DTO_WorkOrderBody
    {
        public int Id { get; set; }
        /// <summary>
        /// 頭部ID
        /// </summary>
        public int WorkOrderHeadId { get; set; }
        /// <summary>
        /// 建立日期
        /// </summary>
        public string CreateDateTime { get; set; }
        /// <summary>
        /// 建立人
        /// </summary>
        public int Create { get; set; }
        /// <summary>
        /// 產品序號
        /// </summary>
        public string ProductNumber { get; set; }
        /// <summary>
        /// 是否測試了
        /// </summary>
        public int IsTest { get; set; }
        /// <summary>
        /// 最後測試時間
        /// </summary>
        public string LastTestDateTime { get; set; }
        /// <summary>
        /// 最後測試人員
        /// </summary>
        public int LastTestUser { get; set; }
        /// <summary>
        /// 最後測試結果
        /// </summary>
        public int LastIsPass { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
    }
}
