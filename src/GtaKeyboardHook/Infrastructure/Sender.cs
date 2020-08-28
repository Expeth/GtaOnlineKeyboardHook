using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Vanara.PInvoke;

namespace GtaKeyboardHook.Infrastructure
{
    public class Sender
    {
        public static void SendKeyPressedEvent(Keys code, bool isKeyUp)
        {
            User32.INPUT input = new User32.INPUT();

            input.type = User32.INPUTTYPE.INPUT_KEYBOARD;
            input.ki = new User32.KEYBDINPUT();
            input.ki.time = 0;
            input.ki.dwFlags = isKeyUp ? User32.KEYEVENTF.KEYEVENTF_KEYUP | User32.KEYEVENTF.KEYEVENTF_SCANCODE : 0 | User32.KEYEVENTF.KEYEVENTF_SCANCODE;
            input.ki.dwExtraInfo = UIntPtr.Zero;
            input.ki.wScan = 0x1F;
            input.ki.wVk = 0;

            User32.SendInput(1, new []{input}, Marshal.SizeOf(typeof(User32.INPUT)));
        }
    }
}