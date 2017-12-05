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
        public unsafe byte* pbData;
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
        private const string CRYPT32 = "crypt32.dll";
        [method: DllImport(CRYPT32, CallingConvention = CallingConvention.Winapi, EntryPoint = "CryptEncodeObjectEx", SetLastError = true)]
        public static extern bool CryptEncodeObjectEx
            (
            [param: In, MarshalAs(UnmanagedType.U4)] EncodingType dwCertEncodingType,
            [param: In, MarshalAs(UnmanagedType.SysInt)] IntPtr lpszStructType,
            [param: In] ref CERT_PUBLIC_KEY_INFO pvStructInfo,
            [param: In, MarshalAs(UnmanagedType.U4)] uint dwFlags,
            [param: In, MarshalAs(UnmanagedType.SysInt)] IntPtr pEncodePara,
            [param: Out] out LocalBufferSafeHandle pvEncoded,
            [param: In, Out, MarshalAs(UnmanagedType.U4)] ref uint pcbEncoded
            );

        [method: DllImport(CRYPT32, CallingConvention = CallingConvention.Winapi, EntryPoint = "CryptDecodeObjectEx", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern unsafe bool CryptDecodeObjectEx
        (
            [param: In, MarshalAs(UnmanagedType.U4)] EncodingType dwCertEncodingType,
            [param: In, MarshalAs(UnmanagedType.SysInt)] IntPtr lpszStructType,
            [param: In, MarshalAs(UnmanagedType.SysInt)] IntPtr pbEncoded,
            [param: In, MarshalAs(UnmanagedType.U4)] uint cbEncoded,
            [param: In, MarshalAs(UnmanagedType.U4)] CryptDecodeFlags dwFlags,
            [param: In, MarshalAs(UnmanagedType.SysInt)] IntPtr pDecodePara,
            [param: Out] out LocalBufferSafeHandle pvStructInfo,
            [param: In, Out, MarshalAs(UnmanagedType.U4)] ref uint pcbStructInfo
        );
    }

    [Flags]
    public enum EncodingType : uint
    {
        PKCS_7_ASN_ENCODING = 65536,
        X509_ASN_ENCODING = 1
    }

    [type: Flags]
    internal enum CryptDecodeFlags : uint
    {
        CRYPT_DECODE_ALLOC_FLAG = 0x8000,
        CRYPT_DECODE_NO_SIGNATURE_BYTE_REVERSAL_FLAG = 0x8
    }
}
