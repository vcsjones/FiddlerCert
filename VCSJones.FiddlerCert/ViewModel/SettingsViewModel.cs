using Fiddler;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using VCSJones.FiddlerCert.Services;

namespace VCSJones.FiddlerCert
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        private bool _checkForUpdates;
        private RelayCommand _saveCommand, _cancelCommand, _hyperlinkCommand;

        public Version LatestVersion => Container.Instance.Resolve<UpdateStatus>().LatestVersion;

        public Version CurrentVersion => typeof(CertInspector).Assembly.GetName().Version;

        public bool CheckForUpdates
        {
            get
            {
                return _checkForUpdates;
            }
            set
            {
                _checkForUpdates = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand SaveCommand
        {
            get
            {
                return _saveCommand;
            }
            set
            {
                _saveCommand = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand CancelCommand
        {
            get
            {
                return _cancelCommand;
            }
            set
            {
                _cancelCommand = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand HyperlinkCommand
        {
            get
            {
                return _hyperlinkCommand;
            }
            set
            {
                _hyperlinkCommand = value;
                OnPropertyChanged();
            }
        }

        public SettingsViewModel()
        {
            SaveCommand = new RelayCommand(_ =>
            {
                FiddlerApplication.Prefs.SetPref(PreferenceNames.CHECK_FOR_UPDATED_PREF, CheckForUpdates);
                //saving - and changing - the settings should count as asking.
                FiddlerApplication.Prefs.SetPref(PreferenceNames.ASK_CHECK_FOR_UPDATES_PREF, true);
                CloseRequest?.Invoke();
            });
            CancelCommand = new RelayCommand(_ =>
            {
                CloseRequest?.Invoke();
            });
            HyperlinkCommand = new RelayCommand(arg =>
            {
                var link = arg as string;
                if (link == null)
                {
                    return;
                }
                var uri = new Uri(link);
                if (uri.Scheme != Uri.UriSchemeHttps)
                {
                    return;
                }
                Process.Start(uri.AbsoluteUri);
            });
            CheckForUpdates = FiddlerApplication.Prefs.GetPref(PreferenceNames.CHECK_FOR_UPDATED_PREF, false);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event Action CloseRequest;

        public void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
