using System;
using System .Windows;
using System .Windows .Input;

using WPFPages .Commands;

/// <summary>
/// Class to host ALL the COMMAND functionality  for menus in the system
/// these are ALL called using Dispatcher() calls
/// </summary>
public static  class MenuCommands
{
	/*
	 * Remember to use the followng to call non static operations
	 * Dispatcher.Invoke( () => {
				MenuCommands .Hello_Executed ( e .Parameter );
					});
	 * */


	public static bool Hello_CanExecute ( object parameter )
	{
		// parameter is the parameter object passed from the main call (string in this case)
		Console .WriteLine (parameter);
		return true;
	}
	public static void Hello_Executed ( object parameter )
	{
		// parameter is the parameter object passed from the main call, (a String in this case)
		if ( parameter .ToString ( ) == "" )
			MessageBox .Show ( "Bye" );
		else
			MessageBox .Show ( parameter as string );
	}
	public static bool Bye_CanExecute ( object parameter )
	{
		// parameter is the parameter object passed from the main call
		return true;
	}
	public static void Bye_Executed ( object parameter )
	{
		// parameter is the parameter object passed from the main call, (a String in this case)
		MessageBox .Show ( parameter as string );
	}
	public static bool Close_CanExecute ( object parameter )
	{
		// parameter is the parameter object passed from the main call
		// a bool in  this case = Isdirty)  We should prcess this here ?
		return true;
	}
	public static void Close_Executed ( object window , object parameter )
	{
		// parameter is the parameter object passed from the main call, (a String in this case)
		MessageBox .Show ( parameter as string );
		Window win = window  as Window;
		win .Close ( );
	}
	public static bool CommandExit_CanExecute ( object parameter )
	{
		// parameter is the parameter object passed from the main call
		// a bool in  this case = Isdirty)  We should prcess this here ?
		if(parameter != null)
			return true;
		return false;
	}
	public static void CommandExit_Executed ( object window , object parameter )
	{
		// parameter is the parameter object passed from the main call, (a String in this case)
		if(parameter != null)
			MessageBox .Show ( parameter as string  + "Closing Window");
		else
			MessageBox .Show ( "No parameter received ?");
		Window win = window  as Window;
		win .Close ( );
	}
}