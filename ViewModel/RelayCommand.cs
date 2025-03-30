using System;
using System.Windows.Input;

namespace ViewModel
{
    public class RelayCommand : ICommand
    {
        private readonly Action execute;
        private readonly Func<bool> canExecute;

        public RelayCommand(Action execute) : this(execute, null)
        {

        }

        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }

            this.execute = execute;
            this.canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        internal void RaiseCanExecuteChanged() 
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool CanExecute(object parameter)
        {
            return canExecute == null || canExecute();
        }

        public void Execute(object parameter)
        {
            execute();
        }
    }

    public class  RelayCommand<T> : ICommand
    {
        private readonly Action<T> execute;
        private readonly Predicate<T> canExecutePredicate;

        public RelayCommand(Action<T> execute) : this(execute, null)
        {

        }

        public RelayCommand(Action<T> execute, Predicate<T> canExecutePredicate)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }

            this.execute = execute;
            this.canExecutePredicate = canExecutePredicate;
        }

        public event EventHandler CanExecuteChanged;

        internal void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool CanExecute(object objectParameter)
        {
            return canExecutePredicate == null ? true : canExecutePredicate((T)objectParameter);
        }

        public void Execute(object parameter)
        {
            execute((T)parameter);
        }
    }
}
