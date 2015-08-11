using System;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Windows.Forms;
using VCSJones.FiddlerCert.Properties;

namespace VCSJones.FiddlerCert
{
    public partial class CertificateControl : UserControl
    {
        X509ChainElement _chainElement;
        public CertificateControl()
        {
            InitializeComponent();
        }

        public void SetCertificate(X509ChainElement chainElement)
        {
            _chainElement = chainElement;
            var certificate = chainElement.Certificate;
            var dn = DistinguishedNameParser.Parse(certificate.SubjectName.Name);
            if (dn.ContainsKey("cn"))
            {
                commonNameLabel.Text = $"Issued To: {string.Join(", ", dn["cn"])}";
                var toolTip = new ToolTip {ShowAlways = true};
                toolTip.SetToolTip(commonNameLabel, commonNameLabel.Text);
            }
            else
            {
                commonNameLabel.Text = certificate.Thumbprint;
            }
            subjectAltNameLabel.Text = certificate.Extensions[KnownOids.SubjectAltNameExtension]?.Format(false) ?? "None";
            thumbprintLabel.Text = certificate.Thumbprint;
            if (certificate.PublicKey.Oid.Value == KnownOids.EccPublicKey)
            {
                var parameterOid = OidParser.ReadFromBytes(certificate.PublicKey.EncodedParameters.RawData);
                algorithmLabel.Text = $"{certificate.PublicKey.Oid.FriendlyName} ({parameterOid.FriendlyName})";
                switch (parameterOid.Value)
                {
                    case KnownOids.EcdsaP256:
                        keySizeLabel.Text = "256-bit";
                        break;
                    case KnownOids.EcdsaP384:
                        keySizeLabel.Text = "384-bit";
                        break;
                    case KnownOids.EcdsaP521:
                        keySizeLabel.Text = "521-bit";
                        break;
                    default:
                        keySizeLabel.Text = "Unknown";
                        break;
                }
            }
            else
            {
                algorithmLabel.Text = certificate.PublicKey.Oid.FriendlyName;
                keySizeLabel.Text = $"{certificate.PublicKey.Key.KeySize}-bit";
            }
            validDatesLabel.Text = $"{certificate.NotBefore.ToString("U")} to {certificate.NotAfter.ToString("U")}";
            hashAlgorithmLabel.Text = certificate.SignatureAlgorithm.FriendlyName;
            if (chainElement.ChainElementStatus.Length == 0 || chainElement.ChainElementStatus.All(status => status.Status == X509ChainStatusFlags.NoError))
            {
                certStatusImage.Image = Resources.Security_Shields_Complete_and_ok_16xLG_color;
                certStatusToolTip.SetToolTip(certStatusImage, "Certificate is OK.");
            }
            else
            {
                certStatusImage.Image = Resources.Security_Shields_Critical_16xLG_color;
                certStatusToolTip.SetToolTip(certStatusImage, $"Errors: {string.Join(", ", chainElement.ChainElementStatus.Select(status => status.Status))}");
            }
            SetFingerprintAsyncronously(certificate);
        }

        private void SetFingerprintAsyncronously(X509Certificate2 certificate)
        {
            Task.Factory.StartNew(() => CertificateHashBuilder.BuildHashForPublicKey<SHA256Managed>(certificate)).ContinueWith(task =>
            {
                shaThumbprintLabel.Text = $@"""{task.Result}""";
            }, TaskScheduler.FromCurrentSynchronizationContext());

        }

        private void viewCertButton_Click(object sender, EventArgs e)
        {
            if (_chainElement.Certificate == null)
            {
                return;
            }
            CertificateUI.ShowCertificate(_chainElement.Certificate, ParentForm);
        }

        private void installCertButton_Click(object sender, EventArgs e)
        {
            if (_chainElement.Certificate == null)
            {
                return;
            }
            CertificateUI.ShowImportCertificate(_chainElement.Certificate, ParentForm);
        }
    }
}
