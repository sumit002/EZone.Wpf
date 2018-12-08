using MahApps.Metro.Controls;

namespace ElectronicZone.Wpf.View.Common
{
    /// <summary>
    /// Interaction logic for AboutUs.xaml
    /// </summary>
    public partial class AboutUs : MetroWindow
    {
        public AboutUs()
        {
            InitializeComponent();

            LoadData();
        }

        private void LoadData()
        {
            string version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            this.versionInfo.Text = $"Version - {version}";
        }
    }
}
