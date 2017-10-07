using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VCSJones.FiddlerCert
{
    public class SpkiHashesModel : INotifyPropertyChanged
    {
        private ObservableCollection<SpkiHashModel> _hashes;

        public ObservableCollection<SpkiHashModel> Hashes
        {
            get => _hashes;
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
}