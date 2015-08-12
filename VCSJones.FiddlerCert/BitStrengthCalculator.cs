using System.Security.Cryptography.X509Certificates;

namespace VCSJones.FiddlerCert
{
    public static class BitStrengthCalculator
    {
        public static CertificateBitStrength CalculateStrength(X509Certificate2 certificate)
        {
            PublicKeyAlgorithm keyAlgorithm;
            int? bitSize;
            switch (certificate.PublicKey.Oid.Value)
            {
                case KnownOids.EccPublicKey:
                    var parameterOid = OidParser.ReadFromBytes(certificate.PublicKey.EncodedParameters.RawData);
                    switch (parameterOid.Value)
                    {
                        case KnownOids.EcdsaP256:
                            keyAlgorithm = PublicKeyAlgorithm.ECDSA;
                            bitSize = 256;
                            break;
                        case KnownOids.EcdsaP384:
                            keyAlgorithm = PublicKeyAlgorithm.ECDSA;
                            bitSize = 384;
                            break;
                        case KnownOids.EcdsaP521:
                            keyAlgorithm = PublicKeyAlgorithm.ECDSA;
                            bitSize = 521;
                            break;
                        default:
                            keyAlgorithm = PublicKeyAlgorithm.Other;
                            bitSize = null;
                            break;
                    }
                    break;
                case KnownOids.RSA:
                    keyAlgorithm = PublicKeyAlgorithm.RSA;
                    bitSize = certificate.PublicKey.Key.KeySize;
                    break;
                default:
                    keyAlgorithm = PublicKeyAlgorithm.Other;
                    bitSize = null;
                    break;
            }
            return new CertificateBitStrength(keyAlgorithm, bitSize);
        }

        public class CertificateBitStrength
        {
            public CertificateBitStrength(PublicKeyAlgorithm algorithmName, int? bitSize)
            {
                AlgorithmName = algorithmName;
                BitSize = bitSize;
            }

            public PublicKeyAlgorithm AlgorithmName { get; }
            public int? BitSize { get; }
        }
    }
}
