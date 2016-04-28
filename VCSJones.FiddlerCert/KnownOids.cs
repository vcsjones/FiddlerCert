namespace VCSJones.FiddlerCert
{
    public static class KnownOids
    {
        public static class X509Algorithms
        {
            public const string RSA = "1.2.840.113549.1.1.1";
            public const string Ecc = "1.2.840.10045.2.1";
        }

        public static class X509Extensions
        {
            public const string SubjectAltNameExtension = "2.5.29.17";
            public const string CertificatePolicies = "2.5.29.32";
            public const string CertificateTimeStampListCT = "1.3.6.1.4.1.11129.2.4.2";
        }

        public static class EccCurves
        {
            public const string EcdsaP256 = "1.2.840.10045.3.1.7";
            public const string EcdsaP384 = "1.3.132.0.34";
            public const string EcdsaP521 = "1.3.132.0.35";
        }

        public static class SignatureAlgorithms
        {
            public const string md5RSA = "1.2.840.113549.1.1.4";
            public const string sha1DSA = "1.2.840.10040.4.3";
            public const string sha1RSA = "1.2.840.113549.1.1.5";
            public const string sha256RSA = "1.2.840.113549.1.1.11";
            public const string sha384RSA = "1.2.840.113549.1.1.12";
            public const string sha512RSA = "1.2.840.113549.1.1.13";
            public const string sha1ECDSA = "1.2.840.10045.4.1";
            public const string sha256ECDSA = "1.2.840.10045.4.3.2";
            public const string sha384ECDSA = "1.2.840.10045.4.3.3";
            public const string sha512ECDSA = "1.2.840.10045.4.3.4";
        }

        public static class HashAlgorithms
        {
            public const string sha1 = "1.3.14.3.2.26";
            public const string sha256 = "2.16.840.1.101.3.4.2.1";
            public const string sha384 = "2.16.840.1.101.3.4.2.2";
            public const string sha512 = "2.16.840.1.101.3.4.2.3";
        }

        public static class Policies
        {
            public const string OrganizationValidated = "2.23.140.1.2.2";
        }
    }
}