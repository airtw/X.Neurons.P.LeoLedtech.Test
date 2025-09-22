namespace X.Neurons.P.LeoLedtech.Test.Server.Models.PDF
{
    public class TestReport
    {
        public string WorkOrderNumber { get; set; } = "";
        public string ProductSerialNumber { get; set; } = "";
        public string TestDateTime { get; set; }
        public List<TestStep> TestSteps { get; set; } = new();
    }

}
