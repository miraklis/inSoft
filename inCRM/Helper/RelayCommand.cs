using System;
using System.Diagnostics;
using System.Windows.Input;

namespace inCRM
{
	public class RelayCommand<T> : ICommand
	{
		#region Fields

		readonly Action<T> _execute;
		readonly Predicate<T> _canExecute;

		#endregion

		#region Constructors

		public RelayCommand(Action<T> execute)
		: this(execute, null)
		{
		}

		public RelayCommand(Action<T> execute, Predicate<T> canExecute)
		{
			if (execute == null)
				throw new ArgumentNullException("execute");

			_execute = execute;
			_canExecute = canExecute;
		}
		#endregion

		#region ICommand Members

		[DebuggerStepThrough]
		public bool CanExecute(object parameter)
		{
			return _canExecute == null ? true : _canExecute((T)parameter);
		}

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}

		public void Execute(object parameter)
		{
			_execute((T)parameter);
		}

		public void Execute(object parameter, object enableParameter)
		{
			if(_canExecute((T)enableParameter))
				_execute((T)parameter);
		}

		#endregion
	}

	public class RelayCommand : ICommand
	{
		#region Fields

		readonly Action<object> _execute;
		readonly Predicate<object> _canExecute;

		#endregion

		#region Constructors

		public RelayCommand(Action<object> execute)
		: this(execute, null)
		{
		}

		public RelayCommand(Action<object> execute, Predicate<object> canExecute)
		{
			if (execute == null)
				throw new ArgumentNullException("execute");

			_execute = execute;
			_canExecute = canExecute;
		}
		#endregion

		#region ICommand Members

		public bool CanExecute(object parameter)
		{
			return _canExecute == null ? true : _canExecute(parameter);
		}

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}

		public void Execute(object parameter)
		{
			_execute(parameter);
		}

		#endregion
	}

}
