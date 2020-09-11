using System.Windows.Forms;
using GtaKeyboardHook.Infrastructure.Helpers;
using NUnit.Framework;

namespace GtaKeyboardHook.Tests
{
    [TestFixture]
    public class GetScanCodeTests
    {
        [Test]
        public void ScanCode_0x12_for_E()
        {
            var expected = 0x12;
            var actual = KeysHelper.GetScanCode(Keys.E);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ScanCode_0x1F_for_S()
        {
            var expected = 0x1F;
            var actual = KeysHelper.GetScanCode(Keys.S);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ScanCode_0x20_for_D()
        {
            var expected = 0x20;
            var actual = KeysHelper.GetScanCode(Keys.D);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ScanCode_0x44_for_F10()
        {
            var expected = 0x44;
            var actual = KeysHelper.GetScanCode(Keys.F10);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ScanCode_0x48_for_NumPad8()
        {
            var expected = 0x48;
            var actual = KeysHelper.GetScanCode(Keys.NumPad8);

            Assert.AreEqual(expected, actual);
        }
    }
}