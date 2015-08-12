using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VCSJones.FiddlerCert
{
    public class CertificateModel : INotifyPropertyChanged
    {
        private AsyncProperty<string> _spkiSHA256Hash;
        private AsyncProperty<string> _spkiSHA1Hash;


        public AsyncProperty<string> SPKISHA256Hash
        {
            get
            {
                return _spkiSHA256Hash;
            }
            set
            {
                _spkiSHA256Hash = value;
                OnPropertyChanged();
            }
        }

        public AsyncProperty<string> SPKISHA1Hash
        {
            get
            {
                return _spkiSHA1Hash;
            }
            set
            {
                _spkiSHA1Hash = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}