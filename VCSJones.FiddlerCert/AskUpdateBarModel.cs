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
                FiddlerApplication.Prefs.SetBoolPref(UpdateServices.ASK_CHECK_FOR_UPDATES_PREF, true);
                FiddlerApplication.Prefs.SetBoolPref(UpdateServices.CHECK_FOR_UPDATED_PREF, true);
                AskRequired = false;
            });
            _noCommand = new RelayCommand(_ =>
            {
                FiddlerApplication.Prefs.SetBoolPref(UpdateServices.ASK_CHECK_FOR_UPDATES_PREF, true);
                FiddlerApplication.Prefs.SetBoolPref(UpdateServices.CHECK_FOR_UPDATED_PREF, false);
                AskRequired = false;
            });
            AskRequired = !FiddlerApplication.Prefs.GetBoolPref(UpdateServices.ASK_CHECK_FOR_UPDATES_PREF, false);
        }

        public RelayCommand YesCommand => _yesCommand;
        public RelayCommand NoCommand => _noCommand;

        public bool AskRequired
        {
            get
            {
                return _askRequired;
            }

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

    public static class UpdateServices
    {
        public const string ASK_CHECK_FOR_UPDATES_PREF = "certinspector.askedcheckforupdates", CHECK_FOR_UPDATED_PREF = "certinspector.checkforupdates";
    }
}
