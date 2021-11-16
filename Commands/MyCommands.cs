using System .Windows .Controls;
using System .Windows .Input;

namespace WPFPages .Commands
{
	/// <summary>
	/// This class holds ALL Routed Custom Commands in the system
	/// </summary>
	public static class MyCommands
	{
		public static readonly RoutedUICommand Exit = new RoutedUICommand
			( "Exit Current Window", "Exit", typeof ( MyCommands ),
				new InputGestureCollection()
				{new KeyGesture(Key.F4, ModifierKeys.Alt)});

		public static readonly RoutedUICommand ShowMessage = new RoutedUICommand
			( "Show Message", "Show_Message", typeof ( MyCommands ),
				new InputGestureCollection()
				{new KeyGesture(Key.F3, ModifierKeys.Alt)});

		public static readonly RoutedUICommand CloseWin = new RoutedUICommand
			( "Close Window", "CloseWin", typeof ( MyCommands ),
							new InputGestureCollection()
				{new KeyGesture(Key.F4, ModifierKeys.Alt)});

		public static readonly RoutedUICommand CopyText = new RoutedUICommand
			( "Copy selected Text", "CopyText", typeof ( MyCommands ) ,
							new InputGestureCollection()
				{new KeyGesture(Key.F6, ModifierKeys.Alt)});

		public static readonly RoutedUICommand PasteText = new RoutedUICommand
			( "Paste from Clipboard", "PasteText", typeof ( MyCommands ),
							new InputGestureCollection()
				{new KeyGesture(Key.F8, ModifierKeys.Alt)});

		// Standard built-in commands (based on copy/paste/new/ etc
		public static readonly RoutedCommand Hello = new RoutedCommand ( );
		public static readonly RoutedCommand Bye = new RoutedCommand ( );


		}
}
