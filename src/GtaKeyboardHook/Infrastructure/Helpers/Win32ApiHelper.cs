using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Vanara.PInvoke;

namespace GtaKeyboardHook.Infrastructure.Helpers
{
    public static class Win32ApiHelper
    {
        public static Color GetPixelColor(int x, int y)
        {
            // get HWND of the Desktop
            var window = User32.GetDesktopWindow();

            // get HDC from HWND
            var hdc = User32.GetDC(window);
            // UINT representation of a pixel information
            uint pixel = Gdi32.GetPixel(hdc, x, y);

            // release HDC
            User32.ReleaseDC(window, hdc);

            // get color
            var color = ColorHelper.FromPixel(pixel);

            return color;
        }

        public static void SendKeyPressedEvent(Keys code, bool isKeyUp)
        {
            var input = new User32.INPUT();

            //TODO: consider moving this code into a factory
            input.type = User32.INPUTTYPE.INPUT_KEYBOARD;
            input.ki = new User32.KEYBDINPUT();
            input.ki.time = 0;
            input.ki.dwFlags = isKeyUp
                ? User32.KEYEVENTF.KEYEVENTF_KEYUP | User32.KEYEVENTF.KEYEVENTF_SCANCODE
                : 0 | User32.KEYEVENTF.KEYEVENTF_SCANCODE;
            input.ki.dwExtraInfo = UIntPtr.Zero;

            //TODO: find a way to get a scancode of any key 
            input.ki.wScan = 0x1F;
            input.ki.wVk = 0;

            User32.SendInput(1, new[] {input}, Marshal.SizeOf(typeof(User32.INPUT)));
        }

        public static void SendKeyPressedEvent(Keys code)
        {
            SendKeyPressedEvent(code, false);
            SendKeyPressedEvent(code, true);
        }
    }
}