using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VCSJones.FiddlerCert
{
    public class CertInspectorModel : INotifyPropertyChanged
    {
        private HttpSecurityModel _httpSecurityModel;
        private UpdateBarModel _updateBarModel;
        private AskUpdateBarModel _askUpdateBarModel;
        private RelayCommand _settingsCommand;

        public HttpSecurityModel HttpSecurityModel
        {
            get => _httpSecurityModel;
            set
            {
                _httpSecurityModel = value;
                OnPropertyChanged();
            }
        }

        public UpdateBarModel UpdateBarModel
        {
            get => _updateBarModel;
            set
            {
                _updateBarModel = value;
                OnPropertyChanged();
            }
        }

        public AskUpdateBarModel AskUpdateBarModel
        {
            get => _askUpdateBarModel;
            set
            {
                _askUpdateBarModel = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand SettingsCommand
        {
            get => _settingsCommand;
            set
            {
                _settingsCommand = value;
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
