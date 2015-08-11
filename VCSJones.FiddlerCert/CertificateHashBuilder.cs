using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using VCSJones.FiddlerCert.Interop;

namespace VCSJones.FiddlerCert
{
    public static class CertificateHashBuilder
    {
        public static string BuildHashForPublicKey<THashAlgorithm>(X509Certificate2 certificate) where THashAlgorithm:HashAlgorithm, new()
        {
            var PUBLIC_KEY_INFO_TYPE = new IntPtr(8);
            IntPtr publicKeyParametersBuffer = IntPtr.Zero, publicKeyBuffer = IntPtr.Zero, encodingBuffer = IntPtr.Zero;
            try
            {
                var publicKey = certificate.GetPublicKey();
                publicKeyParametersBuffer = Marshal.AllocCoTaskMem(certificate.PublicKey.EncodedParameters.RawData.Length);
                publicKeyBuffer = Marshal.AllocCoTaskMem(publicKey.Length);
                Marshal.Copy(certificate.PublicKey.EncodedParameters.RawData, 0, publicKeyParametersBuffer, certificate.PublicKey.EncodedParameters.RawData.Length);
                Marshal.Copy(publicKey, 0, publicKeyBuffer, publicKey.Length);
                var publicKeyInfo = new CERT_PUBLIC_KEY_INFO();
                publicKeyInfo.Algorithm = new CRYPT_ALGORITHM_IDENTIFIER();
                publicKeyInfo.PublicKey = new CRYPT_BIT_BLOB();
                publicKeyInfo.Algorithm.pszObjId = certificate.PublicKey.EncodedKeyValue.Oid.Value;
                publicKeyInfo.Algorithm.Parameters = new CRYPT_OBJID_BLOB();
                publicKeyInfo.PublicKey.cbData = (uint)publicKey.Length;
                publicKeyInfo.PublicKey.cUnusedBits = 0;
                publicKeyInfo.PublicKey.pbData = publicKeyBuffer;
                publicKeyInfo.Algorithm.Parameters.cbData = (uint)certificate.PublicKey.EncodedParameters.RawData.Length;
                publicKeyInfo.Algorithm.Parameters.pbData = publicKeyParametersBuffer;
                uint size = 0;
                if (Crypto32.CryptEncodeObject(1, PUBLIC_KEY_INFO_TYPE, publicKeyInfo, IntPtr.Zero, ref size))
                {
                    encodingBuffer = Marshal.AllocCoTaskMem((int)size);
                    if (Crypto32.CryptEncodeObject(1, PUBLIC_KEY_INFO_TYPE, publicKeyInfo, encodingBuffer, ref size))
                    {
                        var encoded = new byte[size];
                        Marshal.Copy(encodingBuffer, encoded, 0, encoded.Length);
                        using (var algorithm = new THashAlgorithm())
                        {
                            return Convert.ToBase64String(algorithm.ComputeHash(encoded));
                        }
                    }
                }
            }
            finally
            {
                Marshal.FreeCoTaskMem(publicKeyParametersBuffer);
                Marshal.FreeCoTaskMem(encodingBuffer);
                Marshal.FreeCoTaskMem(publicKeyBuffer);
            }
            return null;
        }
    }
}
