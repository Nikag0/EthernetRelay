using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;

namespace EthernetRelay
{
    public class RelayManager : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        private byte[] feedbackByte;

        public byte[] FeedbackByte
        {
            get => feedbackByte;
            set
            {
                feedbackByte = value;
                OnPropertyChanged();
            }
        }


        private UdpClient client = new UdpClient();

        private string feedback = "";

        public string Feedback
        {
            get => feedback;
            set
            {
                feedback = value;
                OnPropertyChanged();
            }
        }

        private string connectionStatusStr = "";

        public string ConnectionStatusStr
        {
            get => connectionStatusStr;
            set
            {
                connectionStatusStr = value;
                OnPropertyChanged();
            }
        }

        private StatusConnect statusLaunching = StatusConnect.Unknown;
        public StatusConnect StatusLaunching { get => statusLaunching; }

        private string patternIP = @"^\d{3}\.\d{3}\.\d\.\d{3}$";

        public string PatternIP { get => patternIP; }

        public void GetState()
        {
            SendACommand(":03;");
        }

        public void Connect(Relay relay)
        {
            client.Connect(relay.Ip, relay.Port);
            GetState();
            AcceptFeedback();
            ConnectionStatus(StatusConnect.Connect);
        }

        public void GetInputs(Relay Relay)
        {
            SendACommand(":02;");
            AcceptFeedback();
            int start = 3;
            int stop = FeedbackByte.Length - 2;
            int lenght = stop - start + 1;
            byte[] selectedBytes = new byte[lenght];
            Array.Copy(FeedbackByte, start, selectedBytes, 0, lenght);

            int a = 0;

            Relay.Inputs = new bool[lenght / 2];

            for (int i = 0; i <= lenght - 1; i++)
            {
                if (i % 2 == 1)
                {
                    if (selectedBytes[i] == 49)
                        Relay.Inputs[a] = true;
                    else
                        Relay.Inputs[a] = false;
                    a++;
                }
            }
        }

        public void Disconnect()
        {
            client.Close();
            ConnectionStatus(StatusConnect.Disconnect);
            Feedback = "";
        }

        public void SendACommand(string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            int numberOfSentBytes = client.Send(data, data.Length);
        }

        public void AcceptFeedback()
        {
            IPEndPoint ip = null;
            FeedbackByte = client.Receive(ref ip);
            Feedback = Encoding.UTF8.GetString(feedbackByte);
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
                    break;
                case StatusConnect.Disconnect:
                    ConnectionStatusStr = "Disconnect";
                    break;
            }
        }
    }
}
