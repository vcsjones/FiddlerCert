using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Input;

namespace VCSJones.FiddlerCert
{
    public class CertificateModel : INotifyPropertyChanged
    {
        private AsyncProperty<string> _spkiSHA256Hash;
        private AsyncProperty<string> _spkiSHA1Hash;
        private string _commonName;
        private string _thumbprint;
        private string _subjectAlternativeName;
        private PublicKeyModel _publicKey;
        private DateTime _beginDate;
        private DateTime _endDate;
        private SignatureAlgorithmModel _signatureAlgorithm;
        private AsyncProperty<CertificateErrors> _errors;
        private RelayCommand _viewCommand, _installCommand;


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
}