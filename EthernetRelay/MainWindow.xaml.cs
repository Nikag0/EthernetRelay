using System.Net.Sockets;
using System.Net;
using System.Windows;
using System.Text;

namespace EthernetRelay
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            using var udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            string message = "Hello METANIT.COM";
            byte[] data = Encoding.UTF8.GetBytes(message);
            EndPoint remotePoint = new IPEndPoint(IPAddress.Parse("192.168.1.203"), 1200);
            int bytes = await udpSocket.SendToAsync(data, remotePoint);
        }
    }
}