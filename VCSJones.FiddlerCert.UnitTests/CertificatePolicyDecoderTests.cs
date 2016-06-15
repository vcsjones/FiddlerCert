

using System.Security.Cryptography.X509Certificates;
using Xunit;

namespace VCSJones.FiddlerCert.UnitTests
{
    public class CertificatePolicyDecoderTests
    {
        [Fact]
        public void ShouldReturnEmptyOnCertificateWithNoPolicies()
        {
            using (var cert = new X509Certificate2(X509Certificate.CreateFromCertFile("cert-test-policy/nopolicy.cer")))
            {
                var decoder = new CertificatePolicyDecoder();
                var policies = decoder.GetPolicies(cert);
                Assert.Empty(policies);
            }
        }

        [Fact]
        public void ShouldReturnAllPolicies()
        {
            using (var cert = new X509Certificate2(X509Certificate.CreateFromCertFile("cert-test-policy/ev-cert.cer")))
            {
                var decoder = new CertificatePolicyDecoder();
                var policies = decoder.GetPolicies(cert);
                Assert.Collection(policies, p => Assert.Equal("2.16.840.1.113733.1.7.23.6", p.PolicyOid.Value));
            }
        }
    }
}
