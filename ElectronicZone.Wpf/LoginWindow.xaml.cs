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
        ILogger logger = new Logger(typeof(LoginWindow));

        public LoginWindow()
        {
            InitializeComponent();
            LoadUIControls();
            OnLoginLoad();
        }

        private void LoadUIControls()
        {
            //string asd = _ezContext.GetCultureInfo.m_resourceManger.GetString("LoginPasswordLabel");
            lblUsername.Content = "Username";// _ezContext.GetCultureInfo.m_resourceManger.GetString("LoginPasswordLabel");
        }

        private bool validateForm()
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
            // this.txtUsername.Focus();
            txtUsername.Text = "admin";
            txtPassword.Password = "123456";
            this.btnLogin.Focus();
        }
        /// <summary>
        /// on login button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            DataTable dt = new DataTable();
            DataAccess dataAccess = new DataAccess();
            try
            {
                if (validateForm())
                {
                    dt = dataAccess.ValidateUserLogin(this.txtUsername.Text.Trim(), this.txtPassword.Password.Trim());
                    if (dt.Rows.Count == 1)
                    {
                        this.Hide();
                        // setting global variables
                        Global.UserId = Convert.ToInt32(dt.Rows[0]["Id"]);
                        Global.Name = Convert.ToString(dt.Rows[0]["Name"]);
                        DashboardWindow dashboard = new DashboardWindow();
                        dashboard.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show("Invalid Username or Password!");
                        logger.LogError(string.Format("Invalid Username {0} or Password {1}", this.txtUsername.Text, this.txtPassword.Password));
                    }
                }
                else
                {
                    MessageBoxResult result = MessageBox.Show("Invalid Data ! Please check the fields entered.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                logger.LogException(ex);
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
                logger.LogException(ex);
            }

        }
    }
}
