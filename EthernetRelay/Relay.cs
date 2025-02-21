using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EthernetRelay
{
    public class Relay
    {
        private string deviceName = "КОРТЕКС 2x2 v.1.2 b45";
        private string ip = "192.168.1.203";
        private int port = 1200;

        public string DeviceName
        { get => deviceName;}
        public string Ip
        { get => ip;}

        public int Port
        { get => port;}
    }
}
