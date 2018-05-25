using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MessageDefinition;

namespace ClientPC
{
    class AsynchronousSocketClient
    {
        public void StartConnect(IPAddress ipAddress, int port)
        {
            Task t = new Task(() =>
            {
                try
                {
                    IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, port);
                    Socket client = new Socket(ipAddress.AddressFamily,
                        SocketType.Stream, ProtocolType.Tcp);
                    client.Connect(ipEndPoint);

                    int bufferSize = 1024;
                    byte[] buffer = new byte[bufferSize];
                    int byteRead = 0;

                    CustomMessage reqConnMessage = new CustomMessage(MessageDefinition.MessageType.CommonMessage, MessageDefinition.CommonMessage.CONN_REQ);
                    buffer = MessageConveter.MessageToByteArray(reqConnMessage);

                    client.Send(buffer);
                    buffer.Initialize();

                    byteRead = client.Receive(buffer);
                    if (byteRead > 0)
                    {
                        CustomMessage msg = MessageConveter.ByteArrayToMessage(buffer);

                        if (msg.MsgType == MessageType.CommonMessage
                        && msg.CommonMessage == CommonMessage.CONN_ACCEPT)
                        {
                            ConnAcceptedEvent(client);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    throw;
                }
            });
            t.Start();
        }

        private void ConnAcceptedEvent(Socket client)
        {
            SocketClientEventArgs args = new SocketClientEventArgs();
            args.socket = client;
            ServerConnected?.Invoke(this, args);

            ListeningForServerMessage(client);
        }

        private void ListeningForServerMessage(Socket client)
        {
            int bufferSize = 1024;
            byte[] buffer = new byte[bufferSize];
            int byteRead = 0;

            try
            {
                while (true)
                {
                    buffer.Initialize();

                    byteRead = client.Receive(buffer);
                    if (byteRead > 0)
                    {
                        CustomMessage msg = MessageConveter.ByteArrayToMessage(buffer);
                        //send mouse event to system
                        SendMessageToSystem(msg);

                        Console.WriteLine("MOUSE: " + msg.XPoint + '|' + msg.YPoint);

                        NotifyReceivedDone(client);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void SendMessageToSystem(CustomMessage msg)
        {
            MessageEventArgs args = new MessageEventArgs();
            args.msg = msg;
            MouseOrKeyboardReceived.Invoke(this, args);
        }

        private void NotifyReceivedDone(Socket socket)
        {
            CustomMessage message = new CustomMessage(MessageType.CommonMessage, CommonMessage.MSG_RECEIVED);
            byte[] bytes = MessageConveter.MessageToByteArray(message);
            socket.Send(bytes);
        }

        public EventHandler<SocketClientEventArgs> ServerConnected;
        public EventHandler<MessageEventArgs> MouseOrKeyboardReceived;

        public class SocketClientEventArgs
        {
            public Socket socket;
        }
    }

    public class MessageEventArgs
    {
        public CustomMessage msg;
    }
}
