using Microsoft.AspNetCore.SignalR;
using System.Globalization;

namespace X.Neurons.P.LeoLedtech.Test.Server.Hubs
{
    public class DashboardHub : Hub
    {
        public string Test()
        {
            return "測試回傳資料";
        }
        public string StationsInfo()
        {
            //var sdsd = GlobalStore.FrontendStations;
            //Console.WriteLine();
            return "";
        }
        //public List<Pages> Pages()
        //{
        //    return GlobalStore.Pages;
        //}
        //public FrontendConfig Config()
        //{
        //    return GlobalStore.FrontendConfig;
        //}
        public async Task SystemTime()
        {
            var systemTime = DateTime.Now.ToString("yyyy/MM/dd tt hh:mm:ss", CultureInfo.CreateSpecificCulture("zh-TW"));
            await Clients.Caller.SendAsync("SystemTime", systemTime);
        }
        public async Task Heartbeat()
        {
            await Clients.Caller.SendAsync("HeartbeatResponse");
        }
    }
}
