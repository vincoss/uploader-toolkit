using System;
using System.Windows.Input;


namespace Vinco.Silverlight.Framework.Commands
{
    // TODO: Ensure that is completed
    public abstract class Command : ICommand
    {
        protected Command()
        {
        }

        public void RaiseCanExecuteChanged()
        {
            OnCanExecuteChanged();
        }

        public abstract void Execute(object parameter);

        public virtual bool CanExecute(object parameter)
        {
            return true;
        }

        #region ICommand Members

        public event EventHandler CanExecuteChanged;

        #endregion

        protected void OnCanExecuteChanged()
        {
            var handler = this.CanExecuteChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}
