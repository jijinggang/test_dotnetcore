using System.Net.Sockets;
using System;
namespace common
{
    public class SockServer{
        Socket _socket;

        public Action<Socket> OnAccept;

        public bool Start(int port){
            //_socket = new Socket()
            return false;
        }

        
    }
}