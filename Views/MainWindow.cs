using System . ComponentModel;
using System . Runtime . CompilerServices;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Input;
using WPFPages . Views;
using WPFPages . ViewModels;

//using WpfUI;
using System;
using System . Data . SqlClient;
using System . Data;
using System . Diagnostics;
using System . Configuration;
using System . IO;
using Newtonsoft . Json . Linq;
using WPFPages . Properties;
using Dapper;
using System . Collections . Generic;
using System . Linq;

using System . Windows . Media;
using WPFPages . AttachedProperties;
using Microsoft . SqlServer . Management . Smo;
using static System . Windows . Forms . VisualStyles . VisualStyleElement . TrayNotify;

using System . Net;
using System . Security . Policy;

namespace WPFPages
{

	#region My MessageBox Definitions

	public struct mb
	{
		static public int nnull = 0;
		static public int NNULL=0;
		static public int ok=1;
		static public int OK=1;
		static public int yes=2;
		static public int YES=2;
		static public int no=3;
		static public int NO=3;
		static public int cancel=4;
		static public int CANCEL=4;
		static public int iconexclm=5;
		static public int ICONEXCLM=5;
		static public int iconwarn=6;
		static public int ICONWARN=6;
		static public int iconerr=7;
		static public int ICONERR=7;
		static public int iconinfo=8;
		static public int ICONINFO=8;
	}

	public struct MB
	{
		static public int nnull = 0;
		static public int NNULL=0;
		static public int ok=1;
		static public int OK=1;
		static public int yes=2;
		static public int YES=2;
		static public int no=3;
		static public int NO=3;
		static public int cancel=4;
		static public int CANCEL=4;
		static public int iconexclm=5;
		static public int ICONEXCLM=5;
		static public int iconwarn=6;
		static public int ICONWARN=6;
		static public int iconerr=7;
		static public int ICONERR=7;
		static public int iconinfo=8;
		static public int ICONINFO=8;
	}

	/// <summary>
	/// output parameters (return values) for my Message Box
	/// </summary>
	public struct Dlgresult
	{
		static public bool result;
		static public int returnint;
		static public string returnstring;
		static public string returnerror;
		static public object obj;
	}
	#endregion My MessageBox Definitions

	#region My MessageBox argument structuress
	/// <summary>
	/// Input parameters for my Message Box
	/// </summary>
	public  struct DlgInput
	{
		static public Msgbox MsgboxWin;
		static public Msgboxs MsgboxSmallWin;
		static public Msgboxs MsgboxMinWin;
		public static SysMenu sysmenu;
		static public bool isClean;
		static public bool resultboolin;
		static public bool UseDarkMode;
		static public bool resetdata;
		static public bool UseIcon;
		static public int intin;
		static public int returnint;
		static public string stringin;
		static public object obj;
		static public string iconstring;
		static public Thickness thickness;
		static public Image image;
		static public Brush dlgbackground;
		static public Brush dlgforeground;
		static public Brush btnbackground;
		static public Brush btnforeground;
		static public Brush Btnborder;
		static public Brush Btnmousebackground;
		static public Brush Btnmouseforeground;
		static public Brush defbtnbackground;
		static public Brush defbtnforeground;
		// Dark mode
		static public Brush BtnborderDark;
		static public Brush btnforegroundDark;
		static public Brush btnbackgroundDark;
		static public Brush defbtnforegroundDark;
		static public Brush defbtnbackgroundDark;
		static public Brush mouseborderDark;
		static public Brush mousebackgroundDark;
		static public Brush mouseforegroundDark;

		static public Thickness BorderSizeNormal;				 // Normal display shadow
		static public Thickness BorderSizeDefault;		// Mouse over / (current Default) display
	}

	public struct defvars
	{
		public static Uri  cookierootname=new(@"C:\Cookie");
		public static String CookieDictionarypath=@"J:\users\ianch\documents\CookieDictionary.ser";
		public static String CookieCollectionpath=@"J:\users\ianch\documents\CookieCollection.ser";
		public static Dictionary<string , string> Cookiedictionary;
		public static CookieCollection  Cookiecollection;
		public static  int NextCookieIndex = 0;
		public static bool CookieAdded=false;
		public static bool FullViewer=false;
	}
	//public static Dictionary<string, string> cookiedict =new Dictionary<string, string>()	;
	#endregion My MessageBox arguments

	delegate void DbEditOcurred ( object Sender , EditEventArgs e );
	delegate void SQLEditOcurred ( object Sender , EditEventArgs e );

	delegate void GrabScreenObject( object Sender , GrabImageArgs e );
	//	BankAccountViewModel bvm = BankAccountViewModel.bvm;

	public partial class MainWindow : System . ComponentModel . INotifyPropertyChanged
	{
		public static  DlgInput  dlgdata;

		// Global pointers to Viewmodel classes
		public static BankAccountViewModel bvm = null;
		public static CustomerViewModel cvm = null;
		public static DetailsViewModel dvm = null;

		public static EditEventArgs EditArgs = new EditEventArgs ( );
		public static DataGridController DgControl = new DataGridController ( );

//		public Frame theFrame;
		public static Page _Blank = new BlankPage ( );
		public static Page _Page0 = new Page0 ( );
		//public static Page _Page1 = new Page1 ( );
		//public static Page _Page2 = new Page2 ( );
		//public static Page _Page3 = new Page3 ( );
		//public static Page _Page4 = new Page4 ( );
		//public static Page _Page5 = new Page5 ( );
//		public static string _baseDataText;
//		private string _randomText1 = "button1";
//		private string _randomText2 = "button2";
		public bool Autoload = false;
		private bool key1 = false;

		public static GridViewer gv = new GridViewer ( );
		public static DbSelector dbs = null;
		public static SysMenu sysmenu;

		public SqlDbViewer tw = null;

		public MainWindow ( )
		{

			Application . Current . Resources . Clear ( );
			// Load a new Theme dictionary ??
			//ResourceDictionary skin = Application.LoadComponent(new Uri("", UriKind.Relative)) as ResourceDictionary;
			// Set this as the main dataContext
			//			DataContext = this;
			InitializeComponent ( );
			Loaded += MainWindowLoaded;

			GetDefaultMsgboxColors ( );

			// Button "Shortforms" declaratons
			{
				// configure global messagebox ID's
				MB . NNULL = mb . NNULL = 0;
				MB . OK = mb . OK = 1;
				MB . YES = mb . YES = 2;
				MB . NO = mb . NO = 3;
				MB . CANCEL = mb . CANCEL = 4;
				MB . ICONEXCLM = mb . ICONEXCLM = 10;
				MB . ICONWARN = mb . ICONWARN = 11;
				MB . ICONERR = mb . ICONERR = 12;
				MB . ICONINFO = mb . ICONINFO = 13;
				MB . nnull = mb . nnull = 0;
				MB . ok = mb . ok = 1;
				MB . yes = mb . yes = 2;
				MB . no = mb . no = 2;
				MB . cancel = mb . cancel = 4;
				MB . iconexclm = mb . iconexclm = 10;
				MB . iconwarn = mb . iconwarn = 11;
				MB . iconerr = mb . iconerr = 12;
				MB . iconinfo = mb . iconinfo = 13;
			}

			this . Topmost = true;
			OntopChkbox . IsChecked = true;
			gv = GridViewer . Viewer_instance;
			// Get this users \Documents folder and use it to save location for our Persistent Search paths
			var defdocuments = Environment . GetFolderPath ( Environment . SpecialFolder . MyDocuments ) + @"\searchpaths.dat";

			Utils . SaveProperty ( "SearchPathFile" , defdocuments );
			//		ConfigurationManager . RefreshSection ( "SearchPathFile" );

			// Save local Documents folder path for later use
			string t = Environment . GetFolderPath ( Environment . SpecialFolder . MyDocuments ) + "\"";
			Utils . SaveProperty ( "DocumentsPath" , t );

			// Get Applications development root folder
			t = Directory . GetParent ( Directory . GetCurrentDirectory ( ) ) . Parent . FullName;

			Library1. AddUpdateAppSettings ( "AppRoot" , t );
			Utils . SaveProperty ( "AppRoot" , t );
			Library1 . AddUpdateAppSettings ( "AppRoot" , t );

			// outputs in Output window
			Utils . ReadAllConfigSettings ( );

			// Clean MsgBox data and reload from disk flie
			ResetMsgBox ( );
			ReadMsgboxData ( );
			MouseMove += Grab_MouseMove;
			KeyDown += Window_PreviewKeyDown;
			//Read in our Cookies.CookieDictionary and Cookies. Cookiecollection from disk (Serialized)
			defvars.Cookiedictionary = Cookies . DeSerialize ( defvars.CookieDictionarypath) as Dictionary<string, string>;
			if ( defvars . Cookiedictionary != null )
			{
				// Set our auto index value for all new cookies paths
				defvars . NextCookieIndex = defvars . Cookiedictionary . Count;
			}
			else
			{
				defvars . Cookiedictionary = new Dictionary<string , string> ( );
				defvars . NextCookieIndex = 1;
			}
			defvars . Cookiecollection = Cookies . DeSerialize ( defvars.CookieCollectionpath ) as CookieCollection;
			if ( defvars.Cookiecollection == null )
				defvars.Cookiecollection = new CookieCollection ( );
			
			// Create some test cookies
			Cookies.CreateTestCookies ( );
		}
		private void Grab_MouseMove ( object sender , MouseEventArgs e )
		{
			if ( e . LeftButton == MouseButtonState . Pressed )
				Utils . Grab_MouseMove ( sender , e );
			e . Handled = true;
		}

		private void Window_PreviewKeyDown ( object sender , KeyEventArgs e )
		{
			if ( e . Key == Key . F11 )
			{
				var pos = Mouse . GetPosition ( this);
				Utils . Grab_Object ( sender , pos );
				if ( Utils . ControlsHitList . Count == 0 )
					return;
				Utils . Grabscreen ( this , Utils . ControlsHitList [ 0 ] . VisualHit , null , sender as Control );
			}
		}

		public static void ResetMsgBox ( )
		{
			DlgInput . isClean = true;
			DlgInput . resultboolin = false;
			DlgInput . UseDarkMode = false;
			DlgInput . resetdata = true;
			DlgInput . UseIcon = true;
			DlgInput . intin = 0;
			DlgInput . stringin = "";
			DlgInput . obj = null;
			DlgInput . dlgbackground = "#9DFFFFFB" . ToSolidBrush ( );
			DlgInput . Btnmouseforeground = "#FF000000" . ToSolidBrush ( );
			DlgInput . Btnborder = "#C1000000" . ToSolidBrush ( );
			DlgInput . btnforeground = "#FF000000" . ToSolidBrush ( );
			DlgInput . btnbackground = "#9DFFFFFB" . ToSolidBrush ( );
			DlgInput . iconstring = "";
			DlgInput . image = null;
		}

		private void GetDefaultMsgboxColors ( )
		{
			// setup "ground zero" colors for my message boxes
			//Messagebox Background
			DlgInput . dlgbackground = Utils . GetNewBrush ( "#50FBFFF6" );          // light gray

			// Buttons configuration
			// Button Background
			DlgInput . btnforeground = Utils . GetNewBrush ( "#FF000500" );     // v. dark grey
														// Button Foreground
			DlgInput . btnbackground = Utils . GetNewBrush ( "#FFFFFFFF" );     // White
														// Button MouseOver Background
			DlgInput . Btnmousebackground = Utils . GetNewBrush ( "#95848284" );     // Mid grey
														 // Button MouseOver Foreground
			DlgInput . Btnmouseforeground = Utils . GetNewBrush ( "#FFFFFFFF" );           /// White

			// Button Border
			DlgInput . Btnborder = Utils . GetNewBrush ( "#FFFF0000" );      // Red
		}
		private void OnClosing ( object sender , CancelEventArgs e )
		{
		}
		// Very IMPORTANT method that loads all the saved values for my messageboxes  into memory
		// filling boh DlgInput and the 6 Attached properties
		// (BkGround, Foreground, MouseoverBackGround, MouseoverForeGround, BorderColor,  BorderSize)
		// So we only need to do this once, unless of course they are changed
		public static void ReadMsgboxData ( )
		{
//			SolidColorBrush brush;
//			bool b = false;
//			int indx=0;
//			string input="";
//			string[] fields;
//			Thickness th = new Thickness();
//			Console . WriteLine ( $"Processing ReadMsgboxData in MainWindow...." );
//			input = File . ReadAllText ( @"Messageboxes.dat" );
//			fields = input . Split ( '\n' );
//			foreach ( var item in fields )
//			{
//				switch ( indx++ )
//				{
//					case 0:
//						DlgInput . bground = item . ToSolidBrush ( );
//						break;
//					case 1:
//						DlgInput . fground = item . ToSolidBrush ( );
//						break;
//					case 2:
////						SetCurrentValue ( BkgroundProperty , item );
//						Msgbox . SetBkGround ( msgbox , item . ToSolidBrush ( ) );
//						if ( item == "" )
//							DlgInput . bbackground = "#9DFFFFFB" . ToSolidBrush ( );
//						else
//							DlgInput . bbackground = item . ToSolidBrush ( );
//						break;
//					case 3:
//						Msgbox . SetForeGround ( msgbox , item . ToSolidBrush ( ) );
//						if ( item == "" )
//							DlgInput . bforeground = "#FFFFFFFF" . ToSolidBrush ( );
//						else
//							DlgInput . bforeground = item . ToSolidBrush ( );
//						break;
//					case 4:
//						Msgbox . SetBorderColor ( msgbox , item . ToSolidBrush ( ) );
//						if ( item == "" )
//							DlgInput . bbackground = "#FFFF0000" . ToSolidBrush ( );
//						else
//							DlgInput . bbackground = item . ToSolidBrush ( );
//						break;
//					case 5:
//						Msgbox . SetMouseoverBackGround ( msgbox , item . ToSolidBrush ( ) );
//						if ( item == "" )
//							DlgInput . bbackground = "#9DFFFFFB" . ToSolidBrush ( );
//						else
//							DlgInput . bbackground = item . ToSolidBrush ( );
//						break;
//					case 6:
//						Msgbox . SetMouseoverForeGround ( msgbox , item . ToSolidBrush ( ) );
//						if ( item == "" )
//							DlgInput . bbackground = "#FFFFFFFF" . ToSolidBrush ( );
//						else
//							DlgInput . bbackground = item . ToSolidBrush ( );
//						break;
//					case 7:
//						th = new Thickness ( );
//						string[] s = item.Split(',');
//						int inx =  0;
//						foreach ( var d in s )
//						{
//							if ( inx == 0 )
//								th . Left = Convert . ToInt32 ( d );
//							else if ( inx == 1 )
//								th . Top = Convert . ToInt32 ( d );
//							else if ( inx == 2 )
//								th . Right = Convert . ToInt32 ( d );
//							else if ( inx == 3 )
//								th . Bottom = Convert . ToInt32 ( d );
//							inx++;
//						}
//						Msgbox . SetBorderSize ( msgbox , th );
//						DlgInput . BorderSize = th;
//						break;
//					case 8:
//						DlgInput . UseIcon = item == "T" ? true : false;
//						break;
//					case 9:
//						DlgInput . UseDarkMode = item == "T" ? true : false;
//						break;
//					case 10:
//						DlgInput . isClean = item == "T" ? true : false;
//						break;
//				}
//			}

//			Console . WriteLine ( $"\nMAINWINDOW.READMSGBOXDATA() : Msgbox Data read in from disk ....\n"
//				+ $"Btn Background :			[{Msgbox . GetBkGround ( msgbox )}]\n"
//				+ $"Btn Foreground :			[{Msgbox . GetForeGround ( msgbox )}]\n"
//				+ $"Btn Mouseover Background :	[{Msgbox . GetMouseoverBackGround ( msgbox )}]\n"
//				+ $"Btn Mouseover Foreground :	[{Msgbox . GetMouseoverForeGround ( msgbox )}]\n"
//				+ $"Btn Border :				[{Msgbox . GetBorderColor ( msgbox )}]\n"
//				+ $"Btn Border Size :			[{th . Top},{th . Left},{th . Right},{th . Bottom}]\n\n"
//				);
		}

		public static void ShowAPDatatoConsole ( )
		{
			//Thickness th = new Thickness();
			//th = Msgbox . GetBorderSize ( msgbox );
			//string output=$"\nMAINWINDOW.READMSGBOXDATA() : \nMsgbox Data read in from disk ....\n"
			//		+ $"Btn Background :			[{Msgbox . GetBkGround ( msgbox )}]\n"
			//		+ $"Btn Foreground :			[{Msgbox . GetForeGround ( msgbox )}]\n"
			//		+ $"Btn Mouseover Background :	[{Msgbox . GetMouseoverBackGround ( msgbox )}]\n"
			//		+ $"Btn Mouseover Foreground :	[{Msgbox . GetMouseoverForeGround ( msgbox )}]\n"
			//		+ $"Btn Border :				[{Msgbox . GetBorderColor ( msgbox )}]\n"
			//		+ $"Btn Border Size :			[{th . Top},{th . Left},{th . Right},{th . Bottom}]\n\n";	
			//Console . WriteLine ( output );
			// output=$"\nMAINWINDOW.READMSGBOXDATA() : \nMsgbox Data read in from disk ....\n"
			//		+ $"Btn Background :			[{Msgbox . GetBkGround ( msgbox )}]\n"
			//		+ $"Btn Foreground :			[{Msgbox . GetForeGround ( msgbox )}]\n"
			//		+ $"Btn Mouseover Background :	[{Msgbox . GetMouseoverBackGround ( msgbox )}]\n"
			//		+ $"Btn Mouseover Foreground :	[{Msgbox . GetMouseoverForeGround ( msgbox )}]\n"
			//		+ $"Btn Border :			[{Msgbox . GetBorderColor ( msgbox )}]\n"
			//		+ $"Btn Border Size :			[{th . Top},{th . Left},{th . Right},{th . Bottom}]\n\n";
			//MessageBox . Show ( output );
		}

		public static void SaveMsgboxData ( Window win )
		{
			//SolidColorBrush brush;
			//bool b = false;
			//string output="";
			//Console . WriteLine ( $"Processing SaveMsgboxData in MainWindow...." );
			//brush = ( SolidColorBrush ) Msgbox . GetBkGround ( msgbox );
			//output += brush . BrushtoText ( ) + "\n";
			//brush = ( SolidColorBrush ) Msgbox . GetForeGround ( msgbox );
			//output += brush . BrushtoText ( ) + "\n";
			//brush = ( SolidColorBrush ) Msgbox . GetMouseoverBackGround ( msgbox );
			//output += brush . BrushtoText ( ) + "\n";
			//brush = ( SolidColorBrush ) Msgbox . GetMouseoverForeGround ( msgbox );
			//output += brush . BrushtoText ( ) + "\n";
			//brush = ( SolidColorBrush ) Msgbox . GetBorderColor ( msgbox );
			//output += brush . BrushtoText ( ) + "\n";
			//Thickness d =  Msgbox . GetBorderSize ( msgbox );
			//output += d . ToString ( ) + "\n";

			//b = DlgInput . UseIcon;
			//output += ( b == true ? "T" : "F" ) + "\n";
			//b = DlgInput . UseDarkMode;
			//output += ( b == true ? "T" : "F" ) + "\n";
			//b = DlgInput . isClean;
			//output += ( b == true ? "T" : "F" ) + "\n";
			//File . WriteAllText ( @"Messageboxes.dat" , output );
			//Console . WriteLine ( $"MainWindow - Data saved to disk ....\n"
			//	+ $"Btn Background :			[{Msgbox . GetBkGround ( win )}]\n"
			//	+ $"Btn Foreground :			[{Msgbox . GetForeGround ( win )}]\n"
			//	+ $"Btn Mouseover Background :	[{Msgbox . GetMouseoverBackGround ( win )}]\n"
			//	+ $"Btn Mouseover Foreground :	[{Msgbox . GetMouseoverForeGround ( win )}]\n"
			//	+ $"Btn Border :				[{Msgbox . GetBorderColor ( win )}]\n"
			//	+ $"Btn Border Size :			[{d . Top}, {d . Left},{d . Right},{d . Bottom}],\n\n"
			//	);

		}
		private void Loaded_click ( object sender , RoutedEventArgs e )
		{
			MainPageHolder . NavigationService . Navigate ( MainWindow . _Blank );
			Utils . SetupWindowDrag ( this );
		}
		private void DoDragMove ( )
		{
			//Handle the button NOT being the left mouse button
			// which will crash the DragMove Fn.....
			MouseButtonState mbs =   Mouse. RightButton ;
			if (mbs == MouseButtonState.Pressed)
				return;
			try
			{
				this . DragMove ( );
			}
			catch
			{
				return;
			}
		}
		//Declare a Property variable to prove Slider changes work both ways
		private int _boundNumber;

		// Now on with other code


		public event PropertyChangedEventHandler PropertyChanged;
		public void MainWindowLoaded ( object sender , RoutedEventArgs e )
		{
		}
		private void Page0_Click ( object sender , RoutedEventArgs e )
		{
			MainPageHolder . NavigationService . Navigate ( MainWindow . _Page0 );
		}
		private void Page1_Click ( object sender , RoutedEventArgs e )
		{
			//Button btn = (Button)sender;
			//btn.FontSize = 28;
			//		MainPageHolder . NavigationService . Navigate ( MainWindow . _Page1 );
		}
		private void Page2_Click ( object sender , RoutedEventArgs e )
		{
			//			MainPageHolder . NavigationService . Navigate ( MainWindow . _Page2 );
		}
		private void Page3_Click ( object sender , RoutedEventArgs e )
		{
			//			MainPageHolder . NavigationService . Navigate ( MainWindow . _Page3 );
		}
		private void Page4_Click ( object sender , RoutedEventArgs e )
		{
			//			MainPageHolder . NavigationService . Navigate ( MainWindow . _Page4 );
		}
		private void Page5_Click ( object sender , RoutedEventArgs e )
		{
			//			MainPageHolder . NavigationService . Navigate ( MainWindow . _Page5 );
		}

		private void Page6_Click ( object sender , RoutedEventArgs e )
		{
			//	this allows the loading of up to 10 different Db Viewer Windows
			//	and to select between them if needed

			// first we have some preapration to get done with pointers tpo the various Classes we are going to access

			// setup global STATIC pointers to Viewmodels
			dvm = new DetailsViewModel ( );
			;
			cvm = new CustomerViewModel ( );
			bvm = new BankAccountViewModel ( );

			// Ok housekeeping over, lets go

			int selected = 0;
			if ( MainWindow . gv . DbSelectorWindow != null )
			{
				if ( MainWindow . gv . DbSelectorWindow . ViewersList . Items . Count > 0 )
				{
					// Been opened before, so just show it again
					MainWindow . gv . DbSelectorWindow . Visibility = Visibility . Visible;
					if ( Flags . DbSelectorOpen . ViewersList . Items . Count > 1 )
					{
						Flags . DbSelectorOpen . ViewerDeleteAll . IsEnabled = true;
						Flags . DbSelectorOpen . ViewerDelete . IsEnabled = true;
						Flags . DbSelectorOpen . SelectBtn . IsEnabled = true;
					}

					MainWindow . gv . DbSelectorWindow . Focus ( );
					MainWindow . gv . DbSelectorWindow . BringIntoView ( );
					MainWindow . gv . DbSelectorWindow . Focus ( );
				}
				Mouse . OverrideCursor = Cursors . Arrow;
				return;
			}
			else
			{
				//Create a new Db Selector Window system.
				DbSelector dbss = new DbSelector ( );
				gv . DbSelectorWindow = dbss;
				dbs = dbss;

				dbs . Show ( );

				//Store the "Handle" to this Db Selector window
				Flags . DbSelectorOpen = dbs;

				if ( MainWindow . sysmenu != null )
				{
					dbs . Visibility = Visibility . Collapsed;
					dbs. Visibility = Visibility . Hidden;
				}

				// Load and display a new viewer for the selected Db Type
				// (returned in the selected var from dbSelector window)
				Mouse . OverrideCursor = Cursors . Wait;
				//				tw = new SqlDbViewer (selected);
				if ( Autoload )
				{

					// find first blank entry of the 10 available slots we have
					// and save our details into it
					for ( int x = 0 ; x < MainWindow . gv . MaxViewers ; x++ )
					{
						if ( gv . window [ x ] == null )
						{
							//						gv.SelectedViewerType = selected; // store the Db type in our "current" viewer type variable
							gv . ViewerSelectiontype = -1;  // reset flag field for next time
							gv . ViewerCount++;

							tw . Show ( );
							tw . Focus ( );
							//Save the Window handle in the Viewer Window itself - Now done in window loaded
							tw . Tag = gv . window [ x ];
							//break;
							break;
						}
					}
				}
				//				Flags.DbSelectorOpen.Show ();
				Mouse . OverrideCursor = Cursors . Arrow;
				//				return;
			}
			//Store window handle to DbSelector window in control structure (GridViewer)
			//			DbSelectorOpen.Show ();

			Mouse . OverrideCursor = Cursors . Arrow;
			return;

		}
		private void ExitButton_Click ( object sender , RoutedEventArgs e ) { App . Current . Shutdown ( ); }

		private void Blank_Click ( object sender , RoutedEventArgs e )
		{
			MainPageHolder . NavigationService . Navigate ( MainWindow . _Blank );
		}

		private void Main_PreviewKeyDown ( object sender , KeyEventArgs e )
		{
			if ( e . Key == Key . RightCtrl || e . Key == Key . Home || e . Key == Key . Enter )
			{
				Page6_Click ( sender , null );
				this . Hide ( );
			}
			else if ( e . Key == Key . OemQuotes )
			{
				EventHandlers . ShowSubscribersCount ( );
				key1 = false;
			}
			else if ( e . Key == Key . LeftCtrl )
				key1 = true;
			else if ( e . Key == Key . RWin )
			{
				if ( key1 )
				{
					Flags . ShowAllFlags ( );
					key1 = false;
				}
			}
			else if ( e . Key == Key . Escape )
				Application . Current . Shutdown ( );

		}

		private void GrabScreenObject( object sender , KeyEventArgs e )
		{
			if ( e . Key == Key . RightCtrl || e . Key == Key . Home || e . Key == Key . Enter )
			{
				Page6_Click ( sender , null );
				this . Hide ( );
			}
			else if ( e . Key == Key . OemQuotes )
			{
				EventHandlers . ShowSubscribersCount ( );
				key1 = false;
			}
			else if ( e . Key == Key . LeftCtrl )
				key1 = true;
			else if ( e . Key == Key . RWin )
			{
				if ( key1 )
				{
					Flags . ShowAllFlags ( );
					key1 = false;
				}
			}
			else if ( e . Key == Key . Escape )
				Application . Current . Shutdown ( );

		}


		private void OntopChkbox_Click ( object sender , RoutedEventArgs e )
		{
			bool? setting = OntopChkbox . IsChecked;
			OntopChkbox . IsChecked = setting;
			Topmost = ( bool ) setting;
		}
	}
}
