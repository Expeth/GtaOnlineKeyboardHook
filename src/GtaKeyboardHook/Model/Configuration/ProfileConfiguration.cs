using Newtonsoft.Json;

namespace GtaKeyboardHook.Model.Configuration
{
    public class ProfileConfiguration
    {
        public int CallbackDuration { get; set; }
        public string HookedKeyCode { get; set; }
        public int HookedCoordinateX { get; set; }
        public int HookedCoordinateY { get; set; }
        public string HookedRgbColorCode { get; set; }

        public override bool Equals(object obj)
        {
            return CallbackDuration == (obj as ProfileConfiguration).CallbackDuration &&
                   HookedKeyCode == (obj as ProfileConfiguration).HookedKeyCode &&
                   HookedCoordinateX == (obj as ProfileConfiguration).HookedCoordinateX &&
                   HookedCoordinateY == (obj as ProfileConfiguration).HookedCoordinateY &&
                   HookedRgbColorCode == (obj as ProfileConfiguration).HookedRgbColorCode;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}