using Xunit;

namespace VCSJones.FiddlerCert.UnitTests
{
    public class OidParserTests
    {
        [Fact]
        public void ShouldHandleNullAndReturnNull()
        {
            Assert.Null(OidParser.ReadFromBytes(null));
        }

        [Fact]
        public void ShouldHandleEmptyArrayAndReturnNull()
        {
            Assert.Null(OidParser.ReadFromBytes(new byte[0]));
        }

        [Fact]
        public void ShouldHandleLengthTooShortEvenWithCorrectPreamble()
        {
            Assert.Null(OidParser.ReadFromBytes(new byte[] { 6 }));
        }

        [Fact]
        public void ShouldHandleValidValue()
        {
            var oid = OidParser.ReadFromBytes(new byte[] { 6, 8, 42, 134, 72, 206, 61, 3, 1, 7 });
            Assert.Equal("1.2.840.10045.3.1.7", oid.Value);
        }

        [Fact]
        public void ShouldHandleIncorrectLengthCheck()
        {
            var oid = OidParser.ReadFromBytes(new byte[] { 6, 9, 42, 134, 72, 206, 61, 3, 1, 7 });
            Assert.Null(oid);
        }

        [Fact]
        public void ShouldHandleIncorrectlyTerminatedData()
        {
            var oid = OidParser.ReadFromBytes(new byte[] { 6, 8, 42, 134, 72, 206, 61, 3, 1, 255 });
            Assert.Null(oid);
        }
    }
}
