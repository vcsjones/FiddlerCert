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
using System.Diagnostics;
using System.Windows.Interop;

namespace VCSJones.FiddlerCert
{
    public class CertInspector : Inspector2, IResponseInspector2
    {
        private readonly Grid _panel;
        private readonly X509Store _rootStore = new X509Store(StoreName.Root, StoreLocation.CurrentUser);
        private readonly X509Store _userStore = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
        private readonly ElementHost _host;

        private readonly AskUpdateBarModel _askUpdateBarModel;

        public CertInspector()
        {
            _askUpdateBarModel = new AskUpdateBarModel();
            _rootStore.Open(OpenFlags.ReadOnly);
            _userStore.Open(OpenFlags.ReadOnly);
            _host = new ElementHost { Dock = DockStyle.Fill };
            _panel = new Grid();
            _host.Child = _panel;
            FiddlerApplication.Prefs.AddWatcher("fiddler.ui.font", FontChanged);
            FiddlerApplication.Prefs.AddWatcher(PreferenceNames.ASK_CHECK_FOR_UPDATES_PREF, AskUpdateChanged);
        }

        private void AskUpdateChanged(object sender, PrefChangeEventArgs e)
        {
            if (e.PrefName == PreferenceNames.ASK_CHECK_FOR_UPDATES_PREF)
            {
                _askUpdateBarModel.AskRequired = !e.ValueBool;
            }
        }

        private void FontChanged(object sender, PrefChangeEventArgs e)
        {
            if (e.PrefName == "fiddler.ui.font.face")
            {
                var font = e.ValueString;
                var fontFamily = new FontFamily(font);
                _panel.Dispatcher.BeginInvoke((Action)(() =>
                {
                    var style = new Style(_panel.GetType(), _panel.Style);
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
                    _panel.Dispatcher.BeginInvoke((Action)(() =>
                   {
                       var fontSizeInPoints = value * 96d / 72d;
                       var style = new Style(_panel.GetType(), _panel.Style);
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

        public override int GetOrder() => int.MaxValue;

        public void Clear() => _panel.Children.Clear();

        public HTTPResponseHeaders headers
        {
            // We don't allow editing, and look only at Session flags
            get => null;
            set { }
        }

        public byte[] body
        {
            // We don't allow editing, and look only at Session flags
            get => null;
            set { }
        }

        // We are never dirty; we don't allow editing
        public bool bDirty => false;

        public bool bReadOnly
        {
            // We don't allow editing
            get => true;
            set { }
        }


        public override void AssignSession(Session oS)
        {
            Clear();
            if (!CertificateInspector.HttpsDecryptionEnabledOnStartup || !CONFIG.bCaptureCONNECT || !CONFIG.bMITM_HTTPS)
            {
                _panel.Children.Add(new System.Windows.Controls.Label {
                    Content = "Fiddler Cert Inspector requires enabling decryption of HTTPS traffic, and restarting Fiddler."
                });
                return;
            }
            if (!CertificateInspector.IsSupportedOperatingSystem)
            {
                _panel.Children.Add(new System.Windows.Controls.Label
                {
                    Content = "Fiddler Cert Inspector requires Windows Vista or Windows Server 2008 or later."
                });
                return;
            }
            var control = new WpfCertificateControl();
            var masterModel = new CertInspectorModel();
            masterModel.UpdateBarModel = new UpdateBarModel(CertificateInspector.LatestVersion?.Item1, CertificateInspector.LatestVersion?.Item2);
            masterModel.AskUpdateBarModel = _askUpdateBarModel;
            masterModel.AskUpdateBarModel.AskRequired = !FiddlerApplication.Prefs.GetBoolPref(PreferenceNames.ASK_CHECK_FOR_UPDATES_PREF, false);
            masterModel.SettingsCommand = new RelayCommand(_ =>
            {
                var window = new SettingsWindow();
                var helper = new WindowInteropHelper(window);
                helper.Owner = FiddlerApplication.UI.Handle;
                window.ShowDialog();
            });
            masterModel.HttpSecurityModel = new HttpSecurityModel
            {
                IsNotTunnel = (oS.BitFlags & SessionFlags.IsDecryptingTunnel) != SessionFlags.IsDecryptingTunnel,
                ContentChain = new AsyncProperty<ObservableCollection<CertificateModel>>(Task.Factory.StartNew(() =>
                {
                    if (!oS.bHasResponse)
                    {
                        return null;
                    }
                    var contentChain = ChainForContent(oS.ResponseBody);
                    if (contentChain == null)
                    {
                        return null;
                    }
                    var chainItems = contentChain.ChainElements.Cast<X509ChainElement>().Select((t, i) => AssignCertificate(t, false, null, contentChain, i)).ToList();
                    return new ObservableCollection<CertificateModel>(chainItems);
                }))
            };
            if (oS.isHTTPS || (oS.BitFlags & SessionFlags.IsDecryptingTunnel) == SessionFlags.IsDecryptingTunnel)
            {
                if (CertificateInspector.ServerCertificates.TryGetValue(Tuple.Create(oS.hostname, oS.port), out var cert))
                {
                    var pkp = oS.ResponseHeaders.Exists("public-key-pins") ? oS.ResponseHeaders["public-key-pins"] : null;
                    var pkpReportOnly = oS.ResponseHeaders.Exists("public-key-pins-report-only") ? oS.ResponseHeaders["public-key-pins-report-only"] : null;
                    var pinnedKeys = pkp == null && pkpReportOnly == null ? null : PublicKeyPinsParser.Parse(pkp ?? pkpReportOnly);
                    var reportOnly = pkpReportOnly != null;
                    var chain = cert.Item1;
                    masterModel.HttpSecurityModel.CertificateChain = new AsyncProperty<ObservableCollection<CertificateModel>>(Task.Factory.StartNew(() =>
                    {
                        var chainItems = chain.ChainElements.Cast<X509ChainElement>().Select((t, i) => AssignCertificate(t, reportOnly, pinnedKeys, chain, i)).ToList();
                        return new ObservableCollection<CertificateModel>(chainItems);
                    }));
                    masterModel.HttpSecurityModel.Hpkp = new HpkpModel
                    {
                        HasHpkpHeaders = pinnedKeys != null,
                        RawHpkpHeader = pkp ?? pkpReportOnly,
                        PinDirectives =
                            pinnedKeys == null ? null
                            : new ObservableCollection<HpkpHashModel>(pinnedKeys.PinnedKeys.Select(pk => new HpkpHashModel { Algorithm = pk.Algorithm, HashBase64 = pk.FingerprintBase64 }).ToArray())
                    };
                }
            }
            control.DataContext = masterModel;
            _panel.Children.Add(control);
                
        }

        private static X509Chain ChainForContent(byte[] content)
        {
            if (content == null || content.Length < 2)
            {
                return null;
            }
            try
            {
                var cert = new X509Certificate2(content);
                var chain = new X509Chain();
                //Revocation is checked async when the view is build.
                //Don't do it here.
                chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
                chain.Build(cert);
                return chain;
            }
            catch (Exception)
            {
                return null;
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
                Thumbprint = CertificateHashBuilder.BuildHashForCertificateHex<SHA256Cng>(certificate),
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
                CertificateCtModel = new AsyncProperty<CertificateCtModel>(Task.Factory.StartNew(() => GetCtModel(certificate))),
                Errors = new AsyncProperty<CertificateErrors>(Task.Factory.StartNew(() => CertificateErrorsCalculator.GetCertificateErrors(chainElement))),
                SpkiHashes = new AsyncProperty<SpkiHashesModel>(Task.Factory.StartNew(() => CalculateHashes(chainElement.Certificate, reportOnly, pinnedKey))),
                InstallCommand = new RelayCommand(parameter => CertificateUI.ShowImportCertificate(chainElement.Certificate, FiddlerApplication.UI)),
                ViewCommand = new RelayCommand(parameter => CertificateUI.ShowCertificate(chainElement.Certificate, FiddlerApplication.UI)),
                BrowseCommand = new RelayCommand(parameter =>
                {
                    var uri = parameter as Uri;
                    if (uri?.Scheme == Uri.UriSchemeHttps)
                    {
                        Process.Start(uri.AbsoluteUri);
                    }
                })
            };
        }



        private static CertificateCtModel GetCtModel(X509Certificate2 certificate)
        {
            var model = new CertificateCtModel();
            var extension = certificate.Extensions[KnownOids.X509Extensions.CertificateTimeStampListCT];
            if (extension != null)
            {
                var scts = SctDecoder.DecodeData(extension);
                foreach (var sct in scts)
                {
                    var log = CtLogs.FindByLogId(sct.LogId);
                    model.Signatures.Add(
                        new SctSignatureModel
                        {
                            HashAlgorithm = sct.HashAlgorithm,
                            SignatureAlgorithm = sct.SignatureAlgorithm,
                            LogIdHex = BitConverter.ToString(sct.LogId).Replace("-", ""),
                            Timestamp = sct.Timestamp,
                            LogName = log?.Name ?? "Unknown",
                            LogUrl = log?.Url ?? "Unknown",
                            Index = sct.Index,
                            SignatureHex = BitConverter.ToString(sct.Signature).Replace("-", ""),
                            RevocationEffective = CtLogs.RevokedCtLogs.FirstOrDefault(l => l.LogId.MemoryCompare(sct.LogId))?.RevocationEffective
                        }
                    );
                }
            }
            return model;
        }

        private static SpkiHashesModel CalculateHashes(X509Certificate2 certificate, bool reportOnly, PublicPinnedKeys pinnedKeys)
        {
            var sha256 = CertificateHashBuilder.BuildHashForPublicKeyBinary<SHA256CryptoServiceProvider>(certificate);
            var model = new SpkiHashesModel
            {
                Hashes = new ObservableCollection<SpkiHashModel>
                {
                    new SpkiHashModel
                    {
                        ReportOnly = reportOnly,
                        Algorithm = PinAlgorithm.SHA256,
                        Hash = sha256,
                        IsPinned = pinnedKeys?.PinnedKeys?.Any(pk => pk.Fingerprint.SequenceEqual(sha256)) ?? false
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
            if (CertificateValidatedChecker.IsCertificateIndividualValidated(certificate))
            {
                return CertificateType.IV;
            }
            if (CertificateValidatedChecker.IsCertificateOrganizationValidated(certificate))
            {
                return CertificateType.OV;
            }
            if (CertificateValidatedChecker.IsCertificateDomainValidated(certificate))
            {
                return CertificateType.DV;
            }
            return CertificateType.None;
        }


        public override InspectorFlags GetFlags()
        {
            // We don't make sense in the AutoResponder and
            // wouldn't work anyway (no Session object)
            return InspectorFlags.HideInAutoResponder;
        }


    }
}
