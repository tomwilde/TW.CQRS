using System;
using System.Windows.Input;

namespace EstateAgent.Common
{
    public class GenericDelegateCommand<T> : ICommand where T : class
    {
        private readonly Action<T> execute;
        private Func<T, bool> canExecute;

        public event EventHandler CanExecuteChanged;

        public GenericDelegateCommand(Action<T> execute, Func<T, bool> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public GenericDelegateCommand(Action<T> execute) : this(execute, null)
        {
        }

        public bool CanExecute(object parameter)
        {
            return CanExecute(parameter as T);
        }

        public bool CanExecute(T parameter)
        {
            if (canExecute == null)
                return true;
            else
                return canExecute.Invoke(parameter);
        }

        public void Execute(object parameter)
        {
            Execute(parameter as T);
        }

        public void Execute(T parameter)
        {
            if (execute == null) return;

            execute.Invoke(parameter);            
        }

        public void UpdateCanExecute(Func<T, bool> newCanExecute)
        {
            canExecute = newCanExecute;
            if (CanExecuteChanged != null) CanExecuteChanged(this, EventArgs.Empty);
        }
    }
}