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
        private RelayCommand _downloadCommand, _closeCommand;

        public bool UpdateAvailable
        {
            get => _updateAvailable;
            set
            {
                _updateAvailable = value;
                OnPropertyChanged();
            }
        }

        public Version Version
        {
            get => _version;
            set
            {
                _version = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand DownloadCommand => _downloadCommand;

        public RelayCommand CloseCommand => _closeCommand;

        public UpdateBarModel(Version version, string downloadUri)
        {
            if (version == null || downloadUri == null)
            {
                UpdateAvailable = false;
                return;
            }
            
            Version = version;
            var currentVersion = typeof(CertInspector).Assembly.GetName().Version;
            var closed = Fiddler.FiddlerApplication.Prefs.GetPref(PreferenceNames.HIDE_UPDATED_PREF, false);
            UpdateAvailable = version > currentVersion && !closed;
            Fiddler.FiddlerApplication.Log.LogString($"FiddlerCert Inspector: Current version is {currentVersion}, latest version is {version}.");
            _downloadCommand = new RelayCommand(_ =>
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
            _closeCommand = new RelayCommand(_ =>
            {
                Fiddler.FiddlerApplication.Prefs.SetPref(PreferenceNames.HIDE_UPDATED_PREF, true);
                UpdateAvailable = false;
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
