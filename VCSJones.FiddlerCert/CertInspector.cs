using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Windows.Forms;
using Fiddler;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms.Integration;

namespace VCSJones.FiddlerCert
{
    public class CertInspector : Inspector2, IResponseInspector2
    {
        private StackPanel _panel;
        private readonly X509Store _rootStore = new X509Store(StoreName.Root, StoreLocation.CurrentUser);
        private readonly X509Store _userStore = new X509Store(StoreName.Root, StoreLocation.LocalMachine);

        public CertInspector()
        {
            _rootStore.Open(OpenFlags.ReadOnly);
            _userStore.Open(OpenFlags.ReadOnly);
        }

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
                    var control = new WpfCertificateControl();
                    var chainItems = (from X509ChainElement t in chain.ChainElements select AssignCertificate(t, oS)).ToList();
                    control.DataContext = new HttpSecurityModel
                    {
                        CertificateChain = new ObservableCollection<CertificateModel>(chainItems)
                    };
                    _panel.Children.Add(control);
                }
            }
            else
            {
                _panel.Children.Add(new System.Windows.Controls.Label {Content = "Certificates are for HTTPS connections only."});
            }
        }


        private CertificateModel AssignCertificate(X509ChainElement chainElement, Session oS)
        {
            var certificate = chainElement.Certificate;
            
            var algorithmBits = BitStrengthCalculator.CalculateStrength(certificate);
            var dn = DistinguishedNameParser.Parse(certificate.Subject);
            return new CertificateModel
            {
                CommonName = dn.ContainsKey("cn") ? dn["cn"].FirstOrDefault() ?? certificate.Thumbprint : certificate.Thumbprint,
                Thumbprint = certificate.Thumbprint,
                SubjectAlternativeName = certificate.Extensions[KnownOids.X509Extensions.SubjectAltNameExtension]?.Format(false) ?? "None",
                PublicKey = new PublicKeyModel
                {
                    Algorithm = algorithmBits.AlgorithmName,
                    KeySizeBits = algorithmBits.BitSize,
                    PublicKey = certificate.PublicKey.EncodedKeyValue.RawData
                },
                BeginDate = certificate.NotBefore,
                EndDate = certificate.NotAfter,
                SignatureAlgorithm = new SignatureAlgorithmModel
                {
                    SignatureAlgorithm = certificate.SignatureAlgorithm,
                    IsTrustedRoot = _rootStore.Certificates.Contains(certificate) || _userStore.Certificates.Contains(certificate)
                },
                Errors = new AsyncProperty<CertificateErrors>(Task.Factory.StartNew(() => CertificateErrorsCalculator.GetCertificateErrors(chainElement))),
                SpkiHashes = new AsyncProperty<SpkiHashesModel>(Task.Factory.StartNew(() => CalculateHashes(chainElement.Certificate, oS))),
                InstallCommand = new RelayCommand(parameter => CertificateUI.ShowImportCertificate(chainElement.Certificate)),
                ViewCommand = new RelayCommand(parameter => CertificateUI.ShowCertificate(chainElement.Certificate))
            };
        }

        private static SpkiHashesModel CalculateHashes(X509Certificate2 certificate, Session session)
        {
            var reportOnly = false;
            var pinnedKeys = session.ResponseHeaders.Exists("public-key-pins") ? PublicKeyPinsParser.Parse(session.ResponseHeaders["public-key-pins"]) : null;
            if (pinnedKeys == null)
            {
                pinnedKeys = session.ResponseHeaders.Exists("public-key-pins-report-only") ? PublicKeyPinsParser.Parse(session.ResponseHeaders["public-key-pins-report-only"]) : null;
                if (pinnedKeys != null)
                {
                    reportOnly = true;
                }
            }

            var sha256 = CertificateHashBuilder.BuildHashForPublicKey<SHA256CryptoServiceProvider>(certificate);
            var sha1 = CertificateHashBuilder.BuildHashForPublicKey<SHA1CryptoServiceProvider>(certificate);
            var model = new SpkiHashesModel
            {
                Hashes = new ObservableCollection<SpkiHashModel>
                {
                    new SpkiHashModel
                    {
                        ReportOnly = reportOnly,
                        Algorithm = PinAlgorithm.SHA1,
                        HashBase64 = sha1,
                        IsPinned = pinnedKeys?.PinnedKeys?.Any(pk => pk.FingerprintBase64 == sha1) ?? false
                    },
                    new SpkiHashModel
                    {
                        ReportOnly = reportOnly,
                        Algorithm = PinAlgorithm.SHA256,
                        HashBase64 = sha256,
                        IsPinned = pinnedKeys?.PinnedKeys?.Any(pk => pk.FingerprintBase64 == sha256) ?? false
                    },
                }
            };
            return model;
        }


        public override InspectorFlags GetFlags()
        {
            // We don't make sense in the AutoResponder and
            // wouldn't work anyway (no Session object)
            return InspectorFlags.HideInAutoResponder;
        }


    }
}
