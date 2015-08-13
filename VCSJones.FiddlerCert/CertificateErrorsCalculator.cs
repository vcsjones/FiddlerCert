using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace VCSJones.FiddlerCert
{
    public static class CertificateErrorsCalculator
    {
        public static CertificateErrors GetCertificateErrors(X509ChainElement chainElement)
        {
            //If the length is not zero and the items aren't "OK", return critical.
            if (chainElement.ChainElementStatus.Length != 0 && !chainElement.ChainElementStatus.All(s => s.Status == X509ChainStatusFlags.NoError))
            {
                return CertificateErrors.Critical;
            }
            //Recheck with revocation
            var chain = new X509Chain();
            chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
            chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EndCertificateOnly;
            chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags & ~X509VerificationFlags.IgnoreEndRevocationUnknown;
            var builtChain = chain.Build(chainElement.Certificate);
            if (chain.ChainStatus.Any(s => s.Status == X509ChainStatusFlags.RevocationStatusUnknown))
            {
                return CertificateErrors.UnknownRevocation;
            }
            if (!builtChain)
            {
                return CertificateErrors.Critical;
            }
            return CertificateErrors.None;
        }
    }
}