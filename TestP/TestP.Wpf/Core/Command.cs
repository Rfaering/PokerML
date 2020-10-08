using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace TestP.Wpf.Context
{
    public class Command : ICommand
    {
        public Action Action { get; }

        public event EventHandler CanExecuteChanged;

        public Command(Action action)
        {
            Action = action;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Action();
        }
    }
}
