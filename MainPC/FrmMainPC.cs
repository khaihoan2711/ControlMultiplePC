using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MessageDefinition;
using WindowsInput;
using WindowsInput.Native;

namespace MainPC
{
    public partial class FrmMainPC : Form
    {
        public FrmMainPC()
        {
            InitializeComponent();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            //Hook to system to get Mouse and Keyboard message
            MouseHook.Instance.MouseMessageReceived += new EventHandler<MouseMessageReceivedEventArgs>(OnMouseMessageReceived);
            MouseHook.Instance.Hook();
            KeyboardHook.Instance.KeyBoardMessageReceived += new EventHandler<KeyBoardMessageReceivedEventArgs>(OnKeyBoardMessageReceived);
            KeyboardHook.Instance.Hook();

            //Start listenning for incomming connection
            Task t = new Task(() =>
            {
                AsynchronousSocketListener socketListener = new AsynchronousSocketListener();
                socketListener.ClientConnected += new EventHandler<SocketListenerEventArgs>(this.SocketListener_OnNewClientConnected);
                socketListener.StartListening();
            });
            t.Start();
        }

        List<VirtualKeyCode> virtualKeyCodes = new List<VirtualKeyCode>();

        private void OnKeyBoardMessageReceived(object sender, KeyBoardMessageReceivedEventArgs e)
        {
            VirtualKeyCode[] combinedKeys = CombineKeys(e.keyboardState, e.virtualKeyCode);
            SendKeyBoardMessageToClients(e.keyboardState, e.virtualKeyCode);
            SendKeyBoardMessageToClients(combinedKeys);
        }

        private void SendKeyBoardMessageToClients(VirtualKeyCode[] combinedKeys)
        {
            foreach (ClientPC pc in clientPCs)
            {
                Task t = new Task(() =>
                {
                    pc.SendKeyboardMessage(combinedKeys);
                });
                t.Start();
            }
        }

        private VirtualKeyCode[] CombineKeys(KeyboardState keyboardState, VirtualKeyCode vkCode)
        {
            if ((vkCode == VirtualKeyCode.SHIFT || vkCode == VirtualKeyCode.LSHIFT || vkCode == VirtualKeyCode.RSHIFT) ||
                (vkCode == VirtualKeyCode.MENU || vkCode == VirtualKeyCode.LMENU || vkCode == VirtualKeyCode.RMENU) ||
                (vkCode == VirtualKeyCode.CONTROL|| vkCode == VirtualKeyCode.LCONTROL|| vkCode == VirtualKeyCode.RCONTROL))
            {
                if (keyboardState == KeyboardState.WM_KEYDOWN && !virtualKeyCodes.Contains(vkCode))
                {
                    virtualKeyCodes.Add(vkCode);
                }
                else if (keyboardState == KeyboardState.WM_KEYUP)
                {
                    virtualKeyCodes.Remove(vkCode);
                }
            }

            return virtualKeyCodes.ToArray();
        }

        private void SendKeyBoardMessageToClients(KeyboardState keyboardState, WindowsInput.Native.VirtualKeyCode virtualKeyCode)
        {
            foreach (ClientPC pc in clientPCs)
            {
                Task t = new Task(() =>
                {
                    pc.SendKeyboardMessage(keyboardState, virtualKeyCode);
                });
                t.Start();
            }
        }

        private void OnMouseMessageReceived(object sender, MouseMessageReceivedEventArgs e)
        {
            Task t = new Task(() =>
            {
                SendMouseMessageToClients(e.mouseState, e.X, e.Y);
            });
            t.Start();
        }

        private void SendMouseMessageToClients(MessageDefinition.MouseState mState, int x, int y)
        {
            foreach (ClientPC pc in clientPCs)
            {
                Task t = new Task(() =>
                {
                    pc.SendMouseMessage(mState, x, y);
                });
                t.Start();
            }
        }

        List<ClientPC> clientPCs = new List<ClientPC>();
        private void SocketListener_OnNewClientConnected(Object sender, SocketListenerEventArgs e)
        {
            foreach (ClientPC pc in clientPCs)
            {
                if (pc.Socket.RemoteEndPoint.AddressFamily.ToString() == e.socket.RemoteEndPoint.AddressFamily.ToString())
                {
                    //Do nothing if this PC is existing.
                    return;
                }
            }

            //Add new client
            ClientPC clientPC = new ClientPC(e.socket);
            clientPC.CloseMessageReceived += new EventHandler(OnClientCloseConn);
            clientPCs.Add(clientPC);

            CustomMessage msg = new CustomMessage(MessageType.CommonMessage, CommonMessage.CONN_ACCEPT);

            clientPC.SendCommonMessage(msg);
        }

        private void OnClientCloseConn(object sender, EventArgs e)
        {
            ClientPC clientPC = (ClientPC)sender;
            clientPCs.Remove(clientPC);

            clientPC.CloseSocket();
            clientPC = null;
        }
    }
}
