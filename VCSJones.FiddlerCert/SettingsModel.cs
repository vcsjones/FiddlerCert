using Fiddler;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VCSJones.FiddlerCert
{
    public class SettingsModel : INotifyPropertyChanged
    {
        private bool _checkForUpdates;
        private RelayCommand _saveCommand, _cancelCommand;

        public Version LatestVersion => CertificateInspector.LatestVersion?.Item1;

        public Version CurrentVersion => typeof(CertInspector).Assembly.GetName().Version;

        public bool CheckForUpdates
        {
            get => _checkForUpdates;
            set
            {
                _checkForUpdates = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand SaveCommand
        {
            get => _saveCommand;
            set
            {
                _saveCommand = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand CancelCommand
        {
            get => _cancelCommand;
            set
            {
                _cancelCommand = value;
                OnPropertyChanged();
            }
        }

        public SettingsModel()
        {
            SaveCommand = new RelayCommand(_ =>
            {
                FiddlerApplication.Prefs.SetBoolPref(PreferenceNames.CHECK_FOR_UPDATED_PREF, CheckForUpdates);
                //saving - and changing - the settings should count as asking.
                FiddlerApplication.Prefs.SetBoolPref(PreferenceNames.ASK_CHECK_FOR_UPDATES_PREF, true);
                CloseRequest?.Invoke();
            });
            CancelCommand = new RelayCommand(_ =>
            {
                CloseRequest?.Invoke();
            });
            CheckForUpdates = FiddlerApplication.Prefs.GetBoolPref(PreferenceNames.CHECK_FOR_UPDATED_PREF, false);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event Action CloseRequest;

        public void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
