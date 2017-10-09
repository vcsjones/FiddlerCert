using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using VCSJones.FiddlerCert.Interop;
using static VCSJones.FiddlerCert.KnownDecodeEncodeConstants;

namespace VCSJones.FiddlerCert
{
    public static class CertificateHashBuilder
    {
        public static byte[] BuildHashForCertificateBinary<THashAlgotihm>(X509Certificate2 certificate) where THashAlgotihm : HashAlgorithm, new()
        {
            var context = (CERT_CONTEXT)Marshal.PtrToStructure(certificate.Handle, typeof(CERT_CONTEXT));
            if (context.dwCertEncodingType == EncodingType.X509_ASN_ENCODING)
            {
                var contents = new byte[context.cbCertEncoded];
                Marshal.Copy(context.pbCertEncoded, contents, 0, contents.Length);
                using (var hash = new THashAlgotihm())
                {
                    return hash.ComputeHash(contents);
                }
            }
            else
            {
                return null;
            }
        }

        public static string BuildHashForCertificateHex<THashAlgotihm>(X509Certificate2 certificate) where THashAlgotihm : HashAlgorithm, new()
        {
            return BitConverter.ToString(BuildHashForCertificateBinary<THashAlgotihm>(certificate)).Replace("-", "");
        }

        public static string BuildHashForPublicKey<THashAlgorithm>(X509Certificate2 certificate) where THashAlgorithm : HashAlgorithm, new()
        {
            return Convert.ToBase64String(BuildHashForPublicKeyBinary<THashAlgorithm>(certificate));
        }

        public static unsafe byte[] BuildHashForPublicKeyBinary<THashAlgorithm>(X509Certificate2 certificate) where THashAlgorithm : HashAlgorithm, new()
        {
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
                    publicKeyInfo.Algorithm.Parameters.pbData = new IntPtr(publicKeyParametersPtr);
                    uint size = 0;
                    if (Crypto32.CryptEncodeObjectEx(EncodingType.X509_ASN_ENCODING, SUBJECT_PUBLIC_KEY_INFO, ref publicKeyInfo, CRYPT_ENCODE_ALLOC_FLAG, IntPtr.Zero, out handle, ref size))
                    {
                        var encoded = new byte[size];
                        handle.CopyTo(encoded);
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
