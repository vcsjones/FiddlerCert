using System;
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
        }

        public HTTPResponseHeaders headers
        {
            // We don't allow editing, and look only at Session flags
            get
            {
                return null;
            }
            set { }
        }

        public byte[] body
        {
            // We don't allow editing, and look only at Session flags
            get
            {
                return null;
            }
            set { }
        }

        // We are never dirty; we don't allow editing
        public bool bDirty => false;

        public bool bReadOnly
        {
            // We don't allow editing
            get { return true; }
            set { }
        }


        public override void AssignSession(Session oS)
        {
            _control.ClearCertificates();
            Tuple<X509Chain, X509Certificate2> cert;
            if (CertificateInspector.ServerCertificates.TryGetValue(Tuple.Create(oS.host, oS.port), out cert))
            {
                var chain = cert.Item1;
                _control.SuspendLayout();
                for (var i = 0; i < chain.ChainElements.Count; i++)
                {
                    _control.AssignCertificate(chain.ChainElements[i]);

                }
                _control.ResumeLayout(true);
            }
        }

        public override InspectorFlags GetFlags()
        {
            // We don't make sense in the AutoResponder and
            // wouldn't work anyway (no Session object)
            return InspectorFlags.HideInAutoResponder;
        }


    }
}
