using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Xunit;

namespace VCSJones.FiddlerCert.UnitTests
{
    public class SctDecoderTests
    {
        [Fact]
        public void Decode()
        {
            using (var cert = new X509Certificate2("cert-test-ct\\symantec.cer"))
            {
                var extension = cert.Extensions[KnownOids.X509Extensions.CertificateTimeStampListCT];
                var scts = SctDecoder.DecodeData(extension);
                Assert.Equal(2, scts.Count);
                var sct = scts[0]; //Pilot
                Assert.Equal(SctHashAlgorithm.HASH_ALGO_SHA256, sct.HashAlgorithm);
                Assert.Equal(SctSignatureAlgorithm.SIG_ALGO_ECDSA, sct.SignatureAlgorithm);
            }
        }
    }
}
