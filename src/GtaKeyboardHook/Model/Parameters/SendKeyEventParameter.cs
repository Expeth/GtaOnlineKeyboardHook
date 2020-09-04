using System.Windows.Forms;

namespace GtaKeyboardHook.Model.Parameters
{
    public class SendKeyEventParameter
    {
        public int DelayDuration { get; set; }
        public Keys HookedKey { get; set; }
    }
}