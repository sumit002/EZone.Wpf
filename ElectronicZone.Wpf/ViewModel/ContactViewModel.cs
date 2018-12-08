using System;
using ElectronicZone.Wpf.Model;
using ElectronicZone.Wpf.Helper;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Data;
using ElectronicZone.Wpf.DataAccessLayer;
using System.Collections.Generic;
using System.Windows;
using MahApps.Metro.Controls.Dialogs;
using System.Threading.Tasks;
using ElectronicZone.Wpf.Utility;

namespace ElectronicZone.Wpf.ViewModel
{
    public class ContactViewModel : ViewModelBase
    {
        #region Properties
        ILogger logger = new Logger(typeof(ContactViewModel));
        public ObservableCollection<Contact> ContactList { get; set; }
        public ObservableCollection<String> SalutationList { get; set; }
        private int _selectedIndex;
        private Contact _selectedResult;
        private IDialogCoordinator _dialogCoordinator;
        private readonly string _tableToUse = "tblSalePerson";
        #endregion

        #region  UI Models
        private int _id;
        // private Salutation _title;
        private string _name;
        private string _contact;
        private string _altContact;
        private string _email;
        private string _address;
        private bool _downloadExcelVisibility;

        public int Id { get => _id; set => _id = value; }
        public string Name { get => _name; set { _name = value; OnPropertyChanged(); } }
        public string PrimaryContact { get => _contact; set { _contact = value; OnPropertyChanged(); } }
        public string AltContact { get => _altContact; set { _altContact = value; OnPropertyChanged(); } }
        public string Email { get => _email; set { _email = value; OnPropertyChanged(); } }
        public string Address { get => _address; set { _address = value; OnPropertyChanged(); } }
        public Contact SelectedResult { get => _selectedResult; set => _selectedResult = value; }
        public bool DownloadExcelVisibility { get => _downloadExcelVisibility; set { _downloadExcelVisibility = value; OnPropertyChanged(); } }

        private string _sSalutation;
        public string SSalutation
        {
            get { return _sSalutation; }
            set { _sSalutation = value; OnPropertyChanged(); }
        }

        public int TabSelectedIndex
        {
            get { return _selectedIndex; }

            set
            {
                _selectedIndex = value;
                OnPropertyChanged();
                if (_selectedIndex == 1)  GetAllContacts(null, null, null, null);
            }
        }

        #endregion

        #region Commands
        public ICommand DeleteContactCmd { get; set; }
        public ICommand EditContactCmd { get; set; }
        public ICommand ResetContactCmd { get; set; }
        //private ICommand AddOrUpdateContactCmd { get; set; }
        public ICommand SearchContactCmd { get; set; }
        public ICommand CopyToClipboardCmd { get; set; }
        public ICommand ContactAddUpdateCmd { get; set; }
        #endregion

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="instance"></param>
        public ContactViewModel(IDialogCoordinator instance)
        {
            this._dialogCoordinator = instance;
            this.ContactList = new ObservableCollection<Contact>();
            this.SalutationList = new ObservableCollection<string>();
            this.TabSelectedIndex = 0;
            this.DownloadExcelVisibility = false;

            DeleteContactCmd = new CommandHandler(DeleteContact, CanExecuteDeleteContactCmd);
            EditContactCmd = new CommandHandler(EditContact, CanExecuteEditContactCmd);
            ResetContactCmd = new CommandHandler(ResetForm, CanExecuteResetContactCmd);

            //AddOrUpdateContactCmd = new CommandHandler(AddContact, CanExecuteAddContactCmd);
            SearchContactCmd = new CommandHandler(SearchContact, CanExecuteSearchContactCmd);
            CopyToClipboardCmd = new CommandHandler(CopyContactToClipboard, CanExecuteCopyContactToClipboardCmd);

            ContactAddUpdateCmd = new CommandHandler(AddOrUpdateContact, CanExecuteAddContactCmd);
            // GetAllContacts();
            LoadSalutation();
        }

        private void CopyContactToClipboard(object obj)
        {
            var contactPerson = (Contact)obj;
            Utility.CommonMethods.SetTextToClipboard($"{contactPerson.Name} {contactPerson.PrimaryContact}");
        }

        private bool CanExecuteCopyContactToClipboardCmd(object arg)
        {
            return true;
        }

        private bool CanExecuteEditContactCmd(object arg)
        {
            return true;
        }

        private void EditContact(object param)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var contactPerson = (Contact)param;
                // ContactList.Remove((Contact)param);
                this.Id = contactPerson.Id;
                this.Name = contactPerson.Name;
                this.PrimaryContact = contactPerson.PrimaryContact;
                this.AltContact = contactPerson.AltContact;
                this.Email = contactPerson.Email;
                this.Address = contactPerson.Address;
            });
            this.TabSelectedIndex = 0;
        }

        private void SearchContact(object obj)
        {
            GetAllContacts(this.Name, this.PrimaryContact, this.Email, this.Address);
            this.DownloadExcelVisibility = true;
        }

        private bool CanExecuteSearchContactCmd(object arg)
        {
            return true;
        }

        private bool CanExecuteResetContactCmd(object arg)
        {
            return true;
        }

        private bool CanExecuteAddContactCmd(object arg)
        {
            if (string.IsNullOrEmpty(this.Name))
            {
                //this.Name.Focus();
                return false;
            }
            else if (string.IsNullOrEmpty(this.PrimaryContact))
            {
                return false;
            }
            else
                return true;
        }

        /// <summary>
        /// Validate Contact
        /// </summary>
        /// <returns></returns>
        private bool ValidateForm(DataAccess da)
        {
            if (string.IsNullOrEmpty(this.Name)) {
                return false;
            }
            else if (string.IsNullOrEmpty(this.PrimaryContact))
            {
                return false;
            }
            else if (this.Id == 0 && !string.IsNullOrEmpty(this.Name) && !string.IsNullOrEmpty(this.PrimaryContact) 
                && da.IfContactExists(_tableToUse, "Name", "Contact", this.Name, this.PrimaryContact))
            {
                MessageBoxResult result = MessageBox.Show("Contact Already Exists!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            else
                return true;
        }

        private void AddOrUpdateContact(object obj)
        {
            using (DataAccess da = new DataAccess())
            {
                try
                {
                    if (ValidateForm(da))
                    {
                        Dictionary<string, string> folderFields = new Dictionary<string, string>();
                        folderFields.Add("Id", this.Id == 0 ? null : this.Id.ToString());
                        folderFields.Add("Title", (SSalutation == null ? string.Empty : SSalutation));
                        folderFields.Add("Name", this.Name);
                        folderFields.Add("Contact", this.PrimaryContact);
                        folderFields.Add("AlternateContact", this.AltContact);
                        folderFields.Add("Email", this.Email);
                        folderFields.Add("Address", this.Address);

                        int status = da.InsertOrUpdateSalePerson(folderFields, _tableToUse);
                        //check if it is insert/updated
                        if (status > 0) {
                            MessageBoxResult result = MessageBox.Show("Contact Updated Successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            ResetForm(new object());
                        }
                        else
                        {
                            MessageBoxResult result = MessageBox.Show("Error While Adding Contact!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        MessageBoxResult result = MessageBox.Show("Invalid Data ! Please check the fields entered.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex) {
                    logger.LogException(ex);
                    da.RollbackTransaction();
                }
            }
        }
        /// <summary>
        /// Reset Contact Fields
        /// </summary>
        /// <param name="obj"></param>
        private void ResetForm(object obj)
        {
            this.Id = 0;
            this.Name = "";
            this.PrimaryContact = "";
            this.AltContact = "";
            this.Email = "";
            this.Address = "";
            this.SSalutation = null;

            this.DownloadExcelVisibility = false;
            this.ContactList.Clear();
        }
        /// <summary>
        /// Load Salutation/Title
        /// </summary>
        private void LoadSalutation()
        {
            SalutationList = Contact.GetSalutationObservableCollection();
        }

        private bool CanExecuteDeleteContactCmd(object arg)
        {
            return SelectedResult != null;
        }

        private void DeleteContact(object param)
        {
            // Show...
            //ProgressDialogController controller = await dialogCoordinator.ShowProgressAsync(this, "HEADER", "MESSAGE");
            //controller.SetIndeterminate();

            if (MessageBox.Show("Are you sure?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) == MessageBoxResult.Yes)
            {
                // Do your work... 
                using (DataAccess da = new DataAccess()) {
                    da.DeleteSalesPerson(((Contact)param).Id);
                }
                ContactList.Remove((Contact)param);
            }

            // Close...
            //await controller.CloseAsync();
        }

        /// <summary>
        /// Get All Contacts
        /// </summary>
        private async void GetAllContacts(string name, string contact, string email, string address)
        {
            var controller = await _dialogCoordinator.ShowProgressAsync(this, "Loading", "Please wait for a while...");
            controller.SetIndeterminate();

            DataTable dt = new DataTable();
            using (DataAccess da = new DataAccess()) {
                dt = da.SearchSalesPerson(name, contact, email, address);
            }
            this.ContactList.Clear();
            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (DataRow row in dt.Rows)
                {
                    ContactList.Add(new Contact()
                    {
                        Id = int.Parse(row["Id"].ToString()),
                        // Title = (Utility.CommonEnum.Salutation)row["Title"],
                        Name = (string)row["Name"],
                        PrimaryContact = Convert.ToString(row["Contact"]),
                        AltContact = Convert.ToString(row["AlternateContact"]),
                        Email = Convert.ToString(row["Email"]),
                        // EmailUri = new Uri(row["Email"].ToString()),
                        Address = Convert.ToString(row["Address"]),
                        IsActive = (bool)row["IsActive"],
                        CanDelete = !Convert.ToBoolean(row["IsUsed"])
                    });
                }
            });
            
            await controller.CloseAsync();
        }
    }
}
