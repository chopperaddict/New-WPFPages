using System . Reflection;
using System . Windows;
using System . Windows . Controls;

namespace WPFPages
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		//		public static int GridWindows = 0;
		//public DataGrid CurrentGrid;
		//public DateTime LoadTime;
#if USEDETAILEDEXCEPTIONHANDLER
		WindowExceptionHandler _exceptionHandler;
#endif

		
		public App ( )
		{
			//SetTheme ( "aero" , "normalcolor" );
			//			new BindingTracer (msg => Debugger.Break ());
#if USEDETAILEDEXCEPTIONHANDLER
			_exceptionHandler = new WindowExceptionHandler ();
#endif
		
		}

		/// <summary>
		/// Sets the WPF system theme.
		/// </summary>
		/// <param name="themeName">The name of the theme. (ie "aero")</param>
		/// <param name="themeColor">The name of the color. (ie "normalcolor")</param>
		public static void SetTheme ( string themeName , string themeColor )
		{
			const BindingFlags staticNonPublic = BindingFlags.Static | BindingFlags.NonPublic;

			var presentationFrameworkAsm = Assembly.GetAssembly(typeof(Window));

			var themeWrapper = presentationFrameworkAsm.GetType("MS.Win32.UxThemeWrapper");

			var isActiveField = themeWrapper.GetField("_isActive", staticNonPublic);
			var themeColorField = themeWrapper.GetField("_themeColor", staticNonPublic);
			var themeNameField = themeWrapper.GetField("_themeName", staticNonPublic);

			// Set this to true so WPF doesn't default to classic.
			//			isActiveField . SetValue ( null , true );
			isActiveField . SetValue ( null , false );

			//			themeColorField . SetValue ( null , themeColor );
			//			themeNameField . SetValue ( null , themeName );
		}



		private void Application_dispatcherUnhandledException ( object sender , System . Windows . Threading . DispatcherUnhandledExceptionEventArgs e )
		{
			MessageBox . Show ( "An unhandled exception just occurred: " + e . Exception . Message , "Exception Sample" , MessageBoxButton . OK , MessageBoxImage . Warning );
			e . Handled = true;
		}

		public static void OnDataContextChanged ( DependencyObject d , DependencyPropertyChangedEventArgs e )
		{
			DataGrid grid = d as DataGrid;
			if ( grid != null )
			{
				foreach ( DataGridColumn col in grid . Columns )
				{
					col . SetValue ( FrameworkElement . DataContextProperty , e . NewValue );
					var header = col . Header as FrameworkElement;
					if ( header != null )
					{
						header . SetValue ( FrameworkElement . DataContextProperty , e . NewValue );
					}
				}
			}
		}

		#region MVVM STUFF

		// In App.xaml.cs
		protected override void OnStartup ( StartupEventArgs e )
		{
			//			base . OnStartup ( e );
		}

		#endregion MVVM STUFF

		//These are used to try to force Textbox's to always select
		// all the content in the field, rahter than the default
		// of putting the cursor at end of the current content
		//protected override void OnStartup ( StartupEventArgs e )
		//{
		//        EventManager . RegisterClassHandler ( typeof ( TextBox ),
		//            TextBox . GotFocusEvent,
		//            new RoutedEventHandler ( TextBox_GotFocus ) );

		//        FrameworkElement . LanguageProperty . OverrideMetadata (
		//         typeof ( FrameworkElement ),
		//         new FrameworkPropertyMetadata (
		//         System . Windows . Markup . XmlLanguage . GetLanguage ( CultureInfo . CurrentUICulture . IetfLanguageTag ) ) );

		//        base . OnStartup ( e );
		//}
	}
}
