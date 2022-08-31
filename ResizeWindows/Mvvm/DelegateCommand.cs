using System;
using System.Windows.Input;

namespace ResizeWindows.Mvvm
{
    public class DelegateCommand : ICommand
    {
        private readonly Action execute;
        public bool CanExecute(object? parameter) => true;

#pragma warning disable CS0067
        public event EventHandler? CanExecuteChanged;
#pragma warning restore CS0067

        public void Execute(object? parameter) => execute();

        public DelegateCommand(Action execute)
        {
            this.execute = execute;
        }
    }
    public class DelegateCommand<T> : ICommand
    {
        private readonly Action<T?> execute;
        bool ICommand.CanExecute(object? parameter) => true;
        void ICommand.Execute(object? parameter) => Execute((T?)parameter);
        //public bool CanExecute(T? parameter) => true;

#pragma warning disable CS0067
        public event EventHandler? CanExecuteChanged;
#pragma warning restore CS0067
        public void Execute(T? parameter) => execute(parameter);


        public DelegateCommand(Action<T?> execute)
        {
            this.execute = execute;
        }
    }
}
