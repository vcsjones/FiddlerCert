using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Windows.Forms;
using Fiddler;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Windows.Media;
using System.Windows.Threading;

namespace VCSJones.FiddlerCert
{
    public class CertInspector : Inspector2, IResponseInspector2
    {
        private readonly Grid _panel;
        private readonly X509Store _rootStore = new X509Store(StoreName.Root, StoreLocation.CurrentUser);
        private readonly X509Store _userStore = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
        private readonly ElementHost _host;

        public CertInspector()
        {
            _rootStore.Open(OpenFlags.ReadOnly);
            _userStore.Open(OpenFlags.ReadOnly);
            _host = new ElementHost {Dock = DockStyle.Fill};
            _panel = new Grid();
            _host.Child = _panel;
            FiddlerApplication.Prefs.AddWatcher("fiddler.ui.font", FontChanged);
        }

        private void FontChanged(object sender, PrefChangeEventArgs e)
        {
            if (e.PrefName == "fiddler.ui.font.face")
            {
                var font = e.ValueString;
                var fontFamily = new FontFamily(font);
                _panel.Dispatcher.BeginInvoke((Action)(() =>
                {
                    var style = new Style(typeof(StackPanel), _panel.Style);
                    style.Setters.Add(new Setter(TextBlock.FontFamilyProperty, fontFamily));
                    style.Seal();
                    _panel.Style = style;
                }));
            }
            if (e.PrefName == "fiddler.ui.font.size")
            {
                double value;
                if (double.TryParse(e.ValueString, out value))
                {
                    _panel.Dispatcher.BeginInvoke((Action) (() =>
                    {
                        var fontSizeInPoints = value*96d/72d;
                        var style = new Style(typeof (StackPanel), _panel.Style);
                        style.Setters.Add(new Setter(TextBlock.FontSizeProperty, fontSizeInPoints));
                        style.Seal();
                        _panel.Style = style;
                    }));
                }
            }
        }

        public override void AddToTab(TabPage o)
        {
            o.Text = "Certificates";
            o.Controls.Add(_host);
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
                if (CertificateInspector.ServerCertificates.TryGetValue(Tuple.Create(oS.hostname, oS.port), out cert))
                {
                    var pkp = oS.ResponseHeaders.Exists("public-key-pins") ? oS.ResponseHeaders["public-key-pins"] : null;
                    var pkpReportOnly = oS.ResponseHeaders.Exists("public-key-pins-report-only") ? oS.ResponseHeaders["public-key-pins-report-only"] : null;
                    var pinnedKeys = pkp == null && pkpReportOnly == null ? null : PublicKeyPinsParser.Parse(pkp ?? pkpReportOnly);
                    var reportOnly = pkpReportOnly != null;
                    var chain = cert.Item1;

                    var control = new WpfCertificateControl();
                    control.DataContext = new HttpSecurityModel
                    {
                        IsNotTunnel = (oS.BitFlags & SessionFlags.IsDecryptingTunnel) != SessionFlags.IsDecryptingTunnel,
                        CertificateChain = new AsyncProperty<ObservableCollection<CertificateModel>>(Task.Factory.StartNew(() =>
                        {
                            var chainItems = chain.ChainElements.Cast<X509ChainElement>().Select((t, i) => AssignCertificate(t, reportOnly, pinnedKeys, chain, i)).ToList();
                            return new ObservableCollection<CertificateModel>(chainItems);
                        })),
                        Hpkp = new HpkpModel
                        {
                            HasHpkpHeaders = pinnedKeys != null,
                            RawHpkpHeader = pkp ?? pkpReportOnly,
                            PinDirectives = 
                                pinnedKeys == null ? null
                                : new ObservableCollection<HpkpHashModel>(pinnedKeys.PinnedKeys.Select(pk => new HpkpHashModel {Algorithm = pk.Algorithm, HashBase64 = pk.FingerprintBase64}).ToArray())
                        }

                    };
                    _panel.Children.Add(control);
                }
            }
            else
            {
                _panel.Children.Add(new System.Windows.Controls.Label {Content = "Certificates are for HTTPS connections only."});
            }
        }


        private CertificateModel AssignCertificate(X509ChainElement chainElement, bool reportOnly, PublicPinnedKeys pinnedKey, X509Chain chain, int index)
        {
            var certificate = chainElement.Certificate;
            var algorithmBits = BitStrengthCalculator.CalculateStrength(certificate);
            var dn = DistinguishedNameParser.Parse(certificate.Subject);
            return new CertificateModel
            {
                CommonName = dn.ContainsKey("cn") ? dn["cn"].FirstOrDefault() ?? certificate.Thumbprint : certificate.Thumbprint,
                Thumbprint = certificate.Thumbprint,
                DistinguishedName = dn,
                SubjectAlternativeName = certificate.Extensions[KnownOids.X509Extensions.SubjectAltNameExtension]?.Format(false) ?? "None",
                PublicKey = new PublicKeyModel
                {
                    Algorithm = algorithmBits.AlgorithmName,
                    KeySizeBits = algorithmBits.BitSize,
                    PublicKey = certificate.PublicKey.EncodedKeyValue.RawData
                },
                BeginDate = certificate.NotBefore,
                EndDate = certificate.NotAfter,
                SerialNumber = certificate.SerialNumber ?? "None",
                SignatureAlgorithm = new SignatureAlgorithmModel
                {
                    SignatureAlgorithm = certificate.SignatureAlgorithm,
                    IsTrustedRoot = _rootStore.Certificates.Contains(certificate) || _userStore.Certificates.Contains(certificate)
                },
                CertificateType = index == 0 ? GetCertificateType(certificate, chain) : CertificateType.None,
                Errors = new AsyncProperty<CertificateErrors>(Task.Factory.StartNew(() => CertificateErrorsCalculator.GetCertificateErrors(chainElement))),
                SpkiHashes = new AsyncProperty<SpkiHashesModel>(Task.Factory.StartNew(() => CalculateHashes(chainElement.Certificate, reportOnly, pinnedKey))),
                InstallCommand = new RelayCommand(parameter => CertificateUI.ShowImportCertificate(chainElement.Certificate, FiddlerApplication.UI)),
                ViewCommand = new RelayCommand(parameter => CertificateUI.ShowCertificate(chainElement.Certificate, FiddlerApplication.UI))
            };
        }

        private static SpkiHashesModel CalculateHashes(X509Certificate2 certificate, bool reportOnly, PublicPinnedKeys pinnedKeys)
        {
            var sha256 = CertificateHashBuilder.BuildHashForPublicKey<SHA256CryptoServiceProvider>(certificate);
            var model = new SpkiHashesModel
            {
                Hashes = new ObservableCollection<SpkiHashModel>
                {
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

        private static CertificateType GetCertificateType(X509Certificate2 certificate, X509Chain chain)
        {
            if (CertificateValidatedChecker.IsCertificateExtendedValidation(certificate, chain))
            {
                return CertificateType.EV;
            }
            if (CertificateValidatedChecker.IsCertificateOrganizationValidated(certificate))
            {
                return CertificateType.OV;
            }
            return CertificateType.DV;
        }


        public override InspectorFlags GetFlags()
        {
            // We don't make sense in the AutoResponder and
            // wouldn't work anyway (no Session object)
            return InspectorFlags.HideInAutoResponder;
        }


    }
}
