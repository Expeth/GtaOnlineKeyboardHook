using System.Drawing;

namespace GtaKeyboardHook.Model
{
    public class Const
    {
        public static readonly Color GtaButtonColor = Color.FromArgb(229, 229, 229);

        public static readonly int WM_KEYDOWN = 0x100;
        public static readonly int WM_KEYUP = 0x101;
        public static readonly int WM_SYSKEYDOWN = 0x104;
        public static readonly int WM_SYSKEYUP = 0x105;
    }
}