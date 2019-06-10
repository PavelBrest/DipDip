using System;

namespace MusicRnn.Models
{
    class DelegateCommand : IDelegateCommand
    {
        Action<object> _execute;
        Func<object, bool> _canExecute;

        public event EventHandler CanExecuteChanged;

        public DelegateCommand(Action<object> execute, Func<object, bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public DelegateCommand(Action<object> execute)
        {
            _execute = execute;
            _canExecute = this.AlwaysCanExecute;
        }

        public bool CanExecute(object parameter)
            => _canExecute(parameter);

        public void Execute(object parameter)
            => _execute(parameter);

        public void RaisCanExecuteChanged()
            => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

        private bool AlwaysCanExecute(object param)
            => true;
    }
}
