using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientPC
{
    public static class VirtualMouse
    {
        [DllImport("user32.dll")]
        static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);
        private const int MOUSEEVENT_MOVE = 0x0001;
        private const int MOUSEEVENT_LEFTDOWN = 0x0002;
        private const int MOUSEEVENT_LEFTUP = 0x0004;
        private const int MOUSEEVENT_RIGHTDOWN = 0x0008;
        private const int MOUSEEVENT_RIGHTUP = 0x0010;
        private const int MOUSEEVENT_MIDDLEDOWN = 0x0020;
        private const int MOUSEEVENT_MIDDLEUP = 0x0040;
        private const int MOUSEEVENT_ABSOLUTE = 0x8000;
        private const int MOUSEEVENT_XDOWN = 0x0080;
        private const int MOUSEEVENT_XUP = 0x0100;
        private const int MOUSEEVENT_WHEEL = 0x0800;
        private const int MOUSEEVENT_HWHEEL = 0x01000;

        //public static void Move(int xDelta, int yDelta)
        //{
        //   mouse_event(MOUSEEVENT_MOVE, xDelta, yDelta, 0, 0);
        //}
        public static void Move(int xDelta, int yDelta)
        {
            xDelta = UpdateAbsoluteCoordinatesOfX(xDelta);
            yDelta = UpdateAbsoluteCoordinatesOfY(yDelta);
            mouse_event(MOUSEEVENT_ABSOLUTE | MOUSEEVENT_MOVE, xDelta, yDelta, 0, 0);
        }
        public static void LeftClick()
        {
            mouse_event(MOUSEEVENT_LEFTDOWN, Control.MousePosition.X, Control.MousePosition.Y, 0, 0);
            mouse_event(MOUSEEVENT_LEFTUP, Control.MousePosition.X, Control.MousePosition.Y, 0, 0);
        }

        public static void LeftDown()
        {
            mouse_event(MOUSEEVENT_LEFTDOWN, Control.MousePosition.X, Control.MousePosition.Y, 0, 0);
        }

        public static void LeftUp()
        {
            mouse_event(MOUSEEVENT_LEFTUP, Control.MousePosition.X, Control.MousePosition.Y, 0, 0);
        }

        public static void RightClick()
        {
            mouse_event(MOUSEEVENT_RIGHTDOWN, Control.MousePosition.X, Control.MousePosition.Y, 0, 0);
            mouse_event(MOUSEEVENT_RIGHTUP, Control.MousePosition.X, Control.MousePosition.Y, 0, 0);
        }

        public static void RightDown()
        {
            mouse_event(MOUSEEVENT_RIGHTDOWN, Control.MousePosition.X, Control.MousePosition.Y, 0, 0);
        }

        public static void RightUp()
        {
            mouse_event(MOUSEEVENT_RIGHTUP, Control.MousePosition.X, Control.MousePosition.Y, 0, 0);
        }

        public static void MiddleClick()
        {
            mouse_event(MOUSEEVENT_MIDDLEDOWN, Control.MousePosition.X, Control.MousePosition.Y, 0, 0);
            mouse_event(MOUSEEVENT_MIDDLEUP, Control.MousePosition.X, Control.MousePosition.Y, 0, 0);
        }

        public static void MiddleDown()
        {
            mouse_event(MOUSEEVENT_MIDDLEDOWN, Control.MousePosition.X, Control.MousePosition.Y, 0, 0);
        }

        public static void MiddleUp()
        {
            mouse_event(MOUSEEVENT_MIDDLEUP, Control.MousePosition.X, Control.MousePosition.Y, 0, 0);
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
    }
}
