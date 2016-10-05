using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Windows.Input;
using WebApplication1;
using WebApplication1.FuzzyLogicBase;
using C__04._07._2015.Algorithms.Clustering;// progr_V.cs
using L.DataStructures.Matrix;// multidim..cs
using L.Algorithms.Clustering;// k_means.cs
using L.DataStructures.Geometry; // cluster.cs

namespace WebApplication1
{
    public class Command : ICommand
    {
        
        private Action<object> _execute;
        private Predicate<object> _canExecute;
        public void SetCanExecute(Predicate<object> value)
        {
            if (value != _canExecute)
            {
                _canExecute = value;
                CanExecuteChanged += Command_CanExecuteChanged;
            }
        }

        void Command_CanExecuteChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        public Command(Action<object> execute, Predicate<object> canExecute = null)
        {
            _execute = execute;
            if (canExecute != null)
                _canExecute = canExecute;
        }

        #region ICommand Members
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        public void Execute(object parameter)
        {
            if (_execute != null)
                _execute(parameter);
        }
        private void CanExecuteHandler(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CanExecute(e.Parameter);
        }
        public bool CanExecute(object parameter)
        {
            return _canExecute != null ? _canExecute(parameter) : true;
        }
        #endregion
    }
}