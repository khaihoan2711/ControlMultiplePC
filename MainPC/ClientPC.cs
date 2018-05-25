using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MessageDefinition;

namespace MainPC
{
    class ClientPC
    {
        private Socket socket = null;

        public ClientPC(Socket socket)
        {
            this.Socket = socket;
            Task t = new Task(() =>
            {
                ReceiveMessage();
            });
            t.Start();
        }
        Object lookMouseMsg = new Object();

        /// <summary>
        /// Send and wait for notify message from client.
        /// After send message, client respond must be received before send next messages
        /// </summary>
        /// <param name="message"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SendMouseMessage(MouseState mState, int x, int y)
        {
            lock (lookMouseMsg)
            {
                CustomMessage msg = new CustomMessage(MessageType.MouseMessage, mState, x, y);
                byte[] data = MessageConveter.MessageToByteArray(msg);
                this.socket.Send(data);
                WaitForMessageSendComplete();
            }
        }

        private void WaitForMessageSendComplete()
        {
            int bufferSize = 1024;
            byte[] bytes = new byte[bufferSize];
            Socket.Receive(bytes);
            CustomMessage msg = MessageConveter.ByteArrayToMessage(bytes);
            if (msg.MsgType != MessageType.CommonMessage || msg.CommonMessage == CommonMessage.MSG_RECEIVED)
            {
                //Error case
            }
        }

        Object lookKeyboardMsg = new Object();

        public void SendKeyboardMessage(KeyboardState keyboardState, WindowsInput.Native.VirtualKeyCode virtualKeyCode)
        {
            lock (lookKeyboardMsg)
            {
                CustomMessage msg = new CustomMessage(MessageType.KeyboardMessage, keyboardState,virtualKeyCode);
                byte[] data = MessageConveter.MessageToByteArray(msg);
                this.socket.Send(data);
                WaitForMessageSendComplete();
            }
        }

        private void ReceiveMessage()
        {
            int bufferSize = 1024;
            byte[] bytes = new byte[bufferSize];
            Task t = new Task(() =>
            {
                Socket.Receive(bytes);
                int msg = BitConverter.ToInt32(bytes, 0);
                if (msg == (Int32)MessageDefinition.CommonMessage.CONN_CLOSE_REQ)
                {
                    CloseClientConn();
                }
            });
        }

        public EventHandler CloseMessageReceived;

        public Socket Socket { get => socket; set => socket = value; }

        private void CloseClientConn()
        {
            //Raise event
            CloseMessageReceived?.Invoke(this, null);
        }

        internal void CloseSocket()
        {
            this.Socket.Shutdown(SocketShutdown.Both);
            this.Socket.Close();
        }

        internal void SendCommonMessage(CustomMessage msg)
        {
            byte[] data = MessageConveter.MessageToByteArray(msg);
            this.socket.Send(data);
        }
    }
}
