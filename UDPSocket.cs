using System.Net.Sockets;
using System;
using System.Net;
namespace test
{
    public sealed class UDPSocket
    {
        public Action<EndPoint, byte[], int> OnReceive;

        public UDPSocket(int port = 0)
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            //不绑定时只能发不能收
            _socket.Bind(new IPEndPoint(IPAddress.Any, port));
            startReceive();
        }


        public bool Send(byte[] data, int len, EndPoint endPoint)
        {
            _socket.SendTo(data, len, SocketFlags.None, endPoint);
            return true;
        }

        //---------------------------------------

        Socket _socket;
        byte[] _buffer = new byte[1024];
        SocketAsyncEventArgs _args = new SocketAsyncEventArgs();
        void startReceive()
        {
            _args.SetBuffer(_buffer, 0, _buffer.Length);
            _args.RemoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
            _args.Completed += processReceive;
            continueReceive(_args);
        }
        void processReceive(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError != SocketError.Success)
            {
                end();
                return;
            }

            if (OnReceive != null)
            {
                OnReceive(args.RemoteEndPoint, args.Buffer, args.BytesTransferred);
            }
            continueReceive(_args);
        }
        void continueReceive(SocketAsyncEventArgs args)
        {
            if (!_socket.ReceiveFromAsync(args))
            {
                end();
            }
        }
        void end()
        {
            _args.Completed -= processReceive;
        }

    }
}