using System.Security.Cryptography.X509Certificates;

namespace VCSJones.FiddlerCert
{
    public static class BitStrengthCalculator
    {
        public static CertificateBitStrength CalculateStrength(X509Certificate2 certificate)
        {
            string algorithmName;
            int? bitSize;
            if (certificate.PublicKey.Oid.Value == KnownOids.EccPublicKey)
            {
                var parameterOid = OidParser.ReadFromBytes(certificate.PublicKey.EncodedParameters.RawData);
                algorithmName = $"{certificate.PublicKey.Oid.FriendlyName} ({parameterOid.FriendlyName})";
                switch (parameterOid.Value)
                {
                    case KnownOids.EcdsaP256:
                        bitSize = 256;
                        break;
                    case KnownOids.EcdsaP384:
                        bitSize = 384;
                        break;
                    case KnownOids.EcdsaP521:
                        bitSize = 521;
                        break;
                    default:
                        bitSize = null;
                        break;
                }
            }
            else
            {
                algorithmName = certificate.PublicKey.Oid.FriendlyName;
                bitSize = certificate.PublicKey.Key.KeySize;
            }
            return new CertificateBitStrength(algorithmName, bitSize);
        }
    }

    public class CertificateBitStrength
    {
        public CertificateBitStrength(string algorithmName, int? bitSize)
        {
            AlgorithmName = algorithmName;
            BitSize = bitSize;
        }

        public string AlgorithmName { get; }
        public int? BitSize { get; }
    }
}
