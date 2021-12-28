using Microsoft . Identity . Client . Extensions . Msal;

using System;
using System . Windows;
using System . Windows . Input;
using System . Windows . Media . Imaging;

using WPFPages;
using WPFPages . Commands;
using WPFPages . Views;

/// <summary>
/// Class to host ALL the COMMAND functionality  for menus in the system
/// these are ALL called using Dispatcher() calls
/// </summary>
public static class MenuCommands
{
	/*
	 * Remember to use the followng to call non static operations
	 * Dispatcher.Invoke( () => {
				MenuCommands .Hello_Executed ( e .Parameter );
					});
	 * */
	 /*******************************************************************************************************************/
	public static bool Grab_CanExecute ( object parameter )
	{
		// parameter is the parameter object passed from the main call (string in this case)
		Console . WriteLine ( $"Grab : {parameter}" );
		return true;
	}
	public static void Grab_Executed ( object parameter )
	{
		// parameter is the parameter object passed from the main call, (a String in this case)
		if ( parameter . ToString ( ) == "" )
			return;
		else
			GrabImage (null, parameter );
	}
	public static void GrabImage (Window win,  object obj)
	{
		RenderTargetBitmap bmp;
		bmp = Utils . CreateControlImage ( obj as FrameworkElement , @"J:\\users\ianch\Documents\capturedimage.png" );
		Utils . SaveImageToFile ( ( RenderTargetBitmap ) bmp , @"C:\users\ianch\documents\Grabimage.png" , "PNG" );
		Grabviewer gv = new Grabviewer(win,win, bmp);
		gv . Grabimage . Source = bmp;
		gv . Title = @"C:\users\ianch\documents\Grabimage.png";
		gv . Show ( );
	}
	/*******************************************************************************************************************/

	public static bool Hello_CanExecute ( object parameter )
	{
		// parameter is the parameter object passed from the main call (string in this case)
		Console . WriteLine ( parameter );
		return true;
	}
	public static void Hello_Executed ( object parameter )
	{
		// parameter is the parameter object passed from the main call, (a String in this case)
		if ( parameter . ToString ( ) == "" )
			MessageBox . Show ( "Bye" );
		else
			MessageBox . Show ( parameter as string );
	}
	public static bool Bye_CanExecute ( object parameter )
	{
		// parameter is the parameter object passed from the main call
		return true;
	}
	public static void Bye_Executed ( object parameter )
	{
		// parameter is the parameter object passed from the main call, (a String in this case)
		MessageBox . Show ( parameter as string );
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
		MessageBox . Show ( parameter as string );
		Window win = window  as Window;
		win . Close ( );
	}
	public static bool CommandExit_CanExecute ( object parameter )
	{
		// parameter is the parameter object passed from the main call
		// a bool in  this case = Isdirty)  We should prcess this here ?
		if ( parameter != null )
			return true;
		return false;
	}
	public static void CommandExit_Executed ( object window , object parameter )
	{
		// parameter is the parameter object passed from the main call, (a String in this case)
		if ( parameter != null )
			MessageBox . Show ( parameter as string + "Closing Window" );
		else
			MessageBox . Show ( "No parameter received ?" );
		Window win = window  as Window;
		win . Close ( );
	}
}