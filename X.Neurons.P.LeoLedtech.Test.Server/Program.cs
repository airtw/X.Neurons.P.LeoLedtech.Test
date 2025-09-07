using X.Neurons.P.LeoLedtech.Test.Server.Services;

namespace X.Neurons.P.LeoLedtech.Test.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebService.Initialization();
            while (true) { Thread.Sleep(1000); }
            ;
        }
    }
}
