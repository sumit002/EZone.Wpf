using ElectronicZone.Wpf.ViewModel;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace ElectronicZone.Wpf.View.Master
{
    /// <summary>
    /// Interaction logic for ContactMaster.xaml
    /// </summary>
    public partial class ContactMaster : MetroWindow
    {
        ContactViewModel vm = new ContactViewModel(DialogCoordinator.Instance);

        //ILogger logger = new Logger(typeof(ContactMaster));
        //DataTable dtContacts = new DataTable();
        public ContactMaster()
        {
            InitializeComponent();
            this.DataContext = vm;

            // this.cbSalutation.Focus();
            // loadSalutation();
        }

        //private void tabControl1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (e.Source is TabControl)
        //    {
        //        //bind brand list on demand
        //        if (tabControl1.SelectedIndex != 0)
        //            loadContacts();
        //    }
        //}

        //private void loadContacts()
        //{
        //    // DataTable dtContacts = new DataTable();
        //    DataAccess da = new DataAccess();
        //    if (dtContacts.Rows.Count == 0)
        //        dtContacts = da.GetAllSalesPerson();
        //    datagridContacts.ItemsSource = dtContacts.DefaultView;
        //}

        //private void btnReset_Click(object sender, RoutedEventArgs e)
        //{
        //    ResetForm();
        //}

        //private void btnSave_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        if (validateForm())
        //        {
        //            //create record
        //            Dictionary<string, string> folderFields = new Dictionary<string, string>();
        //            folderFields.Add("Id", null);
        //            folderFields.Add("Title", (cbSalutation.SelectedValue == null ? string.Empty : cbSalutation.SelectedValue.ToString()));
        //            folderFields.Add("Name", txtName.Text);
        //            folderFields.Add("Contact", txtContact.Text);
        //            folderFields.Add("AlternateContact", txtAltContact.Text);
        //            folderFields.Add("Email", txtEmail.Text);
        //            folderFields.Add("Address", txtAddress.Text);

        //            DataAccess dataAccess = new DataAccess();
        //            int status = dataAccess.InsertOrUpdateSalePerson(folderFields, "tblSalePerson");
        //            //check if it is insert/updated
        //            if (status > 0)
        //            {
        //                MessageBoxResult result = MessageBox.Show("Contact Added Successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        //                ResetForm();
        //            }
        //            else
        //            {
        //                MessageBoxResult result = MessageBox.Show("Error While Adding Contact!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //            }
        //        }
        //        else
        //        {
        //            MessageBoxResult result = MessageBox.Show((string)Application.Current.FindResource("InvalidFormDataWarningMessage"), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.LogException(ex);
        //    }
        //}

        //private void loadSalutation()
        //{
        //    List<string> salutationLst = new List<string>();
        //    Contact con = new Contact();
        //    // salutationLst = con.GetSalutationList();
        //    //salutationLst.Add("Mr");
        //    //salutationLst.Add("Ms");
        //    //salutationLst.Add("Miss");
        //    // cbSalutation.ItemsSource = salutationLst;
        //    //cbSalutation.SelectedIndex = 0;
        //}

        //private bool validateForm()
        //{
        //    if (string.IsNullOrEmpty(txtName.Text.Trim()))
        //    {
        //        txtName.Focus();
        //        return false;
        //    }
        //    else if (string.IsNullOrEmpty(txtContact.Text.Trim()))
        //    {
        //        txtContact.Focus();
        //        return false;
        //    }
        //    else
        //        return true;
        //}

        //private void ResetForm()
        //{
        //    this.txtName.Text = "";
        //    this.txtContact.Text = "";
        //    this.txtAltContact.Text = "";
        //    this.txtEmail.Text = "";
        //    this.txtAddress.Text = "";
        //    this.cbSalutation.SelectedIndex = -1;
        //}
    }
}
