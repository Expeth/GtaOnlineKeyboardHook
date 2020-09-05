using GtaKeyboardHook.Model.Configuration;

namespace GtaKeyboardHook.Tests.TestData
{
    public static class TestDataHolder
    {
        public static ProfileConfiguration ProfileConfigExample1 = new ProfileConfiguration
        {
            CallbackDuration = 4500,
            HookedCoordinateX = 150,
            HookedCoordinateY = 320,
            HookedKeyCode = "E",
            HookedRgbColorCode = "130,130,130"
        };

        public static ProfileConfiguration ProfileConfigExample2 = new ProfileConfiguration
        {
            CallbackDuration = 1200,
            HookedCoordinateX = 320,
            HookedCoordinateY = 580,
            HookedKeyCode = "F10",
            HookedRgbColorCode = "220,220,220"
        };
    }
}