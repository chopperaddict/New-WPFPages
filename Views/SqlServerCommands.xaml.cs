
using System;
using System . Collections;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . ComponentModel;
using System . Data;
using System . Data . SqlClient;
using System . Diagnostics;
using System . IO;
using System . Linq;
using System . Runtime . Serialization . Formatters . Binary;
using System . Text;
using System . Threading . Tasks;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Input;
using System . Windows . Media;
using System . Windows . Media . Imaging;

using WPFPages . ViewModels;

using static Microsoft . SqlServer . Management . Sdk . Sfc . RequestObjectInfo;
using System . Windows . Documents;
using System . Security . AccessControl;

namespace WPFPages . Views
{
	public partial class SqlServerCommands : Window
	{
		public ObservableCollection<GenericClass> Generics = new ObservableCollection<GenericClass>();
		public DapperClass Db = new DapperClass ();
		public List<string> ReceivedDbData = new List<string>();

		public delegate bool LinqDelegate<T, U> ( T s1 , U s2 );
		public delegate bool LinqDelegate1<T, U> ( T s1 , U s2 );
		public delegate bool LinqDelegate2<T> ( T s1 );
		public delegate bool LinqDelegate3<T, U, V> ( T s1 , U s2 , V s3 );

		public static FullGridViewer fgviewer;
		private string[] datastring =
			{
				"3, 4193116, 1055023, 1, 3476.65, 1.27, '1961/04/13', '2000/01/01'",
				"9, 4188178, 1055028, 2, 8636.12, 2.31, '1950/10/27', '1998/04/14'",
				"10, 4188181, 1055033, 2, 12581.50, 2.18, '1952/10/27', '2000/01/01'",
				"11, 4188183, 1055033, 2, 23778.74, 1.50, '1952/04/27', '2000/01/01'",
				"14, 4188186, 1055036, 2, 12345.67, 8.67, '1953/10/06', '2001/05/15'",
				"16, 4188188, 1055038, 2, 25098.94, 8.67, '1995/04/28', '2000/01/01'",
				"23, 4188195, 1055043, 4, 259.72, 2.31, '1996/11/15', '2000/01/01'",
				"27, 4188199, 1055047, 1, 4361.66, 8.67, '1957/05/01', '2000/01/01'",
				"28, 4188200, 1055048, 3, 41816.11, 1.30, '1958/11/15', '2000/01/01'",
				"30, 4188202, 1055050, 2, 1899.29, 4.98, '1987/03/25', '2000/01/01'",
				"33, 4188205, 1055052, 3, 2701.44, 11.40, '1946/03/25', '2000/01/01'",
				"34, 4188206, 1055056, 1, 26278.02, 8.08, '1948/09/11', '1996/04/27'",
				"35, 4188207, 1055057, 2, 9518.09, 2.18, '1988/03/12', '1993/10/27'",
				"38, 4188210, 1055057, 4, 10467.66, 9.57, '1990/04/26', '2000/01/01'",
				"36, 4188208, 1055058, 4, 8961.27, 7.08, '1948/04/26', '1995/04/13' ",
				"39, 4188211, 1055058, 1, 22344.61, 1.75, '1949/10/26', '2000/01/01'"
			} ;
		public string errorstring = "";
		public static string SqlCommand="";
		public static string OriginalSqlCommand="";
		public static string ActiveDbName="";
		public static string CurrentDbName="";
		public static bool ShowGridFlag = false;
		private static bool ProcessSp= false;
		private bool showall=false;

		// Cookie declarations
		public static Uri cookieUri { get; set; }
		public static string cookieKey { get; set; }
		public static string cookieValue { get; set; }
		public static string cookieComment { get; set; }

		public static  List<string> CookiesList = new List<string>();
		//********************************************************************************************************************************************************************************//
		protected override void OnInitialized ( EventArgs e )
		//********************************************************************************************************************************************************************************//
		{
			//if ( DesignerProperties . GetIsInDesignMode ( this ) == true )
			//	LoadListView ( );
			//else
			//	LoadListView ( );



			base . OnInitialized ( e );
		}
		//********************************************************************************************************************************************************************************//
		private void Window_Loaded ( object sender , RoutedEventArgs e )
		//********************************************************************************************************************************************************************************//
		{
			//var v = StringLib.StartsWithUpper ( "gsdfgdgfhd" );
			string[] tmp;
			DataContext = this;
			Library1 . SetupWindowDrag ( this );
			// hide grid
			togglevisibility ( false , "" );
			// Populate the Db Tables && Stored Procedures Combo
			LoadTablesCombo ( );
			LoadSPCombo ( );
			// make sure info panel is hidden
			// Subclass the MouseMove event
			MouseMove += Grab_MouseMove;
			Cookies . LoadCookiesToCombo ( CookiesCombo , out CookiesList );
			DisplayGrid . ItemsSource = null;
			DisplayGrid . Visibility = Visibility . Hidden;
			SetViewButtons ( 1 , ( GridData_Display . Visibility == Visibility . Visible ? true : false ) , ( DisplayGrid . Visibility == Visibility . Visible ? true : false ) );

		}

		private void Grab_MouseMove ( object sender , MouseEventArgs e )
		{
			if ( e . LeftButton == MouseButtonState . Pressed )
				Utils . Grab_MouseMove ( sender , e );
			e . Handled = true;
		}


		//********************************************************************************************************************************************************************************//
		public static T GetElementUnderMouse<T> ( ) where T : UIElement
			//********************************************************************************************************************************************************************************//
		{
			return Utils . FindVisualParent<T> ( Mouse . DirectlyOver as UIElement );
		}

		//********************************************************************************************************************************************************************************//
		public override void OnApplyTemplate ( )
		//********************************************************************************************************************************************************************************//
		{
			base . OnApplyTemplate ( );
			if ( Template != null )
			{
				var v = this . GetTemplateChild ( "ComboScroll" );
			}
			return;

		}
		//********************************************************************************************************************************************************************************//
		public SqlServerCommands ( )
		//********************************************************************************************************************************************************************************//
		{
			InitializeComponent ( );
			if ( DesignerProperties . GetIsInDesignMode ( this ) == true )
			{
				//				if ( System . Reflection . Assembly . GetExecutingAssembly ( ) . Location . Contains ( "VisualStudio" ) )
				//					LoadListView ( );
			}
		}

		//********************************************************************************************************************************************************************************//
		private async void CopyDbBtn_Click ( object sender , RoutedEventArgs e )
		//********************************************************************************************************************************************************************************//
		{
			// Copy Db to new Db  "Select * from xxx into yyy" using dapper
			CopyDbBtn . Focus ( );

			Mouse . OverrideCursor = Cursors . Wait;
			string donor = DbToCopyCombo.SelectedItem.ToString();
			string recip = DbToBeCreated.Text.Trim();
			ActiveDbName = recip;
			SqlCommand = $"select * into {recip} from {donor}";
			OriginalSqlCommand = SqlCommand;
			string errormsg = "", dbnametoopen="";
			int counter = 0;

			if ( recip . ToUpper ( ) == donor . ToUpper ( ) )
			{
				var dr2 = MessageBox . Show ( $"The source & destination Db are the same !!\nYou cannot copy a Db to another with the same name" ,"Sql Error",MessageBoxButton.OK, MessageBoxImage.Warning);
				Mouse . OverrideCursor = Cursors . Arrow;
				DbToBeCreated . SelectAll ( );
				DbToBeCreated . Focus ( );
				return;
			}
			// Allow Overwrite is NOT checked, so we Need to check if the destination already exists
			OriginalSqlCommand = SqlCommand;
			//SqlCommand = $"select count(*) as [Count] from {recip}";
			CurrentCommandLabel . Text = $"[{SqlCommand}]";
			if ( recip == "Enter Table name..." )
			{
				Mouse . OverrideCursor = Cursors . Arrow;
				Utils . Mbox ( this , string1: $"Please enter a Table name  to copy the [{donor . ToUpper ( )}] Db  to ?" , string2: "" , caption: "" , iconstring: "\\icons\\Information.png" , Btn1: MB . OK , Btn2: MB . CANCEL , defButton: MB . OK );
				DbToBeCreated . SelectAll ( );
				DbToBeCreated . Focus ( );
				return;
			}
			try
			{
				DapperSupport . PerformSqlDbTest ( SqlCommand , out errorstring );
				// reset original SQL command string
				SqlCommand = OriginalSqlCommand;
				if ( errorstring . ToUpper ( ) . Contains ( "THERE IS ALREADY AN" ) )
				{
					var dr2 = MessageBox . Show ( $" There is already a Table named [{donor}],\n\nPress No to enter a different name, or Yes to overwrite the current Table." ,
					"Sql Command",
						MessageBoxButton.YesNo,
						MessageBoxImage.Question);
					IEnumerable DbResult=null;
					if ( dr2 == MessageBoxResult . No )
					{
						// This call perfoms the Copy process
						//int result = DapperSupport . PerformSqlExecuteCommand ( SqlCommand , out errormsg);
						//						ObservableCollection<GenericClass> Generics = new ObservableCollection<GenericClass>();
						ObservableCollection<BankAccountViewModel> bvmparam = new ObservableCollection<BankAccountViewModel>();
						Dictionary <string, object>dict = new Dictionary<string, object>();
						List<string> genericlist = new List<string>();

						try
						{
							DapperSupport . CreateGenericCollection ( ref Generics ,
							$"spCopyDb" ,
							$" {donor} {recip}, 1" ,
							"" ,
							"" ,
							ref genericlist ,
							ref errorstring );
						}
						catch ( Exception ex )
						{
							MessageBox . Show ( $"SQL ERROR 1125 - {ex . Message}" );
							Mouse . OverrideCursor = Cursors . Arrow;
							return;
						}
						CurrentCommandLabel . Text = $"[{SqlCommand . ToUpper ( )}]";
						// Save Db name so we can access it later
						CurrentDbName = recip . ToUpper ( );
						this . Title = $"Sql Server Commands : Active Db : {CurrentDbName}";
						this . Refresh ( );
						// Load the grid and display it
						if ( errorstring == "" )
						{
							LoadShowDbGrid ( recip );
							LoadSPCombo ( );
							CurrentCommandLabel . Text = $"[{SqlCommand}]";
							OriginalSqlCommand = SqlCommand;
							DbCopiedResult . Text = $"{SqlCommand} completed successfully ...";
						}
						else
						{
							if ( errorstring . ToUpper ( ) . Contains ( "SPCOPYDB" ) )
								DbCopiedResult . Text = $"The Stored procedure spCopyDb has\nbeen used to successfully Copy the requested Db  [{donor}]\n by overwriting the Db [{recip}]";
							else
								DbCopiedResult . Text = errorstring;
						}

						OriginalSqlCommand = SqlCommand;
						CurrentCommandLabel . Text = $"[{SqlCommand . ToUpper ( )}]";
					}
					else
					{
						Mouse . OverrideCursor = Cursors . Arrow;
						DbToBeCreated . SelectAll ( );
						DbToBeCreated . Focus ( );
					}
				}
				else
				{
					// Looks like Db does  exist
					if ( errorstring . ToUpper ( ) . Contains ( "VIEWS.GENERICCLASS" ) )
					{
						// YEP, WE GOT A GENERIC dB STRUCTURE BACK, SO IT DOES EXIST
						var dr2 = MessageBox . Show ( $"The destination Db already exists !\nDo you want to go ahead and overwrite it ??" ,
						"Sql Command",
						MessageBoxButton.YesNo,
						MessageBoxImage.Question);

						if ( dr2 == MessageBoxResult . Yes )
						{
							// This call perfoms the Copy process
							try
							{
								Task<int> result =  DapperSupport . PerformSqlExecuteCommandAsync ( SqlCommand ,null);
							}
							catch ( Exception ex )
							{
								MessageBox . Show ( $"SQL ERROR ( SqlServercommand 233) \n[{ex . Message}]" );
								Mouse . OverrideCursor = Cursors . Arrow;
							}

							CurrentCommandLabel . Text = $"[{SqlCommand . ToUpper ( )}]";
							// Save Db name so we can access it later
							CurrentDbName = recip . ToUpper ( );
							if ( errormsg != "" )
							{
								DbCopiedResult . Text = $"Sql Command failed : error = [{errormsg}.]";
							}
							else
							{
								DbCopiedResult . Text = $"{SqlCommand} completed by PerformSqlExecuteCommand() ...";
								// Load the grid and display it
								LoadShowDbGrid ( recip );
								LoadSPCombo ( );
							}
						}
						else
						{
							DbToBeCreated . SelectAll ( );
							DbToBeCreated . Focus ( );
						}
					}
					else if ( errormsg == "" )
					{
						// definitely does NOT exist, so proceed as normal
						// This call perfoms the Copy process
						try
						{
							Task<int> result = DapperSupport . PerformSqlExecuteCommandAsync ( SqlCommand ,null);
						}
						catch ( Exception ex )
						{
							MessageBox . Show ( $"SQL ERROR ( SqlServercommand 264) \n[{ex . Message}]" );
						}
						CurrentCommandLabel . Text = $"[{SqlCommand . ToUpper ( )}]";

						// Save Db name so we can access it later
						CurrentDbName = recip . ToUpper ( );
						// We  null this  so it gets a valid load command
						SqlCommand = "";
						// Load the grid and display it
						LoadShowDbGrid ( recip );
						LoadSPCombo ( );
					}
				}
			}
			catch ( Exception ex )
			{
				MessageBox . Show ( $"SQL ERROR ( CopyDbBtn_Click : 146) \n[{ex . Message}]" );
			}
			Mouse . OverrideCursor = Cursors . Arrow;
			return;
		}
		private void CopyList ( List<string> templist )
		{
			ReceivedDbData . Clear ( );
			foreach ( var item in templist )
			{
				ReceivedDbData . Add ( item );
			}
		}
		//********************************************************************************************************************************************************************************//
		private void LoadShowDbGrid ( string recipient )
		//********************************************************************************************************************************************************************************//
		{
			if ( ReceivedDbData . Count == 0 )
			{                       // creates and loads Db into grid
				List<string> templist = new List<string>();
				try
				{
					templist = DapperSupport . GetGenericCollection ( templist ,
					SqlCommand = SqlCommand == "" ? $"select * from {recipient}" : SqlCommand ,
							 false ,
							 "SqlServerCommands" );
					if ( templist . Count == 0 )
						return;
					CurrentCommandLabel . Text = $"[{SqlCommand}]";
					OriginalSqlCommand = SqlCommand;
					//Copy new data  to global List<string>  ReceivedDbData 
					CopyList ( templist );
					// creates and loads Db into grid
					if ( ReceivedDbData . Count > 0 )
						CreateGenericDatabase ( DisplayGrid , ReceivedDbData );
					DbCopiedResult . Text = $"Db [{SqlCommand}] performed successfully...";
				}
				catch ( Exception ex )
				{
					MessageBox . Show ( $"GetGenericCollection (LoadShowDbGrid : 336) error \n\n[{ex . Message}]" );
				}
			}
			else
			{
				// creates and loads Db into grid  & Displays the grid
				if ( ReceivedDbData . Count > 0 )
					CreateGenericDatabase ( DisplayGrid , ReceivedDbData );
			}
			if ( ShowGridFlag == false )
			{
				// hide grid
				togglevisibility ( false , "" );
				DbCopiedResult . Text = "Copy successful, Db is available for access... ";
				this . Title = $"Sql Server Commands : Db : {recipient}";
				//return;
			}
			else
			{
				DbCopiedResult . Text = "Copy successful, Grid is now open... ";
				BankNameLabel . Text = recipient . ToUpper ( );
				//Show Grid
				togglevisibility ( true , "" );
				this . Title = $"Sql Server Commands : Active Db : {recipient}";
				this . Refresh ( );
			}
			//Clear Command buffers ready for next operation
			OriginalSqlCommand = "";
			SqlCommand = "";
		}
		//********************************************************************************************************************************************************************************//
		private void Closegrid_Click ( object sender , RoutedEventArgs e )
		//********************************************************************************************************************************************************************************//
		{
			// hide grid
			togglevisibility ( false , "" );

		}
		//********************************************************************************************************************************************************************************//
		private void ShowGrid_Click ( object sender , RoutedEventArgs e )
		//********************************************************************************************************************************************************************************//
		{
			bool flagstatus = ShowGridFlag;
			//ShowGrid . Focus ( );
			if ( DisplayGrid . Visibility == Visibility . Visible )
			{
				// grid  visible
				// hide grid
				DisplayGrid . Visibility = Visibility . Hidden;
				SetViewButtons ( 1 , ( GridData_Display . Visibility == Visibility . Visible ? true : false ) , ( DisplayGrid . Visibility == Visibility . Visible ? true : false ) );
				UpdateUniversalViewer ( );
				//togglevisibility ( false , "" );
				//if ( GridData_Display . Visibility == Visibility . Hidden )
				//	GridData_Display . Visibility = Visibility . Visible;
				e . Handled = true;
				return;
			}
			else
			{
				// grid  hidden - show it
				//togglevisibility ( true );

				if ( DisplayGrid . Items . Count == 0 )
				{
					// gotta load it first, so set flag to let us as it may be disabled on the screen
					ShowGridFlag = true;
					LoadShowDbGrid ( CurrentDbName );
					//Reset grid visibility flag
					ShowGridFlag = flagstatus;
					ClearGrid . IsEnabled = true;
					SetDumyGridRow ( DisplayGrid );
					DisplayGrid . Visibility = Visibility . Visible;
				}
				else
				{
					//					GridData_Display . Visibility = Visibility . Hidden;
					DisplayGrid . Visibility = Visibility . Visible;
				};

				UpdateUniversalViewer ( );
				BankNameLabel . Text = CurrentDbName;
				SetViewButtons ( 1 , ( GridData_Display . Visibility == Visibility . Visible ? true : false ) , ( DisplayGrid . Visibility == Visibility . Visible ? true : false ) );
				togglevisibility ( true );
				DisplayGrid . Refresh ( );
				DisplayGrid . Focus ( );
			}
			e . Handled = true;
		}
		//********************************************************************************************************************************************************************************//
		private void DeleteRecipientDbBtn_Click ( object sender , RoutedEventArgs e )
		//********************************************************************************************************************************************************************************//
		{
			DapperSupport . DeleteDbTable ( DbToBeCreated . Text );
			//DeleteSource . Opacity = 0.3;
			//MessageBox . Show ( $"The Db should have been deleted., You can now try the Copy button again");

		}
		//********************************************************************************************************************************************************************************//
		private void createstringarray ( )
		//********************************************************************************************************************************************************************************//
		{
			List<string> strlist = new List<string>();
			string s = "dsg dfgd";
			strlist . Add ( s );
		}
		//********************************************************************************************************************************************************************************//
		private void DisplayGrid_MouseDblClick ( object sender , System . Windows . Input . MouseButtonEventArgs e )
		//********************************************************************************************************************************************************************************//
		{
			string buffer="";
			GridData_Display . Visibility = Visibility . Visible;
			//DisplayGrid . Visibility = Visibility . Hidden;
			SetViewButtons ( 2 , ( GridData_Display . Visibility == Visibility . Visible ? true : false ) , ( DisplayGrid . Visibility == Visibility . Visible ? true : false ) );
			buffer = DisplayGrid . SelectedItem?.ToString ( );
			if ( buffer == null )
				return;

			if ( buffer . Contains ( "field1" ) == false )
				GridData_Display . Text = buffer;
			else
			{
				string[] fields = buffer.Split(',');
				buffer = "";
				foreach ( var item in fields )
				{
					if ( buffer == "" )
						buffer = item . Substring ( 1 ) + "\n";
					else
						buffer += item + "\n";
				}
				GridData_Display . Text = buffer;
			}
		}
		//********************************************************************************************************************************************************************************//
		private void togglevisibility ( bool mode , string DbName = "" )
		//********************************************************************************************************************************************************************************//
		{
			if ( DbName == "" )
				DbName = "???";
			if ( mode )
			{
				// show grid
				GridCount . Visibility = Visibility . Visible;
				NameLabel . Visibility = Visibility . Visible;
				BankNameLabel . Visibility = Visibility . Visible;
				GridCount . Text = DisplayGrid . Items . Count . ToString ( );
				BankNameLabel . Text = DbName;
				//				DisplayGrid . Visibility = Visibility . Visible;
				//				SetViewButtons ( 1 , ( GridData_Display . Visibility == Visibility . Visible ? true : false ) , ( DisplayGrid . Visibility == Visibility . Visible ? true : false ) );
			}
			else
			{
				// hide grid
				GridCount . Visibility = Visibility . Hidden;
				NameLabel . Visibility = Visibility . Hidden;
				BankNameLabel . Visibility = Visibility . Hidden;
				//				DisplayGrid . Visibility = Visibility . Hidden;
				//				SetViewButtons ( 1 , ( GridData_Display . Visibility == Visibility . Visible ? true : false ) , ( DisplayGrid . Visibility == Visibility . Visible ? true : false ) );
			}
			Utils . trace ( );
		}

		/// <summary>
		/// checks the full set of SP's in SQL Server agains a received argument and returns true if identified
		///  used for validating the typoe of SQL command a user may have entered  (select/ existing SP/ a.n. other)
		/// </summary>
		/// <param name="sp"></param>
		/// <returns></returns>
		//********************************************************************************************************************************************************************************//
		static public bool CheckforStoredProcedure ( string sp )
		//********************************************************************************************************************************************************************************//
		{
			bool result = false;
			string spUpper = sp . ToUpper ( );
			ObservableCollection <GenericClass>Checklist= new ObservableCollection<GenericClass>();
			SqlCommand = "spGetStoredProcs";
			SqlServerCommands ssc  = new SqlServerCommands ();
			ssc . ExecuteStoredProcedure ( SqlCommand , Checklist , "" , "" , null );
			foreach ( var item in Checklist )
			{
				if ( item . field1 . ToUpper ( ) == spUpper )
				{
					result = true;
					break;
				}
			}
			return result;
		}
		//********************************************************************************************************************************************************************************//
		public void GetSPCheckList ( string sqlcommand )
		//********************************************************************************************************************************************************************************//
		{
			ObservableCollection <GenericClass>Checklist= new ObservableCollection<GenericClass>();
			string originalcommand = sqlcommand;
			if ( SP_checklist . Count == 0 )
			{
				//SqlCommand = "spGetStoredProcs";
				ExecuteStoredProcedure ( "spGetStoredProcs" , Checklist , "" , "" , null );
				foreach ( var item in Checklist )
				{
					SP_checklist . Add ( item . field1 . ToUpper ( ) );
				}
				//SqlCommand = OriginalSqlCommand;
			}
		}
		//********************************************************************************************************************************************************************************//
		private async void ExecCommand_Click ( object sender , RoutedEventArgs e )
		//********************************************************************************************************************************************************************************//
		{
			Task<int> result;

			// Check for it being an SP in the text field - cever eh ?
			GetSPCheckList ( SqlCommand );
			if ( CheckForSPCommand ( SqlCommandString . Text ) )
			{
				SP_checklist . Clear ( );
				DbCopiedResult . Text = $"SQL query [{SqlCommand}] failed to be executed....";
				Mouse . OverrideCursor = Cursors . Arrow;
				return;
			}
			ExecSqlCommand . Focus ( );
			// Button to Execute user entered SQL query
			// may be called to check for existence of a Db
			string DbName = SqlCommandString.Text;
			string upperstring = DbName. ToUpper ( );
			if ( upperstring . Contains ( "ENTER ARGUMENTS HERE" ) )
			{
				Mouse . OverrideCursor = Cursors . Arrow;
				MessageBox . Show ( $"You must enter a valid SQL query in the ' SQL command '  field...'" , "User entry Error !" , MessageBoxButton . OK );
				return;
			}
			if ( upperstring . Contains ( "SELECT " ) == false )
			{
				Mouse . OverrideCursor = Cursors . Wait;
				if ( upperstring . Contains ( " INTO " ) )
				{
					// NEED TO TEST FOR EXISTENCE OF THIS DB
					string[] s = upperstring.Split ( ' ' );
					for ( int x = 0 ; x < s . Length ; x++ )
					{
						if ( s [ x ] . Trim ( ) == "INTO" )
						{
							DbName = s [ x + 1 ];
							break;
						}
					}
					if ( DbName . Length > 0 )
					{
						SqlCommand = $"Select * from {DbName}";
						CurrentCommandLabel . Text = $"[{SqlCommand}]";
					}

					// Test  for validity
					//************************************************************************************************************************************************************************//
					var queryresult = DapperSupport.PerformSqlDbTest(SqlCommand, out errorstring);
					//************************************************************************************************************************************************************************//

					if ( errorstring != null )
					{
						if ( errorstring . ToUpper ( ) . Contains ( "INVALID OBJECT" ) && errorstring . ToUpper ( ) . Contains ( DbName . ToUpper ( ) ) )
						{
							Console . WriteLine ( "Db does NOT exist" );
							SqlCommand = OriginalSqlCommand;
							try
							{
								//************************************************************************************************************************************************************************//
								result = DapperSupport . PerformSqlExecuteCommandAsync ( SqlCommand , null );
								//************************************************************************************************************************************************************************//
								Console . WriteLine ( $"Result returned from Sql Server was  {result}" );
							}
							catch ( Exception ex )
							{
								MessageBox . Show ( $"SQL ERROR ( SqlServercommand 482 \n[{ex . Message}]" );
								Mouse . OverrideCursor = Cursors . Arrow;
							}
						}
					}
					else
					{
						CurrentCommandLabel . Text = SqlCommandString . Text;
						errorstring = "";
					}
				}
				else
				{
					// processing unknown command
					if ( CheckForSPCommand ( SqlCommandString . Text ) == false )
					{
						//						MessageBox . Show ( $"The command you have entered\n\n{SqlCommandString . Text }]\n\nhas not been recognized");
						Mouse . OverrideCursor = Cursors . Arrow;
						DlgInput . Btnmouseforeground = FindResource ( "Black0" ) as Brush;
						Mouse . OverrideCursor = Cursors . Arrow;
						Utils . Mbox ( this , string1: $"The command you have entered\n[{SqlCommandString . Text }]\nhas not been recognized" , string2: "This is the footr string " , caption: "Tis is  the caption string" , iconstring: "" , Btn1: 1 , Btn2: 0 , defButton: 1 );
						DbCopiedResult . Text = $"SQL command\n[{SqlCommandString . Text} failed to execute...";
						//  						MessageBox . Show ("fdgffhhdhh","",MessageBoxButton.OK,MessageBoxImage.Error);
					}
				}
			}
			else
			{
				//processing a user entered cmd containing at lease SELECT 
				SqlCommand = SqlCommandString . Text;
				try
				{
					//					ObservableCollection<GenericClass> generics = new ObservableCollection<GenericClass>();
					Dictionary <string, object>dict = new     Dictionary <string, object>();
					ObservableCollection<BankAccountViewModel> bvmparam = new  ObservableCollection<BankAccountViewModel>();
					List<string> genericlist = new List<string>     ();
					string errormsg="";

					// Return a list<string> from any "Select " query
					DapperSupport . CreateGenericCollection (
					ref Generics ,
					SqlCommand ,
					"" ,
					"" ,
					"" ,
					ref genericlist ,
					ref errormsg );
					// We want a list<string> from this call

					if ( Generics . Count == 0 )
					{
						SetDumyGridRow ( DisplayGrid );
					}
					else
					{
						if ( errormsg == "" )
						{
							DisplayGrid . ItemsSource = null;
							DisplayGrid . Items . Clear ( );
							// Caution : This is a powerful method that loads the List<string> data into the Datagrid with only the selected rows
							// //visible in the grid so do NOT repopulate the grid after making this call
							CreateGenericDatabase ( DisplayGrid , genericlist );
						}
						else
						{
							Mouse . OverrideCursor = Cursors . Arrow;
							Utils . Mbox ( this , string1: $"SQL processing error: \n[{errormsg}]\n" , string2: "hey ho" , caption: "SqlError" , Btn1: mb . OK , defButton: mb . OK , iconstring: "" );
						}
					}
				}
				catch ( Exception ex )
				{
					MessageBox . Show ( $"SQL ERROR ( SqlServercommand 496) \n[{ex . Message}]" );
				}
				Mouse . OverrideCursor = Cursors . Arrow;
				return;
			}
		}

		/// <summary>
		/// Get the Db Name from the SQL command string (if possible)
		/// </summary>
		/// <param name="Sqlcommand"></param>
		/// <returns>Db Name as string</returns>		 		
		//********************************************************************************************************************************************************************************//
		private string GetDbNameFromCommand ( string Sqlcommand )
		//********************************************************************************************************************************************************************************//
		{
			string output="";
			bool flag = false;
			string[] temp=Sqlcommand.ToUpper().Split(' ');
			foreach ( var item in temp )
			{
				if ( item . Contains ( "FROM" ) )
				{
					flag = true;
					continue;
				}
				if ( flag )
				{
					output = item;
					break;
				}
			}
			return output;
		}
		//********************************************************************************************************************************************************************************//
		private void SqlCommandString_PreviewMouseLeftButtonUp ( object sender , System . Windows . Input . MouseButtonEventArgs e )
		//********************************************************************************************************************************************************************************//
		{
			// Select entire string on entry into the field to make it easier to type an entry
			ResetFieldDefaults ( sender as TextBox , true );
			SqlCommandString . Focus ( );
		}
		//********************************************************************************************************************************************************************************//
		private void DbToCopy_PreviewMouseLeftButtonUp ( object sender , System . Windows . Input . MouseButtonEventArgs e )
		//********************************************************************************************************************************************************************************//
		{
			//Utils.SelectTextBoxText ( DbToCopyTb );
		}

		//********************************************************************************************************************************************************************************//
		private List<string>SP_checklist = new List<string>();
		private void DbToBeCreated_PreviewMouseLeftButtonUp ( object sender , System . Windows . Input . MouseButtonEventArgs e )
		//********************************************************************************************************************************************************************************//
		{
			//SelectTextBoxText ( DbToBeCreated );
			ResetFieldDefaults ( sender as TextBox , true );
			GetSPCheckList ( SqlCommand );
		}
		//********************************************************************************************************************************************************************************//
		private void SqlCommandString_KeyDown ( object sender , System . Windows . Input . KeyEventArgs e )
		//********************************************************************************************************************************************************************************//
		{
			if ( e . Key == Key . Enter )
			{
				if ( CheckForSPCommand ( SqlCommandString . Text ) )
				{
					SP_checklist . Clear ( );
					DbCopiedResult . Text = $"SQL command\n[{SqlCommandString . Text} failed to execute...";
					e . Handled = true;
					return;
				}
				SqlCommand = SqlCommandString . Text;
				OriginalSqlCommand = SqlCommand;
				ExecCommand_Click ( this , null );
				e . Handled = true;
			}
		}
		//********************************************************************************************************************************************************************************//
		public bool CheckForSPCommand ( string SqlCommandstring )
		//********************************************************************************************************************************************************************************//
		{
			ObservableCollection <GenericClass>Checklist= new ObservableCollection<GenericClass>();
			GetSPCheckList ( SqlCommandstring );
			if ( SP_checklist . Contains ( SqlCommandstring . ToUpper ( ) ) )
			{
				MessageBox . Show ( $"The command you have entered \n\n            [{SqlCommandString . Text . ToUpper ( )}].\n\nis an existing Stored Procedure.\n\nYou can run these using the option below" );
				SP_checklist . Clear ( );
				DbCopiedResult . Text = $"SQL command\n[{SqlCommand} failed to execute...";
				return true;
			}
			return false;
		}
		//********************************************************************************************************************************************************************************//
		private void ClearGrid_Click ( object sender , RoutedEventArgs e )
		//********************************************************************************************************************************************************************************//
		{
			ClearGrid . Focus ( );

			SetDumyGridRow ( DisplayGrid );
			BankNameLabel . Text = "";
			GridCount . Text = "";
			this . Title = $"Sql Server Commands : Active Db : 'None'";
			//			ClearGrid . IsEnabled = false;
			//			ShowInGrid . IsEnabled = true;
			// cant show procs if grid is cleared as we do not have a Db remaining
			ActiveDbName = "";
			// delete last enquiry
			OriginalSqlCommand = "";
			SqlCommand = "";
			CurrentCommandLabel . Text = "";
			togglevisibility ( false );
		}
		//********************************************************************************************************************************************************************************//
		public void SetDumyGridRow ( DataGrid Grid )
		//********************************************************************************************************************************************************************************//
		{
			//			ObservableCollection<GenericClass> db= new ObservableCollection<GenericClass>();
			GenericClass gc = new GenericClass();
			Grid . ItemsSource = null;
			Grid . Items . Clear ( );
			Grid . Refresh ( );
			gc . field1 = "       No Current Selection ...       ";
			Generics . Clear ( );
			Generics . Add ( gc );
			LoadActiveRowsOnlyInGrid ( Grid , Generics , 1 );
			UpdateUniversalViewer ( );
		}

		#region LoadDbGrid
		/// <summary>
		/// Creates a GenericClass collection and can also populte a datagrid  if LoadGrid==true
		/// and returns the collection to the caller
		/// </summary>
		/// <param name="dgrid"></param>
		/// <param name="ReceivedDbData"></param>
		/// <param name="LoadGrid"></param>
		/// <returns></returns>
		//********************************************************************************************************************************************************************************//
		public ObservableCollection<GenericClass> CreateGenericDatabase ( DataGrid dgrid , List<string> ReceivedDbData , bool LoadGrid = true )
		//********************************************************************************************************************************************************************************//
		{
			string datain="";
			int totalfields = 0;
			// Post process data string received 
			//			ObservableCollection<GenericClass> genericcollection = new ObservableCollection<GenericClass>();
			Generics . Clear ( );
			for ( int x = 0 ; x < ReceivedDbData . Count ; x++ )
			{
				datain = ReceivedDbData [ x ];
				string[] fields = datain.Split(',');
				totalfields = fields . Length;
				GenericClass genclass = new GenericClass();
				for ( int z = 0 ; z < fields . Length ; z++ )
				{
					string[] inner = fields[z].Split('=');
					try
					{
						switch ( z + 1 )
						{
							case 1:
								genclass . field1 = inner [ 1 ];
								break;
							case 2:
								genclass . field2 = inner [ 1 ];
								break;
							case 3:
								genclass . field3 = inner [ 1 ];
								break;
							case 4:
								genclass . field4 = inner [ 1 ];
								break;
							case 5:
								genclass . field5 = inner [ 1 ];
								break;
							case 6:
								genclass . field6 = inner [ 1 ];
								break;
							case 7:
								genclass . field7 = inner [ 1 ];
								break;
							case 8:
								genclass . field8 = inner [ 1 ];
								break;
							case 9:
								genclass . field9 = inner [ 1 ];
								break;
							case 10:
								genclass . field10 = inner [ 1 ];
								break;
							case 11:
								genclass . field11 = inner [ 1 ];
								break;
							case 12:
								genclass . field12 = inner [ 1 ];
								break;
							case 13:
								genclass . field13 = inner [ 1 ];
								break;
							case 14:
								genclass . field14 = inner [ 1 ];
								break;
							case 15:
								genclass . field15 = inner [ 1 ];
								break;
							case 16:
								genclass . field16 = inner [ 1 ];
								break;
							case 17:
								genclass . field17 = inner [ 1 ];
								break;
							case 18:
								genclass . field18 = inner [ 1 ];
								break;
							case 19:
								genclass . field19 = inner [ 1 ];
								break;
							case 20:
								genclass . field20 = inner [ 1 ];
								break;
						}
					}
					catch ( Exception ex ) { }
				}
				Generics . Add ( genclass );
			}
			if ( LoadGrid == true )
			{
				LoadActiveRowsOnlyInGrid ( dgrid , Generics , totalfields );
				UpdateUniversalViewer ( );
			}
			return Generics;
		}

		//********************************************************************************************************************************************************************************//
		public void LoadActiveRowsOnlyInGrid ( DataGrid Grid , ObservableCollection<GenericClass> genericcollection , int total )
		//********************************************************************************************************************************************************************************//
		{
			// filter data to remove all "extraneous" columns
			Grid . ItemsSource = null;
			Grid . Items . Clear ( );
			if ( total == 1 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1}).ToList();
				Grid . ItemsSource = res;
			}
			else if ( total == 2 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1,data.field2}).ToList();
				Grid . ItemsSource = res;
			}
			else if ( total == 3 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1, data.field2,data.field3}).ToList();
				Grid . ItemsSource = res;
			}
			else if ( total == 4 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1, data.field2, data.field3,data.field4}).ToList();
				Grid . ItemsSource = res;
			}
			else if ( total == 5 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1, data.field2, data.field3,data.field4,data.field5}).ToList();
				Grid . ItemsSource = res;
			}
			else if ( total == 6 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1, data.field2, data.field3,data.field4,data.field5,data.field6}).ToList();
				Grid . ItemsSource = res;
			}
			else if ( total == 7 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1, data.field2, data.field3,data.field4,data.field5,data.field6,data.field7}).ToList();
				Grid . ItemsSource = res;
			}
			else if ( total == 8 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1, data.field2, data.field3,data.field4,data.field5,data.field6,data.field7,data.field8}).ToList();
				Grid . ItemsSource = res;
			}
			else if ( total == 9 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1, data.field2, data.field3,data.field4,data.field5,data.field6,data.field7,data.field8,data.field9}).ToList();
				Grid . ItemsSource = res;
			}
			else if ( total == 10 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1, data.field2, data.field3,data.field4,data.field5,data.field6,data.field7,data.field8,data.field9 ,data.field10}).ToList();
				Grid . ItemsSource = res;
			}
			else if ( total == 11 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1, data.field2, data.field3,data.field4,data.field5,data.field6,data.field7,data.field8,data.field9,data.field10,data.field11}).ToList();
				Grid . ItemsSource = res;
			}
			else if ( total == 12 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1, data.field2, data.field3,data.field4,data.field5,data.field6,data.field7,data.field8,data.field9,data.field10,data.field11,data.field12}).ToList();
				Grid . ItemsSource = res;
			}
			else if ( total == 13 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1, data.field2, data.field3,data.field4,data.field5,data.field6,data.field7,data.field8,data.field9,data.field10,data.field11,data.field12,data.field13}).ToList();
				Grid . ItemsSource = res;
			}
			else if ( total == 14 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1, data.field2, data.field3,data.field4,data.field5,data.field6,data.field7,data.field8,data.field9,data.field10,data.field11,data.field12,data.field13,data.field14}).ToList();
				Grid . ItemsSource = res;
			}
			else if ( total == 15 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1, data.field2, data.field3,data.field4,data.field5,data.field6,data.field7,data.field8,data.field9,data.field10,data.field11,data.field12,data.field13,data.field14,data.field15}).ToList();
				Grid . ItemsSource = res;
			}
			else if ( total == 16 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1, data.field2, data.field3,data.field4,data.field5,data.field6,data.field7,data.field8,data.field9,data.field10,data.field11,data.field12,data.field13,data.field14,data.field15,data.field16}).ToList();
				Grid . ItemsSource = res;
			}
			else if ( total == 17 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1, data.field2, data.field3,data.field4,data.field5,data.field6,data.field7,data.field8,data.field9,data.field10,data.field11,data.field12,data.field13,data.field14,data.field15,data.field16,data.field17}).ToList();
				Grid . ItemsSource = res;
			}
			else if ( total == 18 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1, data.field2, data.field3,data.field4,data.field5,data.field6,data.field7,data.field8,data.field9,data.field10,data.field11,data.field12,data.field13,data.field14,data.field15,data.field16,data.field17,data.field18}).ToList();
				Grid . ItemsSource = res;
			}
			else if ( total == 19 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1, data.field2, data.field3,data.field4,data.field5,data.field6,data.field7,data.field8,data.field9,data.field10,data.field11,data.field12,data.field13,data.field14,data.field15,data.field16,data.field17,data.field18,data.field19}).ToList();
				Grid . ItemsSource = res;
			}
			else if ( total == 20 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1, data.field2, data.field3,data.field4,data.field5,data.field6,data.field7,data.field8,data.field9,data.field10,data.field11,data.field12,data.field13,data.field14,data.field15,data.field16,data.field17,data.field18,data.field19,data.field20}).ToList();
				Grid . ItemsSource = res;
			}
			Grid . SelectedIndex = 0;
			Grid . Visibility = Visibility . Visible;
			GridCount . Text = Grid . Items . Count . ToString ( );
			Grid . Refresh ( );
			Grid . Focus ( );
		}
		#endregion LoadDbGrid

		//private void ShowInGrid_Checked ( object sender , RoutedEventArgs e )
		//{
		//	ShowGridFlag = true;
		//}
		//private void ShowInGrid_Unchecked ( object sender , RoutedEventArgs e )
		//{
		//	ShowGridFlag = false;
		//}


		//********************************************************************************************************************************************************************************//
		private void ExecuteSQLCommand_Click ( object sender , RoutedEventArgs e )
		//********************************************************************************************************************************************************************************//
		{
			// Execute Command button
			ExecuteSp . Focus ( );
			Mouse . OverrideCursor = Cursors . Wait;
			string args =SPArgs.Text == "Enter arguments here ..." ? "" : SPArgs.Text;

			if ( args . ToUpper ( ) . Contains ( "SELECT " ) == false )
				SqlCommand = SPCombo . SelectedItem . ToString ( );
			else
				SqlCommand = SPArgs . Text;
			// dummy Fn so we can call it drectly

			// Check for it being an SP in the text field - cever eh ?
			//			ObservableCollection <GenericClass>Checklist= new ObservableCollection<GenericClass>();
			GetSPCheckList ( SqlCommand );
			if ( CheckForSPCommand ( SqlCommandString . Text ) )
			{
				SP_checklist . Clear ( );
				DbCopiedResult . Text = $"SQL command\n[{SqlCommandString . Text} failed to execute...";
				return;
			}
			ExecuteStoredProcedure ( SqlCommand , null , "" , args , e );
			SP_checklist . Clear ( );
			DbCopiedResult . Text = $"[{SqlCommand}] completed successfully....";
			this . Title = $"Sql Server Commands : Active/Last Db/Command : '{SqlCommand}'";
			Mouse . OverrideCursor = Cursors . Arrow;

		}
		//********************************************************************************************************************************************************************************//
		public ObservableCollection<GenericClass> ExecuteStoredProcedure ( string SqlCommand , ObservableCollection<GenericClass> generics = null , string DbName = "" , string Arguments = "" , RoutedEventArgs e = null , bool displayData = true )
		//********************************************************************************************************************************************************************************//
		{
			string SavedValue = SqlCommand;
			string command = "", dbnametoopen = "";
			string errormsg="";
			string  WhereClause="", OrderByClause="";
			bool CheckingArgsOnly = false;
			//			ObservableCollection<GenericClass> Generics = new ObservableCollection<GenericClass>();
			if ( generics != null )
				Generics = generics;
			ObservableCollection<BankAccountViewModel> bvmparam = new ObservableCollection<BankAccountViewModel>();
			Dictionary <string, object>dict = new Dictionary<string, object>();
			IEnumerable DbResult=null;
			CurrentCommandLabel . Text = $"[{SqlCommand}]";

			//============
			// Sanity checks
			//============
			// If it is a CopyDb Procedure, bale out, use the Copy button
			if ( SqlCommand . ToUpper ( ) . Contains ( "SPCOPYDB" ) )
			{
				MessageBox . Show ( $"Please use the 'Copy Db' button at top right to perform this operation.." , "Input error" , MessageBoxButton . OK );
				return null;
			}
			if ( SavedValue == "spGetFullSchema" && Arguments == "FULL" )
			{
				Arguments = "";
			}
			if ( SavedValue == "spGetSpecificSchema" )
			{
				if ( Arguments == "" )
				{
					if ( SPArgs . Text == "Enter arguments here ..." )
					{
						MessageBox . Show ( "You need to specify the SP that you want to view the ARG's\nfor in the field 'Enter arguments here ...'??" ,
							     "Details required" , MessageBoxButton . OK , MessageBoxImage . Warning );
						return null;
					}
					var reslt = MessageBox . Show ("Click Yes to see ALL occurences of @ARG, No for just the header declarations?","Details required", MessageBoxButton.YesNo, MessageBoxImage.Question  );
					if ( reslt == MessageBoxResult . Yes )
					{
						showall = true;
						CheckingArgsOnly = true;
					}
				}
				else if ( DbName != "COMMENTS" )
				{
					//var reslt = MessageBox . Show ( "This will get the @ARGS for the currently selected SP in the Combo above\nClick Yes to proceed or No to select a  different SP in the combo'??" , "Confirmation required" ,
					//	  MessageBoxButton.YesNoCancel, MessageBoxImage .Question);
					//if ( reslt == MessageBoxResult . Yes )
					//{
					var reslt = MessageBox . Show ( "Click Yes to see ALL occurences of @ARG, No for just the header declarations?" , "Details required" , MessageBoxButton . YesNo , MessageBoxImage . Question );
					if ( reslt == MessageBoxResult . Yes )
						showall = true;
					else
						showall = false;
					UpdateUniversalViewer ( );
					CheckingArgsOnly = true;
					GridData_Display . Visibility = Visibility . Hidden;
					SetViewButtons ( 2 , ( GridData_Display . Visibility == Visibility . Visible ? true : false ) , ( DisplayGrid . Visibility == Visibility . Visible ? true : false ) );
				}
			}
			//****************************************************************************//
			// This is the MAIN call made to connect the SQL queries
			//****************************************************************************//
			//if ( Arguments != "" )
			//	Arguments = ParseArguments ( Arguments );

			try
			{
				List<string> genericlist = new List<string>();
				bool usegeneric = false;
				string outbuffer="";

				if ( usegeneric )
				{
					GenericClass gc = new GenericClass ( );
					List<GenericClass> genlist = new List<GenericClass>();
					DapperGeneric<GenericClass , ObservableCollection<GenericClass> , List<GenericClass>> . ExecuteSPFullGenericClass (
						ref Generics ,
						false ,
						ref Generics ,
						SqlCommand ,
						Arguments ,
						"" ,
						"" ,
						ref genlist ,
						 out errormsg );
				}
				else
				{
					DapperSupport . CreateGenericCollection (
						ref Generics ,
						SqlCommand ,
						Arguments ,
						"" ,
						"" ,
						ref genericlist ,
						ref errormsg );
				}
				if ( displayData == false )
				{
					return Generics;
				}

				if ( errormsg == "" )
				{
					int columncount = 0;
					DataTable dt = new DataTable();
					dt = ProcessSqlCommand ( SqlCommand + " " + Arguments );
					if ( dt . Rows . Count == 0 )
					{
						// Dont dfisplay if it is an SQL TYPE/TALE etc  creation command
						if ( errormsg == "" && SqlCommand . Contains ( "SPCREATE_" ) == false )
							MessageBox . Show ( $"Datatable contains Zero records " , $"[{SqlCommand} {Arguments}] SP Script Error" , MessageBoxButton . OK , MessageBoxImage . Warning );
						else if ( SqlCommand . Contains ( "SPCREATE_" ) )
							DbCopiedResult . Text = $"[{SqlCommand}]\n\n'Special' command has been completed sucessfully ...";
						// setup grid as we have no new data
						SetDumyGridRow ( DisplayGrid );
						return null;
					}
					else
					{
						// Got the data, so display it
						int  count = 0;
						string  store="";
						columncount = 0;
						Generics . Clear ( );
						foreach ( var item in dt . Rows )
						{
							GenericClass  gc = new GenericClass ( );
							DataRow dr = item as DataRow;
							columncount = dr . ItemArray . Count ( );
							if ( columncount == 0 )
								columncount = 1;
							// we only need max cols - 1 here !!!
							for ( int x = 0 ; x < columncount ; x++ )
								store = dr . ItemArray [ x ] . ToString ( ) + ",";
							outbuffer += store;
							//						if(columncount > 1)
							CreateGenericRecord ( store , gc , Generics );
							//						else
							//							GridData_Display . Text = store;
							count++;
						}

						if ( DbName == "COMMENTS" )
						{
							string output="";
							string[] flds = outbuffer.Split('\n');
							try
							{
								foreach ( var item in flds )
								{
									if ( item . Length < 3 )
										continue;
									if ( item == "--\r" && item . Length <= 4 )
										break;
									else if ( item . Substring ( 0 , 2 ) == "--" )
										output += item + "\n";
								}
							}
							catch ( Exception ex ) {; }
							if ( output == "" )
							{
								Mouse . OverrideCursor = Cursors . Arrow;
								Utils . Mbox ( this , string1: "No comments were found in the currently selected script" , string2: "" , caption: "SP Comments !" , iconstring: "\\icons\\Information.png" , Btn1: MB . OK , Btn2: MB . NNULL , defButton: MB . OK );
								return null;
							}
							// hide grid
							togglevisibility ( false , "" );
							GridData_Display . Visibility = Visibility . Visible;
							GridData_Display . Text = output;
							//DisplayGrid . Visibility = Visibility . Hidden;
							SetViewButtons ( 1 , ( GridData_Display . Visibility == Visibility . Visible ? true : false ) , ( DisplayGrid . Visibility == Visibility . Visible ? true : false ) );
							UpdateUniversalViewer ( );
							GridData_Display . Focus ( );
							return null;
						}
						if ( SqlCommand != "spGetSpecificSchema" )
						{
							LoadActiveRowsOnlyInGrid ( DisplayGrid , Generics , columncount );
							//GridData_Display . Visibility = Visibility . Hidden;
							DisplayGrid . SelectedIndex = 0;
							DisplayGrid . Visibility = Visibility . Visible;
							GridCount . Text = DisplayGrid . Items . Count . ToString ( );
							UpdateUniversalViewer ( );
							DisplayGrid . Refresh ( );
							DisplayGrid . Focus ( );
							//Show Grid
							SetViewButtons ( 1 , ( GridData_Display . Visibility == Visibility . Visible ? true : false ) , ( DisplayGrid . Visibility == Visibility . Visible ? true : false ) );
							togglevisibility ( true , "" );
						}
						else
						{
							//							DisplayGrid . Visibility = Visibility . Hidden;
							ShowGrid . Content = "Show Grid Viewer";
							if ( CheckingArgsOnly )
							{
								store = ParseforArgsLines ( outbuffer , showall );
							}
							if ( store . Length > 0 )
							{
								if ( showall )
									store = "\nONLY Lines with lines containing '@' ANYWHERE in the selected script are shown :\n\n" + store;
								else
									store = "\nONLY Lines with lines containing @' in the HEADER block alone of the selected script are shown :\n\n" + store;
								GridData_Display . Text = store;
								GridData_Display . Visibility = Visibility . Visible;
								UpdateUniversalViewer ( );
								GridData_Display . Focus ( );
								// hide grid
								togglevisibility ( false , "" );
								SetViewButtons ( 2 , ( GridData_Display . Visibility == Visibility . Visible ? true : false ) , ( DisplayGrid . Visibility == Visibility . Visible ? true : false ) );
							}
							else
							{
								Mouse . OverrideCursor = Cursors . Arrow;
								Utils . Mbox ( this , string1: "The selected stored procedure does not appear to require ANY Arguments..." , string2: "" , caption: "SP Argument check" , iconstring: "\\icons\\Information.png" , Btn1: MB . OK , Btn2: MB . NNULL , defButton: MB . OK );
								return null;
							}
						}
					}
				}
				else if ( errormsg . Contains ( "DYNAMIC:" ) )
				{
					// Process the data we have finally got into the Observabllecollection<GenricsClass>
					// which has 20 columns down to just whatever # of columns we actually have to work with/Display
					//Much cleaner and more pleasant to view
					if ( Generics . Count == 0 && errormsg . Contains ( "DYNAMIC:0" ) )
					{
						MessageBox . Show ( $"SQL Error : \nSql Query was : [{SqlCommand}]  returned ZERO records !" , "SQL Query Error" , MessageBoxButton . OK ,
							    MessageBoxImage . Error );
						this . Title = $"Sql Server Commands : Active Db : 'None'";
						DbCopiedResult . Text = $"SQL query [{SqlCommand}] retuned ZERO records";
						CurrentCommandLabel . Text = SqlCommand + " " + Arguments;
						return null;
					}
					string t = errormsg.Substring(8);
					int colcount =Convert.ToInt32(t );
					Console . WriteLine ( $"Db has {Generics . Count} records and {colcount} columns" );
					//LoadActiveRowsOnlyInGrid ( spViewerGrid , Generics , colcount );
					LoadActiveRowsOnlyInGrid ( DisplayGrid , Generics , colcount );
					UpdateUniversalViewer ( );
					DisplayGrid . SelectedIndex = 0;
					//spViewerGrid . SelectedIndex = 0;
					//spViewerGrid . Refresh ( );
					DisplayGrid . Refresh ( );
					//Show Grid
					togglevisibility ( true , GetDbNameFromCommand ( SqlCommand ) );
					// update  informatoin panels
					DbCopiedResult . Text = $"[{SqlCommand}] has been completed successfully....";
					CurrentCommandLabel . Text = $"[{SqlCommand}]";
					this . Title = $"Sql Server Commands : Active/Last Db/Command : '{SqlCommand}'";

					return null;
				}
				else if ( errormsg . Contains ( "SQL PARSE ERROR" ) || errormsg . Contains ( "SQLERROR :" ) )
				{
					// Process the data we have finally got into the Observabllecollection<GenricsClass>
					// which has 20 columns down to just whatever # of columns we actually have to work with/Display
					//Much cleaner and more pleasant to view
					MessageBoxResult  dr = MessageBox . Show ( errormsg + $"Do you want to view  the full script contents ?" , "SQL Query Error" , MessageBoxButton .YesNo ,
						    MessageBoxImage . Error );
					this . Title = $"Sql Server Commands : Active Db : 'None'";
					DbCopiedResult . Text = $"SQL query [{SqlCommand}] failed to run  correctly";
					CurrentCommandLabel . Text = SqlCommand + " " + Arguments;
					if ( dr == MessageBoxResult . Yes )
					{
						ViewFullSP_Click ( null , null );
					}
					return null;
				}
				else
				{
					// Unkown error ??
					int columncount = 0;
					DataTable dt = new DataTable();
					if ( SqlCommand . Substring ( 0 , 2 ) != "sp" )
					{
						dt = ProcessSqlCommand ( SqlCommand + " " + Arguments );
						if ( dt . Rows . Count == 0 )
						{
							// Dont dfisplay if it is an SQL TYPE/TALE etc  creation command
							if ( errormsg == "" && SqlCommand . Contains ( "SPCREATE_" ) == false )
								MessageBox . Show ( $"Datatable contains Zero records " , $"[{SqlCommand} {Arguments}] SP Script Error" , MessageBoxButton . OK , MessageBoxImage . Warning );
							else if ( SqlCommand . Contains ( "SPCREATE_" ) )
								DbCopiedResult . Text = $"[{SqlCommand}]\n\n'Special' command has been completed sucessfully ...";
							return null;
						}
						int  count = 0;
						columncount = 0;
						Generics . Clear ( );
						foreach ( var item in dt . Rows )
						{
							GenericClass  gc = new GenericClass ( );
							string  store="";
							DataRow dr = item as DataRow;
							columncount = dr . ItemArray . Count ( );
							if ( columncount == 0 )
								columncount = 1;
							// we only need max cols - 1 here !!!
							for ( int x = 0 ; x < columncount ; x++ )
								store += dr . ItemArray [ x ] . ToString ( ) + ",";
							CreateGenericRecord ( store , gc , Generics );
						}
						if ( SavedValue == "spGetSpecificSchema" )
						{
							MessageBoxResult dr= new MessageBoxResult();
							if ( Generics . Count > 0 )
							{
								string buff = GetSpecificSPArguments ( "RETURNEDRESULTS" , Generics [ 0 ] . field1, showall );
								if ( buff . Length == 0 )
								{
									dr = MessageBox . Show ( "The request succeeded, but the selected SP does not require any Arguments\nDo you want to view the full Script contents ?" , "Sql Information" , MessageBoxButton . YesNo , MessageBoxImage . Information );
									return null;
								}
								//DisplayGrid . Visibility = Visibility . Hidden;
								GridData_Display . Visibility = Visibility . Visible;
								GridData_Display . Text = buff;
								CurrentCommandLabel . Text = $"[{SqlCommand}]";
								DbCopiedResult . Text = $"[{SqlCommand}] completed successfully....";
								this . Title = $"Sql Server Commands : Active/Last Db/Command : '{SqlCommand}'";
								if ( dr == MessageBoxResult . Yes )
								{
									ViewFullSP_Click ( null , null );
								}
								// hide grid
								togglevisibility ( false , "" );
								SetViewButtons ( 2 , ( GridData_Display . Visibility == Visibility . Visible ? true : false ) , ( DisplayGrid . Visibility == Visibility . Visible ? true : false ) );
							}
							return null;
						}
						// Loads JUST the rows we actually have, not the full 20 columns !!
						//Much cleaner and more pleasant to view
						LoadActiveRowsOnlyInGrid ( DisplayGrid , Generics , columncount );
						UpdateUniversalViewer ( );
						//Show Grid
						if ( Arguments != "" )
							DbCopiedResult . Text = $"The Stored Procedure [{SqlCommand} [{Arguments}]] \nwas executed successfuly...";
						else
							DbCopiedResult . Text = $"The Stored Procedure [{SqlCommand}] \nwas executed successfuly...";
						DisplayGrid . SelectedIndex = 0;
						DisplayGrid . Visibility = Visibility . Visible;
						SetViewButtons ( 2 , ( GridData_Display . Visibility == Visibility . Visible ? true : false ) , ( DisplayGrid . Visibility == Visibility . Visible ? true : false ) );
						togglevisibility ( true , GetDbNameFromCommand ( SqlCommand ) );
						GridCount . Text = DisplayGrid . Items . Count . ToString ( );
						DisplayGrid . Refresh ( );
						DisplayGrid . Focus ( );
						// update  information panels
						CurrentCommandLabel . Text = $"[{SqlCommand}]";
						OriginalSqlCommand = SqlCommand;
						DbCopiedResult . Text = $"[{SqlCommand}] has been completed successfully....";
						CurrentCommandLabel . Text = $"[{SqlCommand}]";
						this . Title = $"Sql Server Commands : Active/Last Db/Command : '{SqlCommand}'";
					}
				}
			}
			catch ( Exception ex )
			{
				MessageBox . Show ( $"SQL ERROR 1125 - {ex . Message}" );
				return null;
			}
			return null;
		}

		private static string ParseforArgsLines ( string input , bool showall )
		{
			string output="";
			string[] flds;
			flds = input . Split ( '\n' );
			foreach ( var item in flds )
			{
				if ( showall == false && item . Contains ( "@SQL" ) )
					break;
				else
				{
					if ( item . Contains ( "@" ) )
						output += item + "\n";
				}
			}
			return output;
		}
		//********************************************************************************************************************************************************************************//
		public string GetSpArgs ( string spName , bool showfull = false )
		//********************************************************************************************************************************************************************************//
		{
			string output = "";
			string errormsg="";
			int columncount = 0;
			DataTable dt = new DataTable();
			ObservableCollection<GenericClass> Generics = new ObservableCollection<GenericClass>();

			ObservableCollection<BankAccountViewModel> bvmparam = new ObservableCollection<BankAccountViewModel>();
			List<string> genericlist = new List<string>();
			try
			{
				DapperSupport . CreateGenericCollection (
					ref Generics ,
					"spGetSpecificSchema  " ,
					$"{SPCombo . SelectedItem . ToString ( )}" ,
					"" ,
					"" ,
					ref genericlist ,
					ref errormsg );
				dt = ProcessSqlCommand ( "spGetSpecificSchema  " + spName );
				if ( dt . Rows . Count == 0 )
				{
					if ( errormsg == "" )
						MessageBox . Show ( $"No Argument information is available" , $"[{spName }] SP Script Information" , MessageBoxButton . OK , MessageBoxImage . Warning );
					return "";
				}
			}
			catch ( Exception ex )
			{
				MessageBox . Show ( $"SQL ERROR 1125 - {ex . Message}" );
				return "";
			}
			int  count = 0;
			columncount = 0;
			//			Generics . Clear ( );
			foreach ( var item in dt . Rows )
			{
				GenericClass  gc = new GenericClass ( );
				string  store="";
				DataRow dr = item as DataRow;
				columncount = dr . ItemArray . Count ( );
				if ( columncount == 0 )
					columncount = 1;
				// we only need max cols - 1 here !!!
				for ( int x = 0 ; x < columncount ; x++ )
					store += dr . ItemArray [ x ] . ToString ( ) + ",";
				output += store;
				//CreateGenericRecord ( store , gc , Generics );
			}
			if ( showfull == false )
			{
				// we now have the result, so lets process them
				string buffer = output;
				string[] lines = buffer.Split('\n');
				output = "";
				//output = $"Procedure Name : \n{SPCombo . SelectedItem . ToString ( ) . ToUpper ( )}\n\n";
				foreach ( var item in lines )
				{
					if ( item . ToUpper ( ) . Contains ( "@ARG" ) )
						output += item;
					if ( showall == false && item . ToUpper ( ) == "AS\r" )
						break;
				}
				// we now have a list of the Args for the selected SP in output
				// Show it in a TextBox if it takes 1 or more args
				if ( output != "" )
				{
					GridData_Display . Text = $"Procedure Name : {SPCombo . SelectedItem . ToString ( ) . ToUpper ( )}\n\n";
					GridData_Display . Text += output;
					GridData_Display . Text += $"\n\nPress ENTER to execute {SPCombo . SelectedItem . ToString ( ) . ToUpper ( )}\nor ESCAPE to exit the process...\n";
					GridData_Display . Visibility = Visibility . Visible;
					SetViewButtons ( 2 , ( GridData_Display . Visibility == Visibility . Visible ? true : false ) , ( DisplayGrid . Visibility == Visibility . Visible ? true : false ) );
					GridData_Display . Focus ( );
				}
				else
				{
					Mouse . OverrideCursor = Cursors . Arrow;
					Utils . Mbox ( this , string1: $"Procedure [{SPCombo . SelectedItem . ToString ( ) . ToUpper ( )}] \ndoes not Support / Require any arguments" , string2: "" , caption: "" , iconstring: "\\icons\\Information.png" , Btn1: MB . OK , Btn2: MB . NNULL , defButton: MB . OK );
				}
			}
			return output;
		}
		//********************************************************************************************************************************************************************************//
		private string ParseArguments ( string args )
		//********************************************************************************************************************************************************************************//
		{
			int count = 1;
			string Output="";
			string verifyquotes="";
			if ( args . Contains ( "," ) )
			{
				string[] tmp = args.Split(',');
				foreach ( var item in tmp )
				{
					// We must have strings quoted for Dapper args in stored procs
					if ( item . Contains ( "\"" ) == false )
					{
						verifyquotes = "\"" + item + "\"";
						Output += verifyquotes;
					}
					else
						Output += item . Trim ( );
					AddIdentifier ( Output , count );
				}
			}
			else
			{
				if ( args . Contains ( "\"" ) == false )
				{
					verifyquotes = "\"" + args + "\"";
					Output = "@Arg1=" + verifyquotes;
				}
				else
					Output = "@Arg1=" + args . Trim ( );
			}
			return Output;
		}
		//********************************************************************************************************************************************************************************//
		private string AddIdentifier ( string Input , int count )
		//********************************************************************************************************************************************************************************//
		{
			string output="";
			switch ( count )
			{
				case 1:
					output = "@Arg1=" + Input;
					break;
				case 2:
					output = "@Arg2=" + Input;
					break;
				case 3:
					output = "@Arg3=" + Input;
					break;
				case 4:
					output = "@Arg4=" + Input;
					break;
				case 5:
					output = "@Arg5=" + Input;
					break;
				case 6:
					output = "@Arg6=" + Input;
					break;
			}
			return output;
		}
		//********************************************************************************************************************************************************************************//
		public static void CreateGenericRecord ( string store , GenericClass gc , ObservableCollection<GenericClass> gco = null )
		//********************************************************************************************************************************************************************************//
		{
			int colcount = 0;
			string[] entries;
			//if ( store . Length > 50 )
			//{
			//	colcount = 1;
			//	gc . field1 = store;
			//}
			//else
			//{
			if ( store . ToUpper ( ) . Contains ( "CREATE PROCEDURE " ) )
			{
				gc . field1 = store;
			}
			else
			{
				entries = store . Split ( ',' );
				if ( entries . Length <= 1 )
				{
					colcount = 1;
					gc . field1 = store;
				}
				colcount = entries . Length - 1;
				if ( colcount >= 1 )
					gc . field1 = entries [ 0 ];
				if ( colcount >= 2 )
					gc . field2 = entries [ 1 ];
				if ( colcount >= 3 )
					gc . field3 = entries [ 2 ];
				if ( colcount >= 4 )
					gc . field4 = entries [ 3 ];
				if ( colcount >= 5 )
					gc . field5 = entries [ 4 ];
				if ( colcount >= 6 )
					gc . field6 = entries [ 5 ];
				if ( colcount >= 7 )
					gc . field7 = entries [ 6 ];
				if ( colcount >= 8 )
					gc . field8 = entries [ 7 ];
				if ( colcount >= 9 )
					gc . field9 = entries [ 8 ];
				if ( colcount >= 10 )
					gc . field10 = entries [ 9 ];
				if ( colcount >= 11 )
					gc . field11 = entries [ 10 ];
				if ( colcount >= 12 )
					gc . field12 = entries [ 11 ];
				if ( colcount >= 13 )
					gc . field13 = entries [ 12 ];
				if ( colcount >= 14 )
					gc . field14 = entries [ 13 ];
				if ( colcount >= 15 )
					gc . field15 = entries [ 14 ];
				if ( colcount >= 16 )
					gc . field16 = entries [ 15 ];
				if ( colcount >= 17 )
					gc . field17 = entries [ 16 ];
				if ( colcount >= 18 )
					gc . field18 = entries [ 17 ];
				if ( colcount >= 19 )
					gc . field19 = entries [ 18 ];
				if ( colcount >= 20 )
					gc . field20 = entries [ 19 ];
			}
			if ( gco != null )
				gco . Add ( gc );
			else
				gco . Add ( gc );
		}
		//********************************************************************************************************************************************************************************//
		public static DataTable ProcessSqlCommand ( string SqlCommand )
		//********************************************************************************************************************************************************************************//
		{
			SqlConnection con;
			DataTable dt = new DataTable();
			string ConString = "";
			string filterline = "";
			ConString = ( string ) Properties . Settings . Default [ "BankSysConnectionString" ];
			//Debug . WriteLine ( $"Making new SQL connection in DETAILSCOLLECTION,  Time elapsed = {timer . ElapsedMilliseconds}" );
			//SqlCommand += " TempDb";
			con = new SqlConnection ( ConString );
			try
			{
				Debug . WriteLine ( $"Using new SQL connection in PROCESSSQLCOMMAND" );
				using ( con )
				{
					SqlCommand cmd = new SqlCommand ( SqlCommand , con );
					SqlDataAdapter sda = new SqlDataAdapter ( cmd );
					sda . Fill ( dt );
				}
			}
			catch ( Exception ex )
			{
				Debug . WriteLine ( $"ERROR in PROCESSSQLCOMMAND(): Failed to load Datatable :\n {ex . Message}, {ex . Data}" );
				MessageBox . Show ( $"ERROR in PROCESSSQLCOMMAND(): Failed to load datatable\n{ex . Message}" );
			}
			finally
			{
				Console . WriteLine ( $" SQL data loaded from SQLCommand [{SqlCommand . ToUpper ( )}]" );
				con . Close ( );
			}
			return dt;
		}
		//********************************************************************************************************************************************************************************//
		public static ObservableCollection<GenericClass> ProcessSelectedCollection ( DataTable dt )
		//********************************************************************************************************************************************************************************//
		{
			int  count = 0;
			ObservableCollection<GenericClass> genericcollection = new ObservableCollection<GenericClass>();
			try
			{
				for ( int i = 0 ; i < dt . Rows . Count ; i++ )
				{
					genericcollection . Add ( new GenericClass
					{
						field1 = dt?.Rows [ i ] [ 0 ] . ToString ( ) ,
						field2 = dt?.Rows [ i ] [ 1 ] . ToString ( ) ,
						field3 = dt?.Rows [ i ] [ 2 ] . ToString ( ) ,
						field4 = dt?.Rows [ i ] [ 3 ] . ToString ( ) ,
						field5 = dt?.Rows [ i ] [ 4 ] . ToString ( ) ,
						field6 = dt?.Rows [ i ] [ 5 ] . ToString ( ) ,
						field7 = dt?.Rows [ i ] [ 6 ] . ToString ( ) ,
						field8 = dt?.Rows [ i ] [ 7 ] . ToString ( ) ,
						field9 = dt?.Rows [ i ] [ 8 ] . ToString ( ) ,
						field10 = dt?.Rows [ i ] [ 9 ] . ToString ( ) ,
						field11 = dt?.Rows [ i ] [ 10 ] . ToString ( ) ,
						field12 = dt?.Rows [ i ] [ 11 ] . ToString ( ) ,
						field13 = dt?.Rows [ i ] [ 12 ] . ToString ( ) ,
						field14 = dt?.Rows [ i ] [ 13 ] . ToString ( ) ,
						field15 = dt?.Rows [ i ] [ 14 ] . ToString ( ) ,
						field16 = dt?.Rows [ i ] [ 15 ] . ToString ( ) ,
						field17 = dt?.Rows [ i ] [ 16 ] . ToString ( ) ,
						field18 = dt?.Rows [ i ] [ 17 ] . ToString ( ) ,
						field19 = dt?.Rows [ i ] [ 18 ] . ToString ( ) ,
						field20 = dt?.Rows [ i ] [ 19 ] . ToString ( )
					} );
					count++;
				}
			}
			catch ( Exception ex )
			{
				Debug . WriteLine ( $"BANK : SQL Error in BankCollection load function (count = {count}) : {ex . Message}, {ex . Data}" );
			}
			finally
			{
				//				Debug . WriteLine ( $"BANK : Completed load into Bankcollection :  {temp . Count} records loaded successfully ...." );
			}
			return genericcollection;
		}
		//********************************************************************************************************************************************************************************//
		private void DbToCopyCombo_SelectionChanged ( object sender , SelectionChangedEventArgs e )
		//********************************************************************************************************************************************************************************//
		{
			DbToCopyCombo . Focus ( );
			ComboBox cb = sender as  ComboBox;
			ComboBoxItem cbitem = new ComboBoxItem();
			cbitem = cb . SelectedItem as ComboBoxItem;
			if ( cbitem != null )
			{
				cb . SelectedItem = cb . SelectedIndex;
				cbitem . Background = FindResource ( "Blue1" ) as SolidColorBrush;
				cbitem . Foreground = FindResource ( "White0" ) as SolidColorBrush;
				cb . Refresh ( );
			}
		}
		//********************************************************************************************************************************************************************************//
		private void DbToCopyCombo_MouseDoubleClick ( object sender , MouseButtonEventArgs e )
		//********************************************************************************************************************************************************************************//
		{
			DbToCopyCombo . IsDropDownOpen = true;
		}
		//********************************************************************************************************************************************************************************//
		private void SPCombo_MouseDoubleClick ( object sender , MouseButtonEventArgs e )
		//********************************************************************************************************************************************************************************//
		{
			if ( SPCombo . IsDropDownOpen == false )
			{
				SPCombo . IsDropDownOpen = true;
			}
		}
		//********************************************************************************************************************************************************************************//
		private void toggleButton_Click ( object sender , RoutedEventArgs e )
		//********************************************************************************************************************************************************************************//
		{
			// combo button click
		}
		//********************************************************************************************************************************************************************************//
		private void SPArgs_PreviewMouseLeftButtonUp ( object sender , MouseButtonEventArgs e )
		//********************************************************************************************************************************************************************************//
		{
			//SelectTextBoxText ( SpArgs );
		}
		//********************************************************************************************************************************************************************************//
		private void Schemas_Click ( object sender , RoutedEventArgs e )
		//********************************************************************************************************************************************************************************//
		{
			// get a full list of content of all stored procs
			ShowSchemas . Focus ( );
			ObservableCollection<GenericClass> Generics = new ObservableCollection<GenericClass>();
			SqlCommand = "spGetFullSchema";
			//			GenericClass gc = new GenericClass ( );
			List<GenericClass> genlist = new List<GenericClass>();
			List<string> output = new List<string>();
			string errmsg="";

			// Get a Generics Db structure back and then use a stringbuilder to build a complle list of all
			// records in a single text field that we then view using the GirdData_Display TextBox
			Generics = DapperGeneric<GenericClass , ObservableCollection<GenericClass> , List<GenericClass>> . ExecuteSPFullGenericClass (
				ref Generics ,
				false ,
				ref Generics ,
				SqlCommand ,
				"" ,
				"" ,
				"" ,
				ref genlist ,
				 out errmsg );
			if ( errmsg == "" )
			{
				StringBuilder sb = new StringBuilder();
				string tmp="";
				foreach ( var item in Generics )
				{
					sb . AppendLine ( item . field1 );
					//tmp=item.field1.ToString();
					//output . Add ( tmp );
				}
				//spViewerGrid . Visibility = Visibility . Visible;
				//			DisplayGrid . ItemsSource = Generics;
				//DisplayGrid . Visibility = Visibility . Hidden;
				ShowGrid . Content = "Show Grid Viewer";
				GridData_Display . Text = sb . ToString ( );
				GridData_Display . Visibility = Visibility . Visible;
				UpdateUniversalViewer ( );
				// hide grid
				togglevisibility ( false , "" );
				SetViewButtons ( 2 , ( GridData_Display . Visibility == Visibility . Visible ? true : false ) , ( DisplayGrid . Visibility == Visibility . Visible ? true : false ) );
			}
			else
			{
				Mouse . OverrideCursor = Cursors . Arrow;
				Utils . Mbox ( this , string1: "SQL request failed...." , string2: "Data error" , caption: "Oops" , iconstring: "\\icons\\error.png" , Btn1: MB . OK , Btn2: MB . NNULL , defButton: MB . OK );
			}
			return;

		}
		//********************************************************************************************************************************************************************************//
		private void ListSPs_Click ( object sender , RoutedEventArgs e )
		//********************************************************************************************************************************************************************************//
		{
			string cookieout="";
			// get list of all stored procs in our grid
			ObservableCollection<GenericClass> Generics = new ObservableCollection<GenericClass>();
			ObservableCollection<GenericClass> collection= new ObservableCollection<GenericClass>();
			SqlCommand = "spGetStoredProcs";
			ExecuteStoredProcedure ( SqlCommand , Generics , "" , "" , null );
			LoadActiveRowsOnlyInGrid ( DisplayGrid , Generics , 1 );
			//			DisplayGrid . ItemsSource = collection;
			//GridData_Display . Visibility = Visibility . Hidden;
			UpdateUniversalViewer ( );
			DisplayGrid . Visibility = Visibility . Visible;
			DbCopiedResult . Text = $"Sp [SPGETSTOREDPROCS] command to display Full list of  Stored Procedures in Data Grid completed successfully ...";
			SetViewButtons ( 1 , ( GridData_Display . Visibility == Visibility . Visible ? true : false ) , ( DisplayGrid . Visibility == Visibility . Visible ? true : false ) );
			return;
		}
		//********************************************************************************************************************************************************************************//
		private void SPCombo_SelectionChanged ( object sender , SelectionChangedEventArgs e )
		//********************************************************************************************************************************************************************************//
		{
			SPCombo . Focus ( );
			ComboBox cb = sender as  ComboBox;
			ComboBoxItem cbitem = new ComboBoxItem();
			cbitem = cb . SelectedItem as ComboBoxItem;
			if ( cbitem != null )
			{
				cb . SelectedItem = cb . SelectedIndex;
				cbitem . Background = FindResource ( "Blue1" ) as SolidColorBrush;
				cbitem . Foreground = FindResource ( "White0" ) as SolidColorBrush;
				cb . Refresh ( );
				SPArgs . Text = "";
			}
		}
		//********************************************************************************************************************************************************************************//
		private void ViewDbBtn_Click ( object sender , RoutedEventArgs e )
		//********************************************************************************************************************************************************************************//
		{
			// view selected Db in grid
			ViewDbBtn . Focus ( );
			ObservableCollection<GenericClass> collection = new ObservableCollection<GenericClass>();
			ObservableCollection<BankAccountViewModel> bvm = new ObservableCollection<BankAccountViewModel> ();
			ObservableCollection<GenericClass> GenDb = new ObservableCollection<GenericClass>();
			List<string> data = new List<string>();
			Mouse . OverrideCursor = Cursors . Wait;

			if ( ActiveDbName == "" )
			{
				ActiveDbName = DbToCopyCombo . SelectedItem . ToString ( );
			}
			SqlCommand = $"Select * from {ActiveDbName}";
			OriginalSqlCommand = SqlCommand;
			CurrentCommandLabel . Text = $"[{SqlCommand}]";
			string errormsg="";
			try
			{
				DapperSupport . CreateGenericCollection (
					ref collection ,
					SqlCommand ,
					SPArgs . Text . Trim ( ) ,
					"" ,
					"" ,
					ref data ,
					ref errormsg );
				// creates and loads Db into grid  wwith correct total or columns
				if ( collection . Count > 0 )
				{
					LoadActiveRowsOnlyInGrid ( DisplayGrid , collection , DapperSupport . GetGenericColumnCount ( collection ) );
					UpdateInfoPanels ( ActiveDbName );
					UpdateUniversalViewer ( );
					//Show Grid
					togglevisibility ( true , GetDbNameFromCommand ( SqlCommand ) );
					GridData_Display . Visibility = Visibility . Hidden;
					DisplayGrid . Visibility = Visibility . Visible;
					SetViewButtons ( 1 , ( GridData_Display . Visibility == Visibility . Visible ? true : false ) , ( DisplayGrid . Visibility == Visibility . Visible ? true : false ) );
				}
				else
				{
					DisplayGrid . ItemsSource = null;
					DisplayGrid . Items . Clear ( );
					SetDumyGridRow ( DisplayGrid );
					UpdateUniversalViewer ( );
					GridData_Display . Visibility = Visibility . Visible;
					SetViewButtons ( 2 , ( GridData_Display . Visibility == Visibility . Visible ? true : false ) , ( DisplayGrid . Visibility == Visibility . Visible ? true : false ) );
					togglevisibility ( false , GetDbNameFromCommand ( SqlCommand ) );
					//					TextViewerBtn . Content = "Hide Text Viewer";
					MessageBox . Show ( "Table cannot be shown, probable reason is that it is currently\nempty as it may just be a ' temporary table ' created by one or \nmore of our Stored procedure(s)" , "Db Table Empty !" , MessageBoxButton . OK , MessageBoxImage . Information );
				}
				// Enable view  button
				CurrentCommandLabel . Text = $"[{SqlCommand}]";
				OriginalSqlCommand = SqlCommand;
				CurrentCommandLabel . Text = $"Select * from [{ActiveDbName . ToUpper ( )}]";
				DbCopiedResult . Text = $"SQL Command [{SqlCommand}] completed successfully...";
				ActiveDbName = "";
				Mouse . OverrideCursor = Cursors . Arrow;
			}
			catch ( Exception ex )
			{
				MessageBox . Show ( $"SQL ERROR 1125 - {ex . Message}" );
				SetDumyGridRow ( DisplayGrid );
				return;
			}
		}

		//********************************************************************************************************************************************************************************//
		private void SetViewButtons ( int caller , bool TextVis , bool GridVis )
		//********************************************************************************************************************************************************************************//
		{
			// caller == 1 grid made the call
			// caller == 2 text made the call
			if ( caller == 1 )
			{ // Grid button hit
				if ( TextVis == true && GridVis == true )
				{
					//	both viewers visible , grid ontop
					TextViewerBtn . Content = "Show Text Viewer";
					ShowGrid . Content = "Hide Grid Viewer";
					GridData_Display . Visibility = Visibility . Hidden;
					DisplayGrid . Visibility = Visibility . Visible;
					DisplayGrid . BringIntoView ( );
				}
				else if ( TextVis == false && GridVis == false )
				{
					//	both  hidden
					TextViewerBtn . Content = "Show Text Viewer";
					ShowGrid . Content = "Show Grid Viewer";
				}
				else if ( TextVis == true && GridVis == false )
				{
					//	Text viewer visible, grid hidden
					// Showing grid 
					TextViewerBtn . Content = "Show Text Viewer";
					ShowGrid . Content = "Hide Grid Viewer";
					GridData_Display . Visibility = Visibility . Visible;
					DisplayGrid . Visibility = Visibility . Hidden;
					GridData_Display . BringIntoView ( );
				}
				else if ( TextVis == false && GridVis == true )
				{
					//	grid  only visible 
					TextViewerBtn . Content = "Show Text Viewer";
					ShowGrid . Content = "Hide Grid Viewer";
					GridData_Display . Visibility = Visibility . Hidden;
					DisplayGrid . Visibility = Visibility . Visible;
					DisplayGrid . BringIntoView ( );
				}
			}
			else    // Text calling
			{
				if ( TextVis == true && GridVis == true )
				{
					//	both viewers visible , grid ontop
					TextViewerBtn . Content = "Hide Text Viewer";
					ShowGrid . Content = "Show Grid Viewer";
					GridData_Display . Visibility = Visibility . Visible;
					DisplayGrid . Visibility = Visibility . Hidden;
					GridData_Display . BringIntoView ( );
				}
				else if ( TextVis == false && GridVis == false )
				{
					//	both  hidden
					TextViewerBtn . Content = "Show Text Viewer";
					ShowGrid . Content = "Show Grid Viewer";
				}
				else if ( TextVis == true && GridVis == false )
				{
					//	Text viewer visible, grid hidden
					TextViewerBtn . Content = "Hide Text Viewer";
					ShowGrid . Content = "Show Grid Viewer";
					GridData_Display . Visibility = Visibility . Visible;
					DisplayGrid . Visibility = Visibility . Hidden;
					GridData_Display . BringIntoView ( );
				}
				else if ( TextVis == false && GridVis == true )
				{
					//	grid  only visible 
					TextViewerBtn . Content = "Show Text Viewer";
					ShowGrid . Content = "Hide Grid Viewer";
					GridData_Display . Visibility = Visibility . Visible;
					DisplayGrid . Visibility = Visibility . Hidden;
					GridData_Display . BringIntoView ( );
				}
			}
		}
		//********************************************************************************************************************************************************************************//
		private void UpdateInfoPanels ( string DbName )
		//********************************************************************************************************************************************************************************//
		{
			GridCount . Text = DisplayGrid . Items . Count . ToString ( );
			BankNameLabel . Text = DbName . ToUpper ( );
			this . Title = $"Sql Server Commands : Active Db : {DbName . ToUpper ( )}";
		}
		// Handle viewing whatever is in the grid
		//********************************************************************************************************************************************************************************//
		private void GridViewer_Click ( object sender , RoutedEventArgs e )
		//********************************************************************************************************************************************************************************//
		{
			bool completed = false;
			int count = 0;
			int x = 0, startpos=0;
			int index = 0;
			//string proccode="";
			string buffer="";
			string [] entities, fields;
			string tempbuff="", output="";
			StringBuilder sb = new StringBuilder();
			Mouse . SetCursor ( Cursors . Wait );

			// Export all Stored procedures to text file
			foreach ( var item in DisplayGrid . Items )
			{
				var  dgr = item as SingleFieldClass;
				if ( dgr == null )
				{
					// multi column data
					foreach ( var item2 in DisplayGrid . Items )
					{
						DisplayGrid . SelectedIndex = index;
						buffer = DisplayGrid . SelectedItem . ToString ( );
						entities = buffer . Split ( ',' );
						for ( int y = 0 ; y < entities . Length ; y++ )
						{
							fields = entities [ y ] . Split ( '=' );
							tempbuff = fields [ 1 ] . Trim ( );
							if ( tempbuff . Contains ( "}" ) )
							{
								tempbuff = tempbuff . Trim ( );
								tempbuff = tempbuff . Substring ( 0 , tempbuff . Length - 3 ) . Trim ( );
							}
							sb . Append ( tempbuff + ",\t" );
						}
						sb . Append ( "\n" );
						index++;
					}
					completed = true;
				}
				else
				{
					// Single column data !!
					buffer = dgr . Linestring;
					if ( buffer . Contains ( "\n" ) )
					{
						// we have stored proc text - parse it intellgently
						entities = buffer . Split ( '\n' );
						output = "";
						tempbuff = "";
						//					if(count == 0)
						output = $"\nPROCEDURE {x}\n============\n";
						for ( int y = 0 ; y < entities . Length ; y++ )
						{
							if ( entities [ y ] . Contains ( "\r" ) == true && entities [ y ] . Contains ( "\r\n" ) == false && entities [ y ] . Length > 1 )
							{
								tempbuff = entities [ y ] . Substring ( 0 , entities [ y ] . Length - 1 ) . Trim ( );
								output += tempbuff + "\n";
							}
							else
							{
								//	// its an empty line
								output += "\n";
							}
							sb . Append ( output );
							output = "";
							count++;
						}
						output = $"END PROCEDURE {x + 1}\n============\n";
						x++;
					}     // end - frist row/Headers
					else
					{                       // now we can parse out the data  from all remaining rows
						count = 0;
						tempbuff = "";
						output = "";
						entities = buffer . Split ( ',' );
						for ( int y = 0 ; y < entities . Length ; y++ )
						{
							tempbuff = entities [ y ] . Substring ( 11 );
							if ( y == entities . Length - 1 )
							{
								tempbuff = tempbuff . Substring ( 0 , tempbuff . Length - 3 );
							}
							output += tempbuff + ",\t";
						}
						sb . Append ( output . Trim ( ) );
						//x++;
						sb . Append ( "\n" );
					}
				}
				if ( completed )
					break;
			}     // end each grid row

			sb . Append ( "\n*** END OF FILE ***\n" );
			string fname= @"C:\USERS\IANCH\Documents\GridView.DAT";
			File . WriteAllText ( @"C:\USERS\IANCH\Documents\GridView.DAT" , sb . ToString ( ) );
			Console . WriteLine ( $"All the selected data has been Exported Successfully to  [{fname}" );
			//			var dlgresult = MessageBox . Show ( $"All Stored Procedures have been Exported Successfully to  [{fname}.\nDo you want to view them now ?" , "Data Save" ,MessageBoxButton.YesNo,MessageBoxImage.Question);
			//if ( dlgresult == MessageBoxResult . Yes )
			//{
			Process p = null;
			p = Process . Start ( "Wordpad.exe" , fname );
			Mouse . SetCursor ( Cursors . Arrow );
		}
		//********************************************************************************************************************************************************************************//
		//********************************************************************************************************************************************************************************//
		private void SPCombo_PreviewMouseLeftButtonDn ( object sender , MouseButtonEventArgs e )
		//********************************************************************************************************************************************************************************//
		{
			//			if ( SPCombo . IsDropDownOpen == false )
			//				SPCombo . IsDropDownOpen = true;
			//			else
			//				SPCombo . SelectedItem = SPCombo . SelectedIndex;
			e . Handled = false;
		}
		// Menu items
		//********************************************************************************************************************************************************************************//
		private void Option1_Click ( object sender , RoutedEventArgs e )
		//********************************************************************************************************************************************************************************//
		{

		}
		//********************************************************************************************************************************************************************************//
		private void RefreshLists_Click ( object sender , RoutedEventArgs e )
		//********************************************************************************************************************************************************************************//
		{
			// Populate the Db Tables && Stored Procedures Combo
			LoadTablesCombo ( );
			LoadSPCombo ( );

		}
		//********************************************************************************************************************************************************************************//
		private void ViewGridContent_Click ( object sender , RoutedEventArgs e )
		//********************************************************************************************************************************************************************************//
		{
			GridViewer_Click ( null , null );
		}
		//  Load list if Tables in Db
		//********************************************************************************************************************************************************************************//
		private void LoadTablesCombo ( )
		//********************************************************************************************************************************************************************************//
		{
			int currindex = 0;
			ObservableCollection <GenericClass>Generics= new ObservableCollection<GenericClass>();
			SqlCommand = "spGetTablesList";
			ExecuteStoredProcedure ( SqlCommand , Generics , "" , "" , null );
			CurrentCommandLabel . Text = $"[{SqlCommand}]";
			OriginalSqlCommand = SqlCommand;
			DbToCopyCombo . Items . Clear ( );
			foreach ( var item in Generics )
			{
				DbToCopyCombo . Items . Add ( item . field1 );
			}
			DbToCopyCombo . SelectedIndex = currindex == -1 ? 0 : currindex;
			DbToCopyCombo . SelectedItem = DbToCopyCombo . SelectedIndex;
			DbToCopyCombo . AllowDrop = true;
			DbToCopyCombo . IsEditable = true;
			DbToCopyCombo . MaxHeight = 120;
			DbCopiedResult . Text = $"SQL Command [{SqlCommand}] completed successfully...";
		}
		// Load list of Stored Procedures
		//********************************************************************************************************************************************************************************//
		private void LoadSPCombo ( )
		//********************************************************************************************************************************************************************************//
		{
			ObservableCollection <GenericClass>Generics= new ObservableCollection<GenericClass>();
			SqlCommand = "spGetStoredProcs";
			ExecuteStoredProcedure ( SqlCommand , Generics , "" , "" , null );
			OriginalSqlCommand = SqlCommand;
			CurrentCommandLabel . Text = $"[{SqlCommand}]";
			DbCopiedResult . Text = $"SQL command\n[{SqlCommand}] \bhas been executed successfully: ";
			SPCombo . Items . Clear ( );
			foreach ( var item in Generics )
			{
				SPCombo . Items . Add ( item . field1 );
			}
			SPCombo . SelectedIndex = 0;
			SPCombo . SelectedItem = 0;
			SPCombo . AllowDrop = true;
			SPCombo . IsEditable = true;
			SPCombo . MaxHeight = 120;
		}
		//********************************************************************************************************************************************************************************//
		private void Window_MouseRightButtonUp ( object sender , MouseButtonEventArgs e )
		//********************************************************************************************************************************************************************************//
		{
			DataGrid dg = sender as DataGrid;
			if ( dg != null )
				Debugger . Break ( );
		}
		//********************************************************************************************************************************************************************************//
		private void GridData_Display_CanExecute ( object sender , CanExecuteRoutedEventArgs e )
		//********************************************************************************************************************************************************************************//
		{
			e . Handled = true;
			e . CanExecute = false;
		}
		//********************************************************************************************************************************************************************************//
		private void GridData_Display_MouseDoubleClick ( object sender , MouseButtonEventArgs e )
		//********************************************************************************************************************************************************************************//
		{
			if ( GridData_Display . Visibility == Visibility . Visible )
			{
				//GridData_Display . Visibility = Visibility . Hidden;
				DisplayGrid . Visibility = Visibility . Visible;
				UpdateUniversalViewer ( );
				SetViewButtons ( 1 , ( GridData_Display . Visibility == Visibility . Visible ? true : false ) , ( DisplayGrid . Visibility == Visibility . Visible ? true : false ) );
			}
		}
		//********************************************************************************************************************************************************************************//
		private void info_Click ( object sender , RoutedEventArgs e )
		//********************************************************************************************************************************************************************************//
		{

		}
		//********************************************************************************************************************************************************************************//
		private void Help_Click ( object sender , RoutedEventArgs e )
		//********************************************************************************************************************************************************************************//
		{

		}
		//********************************************************************************************************************************************************************************//
		private void SPArgs_KeyDown ( object sender , KeyEventArgs e )
		//********************************************************************************************************************************************************************************//
		{
			if ( e . Key == Key . Enter )
			{
				ExecuteStoredProcedure ( SPCombo . SelectedItem . ToString ( ) , null , "" , SPArgs . Text , null );
				SP_checklist . Clear ( );
			}
		}
		//********************************************************************************************************************************************************************************//
		public string GetSpecificSPArguments ( string ProcedureToFind , string result = "" , bool showall = true )
		//********************************************************************************************************************************************************************************//
		{
			string output="";

			if ( ProcedureToFind == "RETURNEDRESULTS" )
			{
				// we now have the result, so lets process them
				string buffer = result ;
				string[] lines = buffer.Split('\n');
				output = $"Procedure Name : \n{SPCombo . SelectedItem . ToString ( ) . ToUpper ( )}\n\n";
				foreach ( var item in lines )
				{
					if ( item . ToUpper ( ) . Contains ( "@" ) )
						output += item;
					if ( showall == false )
					{
						if ( item . ToUpper ( ) == "AS\r" )
							break;
					}
				}
				output += $"\n\nHit ESCAPE or Double click in viewer to return to Data Grid View...";
			}
			else if ( result == "COMMENTS" )
			{
				// get comments only
				string buffer = "";
				output = $"Procedure Name : \n{SPCombo . SelectedItem . ToString ( ) . ToUpper ( )}\n\n";
				SqlCommand = "spGetSpecificSchema";
				string args = ProcedureToFind;
				ExecuteStoredProcedure ( SqlCommand , null , "COMMENTS" , args , null );
				//output += $"\n\nHit ESCAPE or Double click in viewer to return to Data Grid View...";
			}
			else
			{
				foreach ( var item in SPCombo . Items )
				{
					if ( ProcedureToFind != "" )
					{
						SqlCommand = "spGetSpecificSchema";
						string args = ProcedureToFind;
						ExecuteStoredProcedure ( SqlCommand , null , "RETURNDATA" , args , null );
						break;
					}
					else
					{
						MessageBox . Show ( $"You MUST provide the name of the Stored Procedure that you want tthe arguments for !" , "Input Error" , MessageBoxButton . OK , MessageBoxImage . Exclamation );
						return "";
					}
				}
			}
			return output;
		}
		//********************************************************************************************************************************************************************************//
		private void ResetFieldDefaults ( TextBox tb , bool direction )
		//********************************************************************************************************************************************************************************//
		{
			if ( tb == null )
			{
				if ( SPArgs . Text == "" )
				{
					SPArgs . Text = "Enter arguments here ...";
					SPArgs . Foreground = FindResource ( "Gray0" ) as SolidColorBrush;
					SPArgs . Refresh ( );
				}
				if ( SqlCommandString . Text == "" )
				{
					SqlCommandString . Text = "Enter SQL command ...";
					SqlCommandString . Foreground = FindResource ( "Gray0" ) as SolidColorBrush;
					SqlCommandString . Refresh ( );
				}
				if ( DbToBeCreated . Text == "" )
				{
					DbToBeCreated . Text = "Enter Table name...";
					DbToBeCreated . Foreground = FindResource ( "Gray0" ) as SolidColorBrush;
					DbToBeCreated . Refresh ( );
				}
			}
			else
			{
				if ( direction )
				{
					if ( tb . Text == "Enter arguments here ..." )
					{
						tb . Text = "";
						tb . Foreground = FindResource ( "Red5" ) as SolidColorBrush;
					}
					if ( tb . Text == "Enter SQL command ..." )
					{
						tb . Text = "";
						tb . Foreground = FindResource ( "Red5" ) as SolidColorBrush;
						tb . Refresh ( );
					}
					if ( tb . Text == "Enter Table name..." )
					{
						tb . Text = "";
						tb . Foreground = FindResource ( "Red5" ) as SolidColorBrush;
					}
					tb . Refresh ( );
				}
				else
				{
					if ( tb . Text == "" )
					{
						tb . Text = "Enter arguments here ...";
						tb . Foreground = FindResource ( "Gray0" ) as SolidColorBrush;
						tb . Refresh ( );
					}
					if ( tb . Text == "" )
					{
						tb . Text = "Enter SQL command ...";
						tb . Foreground = FindResource ( "Gray0" ) as SolidColorBrush;
						tb . Refresh ( );
					}
					if ( tb . Text == "" )
					{
						tb . Text = "Enter Table name...";
						tb . Foreground = FindResource ( "Gray0" ) as SolidColorBrush;
					}
				}
			}
		}
		//********************************************************************************************************************************************************************************//
		private void SqlCommandString_LostFocus ( object sender , RoutedEventArgs e )
		//********************************************************************************************************************************************************************************//
		{
			ResetFieldDefaults ( sender as TextBox , false );
		}
		//********************************************************************************************************************************************************************************//
		private void SPArguments_Click ( object sender , RoutedEventArgs e )
		//********************************************************************************************************************************************************************************//
		{
			//SPArguments . Focus ( );
			SqlCommand = "spGetSpecificSchema";
			showall = false;
			GetSpecificSPArguments ( SPCombo . SelectedItem . ToString ( ) , "VIEWONLY" , showall );
			DbCopiedResult . Text = $"Sp [SPGETSPECIFICSCHEMA] command to display currently selected Stored Procedure completed successfully ...";

			//ExecuteStoredProcedure ( SqlCommand , null , "RETURNDATA" , args , null );
		}
		//********************************************************************************************************************************************************************************//
		private void SPArgs_LostFocus ( object sender , RoutedEventArgs e )
		//********************************************************************************************************************************************************************************//
		{
			ResetFieldDefaults ( sender as TextBox , false );
		}
		//********************************************************************************************************************************************************************************//
		private void SpArgsList_PreviewKeyDown ( object sender , KeyEventArgs e )
		//********************************************************************************************************************************************************************************//
		{
			//if ( e . Key == Key . Escape )
			//{
			//	SpArgsList . Visibility = Visibility . Hidden;
			//	ProcessSp = false;
			//}
			//if ( e . Key == Key . Enter )
			//{
			//	SpArgsList . Visibility = Visibility . Hidden;
			//	ProcessSp = true;
			//	// Go ahead and process the SP
			//	ExecuteSQLCommand_Click ( null , null );
			//}
		}
		//********************************************************************************************************************************************************************************//
		private void ViewSPArgs_Click ( object sender , RoutedEventArgs e )
		//********************************************************************************************************************************************************************************//
		{
			//Preview  SP arguments  info in TextBox for current item in Combo
			Mouse . OverrideCursor = Cursors . Wait;
			showall = false;
			string str = GetSpArgs ( SPCombo . SelectedItem . ToString ( ) );
			DbCopiedResult . Text = $"Display selected Stored Procedure Command completed successfully ...";

			Mouse . OverrideCursor = Cursors . Arrow;
		}
		//********************************************************************************************************************************************************************************//
		private void TestGenericLinq ( object sender , RoutedEventArgs e )
		//********************************************************************************************************************************************************************************//
		{
			//int columncount=2;
			//ObservableCollection<GenericClass> Generics = new ObservableCollection<GenericClass>();
			//GenericClass gc = new   GenericClass();
			////			ObservableCollection<int> intcollection = new   ObservableCollection<int>();
			//Generics . Clear ( );
			//gc . field1 = "abc";
			//gc . field2 = "bbc";
			//Generics . Add ( gc );
			//gc . field1 = "ccd";
			//gc . field2 = "dcd";
			//Generics . Add ( gc );

			//LinqDelegate3<string, string, int> ld = teststr3str;
			//var accounts = from items in Generics
			//		   where  ld(items . field1 , items.field2, 25)
			//		   orderby items . field1
			//		   select items;
			////where ( items . field1 == items.field2 )
			//LoadActiveRowsOnlyInGrid ( DisplayGrid , Generics , columncount );
			//DisplayGrid . Visibility = Visibility . Visible;
			//DisplayGrid . Refresh ( );

			//LinqDelegate1<string,string> ld1 = teststr2strnot;
			//var accounts2 = from items in Generics
			//		    where  ld1(items . field1 , items.field2)
			//		    orderby items . field1
			//		    select items;
			//LoadActiveRowsOnlyInGrid ( DisplayGrid , Generics , columncount );
			//DisplayGrid . Visibility = Visibility . Visible;
			//DisplayGrid . Refresh ( );

			//LinqDelegate<string, string> ld2 = teststr2str;
			//var accounts3 = from items in Generics
			//		    where  ld2(items . field1 , items.field2)
			//		    orderby items . field1
			//		    select items;

			//LoadActiveRowsOnlyInGrid ( DisplayGrid , Generics , columncount );
			//DisplayGrid . Visibility = Visibility . Visible;
			//DisplayGrid . Refresh ( );

			///// process 2 Lists of Ints for match
			//List<int> arr1 = new List<int>();
			//List<int> arr2 = new List<int>();
			//List<int> arresult = new List<int>();
			//int y=0;
			//for ( int x = 0 ; x < 10 ; x++ )
			//{
			//	arr1 . Add ( x );
			//	arr2 . Add ( y++ );
			//}
			//// in line call to linq method
			//IEnumerable <int> accounts4 = Checklists ( arr1 , arr2 );
			//// got our list, now parse it into Generic class
			//Generics . Clear ( );
			//foreach ( var item in accounts4 )
			//{
			//	gc = new GenericClass ( );
			//	gc . field1 = item . ToString ( );
			//	Generics . Add ( gc );
			//}
			////Display results
			//DisplayGrid . ItemsSource = Generics;
			//DisplayGrid . Visibility = Visibility . Visible;
			//DisplayGrid . Refresh ( );

			//arr1 . Clear ( );
			//arr2 . Clear ( );
			//for ( int x = 0 ; x < 10 ; x++ )
			//{
			//	arr1 . Add ( x );
			//	arr2 . Add ( ++y );
			//}
			//IEnumerable <int> accounts5 = Checklists ( arr1 , arr2 );
			//// got our list, now parse it into Generic class
			//Generics . Clear ( );
			//foreach ( var item in accounts5 )
			//{
			//	gc = new GenericClass ( );
			//	gc . field1 = item . ToString ( );
			//	Generics . Add ( gc );
			//}
			////Display results
			//DisplayGrid . ItemsSource = Generics;
			//DisplayGrid . Visibility = Visibility . Visible;
			//DisplayGrid . Refresh ( );

			//Generics . Clear ( );
			//List<int> res = ListCompare(arr1, arr2);
			////Display results
			//DisplayGrid . ItemsSource = res;
			//DisplayGrid . Visibility = Visibility . Visible;
			//DisplayGrid . Refresh ( );

		}

		//public List<TResult> ListCompare<TInput, TResult> (
		//	List<TInput> list1 , List<TInput> list2 ,
		//	Func<TInput , TInput , TResult> modified ,
		//	Func<TInput , TResult> added ,
		//	Func<TInput , TResult> deleted )
		//********************************************************************************************************************************************************************************//
		public List<int> ListCompare (
			List<int> list1 , List<int> list2 )
		//********************************************************************************************************************************************************************************//
		{
			// Displaying Matching Records from List1 and List2 by ID
			var matchingRecords= list1
				  .Where(e1 => list2.Any(e2 => e2.Equals(e1)))
				  .Select(e1 =>
				  {
					  var e2 = list2.First(e => e.Equals(e1));
					  return  e2;
				  });
			return null;
		}
		//********************************************************************************************************************************************************************************//
		private IEnumerable<int> Checklists ( List<int> arr1 , List<int> arr2 )
		//********************************************************************************************************************************************************************************//
		{
			int x = 0;
			var a = arr1.GetType();
			var b = arr2.GetType();
			//check if the types are same
			if ( a == b )
			{
				//check if the count is same
				if ( arr1 . Count ( ) == arr2 . Count ( ) )
				{
					var result = from itm in arr1
							 where itm == arr2[x++]
							 select itm;
					return result;
				}
			}
			return null;
		}

		#region Delegate methods
		public bool teststr3str<T, U> ( T arg1 , T arg2 , U arg3 )
		{
			bool b = arg1. Equals ( arg2);
			Console . WriteLine ( $"{arg3}" );
			return b;
		}
		public bool teststr2str<T> ( T arg1 , T arg2 )
		{
			bool c = ((T)arg1). Equals ((T)arg2);
			//			bool b = arg1. Equals ( arg2);
			return c;
		}
		public bool teststr2strnot<T> ( T arg1 , T arg2 )
		{
			bool c = ((T)arg1). Equals ((T)arg2);
			//			bool b = !(arg1. Equals ( arg2));
			return c;
		}
		public bool testint2int<T> ( T arg1 , T arg2 )
		{
			//Compare INT's
			//			if(typeof(T) == int)
			//int a = Convert.ToInt32(arg1 as T);
			//int b = Convert.ToInt32(arg2);
			bool c = ((T)arg1). Equals ((T)arg2);
			return c;
		}
		public static bool testint2intnot ( int arg1 , int arg2 )
		{
			bool b = !(arg1. Equals ( arg2));
			return b;
		}
		public static bool testdec2dect ( decimal arg1 , decimal arg2 )
		{
			bool b = arg1. Equals ( arg2);
			return b;
		}
		#endregion Delegate methods

		//********************************************************************************************************************************************************************************//
		private void SpArgsList_PreviewMouseLeftButtonDown ( object sender , MouseButtonEventArgs e )
		//********************************************************************************************************************************************************************************//
		{
			// can we drag the info window ??
			//			SpArgsList . CaptureMouse ( );
		}
		//********************************************************************************************************************************************************************************//
		private void SpArgsList_PreviewMouseLeftButtonUp ( object sender , MouseButtonEventArgs e )
		//********************************************************************************************************************************************************************************//
		{
			//			SpArgsList . ReleaseMouseCapture ( );
		}
		//********************************************************************************************************************************************************************************//
		private void SpArgsList_MouseMove ( object sender , MouseEventArgs e )
		//********************************************************************************************************************************************************************************//
		{
			//Point pt  = e . GetPosition (this );
			//Console . WriteLine ( $"{pt . X}, {pt . Y}" );
		}
		//********************************************************************************************************************************************************************************//
		private void ViewFullSP_Click ( object sender , RoutedEventArgs e )
		//********************************************************************************************************************************************************************************//
		{
			// view complete script
			ViewFullProcCode . Focus ( );
			string script = GetSpArgs ( SPCombo . SelectedItem . ToString ( ) , true );
			GridData_Display . Text = $"Procedure Name : {SPCombo . SelectedItem . ToString ( ) . ToUpper ( )}\n\nPress Esc or Dbl-Click to return to Grid view...\n";
			GridData_Display . Text += script;
			GridData_Display . Text += $"\n\nPress Esc or Dbl-Click to hide this viewer...\n";
			GridData_Display . Visibility = Visibility . Visible;
			DbCopiedResult . Text = $"Display Full code  for Stored Procedure {SPCombo . SelectedItem . ToString ( ) . ToUpper ( )} Command completed successfully ...";

			// hide grid
			togglevisibility ( false , "" );
			SetViewButtons ( 2 , ( GridData_Display . Visibility == Visibility . Visible ? true : false ) , ( DisplayGrid . Visibility == Visibility . Visible ? true : false ) );
			GridData_Display . Focus ( );
		}
		//********************************************************************************************************************************************************************************//
		private void GridData_Display_PreviewKeyDown ( object sender , KeyEventArgs e )
		//********************************************************************************************************************************************************************************//
		{
			if ( e . Key == Key . Escape )
			{
				GridData_Display . Visibility = Visibility . Hidden;
				SetViewButtons ( 1 , ( GridData_Display . Visibility == Visibility . Visible ? true : false ) , ( DisplayGrid . Visibility == Visibility . Visible ? true : false ) );
			}
		}
		//********************************************************************************************************************************************************************************//
		private void CheckKeyPress ( KeyEventArgs e )
		//********************************************************************************************************************************************************************************//
		{
			//if ( e . Key == Key . Escape )
			//	SpArgsList . Visibility = Visibility . Hidden;
			//if ( e . Key == Key . Enter )
			//{
			//	SpArgsList . Visibility = Visibility . Hidden;
			//	ProcessSp = true;
			//	// Go ahead and process the SP
			//	ExecuteSQLCommand_Click ( null , null );
			//}
		}
		//********************************************************************************************************************************************************************************//
		private void SpArgsList_KeyDown ( object sender , KeyEventArgs e )
		//********************************************************************************************************************************************************************************//
		{
			//			CheckKeyPress ( e );
		}
		//********************************************************************************************************************************************************************************//
		private void SpArgsListtbox_MouseDoubleClick ( object sender , MouseButtonEventArgs e )
		//********************************************************************************************************************************************************************************//
		{
			//			SpArgsList . Visibility = Visibility . Hidden;
		}
		//********************************************************************************************************************************************************************************//
		private void TextBlock_PreviewKeyDown ( object sender , KeyEventArgs e )
		//********************************************************************************************************************************************************************************//
		{
			CheckKeyPress ( e );
		}
		//********************************************************************************************************************************************************************************//
		private void Button_Click ( object sender , RoutedEventArgs e )
		//********************************************************************************************************************************************************************************//
		{
			//			SpArgsList . Visibility = Visibility . Hidden;
		}
		//********************************************************************************************************************************************************************************//
		private void CloseViewer_PreviewMouseDown ( object sender , MouseButtonEventArgs e )
		//********************************************************************************************************************************************************************************//
		{
			//			SpArgsList . Visibility = Visibility . Hidden;
		}
		//********************************************************************************************************************************************************************************//
		private void CloseViewer_MouseEnter ( object sender , MouseEventArgs e )
		//********************************************************************************************************************************************************************************//
		{
			Label  b = sender as Label;
			b . Background = FindResource ( "Green5" ) as SolidColorBrush;
		}
		//********************************************************************************************************************************************************************************//
		private void CloseViewer_MouseLeave ( object sender , MouseEventArgs e )
		//********************************************************************************************************************************************************************************//
		{
			Label b = sender as Label;
			b . Background = FindResource ( "Red5" ) as SolidColorBrush;
		}
		//********************************************************************************************************************************************************************************//
		private void CloseViewer_MouseMove ( object sender , MouseEventArgs e )
		//********************************************************************************************************************************************************************************//
		{
			Label b = sender as Label;
			b . Background = FindResource ( "Green5" ) as SolidColorBrush;
		}
		//********************************************************************************************************************************************************************************//
		private void Close_Click ( object sender , RoutedEventArgs e )
		//********************************************************************************************************************************************************************************//
		{
			Close . Focus ( );
			this . Close ( );
		}
		//********************************************************************************************************************************************************************************//
		private void ViewSPArgs_PreviewMouseLeftButtonDown ( object sender , MouseButtonEventArgs e )
		//********************************************************************************************************************************************************************************//
		{

		}
		//********************************************************************************************************************************************************************************//
		private void ChangeFocus ( object sender , MouseButtonEventArgs e )
		//********************************************************************************************************************************************************************************//
		{
			ResetFieldDefaults ( sender as TextBox , true );
			this . Focus ( );
		}
		//********************************************************************************************************************************************************************************//
		private void DbToBeCreated_LostFocus ( object sender , RoutedEventArgs e )
		//********************************************************************************************************************************************************************************//
		{
			ResetFieldDefaults ( sender as TextBox , false );
		}
		//********************************************************************************************************************************************************************************//
		private void DbToBeCreated_GotFocus ( object sender , RoutedEventArgs e )
		//********************************************************************************************************************************************************************************//
		{

		}

		//********************************************************************************************************************************************************************************//
		private void ExecSqlCommand_Copy_Click ( object sender , RoutedEventArgs e )
		//********************************************************************************************************************************************************************************//
		{
			//Refresh both combos
			LoadTablesCombo ( );
			LoadSPCombo ( );
		}

		//********************************************************************************************************************************************************************************//
		private void Testing1_Click ( object sender , RoutedEventArgs e )
		//********************************************************************************************************************************************************************************//
		{
			if ( img1 . Visibility == Visibility . Hidden )
				img1 . Visibility = Visibility . Visible;
			else
				img1 . Visibility = Visibility . Hidden;
		}

		//********************************************************************************************************************************************************************************//
		public static T deepClone<T> ( T objtoclone )
		//********************************************************************************************************************************************************************************//
		{
			try
			{
				BinaryFormatter BF = new BinaryFormatter();
				MemoryStream MS = new MemoryStream(1000);
				BF . Serialize ( MS , objtoclone );
				MS . Position = 0;
				return ( T ) BF . Deserialize ( MS );
			}
			catch ( Exception ex )
			{
				Console . WriteLine ( $"" );
			}
			return default ( T );
		}

		//********************************************************************************************************************************************************************************//
		private void SqlCommandString_TextChanged ( object sender , TextChangedEventArgs e )
		//********************************************************************************************************************************************************************************//
		{

		}

		//********************************************************************************************************************************************************************************//
		private void Shortmsgbox_Click ( object sender , RoutedEventArgs e )
		//********************************************************************************************************************************************************************************//
		{


			Mouse . OverrideCursor = Cursors . Arrow;
			Utils . Mbox (
				this ,
					string1: "long Message to show how it wraps in a default message box like this ...." ,
					string2: "This is the lower row of data" ,
					caption: "This is  the caption at top of dialog" ,
					iconstring: "\\icons\\Information.png" ,
					Btn1: MB . YES ,
					Btn2: MB . NO ,
					0,0,
					defButton: MB . NO , false );
			//Utils . Mbox (
			//		this ,
			//			string1: "Short Message to show how it is smaller...." ,
			//			string2: "" ,
			//			caption: "" ,
			//			iconstring: "\\icons\\Information.png" ,
			//			Btn1: MB .OK ,
			//			Btn2: MB . NNULL ,
			//			defButton: MB . OK , true );


			//MessageBox . Show ( "" , "" , MessageBoxButton .YesNoCancel, MessageBoxImage.Warning);
		}

		private void Fullmsgbox_Click ( object sender , RoutedEventArgs e )
		{
			Utils . Mssg (
				caption: "*** SQL Query Error ***" ,
				string1: $"This is the Middle, and main row of data used to \ncreate the information provided.\nThis is  the duplicate to make it longer than the window should be able to use, and  this is  This is  the duplicate to make it longer than the window should be able to use" ,
				string2: "string  2 goes here" ,
				string3: "This is string3, a full width footer style row that can be used," ,
				title: "" ,
				iconstring: "\\icons\\exclaim2.png" ,
				defButton: 2 ,
				Btn1: 1 ,
				Btn2: 2 ,
				Btn3: 3 ,
				Btn4: 4 ,
				btn1Text: "Ok" ,
				btn2Text: "Get on with it" ,
				btn3Text: "Bale out quick" ,
				btn4Text: "Run !!!"
			     );
		}

		//********************************************************************************************************************************************************************************//
		private void Grid_Click ( object sender , RoutedEventArgs e )
		//********************************************************************************************************************************************************************************//
		{
			// Click on the grid triggers this !!
			Console . WriteLine ( $"Grid has been clicked on....." );
		}

		//// list each window that wants to support control capture needs to have so
		//// mousemove can add current item under cursor to the list, and then F11 will display it.
		//List<HitTestResult> ControlsHitList = new List<HitTestResult>();
		//********************************************************************************************************************************************************************************//
		private void Window_PreviewKeyDown ( object sender , KeyEventArgs e )
		//********************************************************************************************************************************************************************************//
		{
			//Utils . Grab_GetObject ( sender , e );
			//if ( Utils . ControlsHitList . Count == 0 )
			//	return;

			e . Handled = true;
			if ( e . Key == Key . F11 )
			{
				var pos = Mouse . GetPosition ( this);
				Utils . Grab_Object ( sender , pos );
				//Utils . Grabscreen ( SQLServerCommands , pos, null , sender as Control );
				Utils . Grabscreen ( SQLServerCommands , Utils . ControlsHitList [ 0 ] . VisualHit , null , sender as Control );
			}
		}

		//********************************************************************************************************************************************************************************//
		private void SelectFile_Click ( object sender , RoutedEventArgs e )
		//********************************************************************************************************************************************************************************//
		{
			// Test entry only
			double imgheight = img1.Height;
			double imgwidth = img1.Width;
			string file=@"C:\users\ianch\Documents\image1";
			RenderTargetBitmap img;

			// create an imageof any control in the window, & optionally save to disk file
			Control ctrl = ShowGrid;
			img = Utils . RenderBitmap ( ctrl );
			if ( img . Width < img1 . Width )
				img1 . Width = img . Width;
			if ( img . Height < img1 . Height )
				img1 . Height = img . Height;
			img1 . Source = img;
			img1 . Refresh ( );
			Utils . SaveImageToFile ( img , "" , "GIF" );

			ctrl = this;
			img = Utils . CreateControlImage ( ctrl , @"C:\\users\ianch\Documents\capturedimage.png" );
			if ( img . Width > imgwidth )
				img1 . Width = imgwidth;
			else
				img1 . Width = img . Width;

			if ( img . Height > imgheight )
				img1 . Height = imgheight;
			else
				img1 . Height = img . Height;
			img1 . Source = img;

			img1 . Refresh ( );
			file = @"C:\users\ianch\Documents\image2.png";
			Utils . SaveImageToFile ( img , file , "JPG" );
		}

		//********************************************************************************************************************************************************************************//
		private void DbToBeCreated_TextChanged ( object sender , TextChangedEventArgs e )
		//********************************************************************************************************************************************************************************//
		{

		}

		//********************************************************************************************************************************************************************************//
		public void Cookies_Click ( object sender , RoutedEventArgs e )
		//********************************************************************************************************************************************************************************//
		{
			// List all cookies
			List<string> lst = new List<string>();
			string output = "";
			string[] tmp;
			GridData_Display . Visibility = Visibility . Visible;
			Cookies . LoadCookiesToCombo ( CookiesCombo , out CookiesList );
			//CookiesCombo . ItemsSource = CookiesList;
			lst = Cookies . ListAllCookies ( );
			foreach ( var item in lst )
			{
				tmp = item . Split ( '^' );
				output += tmp [ 0 ] + "\t Key = '" + tmp [ 1 ] . ToUpper ( ) + "'\tValue = '" + tmp [ 2 ] + "'\n";
			}
			GridData_Display . Text = output;
			togglevisibility ( false );
			CookiesCombo . SelectedItem = DbToCopyCombo . SelectedIndex;
			SetViewButtons ( 2 , ( GridData_Display . Visibility == Visibility . Visible ? true : false ) , ( DisplayGrid . Visibility == Visibility . Visible ? true : false ) );
		}

		//********************************************************************************************************************************************************************************//
		private void Cookies_MouseDoubleClick ( object sender , MouseButtonEventArgs e )
		//********************************************************************************************************************************************************************************//
		{
			CookiesCombo . IsDropDownOpen = true;
		}

		//********************************************************************************************************************************************************************************//
		private void Cookies_PreviewMouseLeftButtonDn ( object sender , MouseButtonEventArgs e )
		//********************************************************************************************************************************************************************************//
		{

		}

		//********************************************************************************************************************************************************************************//
		private void Cookies_SelectionChanged ( object sender , SelectionChangedEventArgs e )
		//********************************************************************************************************************************************************************************//
		{
			CookiesCombo . Focus ( );
			ComboBox cb = sender as  ComboBox;
			ComboBoxItem cbitem = new ComboBoxItem();
			cbitem = cb . SelectedItem as ComboBoxItem;
			//			var v = e . Source.ToString();
			if ( cbitem != null )
			{
				cb . SelectedItem = cb . SelectedIndex;
				cbitem . Background = FindResource ( "Blue1" ) as SolidColorBrush;
				cbitem . Foreground = FindResource ( "White0" ) as SolidColorBrush;
				cb . Refresh ( );
			}
			CookiesCombo . Focus ( );
		}

		//********************************************************************************************************************************************************************************//
		public void NewCookie_Click ( object sender , RoutedEventArgs e )
		//********************************************************************************************************************************************************************************//
		{
			NewCookie nc = new NewCookie(this);
			Dlgresult dr = new Dlgresult();

			nc . ShowDialog ( );
			if ( defvars . CookieAdded == false )
			{
				defvars . CookieAdded = false;
			}
			else
				Cookies_Click ( null , null );
			DbCopiedResult . Text = $"Show New Cookie Window loaded successfully ...";
			defvars . CookieAdded = false;
		}

		//********************************************************************************************************************************************************************************//
		private void showcookie_Click ( object sender , RoutedEventArgs e )
		//********************************************************************************************************************************************************************************//
		{
			string info="";
			GridData_Display . Text = "";
			GridData_Display . Text = $"\nPress Esc or Dbl-Click to hide this viewer...\n";
			info = Cookies . ShowAllCookieData ( out int total , "" );
			GridData_Display . Text += $"\nFull Details of ALL {total} Cookies \n";
			GridData_Display . Text += info;
			GridData_Display . Text += $"\n\nPress Esc or Dbl-Click to hide this viewer...\n";
			TextViewerBtn . Content = "Hide Text Viewer";
			// hide grid
			togglevisibility ( false , "" );
			DbCopiedResult . Text = $"Show All Cookies Command completed successfully ...";
			GridData_Display . Visibility = Visibility . Visible;
			SetViewButtons ( 1 , ( GridData_Display . Visibility == Visibility . Visible ? true : false ) , ( DisplayGrid . Visibility == Visibility . Visible ? true : false ) );
			GridData_Display . Focus ( );
		}

		//********************************************************************************************************************************************************************************//
		private void CookiesCombo_PreviewMouseRightButtonDown ( object sender , MouseButtonEventArgs e )
		//********************************************************************************************************************************************************************************//
		{
			// show selected cookie on screen
			string tmp="";
			string[] flds;
			ComboBox cb = sender as ComboBox;
			tmp = cb . SelectedItem . ToString ( );
			string input = CookiesCombo.SelectedItem.ToString();
			flds = input . Split ( '=' );
			tmp = flds [ 1 ] . Substring ( 1 , flds [ 1 ] . Length - 2 );
			//CookiesList =  Cookies . ListAllCookies ( );
			tmp = Cookies . ShowAllCookieData ( out int total , tmp );
			GridData_Display . Text = $"\nPress Esc or Dbl-Click to hide this viewer...\n";
			GridData_Display . Text += $"\nFull Details for Cookie [{flds [ 1 ]}]\n\n";
			GridData_Display . Text += tmp;
			GridData_Display . Text += $"\nPress Esc or Dbl-Click to hide this viewer...\n";
			GridData_Display . Visibility = Visibility . Visible;
			// hide grid
			togglevisibility ( false , "" );
			SetViewButtons ( 2 , ( GridData_Display . Visibility == Visibility . Visible ? true : false ) , ( DisplayGrid . Visibility == Visibility . Visible ? true : false ) );
			GridData_Display . Focus ( );
		}

		private void ShowTextViewer ( object sender , RoutedEventArgs e )
		{
			if ( GridData_Display . Visibility == Visibility . Hidden )
			{
				GridData_Display . Visibility = Visibility . Visible;
				SetViewButtons ( 2 , ( GridData_Display . Visibility == Visibility . Visible ? true : false ) , ( DisplayGrid . Visibility == Visibility . Visible ? true : false ) );
			}
			else
			{
				GridData_Display . Visibility = Visibility . Hidden;
				SetViewButtons ( 2 , ( GridData_Display . Visibility == Visibility . Visible ? true : false ) , ( DisplayGrid . Visibility == Visibility . Visible ? true : false ) );
			}
		}

		private void SPCombo_PreviewMouseRightButtonDown ( object sender , MouseButtonEventArgs e )
		{
			// Show comments in selected SP
			string spname = SPCombo . SelectedItem . ToString ( );
			SqlCommand = "spGetSpecificSchema";
			showall = false;
			string output = GetSpecificSPArguments ( spname , "COMMENTS" , showall );
			DbCopiedResult . Text = $"Sp [SPGETSPECIFICSCHEMA] command to display comments from currently selected Stored Procedure completed successfully ...";
			togglevisibility ( false );
			SetViewButtons ( 2 , ( GridData_Display . Visibility == Visibility . Visible ? true : false ) , ( DisplayGrid . Visibility == Visibility . Visible ? true : false ) );
			e . Handled = true;
		}

		private void showcomments ( object sender , RoutedEventArgs e )
		{
			SPCombo_PreviewMouseRightButtonDown ( null , null );
		}

		private void Exit_Click ( object sender , RoutedEventArgs e )
		{
			this . Close ( );
		}

		private void IsMouseMove ( object sender , MouseEventArgs e )
		{
			Utils . IsMouseMove ( sender , e );
		}

		private void DbToCopyCombo_PreviewMouseRightButtonUp ( object sender , MouseButtonEventArgs e )
		{
			// Get the data from the selected Db and display it in generic grid
			// Generic call that wil return the results of any valid SQL select command as an Observable colection<GenericClass>
			Dictionary < string, string > dic = new Dictionary<string, string>();
			GenericClass gcc = new GenericClass();
			ObservableCollection< GenericClass > generic = new ObservableCollection<GenericClass> ( );
			string errmsg="";
			generic = DapperGeneric<Dictionary<string , string> , GenericClass , bool> . CreateFromDictionary (
				 dic ,
				gcc ,
				$"select * from {DbToCopyCombo . SelectedItem . ToString ( )}" ,
				 ref errmsg );

			if ( errmsg != "" )
			{
				MessageBox . Show ( $"The SQL Query you entered returned the following Error ?\n\n[{errmsg . ToUpper ( )}]" , "SQL error?" );
				Mouse . OverrideCursor = Cursors . Arrow;
				return;
			}
			if ( generic . Count == 0 )
			{
				Mouse . OverrideCursor = Cursors . Arrow;
				Utils . Mbox ( this ,
					string1: $"The selected Data table \n\n[{DbToCopyCombo . SelectedItem . ToString ( ) . ToUpper ( )}] \n\nwas read successfully but returned Zero records" ,
					caption: "Sql Error" ,
					Btn1: mb . OK ,
					Btn2: mb . NNULL ,
					defButton: mb . OK );

				Mouse . OverrideCursor = Cursors . Arrow;
			}
			DisplayGrid . ItemsSource = null;
			DisplayGrid . Items . Clear ( );
			DisplayGrid . ItemsSource = generic;
			DisplayGrid . SelectedIndex = 0;
			DisplayGrid . Visibility = Visibility . Visible;
			UpdateUniversalViewer ( );
			togglevisibility ( true , DbToCopyCombo . SelectedItem . ToString ( ) . ToUpper ( ) );
			SetViewButtons ( 1 , ( GridData_Display . Visibility == Visibility . Visible ? true : false ) , ( DisplayGrid . Visibility == Visibility . Visible ? true : false ) );
			DisplayGrid . Refresh ( );
			DisplayGrid . Focus ( );
			e . Handled = true;
			return;
		}

		private void FullGridView_Click ( object sender , RoutedEventArgs e )
		{
			// load the new Edit Window that lets the data from a GenericClass grid entry to be edited and even saved to a table
			// Setup the Datagrid here after load but before show
			Mouse . OverrideCursor = Cursors . Wait;
			FullGridViewer fgv = new FullGridViewer( );
			// pass to the View window grid the itemssource for the table selected so it can display it...
			fgv . UniversalGrid . ItemsSource = DisplayGrid.ItemsSource;
			string rec  = DisplayGrid.SelectedItem.ToString();
			fgv . currec = rec;
			// Set the total recs counter on the viewer window
			FullGridViewer . reccount = Generics . Count;
			// show Table name on viewer window
			FullGridViewer . Tablename = BankNameLabel.Text;

			// save a pointer ot the viewer window in global
			fgviewer = fgv;
			fgv . Show ( );
			Mouse . OverrideCursor = Cursors . Arrow;
		}
		public void UpdateUniversalViewer ( )
		{
			if ( defvars . FullViewer )
			{
				fgviewer . UniversalGrid . ItemsSource = DisplayGrid . ItemsSource;
				FullGridViewer . reccount = Generics . Count;
				fgviewer . UniversalGrid_Loaded1 (null, null );
				string tmp = BankNameLabel . Text != "" ? BankNameLabel . Text : "Unknown";
				fgviewer . Title = $"Full Grid Viewer : DB : [{tmp}]";
				fgviewer . Refresh ( );
			}
		}
	}
	//private void ButtonTesting_MouseMove ( object sender , MouseEventArgs e )
	//{
	//	if ( DesignerProperties . GetIsInDesignMode ( this ) == false )
	//	{
	//		if ( Hitcontrol . IsHitActive )// && Hitcontrol . ActiveHitsObject != null )
	//		{
	//			// Hits Active option is Active
	//			var Mousecheck = new MouseCheck ( DoMouseCheck );

	//			// call delegate
	//			if ( delegatetest ( e , Hitcontrol , Mousecheck , "" , null ) )
	//			{
	//				if ( Hitcontrol . IsHitActive && Hitcontrol . ActiveHitName != "" )
	//					if ( Mousecheck ( e , Hitcontrol , Hitcontrol . ActiveHitName , Hitcontrol . ActiveHitsObject ) == false )
	//					{
	//						if ( !LockOutput )
	//							currentpoint . Items . Clear ( );
	//					}
	//					else
	//					{
	//						//object obj = new ShadowLabelControl ( );
	//						//MousoverHandlers.GetObjectHierarchy ( obj,  "" );
	//						ListAllMouseoverTargets ( Hitcontrol . ActiveHitName );
	//					}
	//			}
	//		}
	//		else if ( Hitcontrol . ObjectStats && Hitcontrol . ObjectStatsString != "" )
	//		{
	//			if ( !LockOutput )
	//				currentpoint . Items . Clear ( );
	//			// Hits Active option is Active
	//			var Mousecheck = new MouseCheck ( DoMouseCheck );
	//			// call delegate
	//			if ( delegatetest ( e , Hitcontrol , Mousecheck , "" , null ) )
	//				Mousecheck ( e , Hitcontrol , "" , null );

	//			if ( IsControlhit ( Hitcontrol . ObjectStatsString ) == false )
	//				return;
	//			string currentitem = Hitcontrol . ObjectStatsString;
	//			Hitcontrol . ObjectStatsDp = ( FrameworkElement ) e . Source;
	//			if ( currentitem . Contains ( Hitcontrol . ObjectStatsString ) )
	//				DisplayControlStats ( Hitcontrol . ObjectStatsDp , Hitcontrol . ObjectStatsString );
	//		}
	//		else
	//		{
	//			if ( Hitcontrol . ShowAll )
	//			{
	//				var Mousecheck = new MouseCheck ( DoMouseCheck );
	//				// call delegate
	//				if ( delegatetest ( e , Hitcontrol , Mousecheck , "" , null ) )
	//					Mousecheck ( e , Hitcontrol , "" , null );
	//			}
	//		}
	//	}
	//}
}
//************************************************************************************************************************************//
//***********************************************END OF CLASS *****************************************************************//
//************************************************************************************************************************************//

#region other classes
public class MyInts
{
	public MyInts ( )
	{

	}
	private int _INT;

	public int INT
	{
		get { return _INT; }
		set { _INT = value; }
	}

}
public class arrayclass
{
	public string [ ] fields { get; set; }
	public string fldname { get; set; }
}
public class SelectionEntry
{
	public string Entry { get; set; }
}
public class SingleFieldClass : IEnumerable
{
	public SingleFieldClass ( )
	{

	}
	public string Linestring { get; set; }

	public IEnumerator GetEnumerator ( )
	{
		return this . GetEnumerator ( );
	}
}
#endregion other classes



