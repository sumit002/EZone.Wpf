﻿using System;
using System.Data;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using ElectronicZone.Wpf.Utility;
using ElectronicZone.Wpf.DataAccessLayer;
using System.Configuration;

namespace ElectronicZone.Wpf.View.Report
{
    /// <summary>
    /// Interaction logic for PendingPaymentReport.xaml
    /// </summary>
    public partial class PendingPaymentReport : MetroWindow
    {
        ILogger logger = new Logger(typeof(PendingPaymentReport));
        public PendingPaymentReport()
        {
            InitializeComponent();
            LoadSalesPerson();

            // on esc close
            this.PreviewKeyDown += new KeyEventHandler(HandleEsc);
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
                DataTable dtPendingPayment = new DataTable();
                using (DataAccess da = new DataAccess())
                {
                    dtPendingPayment = da.SearchPendingPayment((int?)this.txtPriceFrom.Value, (int?)this.txtPriceTo.Value,
                        string.IsNullOrEmpty(fromDate.Text) ? "" : (DateTime.Parse(fromDate.Text).ToString(ConfigurationManager.AppSettings["DateOnly"])),
                        string.IsNullOrEmpty(toDate.Text) ? "" : (DateTime.Parse(toDate.Text).ToString(ConfigurationManager.AppSettings["DateOnly"])),
                        (this.cbSalesPerson.SelectedValue == null ? string.Empty : this.cbSalesPerson.SelectedValue.ToString()),
                        (this.chkbPaid.IsChecked == null ? 0 : this.chkbPaid.IsChecked == true ? 1 : 0));
                }

                if (dtPendingPayment.Rows.Count > 0) {
                    btnExport.Visibility = System.Windows.Visibility.Visible;
                    dataGridPendingPayment.ItemsSource = dtPendingPayment.DefaultView;
                }
                else
                {
                    btnExport.Visibility = System.Windows.Visibility.Hidden;
                    dataGridPendingPayment.ItemsSource = null;
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
                dataGridPendingPayment.ItemsSource = null;
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
            }
        }

        private void ResetForm()
        {
            // textbox
            this.txtPriceFrom.Value = null;
            this.txtPriceTo.Value = null;
            //date picker
            this.fromDate.Text = "";
            this.toDate.Text = "";
            this.chkbPaid.IsChecked = false;
            this.cbSalesPerson.SelectedIndex = -1;
            // hide export button
            btnExport.Visibility = System.Windows.Visibility.Hidden;
        }
        private void LoadSalesPerson()
        {
            DataTable dtPerson = new DataTable();
            DataAccess da = new DataAccess();
            dtPerson = da.GetAllSalesPerson();
            // bind to combobox
            cbSalesPerson.ItemsSource = dtPerson.DefaultView;
            cbSalesPerson.SelectedItem = null;
            //cbSalesPerson.SelectedIndex = -1;
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
                //ex.Message;
                logger.LogException(ex);
            }
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            goExcelOut goExcelOut = new goExcelOut();
            bool result = goExcelOut.GenerateExcelOutput(dataGridPendingPayment, "PendingPaymentReport");
            //if (result)
            //    MessageBox.Show("File exported successfully.");
        }
    }
}
