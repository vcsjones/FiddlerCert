using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace VCSJones.FiddlerCert
{
    public class SignatureAlgorithmModel : INotifyPropertyChanged
    {
        private Oid _signatureAlgorithm;
        private bool _isTrustedRoot;

        public Oid SignatureAlgorithm
        {
            get => _signatureAlgorithm;
            set
            {
                _signatureAlgorithm = value;
                OnPropertyChanged();
            }
        }

        public bool IsTrustedRoot
        {
            get => _isTrustedRoot;
            set
            {
                _isTrustedRoot = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}