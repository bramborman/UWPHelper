using System;
using System.Windows.Input;

namespace UWPHelper.Utilities
{
    public sealed class DelegateCommand : ICommand
    {
        private readonly Predicate<object> canExecute;

        public event EventHandler CanExecuteChanged;
        public event EventHandler<object> Executed;

        public DelegateCommand(EventHandler<object> executed) : this(executed, null)
        {

        }

        public DelegateCommand(EventHandler<object> executed, Predicate<object> canExecute)
        {
            Executed += executed;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return canExecute?.Invoke(parameter) ?? true;
        }

        public void Execute(object parameter)
        {
            Executed?.Invoke(this, parameter);
        }

        public void InvokeCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }
    }
}