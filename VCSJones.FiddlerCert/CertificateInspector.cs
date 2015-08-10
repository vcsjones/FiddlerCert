using System;
using System.Windows.Forms;
using Fiddler;

namespace VCSJones.FiddlerCert
{
    public class CertificateInspector : IAutoTamper
    {
        private bool _isSupportedOperatingSystem = false;

        public void OnLoad()
        {
            _isSupportedOperatingSystem = Environment.OSVersion.Version >= new Version(5, 2);
            if (!_isSupportedOperatingSystem)
            {
                MessageBox.Show("Windows Vista / Server 2003 or greater is required for the Certificate inspector extension to function.");
            }
        }

        public void OnBeforeUnload()
        {
        }

        public void AutoTamperRequestBefore(Session oSession)
        {
        }

        public void AutoTamperRequestAfter(Session oSession)
        {
        }

        public void AutoTamperResponseBefore(Session oSession)
        {
            if (!_isSupportedOperatingSystem)
            {
                return;
            }
            var certificate = oSession?.oResponse?.pipeServer?.ServerCertificate;
            if (certificate != null)
            {
                var base64Certificate = Convert.ToBase64String(certificate.Export(System.Security.Cryptography.X509Certificates.X509ContentType.Cert));
                oSession[$"{nameof(CertificateInspector)}_ServerCertificate"] = base64Certificate;
            }
        }

        public void AutoTamperResponseAfter(Session oSession)
        {
        }

        public void OnBeforeReturningError(Session oSession)
        {
        }
    }
}
