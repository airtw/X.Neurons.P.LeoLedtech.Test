namespace X.Neurons.P.LeoLedtech.Test.TotalPower
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            if (AppSetting.Default.clientID == "none")
            {
                AppSetting.Default.clientID = Guid.NewGuid().ToString();
                AppSetting.Default.Save();
            }
            GlobalSettings.Jig = new Models.Jig.Message();
            GlobalSettings.Jig.Channels = new List<Models.Jig.Channel>();
            GlobalSettings.Jig.Channels.Add(new Models.Jig.Channel { ID = 1 });
            GlobalSettings.Jig.Channels.Add(new Models.Jig.Channel { ID = 2 });
            GlobalSettings.Jig.Channels.Add(new Models.Jig.Channel { ID = 3 });
            GlobalSettings.Jig.Channels.Add(new Models.Jig.Channel { ID = 4 });
            Application.Run(new Main());
        }
    }
}