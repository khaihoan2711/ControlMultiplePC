using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using WindowsInput;
using WindowsInput.Native;

namespace MessageDefinition
{
    [Serializable] 
    public class CustomMessage
    {
        private MessageType msgType;
        private CommonMessage commonMessage;

        private MouseState mouseMessage;
        private int xPoint;
        private int yPoint;

        private KeyboardState keyboardState;
        private WindowsInput.Native.VirtualKeyCode virtualKeyCode;

        public CustomMessage(MessageType msgType, CommonMessage commonMessage)
        {
            this.MsgType = msgType;
            this.CommonMessage = commonMessage;
        }

        public CustomMessage(MessageType msgType, MouseState mState, int x, int y)
        {
            this.MsgType = msgType;
            this.MouseMessage = mState;
            this.XPoint = x;
            this.yPoint = y;
            
        }

        public CustomMessage(MessageType msgType, KeyboardState keyboardMessage, WindowsInput.Native.VirtualKeyCode vkCode)
        {
            this.MsgType = msgType;
            this.KeyboardMessage = keyboardMessage;
            this.VirtualKeyCode = vkCode;
        }

        public MessageType MsgType { get => msgType; set => msgType = value; }
        public CommonMessage CommonMessage { get => commonMessage; set => commonMessage = value; }
        public MouseState MouseMessage { get => mouseMessage; set => mouseMessage = value; }
        public KeyboardState KeyboardMessage { get => keyboardState; set => keyboardState = value; }
        public int XPoint { get => xPoint; set => xPoint = value; }
        public int YPoint { get => yPoint; set => yPoint = value; }
        public VirtualKeyCode VirtualKeyCode { get => virtualKeyCode; set => virtualKeyCode = value; }
    }

    public enum MessageType
    {
        MouseMessage = 0,
        KeyboardMessage = 1,
        CommonMessage = 3

    }

    public enum CommonMessage
    {
        CONN_REQ = 11,
        CONN_CLOSE_REQ = 12,
        CONN_ACCEPT = 13,
        MSG_RECEIVED = 14,
    }

    public enum MouseState
    {
        WM_LBUTTONUP = 21,
        WM_LBUTTONDOWN = 22,
        WM_MOUSEMOVE = 23,
        WM_MOUSEWHEEL = 24,
        WM_RBUTTONUP = 25,
        WM_RBUTTONDOWN = 26
    }

    public enum KeyboardState
    {
        WM_KEYDOWN = 31,
        WM_KEYUP = 32
    }

    public static class MessageConveter
    {
        public static byte[] MessageToByteArray(CustomMessage message)
        {
            if (message == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, message);
            return ms.ToArray();
        }

        public static CustomMessage ByteArrayToMessage(byte[] arrBytes)
        {
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            CustomMessage message = (CustomMessage)binForm.Deserialize(memStream);
            return message;
        }
    }
}
