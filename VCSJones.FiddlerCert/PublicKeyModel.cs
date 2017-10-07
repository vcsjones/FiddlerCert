using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VCSJones.FiddlerCert
{
    public class PublicKeyModel : INotifyPropertyChanged
    {
        private PublicKeyAlgorithm _algorithm;
        private int? _keySizeBits;
        private byte[] _publicKey;

        public PublicKeyAlgorithm Algorithm
        {
            get => _algorithm;
            set
            {
                _algorithm = value;
                OnPropertyChanged();
            }
        }

        public int? KeySizeBits
        {
            get => _keySizeBits;
            set
            {
                _keySizeBits = value;
                OnPropertyChanged();
            }
        }

        public byte[] PublicKey
        {
            get => _publicKey;
            set
            {
                _publicKey = value;
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