
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
using System . Text;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Input;
using System . Windows . Media;

using WPFPages . ViewModels;

namespace WPFPages . Views
{
	/// <summary>
	/// Interaction logic for SqlServerCommands.xaml
	/// </summary>
	public partial class SqlServerCommands : Window
	{
		public DapperClass Db = new DapperClass ();
		public List<string> ReceivedDbData = new List<string>();
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
		private void LoadListView ( )
		{
			listRound . Items . Add ( "Andhra Pradesh" );
			listRound . Items . Add ( "Arunachal Pradesh" );
			listRound . Items . Add ( "Assam" );
			listRound . Items . Add ( "Bihar" );
			listRound . Items . Add ( "Chhattisgarh" );
			listRound . Items . Add ( "Goa" );
			listRound . Items . Add ( "Gujarat" );
			listRound . Items . Add ( "Haryana" );
			listRound . Items . Add ( "Himachal Pradesh" );
			listRound . Items . Add ( "Jharkhand" );
			listRound . Items . Add ( "Karnataka" );
			listRound . Items . Add ( "Kerala" );
			listRound . Items . Add ( "Madhya Pradesh" );
			listRound . Items . Add ( "Maharashtra" );
			listRound . Items . Add ( "Manipur" );
			listRound . Items . Add ( "Meghalaya" );
			listRound . Items . Add ( "Mizoram" );
			listRound . Items . Add ( "Nagaland" );
			listRound . Items . Add ( "Odisha" );
			listRound . Items . Add ( "Punjab" );
			listRound . Items . Add ( "Rajasthan" );
			listRound . Items . Add ( "Sikkim" );
			listRound . Items . Add ( "Tamil Nadu" );
			listRound . Items . Add ( "Telangana" );
			listRound . Items . Add ( "Tripura" );
			listRound . Items . Add ( "Uttar Pradesh" );
			listRound . Items . Add ( "Uttarakhand" );
			listRound . Items . Add ( "West Bengal" );
		}
		protected override void OnInitialized ( EventArgs e )
		{
			if ( DesignerProperties . GetIsInDesignMode ( this ) == true )
				LoadListView ( );
			else
				LoadListView ( );
			base . OnInitialized ( e );
		}
		private void Window_Loaded ( object sender , RoutedEventArgs e )
		{
			string dbnametoopen = "";
			Utils . SetupWindowDrag ( this );
			togglevisibility ( false );
			if ( ShowInGrid . IsChecked == true )
				ShowGridFlag = true;
			// Populate the Db Tables && Stored Procedures Combo
			LoadTablesCombo ( );
			LoadSPCombo ( );
		}
		public override void OnApplyTemplate ( )
		{
			base . OnApplyTemplate ( );
			if ( Template != null )
			{
				var v = this . GetTemplateChild ( "ComboScroll" );
			}
			return;

		}
		public SqlServerCommands ( )
		{
			InitializeComponent ( );
			if ( DesignerProperties . GetIsInDesignMode ( this ) == true )
			{
				//				if ( System . Reflection . Assembly . GetExecutingAssembly ( ) . Location . Contains ( "VisualStudio" ) )
				LoadListView ( );
				//				else
				//					LoadListView ( );
			}
		}

		private void CopyDbBtn_Click ( object sender , RoutedEventArgs e )
		{
			// Copy Db to new Db  "Select * from xxx into yyy" using dapper
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
				return;
			}
			if ( AllowOverWriteFlag . IsChecked == false )
			{
				// Allow Overwrite is NOT checked, so we Need to check if the destination already exists
				OriginalSqlCommand = SqlCommand;
				//SqlCommand = $"select count(*) as [Count] from {recip}";
				CurrentCommandLabel . Text = $"[{SqlCommand}]";
				DapperSupport . PerformSqlDbTest ( SqlCommand , out errorstring );
				// reset original SQL command string
				SqlCommand = OriginalSqlCommand;
				if ( errorstring . ToUpper ( ) . Contains ( "THERE IS ALREADY AN" ) )
				{
					var dr2 = MessageBox . Show ( $"The destination Db already exists !\nDo you want to go ahead and overwrite it ??" ,
					"Sql Command",
					MessageBoxButton.YesNo,
					MessageBoxImage.Question);
					IEnumerable DbResult=null;
					if ( dr2 == MessageBoxResult . Yes )
					{
						// This call perfoms the Copy process
						//int result = DapperSupport . PerformSqlExecuteCommand ( SqlCommand , out errormsg);
						ObservableCollection<GenericClass> Generics = new ObservableCollection<GenericClass>();
						ObservableCollection<BankAccountViewModel> bvmparam = new ObservableCollection<BankAccountViewModel>();
						Dictionary <string, object>dict = new Dictionary<string, object>();

						DbResult = DapperSupport . ExecuteSPGeneric ( Generics ,
							$"spCopyDb" ,
							$" {donor} {recip}, 1" ,
							"" ,
							"" ,
							"" ,
							Generics ,
							dict ,
							out bvmparam ,
							out errorstring );
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
							int result = DapperSupport . PerformSqlExecuteCommand ( SqlCommand , out errormsg);
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
					}
					else if ( errormsg == "" )
					{
						// definitely does NOT exist, so proceed as normal
						// This call perfoms the Copy process
						int result = DapperSupport . PerformSqlExecuteCommand ( SqlCommand , out errormsg);
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
				return;
			}
			else
			{
				// overwrite flag is set to ALLOW Overwrite, so just do it
				SqlCommand = $"SpCopyDb {donor},{recip}, 1";
				CurrentCommandLabel . Text = $"[{SqlCommand}]";
				int result = DapperSupport . PerformSqlExecuteCommand ( SqlCommand , out errormsg);
				DbCopiedResult . Text = errormsg;
				DbCopiedResult . Refresh ( );
				if ( errormsg . ToUpper ( ) . Contains ( "THERE IS ALREADY AN" ) )
				{
					if ( AllowOverWriteFlag . IsChecked == true )
					{
						// Save Db name so we can access it later
						CurrentDbName = recip . ToUpper ( );
						// Load the grid and display it
						LoadShowDbGrid ( recip );
						LoadSPCombo ( );
						if ( ShowGridFlag == true )
							DbCopiedResult . Text = "Copy completed successful, Db is now open in the Data Grid for you... ";
						else
							DbCopiedResult . Text = "The Db Copy operation completed successfully. You can open it in the Data Grid ...";
					}
				}
				else if ( errormsg . ToUpper ( ) . Contains ( "ERROR" ) )
				{
					MessageBox . Show ( $"An error has been encountered, the informaton returned is\n : {errormsg}" , "Sql Query Error" , MessageBoxButton . OK , MessageBoxImage . Error );
					return;
				}
				else
				{
					// Save Db name so we can access it later
					CurrentDbName = recip . ToUpper ( );
					DbCopiedResult . Text = $"Db [{donor . ToUpper ( )}] was copied to [{recip . ToUpper ( )}] successfully so you can View it in the Data Grid...";
					LoadSPCombo ( );
					// Load the grid and display it
					//LoadShowDbGrid ( recip );
				}
			}
		}
		private void CopyList ( List<string> templist )
		{
			ReceivedDbData . Clear ( );
			foreach ( var item in templist )
			{
				ReceivedDbData . Add ( item );
			}
		}
		private void LoadShowDbGrid ( string recipient )
		{
			if ( ReceivedDbData . Count == 0 )
			{                       // creates and loads Db into grid
				List<string> templist = new List<string>();
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
					CreateDatabase ( DisplayGrid , ReceivedDbData );
				DbCopiedResult . Text = $"Db [{SqlCommand}] performed successfully...";
			}
			else
			{
				// creates and loads Db into grid  & Displays the grid
				if ( ReceivedDbData . Count > 0 )
					CreateDatabase ( DisplayGrid , ReceivedDbData );
			}
			if ( ShowGridFlag == false )
			{
				togglevisibility ( false );
				DbCopiedResult . Text = "Copy successful, Db is available for access... ";
				this . Title = $"Sql Server Commands : Db : {recipient}";
				//return;
			}
			else
			{
				DbCopiedResult . Text = "Copy successful, Grid is now open... ";
				BankNameLabel . Text = recipient . ToUpper ( );
				togglevisibility ( true );
				this . Title = $"Sql Server Commands : Active Db : {recipient}";
				this . Refresh ( );
			}
			//Clear Command buffers ready for next operation
			OriginalSqlCommand = "";
			SqlCommand = "";
		}
		private void Closegrid_Click ( object sender , RoutedEventArgs e )
		{
			togglevisibility ( false );
		}
		private void ShowGrid_Click ( object sender , RoutedEventArgs e )
		{
			bool flagstatus = ShowGridFlag;
			if ( ShowGrid . Content . ToString ( ) == "Hide Grid" )
			{
				togglevisibility ( false );
				return;
			}
			else
			{
				togglevisibility ( true );
				if ( DisplayGrid . Items . Count == 0 )
				{
					// gotta load it first, so set flag to let us as it may be disabled on the screen
					ShowGridFlag = true;
					LoadShowDbGrid ( CurrentDbName );
					//Reset grid visibility flag
					ShowGridFlag = flagstatus;
					ClearGrid . IsEnabled = true;
				}
				else
					DisplayGrid . Visibility = Visibility . Visible;
				BankNameLabel . Text = CurrentDbName;
				DisplayGrid . Refresh ( );
				DisplayGrid . Focus ( );
			}
		}
		private void DeleteRecipientDbBtn_Click ( object sender , RoutedEventArgs e )
		{
			DapperSupport . DeleteDbTable ( DbToBeCreated . Text );
			//DeleteSource . Opacity = 0.3;
			//MessageBox . Show ( $"The Db should have been deleted., You can now try the Copy button again");

		}
		private void createstringarray ( )
		{
			List<string> strlist = new List<string>();
			string s = "dsg dfgd";
			strlist . Add ( s );
		}
		private void DisplayGrid_MouseDblClick ( object sender , System . Windows . Input . MouseButtonEventArgs e )
		{
			string buffer="";
			GridData_Display . Visibility = Visibility . Visible;
			DisplayGrid . Visibility = Visibility . Hidden;
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
		private void togglevisibility ( bool mode , string DbName = "" )
		{
			if ( DbName == "" )
				DbName = "???";
			if ( mode )
			{
				DisplayGrid . Visibility = Visibility . Visible;
				GridCount . Visibility = Visibility . Visible;
				NameLabel . Visibility = Visibility . Visible;
				BankNameLabel . Visibility = Visibility . Visible;
				GridCount . Text = DisplayGrid . Items . Count . ToString ( );
				BankNameLabel . Text = DbName;
				ShowGrid . Content = "Hide Grid";
			}
			else
			{
				DisplayGrid . Visibility = Visibility . Collapsed;
				GridCount . Visibility = Visibility . Collapsed;
				NameLabel . Visibility = Visibility . Collapsed;
				BankNameLabel . Visibility = Visibility . Collapsed;
				ShowGrid . Content = "Show Grid";
			}
		}
		private void ExecCommand_Click ( object sender , RoutedEventArgs e )
		{
			int result = 0;

			// Button to Execute user entered SQL query
			if ( sender == null )
			{
				// being called to check for existence of a Db
				string DbName = SpArgs.Text;
				string upperstring = SqlCommand . ToUpper ( );
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
					}
				}
				//result = performExec ( );
				CurrentCommandLabel . Text = $"[{SqlCommand}]";
				var queryresult = DapperSupport.PerformSqlDbTest(SqlCommand, out errorstring);
				if ( errorstring != null )
				{
					if ( errorstring . ToUpper ( ) . Contains ( "INVALID OBJECT" ) && errorstring . ToUpper ( ) . Contains ( DbName . ToUpper ( ) ) )
					{
						Console . WriteLine ( "Db does NOT exist" );
						SqlCommand = OriginalSqlCommand;
						result = DapperSupport . PerformSqlExecuteCommand ( SqlCommand , out errorstring );
						Console . WriteLine ( $"Result returned from Sql Server was  {result}" );
					}
					else
					{
						SqlCommand = OriginalSqlCommand;
						result = DapperSupport . PerformSqlExecuteCommand ( SqlCommand , out errorstring );
						if ( errorstring != "" )
						{
							var dr2 = MessageBox . Show ( $"The Command does NOT appear to be valid.\nPlease correct and retry the query" ,
								"Sql Command",
								MessageBoxButton.OK,
								MessageBoxImage.Exclamation);
						}
						else
						{
							// Save Db name so we can access it later
							CurrentDbName = GetDbNameFromCommand ( SqlCommand ) . ToUpper ( );
							// Load the grid and display it
							LoadShowDbGrid ( GetDbNameFromCommand ( SqlCommand ) );
						}
						Console . WriteLine ( $"Result returned from Sql Server was  {result}" );
					}
				}
			}
			else
			{
				// called internally by other methods
				if ( OriginalSqlCommand == "" )
					SqlCommand = SqlCommandString . Text;
				else
					SqlCommand = OriginalSqlCommand;
				if ( SqlCommand == "Enter SQL command ..." )
				{
					MessageBox . Show ( $"Sorry, but you MUST enter a valid SQL Query that can be processed " , "Sql Error Encountered" , MessageBoxButton . OK , MessageBoxImage . Information );
					return;
				}
				CurrentCommandLabel . Text = $"[{SqlCommand}]";
				string DbToOpen="";
				IEnumerable DbResult=null;
				ObservableCollection<GenericClass> Generics = new ObservableCollection<GenericClass>();
				ObservableCollection<BankAccountViewModel> bvmparam = new ObservableCollection<BankAccountViewModel>();
				Dictionary <string, object>dict = new Dictionary<string, object>();
				DbResult = DapperSupport . ExecuteSPGeneric ( Generics ,
					SqlCommand ,
					"" ,
					"" ,
					"" ,
					"" ,
					Generics ,
					dict ,
					out bvmparam ,
					out errorstring );

				if ( errorstring . Contains ( "SQLERROR :" ) )
				{
					MessageBox . Show ( $"An SQL Error occured. SQL Error message was\n : {errorstring . Substring ( 10 )}" );
					CurrentCommandLabel . Text = SqlCommand;
					return;
				}
				else if ( errorstring . Contains ( "UNKNOWN :" ) )
				{
					MessageBox . Show ( $"An undefined SQL Error occured. Error message was\n : {errorstring . Substring ( 9 )}" );
					CurrentCommandLabel . Text = SqlCommand;
					return;
				}
				else
				{
					Console . WriteLine ( $"Records loaded from Sql Server by Query [{SqlCommand}]was  {Generics . Count}" );
					// Command appears to be valid, so process it
					DbCopiedResult . Text = $"SQL Command \n[{SqlCommand . ToUpper ( )}]\ncompleted successfully... ";
					DbCopiedResult . Refresh ( );

					// Save Db name so we can access it later
					CurrentDbName = GetDbNameFromCommand ( SqlCommand ) . ToUpper ( );
					// Load the grid and display it
					//					LoadShowDbGrid ( GetDbNameFromCommand ( SqlCommand ) );
					DisplayGrid . ItemsSource = null;
					DisplayGrid . Items . Clear ( );
					DisplayGrid . ItemsSource = Generics;
					DisplayGrid . Visibility = Visibility . Visible;
					DisplayGrid . Refresh ( );
					togglevisibility ( true , GetDbNameFromCommand ( SqlCommand ) );
				}
			}
			return;
		}
		/// <summary>
		/// Get the Db Name from the SQL command string (if possible)
		/// </summary>
		/// <param name="Sqlcommand"></param>
		/// <returns>Db Name as string</returns>		 		
		private string GetDbNameFromCommand ( string Sqlcommand )
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
		private int performExec ( )
		{
			//SqlCommand = SqlCommandString . Text;

			SqlCommand = OriginalSqlCommand;
			CurrentCommandLabel . Text = $"[{SqlCommand}]";
			int result = DapperSupport . PerformSqlExecuteCommand ( SqlCommand , out errorstring );
			Console . WriteLine ( $"Result returned from Sql Server was  {result}" );
			return result;
		}
		private void SqlCommandString_PreviewMouseLeftButtonUp ( object sender , System . Windows . Input . MouseButtonEventArgs e )
		{
			// Select entire string on entry into the field to make it easier to type an entry
			if ( SqlCommandString . Text == "Enter SQL command ..." )
			{
				SqlCommandString . Text = "";
				SqlCommandString . Foreground = FindResource ( "Black0" ) as SolidColorBrush;
			}
		}
		private void DbToCopy_PreviewMouseLeftButtonUp ( object sender , System . Windows . Input . MouseButtonEventArgs e )
		{
			SelectTextBoxText ( DbToCopyTb );
		}
		private void DbToBeCreated_PreviewMouseLeftButtonUp ( object sender , System . Windows . Input . MouseButtonEventArgs e )
		{
			//SelectTextBoxText ( DbToBeCreated );
			if ( SPArgs . Text . Contains ( "Enter arguments here" ) )
			{
				SPArgs . Text = "";
				SPArgs . Foreground = FindResource ( "Red5" ) as SolidColorBrush;
			}
		}
		private void SelectTextBoxText ( TextBox txtbox )
		{
			txtbox . SelectionLength = txtbox . Text . Length;
			txtbox . SelectionStart = 0;
			txtbox . SelectAll ( );
		}
		private void SqlCommandString_KeyDown ( object sender , System . Windows . Input . KeyEventArgs e )
		{
			if ( e . Key == Key . Enter )
			{
				SqlCommand = SqlCommandString . Text;
				OriginalSqlCommand = SqlCommand;
				ExecCommand_Click ( this , null );
			}
		}
		private void ClearGrid_Click ( object sender , RoutedEventArgs e )
		{
			DisplayGrid . ItemsSource = null;
			DisplayGrid . Items . Clear ( );
			DisplayGrid . Refresh ( );
			BankNameLabel . Text = "";
			GridCount . Text = "";
			this . Title = $"Sql Server Commands : Active Db : 'None'";
			//			ClearGrid . IsEnabled = false;
			ShowInGrid . IsEnabled = true;
			// cant show procs if grid is cleared as we do not have a Db remaining
			ActiveDbName = "";
			// delete last enquiry
			OriginalSqlCommand = "";
			SqlCommand = "";
			CurrentCommandLabel . Text = "";
		}

		#region LoadDbGrid
		public void CreateDatabase ( DataGrid dgrid , List<string> ReceivedDbData )
		{
			string datain="";
			int totalfields = 0;
			// Post process data string received 
			ObservableCollection<GenericClass> genericcollection = new ObservableCollection<GenericClass>();
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
				genericcollection . Add ( genclass );
			}
			//			if ( ShowGridFlag  == true )
			LoadActiveRowsOnlyInGrid ( genericcollection , totalfields );
		}

		public void LoadActiveRowsOnlyInGrid ( ObservableCollection<GenericClass> genericcollection , int total )
		{
			// filter data to remove all "extraneous" columns
			DisplayGrid . ItemsSource = null;
			DisplayGrid . Items . Clear ( );
			if ( total == 1 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1}).ToList();
				DisplayGrid . ItemsSource = res;
			}
			else if ( total == 2 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1,data.field2}).ToList();
				DisplayGrid . ItemsSource = res;
			}
			else if ( total == 3 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1, data.field2,data.field3}).ToList();
				DisplayGrid . ItemsSource = res;
			}
			else if ( total == 4 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1, data.field2, data.field3,data.field4}).ToList();
				DisplayGrid . ItemsSource = res;
			}
			else if ( total == 5 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1, data.field2, data.field3,data.field4,data.field5}).ToList();
				DisplayGrid . ItemsSource = res;
			}
			else if ( total == 6 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1, data.field2, data.field3,data.field4,data.field5,data.field6}).ToList();
				DisplayGrid . ItemsSource = res;
			}
			else if ( total == 7 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1, data.field2, data.field3,data.field4,data.field5,data.field6,data.field7}).ToList();
				DisplayGrid . ItemsSource = res;
			}
			else if ( total == 8 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1, data.field2, data.field3,data.field4,data.field5,data.field6,data.field7,data.field8}).ToList();
				DisplayGrid . ItemsSource = res;
			}
			else if ( total == 9 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1, data.field2, data.field3,data.field4,data.field5,data.field6,data.field7,data.field8,data.field9}).ToList();
				DisplayGrid . ItemsSource = res;
			}
			else if ( total == 10 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1, data.field2, data.field3,data.field4,data.field5,data.field6,data.field7,data.field8,data.field9 ,data.field10}).ToList();
				DisplayGrid . ItemsSource = res;
			}
			else if ( total == 11 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1, data.field2, data.field3,data.field4,data.field5,data.field6,data.field7,data.field8,data.field9,data.field10,data.field11}).ToList();
				DisplayGrid . ItemsSource = res;
			}
			else if ( total == 12 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1, data.field2, data.field3,data.field4,data.field5,data.field6,data.field7,data.field8,data.field9,data.field10,data.field11,data.field12}).ToList();
				DisplayGrid . ItemsSource = res;
			}
			else if ( total == 13 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1, data.field2, data.field3,data.field4,data.field5,data.field6,data.field7,data.field8,data.field9,data.field10,data.field11,data.field12,data.field13}).ToList();
				DisplayGrid . ItemsSource = res;
			}
			else if ( total == 14 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1, data.field2, data.field3,data.field4,data.field5,data.field6,data.field7,data.field8,data.field9,data.field10,data.field11,data.field12,data.field13,data.field14}).ToList();
				DisplayGrid . ItemsSource = res;
			}
			else if ( total == 15 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1, data.field2, data.field3,data.field4,data.field5,data.field6,data.field7,data.field8,data.field9,data.field10,data.field11,data.field12,data.field13,data.field14,data.field15}).ToList();
				DisplayGrid . ItemsSource = res;
			}
			else if ( total == 16 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1, data.field2, data.field3,data.field4,data.field5,data.field6,data.field7,data.field8,data.field9,data.field10,data.field11,data.field12,data.field13,data.field14,data.field15,data.field16}).ToList();
				DisplayGrid . ItemsSource = res;
			}
			else if ( total == 17 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1, data.field2, data.field3,data.field4,data.field5,data.field6,data.field7,data.field8,data.field9,data.field10,data.field11,data.field12,data.field13,data.field14,data.field15,data.field16,data.field17}).ToList();
				DisplayGrid . ItemsSource = res;
			}
			else if ( total == 18 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1, data.field2, data.field3,data.field4,data.field5,data.field6,data.field7,data.field8,data.field9,data.field10,data.field11,data.field12,data.field13,data.field14,data.field15,data.field16,data.field17,data.field18}).ToList();
				DisplayGrid . ItemsSource = res;
			}
			else if ( total == 19 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1, data.field2, data.field3,data.field4,data.field5,data.field6,data.field7,data.field8,data.field9,data.field10,data.field11,data.field12,data.field13,data.field14,data.field15,data.field16,data.field17,data.field18,data.field19}).ToList();
				DisplayGrid . ItemsSource = res;
			}
			else if ( total == 20 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1, data.field2, data.field3,data.field4,data.field5,data.field6,data.field7,data.field8,data.field9,data.field10,data.field11,data.field12,data.field13,data.field14,data.field15,data.field16,data.field17,data.field18,data.field19,data.field20}).ToList();
				DisplayGrid . ItemsSource = res;
			}
			DisplayGrid . SelectedIndex = 0;
			DisplayGrid . Visibility = Visibility . Visible;
			GridCount . Text = DisplayGrid . Items . Count . ToString ( );
			DisplayGrid . Refresh ( );
			DisplayGrid . Focus ( );
		}
		#endregion LoadDbGrid

		private void ShowInGrid_Checked ( object sender , RoutedEventArgs e )
		{
			ShowGridFlag = true;
		}
		private void ShowInGrid_Unchecked ( object sender , RoutedEventArgs e )
		{
			ShowGridFlag = false;
		}
		private void ExecuteSQLCommand_Click ( object sender , RoutedEventArgs e )
		{
			// Execute Command button

			string args =SPArgs.Text == "Enter arguments here ..." ? "" : SPArgs.Text;
			SqlCommand = SPCombo . SelectedItem . ToString ( );
			// dummy Fn so we can call it drectly
			ExecuteStoredProcedure ( SqlCommand , null , "" , args , e );
		}
		private string ExecuteStoredProcedure ( string SqlCommand , ObservableCollection<GenericClass> generics = null , string DbName = "" , string Arguments = "" , RoutedEventArgs e = null )
		{
			bool showall=false;
			string SavedValue = SqlCommand;
			string command = "", dbnametoopen = "";
			string errormsg="";
			string Fieldname="", WhereClause="", OrderByClause="";
			ObservableCollection<GenericClass> Generics = new ObservableCollection<GenericClass>();
			if ( generics != null )
				Generics = generics;
			ObservableCollection<BankAccountViewModel> bvmparam = new ObservableCollection<BankAccountViewModel>();
			Dictionary <string, object>dict = new Dictionary<string, object>();
			IEnumerable DbResult=null;
			CurrentCommandLabel . Text = $"[{SqlCommand}]";
			if ( e != null )
				SqlCommand = SPCombo . SelectedItem . ToString ( );
			// If it is a CopyDb Procedure, bale out, use the Copy button
			if ( SqlCommand . ToUpper ( ) . Contains ( "SPCOPYDB" ) )
			{
				MessageBox . Show ( $"Please use the 'Copy Db' button at top right to perform this operation.." , "Input error" , MessageBoxButton . OK );
				return ( $"" );
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
						MessageBox . Show ( "You need to specify the SP that you want to view the ARG's\nfor in the field 'Enter arguments here ...'??" , "Details required" , MessageBoxButton . OK , MessageBoxImage . Warning );
						return "";
					}
					var reslt = MessageBox . Show ("Click Yes to see ALL occurences of @ARG, No for just the header declarations?","Details required", MessageBoxButton.YesNo, MessageBoxImage.Question  );
					if ( reslt == MessageBoxResult . Yes )
					{
						showall = true;
					}
				}
				else
				{
					var reslt = MessageBox . Show ( "This will get the @ARGS for the currently selected SP in the Combo above\nClick Yes to proceed or No to select a  different SP in the combo'??" , "Confirmation required" ,
						  MessageBoxButton.YesNoCancel, MessageBoxImage .Question);
					if ( reslt == MessageBoxResult . Yes )
					{
						reslt = MessageBox . Show ( "Click Yes to see ALL occurences of @ARG, No for just the header declarations?" , "Details required" , MessageBoxButton . YesNo , MessageBoxImage . Question );
						if ( reslt == MessageBoxResult . Yes )
						{
							showall = true;
						}
					}
					else
						return "";
				}
			}
			//****************************************************************************//
			// This is the MAIN call made to connect the SQL queries
			//****************************************************************************//
			//if ( Arguments != "" )
			//	Arguments = ParseArguments ( Arguments );
			DbResult = DapperSupport . ExecuteSPGeneric (
		Generics ,
		SqlCommand ,
		Arguments ,
		"" ,
		"" ,
		"" ,
		null ,
		dict ,
		out bvmparam ,
		out errormsg );
			DisplayGrid . ItemsSource = null;
			DisplayGrid . Items . Clear ( );

			if ( errormsg . Contains ( "DYNAMIC:" ) )
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
					return "";
				}
				string t = errormsg.Substring(8);
				int colcount =Convert.ToInt32(t );
				Console . WriteLine ( $"Db has {Generics . Count} records and {colcount} columns" );
				LoadActiveRowsOnlyInGrid ( Generics , colcount );
				DisplayGrid . SelectedIndex = 0;
				DisplayGrid . Refresh ( );
				togglevisibility ( true , GetDbNameFromCommand ( SqlCommand ) );
				// update  informatoin panels
				DbCopiedResult . Text = $"[{SqlCommand}] has been completed successfully....";
				CurrentCommandLabel . Text = $"[{SqlCommand}]";
				this . Title = $"Sql Server Commands : Active/Last Db/Command : '{SqlCommand}'";
				return "";
			}
			else if ( errormsg . Contains ( "SQL PARSE ERROR" ) )
			{
				// Process the data we have finally got into the Observabllecollection<GenricsClass>
				// which has 20 columns down to just whatever # of columns we actually have to work with/Display
				//Much cleaner and more pleasant to view
				MessageBox . Show ( errormsg , "SQL Query Error" , MessageBoxButton . OK ,
					    MessageBoxImage . Error );
				this . Title = $"Sql Server Commands : Active Db : 'None'";
				DbCopiedResult . Text = $"SQL query [{SqlCommand}] failed to run  correctly";
				CurrentCommandLabel . Text = SqlCommand + " " + Arguments;
				return "";
			}
			else
			{
				int columncount = 0;
				DataTable dt = new DataTable();
				dt = ProcessSqlCommand ( SqlCommand + " " + Arguments );
				if ( dt . Rows . Count == 0 )
				{
					if(errormsg == "")
						MessageBox . Show ( $"Datatable contains Zero records " , $"[{SqlCommand} {Arguments}] SP Script Error" , MessageBoxButton . OK , MessageBoxImage . Warning );
//					else
//						MessageBox . Show ( $"Datatable contains Zero records, Error reported was : \n{errormsg} " , $"[{SqlCommand} {Arguments}] SP Script Error" , MessageBoxButton . OK , MessageBoxImage . Warning );
					return "";
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
					if ( Generics . Count > 0 )
					{
						string buff = GetSpecificSPArguments ( "RETURNEDRESULTS" , Generics [ 0 ] . field1, showall );
						if ( buff . Length == 0 )
						{
							MessageBox . Show ( "The request succeeded, but the selected SP does not require any Arguments" , "Sql Information" , MessageBoxButton . OK , MessageBoxImage . Information );
							return "";
						}
						DisplayGrid . Visibility = Visibility . Hidden;
						GridData_Display . Visibility = Visibility . Visible;
						GridData_Display . Text = buff;
						DbCopiedResult . Text = $"[{SqlCommand}] completed successfully....";
						CurrentCommandLabel . Text = $"[{SqlCommand}]";
						this . Title = $"Sql Server Commands : Active/Last Db/Command : '{SqlCommand}'";
					}
					return "";
				}
				// Loads JUST the rows we actually have, not the full 20 columns !!
				//Much cleaner and more pleasant to view
				LoadActiveRowsOnlyInGrid ( Generics , columncount );
				togglevisibility ( true , GetDbNameFromCommand ( SqlCommand ) );
				if ( Arguments != "" )
					DbCopiedResult . Text = $"The Stored Procedure [{SqlCommand} [{Arguments}]] \nwas executed successfuly...";
				else
					DbCopiedResult . Text = $"The Stored Procedure [{SqlCommand}] \nwas executed successfuly...";
				DisplayGrid . SelectedIndex = 0;
				DisplayGrid . Visibility = Visibility . Visible;
				GridCount . Text = DisplayGrid . Items . Count . ToString ( );
				DisplayGrid . Refresh ( );
				DisplayGrid . Focus ( );
				// update  informatoin panels
				CurrentCommandLabel . Text = $"[{SqlCommand}]";
				OriginalSqlCommand = SqlCommand;
				DbCopiedResult . Text = $"[{SqlCommand}] has been completed successfully....";
				CurrentCommandLabel . Text = $"[{SqlCommand}]";
				this . Title = $"Sql Server Commands : Active/Last Db/Command : '{SqlCommand}'";
			}
			return "";
		}
		private string ParseArguments ( string args )
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
		private string AddIdentifier ( string Input , int count )
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
		public static void CreateGenericRecord ( string store , GenericClass gc , ObservableCollection<GenericClass> gco = null )
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
		public static DataTable ProcessSqlCommand ( string SqlCommand )
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
		public static ObservableCollection<GenericClass> ProcessSelectedCollection ( DataTable dt )
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
		private void DbToCopyCombo_SelectionChanged ( object sender , SelectionChangedEventArgs e )
		{
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
		private void DbToCopyCombo_MouseDoubleClick ( object sender , MouseButtonEventArgs e )
		{
			DbToCopyCombo . IsDropDownOpen = true;
		}
		private void SPCombo_MouseDoubleClick ( object sender , MouseButtonEventArgs e )
		{
			if ( SPCombo . IsDropDownOpen == false )
			{
				SPCombo . IsDropDownOpen = true;
			}
		}
		private void toggleButton_Click ( object sender , RoutedEventArgs e )
		{
			// combo button click
		}
		private void SPArgs_PreviewMouseLeftButtonUp ( object sender , MouseButtonEventArgs e )
		{
			SelectTextBoxText ( SpArgs );
		}
		private void Schemas_Click ( object sender , RoutedEventArgs e )
		{
			ObservableCollection<GenericClass> Generics = new ObservableCollection<GenericClass>();
			// get Args for a specific stored procs
			//			var reslt = MessageBox . Show ( $"This will return the arguments for the currently selected Stored Procedure\nin the Combo box to the left.\nClick Yes  to go ahead, or No\nto let you select the correct procedure", "", MessageBoxButton.YesNo, MessageBoxImage.Question);
			//			if ( reslt == MessageBoxResult . Yes )
			//			{
			SqlCommand = "spGetFullSchema";
			ExecuteStoredProcedure ( SqlCommand , Generics , "" , "FULL" , null );
			GridData_Display . Visibility = Visibility . Hidden;				
			DisplayGrid . Visibility = Visibility . Visible;
			return;

		}
		private void ListSPs_Click ( object sender , RoutedEventArgs e )
		{
			// get list of all stored procs
			ObservableCollection<GenericClass> Generics = new ObservableCollection<GenericClass>();
			SqlCommand = "spGetStoredProcs";
			ExecuteStoredProcedure ( SqlCommand , Generics , "" , "" , null );
			GridData_Display . Visibility = Visibility . Hidden;
			DisplayGrid . Visibility = Visibility . Visible;
			return;

		}
		private void SPCombo_SelectionChanged ( object sender , SelectionChangedEventArgs e )
		{
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
		private void ViewDbBtn_Click ( object sender , RoutedEventArgs e )
		{
			// view selected Db in grid
			List<string> data = new List<string>();
			if ( ActiveDbName == "" )
			{
				ActiveDbName = DbToCopyCombo . SelectedItem . ToString ( );
			}
			SqlCommand = $"Select * from {ActiveDbName}";
			OriginalSqlCommand = SqlCommand;
			CurrentCommandLabel . Text = $"[{SqlCommand}]";
			data = DapperSupport . GetGenericCollection (
				data ,
				SqlCommand ,
				false ,
				"" );
			//Copy new data  to global List<string>  ReceivedDbData 
			CopyList ( data );
			// creates and loads Db into grid
			if ( ReceivedDbData . Count > 0 )
				CreateDatabase ( DisplayGrid , ReceivedDbData );
			UpdateInfoPanels ( ActiveDbName );
			togglevisibility ( true , GetDbNameFromCommand ( SqlCommand ) );

			// Enable view  button
			CurrentCommandLabel . Text = $"[{SqlCommand}]";
			OriginalSqlCommand = SqlCommand;
			CurrentCommandLabel . Text = $"Select * from [{ActiveDbName . ToUpper ( )}]";
			DbCopiedResult . Text = $"SQL Command [{SqlCommand} completed successfully...]";
		}
		private void UpdateInfoPanels ( string DbName )
		{
			GridCount . Text = DisplayGrid . Items . Count . ToString ( );
			BankNameLabel . Text = DbName . ToUpper ( );
			this . Title = $"Sql Server Commands : Active Db : {DbName . ToUpper ( )}";
		}
		// Handle viewing whatever is in the grid
		private void GridViewer_Click ( object sender , RoutedEventArgs e )
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
		private void AllowOverWriteFlag_Checked ( object sender , RoutedEventArgs e )
		{

		}
		private void DbToCopyCombo_PreviewMouseLeftButtonDn ( object sender , MouseButtonEventArgs e )
		{
			//			if ( DbToCopyCombo . IsDropDownOpen == false )
			//				DbToCopyCombo . IsDropDownOpen = true;
			e . Handled = false;
		}
		private void SPCombo_PreviewMouseLeftButtonDn ( object sender , MouseButtonEventArgs e )
		{
			//			if ( SPCombo . IsDropDownOpen == false )
			//				SPCombo . IsDropDownOpen = true;
			//			else
			//				SPCombo . SelectedItem = SPCombo . SelectedIndex;
			e . Handled = false;
		}
		// Menu items
		private void Option1_Click ( object sender , RoutedEventArgs e )
		{

		}
		private void RefreshLists_Click ( object sender , RoutedEventArgs e )
		{
			// Populate the Db Tables && Stored Procedures Combo
			LoadTablesCombo ( );
			LoadSPCombo ( );

		}
		private void ViewGridContent_Click ( object sender , RoutedEventArgs e )
		{
			GridViewer_Click ( null , null );
		}
		//  Load list if Tables in Db
		private void LoadTablesCombo ( )
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
		private void LoadSPCombo ( )
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

		private void Window_MouseRightButtonUp ( object sender , MouseButtonEventArgs e )
		{
			DataGrid dg = sender as DataGrid;
			if ( dg != null )
				Debugger . Break ( );
		}



		private void GridData_Display_CanExecute ( object sender , CanExecuteRoutedEventArgs e )
		{
			e . Handled = true;
			e . CanExecute = false;
		}

		private void GridData_Display_MouseDoubleClick ( object sender , MouseButtonEventArgs e )
		{
			if ( GridData_Display . Visibility == Visibility . Visible )
			{
				GridData_Display . Visibility = Visibility . Hidden;
				DisplayGrid . Visibility = Visibility . Visible;
			}
		}

		private void info_Click ( object sender , RoutedEventArgs e )
		{

		}

		private void Help_Click ( object sender , RoutedEventArgs e )
		{

		}

		private void SPArgs_KeyDown ( object sender , KeyEventArgs e )
		{
			if ( e . Key == Key . Enter )
				ExecuteStoredProcedure ( SPCombo . SelectedItem . ToString ( ) , null , "" , SPArgs . Text , null );
		}
		public string GetSpecificSPArguments ( string ProcedureToFind , string result = "" , bool showall = true )
		{
			string output="";
				
			if ( ProcedureToFind == "RETURNEDRESULTS" )
			{
				// we now have the result, so lets process them
				string buffer = result ;
				string[] lines = buffer.Split('\n');
				output = $"Procedure Name : \n{SPCombo.SelectedItem.ToString(). ToUpper ( )}\n\n";
				foreach ( var item in lines )
				{
					if ( item . ToUpper ( ) . Contains ( "@ARG" ) )
						output += item;
					if ( showall == false )
					{
						if ( item . ToUpper ( ) == "AS\r" )
							break;
					}
				}
				output += $"\n\nDouble click in viewer to return to Data Grid View...";
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

		private void SqlCommandString_GotFocus ( object sender , RoutedEventArgs e )
		{
			if ( SqlCommandString . Text == "" )
			{
				SqlCommandString . Text = "Enter SQL command ...";
				SqlCommandString . Foreground = FindResource ( "Gray0" ) as SolidColorBrush;
			}
		}

		private void SPArgs_LostFocus ( object sender , RoutedEventArgs e )
		{
			if ( SPArgs . Text == "" )
			{
				SPArgs . Text = "Enter arguments here ...";
				SPArgs . Foreground = FindResource ( "Gray0" ) as SolidColorBrush;
			}
		}

		private void SPArgumentss_Click ( object sender , RoutedEventArgs e )
		{

			SqlCommand = "spGetSpecificSchema";
			GetSpecificSPArguments ( SPCombo . SelectedItem . ToString ( ) , "" , true );
			//ExecuteStoredProcedure ( SqlCommand , null , "RETURNDATA" , args , null );
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
}

