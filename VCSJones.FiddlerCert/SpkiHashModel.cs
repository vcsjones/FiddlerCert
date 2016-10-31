using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace VCSJones.FiddlerCert
{
    public class SpkiHashModel : INotifyPropertyChanged
    {
        private byte[] _hash;
        private bool _isPinned;
        private PinAlgorithm _algorithm;
        private bool _reportOnly;
        private RelayCommand _clickCommand;

        public SpkiHashModel()
        {
            _clickCommand = new RelayCommand(parameter =>
            {
                var uri = parameter as Uri;
                if (uri?.Scheme == Uri.UriSchemeHttps)
                {
                    Process.Start(uri.AbsoluteUri);
                }
            });
        }

        public bool ReportOnly
        {
            get { return _reportOnly; }
            set
            {
                _reportOnly = value;
                OnPropertyChanged();
            }
        }

        public byte[] Hash
        {
            get
            {
                return _hash;
            }
            set
            {
                _hash = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HashHex));
                OnPropertyChanged(nameof(HashBase64));
                OnPropertyChanged(nameof(CrtShUri));
            }
        }

        public string HashHex
        {
            get
            {
                return BitConverter.ToString(Hash).Replace("-", "");
            }
        }

        public string HashBase64
        {
            get
            {
                return Convert.ToBase64String(Hash);
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
                OnPropertyChanged(nameof(CrtShUri));
            }
        }

        public Uri CrtShUri
        {
            get
            {
                return new Uri($"https://crt.sh/?spki{Algorithm.ToString().ToLower()}={HashHex}");
            }
        }

        public RelayCommand ClickCommand
        {
            get
            {
                return _clickCommand;
            }
            set
            {
                _clickCommand = value;
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