using NovinDevHubStaffCore.Core;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace NovinDevHubStaffCore
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IExpandable
    {
        const int WindowWidth = 390;
        bool isExpanded = false;
        DispatcherTimer activityTimer;

        public MainWindow()
        {
            InitializeComponent();
            this.Width = WindowWidth;
            this.DataContext = new MainWindowViewModel(this);
        }

        public void Expand()
        {
            isExpanded = true;
            btnExpandWindowIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.ChevronDoubleRight;
            this.Width = WindowWidth * 2.7;
        }

        public void Toggle()
        {
            if (!isExpanded)
            {
                btnExpandWindowIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.ChevronDoubleLeft;
                this.Width = WindowWidth * 2.7;
            }
            else
            {
                btnExpandWindowIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.ChevronDoubleRight;
                this.Width = WindowWidth;
            }
            isExpanded = !isExpanded;
        }

        private void btnExpandWindow_Click(object sender, RoutedEventArgs e)
        {
            Toggle();
        }

        private void btnFullscreen_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState != WindowState.Maximized)
            {
                btnFullScreenIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.FullscreenExit;
                this.WindowState = WindowState.Maximized;
                //isExpanded = true;
                //btnExpandWindowIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.ChevronDoubleLeft;
            }
            else
            {
                btnFullScreenIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Fullscreen;
                this.WindowState = WindowState.Normal;
            }
        }


      
    }
}
