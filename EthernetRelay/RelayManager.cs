using System.Net;
using System.Net.Sockets;

namespace EthernetRelay
{
    public class RelayManager
    {
        UdpClient udpc = new UdpClient(1200);
        IPEndPoint localPoint = new IPEndPoint(IPAddress.Parse("192.168.1.203"), 1200);
    }
}
