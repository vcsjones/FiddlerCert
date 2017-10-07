using Fiddler;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VCSJones.FiddlerCert
{
    public class AskUpdateBarModel : INotifyPropertyChanged
    {
        private bool _askRequired;
        private RelayCommand _yesCommand, _noCommand;

        public AskUpdateBarModel()
        {
            _askRequired = false;
            _yesCommand = new RelayCommand(_ =>
            {
                FiddlerApplication.Prefs.SetBoolPref(PreferenceNames.ASK_CHECK_FOR_UPDATES_PREF, true);
                FiddlerApplication.Prefs.SetBoolPref(PreferenceNames.CHECK_FOR_UPDATED_PREF, true);
                AskRequired = false;
            });
            _noCommand = new RelayCommand(_ =>
            {
                FiddlerApplication.Prefs.SetBoolPref(PreferenceNames.ASK_CHECK_FOR_UPDATES_PREF, true);
                FiddlerApplication.Prefs.SetBoolPref(PreferenceNames.CHECK_FOR_UPDATED_PREF, false);
                AskRequired = false;
            });
        }

        public RelayCommand YesCommand => _yesCommand;
        public RelayCommand NoCommand => _noCommand;

        public bool AskRequired
        {
            get => _askRequired;
            set
            {
                _askRequired = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
