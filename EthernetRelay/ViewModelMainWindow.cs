using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace EthernetRelay
{
    class ViewModelMainWindow : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        private bool isCheckedRele1;

        public bool IsCheckedRele1
        {
            get => isCheckedRele1;
            set
            {
                isCheckedRele1 = value;
                OnPropertyChanged();
            }
        }

        private bool isCheckedRele2;

        public bool IsCheckedRele2
        {
            get => isCheckedRele2;
            set
            {
                isCheckedRele2 = value;
                OnPropertyChanged();
            }
        }

        private Relay relay = new Relay();
        public Relay Relay
        {
            get => relay;
            set
            {
                relay = value;
            }
        }

        private RelayManager relayManager = new RelayManager();
        public RelayManager RelayManager
        {
            get => relayManager;
            set
            {
                relayManager = value;
            }
        }

        public ViewModelMainWindow()
        {

            RelayManager.ConnectionStatus(RelayManager.StatusLaunching);
        }

        public ICommand Connect
        {
            get
            {
                return new DelegateCommands((obj) =>
                {
                    if (Regex.IsMatch(Relay.Ip, RelayManager.PatternIP) && Relay.Port > 0)
                        RelayManager.Connect(Relay);
                });
            }
        }

        public ICommand Disconnect
        {
            get
            {
                return new DelegateCommands((obj) =>
                {
                    if (Regex.IsMatch(Relay.Ip, RelayManager.PatternIP) && Relay.Port > 0)
                        RelayManager.Disconnect();
                });
            }
        }

        public ICommand CheckBoxRelay1
        {
            get
            {
                return new DelegateCommands((obj) =>
                {
                    RelayManager.OnOffRelay(1, IsCheckedRele1);
                });
            }
        }

        public ICommand CheckBoxRelay2
        {
            get
            {
                return new DelegateCommands((obj) =>
                {
                    RelayManager.OnOffRelay(2, IsCheckedRele2);
                });
            }
        }

        public ICommand GetInputs
        {
            get
            {
                return new DelegateCommands((obj) =>
                {
                   RelayManager.GetInputs(Relay);
                });
            }
        }
    }
}
