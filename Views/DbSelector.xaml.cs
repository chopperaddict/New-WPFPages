﻿using System;
using System . ComponentModel;
using System . Configuration;
using System . Data . SqlClient;
using System . Data;
using System . Diagnostics;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Input;
using System .Windows .Media;

using WPFPages;
using WPFPages . ViewModels;
using Dapper;
using System . Linq;
using System . Collections . Generic;

namespace WPFPages . Views
{
	/// <summary>
	/// Interaction logic for DbSelector.xaml
	/// </summary>
	///

	public static class ApplicationState
	{
		private static SqlDbViewer viewer;

		public static SqlDbViewer Viewer
		{
			get { return viewer; }
			set { viewer = value; }
		}
	}

	public partial class DbSelector : Window, System . ComponentModel.INotifyPropertyChanged
	{
		private static BankAccountViewModel bvm = MainWindow . bvm;
		private static CustomerViewModel cvm = MainWindow . cvm;
		private static DetailsViewModel dvm = MainWindow . dvm;
		public BankCollection Bankcollection;

		public int selection = 0;
		private int CurrentList = -1;
		private static bool key1 = false;

		/// <summary>
		///  Handler  for delegate notification FROM SqlDbViewer
		///  Message SENT
		///  100 - Tell Dbs to command this  to start SQL Data loading
		///  Messgaes Received
		///  25 - I can command data loading
		///  102 - Mthod starting
		///  103 - Method ended
		///  111 - General report received
		/// </summary>
		public static void MyNotification ( int status, string info, SqlDbViewer NewSqlViewer )
		{
			//switch ( status )
			//{
			//	case 102:       // Starting a method
			//		Debug . WriteLine ( $"DBSELECTOR NOTIFICATION : {status}  [{info}]" );
			//		break;

			//	case 103:       // Ending a process
			//		Debug . WriteLine ( $"DBSELECTOR NOTIFICATION: {status}  [{info}]" );
			//		break;

			//	case 111:       // Info reports
			//		Debug . WriteLine ( $"DBSELECTOR NOTIFICATION : [{status}] - [{info}]" );
			//		break;

			//	default:
			//		//					Debug . WriteLine ( $"DBSELECTOR NOTIFICATION : [{status}], [{info}]" );
			//		break;
			//}
			//if ( status == 99 )
			//{
			//	Debug . WriteLine ( $"\r\nDBSELECTOR NOTIFICATION : Received [{status}]  - Window is closing down\r\n" );
			//}
			//else if ( status == 100 )
			//{
			//	Debug . WriteLine ( $"\r\nDBSELECTOR NOTIFICATION : Received TEST SIGNAL {status} from SqlDbViewer\r\n" );
			//}
			//else if ( status == 101 )
			//{
			//	// info contains the text to be added to the Viewers ListBox
			//	Debug . WriteLine ( $"\r\nDBSELECTOR - Received request [{status}] to Add Viewer to Current Viewers List.\r\n" );
			//	Flags . SqlViewerIsLoading = true;
			//	DbSelector . AddViewerToList ( info, NewSqlViewer, -1 );
			//	Flags . SqlViewerIsLoading = false;
			//	Debug . WriteLine ( $"\r\nDBSELECTOR - Viewer ADDED to List of Current Viewers \r\n" );
			//}
		}
		private void ChecksMouseMove ( object sender , MouseEventArgs e )
		{
			e . Handled = true;
			if ( e . RightButton == MouseButtonState . Pressed )
				return;
		}

		//Constructor
		public DbSelector ( )
		{
			
			InitializeComponent ( );
			if ( ViewersList? . Items . Count > 2 )
			{// ignore the dummy blank entry line
				ViewersList . SelectedIndex = 2;
				ViewersList . SelectedItem = 2;
			}
			sqlSelector . SelectedIndex = 2;
			sqlSelector . Focus ( );
			//			this . MouseDown += delegate { DoDragMove ( ); };
			Utils.SetupWindowDrag(this);
			//			Utils . GetWindowHandles ( );
			OntopChkbox . IsChecked = false;
			ExecuteFile . Visibility = Visibility . Collapsed;
			this . Topmost = true;
			if( Flags . USECOPYDATA )
				CurrentDbType.Text = "Using Copy Data";
			else
				CurrentDbType . Text = "Using Original Data";
			//			MouseMove += Grab_MouseMove;
			SysMenu sym = new SysMenu();
				sym . Show ( );
			//ags . DbSelectorOpen . Visibility = Visibility . Hidden;

		}


		public static ListBox listbox;
		public static int selected;
		public static string Command;
            //		public static  object  MyDispatcher;

            private Brush itemBkground;

            public Brush ItemBkground
            {
                  get { return itemBkground; }
                  set { itemBkground = value; }
            }

            #region DP's

            public Brush  SelectedBackground
            {
                  get { return ( Brush  ) GetValue ( SelectedBackgroundProperty ); }
                  set { SetValue ( SelectedBackgroundProperty , value ); }
            }

            // Using a DependencyProperty as the backing store for SelectedBackground.  This enables animation, styling, binding, etc...
            public static readonly DependencyProperty SelectedBackgroundProperty =
    DependencyProperty.Register("SelectedBackground", typeof(Brush ), typeof(DbSelector), new PropertyMetadata(default));

            #endregion DP's
            public void SetFocusToExistingViewer ( Guid guid )
		{
			for ( int x = 0 ; x < MainWindow . gv . MaxViewers ; x++ )
			{
				if ( MainWindow . gv . ListBoxId [ x ] == guid )
				{
					MainWindow . gv . window [ x ] . Focus ( );
					MainWindow . gv . window [ x ] . BringIntoView ( );
					break;
				}
			}
		}

		public void ClearClosingViewer ( string CallerDb, Guid guid )
		{
			for ( int x = 0 ; x < MainWindow . gv . MaxViewers ; x++ )
			{
				if ( MainWindow . gv . ListBoxId [ x ] == guid )
				{
					if ( CallerDb == "BANKACCOUNT" )
						MainWindow . gv . Bankviewer = Guid . Empty;
					else if ( CallerDb == "CUSTOMER" )
						MainWindow . gv . Custviewer = Guid . Empty;
					else if ( CallerDb == "DETAILS" )
						MainWindow . gv . Detviewer = Guid . Empty;
					break;
				}
			}
		}


		private  void HandleSelection ( ListBox listbox, string Command )
		{
			// Called when Opening/ Closing/deleting a Db Viewer window
			//and most other functionality in this window (All buttons and double clicks)
			int selected = -1;
			int callertype = -1;
			string selectedItem = "";
			string CallingType = "";

			// Just Testing Dapper - it works too !
			//List<BankAccountViewModel> bvm = new List<BankAccountViewModel>();
			//string ConString = ( string ) Properties . Settings . Default [ "BankSysConnectionString" ];
			//using ( IDbConnection db = new SqlConnection (ConString) )
			//{
			//	bvm = db . Query<BankAccountViewModel> ( "Select * From BankAccount" ) . ToList ( );
			//}

			selected = listbox . SelectedIndex;
			if ( Command == "NEW" )
			{
				selectedItem = listbox . SelectedItem . ToString ( );
				BankCollection Bankcollection = new BankCollection ( );
				AllCustomers Custcollection = new AllCustomers ( );
				DetCollection Detcollection = new DetCollection ( );
				
				// This the DETAILS datagrid actualy
				if ( selectedItem . ToUpper ( ) . Contains ( "MULTI BANK ACCOUNTS" ) )
				{
					// DETAILS DATABASE
					if ( Flags . SqlDetViewer != null )
					{
						Flags . SqlDetViewer . BringIntoView ( );
						Flags . SqlDetViewer . Focus ( );
						return;
					}
					callertype = 2;
					CallingType = "DETAILS";
					// LOADS THE WINDOW HERE - it RETURNS IMMEDIATELY even though the data is not yet fully loaded
					SqlDbViewer sqldbv = new SqlDbViewer ( "DETAILS", Detcollection );
					Flags . CurrentSqlViewer = sqldbv;
					Flags . CurrentSqlViewer . BringIntoView ( );
					Flags . CurrentSqlViewer . Tag = Guid . NewGuid ( );
					MainWindow . gv . SqlViewerGuid = ( Guid ) Flags . CurrentSqlViewer . Tag;
					// This is fine, new windows do NOT have their Guid when they arrive here
					this . Tag = Flags . CurrentSqlViewer . Tag;
					//					}
					callertype = 2;
					CallingType = "DETAILS";
					Flags . CurrentSqlViewer . Show ( );
				}
				else if ( selectedItem . ToUpper ( ) . Contains ( "BANK ACCOUNTS" ) )
				{
					// BANK DATABASE
					if ( MainWindow . gv . Bankviewer != Guid . Empty )
					{
						SetFocusToExistingViewer ( MainWindow . gv . Bankviewer );
						return;
					}
					Flags . CurrentSqlViewer = new SqlDbViewer ( "BANKACCOUNT", Bankcollection );
					Flags . CurrentSqlViewer . BringIntoView ( );

					//Data is loaded by here .....
					Flags . CurrentSqlViewer . Tag = Guid . NewGuid ( );
					MainWindow . gv . SqlViewerGuid = ( Guid ) Flags . CurrentSqlViewer . Tag;

					// This is fine, new windows do NOT have their Guid when they arrive here
					this . Tag = Flags . CurrentSqlViewer . Tag;
					//					}
					Flags . CurrentSqlViewer . Show ( );
					callertype = 0;
					CallingType = "BANKACCOUNT";
				}
				else if ( selectedItem . ToUpper ( ) . Contains ( "CUSTOMER ACCOUNTS" ) )
				{
					// CUSTOMER DATABASE
					if ( MainWindow . gv . Custviewer != Guid . Empty )
					{
						SetFocusToExistingViewer ( MainWindow . gv . Custviewer );
						return;
					}

					Flags . CurrentSqlViewer = new SqlDbViewer ( "CUSTOMER", Custcollection );
					Flags . CurrentSqlViewer . BringIntoView ( );
					Flags . CurrentSqlViewer . Tag = Guid . NewGuid ( );
					MainWindow . gv . SqlViewerGuid = ( Guid ) Flags . CurrentSqlViewer . Tag;

					// This is fine, new windows do NOT have their Guid when they arrive here
					this . Tag = Flags . CurrentSqlViewer . Tag;
					//					}
					callertype = 1;
					CallingType = "CUSTOMER";
					Flags . CurrentSqlViewer . Show ( );
				}
				//When loading a new viewer, the  MainWindow.gv structure is completed correctly !!!!

				// LOAD THE VIEWERS LIST HERE TO AVOID ISSUES
				Flags . SqlViewerIsLoading = true;
				string s = DbSelector . AddViewerToList ( "", Flags . CurrentSqlViewer, callertype );
				//This call sets up all the Gridview gv[] variables and the related singleton pointers in the gv[] structure
				UpdateControlFlags ( Flags . CurrentSqlViewer, CallingType, s );
				Flags . SqlViewerIsLoading = false;
				//Set the viewer Delete one/All/Select buttons up correctly
				UpdateSelectorButtons ( );
			}
			else if ( Command == "DELETEALL" )
			{
				CloseDeleteAllViewers ( );
				//Set the viewer Delete one/All/Select buttons up correctly
				UpdateSelectorButtons ( );
			}
			else if ( Command == "DELETE" )
			{
				//Close selected viewer window
				DeleteCurrentViewer ( );
				Flags . CurrentSqlViewer . Close ( );
				UpdateSelectorButtons ( );
			}
			else if ( Command == "SELECT" )
			{
				ListBoxItem lbi = new ListBoxItem ( );
				Guid tag = Guid . Empty;
				lbi = listbox . SelectedItem as ListBoxItem;
				if ( lbi == null ) return;
				tag = ( Guid ) lbi . Tag;
				for ( int x = 0 ; x < ViewersList . Items . Count ; x++ )
				{
					if ( MainWindow . gv . ListBoxId [ x ] == tag )
					{
						MainWindow . gv . window [ x ] . Focus ( );
						MainWindow . gv . SqlViewerWindow = MainWindow . gv . window [ x ] as SqlDbViewer;
						//Ensure our global viewer pointer is set to last viewer selected
						Flags . CurrentSqlViewer = MainWindow . gv . window [ x ] as SqlDbViewer;
						break;
					}
				}
			}
		}

		private Window SelectAnyOpenViewer ( )
		{
			Window WinHandle = null;
			Guid tag = ( Guid . Empty );
			ListBoxItem lbi = new ListBoxItem ( );
			tag = ( Guid ) lbi . Tag;
			if ( tag == null )
				return null;
			for ( int x = 0 ; x < ViewersList . Items . Count ; x++ )
			{
				lbi = ViewersList . Items [ x ] as ListBoxItem;
				if ( ( Guid ) lbi . Tag != Guid . Empty )
				{
					WinHandle = GetWindowFromTag ( ( Guid ) lbi . Tag );
					break;
				}
			}
			return WinHandle;
		}

		private Window GetWindowFromTag ( Guid guid )
		{
			Window WinHandle = null;
			for ( int x = 0 ; x < MainWindow . gv . MaxViewers ; x++ )
			{
				if ( MainWindow . gv . ListBoxId [ x ] == guid )
				{
					WinHandle = MainWindow . gv . window [ x ] as Window;
					break;
				}
			}
			return WinHandle;
		}

		private void UpdateSelectorButtons ( )
		{
			int counter = 0;
			//Set default active item to 1st valid entry
			counter = ViewersList . Items . Count;
			if ( counter <= 1 )
			{
				ViewerDeleteAll . IsEnabled = false;
				ViewerDelete . IsEnabled = false;
				SelectViewerBtn . IsEnabled = false;
			}
			else
			{
				ViewersList . SelectedIndex = 1;
				if ( counter > 1 )
					ViewerDelete . IsEnabled = true;
				else
					ViewerDelete . IsEnabled = false;
				if ( counter > 2 )
					ViewerDeleteAll . IsEnabled = true;
				else
					ViewerDeleteAll . IsEnabled = false;
				SelectViewerBtn . IsEnabled = true;
			}
		}

		public void DeleteCurrentViewer ( )
		{
			//Remove a SINGLE Viewer Windows data from Flags & gv[]
			Flags . DeleteViewerAndFlags ( ViewersList . SelectedIndex );
			return;

			//int nextWin = -1;
			//ListBoxItem lbi = new ListBoxItem ( );
			//Guid tag = Guid . Empty;
			//lbi = ViewersList . SelectedItem as ListBoxItem;
			//tag = ( Guid ) lbi . Tag;
			////First Close the Viewer window itself & update the control structure
			//for ( int x = 0 ; x < MainWindow . gv . MaxViewers ; x++ )
			//{
			//	if ( MainWindow . gv . ListBoxId [ x ] == tag )
			//	{
			//		//Clear relevant viewer type flag
			//		if ( MainWindow . gv . Bankviewer == MainWindow . gv . ListBoxId [ x ] )
			//			ClearClosingViewer ( "BANKACCOUNT", MainWindow . gv . ListBoxId [ x ] );
			//		else if ( MainWindow . gv . Custviewer == MainWindow . gv . ListBoxId [ x ] )
			//			ClearClosingViewer ( "BANKACCOUNT", MainWindow . gv . ListBoxId [ x ] );
			//		else if ( MainWindow . gv . Detviewer == MainWindow . gv . ListBoxId [ x ] )
			//			ClearClosingViewer ( "BANKACCOUNT", MainWindow . gv . ListBoxId [ x ] );

			//		SqlDbViewer sqlv = MainWindow . gv . window [ x ] as SqlDbViewer;
			//		UpdateDataGridController ( tag );
			//		sqlv . Close ( );
			//		MainWindow . gv . SqlViewerWindow = null;
			//		break;
			//	}
			//}
			//Debug . WriteLine ( $"listbox count = {listbox . Items . Count} before Removeat() " );
			////Remove the listbox entry
			//listbox . Items . RemoveAt ( selected );
			//Debug . WriteLine ( $"listbox count = {listbox . Items . Count} after Removeat() " );
			//Debug . WriteLine ( $"ViewersList count = {Flags . DbSelectorOpen . ViewersList . Items . Count} after Removeat() " );
			//if ( Flags . DbSelectorOpen . ViewersList . Items . Count == 1 )
			//	Debug . WriteLine ( $"All Viewers have been closed and GridView structure is cleared..." );

			//Flags . DbSelectorOpen . ViewersList . Refresh ( );
			////Now highlight first one in list if we have one
			//if ( Flags . DbSelectorOpen . ViewersList . Items . Count > 1 )
			//{
			//	bool success = false;
			//	if ( Flags . DbSelectorOpen . ViewersList . Items . Count == 1 )
			//	{
			//		Flags . CurrentSqlViewer . UpdateDbSelectorBtns ( Flags . CurrentSqlViewer );
			//		// Clear global Flags structure
			//		Flags . ClearGridviewControlStructure ( null, null );
			//		return;
			//	}
			//	else
			//	{
			//		for ( int x = 0 ; x < MainWindow . gv . ViewerCount ; x++ )
			//		{
			//			if ( MainWindow . gv . window [ x ] != null )
			//			{
			//				success = true;
			//				nextWin = x;
			//				break;
			//			}
			//		}
			//	}
			//	if ( success )
			//	{
			//		MainWindow . gv . window [ nextWin ] . Focus ( );
			//		MainWindow . gv . window [ nextWin ] . BringIntoView ( );
			//		MainWindow . gv . window [ nextWin ] . Refresh ( );
			//		MainWindow . gv . SqlViewerWindow = MainWindow . gv . window [ nextWin ] as SqlDbViewer;
			//		Flags . DbSelectorOpen . ViewersList . SelectedIndex = nextWin;
			//		Flags . CurrentSqlViewer . UpdateDbSelectorBtns ( Flags . CurrentSqlViewer );
			//		Flags . CurrentSqlViewer . Focus ( );
			//		Flags . CurrentSqlViewer . BringIntoView ( );
			//		//					ExtensionMethods . Refresh ( Flags . CurrentSqlViewer );
			//		Flags . CurrentSqlViewer . Refresh ( );
			//	}
			//	else if ( MainWindow . gv . ViewerCount == 1 )
			//	{
			//		//no more opeen viewers, so clear control ctructure entirely
			//		Flags . SetGridviewControlFlags ( Flags . CurrentSqlViewer, null );
			//	}
			//}
			//Reset global flags
			//			EventHandlers . ClearWindowHandles ( null, Flags . CurrentSqlViewer );
		}

		public void CloseDeleteAllViewers ( )
		{
			//Close selected viewer window
			// iterate the list form bottom up closing windows
			// and clearing all array entries for this entry
			for ( int x = ViewersList . Items . Count - 1 ; x >= 1 ; x-- )
			{
				if ( MainWindow . gv . window [ x - 1 ] != null )
				{
					//Physically close the window itself
					MainWindow . gv . window [ x - 1 ] . Close ( );
				}
			}

			//Remove all Viewer Windows data from Flags & gv[]
			Flags . DeleteViewerAndFlags ( -1 );
			//UpdateDataGridController ( null );

			// This is done in the call above
			ViewersList . Refresh ( );
			//			MainWindow . gv . SqlViewerWindow = null;
			return;
		}

		/// <summary>
		/// updates the Grid Controller structure when a viewer closes
		/// </summary>
		/// <param name="tag"></param>
		public void UpdateDataGridController ( object Tag )
		{
			if ( Tag == null )
			{
				//Remove a SINGLE Viewer Windows data from Flags & gv[]
				Flags . DeleteViewerAndFlags ( );
				return;
			}
			Guid tag = ( Guid ) Tag;
			// Removes  the Viewer entry identified by it's Tag
			//from the Control structure list & updates it as required
			ListBoxItem lbi = new ListBoxItem ( );
			for ( int y = 0 ; y < Flags . DbSelectorOpen . ViewersList . Items . Count - 1 ; y++ )
			{
				lbi = Flags . DbSelectorOpen . ViewersList . Items [ y + 1 ] as ListBoxItem;
				Guid lbtag = ( Guid ) lbi . Tag;
				if ( tag == Guid . Empty )
				{
					// Command to clear ALL entries
					if ( lbtag == tag )
					{
						//window has been closed - Remove data from Control Structure
						MainWindow . gv . ViewerCount--;
						MainWindow . gv . CurrentDb [ y ] = "";
						MainWindow . gv . ListBoxId [ y ] = Guid . Empty;
						MainWindow . gv . Datagrid [ y ] = null;
						MainWindow . gv . window [ y ] = null;
						MainWindow . gv . SqlViewerWindow = null;
						break;
					}
				}
				else
				{
					//Remove a SINGLE Viewer Windows data from Flags & gv[]
					Flags . DeleteViewerAndFlags ( y );
					break;
					//if (lbtag == (Guid)Tag)
					//{
					//	MainWindow.gv.CurrentDb[y] = "";
					//	MainWindow.gv.ListBoxId[y] = Guid.Empty;
					//	MainWindow.gv.Datagrid[y] = null;
					//	MainWindow.gv.window[y] = null;
					//	MainWindow.gv.SqlViewerWindow = null;

					//	//This needs to stay at ZERO, no tdecrmeented to -1 like the rest
					//	MainWindow.gv.ViewerCount--;
					//	break;
					//}
				}
			}
			//			MainWindow.gv.ViewerCount = 0;
		}

		/// <summary>
		/// PRIMARY Fn TO ADD A VIEWER
		/// Adds the details of the newly loaded viewer window to the DbSelectors ViewersList window
		/// </summary>
		/// <param name="data"></param>
		public static string AddViewerToList ( string data, SqlDbViewer viewer, int DbType )
		{
			if ( viewer == null ) return "";
			if ( viewer . Tag == null )
				return "";
			//Binding binding = new Binding ("Content");
			//binding.Source = Flags.DbSelectorOpen.ListBoxItemText;
			ListBoxItem lbi = new ListBoxItem ( );
			// Set Tag of this LB Item to the DbViewer Windo.w
			lbi . Tag = viewer . Tag;
			Guid guid = ( Guid ) lbi . Tag;
			if ( Utils . CheckForExistingGuid ( guid ) )
			{
				Flags . DbSelectorOpen . UpdateViewersList ( );
			}
			else
			{
				if ( Flags . CurrentSqlViewer . Tag == null ) return "";
				//Set tag in the Windows Memory space
				if ( ( Guid ) Flags . CurrentSqlViewer . Tag == Guid . Empty )
					Flags . CurrentSqlViewer . Tag = lbi . Tag;
				//update our DependencyProperty ListBoxItemText - in DbSelector.cs
				if ( DbType == 0 )
				{
					BankAccountViewModel rec = new BankAccountViewModel ( );
					rec = Flags . CurrentSqlViewer . BankGrid . SelectedItem as BankAccountViewModel;
					MainWindow . gv . PrettyDetails = $"Bank - A/c # {rec?.BankNo}, Cust # {rec?.CustNo}, Balance £ {rec?.Balance}, Interest {rec?.IntRate}%";
					lbi . Content = MainWindow . gv . PrettyDetails;
				}
				else if ( DbType == 1 )
				{
					CustomerViewModel rec = new CustomerViewModel ( );
					rec = Flags . CurrentSqlViewer . CustomerGrid . SelectedItem as CustomerViewModel;
					MainWindow . gv . PrettyDetails = $"Customer - A/c # {rec?.BankNo}, Cust # {rec?.CustNo}, Forename: {rec?.FName}, Surname : {rec?.LName}, Town : {rec?.Town}";
					lbi . Content = MainWindow . gv . PrettyDetails;
				}
				else if ( DbType == 2 )
				{
					DetailsViewModel rec = new DetailsViewModel ( );
					rec = Flags . CurrentSqlViewer . DetailsGrid . SelectedItem as DetailsViewModel;
					MainWindow . gv . PrettyDetails = $"Details - A/c # {rec?.BankNo}, Cust # {rec?.CustNo}, Balance £ {rec?.Balance}, Interest {rec?.IntRate}%";
					lbi . Content = MainWindow . gv . PrettyDetails;
				}
			}
			// This adds the entry on creation of a new viewer.  MainWindow.gv[] is also filled when we get here on new window loading
			int indx = Flags . DbSelectorOpen . ViewersList . Items . Add ( lbi );
			Flags . DbSelectorOpen . ViewersList . Items . Refresh ( );
			//ExtensionMethods . Refresh ( Flags . DbSelectorOpen . ViewersList );
			Flags . DbSelectorOpen . ViewersList . Refresh ( );
			Flags . DbSelectorOpen . ViewersList . SelectedIndex = indx;
			return MainWindow . gv . PrettyDetails;
		}

		private static bool CheckForExistingEntry ( string entry )
		{
			bool result = false;
			ListBoxItem lbi = new ListBoxItem ( );

			foreach ( var item in Flags . DbSelectorOpen . ViewersList . Items )
			{
				lbi = item as ListBoxItem;
				string lbentry = lbi . Content as string;
				// Strings are formatted differently, but this check should identify a match
				if ( lbentry . Contains ( entry ) || entry . Contains ( lbentry ) )
				{
					result = true;
					break;
				}
			}
			return result;
		}

		public string GetCurrentViewerListEntry ( SqlDbViewer sender )
		{
			string retstring = "";
			for ( int x = 0 ; x < Flags . DbSelectorOpen . ViewersList . Items . Count ; x++ )
			{
				ListBoxItem lbi = new ListBoxItem ( );
				//lbi.Tag = viewer.Tag;
				lbi = Flags . DbSelectorOpen . ViewersList . Items [ x ] as ListBoxItem;
				if ( lbi . Tag == null ) return "";
				Guid g = ( Guid ) lbi . Tag;
				if ( g == ( Guid ) Flags . CurrentSqlViewer . Tag )
				{
					string lbistring = lbi . Content as string;
					retstring = lbistring;
					break;
				}
			}
			return retstring;
		}

		public static bool ChangeViewerListEntry ( string currentListentry, string newListEntry, SqlDbViewer viewer )
		{
			bool retval = false;
			for ( int x = 0 ; x < Flags . DbSelectorOpen . ViewersList . Items . Count ; x++ )
			{
				ListBoxItem lbi = new ListBoxItem ( );
				//lbi.Tag = viewer.Tag;
				lbi = Flags . DbSelectorOpen . ViewersList . Items [ x ] as ListBoxItem;
				//				if (lbi.Tag == null) return retval;
				//				Guid g = (Guid)lbi.Tag;
				if ( currentListentry == ( string ) lbi . Content )
				{
					lbi . Content = newListEntry;
					Flags . DbSelectorOpen . ViewersList . Refresh ( );
					retval = true;
					break;
				}
			}
			return retval;
		}

		// Variable to hold string content for ListBox items in ViewerList of DbSelector.
		private string _listBoxItemText;

		public string ListBoxItemText
		{
			get { return _listBoxItemText; }

			set
			{
				_listBoxItemText = value;
				OnPropertyChanged ( ListBoxItemText . ToString ( ) );
			}
		}

		private void OnWindowLoaded ( object sender, RoutedEventArgs e )
		{
			int counter = 0;
			//Set default active item to 1st valid entry
			counter = ViewersList . Items . Count;

			if ( counter <= 1 )
			{
				ViewerDeleteAll . IsEnabled = false;
				ViewerDelete . IsEnabled = false;
				SelectViewerBtn . IsEnabled = false;
			}
			else
			{
				ViewersList . SelectedIndex = 1;
				if ( counter > 1 )
					ViewerDelete . IsEnabled = true;
				else
					ViewerDelete . IsEnabled = false;
				if ( counter > 2 )
					ViewerDeleteAll . IsEnabled = true;
				else
					ViewerDeleteAll . IsEnabled = false;
				SelectViewerBtn . IsEnabled = true;
			}
//			string value = ConfigurationManager.AppSettings.Get("Bodrum");
			// select the 1st entry in the lower (New Viewer) list
			string StartupWindow = ( string ) Properties . Settings . Default [ "StartupWindow" ];
			if ( StartupWindow == "Bank Db Viewer" )
				sqlSelector . SelectedIndex = 0;
			else if ( StartupWindow == "Customer Db Viewer" )
				sqlSelector . SelectedIndex = 1;
			else if ( StartupWindow == "Details Db Viewer" )
				sqlSelector . SelectedIndex = 2;
			this . BringIntoView ( );
			OntopChkbox . IsChecked = false;
			this . Topmost = false;
		}

		//*****************************************************************************************//
		public  void DoDragMove ( )
		{//Handle the button NOT being the left mouse button
		 // which will crash the DragMove Fn.....
			try
			{ this . DragMove ( ); }
			catch { return; }
		}

		//*****************************************************************************************//
		private void Cancel_Click ( object sender, RoutedEventArgs e )
		{
			// close this Db Selector window
			this . Visibility = Visibility . Collapsed;
		}

		private void sqlselector_Select ( object sender, MouseButtonEventArgs e )
		{//Select Btn or dbl click on top list, so get a new window of selected type
			if ( sqlSelector . SelectedIndex == -1 )
				return;
			HandleSelection ( sqlSelector, "NEW" );
		}

		//**************************** LOWER LIST - EXISTING VIEWER *************************************//
		private void SelectViewer_Click ( object sender, RoutedEventArgs e )
		{//Select Btn button for lower viewers list
		 //open / bring the window to the front
			HandleSelection ( ViewersList, "SELECT" );
			//ViewersList_Select (sender, null);
		}

		//********************************************************************************************//
		private void ViewersList_Select ( object sender, MouseButtonEventArgs e )
		{// double click on list2 - existing viewer list - pass the selected item data back
		 // and open/bring the window to the front
			if ( ViewersList . SelectedIndex == -1 )
				return;
			HandleSelection ( ViewersList, "SELECT" );
		}

		//********************************************************************************************//
		private void DeleteViewer_Click ( object sender, RoutedEventArgs e )
		{
			// delete just the selected viewer
			if ( ViewersList . SelectedIndex < 1 )
				return;

			HandleSelection ( ViewersList, "DELETE" );
		}

		//********************************************************************************************//
		private void SQLlist_Focused ( object sender, RoutedEventArgs e )
		{
			//Set the flag so we know which list is active for key press checking
			CurrentList = 1;
		}

		//********************************************************************************************//
		private void Viewerslist_Focused ( object sender, RoutedEventArgs e )
		{
			//Set the flag so we know which list is active for key press checking
			CurrentList = 2;
		}

		//********************************************************************************************//
		private void sqlselectorbtn_Select ( object sender, RoutedEventArgs e )
		{

			SqlServerCommands sqlc = new SqlServerCommands();
			sqlc . Show ( );
			// top list Select button pressed - open a new viewer of selected type
			//if ( sqlSelector . SelectedIndex == -1 )
			//	return;
			//if ( MainWindow . gv . ViewerCount == MainWindow . gv . MaxViewers )
			//{
			//	MessageBox . Show ( $"Sorry, but the maximum of {MainWindow . gv . MaxViewers} Viewer Windows are already open.\r\nPlease close one or more, or select an existing Viewer...", "Maximum viewer count reached" );
			//	return;
			//}

			//HandleSelection ( sqlSelector, "NEW" );
			//			Utils . GetWindowHandles ( );
		}

		//********************************************************************************************//
		private void DeleteAllViewers_Click ( object sender, RoutedEventArgs e )
		{
			if ( ViewersList . Items . Count == 1 )
				return;
			HandleSelection ( ViewersList, "DELETEALL" );
		}

		//*******************************MAIN KEY HANDLER FOR LIST BOXES*************************************//
		private void IsEnterKey ( object sender, KeyEventArgs e )
		{
			//			Debug . WriteLine ( $"Key1 = {key1}, Key : {e . Key . ToString ( )}" );
			//PreviewKeyDown - in either list
			//return;
			if ( e . Key == Key . LeftCtrl )
			{
				key1 = true;
			}
			if ( key1 )
			{
				Utils . HandleCtrlFnKeys ( key1, e );
				key1 = false;
				return;
			}
			else if ( e . Key == Key . Enter )
			{
				if ( CurrentList == 1 )
				{ // Top list - new Viewer type
				  //					sqlselectorbtn_Select (sender, null);
					HandleSelection ( sqlSelector, "NEW" );
				}
				else if ( CurrentList == 2 )
				{ // Lower list (open Viewers)
					if ( ViewersList . SelectedIndex == -1 )
						return;

					HandleSelection ( ViewersList, "SELECT" );
				}
				key1 = false;
				return;
			}
			else if ( e . Key == Key . NumPad2 || e . Key == Key . Down )
			{
				ListBox lb = sender as ListBox;
				if ( lb . SelectedIndex < lb . Items . Count - 1 )
					lb . SelectedIndex++;
				key1 = false;
				return;
			}
			else if ( key1 && e . Key == Key . F12 )
			{
				if ( key1 )
				{
					Flags . ShowAllFlags ( );
					key1 = false;
				}
				return;
			}
			else if ( e . Key == Key . NumPad8 || e . Key == Key . Up )
			{
				ListBox lb = sender as ListBox;
				if ( lb . SelectedIndex > 0 )
					lb . SelectedIndex--;
				key1 = false;
				return;
			}																																									     
			else if ( e . Key == Key . Home )
			{
				Flags . ListGridviewControlFlags ( );
				key1 = false;
				return;
			}
			else if ( key1 && e . Key == Key . End )
			{
				Flags . ListGridviewControlFlags ( 1 );
				key1 = false;
				return;
			}
			else if ( e . Key == Key . Escape )
			{
				Flags . DbSelectorOpen = null;
				Close ( );
				return;
			}
		}

		//********************************************************************************************//
		private void Window_Closing ( object sender, System . ComponentModel . CancelEventArgs e )
		{
			Flags . DbSelectorOpen = null;
		}

		private void MultiViewer_Click ( object sender, RoutedEventArgs e )
		{
			if ( Flags . SqlMultiViewer != null )
			{
				Flags . SqlMultiViewer . BringIntoView ( );
				Flags . SqlMultiViewer . Focus ( );
				return;
			}

			MultiViewer mv = new MultiViewer ( );
//			mv . Show ( );
		}

		private void Grab_GetObject( object sender , MouseEventArgs e )
		{
			if ( e . RightButton == MouseButtonState . Pressed )
				return;
			Point pt = e.GetPosition((UIElement)sender);
			HitTestResult hit = VisualTreeHelper . HitTest ( ( Visual ) sender, pt );
			if ( hit ?. VisualHit != null )
			{
				if ( Utils.ControlsHitList . Count != 0 )
				{
					if ( hit . VisualHit == Utils . ControlsHitList [ 0 ] . VisualHit )
						return;
				}
				Utils . ControlsHitList . Clear ( );
				Utils . ControlsHitList . Add ( hit );
			}
			e . Handled = true;
		}
		private void Window_KeyDown ( object sender, KeyEventArgs e )
		{
			if ( e . Key == Key . LeftCtrl )
			{
				key1 = true;
				return;
			}
			if ( key1 )
			{
				Utils . HandleCtrlFnKeys ( key1, e );
				key1 = false;
				return;
			}
			else if ( key1 && e . Key == Key . System )     // CTRL + F10
			{
				// Major  listof GV[] variables (Guids etc]
				Debug . WriteLine ( "\nGridview GV[] Variables" );
				Flags . ListGridviewControlFlags ( 1 );
				key1 = false;
				e . Handled = true;
				return;
			}
			else if ( e . Key == Key . F11)     // F11
			{
				if ( Utils . ControlsHitList . Count == 0 )
				{
					key1 = false;
					e . Handled = true;
					return;
				}
				Utils . Grabscreen ( this , Utils . ControlsHitList [ 0 ] . VisualHit , null , sender as Control );
				key1 = false;
				e . Handled = true;
				return;
			}
			else if ( e . Key == Key . OemQuestion )
			{
				// list Flags in Console
				Flags . PrintSundryVariables ( "Window_PreviewKeyDown()" );
				e . Handled = true;
				key1 = false;
				return;
			}
			else if ( e . Key == Key . RightAlt ) //|| e . Key == Key . LeftCtrl )
			{       // list Flags in Console
				Flags . ListGridviewControlFlags ( );
				e . Handled = true;
				key1 = false;
				return;
			}
		}

		/// <summary>
		/// Called byViewers when  focus  changes between them
		/// </summary>
		/// <param name="sqlv"></param>
		public static void SelectActiveViewer ( SqlDbViewer sqlv )
		{
			Guid tag = Guid . Empty;
			tag = ( Guid ) sqlv . Tag;
			for ( int x = 0 ; x < MainWindow . gv . MaxViewers ; x++ )
			{
				// find Tag that matches our Tag in ViewersList
				if ( MainWindow . gv . ListBoxId [ x ] == tag )
				{
					for ( int i = 1 ; i < Flags . DbSelectorOpen . ViewersList . Items . Count ; i++ )
					{
						ListBoxItem lbi = Flags . DbSelectorOpen . ViewersList . Items [ i ] as ListBoxItem;
						if ( ( Guid ) lbi . Tag == tag )
						{
							Flags . DbSelectorOpen . ViewersList . SelectedIndex = i;
							Flags . DbSelectorOpen . ViewersList . SelectedItem = i;
							Flags . DbSelectorOpen . ViewersList . Refresh ( );
							break;
						}
					}
					//					Flags . DbSelectorOpen . ViewersList . SelectedIndex = x ;
					//					Flags . DbSelectorOpen . ViewersList . SelectedItem = x ;
					//					Flags . DbSelectorOpen . ViewersList . Refresh ( );
					break;
				}
			}
		}

		/// <summary>
		///  Refresh the text Content of the ViewersList entires ot match currently selected  Viewer
		/// </summary>
		public void UpdateViewersList ( )
		{
			if ( this . Tag == null ) return;
			if ( MainWindow . gv . DbSelectorWindow . ViewersList . Items . Count == 1 )
				return;
			for ( int i = 0 ; i < MainWindow . gv . DbSelectorWindow . ViewersList . Items . Count ; i++ )
			{
				if ( i + 1 == MainWindow . gv . DbSelectorWindow . ViewersList . Items . Count )
					return;
				if ( MainWindow . gv . ListBoxId [ i ] == ( Guid ) Flags . CurrentSqlViewer . Tag )
				{
					ListBoxItem lbi = new ListBoxItem ( );
					lbi = Flags . DbSelectorOpen . ViewersList . Items [ i + 1 ] as ListBoxItem;
					lbi . Content = MainWindow . gv . CurrentDb [ i ];
					break;
				}
			}
		}

		#region GetInstance

		//*****************************************************************************************//
		//this is really clever stuff
		// It lets me call standard methods (private, public, protected etc)
		//from INSIDE a Static method
		// using syntax : GetInstance().MethodToCall();
		//and it works really great
		private static DbSelector _DbsInstance;

		public static DbSelector GetDbsInstance ( )
		{
			if ( _DbsInstance == null )
				_DbsInstance = new DbSelector ( );
			return _DbsInstance;
		}

		private static SqlDbViewer _SqlInstance;

		public static SqlDbViewer GetSqlInstance ( )
		{
			if ( _SqlInstance == null )
				_SqlInstance = new SqlDbViewer ( );
			return _SqlInstance;
		}

		private static CustomerViewModel _CvInstance;

		public static CustomerViewModel GetCvInstance ( )
		{
			if ( _CvInstance == null )
				_CvInstance = new CustomerViewModel ( );
			return _CvInstance;
		}

		private static BankAccountViewModel _BkInstance;

		public static BankAccountViewModel GetBkInstance ( )
		{
			if ( _BkInstance == null )
				_BkInstance = new BankAccountViewModel ( );
			return _BkInstance;
		}

		private static DetailsViewModel _DetInstance;

		public static DetailsViewModel GetDetInstance ( )
		{
			if ( _DetInstance == null )
				_DetInstance = new DetailsViewModel ( );
			return _DetInstance;
		}

		#endregion GetInstance

		private void ViewersList_PreviewMouseDown ( object sender, MouseButtonEventArgs e )
		{
			HandleSelection ( ViewersList, "SELECT" );
		}

		#region PropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged ( string PropertyName )
		{
			if ( null != PropertyChanged )
			{
				PropertyChanged ( this,
					new PropertyChangedEventArgs ( PropertyName ) );
			}
		}

		#endregion PropertyChanged

		//		private void AddViewerToListFromTuple (Tuple<SqlDbViewer, string, int> tuple)
		//		//NOT USED
		//		{
		//			//Create/Add new viewer entry (ListBoxItem) to Selection viewer Listbox
		//			/*
		//				 Item1 = current SqlDbViewer
		//				Item2 = CurrentDb string`
		//				Item3 = Grid.SelectedIndex
		//			 */
		//			ListBoxItem lbi = new ListBoxItem ();
		//			{
		//				// Set Tag of this LB Item to the DbViewer Window
		//				SendViewerCommand (102, ">>> Starting AddViewerToListFromTuple()", Flags.CurrentSqlViewer);
		//				SqlDbViewer sqlv = tuple.Item1 as SqlDbViewer;
		//				lbi.Tag = sqlv.Tag;
		//				Flags.DbSelectorOpen.ListBoxItemText = ">>>>>>>>>>>";
		//				//This is the normal way to update the lists data
		//				if (tuple.Item2 == "BANKACCOUNT")
		//				{
		//					var v = sqlv.BankGrid.SelectedItem as BankAccountViewModel;

		//				}
		//				else if (tuple.Item2 == "CUSTOMER")
		//				{
		//					var v = sqlv.BankGrid.SelectedItem as CustomerViewModel;

		//				}
		//				else if (tuple.Item2 == "DETAILS")
		//				{
		//					var v = sqlv.BankGrid.SelectedItem as DetailsViewModel;

		//				}

		////				AddViewerToList (MainWindow.gv.PrettyDetails, Flags.CurrentSqlViewer);

		//				//lbi.Content = MainWindow.gv.PrettyDetails;

		//				//int indx = this.ViewersList.Items.Add (lbi);
		//				//this.ViewersList.SelectedIndex = indx;
		//				this.ViewersList.Items.Refresh ();
		//				ExtensionMethods.Refresh (this.ViewersList);

		//				if (this.ViewersList.Items.Count > 1)
		//				{
		//					if (this.ViewersList.Items.Count > 1)
		//						this.ViewerDeleteAll.IsEnabled = true;
		//					else
		//						this.ViewerDeleteAll.IsEnabled = false;
		//					this.ViewerDelete.IsEnabled = true;
		//					this.SelectViewerBtn.IsEnabled = true;
		//				}
		//				this.ViewersList.BringIntoView ();
		//				ExtensionMethods.Refresh (this.ViewersList);
		//				// This WORKS for details 2/4/21
		//				Debug.WriteLine ($" *** Current Active...3 =  {Flags.ActiveSqlGridStr}\r\n");
		//				if (Flags.ActiveSqlGrid?.ItemsSource != null)
		//					CollectionViewSource.GetDefaultView (Flags.ActiveSqlGrid.ItemsSource).Refresh ();
		//				SendViewerCommand (103, "<<< Ended AddViewerToList()", Flags.CurrentSqlViewer);

		//				Mouse.OverrideCursor = Cursors.Arrow;

		//				//				dg.CurrentSqlViewer.Focus ();
		//				//				break;
		//			}
		//		}

		private void OntopChkbox_Click ( object sender, RoutedEventArgs e )
		{
			if ( OntopChkbox . IsChecked == true )
				this . Topmost = true;
			else
				this . Topmost = false;
		}


		private void Closeapp_Click ( object sender, RoutedEventArgs e )
		{
			Application . Current . Shutdown ( );
		}

		//private void ComboBox_SelectionChanged ( object sender, SelectionChangedEventArgs e )
		//{
		//	if ( StartUp ) return;
		//	// Open selected Db viewer
		//	var p = ViewerTypes . SelectedItem;// as PropertyInfo ;
		//	string s = ViewerTypes . Text;
		//	//var q = 	 GetValue ( p);
		//	s = p . ToString ( );
		//	//Color selectedColor = ( Color ) ( cmbColors . SelectedItem as PropertyInfo ) . GetValue ( null, null );
		//	//this . Background = new SolidColorBrush ( selectedColor );
		//	if ( s . Contains ( "Bank" ) )
		//	{
		//		Window handle = null;
		//		if ( Utils . FindWindowFromTitle ( "Bank a/c editor", ref handle ) )
		//		{
		//			handle . Focus ( );
		//			handle . BringIntoView ( );
		//			return;
		//		}
		//		else
		//		{
		//			BankDbView cdbv = new BankDbView ( );
		//			cdbv . Show ( );
		//		}
		//	}
		//	else if ( s . Contains ( "Customer" ) )
		//	{
		//		Window handle = null;
		//		if ( Utils . FindWindowFromTitle ( "customer account editor", ref handle ) )
		//		{
		//			handle . Focus ( );
		//			handle . BringIntoView ( );
		//			return;
		//		}
		//		else
		//		{
		//			CustDbView cdbv = new CustDbView ( );
		//			cdbv . Show ( );
		//		}
		//	}
		//	else if ( s . Contains ( "Details" ) )
		//	{
		//		Window handle = null;
		//		if ( Utils . FindWindowFromTitle ( "details a/c editor", ref handle ) )
		//		{
		//			handle . Focus ( );
		//			handle . BringIntoView ( );
		//			return;
		//		}
		//		else
		//		{
		//			DetailsDbView cdbv = new DetailsDbView ( );
		//			cdbv . Show ( );
		//		}
		//	}
		//}

		//********************************************************************************************//
		public static void UpdateControlFlags ( SqlDbViewer caller, string callertype, string PrettyString )
		{
			int x = 0;
			//			Debug . WriteLine ( $"In UpdateControlFlags setting up gv[] structure variables..." );
			// We are starting up a new viewer, so need to create the flags structure
			// Get the first empty set of structures  and fill them out ofr this NEW Viewer Window
			for ( x = 0 ; x < 3 ; x++ )
			{
				// inserted here 29/4/21 to clear Viewerslist when a viewer window closes
				if ( caller == null && callertype == null )
				{
					ListBoxItem lbi = new ListBoxItem ( );

					for ( int i = 1 ; i < MainWindow . gv . DbSelectorWindow . ViewersList . Items . Count ; i++ )
					{
						if ( i >= MainWindow . gv . DbSelectorWindow . ViewersList . Items . Count )
							return;
						if ( MainWindow . gv . PrettyDetails == PrettyString )
						{
							lbi = Flags . DbSelectorOpen . ViewersList . Items [ i ] as ListBoxItem;
							if ( lbi != null )
								lbi . Content = "";
							Flags . DbSelectorOpen . ViewersList . Items . RemoveAt ( i );
							Flags . DbSelectorOpen . ViewersList . Refresh ( );
							break;
						}
					}
					return;
				}
				if ( MainWindow . gv . ListBoxId [ x ] == Guid . Empty )
				{
					MainWindow . gv . CurrentDb [ x ] = callertype;
					MainWindow . gv . window [ x ] = Flags . CurrentSqlViewer;
					MainWindow . gv . PrettyDetails = PrettyString;

					if ( callertype == "BANKACCOUNT" )
					{
						MainWindow . gv . Datagrid [ x ] = caller . BankGrid;
						MainWindow . gv . Bankviewer = MainWindow . gv . ListBoxId [ x ] = ( Guid ) caller . Tag;
						MainWindow . gv . SqlBankViewer = caller;
						Flags . SqlBankViewer = caller;
						Flags . SqlBankViewer . Tag = ( Guid ) caller . Tag;
					}
					else if ( callertype == "CUSTOMER" )
					{
						MainWindow . gv . Datagrid [ x ] = caller . CustomerGrid;
						MainWindow . gv . Custviewer = MainWindow . gv . ListBoxId [ x ] = ( Guid ) caller . Tag;
						MainWindow . gv . SqlCustViewer = caller;
						Flags . SqlCustViewer = caller;
						Flags . SqlCustViewer . Tag = ( Guid ) caller . Tag;
					}
					else if ( callertype == "DETAILS" )
					{
						MainWindow . gv . Datagrid [ x ] = caller . DetailsGrid;
						MainWindow . gv . Detviewer = MainWindow . gv . ListBoxId [ x ] = ( Guid ) caller . Tag;
						MainWindow . gv . SqlDetViewer = caller;
						Flags . SqlDetViewer = caller;
						Flags . SqlDetViewer . Tag = ( Guid ) caller . Tag;
					}

					MainWindow . gv . ViewerCount++;
					MainWindow . gv . SqlViewerWindow = caller;
					break;
				}
			}
		}

		private void Bankedit_Click ( object sender, RoutedEventArgs e )
		{
			Window handle = null;
			if ( Utils . FindWindowFromTitle ( "Bank a/c editor", ref handle ) )
			{
				handle . Focus ( );
				handle . BringIntoView ( );
				return;
			}
			else
			{
				BankDbView cdbv = new BankDbView ( );
				cdbv . Show ( );
			}
		}
		private void Cust_Click ( object sender, RoutedEventArgs e )
		{
			Window handle = null;
			if ( Utils . FindWindowFromTitle ( "Customer Account Editor", ref handle ) )
			{
				handle . Focus ( );
				handle . BringIntoView ( );
				return;
			}
			else
			{
				CustDbView cdbv = new CustDbView ( );
				cdbv . Show ( );
			}
		}

		private void Det_Click ( object sender, RoutedEventArgs e )
		{
			Window handle = null;
			if ( Utils . FindWindowFromTitle ( "Details A/C Editor", ref handle ) )
			{
				handle . Focus ( );
				handle . BringIntoView ( );
				return;
			}
			else
			{
				DetailsDbView cdbv = new DetailsDbView ( null, null, this);
				cdbv . Show ( );
			}
		}

		private void DragDrop_Click ( object sender, RoutedEventArgs e )
		{
			DragDropClient ddc = new DragDropClient ( );
			ddc . Show ( );
		}

		private void ReloadText_Click ( object sender, RoutedEventArgs e )
		{

		}

		private void SaveData_Click ( object sender, RoutedEventArgs e )
		{

		}

		private void ClearGrid_Click ( object sender, RoutedEventArgs e )
		{

		}

		private void ClearText_Click ( object sender, RoutedEventArgs e )
		{

		}

		private void LoadDragClient_Click ( object sender, RoutedEventArgs e )
		{
			DragDropClient ddc = new DragDropClient ( );
			e . Handled = true;
			//ddc . Show ( );
		}

		private void xxxt_Click ( object sender, RoutedEventArgs e )
		{

		}

		private void LoadDragDrop ( object sender, RoutedEventArgs e )
		{
			if ( Flags . DragDropViewer == null ) {
				DragDropClient ddc = new DragDropClient ( );
				ddc . Show ( );
				Flags . DragDropViewer . RemoteLoadGrid ( );
			}
			else
				Flags . DragDropViewer . RemoteLoadGrid ( );
		}

		private void LoadBankDbView_Click ( object sender, RoutedEventArgs e )
		{
			BankDbView bdv = new BankDbView ( );
			bdv . Show ( );

		}

		private void LoadCustDbView_Click ( object sender, RoutedEventArgs e )
		{
			CustDbView cdv = new CustDbView ( );
			cdv . Show ( );
		}

		private void LoadDetailsDbView_Click ( object sender, RoutedEventArgs e )
		{
			DetailsDbView dbv = new DetailsDbView ( );
			dbv . Show ( );
		}

		private void LoadMultiView_Click ( object sender, RoutedEventArgs e )
		{
			MultiViewer mv = new MultiViewer ( );
			mv . Show ( );
		}

		private void Paths_Click ( object sender, RoutedEventArgs e )
		{
			if ( Flags . ExecuteViewer != null )
			{
				Flags . ExecuteViewer . BringIntoView ( );
				Flags . ExecuteViewer . Focus ( );
				return;
			}
			RunSearchPaths rsp = new RunSearchPaths ( );
			rsp . Show ( );
		}
		private void ToggleEnable ( bool value) {
			SelectBtn . IsEnabled = value;
			MultiViewer . IsEnabled = value;
			MultiViewer . IsEnabled = value;
			ViewerDelete . IsEnabled = value;
			ViewerDeleteAll . IsEnabled = value;
			SelectViewerBtn . IsEnabled = value;
			sqlSelector . IsEnabled = value;
			ViewersList . IsEnabled = value;
			//menu1 . IsEnabled = value;
			//menu2 . IsEnabled = value;
			//menu3 . IsEnabled = value;
		}
		private void Execute_Click ( object sender, RoutedEventArgs e )
		{
			ToggleEnable ( false );
			ExecuteFile . Visibility = Visibility . Visible;
			ExecuteFile . BringIntoView ( );
			execName . Focus ( );
		}

		private void Exec_Click ( object sender, RoutedEventArgs e )
		{
//			SupportMethods . ProcessExecuteRequest ( this, null, null, execName . Text );
		}

		private void scratch_Click ( object sender, RoutedEventArgs e )
		{
			ToggleEnable ( true);
			ExecuteFile . Visibility = Visibility . Collapsed;
		}

		private void ContextClose_Click ( object sender, RoutedEventArgs e )
		{
			Close (  );
		}

		private void ContextSave_Click ( object sender, RoutedEventArgs e )
		{

		}

		private void ContextEdit_Click ( object sender, RoutedEventArgs e )
		{

		}

		private void ContextSettings_Click ( object sender, RoutedEventArgs e )
		{
			Setup setup = new Setup ( );
			setup . Show ( );
			setup . BringIntoView ( );
			setup . Topmost = true;
			this . Focus ( );
		}

		private void ContextDisplayJsonData_Click ( object sender, RoutedEventArgs e )
		{

		}

		private void ContextShowJson_Click ( object sender, RoutedEventArgs e )
		{

		}

		private void UserListbox_Click ( object sender, RoutedEventArgs e )
		{
			MultiviewListBoxViewers dblw = new MultiviewListBoxViewers( );
			dblw . Show ( );
		}

		private void Colors_Click ( object sender, RoutedEventArgs e )
		{
			ColorsSelector cs = new ColorsSelector ( "");
			cs . Show ( );
		}

		private void TreeView_Click ( object sender, RoutedEventArgs e )
		{
			TreeView1 tv1 = new TreeView1 ( );
			tv1 . Show ( );
		}

		private void LoadFullNorthwind_Click ( object sender, RoutedEventArgs e )
		{
			NorthwindFullData nwg = new NorthwindFullData ( );
			nwg . Show ( );
		}
		private void LoadSelectedNorthwind_Click ( object sender, RoutedEventArgs e )
		{
			SelectedNwDetails nwg = new SelectedNwDetails ( );
			nwg . Show ( );
		}
		private void LoadBasicNorthwind_Click ( object sender, RoutedEventArgs e )
		{
//			NorthWindGrid nwg = new NorthWindGrid ( );
			
//			nwg . Show ( );
		}

		private void sysColors_Click ( object sender, RoutedEventArgs e )
		{
			SysColors sc = new SysColors ( );
			sc . Show ( );
		}

		private void Backgrounds_Click ( object sender, RoutedEventArgs e )
		{
//			BackgroundDesigner bd = new BackgroundDesigner ("" );
//			e . Handled = true;
			//bd . Show ( );
		}

		private void Animate_Click ( object sender , RoutedEventArgs e )
		{
			AnimationTest at = new AnimationTest ( );
			at . Show ( );
			//e . Handled = true;
		}

		private void AnimMaster_Click ( object sender , RoutedEventArgs e )
		{
			ThreeDeeBtnControl tdbc = new ThreeDeeBtnControl()  ;
//				tdbc . Show ( );
			//e . Handled = true;
		}

                private void ButtonTesting_Click ( object sender, RoutedEventArgs e )
                {
                        ButtonTesting btest = new ButtonTesting ( );
                        btest . Show ( );
                }

                private void MoreTesting_Click ( object sender, RoutedEventArgs e )
                {
                        MoreTesting tst = new MoreTesting ( );
                        tst . Show ( );
                }

                private void MenuItem_Click ( object sender, RoutedEventArgs e )
                {

                }

                private void StoryBoard_Click ( object sender, RoutedEventArgs e )
                {
                        Stylingtest test = new Stylingtest ( );
                       test. Show ( );

                }

                private void Attached_Click ( object sender, RoutedEventArgs e )
                {
//                        ListBoxWindow lbw = new ListBoxWindow ( );
  //                      lbw . Show ( );
                }

            private void Styled_Click ( object sender , RoutedEventArgs e )
            {
    
            }

		private void CustomMsgbox_Click ( object sender , RoutedEventArgs e )
		{
			AboutBox cm = new AboutBox ();
			cm .Show ( );
		}

		private void Bankaccount_Click ( object sender , RoutedEventArgs e )
		{
			Bankaccount ba = new Bankaccount();
			ba .Show ( );
		}

		private void Clock_Click ( object sender , RoutedEventArgs e )
		{
//			AnalogClockHost ah = new AnalogClockHost();
//			ah .Show ( );
		}

		private void Clock2_Click ( object sender , RoutedEventArgs e )
		{
			FullWPFWindow1 fw = new FullWPFWindow1();
			fw .Show ( );
		}

	
		private void ItemsControl_Click ( object sender , RoutedEventArgs e )
		{
			ItemsControlDemo id = new ItemsControlDemo();
			id . Show ( );
		}

		private void Grouping_Click ( object sender , RoutedEventArgs e )
		{
			GroupedAccounts ga = new GroupedAccounts();
			ga . Show ( );
		}

		private void DaperTesting_Click ( object sender , RoutedEventArgs e )
		{
			DapperTesting dpt = new DapperTesting();
			dpt . Show ( );
		}

		private void Dapper_Select ( object sender , RoutedEventArgs e )
		{
			DapperTesting dpt = new DapperTesting();
			dpt . Show ( );
		}

		private void Dbselector_MouseEnter ( object sender , MouseEventArgs e )
		{
			Maingrid . Opacity = 1;
//			this.Focusable = true;
//			Focus ( );
		}

		private void Dbselector_MouseLeave ( object sender , MouseEventArgs e )
		{
			//Maingrid.Background = "Transparent" . ToSolidBrush ( );
			Maingrid . Opacity = 0.4;
		}

		private void Msgbox_Click ( object sender , RoutedEventArgs e )
		{
			Window w = new MsgboxSetup();
			w . Show ( );
		}

		private void MsgboxAPs_Click ( object sender , RoutedEventArgs e )
		{
			MainWindow.ShowAPDatatoConsole ( );

		}

		private void CreateCookie_Click ( object sender , RoutedEventArgs e )
		{
			NewCookie  nc = new NewCookie ( );
			nc . ShowDialog ( );
		}

		private void ListCookieData_Click ( object sender , RoutedEventArgs e )
		{
			Cookies . ShowAllCookieData ( out int total , "" );
		}

		private void NewCookie_Click ( object sender , RoutedEventArgs e )
		{
			Utils . NewCookie_Click ( null , null );
		}

		private void Menu_Select ( object sender , RoutedEventArgs e )
		{
			if ( MainWindow . sysmenu == null )
			{
				SysMenu sm = new SysMenu();
				sm . Show ( );
			}
			else
			{
				MainWindow . sysmenu . Focus ( );
				return;
			}
		}

		private void Context_DbViewers ( object sender , RoutedEventArgs e )
		{
			var mnu = FindResource ( "Context_DbViewers" ) as ContextMenu;
			mnu . IsOpen = true;
		}
	}
}