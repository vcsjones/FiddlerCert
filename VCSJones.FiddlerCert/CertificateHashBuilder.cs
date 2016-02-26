using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using VCSJones.FiddlerCert.Interop;

namespace VCSJones.FiddlerCert
{
    public static class CertificateHashBuilder
    {
        public static string BuildHashForPublicKey<THashAlgorithm>(X509Certificate2 certificate) where THashAlgorithm : HashAlgorithm, new()
        {
            return Convert.ToBase64String(BuildHashForPublicKeyBinary<THashAlgorithm>(certificate));
        }

        public static unsafe byte[] BuildHashForPublicKeyBinary<THashAlgorithm>(X509Certificate2 certificate) where THashAlgorithm : HashAlgorithm, new()
        {
            var PUBLIC_KEY_INFO_TYPE = new IntPtr(8);
            const uint CRYPT_ENCODE_ALLOC_FLAG = 0x8000u;
            var handle = LocalBufferSafeHandle.Zero;
            try
            {
                var publicKey = certificate.GetPublicKey();
                fixed (byte* publicKeyParametersPtr = certificate.PublicKey.EncodedParameters.RawData, publicKeyPtr = publicKey)
                {

                    var publicKeyInfo = new CERT_PUBLIC_KEY_INFO();
                    publicKeyInfo.Algorithm = new CRYPT_ALGORITHM_IDENTIFIER();
                    publicKeyInfo.PublicKey = new CRYPT_BIT_BLOB();
                    publicKeyInfo.Algorithm.pszObjId = certificate.PublicKey.EncodedKeyValue.Oid.Value;
                    publicKeyInfo.Algorithm.Parameters = new CRYPT_OBJID_BLOB();
                    publicKeyInfo.PublicKey.cbData = (uint) publicKey.Length;
                    publicKeyInfo.PublicKey.cUnusedBits = 0;
                    publicKeyInfo.PublicKey.pbData = publicKeyPtr;
                    publicKeyInfo.Algorithm.Parameters.cbData = (uint) certificate.PublicKey.EncodedParameters.RawData.Length;
                    publicKeyInfo.Algorithm.Parameters.pbData = publicKeyParametersPtr;
                    uint size = 0;
                    if (Crypto32.CryptEncodeObjectEx(EncodingType.X509_ASN_ENCODING, PUBLIC_KEY_INFO_TYPE, ref publicKeyInfo, CRYPT_ENCODE_ALLOC_FLAG, IntPtr.Zero, out handle, ref size))
                    {
                        var encoded = new byte[size];
                        Marshal.Copy(handle.DangerousGetHandle(), encoded, 0, encoded.Length);
                        using (var algorithm = new THashAlgorithm())
                        {
                            return algorithm.ComputeHash(encoded);
                        }
                    }
                }
            }
            finally
            {
                handle.Dispose();
            }
            return null;
        }
    }
}
