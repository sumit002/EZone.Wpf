using ElectronicZone.Wpf.Helper;
using System;
using System.Data;
using System.Windows.Input;
using ElectronicZone.Wpf.Utility;
using ElectronicZone.Wpf.DataAccessLayer;
using ElectronicZone.Wpf.View;
using System.Windows;

namespace ElectronicZone.Wpf.ViewModel
{
    public class LoginViewModel : ViewModelBase // , ICloseable
    {
        // public Action DisplayInvalidLoginPrompt;
        ILogger logger = new Logger(typeof(LoginWindow));

        // Commands
        public ICommand MyCommand { get; set; }

        private string _email;
        public string TxtUserName
        {
            get { return _email; }
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }

        private string _password;
        public string TxtPassword
        {
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        public LoginViewModel()
        {
            MyCommand = new CommandHandler(MyAction, CanExecuteLoginClick);
        }

        private bool CanExecuteLoginClick(object parameter)
        {
            if (string.IsNullOrEmpty(_email))
            {
                // _email.Focus();
                return false;
            }
            else if (string.IsNullOrEmpty(_password))
            {
                // txtPassword.Focus();
                return false;
            }
            else
                return true;
        }

        public void MyAction(object parameter)
        {
            MessageBox.Show("Command Fired with no code behind");
            DataTable dt = new DataTable();
            DataAccess dataAccess = new DataAccess();
            try
            {
                //if (validateForm())
                //{
                    dt = dataAccess.ValidateUserLogin(_email.Trim(), _password.Trim());
                    if (dt.Rows.Count == 1)
                    {
                        //LoginWindow loginWindow = new LoginWindow();
                        //loginWindow.Hide();
                        // this.Hide();
                        Application.Current.MainWindow.Close();
                        DashboardWindow dashboardWindow = new DashboardWindow();
                        dashboardWindow.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show("Invalid Username or Password!");
                        logger.LogError("Invalid Username or Password");
                    }
                //}
                //else
                //{
                //    MessageBoxResult result = MessageBox.Show("Invalid Data ! Please check the fields entered.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                logger.LogException(ex);
            }
        }
    }
}
