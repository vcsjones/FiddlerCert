using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using VCSJones.FiddlerCert.Interop;

namespace VCSJones.FiddlerCert
{
    internal class CertificateValidatedChecker
    {
        public static bool IsCertificateExtendedValidation(X509Certificate2 certificate, X509Chain chain)
        {
            var decoder = new CertificatePolicyDecoder();
            var rootCertificate = chain.ChainElements[chain.ChainElements.Count - 1].Certificate;
            var policies = decoder.GetPolicies(certificate);
            if (policies.Count == 0)
            {
                return false;
            }
            var rootThumbprint = rootCertificate.GetCertHash();
            if (rootThumbprint.Length != 20)
            {
                return false;
            }
            var findRoot = Array.Find(EVRootCAMetadata, p => Msvcrt.memcmp(p.Sha1Fingerprint, rootThumbprint, new UIntPtr(20u)) == 0);
            if (findRoot == null)
            {
                return false;
            }
            var oids = Array.FindAll(findRoot.PolicyOids, p => !string.IsNullOrWhiteSpace(p));
            return oids.Intersect(policies.Select(p => p.PolicyOid.Value)).Any();
        }

        public static bool IsCertificateOrganizationValidated(X509Certificate2 certificate)
        {
            var decoder = new CertificatePolicyDecoder();
            var policies = decoder.GetPolicies(certificate);
            if (policies.Count == 0)
            {
                return false;
            }
            return OVOids.Intersect(policies.Select(p => p.PolicyOid.Value)).Any();
        }

        //Converted from https://code.google.com/p/chromium/codesearch#chromium/src/net/cert/ev_root_ca_metadata.cc
        //Last updated: 4/20/2016 @ 15:40 EDT
        //Git SHA: dc2cea95fae5b9f91db96f79398529a0eff4a3d6 
        public static ExtendedValidationMetadata[] EVRootCAMetadata { get; } = new[]
        {
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0x78, 0x6a, 0x74, 0xac, 0x76, 0xab, 0x14, 0x7f, 0x9c, 0x6a,
                      0x30, 0x50, 0xba, 0x9e, 0xa8, 0x7e, 0xfe, 0x9a, 0xce, 0x3c},
            PolicyOids = new string[] {
                        "1.3.6.1.4.1.17326.10.14.2.1.2", "1.3.6.1.4.1.17326.10.14.2.2.2",
                    }
            },
                // AC Camerfirma S.A. Global Chambersign Root - 2008
                // https://server2.camerfirma.com:8082
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0x4a, 0xbd, 0xee, 0xec, 0x95, 0x0d, 0x35, 0x9c, 0x89, 0xae,
                      0xc7, 0x52, 0xa1, 0x2c, 0x5b, 0x29, 0xf6, 0xd6, 0xaa, 0x0c},
            PolicyOids = new string[] {
                        // AC Camerfirma uses the last two arcs to track how the private key
                        // is
                        // managed - the effective verification policy is the same.
                        "1.3.6.1.4.1.17326.10.8.12.1.2", "1.3.6.1.4.1.17326.10.8.12.2.2",
                    }
            },
                // AddTrust External CA Root
                // https://addtrustexternalcaroot-ev.comodoca.com
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0x02, 0xfa, 0xf3, 0xe2, 0x91, 0x43, 0x54, 0x68, 0x60, 0x78,
                      0x57, 0x69, 0x4d, 0xf5, 0xe4, 0x5b, 0x68, 0x85, 0x18, 0x68},
            PolicyOids = new string[] {
                        "1.3.6.1.4.1.6449.1.2.1.5.1",
                        // This is the Network Solutions EV OID. However, this root
                        // cross-certifies NetSol and so we need it here too.
                        "1.3.6.1.4.1.782.1.2.1.8.1",
                    }
            },
                // Actalis Authentication Root CA
                // https://ssltest-a.actalis.it:8443
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0xf3, 0x73, 0xb3, 0x87, 0x06, 0x5a, 0x28, 0x84, 0x8a, 0xf2,
                      0xf3, 0x4a, 0xce, 0x19, 0x2b, 0xdd, 0xc7, 0x8e, 0x9c, 0xac},
            PolicyOids = new string[] {"1.3.159.1.17.1", ""}
            },
                // AffirmTrust Commercial
                // https://commercial.affirmtrust.com/
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0xf9, 0xb5, 0xb6, 0x32, 0x45, 0x5f, 0x9c, 0xbe, 0xec, 0x57,
                      0x5f, 0x80, 0xdc, 0xe9, 0x6e, 0x2c, 0xc7, 0xb2, 0x78, 0xb7},
            PolicyOids = new string[] {"1.3.6.1.4.1.34697.2.1", ""}
            },
                // AffirmTrust Networking
                // https://networking.affirmtrust.com:4431
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0x29, 0x36, 0x21, 0x02, 0x8b, 0x20, 0xed, 0x02, 0xf5, 0x66,
                      0xc5, 0x32, 0xd1, 0xd6, 0xed, 0x90, 0x9f, 0x45, 0x00, 0x2f},
            PolicyOids = new string[] {"1.3.6.1.4.1.34697.2.2", ""}
            },
                // AffirmTrust Premium
                // https://premium.affirmtrust.com:4432/
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0xd8, 0xa6, 0x33, 0x2c, 0xe0, 0x03, 0x6f, 0xb1, 0x85, 0xf6,
                      0x63, 0x4f, 0x7d, 0x6a, 0x06, 0x65, 0x26, 0x32, 0x28, 0x27},
            PolicyOids = new string[] {"1.3.6.1.4.1.34697.2.3", ""}
            },
                // AffirmTrust Premium ECC
                // https://premiumecc.affirmtrust.com:4433/
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0xb8, 0x23, 0x6b, 0x00, 0x2f, 0x1d, 0x16, 0x86, 0x53, 0x01,
                      0x55, 0x6c, 0x11, 0xa4, 0x37, 0xca, 0xeb, 0xff, 0xc3, 0xbb},
            PolicyOids = new string[] {"1.3.6.1.4.1.34697.2.4", ""}
            },
                // Autoridad de Certificacion Firmaprofesional CIF A62634068
                // https://publifirma.firmaprofesional.com/
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0xae, 0xc5, 0xfb, 0x3f, 0xc8, 0xe1, 0xbf, 0xc4, 0xe5, 0x4f,
                   0x03, 0x07, 0x5a, 0x9a, 0xe8, 0x00, 0xb7, 0xf7, 0xb6, 0xfa},
            PolicyOids = new string[] {"1.3.6.1.4.1.13177.10.1.3.10", ""}
            },
                // Baltimore CyberTrust Root
                // https://secure.omniroot.com/repository/
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0xd4, 0xde, 0x20, 0xd0, 0x5e, 0x66, 0xfc, 0x53, 0xfe, 0x1a,
                      0x50, 0x88, 0x2c, 0x78, 0xdb, 0x28, 0x52, 0xca, 0xe4, 0x74},
            PolicyOids = new string[] {"1.3.6.1.4.1.6334.1.100.1", ""}
            },
                // Buypass Class 3 CA 1
                // https://valid.evident.ca13.ssl.buypass.no/
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0x61, 0x57, 0x3A, 0x11, 0xDF, 0x0E, 0xD8, 0x7E, 0xD5, 0x92,
                      0x65, 0x22, 0xEA, 0xD0, 0x56, 0xD7, 0x44, 0xB3, 0x23, 0x71},
            PolicyOids = new string[] {"2.16.578.1.26.1.3.3", ""}
            },
                // Buypass Class 3 Root CA
                // https://valid.evident.ca23.ssl.buypass.no/
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0xDA, 0xFA, 0xF7, 0xFA, 0x66, 0x84, 0xEC, 0x06, 0x8F, 0x14,
                      0x50, 0xBD, 0xC7, 0xC2, 0x81, 0xA5, 0xBC, 0xA9, 0x64, 0x57},
            PolicyOids = new string[] {"2.16.578.1.26.1.3.3", ""}
            },
                // CA 沃通根证书
                // https://root2evtest.wosign.com/
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0x16, 0x32, 0x47, 0x8d, 0x89, 0xf9, 0x21, 0x3a, 0x92, 0x00,
                      0x85, 0x63, 0xf5, 0xa4, 0xa7, 0xd3, 0x12, 0x40, 0x8a, 0xd6},
            PolicyOids = new string[] {"1.3.6.1.4.1.36305.2", ""}
            },
                // Certification Authority of WoSign
                // https://root1evtest.wosign.com/
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0xb9, 0x42, 0x94, 0xbf, 0x91, 0xea, 0x8f, 0xb6, 0x4b, 0xe6,
                      0x10, 0x97, 0xc7, 0xfb, 0x00, 0x13, 0x59, 0xb6, 0x76, 0xcb},
            PolicyOids = new string[] {"1.3.6.1.4.1.36305.2", ""}
            },
                // CertPlus Class 2 Primary CA (KEYNECTIS)
                // https://www.keynectis.com/
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0x74, 0x20, 0x74, 0x41, 0x72, 0x9c, 0xdd, 0x92, 0xec, 0x79,
                      0x31, 0xd8, 0x23, 0x10, 0x8d, 0xc2, 0x81, 0x92, 0xe2, 0xbb},
            PolicyOids = new string[] {"1.3.6.1.4.1.22234.2.5.2.3.1", ""}
            },
                // Certum Trusted Network CA
                // https://juice.certum.pl/
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0x07, 0xe0, 0x32, 0xe0, 0x20, 0xb7, 0x2c, 0x3f, 0x19, 0x2f,
                      0x06, 0x28, 0xa2, 0x59, 0x3a, 0x19, 0xa7, 0x0f, 0x06, 0x9e},
            PolicyOids = new string[] {"1.2.616.1.113527.2.5.1.1", ""}
            },
                // China Internet Network Information Center EV Certificates Root
                // https://evdemo.cnnic.cn/
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0x4F, 0x99, 0xAA, 0x93, 0xFB, 0x2B, 0xD1, 0x37, 0x26, 0xA1,
                      0x99, 0x4A, 0xCE, 0x7F, 0xF0, 0x05, 0xF2, 0x93, 0x5D, 0x1E},
            PolicyOids = new string[] {"1.3.6.1.4.1.29836.1.10", ""}
            },
                // COMODO Certification Authority
                // https://secure.comodo.com/
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0x66, 0x31, 0xbf, 0x9e, 0xf7, 0x4f, 0x9e, 0xb6, 0xc9, 0xd5,
                      0xa6, 0x0c, 0xba, 0x6a, 0xbe, 0xd1, 0xf7, 0xbd, 0xef, 0x7b},
            PolicyOids = new string[] {"1.3.6.1.4.1.6449.1.2.1.5.1", ""}
            },
                // COMODO Certification Authority (reissued certificate with NotBefore of
                // Jan 1 00:00:00 2011 GMT)
                // https://secure.comodo.com/
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0xee, 0x86, 0x93, 0x87, 0xff, 0xfd, 0x83, 0x49, 0xab, 0x5a,
                      0xd1, 0x43, 0x22, 0x58, 0x87, 0x89, 0xa4, 0x57, 0xb0, 0x12},
            PolicyOids = new string[] {"1.3.6.1.4.1.6449.1.2.1.5.1", ""}
            },
                // COMODO ECC Certification Authority
                // https://comodoecccertificationauthority-ev.comodoca.com/
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0x9f, 0x74, 0x4e, 0x9f, 0x2b, 0x4d, 0xba, 0xec, 0x0f, 0x31,
                      0x2c, 0x50, 0xb6, 0x56, 0x3b, 0x8e, 0x2d, 0x93, 0xc3, 0x11},
            PolicyOids = new string[] {"1.3.6.1.4.1.6449.1.2.1.5.1", ""}
            },
                // COMODO RSA Certification Authority
                // https://comodorsacertificationauthority-ev.comodoca.com/
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0xaf, 0xe5, 0xd2, 0x44, 0xa8, 0xd1, 0x19, 0x42, 0x30, 0xff,
                      0x47, 0x9f, 0xe2, 0xf8, 0x97, 0xbb, 0xcd, 0x7a, 0x8c, 0xb4},
            PolicyOids = new string[] {"1.3.6.1.4.1.6449.1.2.1.5.1", ""}
            },
                // Cybertrust Global Root
                // https://evup.cybertrust.ne.jp/ctj-ev-upgrader/evseal.gif
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0x5f, 0x43, 0xe5, 0xb1, 0xbf, 0xf8, 0x78, 0x8c, 0xac, 0x1c,
                      0xc7, 0xca, 0x4a, 0x9a, 0xc6, 0x22, 0x2b, 0xcc, 0x34, 0xc6},
            PolicyOids = new string[] {"1.3.6.1.4.1.6334.1.100.1", ""}
            },
                // DigiCert High Assurance EV Root CA
                // https://www.digicert.com
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0x5f, 0xb7, 0xee, 0x06, 0x33, 0xe2, 0x59, 0xdb, 0xad, 0x0c,
                      0x4c, 0x9a, 0xe6, 0xd3, 0x8f, 0x1a, 0x61, 0xc7, 0xdc, 0x25},
            PolicyOids = new string[] {"2.16.840.1.114412.2.1", ""}
            },
                // D-TRUST Root Class 3 CA 2 EV 2009
                // https://certdemo-ev-valid.ssl.d-trust.net/
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0x96, 0xc9, 0x1b, 0x0b, 0x95, 0xb4, 0x10, 0x98, 0x42, 0xfa,
                      0xd0, 0xd8, 0x22, 0x79, 0xfe, 0x60, 0xfa, 0xb9, 0x16, 0x83},
            PolicyOids = new string[] {"1.3.6.1.4.1.4788.2.202.1", ""}
            },
                // Entrust.net Secure Server Certification Authority
                // https://www.entrust.net/
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0x99, 0xa6, 0x9b, 0xe6, 0x1a, 0xfe, 0x88, 0x6b, 0x4d, 0x2b,
                      0x82, 0x00, 0x7c, 0xb8, 0x54, 0xfc, 0x31, 0x7e, 0x15, 0x39},
            PolicyOids = new string[] {"2.16.840.1.114028.10.1.2", ""}
            },
                // Entrust Root Certification Authority
                // https://www.entrust.net/
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0xb3, 0x1e, 0xb1, 0xb7, 0x40, 0xe3, 0x6c, 0x84, 0x02, 0xda,
                      0xdc, 0x37, 0xd4, 0x4d, 0xf5, 0xd4, 0x67, 0x49, 0x52, 0xf9},
            PolicyOids = new string[] {"2.16.840.1.114028.10.1.2", ""}
            },
                // Equifax Secure Certificate Authority (GeoTrust)
                // https://www.geotrust.com/
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0xd2, 0x32, 0x09, 0xad, 0x23, 0xd3, 0x14, 0x23, 0x21, 0x74,
                      0xe4, 0x0d, 0x7f, 0x9d, 0x62, 0x13, 0x97, 0x86, 0x63, 0x3a},
            PolicyOids = new string[] {"1.3.6.1.4.1.14370.1.6", ""}
            },
                // E-Tugra Certification Authority
                // https://sslev.e-tugra.com.tr
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0x51, 0xC6, 0xE7, 0x08, 0x49, 0x06, 0x6E, 0xF3, 0x92, 0xD4,
                      0x5C, 0xA0, 0x0D, 0x6D, 0xA3, 0x62, 0x8F, 0xC3, 0x52, 0x39},
            PolicyOids = new string[] {"2.16.792.3.0.4.1.1.4", ""}
            },
                // GeoTrust Primary Certification Authority
                // https://www.geotrust.com/
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0x32, 0x3c, 0x11, 0x8e, 0x1b, 0xf7, 0xb8, 0xb6, 0x52, 0x54,
                      0xe2, 0xe2, 0x10, 0x0d, 0xd6, 0x02, 0x90, 0x37, 0xf0, 0x96},
            PolicyOids = new string[] {"1.3.6.1.4.1.14370.1.6", ""}
            },
                // GeoTrust Primary Certification Authority - G2
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0x8d, 0x17, 0x84, 0xd5, 0x37, 0xf3, 0x03, 0x7d, 0xec, 0x70,
                      0xfe, 0x57, 0x8b, 0x51, 0x9a, 0x99, 0xe6, 0x10, 0xd7, 0xb0},
            PolicyOids = new string[] {"1.3.6.1.4.1.14370.1.6", ""}
            },
                // GeoTrust Primary Certification Authority - G3
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0x03, 0x9e, 0xed, 0xb8, 0x0b, 0xe7, 0xa0, 0x3c, 0x69, 0x53,
                      0x89, 0x3b, 0x20, 0xd2, 0xd9, 0x32, 0x3a, 0x4c, 0x2a, 0xfd},
            PolicyOids = new string[] {"1.3.6.1.4.1.14370.1.6", ""}
            },
                // GlobalSign Root CA - R2
                // https://www.globalsign.com/
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0x75, 0xe0, 0xab, 0xb6, 0x13, 0x85, 0x12, 0x27, 0x1c, 0x04,
                      0xf8, 0x5f, 0xdd, 0xde, 0x38, 0xe4, 0xb7, 0x24, 0x2e, 0xfe},
            PolicyOids = new string[] {"1.3.6.1.4.1.4146.1.1", ""}
            },
                // GlobalSign Root CA
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0xb1, 0xbc, 0x96, 0x8b, 0xd4, 0xf4, 0x9d, 0x62, 0x2a, 0xa8,
                      0x9a, 0x81, 0xf2, 0x15, 0x01, 0x52, 0xa4, 0x1d, 0x82, 0x9c},
            PolicyOids = new string[] {"1.3.6.1.4.1.4146.1.1", ""}
            },
                // GlobalSign Root CA - R3
                // https://2029.globalsign.com/
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0xd6, 0x9b, 0x56, 0x11, 0x48, 0xf0, 0x1c, 0x77, 0xc5, 0x45,
                      0x78, 0xc1, 0x09, 0x26, 0xdf, 0x5b, 0x85, 0x69, 0x76, 0xad},
            PolicyOids = new string[] {"1.3.6.1.4.1.4146.1.1", ""}
            },
                // GlobalSign ECC Root CA - R4
                // https://2038r4.globalsign.com
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0x69, 0x69, 0x56, 0x2e, 0x40, 0x80, 0xf4, 0x24, 0xa1, 0xe7,
                      0x19, 0x9f, 0x14, 0xba, 0xf3, 0xee, 0x58, 0xab, 0x6a, 0xbb},
            PolicyOids = new string[] {"1.3.6.1.4.1.4146.1.1", ""}
            },
                // GlobalSign ECC Root CA - R5
                // https://2038r5.globalsign.com/
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0x1f, 0x24, 0xc6, 0x30, 0xcd, 0xa4, 0x18, 0xef, 0x20, 0x69,
                   0xff, 0xad, 0x4f, 0xdd, 0x5f, 0x46, 0x3a, 0x1b, 0x69, 0xaa},
            PolicyOids = new string[] {"1.3.6.1.4.1.4146.1.1", ""}
            },
                // Go Daddy Class 2 Certification Authority
                // https://www.godaddy.com/
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0x27, 0x96, 0xba, 0xe6, 0x3f, 0x18, 0x01, 0xe2, 0x77, 0x26,
                      0x1b, 0xa0, 0xd7, 0x77, 0x70, 0x02, 0x8f, 0x20, 0xee, 0xe4},
            PolicyOids = new string[] {"2.16.840.1.114413.1.7.23.3", ""}
            },
                // Go Daddy Root Certificate Authority - G2
                // https://valid.gdig2.catest.godaddy.com/
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0x47, 0xbe, 0xab, 0xc9, 0x22, 0xea, 0xe8, 0x0e, 0x78, 0x78,
                      0x34, 0x62, 0xa7, 0x9f, 0x45, 0xc2, 0x54, 0xfd, 0xe6, 0x8b},
            PolicyOids = new string[] {"2.16.840.1.114413.1.7.23.3", ""}
            },
                // GTE CyberTrust Global Root
                // https://www.cybertrust.ne.jp/
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0x97, 0x81, 0x79, 0x50, 0xd8, 0x1c, 0x96, 0x70, 0xcc, 0x34,
                      0xd8, 0x09, 0xcf, 0x79, 0x44, 0x31, 0x36, 0x7e, 0xf4, 0x74},
            PolicyOids = new string[] {"1.3.6.1.4.1.6334.1.100.1", ""}
            },
                // Izenpe.com - SHA256 root
                // The first OID is for businesses and the second for government entities.
                // These are the test sites, respectively:
                // https://servicios.izenpe.com
                // https://servicios1.izenpe.com
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0x2f, 0x78, 0x3d, 0x25, 0x52, 0x18, 0xa7, 0x4a, 0x65, 0x39,
                      0x71, 0xb5, 0x2c, 0xa2, 0x9c, 0x45, 0x15, 0x6f, 0xe9, 0x19},
            PolicyOids = new string[] {"1.3.6.1.4.1.14777.6.1.1", "1.3.6.1.4.1.14777.6.1.2"}
            },
                // Izenpe.com - SHA1 root
                // Windows XP finds this, SHA1, root instead. The policy OIDs are the same
                // as for the SHA256 root, above.
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0x30, 0x77, 0x9e, 0x93, 0x15, 0x02, 0x2e, 0x94, 0x85, 0x6a,
                      0x3f, 0xf8, 0xbc, 0xf8, 0x15, 0xb0, 0x82, 0xf9, 0xae, 0xfd},
            PolicyOids = new string[] {"1.3.6.1.4.1.14777.6.1.1", "1.3.6.1.4.1.14777.6.1.2"}
            },
                // Network Solutions Certificate Authority
                // https://www.networksolutions.com/website-packages/index.jsp
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0x74, 0xf8, 0xa3, 0xc3, 0xef, 0xe7, 0xb3, 0x90, 0x06, 0x4b,
                      0x83, 0x90, 0x3c, 0x21, 0x64, 0x60, 0x20, 0xe5, 0xdf, 0xce},
            PolicyOids = new string[] {"1.3.6.1.4.1.782.1.2.1.8.1", ""}
            },
                // Network Solutions Certificate Authority (reissued certificate with
                // NotBefore of Jan  1 00:00:00 2011 GMT).
                // https://www.networksolutions.com/website-packages/index.jsp
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0x71, 0x89, 0x9a, 0x67, 0xbf, 0x33, 0xaf, 0x31, 0xbe, 0xfd,
                      0xc0, 0x71, 0xf8, 0xf7, 0x33, 0xb1, 0x83, 0x85, 0x63, 0x32},
            PolicyOids = new string[] {"1.3.6.1.4.1.782.1.2.1.8.1", ""}
            },
                // QuoVadis Root CA 2
                // https://www.quovadis.bm/
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0xca, 0x3a, 0xfb, 0xcf, 0x12, 0x40, 0x36, 0x4b, 0x44, 0xb2,
                      0x16, 0x20, 0x88, 0x80, 0x48, 0x39, 0x19, 0x93, 0x7c, 0xf7},
            PolicyOids = new string[] {"1.3.6.1.4.1.8024.0.2.100.1.2", ""}
            },
                // QuoVadis Root CA 2 G3
                // https://evsslicag3-v.quovadisglobal.com/
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0x09, 0x3c, 0x61, 0xf3, 0x8b, 0x8b, 0xdc, 0x7d, 0x55, 0xdf,
                      0x75, 0x38, 0x02, 0x05, 0x00, 0xe1, 0x25, 0xf5, 0xc8, 0x36},
            PolicyOids = new string[] {"1.3.6.1.4.1.8024.0.2.100.1.2", ""}
            },
                // SecureTrust CA, SecureTrust Corporation
                // https://www.securetrust.com
                // https://www.trustwave.com/
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0x87, 0x82, 0xc6, 0xc3, 0x04, 0x35, 0x3b, 0xcf, 0xd2, 0x96,
                      0x92, 0xd2, 0x59, 0x3e, 0x7d, 0x44, 0xd9, 0x34, 0xff, 0x11},
            PolicyOids = new string[] {"2.16.840.1.114404.1.1.2.4.1", ""}
            },
                // Secure Global CA, SecureTrust Corporation
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0x3a, 0x44, 0x73, 0x5a, 0xe5, 0x81, 0x90, 0x1f, 0x24, 0x86,
                      0x61, 0x46, 0x1e, 0x3b, 0x9c, 0xc4, 0x5f, 0xf5, 0x3a, 0x1b},
            PolicyOids = new string[] {"2.16.840.1.114404.1.1.2.4.1", ""}
            },
                // Security Communication RootCA1
                // https://www.secomtrust.net/contact/form.html
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0x36, 0xb1, 0x2b, 0x49, 0xf9, 0x81, 0x9e, 0xd7, 0x4c, 0x9e,
                      0xbc, 0x38, 0x0f, 0xc6, 0x56, 0x8f, 0x5d, 0xac, 0xb2, 0xf7},
            PolicyOids = new string[] {"1.2.392.200091.100.721.1", ""}
            },
                // Security Communication EV RootCA1
                // https://www.secomtrust.net/contact/form.html
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0xfe, 0xb8, 0xc4, 0x32, 0xdc, 0xf9, 0x76, 0x9a, 0xce, 0xae,
                      0x3d, 0xd8, 0x90, 0x8f, 0xfd, 0x28, 0x86, 0x65, 0x64, 0x7d},
            PolicyOids = new string[] {"1.2.392.200091.100.721.1", ""}
            },
                // Security Communication EV RootCA2
                // https://www.secomtrust.net/contact/form.html
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0x5F, 0x3B, 0x8C, 0xF2, 0xF8, 0x10, 0xB3, 0x7D, 0x78, 0xB4,
                      0xCE, 0xEC, 0x19, 0x19, 0xC3, 0x73, 0x34, 0xB9, 0xC7, 0x74},
            PolicyOids = new string[] {"1.2.392.200091.100.721.1", ""}
            },
                // Staat der Nederlanden EV Root CA
                // https://pkioevssl-v.quovadisglobal.com/
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0x76, 0xe2, 0x7e, 0xc1, 0x4f, 0xdb, 0x82, 0xc1, 0xc0, 0xa6,
                      0x75, 0xb5, 0x05, 0xbe, 0x3d, 0x29, 0xb4, 0xed, 0xdb, 0xbb},
            PolicyOids = new string[] {"2.16.528.1.1003.1.2.7", ""}
            },
                // StartCom Certification Authority
                // https://www.startssl.com/
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0x3e, 0x2b, 0xf7, 0xf2, 0x03, 0x1b, 0x96, 0xf3, 0x8c, 0xe6,
                      0xc4, 0xd8, 0xa8, 0x5d, 0x3e, 0x2d, 0x58, 0x47, 0x6a, 0x0f},
            PolicyOids = new string[] {"1.3.6.1.4.1.23223.1.1.1", ""}
            },
                // Starfield Class 2 Certification Authority
                // https://www.starfieldtech.com/
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0xad, 0x7e, 0x1c, 0x28, 0xb0, 0x64, 0xef, 0x8f, 0x60, 0x03,
                      0x40, 0x20, 0x14, 0xc3, 0xd0, 0xe3, 0x37, 0x0e, 0xb5, 0x8a},
            PolicyOids = new string[] {"2.16.840.1.114414.1.7.23.3", ""}
            },
                // Starfield Root Certificate Authority - G2
                // https://valid.sfig2.catest.starfieldtech.com/
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0xb5, 0x1c, 0x06, 0x7c, 0xee, 0x2b, 0x0c, 0x3d, 0xf8, 0x55,
                      0xab, 0x2d, 0x92, 0xf4, 0xfe, 0x39, 0xd4, 0xe7, 0x0f, 0x0e},
            PolicyOids = new string[] {"2.16.840.1.114414.1.7.23.3", ""}
            },
                // Starfield Services Root Certificate Authority - G2
                // https://valid.sfsg2.catest.starfieldtech.com/
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0x92, 0x5a, 0x8f, 0x8d, 0x2c, 0x6d, 0x04, 0xe0, 0x66, 0x5f,
                      0x59, 0x6a, 0xff, 0x22, 0xd8, 0x63, 0xe8, 0x25, 0x6f, 0x3f},
            PolicyOids = new string[] {"2.16.840.1.114414.1.7.24.3", ""}
            },
                // SwissSign Gold CA - G2
                // https://testevg2.swisssign.net/
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0xd8, 0xc5, 0x38, 0x8a, 0xb7, 0x30, 0x1b, 0x1b, 0x6e, 0xd4,
                      0x7a, 0xe6, 0x45, 0x25, 0x3a, 0x6f, 0x9f, 0x1a, 0x27, 0x61},
            PolicyOids = new string[] {"2.16.756.1.89.1.2.1.1", ""}
            },
                // Swisscom Root EV CA 2
                // https://test-quarz-ev-ca-2.pre.swissdigicert.ch
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0xe7, 0xa1, 0x90, 0x29, 0xd3, 0xd5, 0x52, 0xdc, 0x0d, 0x0f,
                      0xc6, 0x92, 0xd3, 0xea, 0x88, 0x0d, 0x15, 0x2e, 0x1a, 0x6b},
            PolicyOids = new string[] {"2.16.756.1.83.21.0", ""}
            },
                // Thawte Premium Server CA
                // https://www.thawte.com/
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0x62, 0x7f, 0x8d, 0x78, 0x27, 0x65, 0x63, 0x99, 0xd2, 0x7d,
                      0x7f, 0x90, 0x44, 0xc9, 0xfe, 0xb3, 0xf3, 0x3e, 0xfa, 0x9a},
            PolicyOids = new string[] {"2.16.840.1.113733.1.7.48.1", ""}
            },
                // thawte Primary Root CA
                // https://www.thawte.com/
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0x91, 0xc6, 0xd6, 0xee, 0x3e, 0x8a, 0xc8, 0x63, 0x84, 0xe5,
                      0x48, 0xc2, 0x99, 0x29, 0x5c, 0x75, 0x6c, 0x81, 0x7b, 0x81},
            PolicyOids = new string[] {"2.16.840.1.113733.1.7.48.1", ""}
            },
                // thawte Primary Root CA - G2
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0xaa, 0xdb, 0xbc, 0x22, 0x23, 0x8f, 0xc4, 0x01, 0xa1, 0x27,
                      0xbb, 0x38, 0xdd, 0xf4, 0x1d, 0xdb, 0x08, 0x9e, 0xf0, 0x12},
            PolicyOids = new string[] {"2.16.840.1.113733.1.7.48.1", ""}
            },
                // thawte Primary Root CA - G3
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0xf1, 0x8b, 0x53, 0x8d, 0x1b, 0xe9, 0x03, 0xb6, 0xa6, 0xf0,
                      0x56, 0x43, 0x5b, 0x17, 0x15, 0x89, 0xca, 0xf3, 0x6b, 0xf2},
            PolicyOids = new string[] {"2.16.840.1.113733.1.7.48.1", ""}
            },
                // TWCA Global Root CA
                // https://evssldemo3.twca.com.tw/index.html
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0x9C, 0xBB, 0x48, 0x53, 0xF6, 0xA4, 0xF6, 0xD3, 0x52, 0xA4,
                      0xE8, 0x32, 0x52, 0x55, 0x60, 0x13, 0xF5, 0xAD, 0xAF, 0x65},
            PolicyOids = new string[] {"1.3.6.1.4.1.40869.1.1.22.3", ""}
            },
                // TWCA Root Certification Authority
                // https://evssldemo.twca.com.tw/index.html
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0xcf, 0x9e, 0x87, 0x6d, 0xd3, 0xeb, 0xfc, 0x42, 0x26, 0x97,
                      0xa3, 0xb5, 0xa3, 0x7a, 0xa0, 0x76, 0xa9, 0x06, 0x23, 0x48},
            PolicyOids = new string[] {"1.3.6.1.4.1.40869.1.1.22.3", ""}
            },
                // T-TeleSec GlobalRoot Class 3
                // http://www.telesec.de/ / https://root-class3.test.telesec.de/
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0x55, 0xa6, 0x72, 0x3e, 0xcb, 0xf2, 0xec, 0xcd, 0xc3, 0x23,
                      0x74, 0x70, 0x19, 0x9d, 0x2a, 0xbe, 0x11, 0xe3, 0x81, 0xd1},
            PolicyOids = new string[] {"1.3.6.1.4.1.7879.13.24.1", ""}
            },
                // USERTrust ECC Certification Authority
                // https://usertrustecccertificationauthority-ev.comodoca.com/
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0xd1, 0xcb, 0xca, 0x5d, 0xb2, 0xd5, 0x2a, 0x7f, 0x69, 0x3b,
                      0x67, 0x4d, 0xe5, 0xf0, 0x5a, 0x1d, 0x0c, 0x95, 0x7d, 0xf0},
            PolicyOids = new string[] {"1.3.6.1.4.1.6449.1.2.1.5.1", ""}
            },
                // USERTrust RSA Certification Authority
                // https://usertrustrsacertificationauthority-ev.comodoca.com/
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0x2b, 0x8f, 0x1b, 0x57, 0x33, 0x0d, 0xbb, 0xa2, 0xd0, 0x7a,
                      0x6c, 0x51, 0xf7, 0x0e, 0xe9, 0x0d, 0xda, 0xb9, 0xad, 0x8e},
            PolicyOids = new string[] {"1.3.6.1.4.1.6449.1.2.1.5.1", ""}
            },
                // UTN-USERFirst-Hardware
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0x04, 0x83, 0xed, 0x33, 0x99, 0xac, 0x36, 0x08, 0x05, 0x87,
                      0x22, 0xed, 0xbc, 0x5e, 0x46, 0x00, 0xe3, 0xbe, 0xf9, 0xd7},
            PolicyOids = new string[] {
                        "1.3.6.1.4.1.6449.1.2.1.5.1",
                        // This is the Network Solutions EV OID. However, this root
                        // cross-certifies NetSol and so we need it here too.
                        "1.3.6.1.4.1.782.1.2.1.8.1",
                    }
            },
                // ValiCert Class 2 Policy Validation Authority
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0x31, 0x7a, 0x2a, 0xd0, 0x7f, 0x2b, 0x33, 0x5e, 0xf5, 0xa1,
                      0xc3, 0x4e, 0x4b, 0x57, 0xe8, 0xb7, 0xd8, 0xf1, 0xfc, 0xa6},
            PolicyOids = new string[] {"2.16.840.1.114413.1.7.23.3", "2.16.840.1.114414.1.7.23.3"}
            },
                // VeriSign Class 3 Public Primary Certification Authority
                // https://www.verisign.com/
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0x74, 0x2c, 0x31, 0x92, 0xe6, 0x07, 0xe4, 0x24, 0xeb, 0x45,
                      0x49, 0x54, 0x2b, 0xe1, 0xbb, 0xc5, 0x3e, 0x61, 0x74, 0xe2},
            PolicyOids = new string[] {"2.16.840.1.113733.1.7.23.6", ""}
            },
                // VeriSign Class 3 Public Primary Certification Authority - G4
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0x22, 0xD5, 0xD8, 0xDF, 0x8F, 0x02, 0x31, 0xD1, 0x8D, 0xF7,
                      0x9D, 0xB7, 0xCF, 0x8A, 0x2D, 0x64, 0xC9, 0x3F, 0x6C, 0x3A},
            PolicyOids = new string[] {"2.16.840.1.113733.1.7.23.6", ""}
            },
                // VeriSign Class 3 Public Primary Certification Authority - G5
                // https://www.verisign.com/
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0x4e, 0xb6, 0xd5, 0x78, 0x49, 0x9b, 0x1c, 0xcf, 0x5f, 0x58,
                      0x1e, 0xad, 0x56, 0xbe, 0x3d, 0x9b, 0x67, 0x44, 0xa5, 0xe5},
            PolicyOids = new string[] {"2.16.840.1.113733.1.7.23.6", ""}
            },
                // VeriSign Universal Root Certification Authority
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0x36, 0x79, 0xca, 0x35, 0x66, 0x87, 0x72, 0x30, 0x4d, 0x30,
                      0xa5, 0xfb, 0x87, 0x3b, 0x0f, 0xa7, 0x7b, 0xb7, 0x0d, 0x54},
            PolicyOids = new string[] {"2.16.840.1.113733.1.7.23.6", ""}
            },
                // Wells Fargo WellsSecure Public Root Certificate Authority
                // https://nerys.wellsfargo.com/test.html
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0xe7, 0xb4, 0xf6, 0x9d, 0x61, 0xec, 0x90, 0x69, 0xdb, 0x7e,
                      0x90, 0xa7, 0x40, 0x1a, 0x3c, 0xf4, 0x7d, 0x4f, 0xe8, 0xee},
            PolicyOids = new string[] {"2.16.840.1.114171.500.9", ""}
            },
                // XRamp Global Certification Authority
            new ExtendedValidationMetadata {
            Sha1Fingerprint = new byte[] {0xb8, 0x01, 0x86, 0xd1, 0xeb, 0x9c, 0x86, 0xa5, 0x41, 0x04,
                      0xcf, 0x30, 0x54, 0xf3, 0x4c, 0x52, 0xb7, 0xe5, 0x58, 0xc6},
            PolicyOids = new string[] {"2.16.840.1.114404.1.1.2.4.1", ""}
            },
        };

        //From https://cabforum.org/object-registry/
        //Last updated: 4/20/2016 @ 17:08
        public static string[] OVOids { get; } = new[]
        {
            "2.23.140.1.2.2", //CAB Forum BR Standard
            "2.16.840.1.114412.1.1", //DigiCert
            "1.3.6.1.4.1.4788.2.200.1", //D-Trust
            "2.16.840.1.114413.1.7.23.2", //GoDaddy
            "2.16.528.1.1003.1.2.5.6", //Logius
            "1.3.6.1.4.1.8024.0.2.100.1.1", //QuoVadis
            "2.16.840.1.114414.1.7.23.2", //Starfield
            "2.16.792.3.0.3.1.1.2" //TurkTrust
        };
    }

    internal class ExtendedValidationMetadata
    {
        public byte[] Sha1Fingerprint { get; set; }
        public string[] PolicyOids { get; set; }
    }
}
