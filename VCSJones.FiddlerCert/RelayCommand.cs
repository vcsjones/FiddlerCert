using System;
using System.Windows.Input;

namespace VCSJones.FiddlerCert
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _action;

        public RelayCommand(Action<object> action) => _action = action;

        public void Execute(object parameter) => _action(parameter);

        public bool CanExecute(object parameter) => true;

#pragma warning disable CS0067
        public event EventHandler CanExecuteChanged;
#pragma warning restore CS0067
    }
}