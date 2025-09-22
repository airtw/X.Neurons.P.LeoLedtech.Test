using X.Neurons.P.LeoLedtech.Test.Server.Models.Database.WorkOrder;

namespace X.Neurons.P.LeoLedtech.Test.Server.Models.DTO
{
    public class DTO_WorkOrder
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
        public string ProductModel { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        public List<WorkOrderBody> WorkOrderBody { get; set; }
    }
}
