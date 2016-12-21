using System.Net.Sockets;
using System;
using System.Net;
namespace test
{
    public class TCPServ
    {
        Socket _socket;

        public Action<Peer> OnAccept;

        public bool Start(int port)
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.Bind(new IPEndPoint(0, port));
            _socket.Listen(100);
            acceptLoop();
            return true;
        }

        private void acceptLoop()
        {
            while (true)
            {
                var client = _socket.Accept();
                var peer = new Peer(client);
                if (OnAccept != null)
                    OnAccept(peer);
                peer.startReceive();
            }
        }

        public class Peer
        {
            private Socket _socket;
            private const int BUF_SIZE = 1024;
            private byte[] _buff = new byte[BUF_SIZE];
            public Action<byte[], int> OnReceive;
            public Action OnClose;

            public Peer(Socket sock)
            {
                _socket = sock;
            }
            internal void startReceive()
            {
                var eventArgs = new SocketAsyncEventArgs();
                eventArgs.SetBuffer(_buff, 0, BUF_SIZE);
                eventArgs.Completed += processReceive;
                continueReceive(eventArgs);
            }
            void processReceive(object sender, SocketAsyncEventArgs args)
            {
                if (args.SocketError != SocketError.Success || args.BytesTransferred <= 0)
                {
                    close(args);
                    return;
                }

                if (OnReceive != null)
                {
                    OnReceive(args.Buffer, args.BytesTransferred);
                }
                continueReceive(args);
            }

            private void continueReceive(SocketAsyncEventArgs args)
            {
                if (!_socket.ReceiveAsync(args))
                {
                    close(args);
                }
            }
            private void close(SocketAsyncEventArgs args)
            {
                args.Completed -= processReceive;
                _socket.Shutdown(SocketShutdown.Both);
                if (OnClose != null)
                    OnClose();
            }
            public void Send(byte[] data)
            {
                _socket.Send(data);
            }
            public override string ToString()
            {
                return _socket.RemoteEndPoint.ToString();
            }

        }

    }
}