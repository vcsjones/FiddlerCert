using System;
using System.Runtime.InteropServices;
using VCSJones.FiddlerCert.Interop;

namespace VCSJones.FiddlerCert
{
    public static class EcdsaKeyFormatter
    {
        public static unsafe bool ToEcdsa256PublicKeyBlob(byte[] key, out byte[] blob)
        {
            const int bufferSize = 0x40, keySize = 0x20;
            //Uncompressed ECC key prefix
            if (key.Length != bufferSize+1 || key[0] != 0x04)
            {
                blob = null;
                return false;
            }
            var newKey = new byte[bufferSize];
            Buffer.BlockCopy(key, 1, newKey, 0, bufferSize);
            key = newKey;
            var structSize = Marshal.SizeOf(typeof(BCRYPT_ECCKEY_BLOB));
            var totalSize = structSize + key.Length;
            var structure = stackalloc BCRYPT_ECCKEY_BLOB[1];
            structure->dwMagic = EccBlobType.BCRYPT_ECDSA_PUBLIC_P256_MAGIC;
            structure->cbKey = keySize;
            var blobBuffer = new byte[totalSize];
            Marshal.Copy(new IntPtr(structure), blobBuffer, 0, structSize);
            Buffer.BlockCopy(key, 0, blobBuffer, structSize, key.Length);
            blob = blobBuffer;
            return true;
        }
    }
}
