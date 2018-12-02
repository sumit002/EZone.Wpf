using ElectronicZone.Wpf.ViewModel;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace ElectronicZone.Wpf.View.Payment
{
    /// <summary>
    /// Interaction logic for PendingPayment.xaml
    /// </summary>
    public partial class PendingPayment : MetroWindow
    {
        PendingPaymentViewModel vm = new PendingPaymentViewModel(DialogCoordinator.Instance);
        //ILogger logger = new Logger(typeof(PendingPayment));
        public PendingPayment()
        {
            InitializeComponent();
            this.DataContext = vm;

            //LoadSalesPerson();
            ////loadPendings();

            //// on esc close
            //this.PreviewKeyDown += new KeyEventHandler(HandleEsc);
        }

        //private void LoadSalesPerson()
        //{
        //    DataTable dtPerson = new DataTable();
        //    using (DataAccess da = new DataAccess()) {
        //        dtPerson = da.GetAllSalesPerson();
        //    }
        //    // bind to combobox
        //    cbSalesPerson.ItemsSource = dtPerson.DefaultView;
        //    cbSalesPerson.SelectedItem = null;
        //}

        //private void HandleEsc(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.Escape)
        //        Close();
        //}

        //private void btnSearch_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        LoadPendings();
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.LogException(ex);
        //    }
        //}

        //private void LoadPendings()
        //{
        //    DataTable dtPendingPayment = new DataTable();
        //    using (DataAccess da = new DataAccess())
        //    {
        //        dtPendingPayment = da.SearchPendingPayment(null, null, string.Empty, string.Empty
        //            , (this.cbSalesPerson.SelectedValue == null ? string.Empty : this.cbSalesPerson.SelectedValue.ToString()), 0);
        //    }
        //    dataGridPendingPayment.ItemsSource = dtPendingPayment.DefaultView;
        //}

        //private void btnReset_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        ResetForm();
        //        dataGridPendingPayment.ItemsSource = null;
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.LogException(ex);
        //    }
        //}

        //private void ResetForm()
        //{
        //    // combobox
        //    this.cbSalesPerson.SelectedIndex = -1;
        //}

        //private void dataGridPendingPayment_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        //{
        //    //Get the newly selected cells
        //    IList<DataGridCellInfo> selectedcells = e.AddedCells;

        //    DataRowView drv = (DataRowView)dataGridPendingPayment.SelectedItem;
        //    if (drv != null)
        //    {
        //        var selectedRow = drv.Row.ItemArray;

        //        //open modal for sale Item
        //        ClearPending clearPending = new ClearPending(selectedRow);
        //        clearPending.ShowDialog();

        //        //refresh pending data
        //        LoadPendings();
        //    }
        //}
    }
}
