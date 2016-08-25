using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VCSJones.FiddlerCert
{
    public class HttpSecurityModel : INotifyPropertyChanged
    {
        private AsyncProperty<ObservableCollection<CertificateModel>> _certificateChain;
        private AsyncProperty<ObservableCollection<CertificateModel>> _contentChain;
        private HpkpModel _hpkp;
        private bool _isNotTunnel;

        public AsyncProperty<ObservableCollection<CertificateModel>> CertificateChain
        {
            get { return _certificateChain; }
            set
            {
                _certificateChain = value;
                OnPropertyChanged();
            }
        }
        public AsyncProperty<ObservableCollection<CertificateModel>> ContentChain
        {
            get { return _contentChain; }
            set
            {
                _contentChain = value;
                OnPropertyChanged();
            }
        }

        public HpkpModel Hpkp
        {
            get
            {
                return _hpkp;
            }
            set
            {
                _hpkp = value;
                OnPropertyChanged();
            }
        }

        public bool IsNotTunnel
        {
            get
            {
                return _isNotTunnel;
            }
            set
            {
                _isNotTunnel = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class HpkpModel : INotifyPropertyChanged
    {
        private string _rawHpkpHeader;
        private bool _hasHpkpHeaders;
        private ObservableCollection<HpkpHashModel> _pinDirectives;
        private ObservableCollection<PinCheckResult> _pinningErrors;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool HasHpkpHeaders
        {
            get { return _hasHpkpHeaders; }
            set
            {
                _hasHpkpHeaders = value;
                OnPropertyChanged();
            }
        }

        public string RawHpkpHeader
        {
            get { return _rawHpkpHeader; }
            set
            {
                _rawHpkpHeader = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<HpkpHashModel> PinDirectives
        {
            get { return _pinDirectives; }
            set
            {
                _pinDirectives = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<PinCheckResult> PinningErrors
        {
            get
            {
                return _pinningErrors;
            }
            set
            {
                _pinningErrors = value;
                OnPropertyChanged();
            }
        }


        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}