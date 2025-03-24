using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace EthernetRelay
{           
    public class RelayManager : INotifyPropertyChanged
    {
        private StatusConnection statusLaunching = StatusConnection.Disconnect;
        private UdpClient client = new UdpClient();
        private Relay relay = new Relay();
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
            IPEndPoint? ip = null;

            try
            {
                feedbackByte = client.Receive(ref ip);
            }
            catch (SocketException ac)
            {
                if (ac.SocketErrorCode == SocketError.TimedOut)
                {
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
                return false;
            }

        }

        private void SetStatus()
        {
            SwitchStatusConnection(StatusConnection.Unknown);
            if (AcceptFeedback())
            {
                SwitchStatusConnection(StatusConnection.Connect);
            }
            else
            {
                SwitchStatusConnection(StatusConnection.Unknown);
            }
        }

        private void GetInfo()
        {
            //Получение текущего состояния входов. Для синхронизации реле на момент подключения.
            SendACommand(":04;");
            SetStatus();
            //Можно выделить if из метода GetInputs в отдельный метод для парсинга ответа от реле.
        }

        public void Connect()
        {
            client = new UdpClient();
            client.Client.Connect(relay.Ip, relay.Port);

            GetInfo();
        }

        public void Disconnect()
        {
            client.Client.Close();
            SwitchStatusConnection(StatusConnection.Disconnect);
        }

        public void GetInputs()
        {
            SendACommand(":02;");
            SetStatus();

            if (feedback != null)
            {
                int start = 3;
                int stop = feedbackByte.Length - 2;
                int lenght = stop - start + 1;
                byte[] selectedBytes = new byte[lenght];
                Array.Copy(feedbackByte, start, selectedBytes, 0, lenght);

                relay.Inputs.Clear();
                int a = 0;

                for (int i = 0; i <= lenght - 1; i++)
                {
                    if (i % 2 == 1)
                    {
                        if (selectedBytes[i] == 49)
                            relay.Inputs.Add(true);
                        else
                            relay.Inputs.Add(false);
                        a++;
                    }
                }
            }
        }

        public void OnOffRelay(int numRele, bool IsChecked)
        {
            SendACommand($":31 0{numRele} 0{(IsChecked ? "1;" : "0;")}");
            SetStatus();
        }

        public void SwitchStatusConnection(StatusConnection statusConnection)
        {
            switch (statusConnection)
            {
                case StatusConnection.Unknown:
                    ConnectionStatusStr = "Unknown";
                    Feedback = "Ответ не получен. \n Проверьте данные в полях \"IP-адрес\" и \"Порт\". Убедитесь, что все пропода подключены к устройству.";
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
