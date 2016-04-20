using System;
using System.Runtime.InteropServices;

namespace VCSJones.FiddlerCert.Interop
{
    [type: StructLayout(LayoutKind.Sequential)]
    internal struct CERT_POLICIES_INFO
    {
        public uint cPolicyInfo;
        public IntPtr rgPolicyInfo;
    }

    [type: StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    internal struct CERT_POLICY_INFO
    {
        public string pszPolicyIdentifier;
        public uint cPolicyQualifier;
        public IntPtr rgPolicyQualifier;
    }

    [type: StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    internal struct CERT_POLICY_QUALIFIER_INFO
    {
        public string pszPolicyQualifierId;
        public CRYPT_OBJID_BLOB Qualifier;
    }
}