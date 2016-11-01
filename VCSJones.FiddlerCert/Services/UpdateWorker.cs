using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Threading;

namespace VCSJones.FiddlerCert.Services
{
    public interface IUpdateWorker
    {
        void Start(TimeSpan interval);
        event Action<Exception> OnError;
        event Action OnSuccess;
        void Adjust(TimeSpan interval);
        void Stop();
        void Fire();
    }

    public class UpdateWorker : IUpdateWorker
    {
        private const string UPDATE_URI = "https://api.github.com/repos/vcsjones/FiddlerCert/releases/latest";


        private readonly Timer _timer;
        private readonly UpdateStatus _updateStatus;
        private readonly IFiddlerPreferencesService _preferences;
        private readonly IFiddlerLoggerService _logger;
        private readonly object _reentryLock = new object();

        public UpdateWorker(UpdateStatus updateStatus, IFiddlerPreferencesService preferences, IFiddlerLoggerService logger)
        {
            _updateStatus = updateStatus;
            _preferences = preferences;
            _logger = logger;
            _timer = new Timer(Callback);
        }

        public event Action<Exception> OnError;
        public event Action OnSuccess;

        public void Adjust(TimeSpan interval) => _timer.Change(interval, interval);

        public void Start(TimeSpan interval) => _timer.Change(TimeSpan.Zero, interval);

        public void Stop() => _timer.Change(Timeout.Infinite, Timeout.Infinite);

        public void Fire() => Callback(null);

        private void Callback(object obj)
        {
            bool lockTaken = false;
            try
            {
                Monitor.TryEnter(_reentryLock, ref lockTaken);
                if (!lockTaken)
                {
                    _logger.Log("CertInspector Update check is already in progress.");
                    return;
                }
                if (!_preferences.GetPref(PreferenceNames.CHECK_FOR_UPDATED_PREF, false))
                {

                }
                byte[] latestJson;
                try
                {
                    _logger.Log("CertInspector Checking for updates to FiddlerCert Inspector.");
                    var client = new WebClient();
                    client.Headers.Add("Accept", "application/vnd.github.v3+json");
                    client.Headers.Add("User-Agent", "vcsjones/FiddlerCert");
                    latestJson = client.DownloadData(UPDATE_URI);
                    var serializer = new DataContractJsonSerializer(typeof(Release));
                    using (var ms = new MemoryStream(latestJson, false))
                    {
                        var release = (Release)serializer.ReadObject(ms);
                        _logger.Log($"CertInspector Latest version detected: {release.Name}");
                        var version = new Version(release.Name.Substring(1));
                        var downloadUrl = release.HtmlUrl;
                        if (downloadUrl != null)
                        {
                            _updateStatus.DownloadLocation = downloadUrl;
                            _updateStatus.LatestVersion = version;
                        }
                    }
                    OnSuccess?.Invoke();

                }
                catch (Exception e)
                {
                    _logger.Log($"Failed to check for updates to FiddlerCert Inspector: {e.Message}");
                    _logger.Log("FiddlerCert Inspector will try to check for updates in 5 minutes.");
                    OnError?.Invoke(e);
                }

            }
            catch (Exception e)
            {
                OnError?.Invoke(e);
            }
            finally
            {
                if (lockTaken)
                {
                    Monitor.Exit(_reentryLock);
                }
            }
        }
    }
}
