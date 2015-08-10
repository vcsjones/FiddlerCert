using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
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
            if (chainElement.ChainElementStatus.Length == 0)
            {
                certStatusImage.Image = Resources.Security_Shields_Complete_and_ok_16xLG_color;
                certStatusToolTip.SetToolTip(certStatusImage, "Certificate is OK.");
            }
            else if (chainElement.ChainElementStatus.All(status => status.Status == X509ChainStatusFlags.OfflineRevocation || status.Status == X509ChainStatusFlags.RevocationStatusUnknown))
            {
                certStatusImage.Image = Resources.Security_Shields_Alert_16xLG_color;
                certStatusToolTip.SetToolTip(certStatusImage, "Unable to check revocation status.");
            }
            else
            {
                certStatusImage.Image = Resources.Security_Shields_Critical_16xLG_color;
                certStatusToolTip.SetToolTip(certStatusImage, $"Errors: {string.Join(", ", chainElement.ChainElementStatus.Select(status => status.Status))}");
            }
        }

        private void viewCertButton_Click(object sender, EventArgs e)
        {
            if (_chainElement.Certificate == null)
            {
                return;
            }
            var structConfiguration = new CRYPTUI_VIEWCERTIFICATE_STRUCT();
            structConfiguration.dwSize = (uint)Marshal.SizeOf(typeof(CRYPTUI_VIEWCERTIFICATE_STRUCT));
            structConfiguration.pCertContext = _chainElement.Certificate.Handle;
            structConfiguration.szTitle = "Fiddler: Certificate Information";
            structConfiguration.dwFlags = ViewCertificateFlags.CRYPTUI_DISABLE_EDITPROPERTIES;
            structConfiguration.nStartPage = 0;
            structConfiguration.hwndParent = ParentForm?.Handle ?? IntPtr.Zero;
            var propertiesChanged = false;
            if (!Cryptui.CryptUIDlgViewCertificate(ref structConfiguration, ref propertiesChanged))
            {
                MessageBox.Show("An error occurred viewing the certificate.");
            }
        }

        private void installCertButton_Click(object sender, EventArgs e)
        {
            if (_chainElement.Certificate == null)
            {
                return;
            }
            var cryptuiWizImportSrcInfo = new CRYPTUI_WIZ_IMPORT_SRC_INFO();
            cryptuiWizImportSrcInfo.dwSize = (uint) Marshal.SizeOf(typeof (CRYPTUI_WIZ_IMPORT_SRC_INFO));
            cryptuiWizImportSrcInfo.dwSubjectChoice = ImportSourceSubjectChoice.CRYPTUI_WIZ_IMPORT_SUBJECT_CERT_CONTEXT;
            cryptuiWizImportSrcInfo.pCertContext = _chainElement.Certificate.Handle;
            cryptuiWizImportSrcInfo.pwszPassword = "";
            cryptuiWizImportSrcInfo.dwFlags = 0u;
            if (!Cryptui.CryptUIWizImport(ImportCertificateFlags.CRYPTUI_WIZ_IMPORT_ALLOW_CERT, ParentForm?.Handle ?? IntPtr.Zero, "Fiddler: Import Certificate", cryptuiWizImportSrcInfo, IntPtr.Zero))
            {
                MessageBox.Show("An error occurred installing the certificate.");
            }
        }
    }
}
