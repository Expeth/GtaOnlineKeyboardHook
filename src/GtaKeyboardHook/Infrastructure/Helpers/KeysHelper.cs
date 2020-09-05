using System.Windows.Forms;
using Vanara.PInvoke;

namespace GtaKeyboardHook.Infrastructure.Helpers
{
    public static class KeysHelper
    {
        public static uint GetScanCode(Keys key)
        {
            var keyCode = (uint) key;
            var scanCode = User32.MapVirtualKey(keyCode, User32.MAPVK.MAPVK_VK_TO_VSC);

            return (ushort) scanCode;
        }
    }
}