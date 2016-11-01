using System;
using System.Windows.Forms;
using Fiddler;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Net;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Threading;
using VCSJones.FiddlerCert.Services;

namespace VCSJones.FiddlerCert
{
    public class CertificateInspector : IFiddlerExtension
    {
        internal static bool IsSupportedOperatingSystem { get; private set; } = false;
        internal static readonly Dictionary<Tuple<string, int>, Tuple<X509Chain, X509Certificate2>> ServerCertificates = new Dictionary<Tuple<string, int>, Tuple<X509Chain, X509Certificate2>>();
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

        public CertificateInspector()
        {
            Container.Instance.Register<UpdateStatus>();
            Container.Instance.Register<IUpdateWorker, UpdateWorker>();
            Container.Instance.Register<IFiddlerLoggerService, FiddlerLoggerService>();
            Container.Instance.Register<IFiddlerPreferencesService, FiddlerPreferencesService>();
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
            var updateWorker = Container.Instance.Resolve<IUpdateWorker>();
            updateWorker.OnError += e => updateWorker.Adjust(_failedUpdateInterval);
            updateWorker.OnSuccess += () => updateWorker.Adjust(_normalUpdateInterval);
            updateWorker.Start(_normalUpdateInterval);
            FiddlerApplication.Prefs.AddWatcher(PreferenceNames.CHECK_FOR_UPDATED_PREF, (s, e) =>
            {
                if (e.PrefName == PreferenceNames.CHECK_FOR_UPDATED_PREF && e.ValueBool)
                {
                    FiddlerApplication.Log.LogString("CertInspector update check enabled.");
                    updateWorker.Fire();
                }
                if (e.PrefName == PreferenceNames.CHECK_FOR_UPDATED_PREF && !e.ValueBool)
                {
                    FiddlerApplication.Log.LogString("CertInspector update check disabled.");
                    var status = Container.Instance.Resolve<UpdateStatus>();
                    status.DownloadLocation = null;
                    status.LatestVersion = null;
                }
            });
        }

     
        public void OnBeforeUnload()
        {
            FiddlerApplication.OnValidateServerCertificate -= CertificateValidationHandler;
        }
    }
}

