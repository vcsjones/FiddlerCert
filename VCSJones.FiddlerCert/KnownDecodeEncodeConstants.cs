using System;

namespace VCSJones.FiddlerCert
{
    internal class KnownDecodeEncodeConstants
    {
        public static IntPtr X509_OCTET_STRING { get; } = (IntPtr)25;
        public static IntPtr X509_CERT_POLICIES { get; } = (IntPtr)16;
        public static IntPtr SUBJECT_PUBLIC_KEY_INFO { get; } = (IntPtr)8;
    }
}
