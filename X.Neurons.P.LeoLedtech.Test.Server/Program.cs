using X.Neurons.P.LeoLedtech.Test.Server.Services;

namespace X.Neurons.P.LeoLedtech.Test.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //GlobalSettings.DBConnectionString = "Server=127.0.0.1;Port=3306;User ID=leoledtech;Password=!leo28015742;Database=leoledtech;SslMode=None;";
            GlobalSettings.DBConnectionString = "Server=127.0.0.1;Port=3306;User ID=eason;Password=a29025110;Database=leoledtech;SslMode=None;";
            WebService.Initialization();
            while (true) { Thread.Sleep(1000); }
            
        }
    }
}
