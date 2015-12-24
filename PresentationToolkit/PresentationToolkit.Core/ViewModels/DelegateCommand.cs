using System;
using System.Diagnostics;
using System.Windows.Input;

namespace PresentationToolkit.Core.ViewModels
{
    /// <summary>
    /// Delegate command for command binding to actions.
    /// </summary>
    public class DelegateCommand : ICommand
    {
        private readonly Action execute;
        private readonly Func<bool> canExecute;

        /// <summary>
        /// Raised when the can execute status is changed.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Initializes a new instance of <see cref="DelegateCommand"/>.
        /// </summary>
        /// <param name="execute">The command action to execute.</param>
        /// <param name="canexecute">The command status action to execute.</param>
        public DelegateCommand(Action execute, Func<bool> canexecute = null)
        {
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }
            this.execute = execute;
            this.canExecute = canexecute ?? (() => true);
        }

        /// <summary>
        /// Indicates whether the command can be executed or not. Note that if there
        /// are exceptions thrown, this method catches the exception and return false.
        /// </summary>
        /// <param name="p">The command parameter if any.</param>
        /// <returns>True if the command can be executed. Otherwise false.</returns>
        [DebuggerStepThrough]
        public bool CanExecute(object p = null)
        {
            try
            {
                return canExecute();
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Executes the action.
        /// </summary>
        /// <param name="p">The command parameter if any.</param>
        public void Execute(object p = null)
        {
            if (!CanExecute(p))
            {
                return;
            }

            execute();
        }

        /// <summary>
        /// Raised notification for the command status change.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Delegate command for command binding to actions with parameter type.
    /// </summary>
    /// <typeparam name="T">The parameter type.</typeparam>
    public class DelegateCommand<T> : ICommand
    {
        private readonly Action<T> execute;
        private readonly Func<T, bool> canExecute;

        /// <summary>
        /// Raised when the can execute status is changed.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Initializes a new instance of <see cref="DelegateCommand"/>.
        /// </summary>
        /// <param name="execute">The command action to execute.</param>
        /// <param name="canexecute">The command status action to execute.</param>
        public DelegateCommand(Action<T> execute, Func<T, bool> canexecute = null)
        {
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }

            this.execute = execute;
            this.canExecute = canexecute ?? (e => true);
        }

        /// <summary>
        /// Indicates whether the command can be executed or not. Note that if there
        /// are exceptions thrown, this method catches the exception and return false.
        /// </summary>
        /// <param name="p">The command parameter if any.</param>
        /// <returns>True if the command can be executed. Otherwise false.</returns>
        [DebuggerStepThrough]
        public bool CanExecute(object p)
        {
            try
            {
                var target = (T)Convert.ChangeType(p, typeof(T));
                return canExecute == null 
                    ? true 
                    : canExecute(target);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Executes the action.
        /// </summary>
        /// <param name="p">The command parameter if any.</param>
        public void Execute(object p)
        {
            if (!CanExecute(p))
            {
                return;
            }

            var target = (T)Convert.ChangeType(p, typeof(T));
            execute(target);
        }

        /// <summary>
        /// Raised notification for the command status change.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
