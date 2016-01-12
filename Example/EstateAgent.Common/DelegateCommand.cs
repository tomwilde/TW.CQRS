using System;
using System.Windows.Input;

namespace EstateAgent.Common
{
    public class DelegateCommand : ICommand 
    {
        private readonly Action<object> execute;
        private Predicate<object> canExecute;

        public event EventHandler CanExecuteChanged;

        public DelegateCommand(Action<object> execute, Predicate<object> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public DelegateCommand(Action<object> execute) : this(execute, null)
        {
        }

        public bool CanExecute(object parameter)
        {
            if (canExecute == null)
                return true;
            else
                return canExecute.Invoke(parameter);
        }

        public void Execute(object parameter)
        {
            if (execute == null) return;

            execute.Invoke(parameter);            
        }

        public void UpdateCanExecute(Predicate<object> newCanExecute)
        {
            canExecute = newCanExecute;
            if (CanExecuteChanged != null) CanExecuteChanged(this, EventArgs.Empty);
        }
    }
}