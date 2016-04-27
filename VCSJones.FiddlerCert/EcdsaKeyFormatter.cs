using System;
using System.Runtime.InteropServices;
using VCSJones.FiddlerCert.Interop;

namespace VCSJones.FiddlerCert
{
    public static class EcdsaKeyFormatter
    {
        public static unsafe byte[] ToEcdsa256PublicKeyBlob(byte[] key)
        {
            const int bufferSize = 0x40, keySize = 0x20;
            //Uncompressed ECC key prefix
            if (key[0] == 0x04 && key.Length == 65)
            {
                var newKey = new byte[bufferSize];
                Buffer.BlockCopy(key, 1, newKey, 0, bufferSize);
                key = newKey;
            }
            var structSize = Marshal.SizeOf(typeof(BCRYPT_ECCKEY_BLOB));
            var totalSize = structSize + key.Length;
            var structure = stackalloc BCRYPT_ECCKEY_BLOB[1];
            structure->dwMagic = EccBlobType.BCRYPT_ECDSA_PUBLIC_P256_MAGIC;
            structure->cbKey = keySize;
            var blob = new byte[totalSize];
            Marshal.Copy(new IntPtr(structure), blob, 0, structSize);
            Buffer.BlockCopy(key, 0, blob, structSize, key.Length);
            return blob;
        }
    }
}
