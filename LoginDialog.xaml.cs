using Microsoft.Toolkit.Mvvm.DependencyInjection;
using NovinDevHubStaffCore.Core;
using NovinDevHubStaffCore.Services;
using NovinDevHubStaffCore.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NovinDevHubStaffCore
{
    /// <summary>
    /// Interaction logic for LoginDialog.xaml
    /// </summary>
    public partial class LoginDialog : Window, ICloseable
    {
        public LoginDialog()
        {
            InitializeComponent();
            this.DataContext = new LoginDialogViewModel(this);
        }

        public void Close(bool result)
        {
            this.DialogResult = result;
            this.Close();
        }

        private void password_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ((LoginDialogViewModel)DataContext).Password = new System.Net.NetworkCredential(string.Empty, ((PasswordBox)sender).SecurePassword).Password ;
        }
    }
}
