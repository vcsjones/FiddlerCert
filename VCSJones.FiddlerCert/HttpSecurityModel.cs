using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VCSJones.FiddlerCert
{
    public class HttpSecurityModel : INotifyPropertyChanged
    {
        private AsyncProperty<ObservableCollection<CertificateModel>> _certificateChain;

        public AsyncProperty<ObservableCollection<CertificateModel>> CertificateChain
        {
            get { return _certificateChain; }
            set
            {
                _certificateChain = value;
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