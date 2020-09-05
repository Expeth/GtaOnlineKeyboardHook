using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using GtaKeyboardHook.Model;
using Serilog;
using Vanara.PInvoke;

namespace GtaKeyboardHook.Infrastructure
{
    public class KeyboardHook
    {
        private static readonly ILogger Logger = Log.ForContext<KeyboardHook>();

        private User32.SafeHHOOK _hook;
        private User32.HookProc _hookProc;

        public KeyboardHook()
        {
            EnableHook();
        }

        public Keys HookedKey { get; set; } = Keys.E;
        public event KeyEventHandler KeyDownEvent;
        public event KeyEventHandler KeyUpEvent;

        private IntPtr HookProc(int code, IntPtr wParam, IntPtr lParam)
        {
            var keyboardInput = Marshal.PtrToStructure<User32.KEYBDINPUT>(lParam);
            var pressedKey = (Keys) keyboardInput.wVk;

            if (code >= 0 && HookedKey == pressedKey)
            {
                var keyEvent = (int) wParam;

                Logger.Information("Key {pressedKey} was found", pressedKey);

                var keyEventArgs = new KeyEventArgs(pressedKey);
                // key down
                if (keyEvent == Constants.WM_KEYDOWN || keyEvent == Constants.WM_SYSKEYDOWN)
                    KeyDownEvent?.Invoke(this, keyEventArgs);
                // key up
                else if (keyEvent == Constants.WM_KEYUP || keyEvent == Constants.WM_SYSKEYUP)
                    KeyUpEvent?.Invoke(this, keyEventArgs);

                if (keyEventArgs.Handled) return new IntPtr(1);
            }

            return User32.CallNextHookEx(_hook, code, wParam, lParam);
        }

        private void EnableHook()
        {
            var hInstance = Kernel32.LoadLibrary("User32");
            _hookProc = HookProc;

            _hook = User32.SetWindowsHookEx(User32.HookType.WH_KEYBOARD_LL, _hookProc, hInstance, 0);
        }

        private void DisableHook()
        {
            User32.UnhookWindowsHookEx(_hook);
        }

        ~KeyboardHook()
        {
            DisableHook();
        }
    }
}