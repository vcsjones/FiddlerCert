using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace VCSJones.FiddlerCert
{
    public class UpdateBarModel : INotifyPropertyChanged
    {
        private bool _updateAvailable;
        private Version _version;
        private RelayCommand _dismissCommand, _downloadCommand;

        public bool UpdateAvailable
        {
            get
            {
                return _updateAvailable;
            }
            set
            {
                _updateAvailable = value;
                OnPropertyChanged();
            }
        }

        public Version Version
        {
            get
            {
                return _version;
            }
            set
            {
                _version = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand DismissCommand => _dismissCommand;

        public RelayCommand DownloadCommand => _downloadCommand;

        public UpdateBarModel(Version version, string downloadUri)
        {
            if (version == null || downloadUri == null)
            {
                UpdateAvailable = false;
                return;
            }
            const string DISMISSED_VERSION = "certinspector.dismissedversion";
            Version = version;
            var dismissed = Fiddler.FiddlerApplication.Prefs.GetStringPref(DISMISSED_VERSION, null);
            var isVersionDismissed = false;
            if (dismissed != null)
            {
                try
                {
                    //User has indicated they don't want this version.
                    var dismissedVersion = new Version(dismissed);
                    isVersionDismissed = dismissedVersion >= version;
                }
                catch
                {
                    Fiddler.FiddlerApplication.Log.LogString("Preference contains bogus version for dismissal. Clearing.");
                    Fiddler.FiddlerApplication.Prefs.RemovePref(DISMISSED_VERSION);
                    isVersionDismissed = false;
                }
            }
            var currentVersion = typeof(CertInspector).Assembly.GetName().Version;
            if (isVersionDismissed)
            {
                Fiddler.FiddlerApplication.Log.LogString($"The version {dismissed} has been dismissed. No notification bar will appear.");
            }
            UpdateAvailable = version > currentVersion && !isVersionDismissed;
            Fiddler.FiddlerApplication.Log.LogString($"FiddlerCert Inspector: Current version is {currentVersion}, latest version is {version}.");
            _dismissCommand = new RelayCommand((_) =>
            {
                Fiddler.FiddlerApplication.Prefs.SetStringPref(DISMISSED_VERSION, version.ToString(4));
                UpdateAvailable = false;
            });
            _downloadCommand = new RelayCommand((_) =>
            {
                var uri = new Uri(downloadUri);
                if (uri?.Scheme == Uri.UriSchemeHttps)
                {
                    Process.Start(uri.AbsoluteUri);
                }
                else
                {
                    Fiddler.FiddlerApplication.Log.LogString("Refusing to open non-HTTPS page.");
                }
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
