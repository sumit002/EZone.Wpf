using ElectronicZone.Wpf.DataAccessLayer;
using ElectronicZone.Wpf.Utility;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ElectronicZone.Wpf.View.Payment
{
    /// <summary>
    /// Interaction logic for PendingPayment.xaml
    /// </summary>
    public partial class PendingPayment : MetroWindow
    {
        ILogger logger = new Logger(typeof(PendingPayment));
        public PendingPayment()
        {
            InitializeComponent();
            loadSalesPerson();
            loadPendings();

            // on esc close
            this.PreviewKeyDown += new KeyEventHandler(HandleEsc);
        }

        private void loadSalesPerson()
        {
            DataTable dtPerson = new DataTable();
            DataAccess da = new DataAccess();
            dtPerson = da.GetAllSalesPerson();
            // bind to combobox
            cbSalesPerson.ItemsSource = dtPerson.DefaultView;
            cbSalesPerson.SelectedItem = null;
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
                loadPendings();
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
            }
        }

        private void loadPendings()
        {
            DataTable dtPendingPayment = new DataTable();
            DataAccess da = new DataAccess();
            dtPendingPayment = da.SearchPendingPayment(null, null, string.Empty, string.Empty
                , (this.cbSalesPerson.SelectedValue == null ? string.Empty : this.cbSalesPerson.SelectedValue.ToString()), 0);

            dataGridPendingPayment.ItemsSource = dtPendingPayment.DefaultView;
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
            // combobox
            this.cbSalesPerson.SelectedIndex = -1;
        }

        private void dataGridPendingPayment_SelectedCellsChanged_1(object sender, SelectedCellsChangedEventArgs e)
        {
            //Get the newly selected cells
            IList<DataGridCellInfo> selectedcells = e.AddedCells;

            DataRowView drv = (DataRowView)dataGridPendingPayment.SelectedItem;
            if (drv != null)
            {
                var selectedRow = drv.Row.ItemArray;

                //open modal for sale Item
                ClearPending clearPending = new ClearPending(selectedRow);
                clearPending.ShowDialog();

                //refresh pending data
                loadPendings();
            }
        }
    }
}
