using System;
using System.Windows.Input;

namespace ElectronicZone.Wpf.Helper
{
    public class CommandHandler : ICommand
    {
        private Action<object> _executeMethod;
        Func<object, bool> _canExecuteMethod;
        // private bool _canExecute;
        public CommandHandler(Action<object> action, Func<object, bool> canExecuteMethod) {
            _executeMethod = action;
            _canExecuteMethod = canExecuteMethod;
        }

        public bool CanExecute(object parameter) {
            return true;
        }
        public void Execute(object parameter) {
            _executeMethod(parameter);
        }
        public event EventHandler CanExecuteChanged;
    }
}
