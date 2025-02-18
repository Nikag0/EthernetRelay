using System.Net.Sockets;
using System.Windows;
using System.Text;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using System.Windows.Controls;

namespace EthernetRelay
{
    public partial class MainWindow : Window
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        private bool relay = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void CheckBoxRelay1Click(object sender, RoutedEventArgs e)
        {

            if (CheckBoxRelay1.IsChecked == true)
            {
                UdpClient client = new UdpClient();
                client.Connect("192.168.1.203", 1200);
                string message = ":01 01 00 00 00;";
                byte[] data = Encoding.UTF8.GetBytes(message);
                int numberOfSentBytes = client.Send(data, data.Length);
            }
            else
            {
                UdpClient client = new UdpClient();
                client.Connect("192.168.1.203", 1200);
                string message = ":01 00 00 00 00;";
                byte[] data = Encoding.UTF8.GetBytes(message);
                int numberOfSentBytes = client.Send(data, data.Length);
            }
        }

        private void CheckBoxRelay2Click(object sender, RoutedEventArgs e)
        {

            if (CheckBoxRelay2.IsChecked == true)
            {
                UdpClient client = new UdpClient();
                client.Connect("192.168.1.203", 1200);
                string message = ":01 00 01 00 00;";
                byte[] data = Encoding.UTF8.GetBytes(message);
                int numberOfSentBytes = client.Send(data, data.Length);
            }
            else
            {
                UdpClient client = new UdpClient();
                client.Connect("192.168.1.203", 1200);
                string message = ":01 00 00 00 00;";
                byte[] data = Encoding.UTF8.GetBytes(message);
                int numberOfSentBytes = client.Send(data, data.Length);
            }
        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            if (!relay)
            {
                UdpClient client = new UdpClient();
                client.Connect("192.168.1.203", 1200);
                string message = ":01 01 01 00 00;";
                byte[] data = Encoding.UTF8.GetBytes(message);
                int numberOfSentBytes = client.Send(data, data.Length);
                ButtonOnOff.Content = ("Click to Off");
                relay = true;
            }
            else
            {
                UdpClient client = new UdpClient();
                client.Connect("192.168.1.203", 1200);
                string message = ":01 00 00 00 00;";
                byte[] data = Encoding.UTF8.GetBytes(message);
                int numberOfSentBytes = client.Send(data, data.Length);
                ButtonOnOff.Content = ("Click to On");
                relay = false;
            }
        }
    }
}