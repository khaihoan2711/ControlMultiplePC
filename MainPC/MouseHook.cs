using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MainPC
{
    class MouseHook
    {
        private LowLevelMouseProc _proc;
        private IntPtr _hookID = IntPtr.Zero;

        //public static void Main()
        //{
        //    _hookID = SetHook(_proc);
        //    Application.Run();
        //    UnhookWindowsHookEx(_hookID);
        //}

        //Apply Singleton Design STT
        static object key = new object();
        private static MouseHook instance;
        public static MouseHook Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (key) { instance = new MouseHook(); }
                }
                return instance;
            }
        }
        //Apply Singleton Design END

        public void Hook()
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                _proc = HookCallback;
                _hookID = SetWindowsHookEx(WH_MOUSE_LL, _proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        public void UnHook()
        {
            UnhookWindowsHookEx(_hookID);
        }

        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            //if (nCode >= 0 && MouseMessages.WM_LBUTTONDOWN == (MouseMessages)wParam)
            //{
            //    MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
            //}

            if (nCode >= 0)
            {
                if ((MouseMessages)wParam == MouseMessages.WM_LBUTTONUP)
                {
                    MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                    //Console.WriteLine("LMouse Up", hookStruct.pt.x , hookStruct.pt.y);
                    NotifyMessage(MessageDefinition.MouseState.WM_LBUTTONUP, hookStruct.pt.x, hookStruct.pt.y);
                }
                if ((MouseMessages)wParam == MouseMessages.WM_LBUTTONDOWN)
                {
                    MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                    //Console.WriteLine("LMouse Down", hookStruct.pt.x , hookStruct.pt.y);
                    NotifyMessage(MessageDefinition.MouseState.WM_LBUTTONDOWN, hookStruct.pt.x, hookStruct.pt.y);
                }
                if ((MouseMessages)wParam == MouseMessages.WM_MOUSEMOVE)
                {
                    MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                    //Console.WriteLine("Mouse Move", hookStruct.pt.x , hookStruct.pt.y);
                    int x, y;
                    x = UpdateAbsoluteCoordinatesOfX(hookStruct.pt.x);
                    y = UpdateAbsoluteCoordinatesOfY(hookStruct.pt.y);
                    NotifyMessage(MessageDefinition.MouseState.WM_MOUSEMOVE, x, y);
                }
                if ((MouseMessages)wParam == MouseMessages.WM_MOUSEWHEEL)
                {
                    MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                    //Console.WriteLine("Mouse Wheel", hookStruct.pt.x , hookStruct.pt.y);
                    NotifyMessage(MessageDefinition.MouseState.WM_MOUSEWHEEL, hookStruct.pt.x, hookStruct.pt.y);
                }
                if ((MouseMessages)wParam == MouseMessages.WM_RBUTTONDOWN)
                {
                    MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                    //Console.WriteLine("RMouse Down", hookStruct.pt.x , hookStruct.pt.y);
                    NotifyMessage(MessageDefinition.MouseState.WM_RBUTTONDOWN, hookStruct.pt.x, hookStruct.pt.y);
                }
                if ((MouseMessages)wParam == MouseMessages.WM_RBUTTONUP)
                {
                    MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                    //Console.WriteLine("RMouse Up", hookStruct.pt.x , hookStruct.pt.y);
                    NotifyMessage(MessageDefinition.MouseState.WM_RBUTTONUP, hookStruct.pt.x, hookStruct.pt.y);
                }
            }

            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        private static int UpdateAbsoluteCoordinatesOfX(int inputXinPixels)
        {
            var screenBounds = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
            var x = inputXinPixels * 65535 / screenBounds.Width;
            return x;
        }

        private static int UpdateAbsoluteCoordinatesOfY(int inputYinPixels)
        {
            var screenBounds = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
            var y = inputYinPixels * 65535 / screenBounds.Height;
            return y;
        }

        private const int WH_MOUSE_LL = 14;

        private enum MouseMessages
        {
            WM_LBUTTONDOWN = 0x0201,
            WM_LBUTTONUP = 0x0202,
            WM_MOUSEMOVE = 0x0200,
            WM_MOUSEWHEEL = 0x020A,
            WM_RBUTTONDOWN = 0x0204,
            WM_RBUTTONUP = 0x0205
        }


        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);


        private void NotifyMessage(MessageDefinition.MouseState mState, int xPoint, int yPoint)
        {
            MouseMessageReceivedEventArgs args = new MouseMessageReceivedEventArgs { mouseState = mState, X = xPoint, Y = yPoint };
            OnMouseMessageReceived(args);
        }

        //STT: Create a custome Event that fired when a message is received from Key hook
        protected virtual void OnMouseMessageReceived(MouseMessageReceivedEventArgs e)
        {
            EventHandler<MouseMessageReceivedEventArgs> handler = MouseMessageReceived;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler<MouseMessageReceivedEventArgs> MouseMessageReceived;
        //END: Create a custome Event that fired when a message is received from Key hook
    }


    //STT: Create a custome Event Args
    public class MouseMessageReceivedEventArgs : EventArgs
    {
        public MessageDefinition.MouseState mouseState { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }
    //END: Create a custome Event Args
}
