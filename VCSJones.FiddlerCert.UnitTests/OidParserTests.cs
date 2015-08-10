using NUnit.Framework;

namespace VCSJones.FiddlerCert.UnitTests
{
    [TestFixture]
    public class OidParserTests
    {
        [Test]
        public void ShouldHandleNullAndReturnNull()
        {
            Assert.That(OidParser.ReadFromBytes(null), Is.Null);
        }

        [Test]
        public void ShouldHandleEmptyArrayAndReturnNull()
        {
            Assert.That(OidParser.ReadFromBytes(new byte[0]), Is.Null);
        }

        [Test]
        public void ShouldHandleLengthTooShortEvenWithCorrectPreamble()
        {
            Assert.That(OidParser.ReadFromBytes(new byte[] { 6 }), Is.Null);
        }

        [Test]
        public void ShouldHandleValidValue()
        {
            var oid = OidParser.ReadFromBytes(new byte[] { 6, 8, 42, 134, 72, 206, 61, 3, 1, 7 });
            Assert.That(oid.Value, Is.EqualTo("1.2.840.10045.3.1.7"));
        }

        [Test]
        public void ShouldHandleIncorrectLengthCheck()
        {
            var oid = OidParser.ReadFromBytes(new byte[] { 6, 9, 42, 134, 72, 206, 61, 3, 1, 7 });
            Assert.That(oid, Is.Null);
        }

        [Test]
        public void ShouldHandleIncorrectlyTerminatedData()
        {
            var oid = OidParser.ReadFromBytes(new byte[] { 6, 8, 42, 134, 72, 206, 61, 3, 1, 255 });
            Assert.That(oid, Is.Null);
        }
    }
}
