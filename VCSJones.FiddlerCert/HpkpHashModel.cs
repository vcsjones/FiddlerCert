using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VCSJones.FiddlerCert
{
    public class HpkpHashModel : INotifyPropertyChanged
    {
        private string _hashBase64;
        private PinAlgorithm _algorithm;

        public string HashBase64
        {
            get => _hashBase64;
            set
            {
                _hashBase64 = value;
                OnPropertyChanged();
            }
        }

        public PinAlgorithm Algorithm
        {
            get => _algorithm;
            set
            {
                _algorithm = value;
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