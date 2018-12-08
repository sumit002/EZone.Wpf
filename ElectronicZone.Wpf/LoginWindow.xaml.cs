using System;
using System.Data;
using System.Windows;
using MahApps.Metro.Controls;
using ElectronicZone.Wpf.View;
using ElectronicZone.Wpf.Utility;
using ElectronicZone.Wpf.DataAccessLayer;

namespace ElectronicZone.Wpf
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : MetroWindow
    {
        //private readonly Context _ezContext;
        ILogger _logger = new Logger(typeof(LoginWindow));

        public LoginWindow()
        {
            InitializeComponent();
            //_logger.LogInfoMessage("Initialising Login Page ...");
            //LoadUIControls();
            OnLoginLoad();
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrEmpty(txtUsername.Text.Trim()))
            {
                txtUsername.Focus();
                return false;
            }
            else if (string.IsNullOrEmpty(txtPassword.Password.Trim()))
            {
                txtPassword.Focus();
                return false;
            }
            else
                return true;
        }

        private void OnLoginLoad()
        {
            this.txtUsername.Focus();
            //txtUsername.Text = "admin";
            //txtPassword.Password = "123456";
            //this.btnLogin.Focus();
        }
        /// <summary>
        /// on login button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            _logger.LogInfoMessage($"Login Button Clicked with username : {this.txtUsername.Text.Trim()}");
            try
            {
                if (ValidateForm()) {
                    ValidateUserLogin();
                }
                else {
                    MessageBoxResult result = MessageBox.Show("Invalid Data ! Please check the fields entered.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                _logger.LogException(ex);
            }
        }

        private void ValidateUserLogin()
        {
            DataTable dt = new DataTable();
            using (DataAccess da = new DataAccess())
            { dt = da.ValidateUserLogin(this.txtUsername.Text.Trim(), this.txtPassword.Password.Trim()); }
                
            if (dt.Rows.Count == 1) {
                this.Hide();
                // setting global variables
                Global.UserId = Convert.ToInt32(dt.Rows[0]["Id"]);
                Global.Name = Convert.ToString(dt.Rows[0]["Name"]);
                Global.UserName = Convert.ToString(dt.Rows[0]["Username"]);
                Global.IsAdmin = Convert.ToBoolean(dt.Rows[0]["IsAdmin"]);

                MainWindow _window = new MainWindow();
                _window.ShowDialog();
                //DashboardWindow dashboard = new DashboardWindow();
                //dashboard.ShowDialog();
            }
            else
            {
                MessageBox.Show("Invalid Username or Password!");
                _logger.LogError(string.Format("Invalid Username {0} or Password {1}", this.txtUsername.Text, this.txtPassword.Password));
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
            }

        }
    }
}
