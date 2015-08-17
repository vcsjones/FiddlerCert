using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VCSJones.FiddlerCert
{
    public class SpkiHashModel : INotifyPropertyChanged
    {
        private string _hashBase64;
        private bool _isPinned;
        private PinAlgorithm _algorithm;
        private bool _reportOnly;

        public bool ReportOnly
        {
            get { return _reportOnly; }
            set
            {
                _reportOnly = value;
                OnPropertyChanged();
            }
        }


        public string HashBase64
        {
            get
            {
                return _hashBase64;
            }
            set
            {
                _hashBase64 = value;
                OnPropertyChanged();
            }
        }

        public bool IsPinned
        {
            get
            {
                return _isPinned;
            }
            set
            {
                _isPinned = value;
                OnPropertyChanged();
            }
        }

        public PinAlgorithm Algorithm
        {
            get
            {
                return _algorithm;
            }
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