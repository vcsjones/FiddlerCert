using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VCSJones.FiddlerCert
{
    public class CertificateCtModel : INotifyPropertyChanged
    {
        private ObservableCollection<SctSignatureModel> _signatures = new ObservableCollection<SctSignatureModel>();

        public ObservableCollection<SctSignatureModel> Signatures
        {
            get => _signatures;
            set
            {
                _signatures = value;
                OnPropertyChanged();
            }
        }

        public bool HasSignatures => Signatures.Count > 0;

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class SctSignatureModel
    {
        public DateTime Timestamp { get; set; }
        public string LogIdHex { get; set; }
        public SctSignatureAlgorithm SignatureAlgorithm { get; set; }
        public SctHashAlgorithm HashAlgorithm { get; set; }
        public string LogName { get; set; }
        public string LogUrl { get; set; }
        public int Index { get; set; }
        public string SignatureHex { get; set; }
        public DateTimeOffset? RevocationEffective { get; set; }
        public bool Revoked => Timestamp > RevocationEffective;
    }
}
