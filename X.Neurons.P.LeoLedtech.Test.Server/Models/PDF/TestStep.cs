namespace X.Neurons.P.LeoLedtech.Test.Server.Models.PDF
{
    public class TestStep
    {
        public string StepNumber { get; set; } = "";
        public string TestTime { get; set; }
        public string StepDetails { get; set; } = "";
        public List<ChannelResult> ChannelResults { get; set; } = new();
    }
}
