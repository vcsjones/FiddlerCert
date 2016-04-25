using System.Security.Cryptography.X509Certificates;
using Xunit;

namespace VCSJones.FiddlerCert.UnitTests
{
    public class SctDecoderTests
    {
        [Fact]
        public void Decode()
        {
            using (var cert = new X509Certificate2("cert-test-ct\\chromium-test-ct.cer"))
            {
                var extension = cert.Extensions[KnownOids.X509Extensions.CertificateTimeStampListCT];
                SctDecoder.DecodeData(extension);
            }
        }
    }
}
