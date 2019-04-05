using ElectronicZone.Wpf.DataAccessLayer;
using ElectronicZone.Wpf.Utility;
using MahApps.Metro.Controls;
using System;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ElectronicZone.Wpf.View.Report
{
    /// <summary>
    /// Interaction logic for SupportPaymentReport.xaml
    /// </summary>
    public partial class SupportPaymentReport : MetroWindow
    {
        ILogger logger = new Logger(typeof(SupportPaymentReport));
        public SupportPaymentReport()
        {
            InitializeComponent();

            DateTimeUtility dtUtility = new DateTimeUtility();
            fromDate.SelectedDate = dtUtility.GetMonthStartDate(null);
            toDate.SelectedDate = DateTime.Now;

            // on esc close
            this.PreviewKeyDown += new KeyEventHandler(HandleEsc);
            this.txtDescription.Focus();
        }

        private void HandleEsc(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataTable dtSupportPayment = new DataTable();
                using (DataAccess da = new DataAccess()) {
                    dtSupportPayment = da.SearchSupportPayment((int?)null, (int?)null,
                        string.IsNullOrEmpty(fromDate.Text) ? "" : (DateTime.Parse(fromDate.Text).ToString(ConfigurationManager.AppSettings["DateOnly"])),
                        string.IsNullOrEmpty(toDate.Text) ? "" : (DateTime.Parse(toDate.Text).ToString(ConfigurationManager.AppSettings["DateOnly"])), this.txtDescription.Text);
                }

                if (dtSupportPayment.Rows.Count > 0)
                {
                    btnExport.Visibility = System.Windows.Visibility.Visible;
                    dataGridSupportPayment.ItemsSource = dtSupportPayment.DefaultView;
                }
                else
                {
                    btnExport.Visibility = System.Windows.Visibility.Hidden;
                    dataGridSupportPayment.ItemsSource = null;
                    MessageBoxResult result = MessageBox.Show((string)Application.Current.FindResource("NoDataFoundInfoMessage"), "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
            }
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ResetForm();
                dataGridSupportPayment.ItemsSource = null;
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
            }
        }

        private void ResetForm()
        {
            this.fromDate.Text = "";
            this.toDate.Text = "";
            this.txtDescription.Text = "";
            btnExport.Visibility = System.Windows.Visibility.Hidden;
        }

        private void fromDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            ValidateCalendarMinMaxDate();
        }

        private void toDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            ValidateCalendarMinMaxDate();
        }

        private void ValidateCalendarMinMaxDate()
        {
            try
            {
                this.fromDate.DisplayDateEnd = string.IsNullOrEmpty(toDate.Text) ? (DateTime?)null : DateTime.Parse(toDate.Text);
                this.toDate.DisplayDateStart = string.IsNullOrEmpty(fromDate.Text) ? (DateTime?)null : DateTime.Parse(fromDate.Text);
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
            }
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            goExcelOut goExcelOut = new goExcelOut();
            bool result = goExcelOut.GenerateExcelOutput(dataGridSupportPayment, "SupportPaymentReport");
            //if (result)
            //    MessageBox.Show("File exported successfully.");
        }
    }
}
