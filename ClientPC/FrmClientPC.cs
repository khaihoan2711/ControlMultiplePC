using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsInput;

namespace ClientPC
{
    public partial class FrmClientPC : Form
    {
        AsynchronousSocketClient socketClient;
        InputSimulator inputSimulator;

        public FrmClientPC()
        {
            InitializeComponent();
            socketClient = new AsynchronousSocketClient();
            inputSimulator = new InputSimulator();
        }

        private void OnServerConnected(object sender, AsynchronousSocketClient.SocketClientEventArgs e)
        {
            MessageBox.Show("Establish connect to Server OK");
        }

        private void BtnConnect_Click(object sender, EventArgs e)
        {
            IPAddress ip = IPAddress.Parse(textBox1.Text.Trim());

            socketClient.ServerConnected += new EventHandler<AsynchronousSocketClient.SocketClientEventArgs>(OnServerConnected);
            socketClient.MouseOrKeyboardReceived += new EventHandler<MessageEventArgs>(OnMouseOrKeyboardReceived);
            socketClient.StartConnect(ip, 11000);
        }

        private void OnMouseOrKeyboardReceived(object sender, MessageEventArgs e)
        {
            if (e.msg.MsgType == MessageDefinition.MessageType.CommonMessage)
            {
                //...
                Console.WriteLine(e.msg.CommonMessage.ToString());
            }
            else if (e.msg.MsgType == MessageDefinition.MessageType.KeyboardMessage)
            {
                switch (e.msg.KeyboardMessage)
                {
                    case MessageDefinition.KeyboardState.WM_KEYDOWN:
                        inputSimulator.Keyboard.KeyDown(e.msg.VirtualKeyCode);
                        break;
                    case MessageDefinition.KeyboardState.WM_KEYUP:
                        inputSimulator.Keyboard.KeyUp(e.msg.VirtualKeyCode);
                        break;
                    default:
                        break;
                }
                Console.WriteLine(e.msg.KeyboardMessage.ToString());
            }
            else if (e.msg.MsgType == MessageDefinition.MessageType.MouseMessage)
            {
                switch (e.msg.MouseMessage)
                {
                    case MessageDefinition.MouseState.WM_LBUTTONUP:
                        inputSimulator.Mouse.LeftButtonUp();
                        break;
                    case MessageDefinition.MouseState.WM_LBUTTONDOWN:
                        inputSimulator.Mouse.LeftButtonDown();
                        break;
                    case MessageDefinition.MouseState.WM_MOUSEMOVE:
                        inputSimulator.Mouse.MoveMouseTo(e.msg.XPoint, e.msg.YPoint);
                        break;
                    case MessageDefinition.MouseState.WM_MOUSEWHEEL:
                        //Not finished yet
                        break;
                    case MessageDefinition.MouseState.WM_RBUTTONUP:
                        inputSimulator.Mouse.RightButtonUp();
                        break;
                    case MessageDefinition.MouseState.WM_RBUTTONDOWN:
                        inputSimulator.Mouse.RightButtonDown();
                        break;
                    default:
                        break;
                }
                Console.WriteLine(e.msg.MouseMessage.ToString());
            }
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {

        }
    }
}
