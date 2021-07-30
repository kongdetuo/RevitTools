using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using PropertyChanged;

namespace RevitCodePad.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class HistoryItem
    {
        public string Title { get; set; }
        public string FullName { get; set; }
    }

    public class DelegateCommand : ICommand
    {
        private readonly Action<object> _executeObj;

        public DelegateCommand(Action<object> executeObj)
        {
            this._executeObj = executeObj;
        }


        public void Execute(object parameter)
        {
            if (_executeObj != null)
                _executeObj.Invoke(parameter);
        }

        public bool CanExecute(object parameter)
        {
            return _executeObj != null;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }

}
