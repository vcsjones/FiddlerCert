using System.Security.Cryptography.X509Certificates;
using Xunit;

namespace VCSJones.FiddlerCert.UnitTests
{
    public class CtLogTests
    {
        [Fact]
        public void ParseKey()
        {
            using (var cert = new X509Certificate2("cert-test-ct\\symantec.cer"))
            {
                var extension = cert.Extensions[KnownOids.X509Extensions.CertificateTimeStampListCT];
                var scts = SctDecoder.DecodeData(extension);
                Assert.Equal(2, scts.Count);
                var sct = scts[0]; //Pilot
                var ctKey = CtLogs.FindByLogId(sct.LogId);
                var verifier = new CtVerifier(ctKey.Key);
                Assert.True(verifier.Verify(sct));
            }
        }
    }
}
