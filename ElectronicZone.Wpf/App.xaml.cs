using ElectronicZone.Wpf.ViewModel;
using System;
using System.Windows;

namespace ElectronicZone.Wpf
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Create the SplashScreen Window
            SplashScreen splashScreen = new SplashScreen("Resources/dashboard.ico");
            splashScreen.Show(true);
            splashScreen.Close(new TimeSpan(0, 0, 1));
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Create Main Window instance
            LoginWindow window = new LoginWindow();

            //// Create Main Window View Model
            //LoginViewModel viewModel = new LoginViewModel();

            //// Associate DataContext
            //window.DataContext = viewModel;

            Application.Current.MainWindow = window;
            window.Show();
        }
    }
}
