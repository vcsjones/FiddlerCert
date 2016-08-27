using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms.VisualStyles;

namespace VCSJones.FiddlerCert
{
    public class EVPolicy
    {
        public string CaName { get; }
        public byte[] RootSha1Fingerprint { get; }
        public Oid[] PolicyIds { get; }

        public EVPolicy(string caName, byte[] rootSha1Fingerprint, Oid[] policyIds)
        {
            CaName = caName;
            RootSha1Fingerprint = rootSha1Fingerprint;
            PolicyIds = policyIds;
        }
    }
    public class EVPolicies
    {

        static EVPolicies()
        {
            Policies = new []
            {
                new EVPolicy(
                    "AC Camerfirma S.A. Chambers of Commerce Root - 2008",
                    new byte[] { 0x78, 0x6a, 0x74, 0xac, 0x76, 0xab, 0x14, 0x7f, 0x9c, 0x6a,
                                 0x30, 0x50, 0xba, 0x9e, 0xa8, 0x7e, 0xfe, 0x9a, 0xce, 0x3c },
                    new []{new Oid("1.3.6.1.4.1.17326.10.14.2.1.2"), new Oid("1.3.6.1.4.1.17326.10.14.2.2.2")}), 
            };
        }
        public static EVPolicy[] Policies { get; }
    }
}
