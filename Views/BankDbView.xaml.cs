﻿using System;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . ComponentModel;
using System . Diagnostics;
using System . Globalization;
using System . IO;
using System . Linq;
using System . Threading;
using System . Threading . Tasks;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Data;
using System . Windows . Input;
using System . Windows . Media;
using System . Windows . Media . Imaging;
using WPFPages . Commands;
using Newtonsoft . Json;

using WPFPages . ViewModels;
namespace WPFPages . Views
{

	/// <summary>
	/// Interaction logic for BankDbView.xaml
	/// </summary>
	public partial class BankDbView : Window
	{
//		private  BankCollection BankView = null;
		private Stopwatch timer = new Stopwatch();
		static int counter = 0;

		private static ObservableCollection<BankAccountViewModel> BankAccounts = new  ObservableCollection<BankAccountViewModel>();
		//private static CollectionView BankView;
		private static readonly DataGridColumn dataGridColumn   ;
		private DataGridColumn[] DGBankColumnsCollection = {dataGridColumn,dataGridColumn,dataGridColumn,dataGridColumn,
			dataGridColumn,dataGridColumn,dataGridColumn,dataGridColumn };

		// Get our personal Collection view of the Db
		private static CollectionView BankView
		{
			get; set;
		}
		#region Declarations
		private bool IsDirty = false;
		static bool Startup = true;
		private bool LinktoParent = false;
		private bool LinkToAllRecords = false;
		private bool LinktoMultiParent = false;
		//		private bool Triggered = false;
		private bool LoadingDbData = false;
		private bool RowHasBeenEdited
		{
			get; set;
		}
		private bool keyshifted
		{
			get; set;
		}
		private bool IsEditing
		{
			get; set;
		}
		public static int bindex
		{
			get; set;
		}
		public bool IsLeftButtonDown
		{
			get; set;
		}

		private Point _startPoint
		{
			get; set;
		}
		private string _bankno = "";
		private string _custno = "";
		private string _actype = "";
		private string _balance = "";
		private string _odate = "";
		private string _cdate = "";
		private SqlDbViewer SqlParentViewer;
		private MultiViewer MultiParentViewer;
		private Thread t1;

		// Crucial structure for use when a Grid row is being edited
		private RowData bvmCurrent = null;
		#endregion Declarations

		public BankDbView ( )
		{
			Startup = true;
			InitializeComponent ( );
			this . Show ( );
			//Identify individual windows for update protection
			this . Tag = ( Guid ) Guid . NewGuid ( );
			this . Refresh ( );
			this . DataContext = BankView;
			// This STOPS all those infuriating binding debug messages from appearing
			// Add it to any window you do not want these messages to show in
			System . Diagnostics . PresentationTraceSources . DataBindingSource . Switch . Level = System . Diagnostics . SourceLevels . Critical;
			MouseMove += Grab_MouseMove;
		}
		private void Grab_MouseMove ( object sender , MouseEventArgs e )
		{
			if ( e . LeftButton == MouseButtonState . Pressed )
				Utils . Grab_MouseMove ( sender , e );
			e . Handled = true;
		}

		#region Mouse support
		//private void DoDragMove ( )
		//{//Handle the button NOT being the left mouse button
		// // which will crash the DragMove Fn.....
		//	try
		//	{ this . DragMove ( ); }
		//	catch { return; }
		//}
		#endregion Mouse support

		#region Startup/ Closedown
		private async void Window_Loaded ( object sender , RoutedEventArgs e )
		{
			Mouse . OverrideCursor = System . Windows . Input . Cursors . Wait;
			this . Show ( );
			this . Refresh ( );
			Startup = true;
			ToggleViewStatus . IsChecked = Flags . UseSharedView;

			ColumnSelection . Items . Add ( 0 );
			ColumnSelection . Items . Add ( 1 );
			ColumnSelection . Items . Add ( 2 );

			string ndx = ( string ) Properties . Settings . Default [ "BankDbView_bindex" ];
			bindex = int . Parse ( ndx );
			this . BankGrid . SelectedIndex = bindex < 0 ? 0 : bindex;

			Utils . SetupWindowDrag ( this );
			// An EditDb has changed the current index 
			EventControl . EditIndexChanged += EventControl_EditIndexChanged;
			// A Multiviewer has changed the current index 
			EventControl . MultiViewerIndexChanged += EventControl_EditIndexChanged;
			// Another viewer has changed the current index 
			EventControl . ViewerIndexChanged += EventControl_EditIndexChanged;      // Callback in THIS FILE
			EventControl . ViewerDataUpdated += EventControl_DataUpdated;
			EventControl . BankDataLoaded += EventControl_BankDataLoaded;
			EventControl . GlobalDataChanged += EventControl_GlobalDataChanged;

			EventControl . ViewSharingChanged += EventControl_ViewSharingChanged;

			Flags . SqlBankActive = true;

			if ( Flags . UseSharedView )
			{
				if ( BankAccountViewModel . BankCollectionView != null )
				{
					//Make sure we have got some data, else we will load it anyway
					//if ( BankAccountViewModel . BankCollectionView . IsEmpty == false )
					//{
					//	//Use  the available App wide View ,so we cannot add/remove Sort Descriptions/Filters etc
					//	BankView = BankAccountViewModel . BankCollectionView;
					//	this . BankGrid . ItemsSource = BankView;
					//	Mouse . OverrideCursor = Cursors . Arrow;
					//}
					//else
					//{
						//						await BankCollection . LoadBank ( BankView , "BANKDBVIEW" , 99 , true );
						DapperSupport . GetBankObsCollection ( collection: BankAccounts ,
							DbNameToLoad:"NewBank"	,
							wantSort: false ,
							Notify: true, 
									Caller:"BANKDBVIEW");
					//}
				}
			}
			else
			{
				//				await BankCollection . LoadBank ( BankView , "BANKDBVIEW" , 99 , true );
				DapperSupport . GetBankObsCollection ( collection: BankAccounts ,
						wantSort: false ,
							Notify: true,
							Caller: "BANKDBVIEW" );
			}
			BankView = CollectionViewSource . GetDefaultView ( BankAccounts ) as CollectionView;

			//			BankCollection . LoadBank ( BankView , "BANKDBVIEW" , 3 , true );

			SaveBttn . IsEnabled = false;
			// Save linkage setting as we need to disable it while we are loading
			bool tmp = Flags . LinkviewerRecords;

			Flags . BankDbEditor = this;
			// Set window to TOPMOST
			OntopChkbox . IsChecked = true;
			this . Topmost = true;
			this . Focus ( );
			// Reset linkage setting
			Flags . LinkviewerRecords = tmp;
			if ( Flags . LinkviewerRecords )
			{
				LinkRecords . IsChecked = true;
				LinkToAllRecords = true;
				LinktoParent = true;
			}
			else
			{
				LinkRecords . IsChecked = false;
				LinkToAllRecords = false;
				LinktoParent = false;
			}
			LinktoMultiParent = false;
			// start our linkage monitor
			t1 = new Thread ( checkLinkages );
			t1 . IsBackground = true;
			t1 . Priority = ThreadPriority . Lowest;
			t1 . Start ( );
			Startup = false;
			Utils . SetGridRowSelectionOn ( BankGrid , 0 );
		}


		private void EventControl_GlobalDataChanged ( object sender , GlobalEventArgs e )
		{
			if ( e . CallerType == "BANKDBVIEW" )
				return;
			Flags . SqlBankActive = true;
			//Force it to reload using the correct SortOrder
			//			BankCollection . LoadBank ( null , "BANKDBVIEW" , 1 , true );
			DapperSupport . GetBankObsCollection ( collection: BankAccounts ,
					wantSort: false ,
						Notify: true,
						Caller: "BANKDBVIEW" );
			BankView = CollectionViewSource . GetDefaultView ( BankAccounts ) as CollectionView;
		}

		private void EventControl_EditIndexChanged ( object sender , IndexChangedArgs e )
		{
			//			Triggered = true;
			// Handle Selection change in another windowif linkage is ON
			if ( IsEditing || LinkRecords . IsChecked == false )
			{
				//IsEditing = false;
				return;
			}
			this . BankGrid . SelectedIndex = e . Row;
			bindex = e . Row;
			this . BankGrid . Refresh ( );
			//			Triggered = false;
		}

		private void EventControl_DataUpdated ( object sender , LoadedEventArgs e )
		{
			if ( e . CallerDb == "BANKDBVIEW" || e . CallerDb == "BANKACCOUNT" )
				return;
			int currsel = this . BankGrid . SelectedIndex;
			Debug . WriteLine ( $"BankDbView : Data changed event notification received successfully." );
			this . BankGrid . ItemsSource = null;
			this . BankGrid . Items . Clear ( );
			Mouse . OverrideCursor = Cursors . Wait;
			//Reload our data base data, it will be loaded when we are notified it is ready via the BankDataLoaded Event
			Flags . SqlBankActive = true;
			//			BankCollection . LoadBank ( BankView , "BANKDBVIEW" , 3 , true );
			DapperSupport . GetBankObsCollection ( collection: BankAccounts ,
					wantSort: false ,
						Notify: true,
						Caller: "BANKDBVIEW" );
			BankView = CollectionViewSource . GetDefaultView ( BankAccounts ) as CollectionView;
			IsDirty = false;
		}

		public void ExternalDataUpdate ( int DbEditChangeType , int row , string currentDb )
		{
			// Reciiving Notifiaction from a remote viewer that data has been changed, so we MUST now update our DataGrid
			Debug . WriteLine ( $"BankDbView : Data changed event notification received successfully." );
			this . BankGrid . ItemsSource = null;
			this . BankGrid . Items . Clear ( );
			this . BankGrid . ItemsSource = BankView;
			this . BankGrid . Refresh ( );
		}
		#endregion Startup/ Closedown

		#region DATA EDIT CONTROL METHODS
		/// <summary>
		///  DATA EDIT CONTROL METHODS
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void BankGrid_BeginningEdit ( object sender , DataGridBeginningEditEventArgs e )
		{
			IsEditing = true;
			// Save  the current data for checking later on when we exit editing
			// but first, check to see if we already have one being saved !
			if ( bvmCurrent == null )
			{
				// Nope, so create a new one and get on with the edit process
				BankAccountViewModel tmp = new BankAccountViewModel ( );
				tmp = e . Row . Item as BankAccountViewModel;
				// This sets up a new bvmControl object if needed, else we  get a null back
				bvmCurrent = CellEditControl . BankGrid_EditStart ( bvmCurrent , e );
			}
			// doesn't work right now - returns NULL
			//string str = CellEditControl.GetSelectedCellValue ( this . BankGrid );
		}

		/// <summary>
		/// does nothing at all because it is called whenver any single cell is exited
		///     and not just when ENTER is hit to save any changes
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void BankGrid_CellEditEnding ( object sender , DataGridCellEditEndingEventArgs e )
		{
			if ( bvmCurrent == null )
				return;

			// Has Data been changed in one of our rows. ?
			BankAccountViewModel dvm = this . BankGrid . SelectedItem as BankAccountViewModel;
			dvm = e . Row . Item as BankAccountViewModel;

			// The sequence of these next 2 blocks is critical !!!
			//if we get here, make sure we have been NOT been told to EsCAPE out
			//	this is a DataGridEditAction dgea
			if ( e . EditAction == 0 )
			{
				// ENTER was hit, so data has been saved - go ahead and reload our grid with new data
				// and this will notify any other open viewers as well
				bvmCurrent = null;
				Flags . SqlBankActive = true;
				//				BankCollection . LoadBank ( BankView , "BANKDBVIEW" , 1 , true );
				DapperSupport . GetBankObsCollection ( collection: BankAccounts ,
						wantSort: false ,
							Notify: true );
				BankView = CollectionViewSource . GetDefaultView ( BankAccounts ) as CollectionView;
				return;
			}

			if ( CellEditControl . BankGrid_EditEnding ( bvmCurrent , BankGrid , e ) == false )
			{       // No change made
				return;
			}
		}

		/// <summary>
		/// Compares 2 rows of BANKACCOUNT or DETAILS data to see if there are any changes
		/// </summary>
		/// <param name="ss"></param>
		/// <returns></returns>
		private bool CompareDataContent ( BankAccountViewModel ss )
		{
			if ( ss . CustNo != bvmCurrent . _CustNo . ToString ( ) )
				return false;
			if ( ss . BankNo != bvmCurrent . _BankNo . ToString ( ) )
				return false;
			if ( ss . AcType != bvmCurrent . _AcType )
				return false;
			if ( ss . IntRate != bvmCurrent . _IntRate )
				return false;
			if ( ss . Balance != bvmCurrent . _Balance )
				return false;
			if ( ss . ODate != bvmCurrent . _ODate )
				return false;
			if ( ss . CDate != bvmCurrent . _CDate )
				return false;
			return true;
		}
		/// <summary>
		/// Called when an EDIT ends. This occurs whenever a field is exited, even if ENTER has NOT been pressed
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void ViewerGrid_RowEditEnding ( object sender , System . Windows . Controls . DataGridRowEditEndingEventArgs e )
		{
			int currow = 0;
			// if our saved row is null, it has already been checked in Cell_EndDedit processing
			// and found no changes have been made, so we can abort this update
			if ( bvmCurrent == null )
				return;

			// This is now confirmed as being CHANGED DATA in the current row
			// So we proceed and update SQL Db's' and notify all open viewers as well
			BankAccountViewModel ss = new BankAccountViewModel ( );
			ss = this . BankGrid . SelectedItem as BankAccountViewModel;
			SQLHandlers sqlh = new SQLHandlers ( );
			await sqlh . UpdateDbRowAsync ( "BANKACCOUNT" , ss , this . BankGrid . SelectedIndex );

			this . BankGrid . SelectedIndex = currow;
			this . BankGrid . SelectedItem = currow;
			Utils . SetUpGridSelection ( this . BankGrid , currow );
			IsDirty = false;
			// Notify EditDb to upgrade its grid
			if ( Flags . CurrentEditDbViewer != null )
				Flags . CurrentEditDbViewer . UpdateGrid ( "BANKACCOUNT" );

			// ***********  DEFINITE WIN  **********
			// This DOES trigger a notification to SQLDBVIEWER AND OTHERS for sure !!!   14/5/21
			EventControl . TriggerViewerDataUpdated ( BankView ,
				new LoadedEventArgs
				{
					CallerType = "BANKDBVIEW" ,
					CallerDb = "BANKACCOUNT" ,
					DataSource = BankView ,
					SenderGuid = this . Tag . ToString ( ) ,
					RowCount = this . BankGrid . SelectedIndex
				} );
			EventControl . TriggerGlobalDataChanged ( this , new GlobalEventArgs
			{
				CallerType = "BANKDBVIEW" ,
				AccountType = "BANKACCOUNT" ,
				SenderGuid = this . Tag?.ToString ( )
			} );
		}

		#endregion DATA EDIT CONTROL METHODS

		private void Close_Click ( object sender , RoutedEventArgs e )
		{
			Close ( );
		}

		private void Window_Closing ( object sender , System . ComponentModel . CancelEventArgs e )
		{
			if ( ( Flags . LinkviewerRecords == false && IsDirty )
					|| SaveBttn . IsEnabled )
			{
				MessageBoxResult result = MessageBox . Show
					( "You have unsaved changes.  Do you want them saved now ?", "Possible Data Loss", MessageBoxButton . YesNo, MessageBoxImage . Question, MessageBoxResult . Yes );
				if ( result == MessageBoxResult . Yes )
				{
					SaveButton ( );
				}
				// Do not want ot save it, so disable  save button again
				SaveBttn . IsEnabled = false;
				IsDirty = false;
			}
			Flags . BankDbEditor = null;
			EventControl . EditIndexChanged -= EventControl_EditIndexChanged;
			// A Multiviewer has changed the current index 
			EventControl . MultiViewerIndexChanged -= EventControl_EditIndexChanged;
			// Another SqlDbviewer has changed the current index 
			EventControl . ViewerIndexChanged -= EventControl_EditIndexChanged;      // Callback in THIS FILE
			EventControl . ViewerDataUpdated -= EventControl_DataUpdated;
			EventControl . BankDataLoaded -= EventControl_BankDataLoaded;
			EventControl . GlobalDataChanged -= EventControl_GlobalDataChanged;

			//			DataFields . DataContext = this . BankGrid . SelectedItem;
			BankView = null;
			Utils . SaveProperty ( "BankDbView_bindex" , bindex . ToString ( ) );

			// We must clear our "loaded" columns, or else it stopsworking
			ObservableCollection<DataGridColumn> dgc = BankGrid.Columns;
			dgc . Clear ( );
		}

   		private void BankGrid_SelectionChanged ( object sender , System . Windows . Controls . SelectionChangedEventArgs e )
		{
			if ( LoadingDbData )
			{
				LoadingDbData = false;
				return;
			}
			try
			{
				Startup = true;
				try
				{
					DataFields . DataContext = this . BankGrid . SelectedItem as BankAccountViewModel;
				}
				catch ( Exception ex )
				{
					Debug . WriteLine ( $"{ex . Message}, {ex . Data}" );
				}
				bindex = this . BankGrid . SelectedIndex;
				Utils . SetUpGridSelection ( this . BankGrid , this . BankGrid . SelectedIndex );
				if ( LinkToAllRecords )// && Triggered == false )
				{
					TriggerViewerIndexChanged ( this . BankGrid );
				}

				// check to see if an SqlDbViewer has been opened that we can link to
				if ( Flags . SqlBankViewer != null && LinkToParent . IsEnabled == false )
				{
					LinkToParent . IsEnabled = true;
					SqlParentViewer = Flags . SqlBankViewer;
				}
				// Only  do this if global link is OFF
				if ( LinktoParent )// && LinkRecords . IsChecked == false )
				{
					// update parents row selection
					var dvm = this . BankGrid . SelectedItem as BankAccountViewModel;
					if ( dvm == null )
						return;

					if ( SqlParentViewer != null )
					{
						int rec = Utils . FindMatchingRecord ( dvm . CustNo, dvm . BankNo, SqlParentViewer . BankGrid, "BANKACCOUNT" );
						SqlParentViewer . BankGrid . SelectedIndex = rec;
						Utils . SetUpGridSelection ( SqlParentViewer . BankGrid , rec );
					}
					else if ( MultiParentViewer != null )
					{
						int rec = Utils . FindMatchingRecord ( dvm . CustNo, dvm . BankNo, MultiParentViewer . BankGrid, "BANKACCOUNT" );
						MultiParentViewer . BankGrid . SelectedIndex = rec;
						Utils . SetUpGridSelection ( MultiParentViewer . BankGrid , rec );
					}
				}
				if ( LinktoMultiParent )
				{
					Flags . SqlMultiViewer . BankGrid . SelectedIndex = this . BankGrid . SelectedIndex;
					Flags . SqlMultiViewer . BankGrid . ScrollIntoView ( this . BankGrid . SelectedIndex );
					Utils . SetUpGridSelection ( Flags . SqlMultiViewer . BankGrid , this . BankGrid . SelectedIndex );
				}
			}
			catch ( Exception ex )
			{
				Debug . WriteLine ( $"DEBUG : {ex . Message}, {ex . Data}" );
			}
			Count . Text = $"{this . BankGrid . SelectedIndex} / { this . BankGrid . Items . Count . ToString ( )}";

			IsDirty = false;
			Startup = false;
		}

		private async Task<bool> SaveButton ( object sender = null , RoutedEventArgs e = null )
		{
			//inprogress = true;
			//bindex = this . BankGrid . SelectedIndex;
			//cindex = this . CustomerGrid . SelectedIndex;
			//dindex = this . DetailsGrid . SelectedIndex;

			// Get the current rows data
			IsDirty = false;
			int CurrentSelection = this . BankGrid . SelectedIndex;
			this . BankGrid . SelectedItem = this . BankGrid . SelectedIndex;
			BankAccountViewModel bvm = new BankAccountViewModel ( );
			bvm = this . BankGrid . SelectedItem as BankAccountViewModel;
			if ( bvm == null )
				return false;

			SaveFieldData ( );

			// update the current rows data content to send  to Update process
			bvm . BankNo = Bankno . Text;
			bvm . CustNo = Custno . Text;
			bvm . AcType = Convert . ToInt32 ( acType . Text );
			bvm . Balance = Convert . ToDecimal ( balance . Text );
			bvm . ODate = Convert . ToDateTime ( odate . Text );
			bvm . CDate = Convert . ToDateTime ( cdate . Text );
			// Call Handler to update ALL Db's via SQL
			SQLHandlers sqlh = new SQLHandlers ( );
			await sqlh . UpdateDbRow ( "BANKACCOUNT" , bvm );

			BankView = null;
			BankCollection bank = new BankCollection ( );
			BankAccounts = await bank . ReLoadBankData ( );

			EventControl . TriggerViewerDataUpdated ( BankView ,
				new LoadedEventArgs
				{
					CallerType = "BANKDBVIEW" ,
					CallerDb = "BANKACCOUNT" ,
					DataSource = BankView ,
					SenderGuid = this . Tag . ToString ( ) ,
					RowCount = this . BankGrid . SelectedIndex
				} );
			EventControl . TriggerGlobalDataChanged ( this , new GlobalEventArgs
			{
				CallerType = "BANKDBVIEW" ,
				AccountType = "BANKACCOUNT" ,
				SenderGuid = this . Tag?.ToString ( )
			} );

			//Gotta reload our data because the update clears it down totally to null
			//this . BankGrid . SelectedIndex = CurrentSelection;
			//this . BankGrid . SelectedItem = CurrentSelection;
			//this . BankGrid . Refresh ( );

			//this . BankGrid . ItemsSource = null;
			//this . BankGrid . ItemsSource = BankView;
			//this . BankGrid . Refresh ( );

			SaveBttn . IsEnabled = false;
			IsDirty = false;
			return true;
		}


		/// <summary>
		/// Called by ALL edit fields
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SelectionChanged ( object sender , RoutedEventArgs e )
		{
			if ( !Startup )
				SaveFieldData ( );
		}

		/// <summary>
		/// Called by ALL edit fields when text is changed
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TextChanged ( object sender , TextChangedEventArgs e )
		{
			return;
			//if ( !Startup )
			//	CompareFieldData ( );
		}

		private void SaveFieldData ( )
		{
			_bankno = Bankno . Text;
			_custno = Custno . Text;
			_actype = acType . Text;
			_balance = balance . Text;
			_odate = odate . Text;
			_cdate = cdate . Text;
		}
		private void CompareFieldData ( )
		{
			if ( SaveBttn == null )
				return;
			if ( _bankno != Bankno . Text )
				SaveBttn . IsEnabled = true;
			if ( _custno != Custno . Text )
				SaveBttn . IsEnabled = true;
			if ( _actype != acType . Text )
				SaveBttn . IsEnabled = true;
			if ( _balance != balance . Text )
				SaveBttn . IsEnabled = true;
			if ( _odate != odate . Text )
				SaveBttn . IsEnabled = true;
			if ( _cdate != cdate . Text )
				SaveBttn . IsEnabled = true;

			if ( SaveBttn . IsEnabled )
				IsDirty = true;
		}

		private void OntopChkbox_Click ( object sender , RoutedEventArgs e )
		{
			if ( OntopChkbox . IsChecked == ( bool? ) true )
				this . Topmost = true;
			else
				this . Topmost = false;
		}

		private void SaveBtn ( object sender , RoutedEventArgs e )
		{
			SaveButton ( sender , e );
		}

		private void LinkRecords_Click ( object sender , RoutedEventArgs e )
		{
			//			bool reslt = false;
			//if ( IsLinkActive ( reslt ) == false )
			//{
			//	LinkToParent . IsEnabled = false;
			//	LinkToParent . IsChecked = false;
			//	SqlParentViewer = null;
			//	LinkRecords . IsChecked = false;
			//}
			//else
			//{
			//	LinktoParent = !LinktoParent;
			//}
			// force viewers to change records in line with each other
			if ( LinkRecords . IsChecked == true )
			{
				Flags . LinkviewerRecords = true;
				LinkToAllRecords = true;
			}
			else
			{
				Flags . LinkviewerRecords = false;
				LinkToAllRecords = false;
			}
			LinkRecords . Refresh ( );
			if ( Flags . SqlBankViewer != null )
				Flags . SqlBankViewer . LinkRecords . IsChecked = Flags . LinkviewerRecords;
			if ( Flags . SqlCustViewer != null )
				Flags . SqlCustViewer . LinkRecords . IsChecked = Flags . LinkviewerRecords;
			if ( Flags . SqlDetViewer != null )
				Flags . SqlDetViewer . LinkRecords . IsChecked = Flags . LinkviewerRecords;
			if ( Flags . SqlMultiViewer != null )
				Flags . SqlMultiViewer . LinkRecords . IsChecked = Flags . LinkviewerRecords;
			if ( Flags . CustDbEditor != null )
				Flags . CustDbEditor . LinkRecords . IsChecked = Flags . LinkviewerRecords;
			if ( Flags . DetDbEditor != null )
				Flags . DetDbEditor . LinkRecords . IsChecked = Flags . LinkviewerRecords;
			LinkRecords . Refresh ( );
			if ( LinkToAllRecords == true )
			{
				LinktoParent = false;
				LinkToParent . IsEnabled = false;
				LinkToParent . IsChecked = false;
			}
			else
			{
				if ( SqlParentViewer != null )
					LinkToParent . IsEnabled = true;
				else
					LinkToParent . IsEnabled = false;
			}
		}
		/// <summary>
		/// Generic method to send Index changed Event trigger so that 
		/// other viewers can update thier own grids as relevant
		/// </summary>
		/// <param name="grid"></param>
		//*************************************************************************************************************//
		public void TriggerViewerIndexChanged ( DataGrid grid )
		{
			string SearchCustNo = "";
			string SearchBankNo = "";
			if ( grid . ItemsSource == null )
				return;
			BankAccountViewModel CurrentBankSelectedRecord = grid . SelectedItem as BankAccountViewModel;
			SearchCustNo = CurrentBankSelectedRecord . CustNo;
			SearchBankNo = CurrentBankSelectedRecord . BankNo;
			EventControl . TriggerViewerIndexChanged ( this ,
				new IndexChangedArgs
				{
					Senderviewer = this ,
					Bankno = SearchBankNo ,
					Custno = SearchCustNo ,
					dGrid = grid ,
					Sender = "BANKACCOUNT" ,
					SenderId = "BANKDBVIEW" ,
					Row = grid . SelectedIndex
				} );
		}
		private void BankGrid_PreviewMouseRightButtonDown ( object sender , MouseButtonEventArgs e )
		{
			ContextMenu cm = this . FindResource ( "ContextMenu1" ) as ContextMenu;
			cm . PlacementTarget = this . BankGrid as DataGrid;
			cm . IsOpen = true;
		}


		#region Drag/Drop handlers

		private void BankGrid_GiveFeedback ( object sender , GiveFeedbackEventArgs e )
		{
			Mouse . SetCursor ( Cursors . Hand );
			//e . Handled = true;
		}


		private void BankGrid_PreviewQueryContinueDrag ( object sender , QueryContinueDragEventArgs e )
		{
			Mouse . SetCursor ( Cursors . Hand );
			e . Action = DragAction . Continue;
		}

		private void BankGrid_DragEnter ( object sender , DragEventArgs e )
		{
			e . Effects = ( DragDropEffects ) DragDropEffects . Move;
			Mouse . SetCursor ( Cursors . Hand );
		}

		private void BankGrid_PreviewMouseLeftButtondown ( object sender , MouseButtonEventArgs e )
		{
			// Gotta make sure it is not anywhere in the Scrollbar we clicked on 
			if ( Utils . HitTestScrollBar ( sender , e ) )
				return;
			if ( Utils . HitTestHeaderBar ( sender , e ) )
				return;
			_startPoint = e . GetPosition ( null );
			// Make sure the left mouse button is pressed down so we are really moving a record
			if ( e . LeftButton == MouseButtonState . Pressed )
			{
				IsLeftButtonDown = true;
			}
//			e . Handled = true;
		}

		private void BankGrid_PreviewMouseMove ( object sender , MouseEventArgs e )
		{
			Point mousePos = e . GetPosition ( null );
			Vector diff = _startPoint - mousePos;

			if ( e . LeftButton == MouseButtonState . Pressed &&
			    Math . Abs ( diff . X ) > SystemParameters . MinimumHorizontalDragDistance ||
			    Math . Abs ( diff . Y ) > SystemParameters . MinimumVerticalDragDistance )
			{
				if ( IsLeftButtonDown && e . LeftButton == MouseButtonState . Pressed )
				{
					if ( BankGrid . SelectedItem != null )
					{
						// We are dragging from the DETAILS grid
						//Working string version
						BankAccountViewModel bvm = new BankAccountViewModel ( );
						bvm = BankGrid . SelectedItem as BankAccountViewModel;
						string str = GetExportRecords . CreateTextFromRecord ( bvm, null, null, true, false );
						string dataFormat = DataFormats . Text;
						DataObject dataObject = new DataObject ( dataFormat, str );
						DragDrop . DoDragDrop (
						BankGrid ,
						dataObject ,
						DragDropEffects . Copy );
						IsLeftButtonDown = false;
					}
				}
			}
		}
		private void BankGrid_PreviewDragOver ( object sender , DragEventArgs e )
		{
			Point mousePos = e . GetPosition ( BankGrid );
			Vector diff = _startPoint - mousePos;
		}
		#endregion Drag/Drop handlers

		#region Menu Linq handlers

		private void Linq1_Click ( object sender , RoutedEventArgs e )
		{
			//select items;
			var bankaccounts = from items in BankAccounts
						 where ( items . AcType == 1 )
						 orderby items . CustNo
						 select items;
			this . BankGrid . ItemsSource = bankaccounts;
		}
		private void Linq2_Click ( object sender , RoutedEventArgs e )
		{
			//select items;
			var bankaccounts = from items in BankAccounts
						 where ( items . AcType == 2 )
						 orderby items . CustNo
						 select items;
			this . BankGrid . ItemsSource = bankaccounts;
		}
		private void Linq3_Click ( object sender , RoutedEventArgs e )
		{
			//select items;
			var bankaccounts = from items in BankAccounts
						 where ( items . AcType == 3 )
						 orderby items . CustNo
						 select items;
			this . BankGrid . ItemsSource = bankaccounts;
		}
		private void Linq4_Click ( object sender , RoutedEventArgs e )
		{
			//select items;
			var bankaccounts = from items in BankAccounts
						 where ( items . AcType == 4 )
						 orderby items . CustNo
						 select items;
			this . BankGrid . ItemsSource = bankaccounts;
		}
		private void Linq5_Click ( object sender , RoutedEventArgs e )
		{
			//select All the items first;			
			var bankaccounts = from items in BankAccounts orderby items . CustNo, items . AcType select items;
			//Next Group BankAccountViewModel collection on Custno
			var grouped = bankaccounts . GroupBy (
				b => b . CustNo );

			//Now filter content down to only those a/c's with multiple Bank A/c's
			var sel = from g in grouped
				    where g . Count ( ) > 1
				    select g;

			// Finally, iterate thru the list of grouped CustNo's matching to CustNo in the full Bankaccounts data
			// giving us ONLY the full records for any recordss that have > 1 Bank accounts
			List<BankAccountViewModel> output = new List<BankAccountViewModel> ( );
			foreach ( var item1 in sel )
			{
				foreach ( var item2 in bankaccounts )
				{
					if ( item2 . CustNo . ToString ( ) == item1 . Key )
					{
						output . Add ( item2 );
					}
				}
			}
			this . BankGrid . ItemsSource = output;
		}
		private void Linq6_Click ( object sender , RoutedEventArgs e )
		{
			var accounts = from items in BankAccounts orderby items . CustNo, items . AcType select items;
			this . BankGrid . ItemsSource = accounts;
		}

		private void Linq7_Click ( object sender , RoutedEventArgs e )
		{
			string Output = "";
			string path = @"C:\users\ianch\documents\multiaccounts.dat";
			//select All the items first;			
			var bankaccounts = from items in BankAccounts orderby items . CustNo, items . AcType select items;
			//Next Group BankAccountViewModel collection on Custno
			var grouped = bankaccounts . GroupBy (
				b => b . CustNo );

			//Now filter content down to only those a/c's with multiple Bank A/c's
			var sel = from g in grouped
				    where g . Count ( ) > 1
				    select g;

			// Finally, iterate thru the list of grouped CustNo's matching to CustNo in the full Bankaccounts data
			// giving us ONLY the full records for any recordss that have > 1 Bank accounts
			List<BankAccountViewModel> output = new List<BankAccountViewModel> ( );
			foreach ( var item1 in sel )
			{
				foreach ( var item2 in bankaccounts )
				{
					if ( item2 . CustNo . ToString ( ) == item1 . Key )
					{
						output . Add ( item2 );
					}
				}
			}
			foreach ( var item in output )
			{
				Output += item . CustNo + "," + item . BankNo + "," + item . AcType + "," + item . Balance + "," + item . ODate + "," + item . CDate + "\n";
			}
			File . WriteAllText ( path , Output );
			MessageBox . Show ( $"Data saved to file\n{path} successfully " );
		}

		private void Filter_Click ( object sender , RoutedEventArgs e )
		{
			// Show Filter system
			MessageBox . Show ( "Filter dialog will appear here !!" );
		}

		private void Exit_Click ( object sender , RoutedEventArgs e )
		{
			Close ( );
		}

		private void Options_Click ( object sender , RoutedEventArgs e )
		{

		}
		#endregion Menu items

		/// <summary>
		/// Link record selection to parent SQL viewer window only
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LinkToParent_Click ( object sender , RoutedEventArgs e )
		{
			bool reslt = false;
			if ( LinkToParent . IsEnabled == false )
				return;

			if ( LinkToAllRecords == true )
			{
				LinkToParent . IsEnabled = false;
				LinkToParent . IsChecked = false;
				LinktoParent = false;
				return;
			}
			else
			{
				// NOT  linked to All Viewers
				if ( LinkToParent . IsChecked == true )
				{
					LinkRecords . IsEnabled = false;
					LinktoParent = true;
				}
				else
				{
					LinkRecords . IsEnabled = true;
					LinktoParent = false;
				}
			}
		}

		private void Edit_LostFocus ( object sender , RoutedEventArgs e )
		{
			IsDirty = true;
			SaveBttn . IsEnabled = true;
		}
		#region KEYHANDLER for EDIT fields

		// These let us tab thtorugh the editfields back and forward correctly
		private void Window_PreviewKeyUp ( object sender , KeyEventArgs e )
		{
			Debug . WriteLine ( $"  KEYUP key = {e . Key}, Shift = {keyshifted}" );

			if ( e . Key == Key . RightShift || e . Key == Key . LeftShift )
			{
				keyshifted = false;
				return;
			}

			if ( keyshifted && ( e . Key == Key . RightShift || e . Key == Key . LeftShift ) )
			{
				keyshifted = false;
				e . Handled = true;
				return;
			}

		}

		/// <summary>
		/// Key handling to allow proper tabbing between data Editing fieds
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Window_PreviewKeyDown ( object sender , KeyEventArgs e )
		{
			if ( e . Key == Key . RightShift || e . Key == Key . LeftShift )
			{
				keyshifted = true;
				e . Handled = true;
				return;
			}

			if ( keyshifted == false )
			{
				if ( e . Key == Key . Tab && e . Source == cdate )
				{
					e . Handled = true;
					Custno . Focus ( );
					return;
				}
				else if ( e . Key == Key . F11 )
				{
					var pos = Mouse . GetPosition ( this);
					Utils . Grab_Object ( sender , pos );
					if ( Utils . ControlsHitList . Count == 0 )
						return;
					Utils . Grabscreen ( this , Utils . ControlsHitList [ 0 ] . VisualHit , null , sender as Control );
				}
				return;
			}
			else
			{
				// SHIFT KEY DOWN - KEY DOWN
				// Handle  the tabs to make them cycle around the data entry fields
				if ( e . Key == Key . Tab && e . Source == cdate )
				{
					e . Handled = true;
					odate . Focus ( );
					return;
				}
				else if ( e . Key == Key . Tab && e . Source == Custno )
				{
					e . Handled = true;
					cdate . Focus ( );
					//					Debug . WriteLine ( $"KEYDOWN Shift turned OFF" );
					return;
				}
				else if ( e . Key == Key . F11 )
				{
					var pos = Mouse . GetPosition ( this);
					Utils . Grab_Object ( sender , pos );
					if ( Utils . ControlsHitList . Count == 0 )
						return;
					Utils . Grabscreen ( this, Utils . ControlsHitList [ 0 ] . VisualHit , null , sender as Control );
				}
			}
		}

		#endregion KEYHANDLER for EDIT fields


		#region HANDLERS for linkage checkboxes, inluding Thread montior

		static bool IsLinkActive ( bool ParentLinkTo )
		{
			return Flags . SqlBankViewer != null && ParentLinkTo == false;
		}
		static bool IsMultiLinkActive ( bool MultiParentLinkTo )
		{
			if ( Flags . SqlMultiViewer == null )
				return false;
			else
				return true;
		}

		private void LinkToMulti_Click ( object sender , RoutedEventArgs e )
		{
			bool reslt = false;

			if ( IsMultiLinkActive ( reslt ) == false )
			{
				LinkToMulti . IsEnabled = false;
				LinkToMulti . IsChecked = false;
				MultiParentViewer = null;
				LinktoMultiParent = false;
			}
			else
			{
				LinktoMultiParent = !LinktoMultiParent;
				if ( LinktoMultiParent )
				{
					LinkToMulti . IsChecked = true;
					LinktoMultiParent = true;
				}
				else
				{
					LinkToMulti . IsChecked = false;
					LinktoMultiParent = false;
				}
			}
		}
		/// <summary>
		/// Runs as a thread to monitor SqlDbviewer & Multiviewer availabilty
		/// and resets checkboxes as necessary  - thread delay is 3.5  seconds
		/// </summary>
		private void checkLinkages ( )
		{
			while ( true )
			{
				int AllLinks = 0;
				Thread . Sleep ( 3500 );

				bool reslt = LinktoParent;
				if ( LinkToAllRecords == false && LinkToAllRecords == false )
				{
					// Link to  ALL is UNCHECKED, so make sure Parent link is ENABLED
					Application . Current . Dispatcher . Invoke ( ( ) =>
					{
						ResetLinkages ( "LINKTOPARENT" , true );
					} );
				}
				else
				{
					// Link to  ALL is CHECKED, so make sure Parent link is DISABLED and Unchecked
					Application . Current . Dispatcher . Invoke ( ( ) =>
					{
						ResetLinkages ( "LINKTOPARENT" , false );
					} );

				}
				if ( IsMultiLinkActive ( reslt ) == false )
				{
					Application . Current . Dispatcher . Invoke ( ( ) =>
					{
						ResetLinkages ( "MULTILINKTOPARENT" , false );
					} );
				}
				else
				{
					AllLinks++;
					Application . Current . Dispatcher . Invoke ( ( ) =>
					{
						ResetLinkages ( "MULTILINKTOPARENT" , true );
					} );
				}
				//				if ( AllLinks >= 1 )
				Application . Current . Dispatcher . Invoke ( ( ) =>
				{
					ResetLinkages ( "ALLLINKS" , Flags . LinkviewerRecords );
				} );
				//else
				//	Application . Current . Dispatcher . Invoke ( ( ) =>
				//	{
				//		ResetLinkages ( "ALLLINKS", false );
				//	} );

			}
		}
		private void ResetLinkages ( string linktype , bool value )
		{
			if ( linktype == "LINKTOPARENT" )
			{
				LinkToParent . IsEnabled = value;
				LinktoParent = value;
				if ( value )
					SqlParentViewer = Flags . SqlBankViewer;
				else
				{
					LinktoParent = false;
					LinkToParent . IsEnabled = false;
					//SqlParentViewer = null;
				}
			}
			if ( linktype == "MULTILINKTOPARENT" )
			{
				if ( value )
				{
					LinkToMulti . IsEnabled = value;
					MultiParentViewer = Flags . SqlMultiViewer;
				}
				else
				{
					LinkToMulti . IsEnabled = false;
					LinkToMulti . IsChecked = false;
					MultiParentViewer = null;
					LinktoMultiParent = false;
				}
			}
			if ( linktype == "ALLLINKS" )
				LinkRecords . IsChecked = value;

			#endregion HANDLERS for linkage checkboxes, inluding Thread montior

		}
		private void Window_MouseDown ( object sender , MouseButtonEventArgs e )
		{

		}

		private void Minimize_click ( object sender , RoutedEventArgs e )
		{
			this . WindowState = WindowState . Normal;
		}


		private void ContextShowJson_Click ( object sender , RoutedEventArgs e )
		{
			//============================================//
			//MENU ITEM 'Read and display JSON File'
			//============================================//
			string Output = "";
			this . Refresh ( );
			////We need to save current Collectionview as a Json (binary) data to disk
			//// this is the best way to save persistent data in Json format
			////using tmp folder for interim file that we will then display
			BankAccountViewModel bvm = this . BankGrid . SelectedItem as BankAccountViewModel;
			Output = JsonSupport . CreateShowJsonText ( true , "BANKACCOUNT" , bvm , "BankAccountViewModel" );
			MessageBox . Show ( Output , "Currently selected record in JSON format" , MessageBoxButton . OK , MessageBoxImage . Information , MessageBoxResult . OK );
		}

		private void ContextEdit_Click ( object sender , RoutedEventArgs e )
		{
			// handle flags to let us know WE have triggered the selectedIndex change
			//MainWindow . DgControl . SelectionChangeInitiator = 2; // tells us it is a EditDb initiated the record change
			BankAccountViewModel bvm = new BankAccountViewModel ( );
			int currsel = 0;
			DataGridRow RowData = new DataGridRow ( );
			bvm = this . BankGrid . SelectedItem as BankAccountViewModel;
			currsel = this . BankGrid . SelectedIndex;
			//int row = DataGridSupport . GetDataGridRowFromTree ( e, out RowData );
			//if ( row == -1 ) row = 0;
			RowInfoPopup rip = new RowInfoPopup ( "BANKACCOUNT", BankGrid );
			rip . Topmost = true;
			rip . DataContext = RowData;
			rip . BringIntoView ( );
			rip . Focus ( );
			rip . ShowDialog ( );

			//If data has been changed, update everywhere
			// Update the row on return in case it has been changed
			if ( rip . IsDirty )
			{
				this . BankGrid . ItemsSource = null;
				this . BankGrid . Items . Clear ( );
				//Reload our data base data, it will be loaded when we are notified it is ready via the BankDataLoaded Event
				Flags . SqlBankActive = true;
				//				BankCollection . LoadBank ( BankView , "BANKDBVIEW" , 1 , true );
				DapperSupport . GetBankObsCollection ( collection: BankAccounts ,
						wantSort: false ,
							Notify: true );
				BankView = CollectionViewSource . GetDefaultView ( BankAccounts ) as CollectionView;
				//				this . BankGrid . ItemsSource = BankView;
				// Notify everyone else of the data change
				EventControl . TriggerViewerDataUpdated ( BankView ,
					new LoadedEventArgs
					{
						CallerType = "BANKBVIEW" ,
						CallerDb = "BANKACCOUNT" ,
						DataSource = BankView ,
						SenderGuid = this . Tag . ToString ( ) ,
						RowCount = this . BankGrid . SelectedIndex
					} );
				EventControl . TriggerGlobalDataChanged ( this , new GlobalEventArgs
				{
					CallerType = "BANKDBVIEW" ,
					AccountType = "BANKACCOUNT" ,
					SenderGuid = this . Tag?.ToString ( )
				} );
			}
			//else
			//	this . BankGrid . SelectedItem = RowData . Item;

			//// This sets up the selected Index/Item and scrollintoview in one easy FUNC function call (GridInitialSetup is  the FUNC name)
			//this . BankGrid . SelectedIndex = currsel;
			//Count . Text = $"{this . BankGrid . SelectedIndex} / { this . BankGrid . Items . Count . ToString ( )}";
			// This is essential to get selection activated again
			BankGrid . Focus ( );
			Utils . SetGridRowSelectionOn ( BankGrid , BankGrid . SelectedIndex );
			this . BankGrid . Focus ( );
		}

		private async void ContextSave_Click ( object sender , RoutedEventArgs e )
		{
			//============================================//
			//MENU ITEM 'Save current Grid Db data as JSON File'
			//============================================//
			object DbData = new object ( );
			string resultString = "", path = "";
			string jsonresult = "";
			// Get default text files viewer application from App resources
			string program = ( string ) Properties . Settings . Default [ "DefaultTextviewer" ];

			//HOW to save current Collectionview as a Json (binary) data from disk
			// this is the best way to save persistent data in Json format
			//Save data (XXXXViewModel[]) as binary to disk file
			path = @"C:\\Users\\Ianch\\Documents\\BankCollectiondata.json";
			jsonresult = JsonConvert . SerializeObject ( BankView );
			JsonSupport . JsonSerialize ( jsonresult , path );
			MessageBox . Show ( $"The data from this Database has been saved\nfor you in 'Json' format successfully ...\n\nFile is : {path}" , "Data Persistence System" );
		}

		private void ContextDisplayJsonData_Click ( object sender , RoutedEventArgs e )
		{
			//============================================//
			//MENU ITEM 'Read and display JSON File'
			//============================================//
			JsonSupport . CreateShowJsonText ( false , "BANKACCOUNT" , BankView );

		}

		private void ContextSettings_Click ( object sender , RoutedEventArgs e )
		{
			Setup setup = new Setup ( );
			setup . Show ( );
			setup . BringIntoView ( );
			setup . Topmost = true;
			this . Focus ( );
		}

		private void ContextClose_Click ( object sender , RoutedEventArgs e )
		{
			Close ( );
		}

		public void changesize_Click2 ( object sender , RoutedEventArgs e )
		{
			Thickness t = new Thickness ( );

			if ( BankGrid . RowHeight == 32 )
			{
				BankGrid . RowHeight = 25;
				SizeChangeMenuItem2 . Header = "Larger Font";
				SizeChangeMenuItem2 . FontSize = 16;
				t . Top = 0;
				t . Bottom = 0;
				SizeChangeMenuItem2 . Margin = t;
				Brush br = Utils . GetDictionaryBrush ( "White0" );
				SizeChangeMenuItem2 . Foreground = br;

				string path = @"/Views/magnify plus red.png";
				FontsizeIcon2 . Source = new BitmapImage ( new Uri ( path , UriKind . RelativeOrAbsolute ) );
				t . Top = 0;
				t . Bottom = 0;
				FontsizeIcon2 . Margin = t;
			}
			else
			{
				BankGrid . RowHeight = 32;
				SizeChangeMenuItem2 . Header = "Smaller Font";
				SizeChangeMenuItem2 . FontSize = 10;
				t . Top = ( double ) 8;
				SizeChangeMenuItem2 . Margin = t;
				Brush br = Utils . GetDictionaryBrush ( "White0" );
				SizeChangeMenuItem2 . Foreground = br;

				string path = @"/Views/magnify minus red.png";
				FontsizeIcon2 . Source = new BitmapImage ( new Uri ( path , UriKind . RelativeOrAbsolute ) );
				t . Top = -5;
				//				t . Bottom = 5;
				//				t . Right = 5;
				FontsizeIcon2 . Margin = t;
				//				FontsizeIcon . Width = 30;
			}
		}
		private void sysColors_Click ( object sender , RoutedEventArgs e )
		{
			SysColors sc = new SysColors ( );
			sc . Show ( );
		}
		//#####################
		#region Custom Commands

		#region ApplicationsCommands.New Command handlers

		// Using a built in ApplicationComands.xxxx Command todo whatever we want
		private void CommandNew_CanExecute ( object sender , CanExecuteRoutedEventArgs e )
		{
			e . CanExecute = true;
		}
		private void Command_New ( object sender , ExecutedRoutedEventArgs e )
		{
			//handle the actual command code here
			BankDbView bdv = sender as BankDbView;
			int x = bdv . BankGrid . SelectedIndex;
			MessageBox . Show ( $"New Command has been run... Yeaaaahh !!!\nIndex is {x}" );
		}
		#endregion ApplicationsCommands.New Command handlers

		#region ApplicationsCommands.Cut Command handlers


		private void CommandCopy_CanExecute ( object sender , CanExecuteRoutedEventArgs e )
		{
			e . CanExecute = true;
		}
		private void Command_Copy ( object sender , ExecutedRoutedEventArgs e )
		{
		}
		private void CommandPaste_CanExecute ( object sender , CanExecuteRoutedEventArgs e )
		{
			e . CanExecute = true;
		}
		private void Command_Paste ( object sender , ExecutedRoutedEventArgs e )
		{
		}
		// This is a testing Command, implementing the Applications.Cut Built in command
		// as a shell to let us do whatever we want to do do inside the method
		// the e contains a CommandParameter OBJECT that can be passed by  the calling function
		private void CommandCut_CanExecute ( object sender , CanExecuteRoutedEventArgs e )
		{
			e . CanExecute = true;
		}
		private void Command_Cut ( object sender , ExecutedRoutedEventArgs e )
		{
			BankDbView bdv = sender as BankDbView;
			BankAccountViewModel bvm = new BankAccountViewModel ( );
			bvm = BankGrid . SelectedItem as BankAccountViewModel;
			int x = bdv . BankGrid . SelectedIndex;
			MessageBox . Show ( $"Cut Command has been run... Yeaaaahh !!!\nCustNo is {bvm . CustNo}" );
		}
		#endregion ApplicationsCommands.Cut Command handlers

		#region ApplicationsCommands.Paste Command handlers

		private void Paste_Executed ( object sender , ExecutedRoutedEventArgs e )
		{
			MessageBox . Show ( "Command has been run... Yeaaaahh !!!" );
		}
		private void Paste_CanExecute ( object sender , CanExecuteRoutedEventArgs e )
		{
			e . CanExecute = true;
		}

		#endregion ApplicationsCommands.Paste Command handlers
		//^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
		#region MyCommands.Exit CUSTOM Command handler
		/// <summary>
		/// CUSTOM Command implementation
		/// the Command itself is declared in MyCommands.CS
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CommandExit_CanExecute ( object sender , CanExecuteRoutedEventArgs e )
		{
			e . CanExecute = true;
		}
		private void Command_Exit ( object sender , ExecutedRoutedEventArgs e )
		{
			Window win = sender as Window;
			win . Close ( );
			MessageBox . Show ( "Custom Exit Command has been run by  a Command... Yeaaaahh !!!" );
		}
		#endregion MyCommands.ExitCommand handlers

		#region MyCommands.ShowMessage CUSTOM Command handler
		/// <summary>
		/// CUSTOM Command implementation
		/// the Command itself is declared in MyCommands.CS
		/// Declaration in <CommandBindings> is:
		/// 	<CommandBinding Command="self:MyCommands.ShowMessage" 
		///	CanExecute="ShowMessage_CanExecute"
		///	Executed="Show_Message"/>
		///
		/// Menu is :
		/// 			
		/// <MenuItem
		/// Command="self:MyCommands.ShowMessage"
		/// CommandParameter="Nuffink"
		/// Width="70"
		/// Foreground="{StaticResource White1}"
		/// Header="Show Cmd" />
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ShowMessage_CanExecute ( object sender , CanExecuteRoutedEventArgs e )
		{
			e . CanExecute = true;
		}
		/// NB
		/// The Actual method to be performed MUST be in the CodeBehind, it CANNOT be in a seperate file
		private void Show_Message ( object sender , ExecutedRoutedEventArgs e )
		{
			MessageBox . Show ( "Custom Show Message() Command has been run... Yeaaaahh !!!" );
		}

		#endregion MyCommands.ShowMessage CUSTOM handlers		

		#region Hello-Bye Hot key test

		bool mHelloSaid = true;
		private void Hello_CanExecute ( object sender , CanExecuteRoutedEventArgs e )
		{
			e . CanExecute = true;
		}
		private void Hello_Executed ( object sender , RoutedEventArgs e )
		{
			if ( ( string ) HelloItem . Header == "Bye..." )
			{
				//MessageBox .Show ( "Bye" );
				HelloItem . Header = "Hello !";
			}
			else
			{
				//MessageBox .Show ( "Hello!!" );
				HelloItem . Header = "Bye...";
			}
		}
		private void Bye_CanExecute ( object sender , CanExecuteRoutedEventArgs e )
		{
			e . CanExecute = mHelloSaid;
		}
		void Bye_Executed ( object sender , ExecutedRoutedEventArgs e )
		{
			//			mHelloSaid = false;
			MessageBox . Show ( "Bye!!" );
			HelloItem . Header = "Bye...";
		}
		#endregion Hello-Bye Hot key test

		#region Close menu option Command
		private void Close_CanExecute ( object sender , CanExecuteRoutedEventArgs e )
		{
			if ( !IsDirty )
				e . CanExecute = true;
		}

		private void CloseWin ( object sender , ExecutedRoutedEventArgs e )
		{
			Window uie = sender as Window;
			MessageBox . Show ( "Custom Command is going to close the current Window" );
			uie . Close ( );
		}
		#endregion Close menu option Command

		private void EventControl_BankDataLoaded ( object sender , LoadedEventArgs e )
		{
			bool isloaded = false;
			// Event handler for BankDataLoaded
			if ( e . DataSource == null )
				return;
			Flags . SqlBankActive = false;
			// ONLY proceeed if we triggered the new data request
			if ( e . CallerDb != "BANKDBVIEW" )
				return;

			this . BankGrid . ItemsSource = null;
			this . BankGrid . Items . Clear ( );
			LoadingDbData = true;

			BankAccounts = e . DataSource as ObservableCollection<BankAccountViewModel>;
			//Assign our view to the recently loaded data
			BankView = ( CollectionView ) CollectionViewSource . GetDefaultView ( BankAccounts );
			// Lets each sort get set  up in the view without updating the window between sort orders
			//Add  this view to global View list
			BankAccountViewModel . BankCollectionView = BankView;
			// Set our grids items source
			this . BankGrid . ItemsSource = BankView;
			// Add our sort's
			if ( Flags . UseSharedView == true )
			{
				BankView . SortDescriptions . Add ( new SortDescription ( "AcType" , ListSortDirection . Ascending ) );
				BankView . SortDescriptions . Add ( new SortDescription ( "Balance" , ListSortDirection . Descending ) );
				BankView . Refresh ( );
			}

			this . BankGrid . SelectedIndex = bindex;
			this . BankGrid . SelectedItem = bindex;
			this . BankGrid . CurrentItem = bindex;
			this . BankGrid . UpdateLayout ( );
			this . BankGrid . Refresh ( );
			Utils . SetUpGridSelection ( BankGrid , bindex );
			Mouse . OverrideCursor = Cursors . Arrow;
			Thread . Sleep ( 150 );
			DataFields . Refresh ( );
			Count . Text = $"{this . BankGrid . SelectedIndex} / { this . BankGrid . Items . Count . ToString ( )}";
			IsDirty = false;
			this . BankGrid . SelectedItem = bindex;
			this . BankGrid . CurrentItem = bindex;
			this . BankGrid . Focus ( );
			DataFields . Refresh ( );
			Console . WriteLine ( $"BANKDBVIEW: Bank Data fully loaded from Sql in {timer . ElapsedMilliseconds} milliseconds\n" );
		}


		#region CollectionView handlers
		private async void EventControl_ViewSharingChanged ( object sender , NotifyAllViewersOfViewSharingStatus e )
		{
			if ( timer . IsRunning == false )
			{
				timer . Start ( );
				counter++;
			}
			else if ( counter > 1 )
			{
				timer . Stop ( );
				counter = 0;
				return;
			}
			if ( e . Sender != "BANKDBVIEW" )
			{
				ToggleViewStatus . IsChecked = e . IsShared;
				BankGrid . ItemsSource = null;
				Console . WriteLine ( $"BANKDBVIEW : EventControl ViewSharing loading Db via SQL, counter = {counter}" );
				//				await BankCollection . LoadBank ( BankView , "BANKDBVIEW" , 99 , true );
				DapperSupport . GetBankObsCollection ( collection: BankAccounts ,
						wantSort: false ,
							Notify: true ,
							Caller: "BANKDBVIEW" );
				BankView = CollectionViewSource . GetDefaultView ( BankAccounts ) as CollectionView;
			}
		}

		private async void ToggleViewStatus_Checked ( object sender , RoutedEventArgs e )
		{
			Console . WriteLine ( $"BANKDBVIEW: View Toggle CHECKED called" );
			if ( Flags . UseSharedView == false )
			{
				EventControl . TriggerViewSharingChanged ( sender ,
					  new NotifyAllViewersOfViewSharingStatus
					  {
						  IsShared = true ,
						  Sender = "BANKDBVIEW"

					  } );

				BankGrid . ItemsSource = null;
				//				await BankCollection . LoadBank ( BankView , "BANKDBVIEW" , 99 , true );
				DapperSupport . GetBankObsCollection ( collection: BankAccounts ,
						wantSort: false ,
							Notify: true );
				BankView = CollectionViewSource . GetDefaultView ( BankAccounts ) as CollectionView;
				Flags . UseSharedView = true;
			}
		}

		private async void ToggleViewStatus_Unchecked ( object sender , RoutedEventArgs e )
		{
			Console . WriteLine ( $"BANKDBVIEW: View Toggle UNCHECKED called" );
			if ( Flags . UseSharedView == true )
			{

				EventControl . TriggerViewSharingChanged ( sender ,
					  new NotifyAllViewersOfViewSharingStatus
					  {
						  IsShared = false ,
						  Sender = "BANKDBVIEW"

					  } );

				BankGrid . ItemsSource = null;
				//				await BankCollection . LoadBank ( BankView , "BANKDBVIEW" , 99 , true );
				DapperSupport . GetBankObsCollection ( collection: BankAccounts ,
						wantSort: false ,
							Notify: true );
				BankView = CollectionViewSource . GetDefaultView ( BankAccounts ) as CollectionView;
				Flags . UseSharedView = false;
			}
		}

		private async void Reload_Click ( object sender , RoutedEventArgs e )
		{
			BankGrid . ItemsSource = null;
			//			await BankCollection . LoadBank ( BankView , "BANKDBVIEW" , 99 , true );
			DapperSupport . GetBankObsCollection ( collection: BankAccounts ,
					wantSort: false ,
						Notify: true ,
						Caller:"BANKDBVIEW");
			BankView = CollectionViewSource . GetDefaultView ( BankAccounts ) as CollectionView;
		}

		private void BankGrid_Loaded ( object sender , RoutedEventArgs e )
		{
			int counter = 0;
			foreach ( var item in BankGrid . Columns )
			{
				DGBankColumnsCollection [ counter++ ] = item;
			}
			DataGridSupport . SortBankColumns ( BankGrid , DGBankColumnsCollection );
		}

		private void Columns_SelectionChanged ( object sender , SelectionChangedEventArgs e )
		{
			ListBox lb = sender as ListBox;
			var  Content = lb . SelectedItem;
			//ListBoxItem lbi = Content as  ListBoxItem;
			var selection = int.Parse(Content.ToString());
			DataGridSupport . SortBankColumns ( BankGrid , DGBankColumnsCollection , selection );
			BankGrid . Refresh ( );
		}

		private void bankdbview_PreviewKeyDown ( object sender , KeyEventArgs e )
		{

		}

		private void ChecksMouseMove ( object sender , MouseEventArgs e )
		{
			e . Handled = true;
			if ( e . RightButton == MouseButtonState . Pressed )
				return;
		}
		#endregion CollectionView handlers

		#endregion Commands
		//#####################

	}
}
