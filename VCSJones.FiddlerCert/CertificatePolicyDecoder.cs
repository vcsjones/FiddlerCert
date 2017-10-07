using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using VCSJones.FiddlerCert.Interop;
using static VCSJones.FiddlerCert.KnownDecodeEncodeConstants;

namespace VCSJones.FiddlerCert
{
    public class CertificatePolicyDecoder
    {
        public IList<CertificatePolicy> GetPolicies(X509Certificate2 certificate)
        {
            var extension = certificate.Extensions[KnownOids.X509Extensions.CertificatePolicies];
            if (extension == null)
            {
                return new List<CertificatePolicy>();
            }
            var handle = default(GCHandle);
            try
            {
                handle = GCHandle.Alloc(extension.RawData, GCHandleType.Pinned);
                var size = 0u;
                const EncodingType encodingType = EncodingType.PKCS_7_ASN_ENCODING | EncodingType.X509_ASN_ENCODING;
                if (!Crypto32.CryptDecodeObjectEx(encodingType, X509_CERT_POLICIES, handle.AddrOfPinnedObject(), (uint)extension.RawData.Length, CryptDecodeFlags.CRYPT_DECODE_ALLOC_FLAG, IntPtr.Zero, out var buffer, ref size))
                {
                    //Can't decode it, gracefully retun an empty collection.
                    return new List<CertificatePolicy>();
                }
                using (buffer)
                {
                    var list = new List<CertificatePolicy>();
                    var policies = (CERT_POLICIES_INFO)Marshal.PtrToStructure(buffer.DangerousGetHandle(), typeof(CERT_POLICIES_INFO));
                    var certPolicySize = Marshal.SizeOf(typeof(CERT_POLICY_INFO));
                    for (var i = 0; i < policies.cPolicyInfo; i++)
                    {
                        var addr = new IntPtr(unchecked(((long)policies.rgPolicyInfo + (i * certPolicySize))));
                        var policy = (CERT_POLICY_INFO)Marshal.PtrToStructure(addr, typeof(CERT_POLICY_INFO));
                        var identifier = policy.pszPolicyIdentifier;
                        list.Add(new CertificatePolicy { PolicyOid = new Oid(policy.pszPolicyIdentifier) });
                    }
                    return list;
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
    }

    public class CertificatePolicy
    {
        public Oid PolicyOid { get; set; }
    }
}
