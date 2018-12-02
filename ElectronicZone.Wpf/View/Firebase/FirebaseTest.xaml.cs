using ElectronicZone.Wpf.FireBaseModel;
using ElectronicZone.Wpf.ViewModel;
using System;
using System.Windows;

namespace ElectronicZone.Wpf.View.Firebase
{
    /// <summary>
    /// Interaction logic for FirebaseTest.xaml
    /// </summary>
    public partial class FirebaseTest : Window
    {
        // private readonly FirebaseApp _app;
        FirebaseProductViewModel vm = new FirebaseProductViewModel();
        public FirebaseTest()
        {
            InitializeComponent();
            this.DataContext = vm;
        }
    }
}
