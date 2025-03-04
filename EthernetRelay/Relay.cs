using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EthernetRelay 
{
    public class Relay : INotifyPropertyChanged
    {
   
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        private string deviceName = "CACED0D2C5CAD12032783220762E312E3220623435";
        private string ip = ""; 
        private int port; 

        public string DeviceName
        { get => deviceName; }
        public string Ip
        { 
            get => ip;
            set
            {
                ip = value;
                OnPropertyChanged();
            }
        }

        public int Port
        { 
            get => port;
            set
            {
                port = value;
                OnPropertyChanged();
            }
        }

        private bool[] inputs;

        public bool[] Inputs
        {
            get => inputs;
            set
            {
                inputs = value;
                OnPropertyChanged();
            }
        }
    }
}

