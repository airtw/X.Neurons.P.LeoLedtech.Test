namespace X.Neurons.P.LeoLedtech.Test.Server.Models.Database.WorkOrder
{
    public class RequestWorkOrder
    {
        public WorkOrderBody WorkOderNumber { get; set; }
        public string ProductNumber { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }
}
