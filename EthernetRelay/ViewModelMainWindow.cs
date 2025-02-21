using System.ComponentModel;
using System.Runtime.CompilerServices;
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
            relayManager.Connection();
        }

        public ICommand CheckBoxRelay1
        {
            get
            {
                return new DelegateCommands((obj) =>
                {
                    relayManager.OnOffRelay1(IsCheckedRele1);
                });
            }
        }

        public ICommand CheckBoxRelay2
        {
            get
            {
                return new DelegateCommands((obj) =>
                {
                    relayManager.OnOffRelay2(IsCheckedRele2);
                });
            }
        }
    }
}
