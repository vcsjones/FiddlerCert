using System;
using System.Runtime.InteropServices;
using VCSJones.FiddlerCert.Interop;
using static VCSJones.FiddlerCert.KnownDecodeEncodeConstants;

namespace VCSJones.FiddlerCert
{
    public class CtVerifier
    {
        private readonly CtPublicKey _publicKey;
        public CtVerifier(byte[] publicKey)
        {
            var key = DecodeSubjectPublicKeyInfo(publicKey);
            if (key == null)
            {
                throw new ArgumentException("Could not decode public key.", nameof(publicKey));
            }
            _publicKey = key;
        }

        private static CtPublicKey DecodeSubjectPublicKeyInfo(byte[] publicKey)
        {
            const EncodingType encodingType = EncodingType.PKCS_7_ASN_ENCODING | EncodingType.X509_ASN_ENCODING;
            var handle = default(GCHandle);
            try
            {
                handle = GCHandle.Alloc(publicKey, GCHandleType.Pinned);
                LocalBufferSafeHandle buffer;
                var size = 0u;
                if (Crypto32.CryptDecodeObjectEx(encodingType, SUBJECT_PUBLIC_KEY_INFO, handle.AddrOfPinnedObject(), (uint)publicKey.Length, CryptDecodeFlags.CRYPT_DECODE_ALLOC_FLAG, IntPtr.Zero, out buffer, ref size))
                {
                    using (buffer)
                    {
                        //don't free the buffer until the structure's been fully read.
                        var structure = (CERT_PUBLIC_KEY_INFO)Marshal.PtrToStructure(buffer.DangerousGetHandle(), typeof(CERT_PUBLIC_KEY_INFO));
                        return FromWin32Structure(structure);
                    }
                }
                else
                {
                    return null;
                }
            }
            finally
            {
                if (handle.IsAllocated)
                {
                    handle.Free();
                }
            }
        }

        private unsafe static CtPublicKey FromWin32Structure(CERT_PUBLIC_KEY_INFO spki)
        {
            CtPublicKeyType type;
            switch (spki.Algorithm.pszObjId)
            {
                case KnownOids.X509Algorithms.RSA:
                    type = CtPublicKeyType.RSA_PKCS15;
                    break;
                case KnownOids.X509Algorithms.Ecc:
                    var parameters = new byte[spki.Algorithm.Parameters.cbData];
                    Marshal.Copy(new IntPtr(spki.Algorithm.Parameters.pbData), parameters, 0, parameters.Length);
                    var curve = OidParser.ReadFromBytes(parameters);
                    switch (curve?.Value)
                    {
                        case KnownOids.EccCurves.EcdsaP256:
                            type = CtPublicKeyType.ECDSA_P256;
                            break;
                        default:
                            return null;
                    }
                    break;
                default:
                    return null;
            }
            var publicKey = new byte[spki.PublicKey.cbData];
            Marshal.Copy(new IntPtr(spki.PublicKey.pbData), publicKey, 0, publicKey.Length);
            return new CtPublicKey
            {
                KeyType = type,
                Key = publicKey
            };
        }
    }

    public class CtPublicKey
    {
        public CtPublicKeyType KeyType { get; set; }
        public byte[] Key { get; set; }
    }

    public enum CtPublicKeyType
    {
        ECDSA_P256,
        RSA_PKCS15
    }
}
