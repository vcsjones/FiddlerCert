using System;
using System.Windows.Forms;
using Fiddler;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace VCSJones.FiddlerCert
{
    public class CertificateInspector : IFiddlerExtension
    {
        private bool _isSupportedOperatingSystem = false;
        internal static readonly Dictionary<string, X509Certificate2> ServerCertificates = new Dictionary<string, X509Certificate2>();

        private void CertificateValidationHandler(object sender, ValidateServerCertificateEventArgs e)
        {
            lock (ServerCertificates)
            {
                if (!ServerCertificates.ContainsKey(e.Session.host))
                {
                    ServerCertificates.Add(e.Session.host, new X509Certificate2(e.ServerCertificate));
                }
            }
        }

        public void OnLoad()
        {
            _isSupportedOperatingSystem = Environment.OSVersion.Version >= new Version(5, 2);
            if (!_isSupportedOperatingSystem)
            {
                MessageBox.Show("Windows Vista / Server 2003 or greater is required for the Certificate inspector extension to function.");
            }
            else
            {
                FiddlerApplication.OnValidateServerCertificate += CertificateValidationHandler;
            }
        }

        public void OnBeforeUnload()
        {
        }
    }

}

