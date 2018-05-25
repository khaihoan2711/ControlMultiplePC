using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MessageDefinition;

namespace MainPC
{
    public class AsynchronousSocketListener
    {
        /// <summary>
        /// Start listening for client connect request.
        /// Each connect must be verified before create new CLientPC object to handle this connection.
        /// </summary>
        public void StartListening()
        {
            //Start Listening

            // Data buffer for incoming data.
            int bufferSize = 1024;
            byte[] bytes = new Byte[bufferSize];

            // Establish the local endpoint for the socket.  
            // The DNS name of the computer  
            // running the listener is "host.contoso.com".  
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[5];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

            // Create a TCP/IP socket.  
            Socket listener = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.  
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);

                while (true)
                {
                    // Start an asynchronous socket to listen for connections.  
                    Console.WriteLine("Waiting for a connection...");
                    Socket handler = listener.Accept();
                    handler.Receive(bytes);
                    CustomMessage msg = MessageConveter.ByteArrayToMessage(bytes);

                    if (msg.MsgType == MessageType.CommonMessage)
                    {
                        //When a client send a connect request.
                        //Main form will be informed by this EventHandler
                        NotifyClientConnected(handler);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void NotifyClientConnected(Socket handler)
        {
            SocketListenerEventArgs arg = new SocketListenerEventArgs();
            arg.socket = handler;
            ClientConnected?.Invoke(this, arg);
        }

        public EventHandler<SocketListenerEventArgs> ClientConnected;
    }

    public class SocketListenerEventArgs
    {
        public Socket socket;
    }
}
