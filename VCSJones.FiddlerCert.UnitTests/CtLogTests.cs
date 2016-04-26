using Xunit;

namespace VCSJones.FiddlerCert.UnitTests
{
    public class CtLogTests
    {
        [Fact]
        public void ParseKey()
        {
            var verifier = new CtVerifier(CtLogs.Logs[6].Key);
        }

    }
}
