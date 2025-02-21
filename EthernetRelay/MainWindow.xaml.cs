using System.Net.Sockets;
using System.Windows;
using System.Text;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Net;

namespace EthernetRelay
{
    public partial class MainWindow : Window
    {
        private ViewModelMainWindow viewModel = new ViewModelMainWindow();
        public MainWindow()
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}