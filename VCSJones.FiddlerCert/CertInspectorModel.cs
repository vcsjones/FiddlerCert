using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VCSJones.FiddlerCert
{
    public class CertInspectorModel : INotifyPropertyChanged
    {
        private HttpSecurityModel _httpSecurityModel;
        private UpdateBarModel _updateBarModel;
        private AskUpdateBarModel _askUpdateBarModel;

        public HttpSecurityModel HttpSecurityModel
        {
            get
            {
                return _httpSecurityModel;
            }
            set
            {
                _httpSecurityModel = value;
                OnPropertyChanged();
            }
        }

        public UpdateBarModel UpdateBarModel
        {
            get
            {
                return _updateBarModel;
            }
            set
            {
                _updateBarModel = value;
                OnPropertyChanged();
            }
        }

        public AskUpdateBarModel AskUpdateBarModel
        {
            get
            {
                return _askUpdateBarModel;
            }
            set
            {
                _askUpdateBarModel = value;
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
