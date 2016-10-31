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

        public SettingsModel()
        {
            SaveCommand = new RelayCommand(_ =>
            {
                FiddlerApplication.Prefs.SetBoolPref(UpdateServices.CHECK_FOR_UPDATED_PREF, CheckForUpdates);
                CloseRequest?.Invoke();
            });
            CancelCommand = new RelayCommand(_ =>
            {
                CloseRequest?.Invoke();
            });
            CheckForUpdates = FiddlerApplication.Prefs.GetBoolPref(UpdateServices.CHECK_FOR_UPDATED_PREF, CheckForUpdates);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event Action CloseRequest;

        public void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
