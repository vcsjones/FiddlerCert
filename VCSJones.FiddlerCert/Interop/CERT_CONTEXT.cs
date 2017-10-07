using System;
using System.Runtime.InteropServices;

namespace VCSJones.FiddlerCert.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct CERT_CONTEXT
    {
        public EncodingType dwCertEncodingType;
        public IntPtr pbCertEncoded;
        public uint cbCertEncoded;
        public IntPtr pCertInfo;
        public IntPtr hCertStore;
    }
}
