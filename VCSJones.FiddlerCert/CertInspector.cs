using System;
using System.Windows.Forms;
using Fiddler;
using System.Security.Cryptography.X509Certificates;

namespace VCSJones.FiddlerCert
{
    public class CertInspector : Inspector2, IResponseInspector2
    {
        private ScrollableControl _control;

        public override void AddToTab(TabPage o)
        {
            o.Text = "Certificates";
            o.AutoScroll = true;
            _control = o;
        }

        public override int GetOrder()
        {
            return int.MaxValue;
        }

        public void Clear()
        {
            _control.Controls.Clear();
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
            _control.SuspendLayout();
            _control.Controls.Clear();
            if (oS.isHTTPS || (oS.BitFlags & SessionFlags.IsDecryptingTunnel) == SessionFlags.IsDecryptingTunnel)
            {
                Tuple<X509Chain, X509Certificate2> cert;
                if (CertificateInspector.ServerCertificates.TryGetValue(Tuple.Create(oS.host, oS.port), out cert))
                {
                    var chain = cert.Item1;
                    for (var i = 0; i < chain.ChainElements.Count; i++)
                    {
                        AssignCertificate(chain.ChainElements[i]);

                    }
                }
            }
            _control.ResumeLayout(false);
        }


        private void AssignCertificate(X509ChainElement chainElement)
        {
            const int CERT_HEIGHT = 200;
            const int CERT_PADDING = 5;
            var numberOfCertificates = _control.Controls.Count;
            var currentOffset = (numberOfCertificates * CERT_HEIGHT) + (CERT_PADDING * numberOfCertificates) + CERT_PADDING;
            var newCertificate = new CertificateControl(chainElement);
            newCertificate.Top = currentOffset;
            newCertificate.Height = CERT_HEIGHT;
            newCertificate.Width = _control.Width;
            newCertificate.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left;
            _control.Controls.Add(newCertificate);
        }

        public override InspectorFlags GetFlags()
        {
            // We don't make sense in the AutoResponder and
            // wouldn't work anyway (no Session object)
            return InspectorFlags.HideInAutoResponder;
        }


    }
}
