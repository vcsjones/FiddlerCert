using System;
using System.Runtime.InteropServices;

namespace VCSJones.FiddlerCert.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct CERT_PUBLIC_KEY_INFO
    {
        public CRYPT_ALGORITHM_IDENTIFIER Algorithm;
        public CRYPT_BIT_BLOB PublicKey;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    internal struct CRYPT_ALGORITHM_IDENTIFIER
    {
        public string pszObjId;
        public CRYPT_OBJID_BLOB Parameters;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct CRYPT_BIT_BLOB
    {
        public uint cbData;
        public IntPtr pbData;
        public uint cUnusedBits;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct CRYPT_OBJID_BLOB
    {
        public uint cbData;
        public IntPtr pbData;
    }
    internal static class Crypto32
    {
        [method: DllImport("Crypt32.dll", CallingConvention = CallingConvention.Winapi, SetLastError = true)]
        internal static extern bool CryptEncodeObject(
                [In, MarshalAs(UnmanagedType.U4)]uint dwCertEncodingType,
                [In, MarshalAs(UnmanagedType.SysInt)]IntPtr lpszStructType,
                [In, MarshalAs(UnmanagedType.Struct)]CERT_PUBLIC_KEY_INFO pInfo,
                [In, MarshalAs(UnmanagedType.SysInt)] IntPtr pbEncoded,
                [In, Out, MarshalAs(UnmanagedType.U4)]ref uint cbEncoded
            );
    }
}
