using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Windows.Input;

namespace VCSJones.FiddlerCert
{
    public class CertificateModel : INotifyPropertyChanged
    {
        private string _commonName;
        private string _thumbprint;
        private string _subjectAlternativeName;
        private PublicKeyModel _publicKey;
        private DateTime _beginDate;
        private DateTime _endDate;
        private SignatureAlgorithmModel _signatureAlgorithm;
        private AsyncProperty<CertificateErrors> _errors;
        private RelayCommand _viewCommand, _installCommand;
        private AsyncProperty<SpkiHashesModel> _spkiHashes;

        public string CommonName
        {
            get
            {
                return _commonName;
            }
            set
            {
                _commonName = value;
                OnPropertyChanged();
            }
        }

        public string Thumbprint
        {
            get
            {
                return _thumbprint;
            }
            set
            {
                _thumbprint = value;
                OnPropertyChanged();
            }
        }

        public string SubjectAlternativeName
        {
            get
            {
                return _subjectAlternativeName;
            }
            set
            {
                _subjectAlternativeName = value;
                OnPropertyChanged();
            }
        }

        public PublicKeyModel PublicKey
        {
            get { return _publicKey; }
            set
            {
                _publicKey = value;
                OnPropertyChanged();
            }
        }

        public DateTime BeginDate
        {
            get { return _beginDate; }
            set
            {
                _beginDate = value;
                OnPropertyChanged();
            }
        }

        public DateTime EndDate
        {
            get { return _endDate; }
            set
            {
                _endDate = value;
                OnPropertyChanged();
                // ReSharper disable once ExplicitCallerInfoArgument
                OnPropertyChanged(nameof(ExpiresIn));
            }
        }

        public SignatureAlgorithmModel SignatureAlgorithm
        {
            get { return _signatureAlgorithm; }
            set
            {
                _signatureAlgorithm = value;
                OnPropertyChanged();
            }
        }

        public TimeSpan ExpiresIn => EndDate - DateTime.Now;

        public AsyncProperty<CertificateErrors> Errors
        {
            get
            {
                return _errors;
            }
            set
            {
                _errors = value;
                OnPropertyChanged();
            }
        }

        public AsyncProperty<SpkiHashesModel> SpkiHashes
        {
            get { return _spkiHashes; }
            set
            {
                _spkiHashes = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand ViewCommand
        {
            get { return _viewCommand; }
            set
            {
                _viewCommand = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand InstallCommand
        {
            get { return _installCommand; }
            set
            {
                _installCommand = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class SpkiHashesModel : INotifyPropertyChanged
    {
        
        private ObservableCollection<SpkiHashModel> _hashes;

        public ObservableCollection<SpkiHashModel> Hashes
        {
            get
            {
                return _hashes;
            }
            set
            {
                _hashes = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

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


    public enum CertificateErrors
    {
        Unknown,
        Critical,
        UnknownRevocation,
        None
    }

    public class SignatureAlgorithmModel : INotifyPropertyChanged
    {
        private Oid _signatureAlgorithm;
        private bool _isTrustedRoot;

        public Oid SignatureAlgorithm
        {
            get { return _signatureAlgorithm; }
            set
            {
                _signatureAlgorithm = value;
                OnPropertyChanged();
            }
        }

        public bool IsTrustedRoot
        {
            get { return _isTrustedRoot; }
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

    public class PublicKeyModel : INotifyPropertyChanged
    {
        private PublicKeyAlgorithm _algorithm;
        private int? _keySizeBits;
        private byte[] _publicKey;

        public PublicKeyAlgorithm Algorithm
        {
            get { return _algorithm; }
            set
            {
                _algorithm = value;
                OnPropertyChanged();
            }
        }

        public int? KeySizeBits
        {
            get { return _keySizeBits; }
            set
            {
                _keySizeBits = value;
                OnPropertyChanged();
            }
        }

        public byte[] PublicKey
        {
            get { return _publicKey; }
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

    public class RelayCommand : ICommand
    {
        private readonly Action<object> _action;

        public RelayCommand(Action<object> action)
        {
            _action = action;
        }

        public void Execute(object parameter)
        {
            _action(parameter);
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
    }

    public class HttpSecurityModel : INotifyPropertyChanged
    {
        private ObservableCollection<CertificateModel> _certificateChain;

        public ObservableCollection<CertificateModel> CertificateChain
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