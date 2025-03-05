using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;

namespace EthernetRelay
{
    public class RelayManager : INotifyPropertyChanged
    {
        private StatusConnect statusLaunching = StatusConnect.Unknown;
        private UdpClient client = new UdpClient();
        private string connectionStatusStr = "";
        private string patternIP = @"^\d{3}\.\d{3}\.\d\.\d{3}$";
        private string feedback = "";
        private byte[] feedbackByte;

        public event PropertyChangedEventHandler? PropertyChanged;
        public StatusConnect StatusLaunching {get => statusLaunching;}
        public string ConnectionStatusStr 
        {
            get => connectionStatusStr;
            private set
            {
                connectionStatusStr = value;
                OnPropertyChanged();
            }
        }
        public string PatternIP {get => patternIP;}
        public string Feedback 
        {
            get => feedback;
            private set
            {
                feedback = value;
                OnPropertyChanged();
            }
        }

        private void SendACommand(string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            int numberOfSentBytes = client.Send(data, data.Length);
        }

        private bool AcceptFeedback()
        {
            IPEndPoint ip = null;
            feedbackByte = client.Receive(ref ip);
            if (feedbackByte != null)
            {
                feedback = Encoding.UTF8.GetString(feedbackByte);
                return true;
            }
            return false;
        }

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public void GetState()
        {
            SendACommand(":03;");
        }

        public void Connect(Relay relay)
        {
            client.Client.ReceiveTimeout = 500;
            try
            {
                client.Connect(relay.Ip, relay.Port);
                GetState();
                AcceptFeedback();
                ConnectionStatus(StatusConnect.Connect);
            }
            catch (SocketException)
            {
                Feedback = "Ответа не получено";
            }
        }

        public void GetInputs(Relay Relay)
        {
            SendACommand(":02;");

            int start = 3;
            int stop = feedbackByte.Length - 2;
            int lenght = stop - start + 1;
            byte[] selectedBytes = new byte[lenght];
            Array.Copy(feedbackByte, start, selectedBytes, 0, lenght);

            Relay.Inputs.Clear();
            int a = 0;

            for (int i = 0; i <= lenght - 1; i++)
            {
                if (i % 2 == 1)
                {
                    if (selectedBytes[i] == 49)
                        Relay.Inputs.Add(true);
                    else
                        Relay.Inputs.Add(false);
                    a++;
                }
            }
        }

        public void Disconnect()
        {
            client.Close();
            ConnectionStatus(StatusConnect.Disconnect);
        }


        public void OnOffRelay(int nuumRele, bool IsChecked)
        {
            if (IsChecked == true)
            {
                SendACommand($":31 0{nuumRele} 01;");
            }
            else
            {
                SendACommand($":31 0{nuumRele} 00;");
            }
        }

        public void ConnectionStatus(StatusConnect statusConnect)
        {
            switch (statusConnect)
            {
                case StatusConnect.Unknown:
                    ConnectionStatusStr = "Unknown";
                    Feedback = "The IP or Port fields are empty";
                    break;
                case StatusConnect.Connect:
                    ConnectionStatusStr = "Connect";
                    Feedback = "Устройство подключено";
                    break;
                case StatusConnect.Disconnect:
                    ConnectionStatusStr = "Disconnect";
                    Feedback = "Устройство отключено";
                    break;
            }
        }
    }
}
