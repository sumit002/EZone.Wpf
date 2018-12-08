using log4net;
using System;
using System.Windows;

namespace ElectronicZone.Wpf
{
    public partial class App : Application
    {
        ILog log = LogManager.GetLogger(typeof(App));
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Create the SplashScreen Window
            SplashScreen splashScreen = new SplashScreen("Resources/dashboard.ico");
            splashScreen.Show(true);
            splashScreen.Close(new TimeSpan(0, 0, 1));
        }
    }
}
