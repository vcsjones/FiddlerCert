using System.Runtime.InteropServices;

namespace VCSJones.FiddlerCert.Interop
{
    internal enum EccBlobType : uint
    {
        BCRYPT_ECDSA_PUBLIC_P256_MAGIC = 0x31534345,
        BCRYPT_ECDSA_PRIVATE_P256_MAGIC = 0x32534345,
        BCRYPT_ECDSA_PUBLIC_P384_MAGIC = 0x33534345,
        BCRYPT_ECDSA_PRIVATE_P384_MAGIC = 0x34534345,
        BCRYPT_ECDSA_PUBLIC_P521_MAGIC = 0x35534345,
        BCRYPT_ECDSA_PRIVATE_P521_MAGIC = 0x36534345,
    }

    [type: StructLayout(LayoutKind.Sequential)]
    internal struct BCRYPT_ECCKEY_BLOB
    {
        public EccBlobType dwMagic;
        public uint cbKey;
    }
}
