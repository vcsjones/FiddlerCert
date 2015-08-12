using System.ComponentModel;
using System.Threading.Tasks;

namespace VCSJones.FiddlerCert
{
    public class AsyncProperty<TResult> : INotifyPropertyChanged
    {
        private readonly TResult _defaultValue;
        public event PropertyChangedEventHandler PropertyChanged;

        public AsyncProperty(Task<TResult> task, TResult defaultValue = default(TResult))
        {
            _defaultValue = defaultValue;
            Task = task;
            if (!task.IsCompleted)
            {
                var _ = WatchTaskAsync(task);
            }
        }

        public TResult Result => (Task.Status == TaskStatus.RanToCompletion) ? Task.Result : _defaultValue;

        private Task WatchTaskAsync(Task task)
        {
            return task.ContinueWith(t =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Result)));
            });
        }

        public Task<TResult> Task { get; }
    }
}