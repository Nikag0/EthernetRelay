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
        private bool isCheckedRele2;
        private bool isCheckedRele1;
        
        public event PropertyChangedEventHandler? PropertyChanged;
        public RelayManager RelayManager 
        {
            get => relayManager;
            private set
            {
                relayManager = value;
                OnPropertyChanged();
            }
        }
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

        //public ICommand  ConnectCommand { get; }

        public ViewModelMainWindow()
        {
            relayManager.SwitchStatusConnection(relayManager.StatusLaunching);
            //ConnectCommand = new DelegateCommands(connect => Connect());
        }

        //public void Connect()
        //{
        //    relayManager.Connect();
        //}

        public ICommand Connect
        {
            get
            {
                return new DelegateCommands((obj) =>
                {
                    relayManager.Connect();
                });
            }
        }

        public ICommand Disconnect
        {
            get
            {
                return new DelegateCommands((obj) =>
                {
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
                   relayManager.GetInputs();
                });
            }
        }
        
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
