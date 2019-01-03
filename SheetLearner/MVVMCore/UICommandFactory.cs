namespace MVVMHeplers
{
    using System;
    using System.Windows.Input;

    public class UICommandFactory
    {
        public static ICommand Create(Action<object> a)
        {
            var cmd = new CommandWrapper(a);
            return cmd;
        }

        public static ICommand Create(Action<object> a, Func<bool> canExecute)
        {
            var cmd = new CommandWrapper(a,canExecute);
            return cmd;
        }

        private class CommandWrapper : ICommand
        {
            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }

            Action<object> theCommand;
            Func<bool> canExecuteCondition;

            public CommandWrapper(Action<object> a, Func<bool> canExecute)
            {
                theCommand = a;
                canExecuteCondition = canExecute;
            }

            public CommandWrapper(Action<object> a)
            {
                theCommand = a;
                canExecuteCondition = () => true;
            }

            public static CommandWrapper Create(Action<object> a)
            {
                return new CommandWrapper(a);
            }


            //Can exexute if either the condition for execution is true
            //or if only an action is set
            public bool CanExecute(object parameter)
            {
                if (canExecuteCondition != null)
                    return canExecuteCondition();

                if (theCommand != null)
                    return true;

                return false;

            }

            public void Execute(object parameter)
            {
                if (theCommand != null)
                    theCommand(parameter);
            }
        }
    }

}
