using System;
using ElectronicZone.Wpf.Model;
using ElectronicZone.Wpf.Helper;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Data;
using ElectronicZone.Wpf.DataAccessLayer;
using System.Collections.Generic;
using System.Windows;

namespace ElectronicZone.Wpf.ViewModel
{
    public class ContactViewModel : ViewModelBase
    {
        public ObservableCollection<Contact> ContactList { get; set; }
        public ObservableCollection<String> SalutationList { get; set; }
        private bool downloadExcelVisibility;
        private int _selectedIndex;
        private Contact _selectedResult;

        #region  UI Models
        private int _id;
        // private Salutation _title;
        private string _name;
        private string _contact;
        private string _altContact;
        private string _email;
        private string _address;

        public int Id { get => _id; set => _id = value; }
        public string Name { get => _name; set => _name = value; }
        public string PrimaryContact { get => _contact; set => _contact = value; }
        public string AltContact { get => _altContact; set => _altContact = value; }
        public string Email { get => _email; set => _email = value; }
        public string Address { get => _address; set => _address = value; }
        #endregion


        // Actions/Commands
        public ICommand DeleteContactCmd { get; set; }
        public ICommand EditContactCmd { get; set; }
        public ICommand ResetContactCmd { get; set; }
        private ICommand AddContactCmd { get; set; }

        public ICommand SearchContactCmd { get; set; }

        public Contact SelectedResult { get => _selectedResult; set => _selectedResult = value; }
        public int TabSelectedIndex {
            get { return _selectedIndex; }

            set { _selectedIndex = value;
                    GetAllContacts();  }
        }

        public bool DownloadExcelVisibility { get => downloadExcelVisibility; set => downloadExcelVisibility = value; }
        

        public ContactViewModel()
        {
            this.ContactList = new ObservableCollection<Contact>();
            this.SalutationList = new ObservableCollection<string>();
            this._selectedIndex = 0;
            this.downloadExcelVisibility = false;

            DeleteContactCmd = new CommandHandler(DeleteContact, CanExecuteDeleteContactCmd);
            EditContactCmd = new CommandHandler(EditContact, CanExecuteEditContactCmd);
            ResetContactCmd = new CommandHandler(ResetForm, CanExecuteResetContactCmd);
            AddContactCmd = new CommandHandler(AddContact, CanExecuteAddContactCmd);
            SearchContactCmd = new CommandHandler(SearchContact, CanExecuteSearchContactCmd);

            // GetAllContacts();
            LoadSalutation();
        }

        private bool CanExecuteEditContactCmd(object arg)
        {
            return true;
        }

        private void EditContact(object param)
        {
            var contactPerson = (Contact)param;
            // ContactList.Remove((Contact)param);
            this.Id = contactPerson.Id;
            this.Name = contactPerson.Name;
            this.PrimaryContact = contactPerson.PrimaryContact;
            this.AltContact = contactPerson.AltContact;
            this.Email = contactPerson.Email;
            this.Address = contactPerson.Address;

            this._selectedIndex = 0;
        }

        private void SearchContact(object obj)
        {
            // throw new NotImplementedException();
            GetAllContacts();
            this.downloadExcelVisibility = true;
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
            //if (string.IsNullOrEmpty(txtName.Text.Trim()))
            //{
            //    //txtName.Focus();
            //    return false;
            //}
            //else if (string.IsNullOrEmpty(txtContact.Text.Trim()))
            //{
            //    //txtContact.Focus();
            //    return false;
            //}
            //else
                return true;
        }

        private void AddContact(object obj)
        {
            MessageBoxResult result = MessageBox.Show("AddContact clicked", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            ////create record
            //Dictionary<string, string> folderFields = new Dictionary<string, string>();
            //folderFields.Add("Id", null);
            //folderFields.Add("Title", (cbSalutation.SelectedValue == null ? string.Empty : cbSalutation.SelectedValue.ToString()));
            //folderFields.Add("Name", txtName.Text);
            //folderFields.Add("Contact", txtContact.Text);
            //folderFields.Add("AlternateContact", txtAltContact.Text);
            //folderFields.Add("Email", txtEmail.Text);
            //folderFields.Add("Address", txtAddress.Text);

            //DataAccess dataAccess = new DataAccess();
            //int status = dataAccess.InsertOrUpdateSalePerson(folderFields, "tblSalePerson");
            ////check if it is insert/updated
            //if (status > 0)
            //{
            //    MessageBoxResult result = MessageBox.Show("Contact Added Successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            //    ResetForm(new object());
            //}
            //else
            //{
            //    MessageBoxResult result = MessageBox.Show("Error While Adding Contact!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            //}
        }

        private void ResetForm(object obj)
        {
            //this.txtName.Text = "";
            //this.txtContact.Text = "";
            //this.txtAltContact.Text = "";
            //this.txtEmail.Text = "";
            //this.txtAddress.Text = "";
            // this.cbSalutation.SelectedIndex = -1;
        }

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
            DataAccess da = new DataAccess();
            da.DeleteSalesPerson(((Contact)param).Id);
            ContactList.Remove((Contact)param);
        }

        /// <summary>
        /// Get All Contacts
        /// </summary>
        private void GetAllContacts()
        {
            DataTable dt = new DataTable();
            DataAccess da = new DataAccess();
            dt = da.GetAllSalesPerson();

            foreach (DataRow row in dt.Rows)
            {
                ContactList.Add(new Contact()
                {
                    Id = int.Parse(row["Id"].ToString()),
                    // Title = (Contact.Salutation)row["Title"],
                    Name = (string)row["Name"],
                    PrimaryContact = Convert.ToString(row["Contact"]),
                    AltContact = Convert.ToString(row["AlternateContact"]),
                    Email = (string)row["Email"],
                    Address = (string)row["Address"]
                    ,IsActive = (bool)row["IsActive"]
                });
            }
        }
    }
}
