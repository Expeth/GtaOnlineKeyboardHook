using System;
using GtaKeyboardHook.Model;
using GtaKeyboardHook.Tests.TestData;
using NUnit.Framework;

namespace GtaKeyboardHook.Tests
{
    [TestFixture]
    public class JsonConfigurationManagerTests
    {
        private string _validFilePath;
        
        public JsonConfigurationManagerTests()
        {  
            _validFilePath = $"{AppDomain.CurrentDomain.BaseDirectory}\\TestData\\validconfig.json";
        }
        
        [Test]
        public void ConfigLoadSuccess()
        {
            var manager = new JsonConfigurationManager(_validFilePath);
            manager.LoadFromSource();

            var actual = manager.GetConfig();
            var expected = TestDataHolder.ProfileConfigExample1;

            Assert.AreEqual(expected, actual);
        }
        
        [Test]
        public void ConfigSaveSuccess()
        {
            var manager = new JsonConfigurationManager(_validFilePath);
            manager.LoadFromSource();

            var actual = manager.GetConfig();
            var expected = TestDataHolder.ProfileConfigExample2;

            actual.CallbackDuration = expected.CallbackDuration;
            actual.HookedCoordinateX = expected.HookedCoordinateX;
            actual.HookedCoordinateY = expected.HookedCoordinateY;
            actual.HookedKeyCode = expected.HookedKeyCode;
            actual.HookedRgbColorCode = expected.HookedRgbColorCode;

            manager.Save();
            manager.LoadFromSource();
            actual = manager.GetConfig();
            
            Assert.AreEqual(expected, actual);
            
            actual.CallbackDuration = TestDataHolder.ProfileConfigExample1.CallbackDuration;
            actual.HookedCoordinateX = TestDataHolder.ProfileConfigExample1.HookedCoordinateX;
            actual.HookedCoordinateY = TestDataHolder.ProfileConfigExample1.HookedCoordinateY;
            actual.HookedKeyCode = TestDataHolder.ProfileConfigExample1.HookedKeyCode;
            actual.HookedRgbColorCode = TestDataHolder.ProfileConfigExample1.HookedRgbColorCode;

            manager.Save();
        }
    }
}