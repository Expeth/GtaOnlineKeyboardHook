using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Serilog;
using GtaKeyboardHook.Model;
using Vanara.PInvoke;

namespace GtaKeyboardHook.Infrastructure
{
    public class KeyboardHook
    {
        private static readonly ILogger Logger = Log.ForContext<KeyboardHook>();
        
        public Keys HookedKey { get; set; } = Keys.E;

        public event KeyEventHandler KeyDownEvent;
        public event KeyEventHandler KeyUpEvent;
		
        private User32.SafeHHOOK _hook;
        private User32.HookProc _hookProc;

        public KeyboardHook()
        {
            EnableHook();
        }

        private IntPtr HookProc(int code, IntPtr wParam, IntPtr lParam)
        {
            var keyboardInput = Marshal.PtrToStructure<User32.KEYBDINPUT>(lParam);
            var pressedKey = (Keys) keyboardInput.wVk;
         
            if (code >= 0 && HookedKey == pressedKey)
            {
                var keyEvent = (Int32) wParam;
                
                Logger.Information("Key {pressedKey} was found", pressedKey);

                var keyEventArgs = new KeyEventArgs(pressedKey);
                // key down
                if (keyEvent == Const.WM_KEYDOWN || keyEvent == Const.WM_SYSKEYDOWN)
                    KeyDownEvent?.Invoke(this, keyEventArgs);
                // key up
                else if (keyEvent == Const.WM_KEYUP || keyEvent == Const.WM_SYSKEYUP)
                    KeyUpEvent?.Invoke(this, keyEventArgs);

                if (keyEventArgs.Handled)
                    return new IntPtr(1);
            }

            return User32.CallNextHookEx(_hook, code, wParam, lParam);
        }

        private void EnableHook()
        {
            var hInstance = Kernel32.LoadLibrary("User32");
            _hookProc = new User32.HookProc(HookProc);

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