using ElectronicZone.Wpf.View.Common;
using ElectronicZone.Wpf.View.Master;
using ElectronicZone.Wpf.View.Payment;
using ElectronicZone.Wpf.View.Report;
using ElectronicZone.Wpf.View.Sale;
using ElectronicZone.Wpf.ViewModel;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ElectronicZone.Wpf.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private bool _shutdown;
        private readonly MainWindowViewModel _viewModel;


        public MainWindow()
        {
            _viewModel = new MainWindowViewModel(DialogCoordinator.Instance);
            this.DataContext = _viewModel;

            InitializeComponent();
        }

        #region Menu Clicks
        private void Info_Click(object sender, RoutedEventArgs e)
        {
            //SoundPlayer asd = new SoundPlayer(@"e:\3D_drums.wav");// it requires wmv files only
            //asd.Play();
            Info info = new Info();
            info.ShowDialog();
        }

        private void AboutUs_Click(object sender, RoutedEventArgs e)
        {
            AboutUs about = new AboutUs();
            about.ShowDialog();
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            Help help = new Help();
            help.ShowDialog();
        }

        private void Invoice_Click(object sender, RoutedEventArgs e)
        {
            SaleInvoice saleInvoice = new SaleInvoice();
            saleInvoice.ShowDialog();
        }

        private void InvoiceMaster_Click(object sender, RoutedEventArgs e)
        {
            InvoiceMaster invMaster = new InvoiceMaster();
            invMaster.ShowDialog();
        }




        // Masters Section
        private void ProductMaster_Click(object sender, RoutedEventArgs e)
        {
            ProductMaster product = new ProductMaster();
            product.ShowDialog();
        }

        private void BrandMaster_Click(object sender, RoutedEventArgs e)
        {
            BrandMaster brand = new BrandMaster();
            brand.ShowDialog();
        }

        private void StockMaster_Click(object sender, RoutedEventArgs e)
        {
            PurchaseMaster purchaseMaster = new PurchaseMaster();
            purchaseMaster.ShowDialog();
        }

        private void Sale_Click(object sender, RoutedEventArgs e)
        {
            SaleMaster sale = new SaleMaster();
            sale.ShowDialog();
        }

        private void Dashboard_Click(object sender, RoutedEventArgs e)
        {
            DashboardWindow sale = new DashboardWindow();
            sale.ShowDialog();
        }

        private void Firebase_Click(object sender, RoutedEventArgs e)
        {
            Firebase.FirebaseTest firebase = new Firebase.FirebaseTest();
            firebase.ShowDialog();
        }

        private void Support_Click(object sender, RoutedEventArgs e)
        {
            SupportIncomeMaster supportPayment = new SupportIncomeMaster();
            supportPayment.ShowDialog();
        }

        // Reports Section
        private void SalesReport_Click(object sender, RoutedEventArgs e)
        {
            SalesReport salesReport = new SalesReport();
            salesReport.ShowDialog();
        }

        private void PurchaseReport_Click(object sender, RoutedEventArgs e)
        {
            PurchaseReport purchaseReport = new PurchaseReport();
            purchaseReport.ShowDialog();
        }

        private void SupportPaymentReport_Click(object sender, RoutedEventArgs e)
        {
            SupportPaymentReport supportPaymentReport = new SupportPaymentReport();
            supportPaymentReport.ShowDialog();
        }

        private void ContactReport_Click(object sender, RoutedEventArgs e)
        {
            ContactReport contactReport = new ContactReport();
            contactReport.ShowDialog();
        }

        private void ContactsReport_Click(object sender, RoutedEventArgs e)
        {
            ContactReport contactReport = new ContactReport();
            contactReport.ShowDialog();
        }

        private void PendingPaymentReport_Click(object sender, RoutedEventArgs e)
        {
            PendingPaymentReport pendingPaymentReport = new PendingPaymentReport();
            pendingPaymentReport.ShowDialog();
        }

        private void Contact_Click(object sender, RoutedEventArgs e)
        {
            ContactMaster contactMaster = new ContactMaster();
            contactMaster.ShowDialog();
        }

        private void PendingPayment_Click(object sender, RoutedEventArgs e)
        {
            PendingPayment pendingPayment = new PendingPayment();
            pendingPayment.Show();
        }
        #endregion



        /*******************************  Luncher  *****************************/
        private void LaunchOnGitHub(object sender, RoutedEventArgs e)
        {
            // ToDo: Get The link from AppSettings
            System.Diagnostics.Process.Start("https://github.com/sumit002/EZone.Wpf");
        }

        private void LaunchREADMEGitHub(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/sumit002/EZone.Wpf/blob/master/README.md");
        }

        private void LaunchLICENSEGitHub(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/sumit002/EZone.Wpf/blob/master/LICENSE");
        }

        private void LaunchOnTwitter(object sender, RoutedEventArgs e)
        {
            // ToDo: Get The link from AppSettings
            System.Diagnostics.Process.Start("https://twitter.com/summit_dash");
        }

        private void LaunchSizeToContentDemo(object sender, RoutedEventArgs e)
        {
            // new SizeToContentDemo() { Owner = this }.Show();
        }

        private void MenuWindowWithoutBorderOnClick(object sender, RoutedEventArgs e)
        {
            var w = this.GetTestWindow();
            w.Content = new TextBlock() { Text = "MetroWindow without Border", FontSize = 28, FontWeight = FontWeights.Light, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center };
            w.BorderThickness = new Thickness(0);
            w.Show();
        }

        private void MenuWindowWithBorderOnClick(object sender, RoutedEventArgs e)
        {
            var w = this.GetTestWindow();
            w.Content = new TextBlock() { Text = "MetroWindow with Border", FontSize = 28, FontWeight = FontWeights.Light, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center };
            w.Show();
        }

        private void MenuWindowWithGlowOnClick(object sender, RoutedEventArgs e)
        {
            var w = this.GetTestWindow();
            w.Content = new Button() { Content = "MetroWindow with Glow", ToolTip = "And test tool tip", FontSize = 28, FontWeight = FontWeights.Light, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center };
            w.BorderThickness = new Thickness(1);
            w.BorderBrush = null;
            w.SetResourceReference(MetroWindow.GlowBrushProperty, "AccentColorBrush");
            w.Show();
        }

        private void MenuWindowWithShadowOnClick(object sender, RoutedEventArgs e)
        {
            var w = this.GetTestWindow();
            w.Content = new TextBlock() { Text = "Window with drop shadow", FontSize = 28, FontWeight = FontWeights.Light, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center };
            w.BorderThickness = new Thickness(0);
            w.BorderBrush = null;
            w.GlowBrush = Brushes.Black;
            w.Show();
        }

        private MetroWindow testWindow;

        private MetroWindow GetTestWindow()
        {
            if (testWindow != null)
            {
                testWindow.Close();
            }
            testWindow = new MetroWindow() { Owner = this, WindowStartupLocation = WindowStartupLocation.CenterOwner, Title = "Another Test...", Width = 500, Height = 300 };
            testWindow.Closed += (o, args) => testWindow = null;
            return testWindow;
        }

        private async void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (e.Cancel)
            {
                return;
            }

            e.Cancel = !_shutdown && _viewModel.QuitConfirmationEnabled;
            if (!e.Cancel)
            {
                return;
            }

            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Quit",
                NegativeButtonText = "Cancel",
                AnimateShow = true,
                AnimateHide = false
            };

            var result = await this.ShowMessageAsync("Quit application?",
                                                     "Are you sure want to quit application?",
                                                     MessageDialogStyle.AffirmativeAndNegative, mySettings);

            _shutdown = result == MessageDialogResult.Affirmative;

            if (_shutdown)
            {
                Application.Current.Shutdown();
            }
        }
    }
}
