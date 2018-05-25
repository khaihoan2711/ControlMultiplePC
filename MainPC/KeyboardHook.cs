using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsInput;

namespace MainPC
{
    class KeyboardHook
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private LowLevelKeyboardProc _proc;
        private IntPtr _hookID = IntPtr.Zero;

        //public static void Main()
        //{
        //    _hookID = SetHook(_proc);
        //    Application.Run();
        //    UnhookWindowsHookEx(_hookID);
        //}

        //Apply Singleton Design STT
        private static KeyboardHook instance;
        public static KeyboardHook Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new KeyboardHook();
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
                _hookID = SetWindowsHookEx(WH_KEYBOARD_LL, _proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        public void UnHook()
        {
            UnhookWindowsHookEx(_hookID);
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                   
                if (wParam == (IntPtr)WM_KEYDOWN)
                {
                    int vkCode = Marshal.ReadInt32(lParam);
                    
                    NotifyMessage(MessageDefinition.KeyboardState.WM_KEYDOWN, (WindowsInput.Native.VirtualKeyCode)vkCode);
                }
                if (wParam == (IntPtr)WM_KEYUP)
                {
                    int vkCode = Marshal.ReadInt32(lParam);
                    NotifyMessage(MessageDefinition.KeyboardState.WM_KEYUP, (WindowsInput.Native.VirtualKeyCode)vkCode);
                }
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);


        private void NotifyMessage(MessageDefinition.KeyboardState keyboardState, WindowsInput.Native.VirtualKeyCode vKCode)
        {
            KeyBoardMessageReceivedEventArgs args = new KeyBoardMessageReceivedEventArgs { keyboardState = keyboardState, virtualKeyCode = vKCode };
          
            KeyBoardMessageReceived?.Invoke(this, args);
        }
        
        public event EventHandler<KeyBoardMessageReceivedEventArgs> KeyBoardMessageReceived;
        //END: Create a custome Event that fired when a message is received from Key hook
    }


    //STT: Create a custome Event Args
    public class KeyBoardMessageReceivedEventArgs : EventArgs
    {
        public MessageDefinition.KeyboardState keyboardState { get; set; }
        public WindowsInput.Native.VirtualKeyCode virtualKeyCode { get; set; }
    }
    //END: Create a custome Event Args
}
