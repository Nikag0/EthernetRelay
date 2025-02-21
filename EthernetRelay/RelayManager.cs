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

        private Relay relay = new Relay();
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

        public void Connection()
        {
            GiveACommand(Commands.Condition);

            IPEndPoint ip = null;
            byte[] feedbackByte = client.Receive(ref ip);
            int startIndex = 9;
            int endIndex = 50;
            int length = 42;
            byte[] selectedArray = new byte[length];
            Array.Copy(feedbackByte, startIndex, selectedArray, 0, length);
            string FeedbackASCII = Encoding.ASCII.GetString(selectedArray);
            byte[] data1 = Convert.FromHexString(FeedbackASCII);
            string deviceName = System.Text.Encoding.UTF8.GetString(data1);
            //if (relay.DeviceName != deviceName)
            //{
            //Feedback = "Имя устройства не совпадает с именем реле";
            //}
            Feedback = deviceName;
        }

        public void SendACommand(string message)
        {
            client.Connect(relay.Ip, relay.Port);
            byte[] data = Encoding.UTF8.GetBytes(message);
            int numberOfSentBytes = client.Send(data, data.Length);
        }

        public void OnOffRelay1(bool IsChecked)
        {

            if (IsChecked == true)
            {
                GiveACommand(Commands.OnRelay1);
            }
            else
            {
                GiveACommand(Commands.OffRelay1);
            }
        }

        public void OnOffRelay2(bool IsChecked)
        {
            if (IsChecked == true)
            {
                GiveACommand(Commands.OnRelay2);
            }
            else
            {
                GiveACommand(Commands.OffRelay2);
            }   
        }
        enum Commands
        {
            OnRelay1,
            OffRelay1,
            OnRelay2,
            OffRelay2,
            Condition
        }
        private void GiveACommand(Commands commands)
        {
            switch (commands)
            {
                case Commands.OnRelay1:
                    SendACommand(":31 01 01;");
                    break;
                case Commands.OffRelay1:
                    SendACommand(":31 01 00;");
                    break;
                case Commands.OnRelay2:
                    SendACommand(":31 02 01;");
                    break;
                case Commands.OffRelay2:
                    SendACommand(":31 02 00;");
                    break;
                case Commands.Condition:
                    SendACommand(":03;");
                    break;
            }
        }
    }
}
