using System;
using System.Security.Cryptography;
using System.Windows.Forms;
using Fiddler;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms.Integration;

namespace VCSJones.FiddlerCert
{
    public class CertInspector : Inspector2, IResponseInspector2
    {
        private StackPanel _panel;

        public override void AddToTab(TabPage o)
        {
            o.Text = "Certificates";
            var host = new ElementHost();
            host.Dock = DockStyle.Fill;
            var stackPanel = new StackPanel();
            host.Child = new ScrollViewer { Content = stackPanel };
            o.Controls.Add(host);
            _panel = stackPanel;
        }

        public override int GetOrder()
        {
            return int.MaxValue;
        }

        public void Clear()
        {
            _panel.Children.Clear();
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
            Clear();
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
        }


        private void AssignCertificate(X509ChainElement chainElement)
        {
            var control = new WpfCertificateControl();
            control.DataContext = new CertificateModel
            {
                SPKISHA256Hash = new AsyncProperty<string>(Task.Factory.StartNew(() => CertificateHashBuilder.BuildHashForPublicKey<SHA256CryptoServiceProvider>(chainElement.Certificate)), "Calculating..."),
                SPKISHA1Hash = new AsyncProperty<string>(Task.Factory.StartNew(() => CertificateHashBuilder.BuildHashForPublicKey<SHA1CryptoServiceProvider>(chainElement.Certificate)), "Calculating...")
            };
            _panel.Children.Add(control);
        }

        public override InspectorFlags GetFlags()
        {
            // We don't make sense in the AutoResponder and
            // wouldn't work anyway (no Session object)
            return InspectorFlags.HideInAutoResponder;
        }


    }
}
