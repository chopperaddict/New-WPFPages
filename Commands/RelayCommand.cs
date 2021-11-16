using System;
using System .Collections .Generic;
using System .ComponentModel;
using System .Linq;
using System .Text;
using System .Threading .Tasks;
using System .Windows;
using System .Windows .Controls;
using System .Windows .Input;

namespace WPFPages .Commands
{
	/// <summary>
	/// Generic ICommand handler to simplify later commands 
	/// </summary>
	public class RelayCommand : ICommand

	{
		public Action<object> executeAction;
		public Func<object, bool> canExecute;
		//		bool canExecuteCache;
		public RelayCommand ( Action<object> executeaction , Func<object , bool> canExecute = null) // bool canExecuteCache )
		{
			this .canExecute = canExecute;
			this .executeAction = executeAction;
//			canExecuteCache = canExecuteCache;
		}

		public bool CanExecute ( object parameter )
		{
			if ( canExecute == ( object ) null )
				return true;
			else
				return canExecute ( parameter );
		}

		public void Execute ( object parameter )
		{
			if ( parameter != (object)null )
				executeAction ( parameter );

			Console .WriteLine ("Relay Command has been executed - eeeeeeeeeeeeekkkkk" );
		}

		public event EventHandler CanExecuteChanged
		{
			add {CommandManager .RequerySuggested += value;}
			remove {CommandManager .RequerySuggested -= value;}
		}


	}
}
