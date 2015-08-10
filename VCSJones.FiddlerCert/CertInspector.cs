using System.Windows.Forms;
using Fiddler;
using System.Security.Cryptography.X509Certificates;


namespace VCSJones.FiddlerCert
{
    public class CertInspector : Inspector2, IResponseInspector2
    {
        readonly CertificatesControl _control = new CertificatesControl
        {
            Dock = DockStyle.Fill
        };

        public override void AddToTab(TabPage o)
        {
            o.Text = "Certificates";
            o.Controls.Add(_control);
        }

        public override int GetOrder()
        {
            return int.MaxValue;
        }

        public void Clear()
        {
            _control.ClearCertificates();
            bDirty = true;
        }

        public byte[] body
        {
            get; set;
        }

        public bool bDirty { get; set; }

        public bool bReadOnly { get; set; }

        public override void AssignSession(Session oS)
        {
            _control.ClearCertificates();
            var certificate = oS[$"{nameof(CertificateInspector)}_Certificate"];
            if (certificate != null)
            {
                var x509cert = new X509Certificate2(System.Convert.FromBase64String(certificate));
                var chain = new X509Chain();
                chain.Build(x509cert); //We don't really care about the results, we just want the elements.
                _control.SuspendLayout();
                for (var i = 0; i < chain.ChainElements.Count; i++)
                {
                    _control.AssignCertificate(chain.ChainElements[i]);

                }
                _control.ResumeLayout(true);
                bDirty = true;
            }
        }

        public HTTPResponseHeaders headers { get; set; }

    }
}
