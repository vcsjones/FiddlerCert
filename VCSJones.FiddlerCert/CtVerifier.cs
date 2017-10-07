using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using VCSJones.FiddlerCert.Interop;
using static VCSJones.FiddlerCert.KnownDecodeEncodeConstants;

namespace VCSJones.FiddlerCert
{
    public class CtVerifier
    {
        private readonly CtPublicKey _key;

        public CtVerifier(byte[] publicKey)
        {
            var key = DecodeSubjectPublicKeyInfo(publicKey);
            _key = key ?? throw new ArgumentException("Could not decode public key.", nameof(publicKey));
            using (var sha = new SHA256Cng())
            {
                LogId = sha.ComputeHash(publicKey);
            }
        }

        public byte[] LogId { get; }

        public bool Verify(SctSignature signature)
        {
            if (signature.LogId.Length != LogId.Length || signature.LogId.Length == 0)
            {
                return false;
            }
            //verify this signature is for this log key.
            if (!signature.LogId.MemoryCompare(LogId))
            {
                return false;
            }
            SignatureVerifier verifier;
            switch (_key.KeyType)
            {
                case CtPublicKeyType.ECDSA_P256:
                    if (signature.SignatureAlgorithm != SctSignatureAlgorithm.SIG_ALGO_ECDSA)
                        return false;
                    verifier = new EcdsaP256SignatureVerifier(_key.Key);
                    break;
                case CtPublicKeyType.RSA_PKCS15:
                    if (signature.SignatureAlgorithm != SctSignatureAlgorithm.SIG_ALGO_RSA)
                        return false;
                    verifier = new RsaSignatureVerifier(_key.Key);
                    break;
                default:
                    return false;
            }
            return verifier.Verify(new byte[0], signature.Signature, signature.HashAlgorithm);
        }

        private static CtPublicKey DecodeSubjectPublicKeyInfo(byte[] publicKey)
        {
            const EncodingType encodingType = EncodingType.PKCS_7_ASN_ENCODING | EncodingType.X509_ASN_ENCODING;
            var handle = default(GCHandle);
            try
            {
                handle = GCHandle.Alloc(publicKey, GCHandleType.Pinned);
                var size = 0u;
                if (Crypto32.CryptDecodeObjectEx(encodingType, SUBJECT_PUBLIC_KEY_INFO, handle.AddrOfPinnedObject(), (uint)publicKey.Length, CryptDecodeFlags.CRYPT_DECODE_ALLOC_FLAG, IntPtr.Zero, out LocalBufferSafeHandle buffer, ref size))
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
            var parameters = new byte[spki.Algorithm.Parameters.cbData];
            if (spki.Algorithm.Parameters.cbData > 0)
            {
                Marshal.Copy(spki.Algorithm.Parameters.pbData, parameters, 0, parameters.Length);
            }
            switch (spki.Algorithm.pszObjId)
            {
                case KnownOids.X509Algorithms.RSA:
                    if (parameters.Length == 0 || IsAsnNull(parameters))
                        type = CtPublicKeyType.RSA_PKCS15;
                    else
                        goto default;
                    break;
                case KnownOids.X509Algorithms.Ecc:
                    var curve = OidParser.ReadFromBytes(parameters);
                    if (curve?.Value == KnownOids.EccCurves.EcdsaP256)
                        type = CtPublicKeyType.ECDSA_P256;
                    else
                        goto default;
                    break;
                default:
                    return null;
            }
            if (spki.PublicKey.cbData == 0)
            {
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

        private static bool IsAsnNull(byte[] data) => data.Length == 2 && data[0] == 5 && data[1] == 0;

        private abstract class SignatureVerifier
        {
            protected readonly byte[] _key;

            protected SignatureVerifier(byte[] key)
            {
                _key = key;
            }

            public abstract bool Verify(byte[] data, byte[] signature, SctHashAlgorithm algorithm);
        }

        private class EcdsaP256SignatureVerifier : SignatureVerifier
        {
            public EcdsaP256SignatureVerifier(byte[] key) : base(key)
            {
            }

            public override bool Verify(byte[] data, byte[] signature, SctHashAlgorithm algorithm)
            {
                if (!EcdsaKeyFormatter.ToEcdsa256PublicKeyBlob(_key, out byte[] blob))
                {
                    return false;
                }
                using (var key = CngKey.Import(blob, CngKeyBlobFormat.EccPublicBlob, CngProvider.MicrosoftSoftwareKeyStorageProvider))
                {
                    using (var ecdsa = new ECDsaCng(key))
                    {
                        var hashAlgorithm = SctHashAlgorithmToCng(algorithm);
                        if (hashAlgorithm == null)
                        {
                            return false;
                        }
                        ecdsa.HashAlgorithm = hashAlgorithm;
                        return ecdsa.VerifyData(data, signature);
                    }
                }
            }
        }

        private class RsaSignatureVerifier : SignatureVerifier
        {
            public RsaSignatureVerifier(byte[] key) : base(key)
            {
            }

            public override bool Verify(byte[] data, byte[] signature, SctHashAlgorithm algorithm)
            {
                var publicKey = new PublicKey(new Oid(KnownOids.X509Algorithms.RSA), new AsnEncodedData(_key), AsnNull);
                var rsa = publicKey.Key as RSACryptoServiceProvider;
                if (rsa == null)
                {
                    return false;
                }
                return rsa.VerifyData(data, SctHashAlgorithmToOid(algorithm), signature);
            }

            private readonly static AsnEncodedData AsnNull = new AsnEncodedData(new byte[] { 0, 5 });
        }


        private static CngAlgorithm SctHashAlgorithmToCng(SctHashAlgorithm algorithm)
        {
            switch (algorithm)
            {
                case SctHashAlgorithm.HASH_ALGO_SHA1:
                    return CngAlgorithm.Sha1;
                case SctHashAlgorithm.HASH_ALGO_SHA256:
                    return CngAlgorithm.Sha256;
                case SctHashAlgorithm.HASH_ALGO_SHA384:
                    return CngAlgorithm.Sha384;
                case SctHashAlgorithm.HASH_ALGO_SHA512:
                    return CngAlgorithm.Sha512;
                default:
                    return null;
            }
        }
        private static string SctHashAlgorithmToOid(SctHashAlgorithm algorithm)
        {
            switch (algorithm)
            {
                case SctHashAlgorithm.HASH_ALGO_SHA1:
                    return KnownOids.HashAlgorithms.sha1;
                case SctHashAlgorithm.HASH_ALGO_SHA256:
                    return KnownOids.HashAlgorithms.sha256;
                case SctHashAlgorithm.HASH_ALGO_SHA384:
                    return KnownOids.HashAlgorithms.sha384;
                case SctHashAlgorithm.HASH_ALGO_SHA512:
                    return KnownOids.HashAlgorithms.sha512;
                default:
                    return null;
            }
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
