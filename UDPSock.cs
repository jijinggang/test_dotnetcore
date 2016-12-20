using System.Net.Sockets;
using System;
using System.Net;
namespace test
{
    public class UDPSock
    {
        public Action<EndPoint, byte[], int> OnReceive;

        public UDPSock(int port = 0)
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            if (port != 0)
                _socket.Bind(new IPEndPoint(0, port));
            startReceive();
        }


        public bool Send(byte[] data, int len, EndPoint ep)
        {
            _socket.SendTo(data, len, SocketFlags.None, ep);
            return true;
        }

        //---------------------------------------

        Socket _socket;
        byte[] _buff = new byte[1024];

        void startReceive()
        {
            var eventArgs = new SocketAsyncEventArgs();
            eventArgs.SetBuffer(_buff, 0, _buff.Length);
            eventArgs.Completed += processReceive;
            continueReceive(eventArgs);
        }
        void processReceive(object sender, SocketAsyncEventArgs args)
        {
            if (args.BytesTransferred <= 0)
            {
                end(args);
                return;
            }

            if (OnReceive != null)
            {
                OnReceive(args.RemoteEndPoint, args.Buffer, args.BytesTransferred);
            }
            continueReceive(args);
        }
        void continueReceive(SocketAsyncEventArgs args)
        {
            if (!_socket.ReceiveAsync(args))
            {
                end(args);
            }
        }
        void end(SocketAsyncEventArgs args)
        {
            args.Completed -= processReceive;
        }

    }
}