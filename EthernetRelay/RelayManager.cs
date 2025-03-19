using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;

namespace EthernetRelay
{
    public class RelayManager : INotifyPropertyChanged
    {
        private StatusConnection statusLaunching = StatusConnection.Unknown;
        private UdpClient client = new UdpClient();
        private string connectionStatusStr = "";
        private string patternIP = @"^\d{3}\.\d{3}\.\d\.\d{3}$";
        private string feedback = "";
        private byte[]? feedbackByte;

        public event PropertyChangedEventHandler? PropertyChanged;
        public StatusConnection StatusLaunching {get => statusLaunching;}
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
            set
            {
                feedback = value;
                OnPropertyChanged();
            }
        }

        private void SendACommand(string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            int numberOfSentBytess = client.Client.Send(data);
        }

        private bool AcceptFeedback()
        {
            client.Client.ReceiveTimeout = 500;
            //feedbackByte = null;
            IPEndPoint? ip = null;

            try
            {
                feedbackByte = client.Receive(ref ip);
            }
            catch (SocketException ac)
            {
                if (ac.SocketErrorCode == SocketError.TimedOut)
                {
                    SwitchStatusConnection(StatusConnection.Unknown);
                    Feedback = "Ответ не получен. \n Проверьте данные в полях \"IP-адрес\", \"Порт\". Убедитесь, что все пропода подключены к устройству.";
                    return false;
                }
            }

            if (feedbackByte != null)
            {
                Feedback = Encoding.UTF8.GetString(feedbackByte);
                return true;
            }
            else
            {
                Feedback = "Ответ получен пустым.";
                return false;
            }

        }

        public void GetState()
        {
            SendACommand(":03;");
        }

        public void Connect(Relay Relay)
        {
            client = new UdpClient();
            client.Client.Connect(Relay.Ip, Relay.Port);

                GetState();
                if (AcceptFeedback())
                {
                    SwitchStatusConnection(StatusConnection.Connect);
                }
                else
                    SwitchStatusConnection(StatusConnection.Unknown);
        }
        public void Disconnect()
        {
            client.Client.Close();
            SwitchStatusConnection(StatusConnection.Disconnect);
        }

        public void GetInputs(Relay Relay)
        {
            SendACommand(":02;");

            if (!AcceptFeedback())
            {
                Disconnect();
                return;
            } 
            else
            {

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
        }

        public void OnOffRelay(int numRele, bool IsChecked)
        {
            SendACommand($":31 0{numRele} 0{(IsChecked ? "1;" : "0;")}");
            AcceptFeedback();
        }

        public void SwitchStatusConnection(StatusConnection statusConnection)
        {
            switch (statusConnection)
            {
                case StatusConnection.Unknown:
                    ConnectionStatusStr = "Unknown";
                    break;
                case StatusConnection.Connect:
                    ConnectionStatusStr = "Connect";
                    Feedback = "Устройство подключено";
                    break;
                case StatusConnection.Disconnect:
                    ConnectionStatusStr = "Disconnect";
                    Feedback = "Устройство отключено";
                    break;
            }
        }

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
