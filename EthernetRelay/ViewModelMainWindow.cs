using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace EthernetRelay
{
    class ViewModelMainWindow : INotifyPropertyChanged
    {
        private RelayManager relayManager = new RelayManager();
        private Relay relay = new Relay();
        private bool isCheckedRele2;
        private bool isCheckedRele1;

        public event PropertyChangedEventHandler? PropertyChanged;
        public RelayManager RelayManager 
        {
            get => relayManager;
            set
            {
                relayManager = value;
                OnPropertyChanged();
            }
        }
        public Relay Relay {get => relay;}
        public bool IsCheckedRele1
        {
            get => isCheckedRele1;
            set
            {
                isCheckedRele1 = value;
                OnPropertyChanged();
            }
        }
        public bool IsCheckedRele2
        {
            get => isCheckedRele2;
            set
            {
                isCheckedRele2 = value;
                OnPropertyChanged();
            }
        }

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public ViewModelMainWindow()
        {
            relayManager.ConnectionStatus(relayManager.StatusLaunching);
        }

        public ICommand Connect
        {
            get
            {
                return new DelegateCommands((obj) =>
                {
                    if (Regex.IsMatch(relay.Ip, relayManager.PatternIP) && relay.Port > 0)
                        relayManager.Connect(relay);
                });
            }
        }

        public ICommand Disconnect
        {
            get
            {
                return new DelegateCommands((obj) =>
                {
                    if (Regex.IsMatch(relay.Ip, relayManager.PatternIP) && relay.Port > 0)
                        relayManager.Disconnect();
                });
            }
        }

        public ICommand CheckBoxRelay1
        {
            get
            {
                return new DelegateCommands((obj) =>
                {
                    relayManager.OnOffRelay(1, IsCheckedRele1);
                });
            }
        }

        public ICommand CheckBoxRelay2
        {
            get
            {
                return new DelegateCommands((obj) =>
                {
                    relayManager.OnOffRelay(2, IsCheckedRele2);
                });
            }
        }

        public ICommand GetInputs
        {
            get
            {
                return new DelegateCommands((obj) =>
                {
                   relayManager.GetInputs(relay);
                });
            }
        }
    }
}
