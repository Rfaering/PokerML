using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace TestP.Wpf.Context
{
    public class Command : ICommand
    {
        private readonly Action action;
        private readonly Func<bool> canExecute;

        public event EventHandler CanExecuteChanged;

        public Command(Action action, Func<bool> canExecute = null)
        {
            this.action = action;
            this.canExecute = canExecute;
        }

        public void CanExecuteHasChanged()
        {
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }

        public bool CanExecute(object parameter)
        {
            return canExecute != null ? canExecute() : true;
        }

        public void Execute(object parameter)
        {
            action();
        }
    }
}
