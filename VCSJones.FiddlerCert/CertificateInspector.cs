using System;
using Fiddler;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Net;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Threading;

namespace VCSJones.FiddlerCert
{
    public class CertificateInspector : IFiddlerExtension
    {
        internal static bool IsSupportedOperatingSystem { get; private set; } = false;
        internal static readonly Dictionary<Tuple<string, int>, Tuple<X509Chain, X509Certificate2>> ServerCertificates = new Dictionary<Tuple<string, int>, Tuple<X509Chain, X509Certificate2>>();
        internal static Tuple<Version, string> LatestVersion { get; set; }
        private System.Threading.Timer _timer;
        private const string UPDATE_URI = "https://api.github.com/repos/vcsjones/FiddlerCert/releases/latest";
        private const string INSTALLER_FILE_NAME = "FiddlerCertInspector.exe";
        private readonly object _updateLockCheck = new object();
        public static bool HttpsDecryptionEnabledOnStartup { get; private set; } = false;

        private static readonly TimeSpan _normalUpdateInterval = TimeSpan.FromDays(1);
        private static readonly TimeSpan _failedUpdateInterval = TimeSpan.FromMinutes(5);

        private void CertificateValidationHandler(object sender, ValidateServerCertificateEventArgs e)
        {
            lock (ServerCertificates)
            {
                var key = Tuple.Create(e.Session.hostname, e.Session.port);
                if (!ServerCertificates.ContainsKey(key))
                {
                    ServerCertificates.Add(key, Tuple.Create(new X509Chain(e.ServerCertificateChain.ChainContext), new X509Certificate2(e.ServerCertificate)));
                }
            }
        }

        public void OnLoad()
        {
            IsSupportedOperatingSystem = Environment.OSVersion.Version >= new Version(6, 0);
            HttpsDecryptionEnabledOnStartup = CONFIG.bCaptureCONNECT && CONFIG.bMITM_HTTPS;
            if (!IsSupportedOperatingSystem)
            {
                FiddlerApplication.Log.LogString("Fiddler Cert Inspector not supported on this operating system.");
                return;
            }

            else if (HttpsDecryptionEnabledOnStartup)
            {
                FiddlerApplication.OnValidateServerCertificate += CertificateValidationHandler;
            }
            _timer = new Timer(TimerCallback, null, TimeSpan.Zero, TimeSpan.FromDays(1));
            //If the bar changes the preference to "yes", trigger a callback
            //otherwise, null out the latest version info so the bar and settings window don't know
            //there a new version anymore.
            FiddlerApplication.Prefs.AddWatcher(PreferenceNames.CHECK_FOR_UPDATED_PREF, (s, e) =>
            {
                if (e.PrefName == PreferenceNames.CHECK_FOR_UPDATED_PREF && e.ValueBool)
                {
                    FiddlerApplication.Log.LogString("CertInspector update check enabled.");
                    TimerCallback(null);
                }
                if (e.PrefName == PreferenceNames.CHECK_FOR_UPDATED_PREF && !e.ValueBool)
                {
                    FiddlerApplication.Log.LogString("CertInspector update check disabled.");
                    LatestVersion = null;
                }
            });
        }

        private void TimerCallback(object arg)
        {
            var lockTaken = false;
            try
            {
                Monitor.TryEnter(_updateLockCheck, ref lockTaken);
                if (!lockTaken)
                {
                    FiddlerApplication.Log.LogString("CertInspector Update check is already in progress.");
                    return;
                }
                if (!FiddlerApplication.Prefs.GetBoolPref(PreferenceNames.CHECK_FOR_UPDATED_PREF, false))
                {
                    return;
                }
                byte[] latestJson;
                try
                {
                    FiddlerApplication.Log.LogString("CertInspector Checking for updates to FiddlerCert Inspector.");
                    var client = new WebClient();
                    client.Headers.Add("Accept", "application/vnd.github.v3+json");
                    client.Headers.Add("User-Agent", "vcsjones/FiddlerCert");
                    latestJson = client.DownloadData(UPDATE_URI);
                    var serializer = new DataContractJsonSerializer(typeof(Release));
                    using (var ms = new MemoryStream(latestJson, false))
                    {
                        var release = (Release)serializer.ReadObject(ms);
                        FiddlerApplication.Log.LogString($"CertInspector Latest version detected: {release.Name}");
                        var version = new Version(release.Name.Substring(1));
                        var downloadUrl = release.HtmlUrl;
                        if (downloadUrl != null)
                        {
                            LatestVersion = Tuple.Create(version, downloadUrl);
                        }
                    }
                    _timer.Change(_normalUpdateInterval, _normalUpdateInterval);

                }
                catch (Exception e)
                {
                    FiddlerApplication.Log.LogFormat("Failed to check for updates to FiddlerCert Inspector: {0}", e.Message);
                    FiddlerApplication.Log.LogString("FiddlerCert Inspector will try to check for updates in 5 minutes.");
                    try
                    {
                        _timer.Change(_failedUpdateInterval, _failedUpdateInterval);
                    }
                    catch (ObjectDisposedException)
                    {
                        //Fiddler is trying to unload. Don't blow up.
                    }
                }
            }
            finally
            {
                if (lockTaken)
                {
                    Monitor.Exit(_updateLockCheck);
                }
            }
        }

        public void OnBeforeUnload()
        {
            _timer.Dispose();
            FiddlerApplication.OnValidateServerCertificate -= CertificateValidationHandler;
        }
    }
}

