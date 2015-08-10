using Fiddler;

namespace VCSJones.FiddlerCert
{
    public class CertificateInspector : IAutoTamper
    {
        public void OnLoad()
        {
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
            var certificate = oSession?.oResponse?.pipeServer?.ServerCertificate;
            if (certificate != null)
            {
                var base64Certificate = System.Convert.ToBase64String(certificate.Export(System.Security.Cryptography.X509Certificates.X509ContentType.Cert));
                oSession[$"{nameof(CertificateInspector)}_Certificate"] = base64Certificate;
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
