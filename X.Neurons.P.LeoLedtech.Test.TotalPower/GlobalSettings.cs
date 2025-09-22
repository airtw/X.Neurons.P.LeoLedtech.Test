using X.Neurons.P.LeoLedtech.Test.TotalPower.BasicUtlis;

namespace X.Neurons.P.LeoLedtech.Test.TotalPower
{
    public static class GlobalSettings
    {
        public static string ServerIP { get; set; }
        public static ApiClient ServerApiClient { get; set; }
        public static bool IsServerConnect { get; set; } = false;
        public static Models.Jig.Message Jig {get;set;}
    }
}
