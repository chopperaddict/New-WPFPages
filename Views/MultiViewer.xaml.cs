﻿using System;
using System . Collections . Generic;
using System . Diagnostics;
using System . Linq;
using System . Net . Mail;
using System . Threading;
using System . Threading . Tasks;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Data;
using System . Windows . Input;
using System . Windows . Media;
using Newtonsoft . Json . Linq;
using Newtonsoft . Json;
using WPFPages . ViewModels;
using DataGrid = System . Windows . Controls . DataGrid;
using System . IO;
using System . Xml . Linq;
using System . Collections . ObjectModel;
using System . Windows . Documents;

using System . Windows . Media . Animation;

namespace WPFPages . Views
{
	/// <summary>
	/// Interaction logic for MultiViewer.xaml
	/// </summary>
	public partial class MultiViewer : Window
	{
		public static ObservableCollection<BankAccountViewModel >SqlBankAccounts= new ObservableCollection<BankAccountViewModel >();
		public static ObservableCollection<CustomerViewModel>SqlCustAccounts= new ObservableCollection<CustomerViewModel>();
		public static ObservableCollection<DetailsViewModel>SqlDetAccounts= new ObservableCollection<DetailsViewModel>();

//		private BankCollection SqlBankAccounts = null;
		//		public Stopwatch stopwatch1 = new Stopwatch ( );
		//		public Stopwatch stopwatch2 = new Stopwatch ( );
		//		public Stopwatch stopwatch3 = new Stopwatch ( );

		private static readonly DataGridColumn dataGridColumn   ;
		private DataGridColumn[] DGBankColumnsCollection = {dataGridColumn,dataGridColumn,dataGridColumn,dataGridColumn,
			dataGridColumn,dataGridColumn,dataGridColumn,dataGridColumn };
		private DataGridColumn[] DGCustColumnsCollection
			= {dataGridColumn,dataGridColumn,dataGridColumn,dataGridColumn,dataGridColumn,dataGridColumn,dataGridColumn,dataGridColumn,dataGridColumn,
			dataGridColumn,dataGridColumn,dataGridColumn,dataGridColumn,dataGridColumn ,dataGridColumn };
		private DataGridColumn[] DGDetailsColumnsCollection= {dataGridColumn,dataGridColumn,dataGridColumn,dataGridColumn,
			dataGridColumn,dataGridColumn,dataGridColumn,dataGridColumn };
		public Point _startPoint { get; set; }
		// These MAINTAIN setting values across instances !!!
		public static int bindex { get; set; }
		public static int cindex { get; set; }
		public static int dindex { get; set; }
		public int CurrentSelection { get; set; }
		public bool key1 { get; set; }
		public bool GridsLinked { get; set; }
		public bool ScrollBarMouseMove { get; set; }
		public static RoutedCommand CloseApp = new RoutedCommand ( );

		#region DECLARATIONS

		public string CurrentDb { get; set; }
		private bool inprogress { get; set; }
		private bool Triggered { get; set; }
		private bool ReloadingData { get; set; }
		private bool IsEditing { get; set; }
		public bool isLoading { get; set; }
		public bool IsDirty { get; set; }
		public bool LoadingDbData { get; set; }

		List<string> tmp3 = new List<string> ( );

		public bool IsLeftButtonDown { get; set; }
		#endregion DECLARATIONS

		#region STARTUP/CLOSE

		public MultiViewer ( )
		{
			InitializeComponent ( );
			//CommandBinding myCommandBinding = new CommandBinding ( MyCommands.myCommand, MyCommands . ExecutedmyCommand, MyCommands . CanExecutemyCommand );
			// attach CommandBinding to root element
			//			this . CommandBindings . Add ( myCommandBinding );
			this . Show ( );

			//Identify individual windows for update protection
			this . Tag = ( Guid ) Guid . NewGuid ( );

			this . WaitMessage . Visibility = Visibility . Visible;
			this . Refresh ( );
			tmp3 . Add ( $"Please wait, The system is loading the data from 3 seperate SQL Databases..." );
			//			tmp3 . Add ( $"This process can take a few soconds or so." );
			// Show our wait message initially
			WaitMessage . ItemsSource = tmp3;
			WaitMessage . SelectedIndex = 0;
			WaitMessage . SelectedItem = 0;
			WaitMessage . CurrentItem = 1;
			ColumnSelection . Items . Add ( 0 );
			ColumnSelection . Items . Add ( 1 );
			ColumnSelection . Items . Add ( 2 );

			WaitMessage . Refresh ( );
			this . Refresh ( );
		}

		private async void Window_Loaded ( object sender , RoutedEventArgs e )
		{
			DataGridColumn[] dgc;
			int counter = 0;

			Mouse . OverrideCursor = Cursors . Wait;
			inprogress = true;

			foreach ( var item in BankGrid . Columns )
			{
				DGBankColumnsCollection [ counter++ ] = item;
			}
			DataGridSupport . SortBankColumns ( BankGrid , DGBankColumnsCollection );
			counter = 0;
			foreach ( var item in CustomerGrid . Columns )
			{
				DGCustColumnsCollection [ counter++ ] = item;
			}
			DataGridSupport . SortCustomerColumns ( CustomerGrid , DGCustColumnsCollection );
			counter = 0;
			foreach ( var item in DetailsGrid . Columns )
			{
				DGDetailsColumnsCollection [ counter++ ] = item;
			}
			DataGridSupport . SortDetailsColumns ( DetailsGrid , DGDetailsColumnsCollection );

			//BankGrid . Columns
			string ndx = ( string ) Properties . Settings . Default [ "Multi_bindex" ];
			bindex = int . Parse ( ndx );
			ndx = ( string ) Properties . Settings . Default [ "Multi_cindex" ];
			cindex = int . Parse ( ndx );
			ndx = ( string ) Properties . Settings . Default [ "Multi_dindex" ];
			dindex = int . Parse ( ndx );
			BankGrid . SelectedIndex = bindex < 0 ? 0 : bindex;
			CustomerGrid . SelectedIndex = cindex < 0 ? 0 : bindex;
			DetailsGrid . SelectedIndex = dindex < 0 ? 0 : bindex;
			SubscribeToEvents ( );
			this . Show ( );
			WaitMessage . UpdateLayout ( );
			await LoadAllData ( );

			Flags . SqlMultiViewer = this;
			Flags . SqlMultiViewer = this;
			// Setup global pointers to our data grids
			Flags . SqlBankGrid = this . BankGrid;
			Flags . SqlCustGrid = this . CustomerGrid;
			Flags . SqlDetGrid = this . DetailsGrid;

			LinkGrids . IsChecked = false;
			GridsLinked = false;
			if ( Flags . LinkviewerRecords )
			{
				LinkRecords . IsChecked = true;
				GridsLinked = true;
			}
			// Set window to TOPMOST
			OntopChkbox . IsChecked = true;
			this . Topmost = true;
			inprogress = false;
			Mouse . OverrideCursor = Cursors . Arrow;
			isLoading = false;
		}
		private void ChecksMouseMove ( object sender , MouseEventArgs e )
		{
			e . Handled = true;
			if ( e . RightButton == MouseButtonState . Pressed )
				return;
		}

		private void Window_Closing ( object sender , System . ComponentModel . CancelEventArgs e )
		{
			// Unsubscribe from Bank data change event notificatoin
			// Main update notification handler
			EventControl . ViewerDataUpdated -= EventControl_SqlViewerDataUpdated;
			EventControl . EditDbDataUpdated -= EventControl_DataUpdated;
			EventControl . MultiViewerDataUpdated -= EventControl_ViewerDataUpdated;

			// Event triggers when a Specific Db viewer (BankDbViewer etc) updates the data
			EventControl . BankDataLoaded -= EventControl_BankDataLoaded;
			EventControl . CustDataLoaded -= EventControl_CustDataLoaded;
			EventControl . DetDataLoaded -= EventControl_DetDataLoaded;
			EventControl . GlobalDataChanged -= EventControl_GlobalDataChanged;

			// Listen ofr index changes
			EventControl . ViewerIndexChanged -= EventControl_ViewerIndexChanged;
			EventControl . EditIndexChanged -= EventControl_ViewerIndexChanged;

			Utils . SaveProperty ( "Multi_bindex" , bindex . ToString ( ) );
			Utils . SaveProperty ( "Multi_cindex" , cindex . ToString ( ) );
			Utils . SaveProperty ( "Multi_dindex" , dindex . ToString ( ) );

			//// We must also clear our "loaded" columns, or else it stops working
			ObservableCollection<DataGridColumn> dgc = CustomerGrid.Columns;
			dgc . Clear ( );
			ObservableCollection<DataGridColumn> dgc2 = BankGrid.Columns;
			dgc2 . Clear ( );
			ObservableCollection<DataGridColumn> dgc3 = DetailsGrid.Columns;
			dgc3 . Clear ( );

			// Clear databases
			SqlBankAccounts?.Clear ( );
			//			SqlCustAccounts?.Clear ( );
			SqlDetAccounts?.Clear ( );
			Flags . SqlMultiViewer = null;
		}

		#region WATCH FOR Db CLOSURE - not used right now, but works well' ish
		public async void StartDataWatcher ( )
		{
			Task t1 = Task . Run ( WatchForDbLoss );
		}
		public async void WatchForDbLoss ( )
		{
			while ( true )
			{
				Thread . Sleep ( 1500 );

				if ( isLoading )
					continue;
				if ( this . BankGrid . Items . Count == 0 )
				{
					Application . Current . Dispatcher . Invoke ( ( ) =>
					{
						Mouse . OverrideCursor = Cursors . Wait;
						ReloadBankDb ( );
					} );
					Thread . Sleep ( 5000 );
				}
				else if ( this . CustomerGrid . Items . Count == 0 )
				{
					Application . Current . Dispatcher . Invoke ( ( ) =>
					{
						Mouse . OverrideCursor = Cursors . Wait;
						ReloadCustDb ( );
					} );
					Thread . Sleep ( 5000 );
				}
				else if ( this . DetailsGrid . Items . Count == 0 )
				{
					Application . Current . Dispatcher . Invoke ( ( ) =>
					{
						Mouse . OverrideCursor = Cursors . Wait;
						ReloadDetDb ( );
					} );
					Thread . Sleep ( 5000 );
				}
			}
			return;
		}
		private Task ReloadBankDb ( )
		{
			Task t1 = null;
			isLoading = true;
			Mouse . OverrideCursor = Cursors . Wait;
			Application . Current . Dispatcher . Invoke ( ( ) =>
			{
				Flags . SqlBankActive = true;
				//				BankCollection . LoadBank ( SqlBankAccounts , "MULTIVIEWER" , 3 , true );
				if ( Flags . USECOPYDATA )
				{
					DapperSupport . GetBankObsCollection ( collection: SqlBankAccounts ,
					DbNameToLoad: Flags.COPYBANKDATANAME,
					wantSort: true ,
					Notify: true ,
					Caller: "MULTIVIEWER" );
				}
				else
				{
					DapperSupport . GetBankObsCollection ( collection: SqlBankAccounts ,
						wantSort: true ,
						Notify: true ,
						Caller: "MULTIVIEWER" );
				}
			} );
			//			Thread . Sleep ( 5000 );
			return t1;
		}
		private Task ReloadCustDb ( )
		{
			Task t1 = null;
			Application . Current . Dispatcher . Invoke ( ( ) =>
			{
				Mouse . OverrideCursor = Cursors . Wait;
				Flags . SqlCustActive = true;
				//				AllCustomers . LoadCust ( SqlCustAccounts , "MULTIVIEWER" , 3 , true );
				if ( Flags . USECOPYDATA )
				{
					SqlCustAccounts = DapperSupport . GetCustObsCollection ( collection: SqlCustAccounts ,
					DbNameToLoad: Flags . COPYCUSTDATANAME ,
					Caller: "MULTIVIEWER" ,
					Notify: true );
				}
				else
				{
					SqlCustAccounts = DapperSupport . GetCustObsCollection ( collection: SqlCustAccounts ,
						Caller: "MULTIVIEWER" ,
						Notify: true );
				}
			} );
			//			Thread . Sleep ( 5000 );
			return t1;
		}
		private Task ReloadDetDb ( )
		{
			Task t1 = null;
			Application . Current . Dispatcher . Invoke ( ( ) =>
			{
				Mouse . OverrideCursor = Cursors . Wait;
				Flags . SqlDetActive = true;
				//				DetailCollection . LoadDet ( "MULTIVIEWER" , 3 , true );
				if ( Flags . USECOPYDATA )
				{
					DapperSupport . GetDetailsObsCollection ( SqlDetAccounts ,
					DbNameToLoad: Flags . COPYDETDATANAME ,
					wantSort: true ,
					wantDictionary: false ,
					Notify: true ,
					Caller: "MULTIVIEWER" );
				}
				else
				{
					DapperSupport . GetDetailsObsCollection ( SqlDetAccounts ,
					wantSort: true ,
					wantDictionary: false ,
					Notify: true ,
					Caller: "MULTIVIEWER" );
				}
			} );
			//			Thread . Sleep ( 5000 );
			return t1;
		}

		#endregion WATCH FOR Db CLOSURE
		#region Post Data Reloaded event handlers - ALL WORKING WELL 26/5/21

		/// <summary>
		/// Handles rsetting the index after Bank data has been reoloaded
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void EventControl_BankDataLoaded ( object sender , LoadedEventArgs e )
		{
			if ( e . DataSource == null )
				return; //|| this . BankGrid . Items . Count > 0 ) return;
					  //			if ( e . CallerDb != "MULTIVIEWER" )
					  //				return;
			Flags . SqlBankActive = false;
			//			Debug . WriteLine ($"\n*** Loading Bank data in BankDbView after BankDataLoaded trigger\n" );
			// ONLY proceed if we triggered the new data request
			//			if ( e . CallerDb != "MULTIVIEWER" ) return;
			Debug . WriteLine ( $"\n*** Loading Bank data in MultiViewer after BankDataLoaded trigger\n" );

			this . BankGrid . ItemsSource = null;

			//			stopwatch1 . Stop ( );
			Debug . WriteLine ( $"MULTIVIEWER : Bank Data fully loaded : " );
			LoadingDbData = true;
			BankGrid . ItemsSource = null;
			BankGrid . Items . Clear ( );
			SqlBankAccounts?.Clear ( );
			// This is how to convert to  CollectionView
			SqlBankAccounts = e . DataSource as ObservableCollection<BankAccountViewModel>;
			this . BankGrid . ItemsSource = CollectionViewSource . GetDefaultView ( SqlBankAccounts );

			this . BankGrid . Refresh ( );
			BankGrid . SelectedIndex = bindex;
			BankGrid . SelectedItem = bindex;
			BankGrid . Refresh ( );
			BankGrid . UpdateLayout ( );
			Utils . SetUpGridSelection ( this . BankGrid , bindex );
			BankCount . Text = $"{this . BankGrid . SelectedIndex} / {this . BankGrid . Items . Count}";
			Mouse . OverrideCursor = Cursors . Arrow;
			isLoading = false;
			IsDirty = false;
			// Let em see our message
			ClearWaitMessage ( );

		}

		private void ClearWaitMessage ( )
		{
			WaitMessage . Refresh ( );
			//Thread . Sleep ( 500 );
			this . BankGrid . Visibility = Visibility . Visible;
			this . CustomerGrid . Visibility = Visibility . Visible;
			this . DetailsGrid . Visibility = Visibility . Visible;
			WaitMessage . Visibility = Visibility . Collapsed;
		}

		private async void EventControl_CustDataLoaded ( object sender , LoadedEventArgs e )
		/// <summary>
		/// Handles rsetting the index after Customer data has been reoloaded
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		{
			if ( e . DataSource == null )
				return;
			// ONLY proceeed if we triggered the new data request
			Flags . SqlCustActive = false;
			if ( e . CallerDb != "MULTIVIEWER" )
				return;

			//			stopwatch2 . Stop ( );
			Debug . WriteLine ( $"MULTIVIEWER : Customer Data fully loaded : " );
			LoadingDbData = true;
			CustomerGrid . ItemsSource = null;
			CustomerGrid . Items . Clear ( );
			SqlCustAccounts?.Clear ( );
			// This is how to convert to  CollectionView
			SqlCustAccounts = e . DataSource as ObservableCollection<CustomerViewModel>;
			this . CustomerGrid . ItemsSource = CollectionViewSource . GetDefaultView ( SqlCustAccounts );

			this . CustomerGrid . Refresh ( );
			CustomerGrid . SelectedIndex = cindex;
			CustomerGrid . SelectedItem = cindex;
			CustomerGrid . Refresh ( );
			CustomerGrid . UpdateLayout ( );
			Utils . SetUpGridSelection ( this . CustomerGrid , cindex );
			CustCount . Text = $"{this . CustomerGrid . SelectedIndex} / {this . CustomerGrid . Items . Count}";
			Mouse . OverrideCursor = Cursors . Arrow;
			LoadingDbData = false;
			IsDirty = false;
			// Let em see our message
			ClearWaitMessage ( );
		}
		/// <summary>
		/// Handles resetting the index after Details data has been reloaded
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void EventControl_DetDataLoaded ( object sender , LoadedEventArgs e )
		{
			if ( e . DataSource == null )
				return;//|| this.DetailsGrid.Items.Count > 0) return;
			if ( e . CallerDb != "MULTIVIEWER" && e . CallerType != "SQLSERVER" )
				return;

			//Flags . SqlDetActive = false;
			//			stopwatch3 . Stop ( );
			Debug . WriteLine ( $"MULTIVIEWER : Details Data fully loaded with {e . RowCount} " );
			LoadingDbData = true;
			DetailsGrid . ItemsSource = null;
			DetailsGrid . Items . Clear ( );
			//			SqlDetAccounts?.Clear ( );
			SqlDetAccounts = e . DataSource as ObservableCollection<DetailsViewModel>;
			this . DetailsGrid . ItemsSource = CollectionViewSource . GetDefaultView ( SqlDetAccounts );

			this . DetailsGrid . Refresh ( );
			DetailsGrid . SelectedIndex = dindex;
			DetailsGrid . SelectedItem = dindex;
			DetailsGrid . Refresh ( );
			DetailsGrid . UpdateLayout ( );
			Utils . SetUpGridSelection ( this . DetailsGrid , dindex );
			DetCount . Text = $"{this . DetailsGrid . SelectedIndex} / {this . DetailsGrid . Items . Count}";
			Mouse . OverrideCursor = Cursors . Arrow;
			LoadingDbData = false;
			Debug . WriteLine ( $"MULTIVIEWER : Datagrid fully loaded with {DetailsGrid . Items . Count} " );
			// Let em see our message
			ClearWaitMessage ( );
		}
		#endregion Post Data Reloaded event handlers

		private void SubscribeToEvents ( )
		{
			Utils . SetupWindowDrag ( this );

			// An EditDb has changed the current index
			EventControl . EditIndexChanged += EventControl_ViewerIndexChanged;
			// Another SqlDbviewer has changed the current index
			EventControl . ViewerIndexChanged += EventControl_ViewerIndexChanged;      // Callback in THIS FILE

			// data updated by another grid 
			EventControl . ViewerDataUpdated += EventControl_SqlViewerDataUpdated;
			EventControl . MultiViewerDataUpdated += EventControl_ViewerDataUpdated;
			EventControl . GlobalDataChanged += EventControl_GlobalDataChanged;
			EventControl . EditDbDataUpdated += EventControl_DataUpdated;
			// Data loaded event handlers
			EventControl . BankDataLoaded += EventControl_BankDataLoaded;
			EventControl . CustDataLoaded += EventControl_CustDataLoaded;
			EventControl . DetDataLoaded += EventControl_DetDataLoaded;
		}
		private async void EventControl_GlobalDataChanged ( object sender , GlobalEventArgs e )
		{
			if ( e . CallerType == "MULTIVIEWER" && e . AccountType == CurrentDb )
				return;
			// update all grids EXCEPT the default in AccountType
			//Update our own data tyoe only
			if ( CurrentDb == "BANKACCOUNT" )
			{
				Flags . SqlBankActive = true;
				//				BankCollection . LoadBank ( null , "BANKACCOUNT" , 1 , true );
				if ( Flags . USECOPYDATA )
				{
					DapperSupport . GetBankObsCollection ( collection: SqlBankAccounts ,
					DbNameToLoad: Flags . COPYBANKDATANAME ,
					wantSort: true ,
					Notify: true ,
					Caller: "MULTIVIEWER" );
				}
				else
				{
					DapperSupport . GetBankObsCollection ( collection: SqlBankAccounts ,
						wantSort: true ,
						Notify: true ,
						Caller: "MULTIVIEWER" );
				}
			}
			else if ( CurrentDb == "CUSTOMER" )
			{
				Flags . SqlCustActive = true;
				//				AllCustomers . LoadCust ( null , "CUSTOMER" , 2 , true );
				if ( Flags . USECOPYDATA )
				{
					SqlCustAccounts = DapperSupport . GetCustObsCollection ( collection: SqlCustAccounts ,
					DbNameToLoad: Flags . COPYCUSTDATANAME ,
					Caller: "MULTIVIEWER" ,
					Notify: true );
				}
				else
				{
					SqlCustAccounts = DapperSupport . GetCustObsCollection ( collection: SqlCustAccounts ,
						Caller: "MULTIVIEWER" ,
						Notify: true );
				}
			}
			else if ( CurrentDb == "DETAILS" )
			{
				Flags . SqlDetActive = true;
				//				DetailCollection . LoadDet ( "DETAILS" , 1 , true );
				if ( Flags . USECOPYDATA )
				{
					DapperSupport . GetDetailsObsCollection ( SqlDetAccounts ,
					DbNameToLoad: Flags . COPYDETDATANAME ,
					wantSort: true ,
					wantDictionary: false ,
					Notify: true ,
					Caller: "MULTIVIEWER" );
				}
				else
				{
					DapperSupport . GetDetailsObsCollection ( SqlDetAccounts ,
					wantSort: true ,
					wantDictionary: false ,
					Notify: true ,
					Caller: "MULTIVIEWER" );
				}
			}
		}

		private async void EventControl_ViewerIndexChanged ( object sender , IndexChangedArgs e )
		{
			if ( IsEditing )
			{
				return;
			}

			if ( Flags . LinkviewerRecords && Triggered == false ) //|| Flags.IsFiltered|| Flags.IsMultiMode	)
			{
				//				object RowTofind = null;
				//				object gr = null;
				int rec = 0;
				//				if ( inprogress )
				//					return;
				inprogress = true;
				if ( GridsLinked == false )
				{
					if ( e . Sender == "BANKACCOUNT" )
					{
						// Only Update the specific grid
						rec = Utils . FindMatchingRecord ( e . Custno , e . Bankno , this . BankGrid , "BANKACCOUNT" );
						this . BankGrid . SelectedIndex = rec != -1 ? rec : 0;
						bindex = rec;
						Utils . ScrollRecordIntoView ( this . BankGrid , rec );
						inprogress = false;
						SaveCurrentIndex ( 1 , BankGrid . SelectedIndex );
						BankCount . Text = $"{this . BankGrid . SelectedIndex} / {this . BankGrid . Items . Count}";
						return;
					}
					else if ( e . Sender == "CUSTOMER" )
					{
						// Only Update the specific grid
						rec = Utils . FindMatchingRecord ( e . Custno , e . Bankno , this . CustomerGrid , "CUSTOMER" );
						this . CustomerGrid . SelectedIndex = rec != -1 ? rec : 0;
						cindex = rec;
						BankData . DataContext = this . CustomerGrid . SelectedItem;
						Utils . ScrollRecordIntoView ( this . CustomerGrid , rec );
						inprogress = false;
						SaveCurrentIndex ( 2 , CustomerGrid . SelectedIndex );
						CustCount . Text = $"{this . CustomerGrid . SelectedIndex} / {this . CustomerGrid . Items . Count}";
						return;
					}
					if ( e . Sender == "DETAILS" )
					{
						// Only Update the specific grid
						rec = Utils . FindMatchingRecord ( e . Custno , e . Bankno , this . DetailsGrid , "DETAILS" );
						this . DetailsGrid . SelectedIndex = rec != -1 ? rec : 0;
						dindex = rec;
						Utils . ScrollRecordIntoView ( this . DetailsGrid , rec );
						inprogress = false;
						SaveCurrentIndex ( 3 , DetailsGrid . SelectedIndex );
						DetCount . Text = $"{this . DetailsGrid . SelectedIndex} / {this . DetailsGrid . Items . Count}";
						return;
					}
				}
				else
				{
					// Update all three grids
					rec = Utils . FindMatchingRecord ( e . Custno , e . Bankno , this . BankGrid , "BANKACCOUNT" );
					this . BankGrid . SelectedIndex = rec != -1 ? rec : 0;
					bindex = rec;
					Utils . ScrollRecordIntoView ( this . BankGrid , rec );
					rec = Utils . FindMatchingRecord ( e . Custno , e . Bankno , this . CustomerGrid , "CUSTOMER" );
					this . CustomerGrid . SelectedIndex = rec != -1 ? rec : 0;
					cindex = rec;
					BankData . DataContext = this . CustomerGrid . SelectedItem;
					Utils . ScrollRecordIntoView ( this . CustomerGrid , rec );
					rec = Utils . FindMatchingRecord ( e . Custno , e . Bankno , this . DetailsGrid , "DETAILS" );
					this . DetailsGrid . SelectedIndex = rec != -1 ? rec : 0;
					dindex = rec;
					Utils . ScrollRecordIntoView ( this . DetailsGrid , rec );

					BankCount . Text = $"{this . BankGrid . SelectedIndex} / {this . BankGrid . Items . Count}";
					CustCount . Text = $"{this . CustomerGrid . SelectedIndex} / {this . CustomerGrid . Items . Count}";
					DetCount . Text = $"{this . DetailsGrid . SelectedIndex} / {this . DetailsGrid . Items . Count}";

					// Finally, tell other viewers about the index change
					if ( e . Sender == "BANKACCOUNT" )
					{
						SaveCurrentIndex ( 1 , BankGrid . SelectedIndex );
						BankAccountViewModel bvm = new BankAccountViewModel ( );
						bvm = this . BankGrid . CurrentItem as BankAccountViewModel;
						if ( bvm == null )
						{
							inprogress = false;
							return;
						}
						if ( e . Sender == "MULTIVIEWER" )
							TriggerMultiViewerIndexChanged ( this . BankGrid );
					}
					else if ( e . Sender == "CUSTOMER" )
					{
						SaveCurrentIndex ( 2 , CustomerGrid . SelectedIndex );
						CustomerViewModel bvm = new CustomerViewModel ( );
						bvm = this . CustomerGrid . CurrentItem as CustomerViewModel;
						if ( bvm == null )
						{
							inprogress = false;
							return;
						}
						if ( e . Sender == "MULTIVIEWER" )
							TriggerMultiViewerIndexChanged ( this . CustomerGrid );
					}
					else if ( e . Sender == "DETAILS" )
					{
						SaveCurrentIndex ( 3 , DetailsGrid . SelectedIndex );
						DetailsViewModel bvm = new DetailsViewModel ( );
						bvm = this . DetailsGrid . CurrentItem as DetailsViewModel;
						if ( bvm == null )
						{
							inprogress = false;
							return;
						}
						if ( e . Sender == "MULTIVIEWER" )
							TriggerMultiViewerIndexChanged ( this . DetailsGrid );
					}
				}
				BankCount . Text = $"{this . BankGrid . SelectedIndex} / {this . BankGrid . Items . Count}";
				CustCount . Text = $"{this . CustomerGrid . SelectedIndex} / {this . CustomerGrid . Items . Count}";
				DetCount . Text = $"{this . DetailsGrid . SelectedIndex} / {this . DetailsGrid . Items . Count}";
				inprogress = false;
			}
		}


		private async Task LoadAllData ( )
		{
			// load the data
			Mouse . OverrideCursor = Cursors . Wait;
			//			if ( MultiBankcollection == null || MultiBankcollection . Count == 0 )
			SqlBankAccounts = null;
			///			stopwatch1 . Start ( );
			Flags . SqlBankActive = true;
			//			BankCollection . LoadBank ( SqlBankAccounts , "MULTIVIEWER" , 3 , true );
			if ( Flags . USECOPYDATA )
			{
				DapperSupport . GetBankObsCollection ( collection: SqlBankAccounts ,
					DbNameToLoad: Flags . COPYBANKDATANAME ,
					wantSort: true ,
					Notify: true ,
					Caller: "MULTIVIEWER" );
			}
			else
			{
				DapperSupport . GetBankObsCollection ( collection: SqlBankAccounts ,
					wantSort: true ,
					Notify: true ,
					Caller: "MULTIVIEWER" );
			}
			//BankGrid . ItemsSource = MultiBankcollection;
			//			if ( MultiCustcollection == null || MultiCustcollection . Count == 0 )
			SqlCustAccounts = null;
			//			stopwatch2 . Start ( );
			Flags . SqlCustActive = true;
			//			AllCustomers . LoadCust ( SqlCustAccounts , "MULTIVIEWER" , 3 , true );
			if ( Flags . USECOPYDATA )
			{
				SqlCustAccounts = DapperSupport . GetCustObsCollection ( collection: SqlCustAccounts ,
					DbNameToLoad: Flags . COPYCUSTDATANAME ,
				Caller: "MULTIVIEWER" ,
				Notify: true );
			}
			else
			{
				SqlCustAccounts = DapperSupport . GetCustObsCollection ( collection: SqlCustAccounts ,
					Caller: "MULTIVIEWER" ,
					Notify: true );
			}
			//			if ( MultiDetcollection == null || MultiDetcollection . Count == 0 )
			SqlDetAccounts = null;

			DetCollection det = new DetCollection ( );
			//			stopwatch3 . Start ( );
			Flags . SqlDetActive = true;
			//			DetailCollection . LoadDet ( "MULTIVIEWER" , 2 , true );
			if ( Flags . USECOPYDATA )
			{
				DapperSupport . GetDetailsObsCollection ( SqlDetAccounts ,
					DbNameToLoad: Flags . COPYDETDATANAME ,
				wantSort: true ,
				wantDictionary: false ,
				Notify: true ,
				Caller: "MULTIVIEWER" );
			}
			else
			{
				DapperSupport . GetDetailsObsCollection ( SqlDetAccounts ,
				wantSort: true ,
				wantDictionary: false ,
				Notify: true ,
				Caller: "MULTIVIEWER" );
			}

			Flags . SqlMultiViewer = this;
		}

		#endregion STARTUP/CLOSE

		#region EVENT HANDLERS

		#endregion EVENT HANDLERS

		#region DATA UPDATING

		/// <summary>
		/// Handles the SQL updateof any changes made and updates all grids
		/// </summary>
		/// <param name="CurrentDb"></param>
		/// <param name="e"></param>
		public void UpdateOnDataChange ( string CurrentDb , DataGridRowEditEndingEventArgs e )
		{
			// Call Handler to update ALL Db's via SQL
			Mouse . OverrideCursor = Cursors . Wait;
			SQLHandlers sqlh = new SQLHandlers ( );
			sqlh . UpdateAllDb ( CurrentDb , e );
			//bindex = this . BankGrid . SelectedIndex;
			//cindex = this . CustomerGrid . SelectedIndex;
			//dindex = this . DetailsGrid . SelectedIndex;
			////Gotta reload our data because the update clears it down totally to null
			//// Refresh our grids
			//RefreshAllGrids ( CurrentDb, e . Row . GetIndex ( ) );
			//inprogress = false;
			Mouse . OverrideCursor = Cursors . Arrow;
		}
		public void RefreshAllGrids ( string CurrentDb , int row , string Custno = "" , string Bankno = "" )
		{
			Mouse . OverrideCursor = Cursors . Wait;
			ReloadingData = true;
			if ( row == -1 )
				row = 0;
			ReLoadAllDataBases ( "" , row , Custno , Bankno );
			ReloadingData = false;

			StatusBar . Text = "All available Records are shown above in all three grids";
			Mouse . OverrideCursor = Cursors . Arrow;
			ReloadingData = false;
		}

		private void ReLoadAllDataBases ( string CurrentD , int row , string Custno = "" , string Bankno = "" )
		{
			int bbindex = 0;
			int ccindex = 0;
			int ddindex = 0;
			//			bool DataAvailable = false;
			BankAccountViewModel bvm = new BankAccountViewModel ( );
			CustomerViewModel cvm = new CustomerViewModel ( );
			DetailsViewModel dvm = new DetailsViewModel ( );
			//			int rec = 0;
			if ( row == -1 )
				row = 0;

			// If we have received updated data, reset global indexes
			if ( Custno != "" && Bankno != "" )
			{
				bbindex = Utils . FindMatchingRecord ( Custno , Bankno , this . BankGrid , "BANKACCOUNT" );
				ccindex = Utils . FindMatchingRecord ( Custno , Bankno , this . CustomerGrid , "CUSTOMER" );
				ddindex = Utils . FindMatchingRecord ( Custno , Bankno , this . DetailsGrid , "DETAILS" );
				//				DataAvailable = true;
			}
			else
			{
				// Get the current records data from each datagrid
				bbindex = this . BankGrid . SelectedIndex < 0 ? 0 : this . BankGrid . SelectedIndex;
				ccindex = this . CustomerGrid . SelectedIndex < 0 ? 0 : this . CustomerGrid . SelectedIndex;
				ddindex = this . DetailsGrid . SelectedIndex < 0 ? 0 : this . DetailsGrid . SelectedIndex;
			}
			//			Utils . PlayMary ( );

			// Assign correct index to item source
			// These SelectedIndex changes ALL trigger the SelectionChanged Method() !!!!!
			this . BankGrid . SelectedIndex = bbindex;
			this . CustomerGrid . SelectedIndex = ccindex;
			this . DetailsGrid . SelectedIndex = ddindex;


			this . BankGrid . SelectedItem = bbindex;
			this . CustomerGrid . SelectedItem = ccindex;
			this . DetailsGrid . SelectedItem = ddindex;

			bvm = this . BankGrid . SelectedItem as BankAccountViewModel;
			cvm = this . CustomerGrid . SelectedItem as CustomerViewModel;
			dvm = this . DetailsGrid . SelectedItem as DetailsViewModel;
			Custno = bvm?.CustNo;
			Bankno = bvm?.BankNo;

			// Now go ahead and clear the data and then reload it
			// These SelectedIndex changes ALL trigger the SelectionChanged Method() !!!!!
			this . BankGrid . ItemsSource = null;
			this . CustomerGrid . ItemsSource = null;
			this . DetailsGrid . ItemsSource = null;
			this . BankGrid . Items . Clear ( );
			this . CustomerGrid . Items . Clear ( );
			this . DetailsGrid . Items . Clear ( );

			/// Reoad the data into our Items Source collections
			Flags . SqlBankActive = true;
			Flags . SqlCustActive = true;
			Flags . SqlDetActive = true;
			SqlBankAccounts? . Clear ( );
			BankGrid . Items . Clear ( );
			BankGrid . ItemsSource = null;
			SqlCustAccounts? . Clear ( );
			CustomerGrid . Items . Clear ( );
			CustomerGrid . ItemsSource = null;
			SqlDetAccounts? . Clear ( );
			DetailsGrid . Items . Clear ( );
			DetailsGrid . ItemsSource = null;
			if ( Flags . FilterCommand == "" )
			{
				//				BankCollection . LoadBank ( SqlBankAccounts , "MULTIVIEWER" , 3 , true );
				if ( Flags . USECOPYDATA )
				{
					DapperSupport . GetBankObsCollection ( collection: SqlBankAccounts ,
					DbNameToLoad: Flags . COPYBANKDATANAME ,
					wantSort: true ,
					Notify: true ,
					Caller: "MULTIVIEWER" );
				}
				else
				{
					DapperSupport . GetBankObsCollection ( collection: SqlBankAccounts ,
						wantSort: true ,
						Notify: true ,
						Caller: "MULTIVIEWER" );
				}
				//				AllCustomers . LoadCust ( SqlCustAccounts , "MULTIVIEWER" , 3 , true );
				if ( Flags . USECOPYDATA )
				{
					SqlCustAccounts = DapperSupport . GetCustObsCollection ( collection: SqlCustAccounts ,
					DbNameToLoad: Flags . COPYCUSTDATANAME ,
					Caller: "MULTIVIEWER" ,
					Notify: true );
				}
				else
				{
					SqlCustAccounts = DapperSupport . GetCustObsCollection ( collection: SqlCustAccounts ,
						Caller: "MULTIVIEWER" ,
						Notify: true );
				}
//				DetailCollection . LoadDet ( "MULTIVIEWER" , 3 , true );
			}
			else
			{
				string tmp = Flags.FilterCommand.Substring(26);
				//				BankCollection . LoadBank ( SqlBankAccounts , "MULTIVIEWER" , 3 , true );
				if ( Flags . USECOPYDATA )
				{
					DapperSupport . GetBankObsCollection ( collection: SqlBankAccounts ,
					DbNameToLoad: Flags . COPYBANKDATANAME ,
					wantSort: true ,
					Notify: true ,
					Caller: "MULTIVIEWER" );
				}
				else
				{
					DapperSupport . GetBankObsCollection ( collection: SqlBankAccounts ,
						wantSort: true ,
						Notify: true ,
						Caller: "MULTIVIEWER" );
				}
				Flags . FilterCommand = $"Select * from Customer {tmp}";
				//				AllCustomers . LoadCust ( SqlCustAccounts , "MULTIVIEWER" , 3 , true );
				if ( Flags . USECOPYDATA )
				{
					SqlCustAccounts = DapperSupport . GetCustObsCollection ( collection: SqlCustAccounts ,
					DbNameToLoad: Flags . COPYCUSTDATANAME ,
					Caller: "MULTIVIEWER" ,
					Notify: true );
				}
				else
				{
					SqlCustAccounts = DapperSupport . GetCustObsCollection ( collection: SqlCustAccounts ,
						Caller: "MULTIVIEWER" ,
						Notify: true );
				}
				Flags . FilterCommand = $"Select * from Secaccounts {tmp}";
				//				DetailCollection . LoadDet ( "MULTIVIEWER" , 3 , true );
				if ( Flags . USECOPYDATA )
				{
					DapperSupport . GetDetailsObsCollection ( SqlDetAccounts ,
					DbNameToLoad: Flags . COPYDETDATANAME ,
					wantSort: true ,
					wantDictionary: false ,
					Notify: true ,
					Caller: "MULTIVIEWER" );
				}
				else
				{
					DapperSupport . GetDetailsObsCollection ( SqlDetAccounts ,
					wantSort: true ,
					wantDictionary: false ,
					Notify: true ,
					Caller: "MULTIVIEWER" );
				}
				Mouse . OverrideCursor = System . Windows . Input . Cursors . Arrow;
			}
		}
		#endregion EVENT DATA UPDATING

		#region User Defined Commands
		//
		//		MyCommands _InfoCommand = new MyCommands ( );
		//public MyCommands InformationCommand
		//{
		//	get { return _InfoCommand; }
		//}

		//private void ExecutedCloseApp ( object sender, ExecutedRoutedEventArgs e )
		//{
		//	App . Current . Shutdown ( );
		//}
		#endregion User Defined Commands


		public void RefreshData ( int row , string Custno = "" , string Bankno = "" )
		{
			//bindex = this . BankGrid . SelectedIndex;
			//cindex = this . CustomerGrid . SelectedIndex;
			//dindex = this . DetailsGrid . SelectedIndex;
			//this . BankGrid . ItemsSource = null;
			//this . BankGrid . ItemsSource = MultiBankcollection;
			//this . CustomerGrid . ItemsSource = null;
			//this . CustomerGrid . ItemsSource = MultiCustcollection;
			//this . DetailsGrid . ItemsSource = null;
			//this . DetailsGrid . ItemsSource = MultiDetcollection;

			// This handles row selection AND refocus
			RefreshAllGrids ( CurrentDb , row );
			Mouse . OverrideCursor = Cursors . Wait;
			inprogress = false;
			Mouse . OverrideCursor = System . Windows . Input . Cursors . Arrow;
		}

		private void Close_Click ( object sender , RoutedEventArgs e )
		{
			Close ( );
		}
		#region DATAGRID  SELECTION CHANGE  HANDLING  (SelectedIndex matching across all 3 grids)
		/// <summary>
		/// /// *************************************************************************
		/// THESE ALL WORK CORRECTLY, AND THE SELECTED ROWS ALL MATCH PERFECTLY - 15/5/2021
		/// /// *************************************************************************
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		//****************************************************************************************************//
		private async void BankGrid_SelectionChanged ( object sender , SelectionChangedEventArgs e )
		{
			int rec = 0;
			string SearchCustNo = "";
			string SearchBankNo = "";

			if ( LoadingDbData || ReloadingData )
			{
				LoadingDbData = false;
				return;
			}
			if ( inprogress )
				return;

			if ( IsEditing )
			{
				e . Handled = true;
				return;
			}

			BankAccountViewModel CurrentSelectedRecord = this . BankGrid . SelectedItem as BankAccountViewModel;
			if ( CurrentSelectedRecord == null )
				return;
			bindex = this . BankGrid . SelectedIndex;

			inprogress = true;

			// See if we need to update th eother grids on this multi viewer
			if ( LinkGrids . IsChecked == true )
			{
				int currsel = this . BankGrid . SelectedIndex;
				// We have the link our own grids option checked
				// so update our other 2 grids positions
				SearchCustNo = CurrentSelectedRecord?.CustNo;
				SearchBankNo = CurrentSelectedRecord?.BankNo;
				if ( SearchCustNo == null && SearchBankNo == null )
				{ inprogress = false; return; }

				Triggered = true;

				rec = Utils . FindMatchingRecord ( SearchCustNo , SearchBankNo , this . CustomerGrid , "CUSTOMER" );
				// Store current index to global
				cindex = rec;
				this . CustomerGrid . UnselectAll ( );
				this . CustomerGrid . SelectedIndex = rec;
				this . CustomerGrid . SelectedItem = rec;
				Utils . SetUpGridSelection ( this . CustomerGrid , rec );
				rec = Utils . FindMatchingRecord ( SearchCustNo , SearchBankNo , this . DetailsGrid , "DETAILS" );
				// Store current index to global
				dindex = rec;
				this . DetailsGrid . UnselectAll ( );
				this . DetailsGrid . SelectedIndex = rec;
				this . DetailsGrid . SelectedItem = rec;
				Utils . SetUpGridSelection ( this . DetailsGrid , rec );
				// The global linkage is ALSO set, so
				// we must notify any other windows that may need to update themselves
				if ( Flags . LinkviewerRecords )
					TriggerMultiViewerIndexChanged ( this . BankGrid );
				BankData . DataContext = this . BankGrid . SelectedItem;
				Triggered = false;
			}
			else if ( Flags . LinkviewerRecords )
				TriggerMultiViewerIndexChanged ( this . BankGrid );

			BankCount . Text = $"{this . BankGrid . SelectedIndex} / {this . BankGrid . Items . Count}";
			CustCount . Text = $"{this . CustomerGrid . SelectedIndex} / {this . CustomerGrid . Items . Count}";
			DetCount . Text = $"{this . DetailsGrid . SelectedIndex} / {this . DetailsGrid . Items . Count}";
			bindex = this . BankGrid . SelectedIndex;
			BankCount . Text = $"{bindex} / {this . BankGrid . Items . Count}";
			//			Debug . WriteLine ( $"BankGrid Index = {bindex}" );
			SaveCurrentIndex ( 1 , BankGrid . SelectedIndex );
			inprogress = false;
			try
			{ e . Handled = true; }
			catch { }
			return;
		}
		//****************************************************************************************************//
		private async void CustGrid_SelectionChanged ( object sender , SelectionChangedEventArgs e )
		{
			int rec = 0;
			string SearchCustNo = "";
			string SearchBankNo = "";

			if ( LoadingDbData || ReloadingData )
			{
				LoadingDbData = false;
				return;
			}
			if ( inprogress )
				return;

			if ( IsEditing )
			{
				e . Handled = true;
				return;
			}

			CustomerViewModel CurrentSelectedRecord = this . CustomerGrid . SelectedItem as CustomerViewModel;
			if ( CurrentSelectedRecord == null )
				return;
			cindex = this . CustomerGrid . SelectedIndex;

			inprogress = true;

			if ( LinkGrids . IsChecked == true )
			{
				int currsel = this . CustomerGrid . SelectedIndex;
				// We triggered this change
				SearchCustNo = CurrentSelectedRecord?.CustNo;
				SearchBankNo = CurrentSelectedRecord?.BankNo;
				if ( SearchCustNo == null && SearchBankNo == null )
				{ inprogress = false; return; }

				// We have the link our own grids option checked
				// so update all our grids position
				Triggered = true;

				rec = Utils . FindMatchingRecord ( SearchCustNo , SearchBankNo , this . BankGrid , "BANKACCOUNT" );
				// Store current index to global
				bindex = rec;
				this . BankGrid . UnselectAll ( );
				this . BankGrid . SelectedIndex = rec;
				this . BankGrid . SelectedItem = rec;
				Utils . SetUpGridSelection ( this . BankGrid , rec );
				//Utils . ScrollRecordIntoView ( this . BankGrid, rec );

				rec = Utils . FindMatchingRecord ( SearchCustNo , SearchBankNo , this . DetailsGrid , "DETAILS" );
				// Store current index to global
				dindex = rec;
				this . DetailsGrid . UnselectAll ( );
				this . DetailsGrid . SelectedIndex = rec;
				this . DetailsGrid . SelectedItem = rec;
				Utils . SetUpGridSelection ( this . DetailsGrid , rec );

				BankData . DataContext = this . DetailsGrid . SelectedItem;
				// The global linkage is set, so
				// we must notify any other windows that may need to update themselves
				if ( Flags . LinkviewerRecords )
					TriggerMultiViewerIndexChanged ( this . CustomerGrid );

				Triggered = false;
			}
			else if ( Flags . LinkviewerRecords )
				TriggerMultiViewerIndexChanged ( this . CustomerGrid );

			BankCount . Text = $"{this . BankGrid . SelectedIndex} / {this . BankGrid . Items . Count}";
			CustCount . Text = $"{this . CustomerGrid . SelectedIndex} / {this . CustomerGrid . Items . Count}";
			DetCount . Text = $"{this . DetailsGrid . SelectedIndex} / {this . DetailsGrid . Items . Count}";
			inprogress = false;
			cindex = this . CustomerGrid . SelectedIndex;
			CustCount . Text = $"{cindex} / {this . CustomerGrid . Items . Count}";
			SaveCurrentIndex ( 2 , CustomerGrid . SelectedIndex );

			try
			{ e . Handled = true; }
			catch { }
			return;

		}

		private async void DetGrid_SelectionChanged ( object sender , SelectionChangedEventArgs e )
		{
			int rec = 0;
			string SearchCustNo = "";
			string SearchBankNo = "";

			if ( LoadingDbData || ReloadingData )
			{
				LoadingDbData = false;
				return;
			}
			if ( inprogress )
				return;

			if ( IsEditing )
			{
				e . Handled = true;
				return;
			}

			dindex = this . DetailsGrid . SelectedIndex;
			DetailsViewModel CurrentSelectedRecord = this . DetailsGrid . SelectedItem as DetailsViewModel;
			if ( CurrentSelectedRecord == null )
				return;

			inprogress = true;

			if ( LinkGrids . IsChecked == true )
			{
				int currsel = this . DetailsGrid . SelectedIndex;
				SearchCustNo = CurrentSelectedRecord?.CustNo;
				SearchBankNo = CurrentSelectedRecord?.BankNo;
				if ( SearchCustNo == null && SearchBankNo == null )
				{ inprogress = false; return; }

				// update all grids position
				Triggered = true;

				rec = Utils . FindMatchingRecord ( SearchCustNo , SearchBankNo , this . BankGrid , "BANKACCOUNT" );
				// Store current index to global
				bindex = rec;
				//				this . BankGrid . UnselectAll ( );
				this . BankGrid . SelectedIndex = rec;
				this . BankGrid . SelectedItem = rec;
				Utils . SetUpGridSelection ( this . BankGrid , rec );

				rec = Utils . FindMatchingRecord ( SearchCustNo , SearchBankNo , this . CustomerGrid , "CUSTOMER" );
				// Store current index to global
				cindex = rec;
				this . CustomerGrid . UnselectAll ( );
				this . CustomerGrid . SelectedIndex = rec;
				this . CustomerGrid . SelectedItem = rec;
				Utils . SetUpGridSelection ( this . CustomerGrid , rec );
				// The global linkage is set, so
				BankData . DataContext = this . DetailsGrid . SelectedItem;
				// we must notify any other windows that may need to update themselves
				if ( Flags . LinkviewerRecords )
					TriggerMultiViewerIndexChanged ( this . DetailsGrid );
				Triggered = false;
			}
			else if ( Flags . LinkviewerRecords )
				TriggerMultiViewerIndexChanged ( this . DetailsGrid );

			BankCount . Text = $"{this . BankGrid . SelectedIndex} / {this . BankGrid . Items . Count}";
			CustCount . Text = $"{this . CustomerGrid . SelectedIndex} / {this . CustomerGrid . Items . Count}";
			DetCount . Text = $"{this . DetailsGrid . SelectedIndex} / {this . DetailsGrid . Items . Count}";

			inprogress = false;
			dindex = this . DetailsGrid . SelectedIndex;
			DetCount . Text = $"{dindex} / {this . DetailsGrid . Items . Count}";
			SaveCurrentIndex ( 3 , DetailsGrid . SelectedIndex );
			try
			{ e . Handled = true; }
			catch { }
			return;
		}

		#endregion DATAGRID  SELECTION CHANGE  HANDLING

		#region focus events

		private void CustGrid_GotFocus ( object sender , RoutedEventArgs e )
		{ CurrentDb = "CUSTOMER"; }
		private void BankGrid_GotFocus ( object sender , RoutedEventArgs e )
		{ CurrentDb = "BANKACCOUNT"; }
		private void DetGrid_GotFocus ( object sender , RoutedEventArgs e )
		{ CurrentDb = "DETAILS"; }

		#endregion focus events

		#region SCROLLBARS

		// scroll bar movement is automatically   stored by these three methods
		// So we can use them to reset position CORRECTLY after refreshes
		private void BankGrid_ScrollChanged ( object sender , ScrollChangedEventArgs e )
		{
			//int rec = 0;
			//DataGrid dg = null;
			//dg = sender as DataGrid;
			//var scroll = DataGridNavigation . FindVisualChild<ScrollViewer> ( ( DependencyObject ) dg );
			//scroll . CanContentScroll = true;
			//SetScrollVariables ( sender );
			//Utils . SetUpGridSelection ( this . BankGrid, this . BankGrid . SelectedIndex );
			//Utils . SetUpGridSelection ( this . CustomerGrid, this . BankGrid . SelectedIndex );
			//Utils . SetUpGridSelection ( this . DetailsGrid, this . BankGrid . SelectedIndex );
			////			this . BankGrid . ScrollIntoView ( this . BankGrid . SelectedIndex );
			////			this . CustomerGrid . ScrollIntoView ( this . BankGrid . SelectedIndex );
			////			this . DetailsGrid . ScrollIntoView ( this . BankGrid . SelectedIndex );

		}
		private void CustGrid_ScrollChanged ( object sender , ScrollChangedEventArgs e )
		{
			//DataGrid dg = null;
			//dg = sender as DataGrid;
			//var scroll = DataGridNavigation . FindVisualChild<ScrollViewer> ( ( DependencyObject ) dg );
			//scroll . CanContentScroll = true;
			//SetScrollVariables ( sender );
			//Utils . SetUpGridSelection ( this . BankGrid, this . BankGrid . SelectedIndex );
			//Utils . SetUpGridSelection ( this . CustomerGrid, this . BankGrid . SelectedIndex );
			//Utils . SetUpGridSelection ( this . DetailsGrid, this . BankGrid . SelectedIndex );
			////			this . BankGrid . ScrollIntoView ( this . CustomerGrid . SelectedIndex );
			////			this . CustomerGrid . ScrollIntoView ( this . CustomerGrid . SelectedIndex );
			////			this . DetailsGrid . ScrollIntoView ( this . CustomerGrid . SelectedIndex );
		}

		private void DetGrid_ScrollChanged ( object sender , ScrollChangedEventArgs e )
		{
			//DataGrid dg = null;
			//dg = sender as DataGrid;
			//var scroll = DataGridNavigation . FindVisualChild<ScrollViewer> ( ( DependencyObject ) dg );
			//scroll . CanContentScroll = true;
			//SetScrollVariables ( sender );
			//Utils . SetUpGridSelection ( this . BankGrid, this . DetailsGrid . SelectedIndex );
			//Utils . SetUpGridSelection ( this . CustomerGrid, this . DetailsGrid . SelectedIndex );
			//Utils . SetUpGridSelection ( this . DetailsGrid, this . DetailsGrid . SelectedIndex );
			////			this . CustomerGrid . ScrollIntoView ( this . DetailsGrid . SelectedIndex );
			////			this . BankGrid . ScrollIntoView ( this . DetailsGrid . SelectedIndex );
			////			this . DetailsGrid . ScrollIntoView ( this . DetailsGrid . SelectedIndex );
		}
		#endregion SCROLLBARS

		#region Scroll bar utilities
		public void SetScrollVariables ( object sender )
		{
			SetTopViewRow ( sender );
			SetBottomViewRow ( sender );
			SetViewPort ( sender );
		}
		public void SetTopViewRow ( object sender )
		{
			DataGrid dg = null;
			dg = sender as DataGrid;
			if ( dg . SelectedItem == null )
				return;
			var scroll = DataGridNavigation . FindVisualChild<ScrollViewer> ( ( DependencyObject ) sender );
			if ( scroll == null )
				return;
			scroll . CanContentScroll = true;
			double d = scroll . VerticalOffset;
			int rounded = Convert . ToInt32 ( d );
			if ( dg == this . BankGrid )
			{
				//				Debug . WriteLine ( $"\n######## Flags . TopVisibleDetGridRow == {scroll . VerticalOffset}\n######## TopVisible = { Flags . TopVisibleBankGridRow}\n######## NEW Value = { scroll . VerticalOffset}" );
				Flags . TopVisibleBankGridRow = ( double ) rounded;
				ScrollData . Banktop = ( double ) rounded;
			}
			else if ( dg == this . CustomerGrid )
			{
				//				Debug . WriteLine ( $"\n######## Flags . TopVisibleDetGridRow == {scroll . VerticalOffset}\n######## TopVisible = { Flags . TopVisibleCustGridRow}\n######## NEW Value = { scroll . VerticalOffset}" );
				Flags . TopVisibleCustGridRow = ( double ) rounded;
				ScrollData . Custtop = ( double ) rounded;
			}
			else if ( dg == this . DetailsGrid )
			{
				//				Debug . WriteLine ( $"\n######## Flags . TopVisibleDetGridRow == {scroll . VerticalOffset}\n######## TopVisible = { Flags . TopVisibleDetGridRow}\n######## NEW Value = { scroll . VerticalOffset}" );
				Flags . TopVisibleDetGridRow = ( double ) rounded;
				ScrollData . Dettop = ( double ) rounded;
			}
			//			Flags . ViewPortHeight = scroll . ViewportHeight;
		}

		public void SetBottomViewRow ( object sender )
		{
			DataGrid dg = null;
			dg = sender as DataGrid;
			if ( dg . SelectedItem == null )
				return;
			var scroll = DataGridNavigation . FindVisualChild<ScrollViewer> ( ( DependencyObject ) dg );
			if ( scroll == null )
				return;
			scroll . CanContentScroll = true;
			double d = scroll . VerticalOffset;
			int rounded = Convert . ToInt32 ( d );
			if ( dg == this . BankGrid )
			{
				//				Debug . WriteLine ( $"\n######## Flags . TopVisibleDetGridRow == {scroll . VerticalOffset}\n######## TopVisible = { Flags . BottomVisibleBankGridRow}\n######## NEW Value = { scroll . VerticalOffset}" );
				Flags . BottomVisibleBankGridRow = ( double ) rounded;
				ScrollData . Bankbottom = ( double ) rounded;
			}
			else if ( dg == this . CustomerGrid )
			{
				//				Debug . WriteLine ( $"\n######## Flags . TopVisibleDetGridRow == {scroll . VerticalOffset}\n######## TopVisible = { Flags . BottomVisibleCustGridRow}\n######## NEW Value = { scroll . VerticalOffset}" );
				Flags . BottomVisibleCustGridRow = ( double ) rounded;
				ScrollData . Custbottom = ( double ) rounded;
			}
			else if ( dg == this . DetailsGrid )
			{
				//				Debug . WriteLine ( $"\n######## Flags . TopVisibleDetGridRow == {scroll . VerticalOffset}\n######## TopVisible = { Flags . BottomVisibleDetGridRow}\n######## NEW Value = { scroll . VerticalOffset}" );
				Flags . BottomVisibleDetGridRow = ( double ) rounded;
				ScrollData . Detbottom = ( double ) rounded;
			}
		}
		public void SetViewPort ( object sender )
		{
			DataGrid dg = null;
			dg = sender as DataGrid;
			if ( dg . SelectedItem == null )
				return;
			var scroll = DataGridNavigation . FindVisualChild<ScrollViewer> ( ( DependencyObject ) dg );
			if ( scroll == null )
				return;
			scroll . CanContentScroll = true;
			Flags . ViewPortHeight = scroll . ViewportHeight;
			if ( dg == this . BankGrid )
				ScrollData . BankVisible = ( double ) scroll . ViewportHeight;
			else if ( dg == this . CustomerGrid )
				ScrollData . CustVisible = ( double ) scroll . ViewportHeight;
			else if ( dg == this . DetailsGrid )
				ScrollData . DetVisible = ( double ) scroll . ViewportHeight;
		}


		#endregion Scroll bar utilities

		private void DoDragMove ( )
		{
			//Handle the button NOT being the left mouse button
			// which will crash the DragMove Fn.....     cos it has to be the primary button !!!
			try
			{ DragMove ( ); }
			catch ( Exception ex )
			{ Debug . WriteLine ( $"General Exception : {ex . Message}, {ex . Data}" ); return; }
		}



		/// <summary>
		/// Limit datagrid content to multiple accounts data only
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Details_Click ( object sender , RoutedEventArgs e )
		{
			// display multi data only
			//			ReloadDataAllDataAsMulti ( );
			string s = MultiAccountText . Text;
			if ( s . Contains ( "<<-" ) || s . Contains ( "Show All" ) )
			{
				Flags . IsMultiMode = false;
				MultiAccountText . Text = "Multi Accounts";
				//// Set the gradient background
				ControlTemplate tmp = Utils . GetDictionaryControlTemplate ( "HorizontalGradientTemplateGray" );
				Multiaccounts . Template = tmp;
				Brush br = Utils . GetDictionaryBrush ( "HeaderBrushGray" );
				Multiaccounts . Background = br;
				Multiaccounts . Content = "Multi A/c Only";
				MultiAccountText . Text = "Multi A/c Only";
				Linq6_Click ( null , null );
			}
			else
			{
				Flags . IsMultiMode = true;
				ControlTemplate tmp = Utils . GetDictionaryControlTemplate ( "HorizontalGradientTemplateGreen" );
				Multiaccounts . Template = tmp;
				Brush br = Utils . GetDictionaryBrush ( "HeaderBrushGreen" );
				Multiaccounts . Background = br;
				Multiaccounts . Content = "Show All A/c's";
				MultiAccountText . Text = "Show All A/c's";
				Linq5_Click ( null , null );
			}
		}

		private async Task ReloadDataAllDataAsMulti ( )
		{
			// Make sure this window has it's pointer "Registered" cos we can
			//Show only Customers with multiple Bank Accounts
			string s = MultiAccountText . Text;
			if ( s . Contains ( "<<-" ) || s . Contains ( "Show All" ) )
			{
				Flags . IsMultiMode = false;
				MultiAccountText . Text = "Multi Accounts";
				//// Set the gradient background
				ControlTemplate tmp = Utils . GetDictionaryControlTemplate ( "HorizontalGradientTemplateGray" );
				Multiaccounts . Template = tmp;
				Brush br = Utils . GetDictionaryBrush ( "HeaderBrushGray" );
				Multiaccounts . Background = br;
				Multiaccounts . Content = "Multi A/c Only";
				MultiAccountText . Text = "Multi A/c Only";
			}
			else
			{
				Flags . IsMultiMode = true;
				ControlTemplate tmp = Utils . GetDictionaryControlTemplate ( "HorizontalGradientTemplateGreen" );
				Multiaccounts . Template = tmp;
				Brush br = Utils . GetDictionaryBrush ( "HeaderBrushGreen" );
				Multiaccounts . Background = br;
				Multiaccounts . Content = "Show All A/c's";
				MultiAccountText . Text = "Show All A/c's";
			}
			this . BankGrid . ItemsSource = null;
			this . BankGrid . Items . Clear ( );
			Flags . SqlBankActive = true;
			//			BankCollection . LoadBank ( SqlBankAccounts , "MULTIVIEWER" , 1 , true );
			if ( Flags . USECOPYDATA )
			{
				DapperSupport . GetBankObsCollection ( collection: SqlBankAccounts ,
					DbNameToLoad: Flags . COPYBANKDATANAME ,
				wantSort: true ,
				Notify: true ,
				Caller: "MULTIVIEWER" );
			}
			else
			{
				DapperSupport . GetBankObsCollection ( collection: SqlBankAccounts ,
					wantSort: true ,
					Notify: true ,
					Caller: "MULTIVIEWER" );
			}
			//			this . BankGrid . ItemsSource = CollectionViewSource . GetDefaultView ( SqlBankAccounts );
			//			this . BankGrid . Refresh ( );
			this . CustomerGrid . ItemsSource = null;
			this . CustomerGrid . Items . Clear ( );
			Flags . SqlCustActive = true;
			//			AllCustomers . LoadCust ( SqlCustAccounts , "MULTIVIEWER" , 3 , true );
			if ( Flags . USECOPYDATA )
			{
				SqlCustAccounts = DapperSupport . GetCustObsCollection ( collection: SqlCustAccounts ,
					DbNameToLoad: Flags . COPYCUSTDATANAME ,
				Caller: "MULTIVIEWER" ,
				Notify: true );
			}
			else
			{
				SqlCustAccounts = DapperSupport . GetCustObsCollection ( collection: SqlCustAccounts ,
					Caller: "MULTIVIEWER" ,
					Notify: true );
			}
			//this . CustomerGrid . ItemsSource = CollectionViewSource . GetDefaultView ( SqlCustAccounts );
			//this . CustomerGrid . Refresh ( );
			//			ExtensionMethods . Refresh ( this . CustomerGrid );
			this . DetailsGrid . ItemsSource = null;
			this . DetailsGrid . Items . Clear ( );
			Flags . SqlDetActive = true;
			//			DetailCollection . LoadDet ( "MULTIVIEWER" , 3 , true );
			if ( Flags . USECOPYDATA )
			{
				DapperSupport . GetDetailsObsCollection ( SqlDetAccounts ,
					DbNameToLoad: Flags . COPYDETDATANAME ,
				wantSort: true ,
				wantDictionary: false ,
				Notify: true ,
				Caller: "MULTIVIEWER" );
			}
			else
			{
				DapperSupport . GetDetailsObsCollection ( SqlDetAccounts ,
				wantSort: true ,
				wantDictionary: false ,
				Notify: true ,
				Caller: "MULTIVIEWER" );
			}
			//this . DetailsGrid . ItemsSource = CollectionViewSource . GetDefaultView ( SqlDetAccounts );
			//this . DetailsGrid . Refresh ( );
			//			ExtensionMethods . Refresh ( this . DetailsGrid );
		}

		private void Filter_Click ( object sender , RoutedEventArgs e )
		{
			if ( CurrentDb == "" )
			{
				MessageBox . Show ( "Please select an entry in one of the data grids before trying to filter the data listed." );
				return;
			}
			if ( ( string ) FilterBtnText . Text == "Clear Filter" )
			{
				Flags . FilterCommand = "";
				ReLoadAllDataBases ( CurrentDb , -1 );
				FilterBtnText . Text = "Filter";
				ControlTemplate tmp = Utils . GetDictionaryControlTemplate ( "HorizontalGradientTemplateGray" );
				FilterBtn . Template = tmp;
				Brush br = Utils . GetDictionaryBrush ( "HeaderBrushGray" );
				FilterBtn . Background = br;
			}
			else
			{
				Filtering f = new Filtering ( );
				Flags . FilterCommand = f . DoFilters ( this , "BANKACCOUNT" , 1 );
				if ( Flags . FilterCommand == "" )
					return;
				ReLoadAllDataBases ( CurrentDb , -1 );
				// Clear our filter string
				Flags . FilterCommand = "";
				FilterBtnText . Text = "Clear Filter";
				ControlTemplate tmp = Utils . GetDictionaryControlTemplate ( "HorizontalGradientTemplateGreen" );
				FilterBtn . Template = tmp;
				Brush br = Utils . GetDictionaryBrush ( "HeaderBrushGreen" );
				FilterBtn . Background = br;
			}
		}

		private void BankGrid_Selected ( object sender , RoutedEventArgs e )
		{
			// hit when grid selection is changed by anything
			//int x = 0;
			//Console . WriteLine("...");
		}


		private void Refresh_Click ( object sender , RoutedEventArgs e )
		{
			RefreshData ( -1 );
		}

		private void BankDb_Click ( object sender , RoutedEventArgs e )
		{
			Window handle = null;
			if ( Utils . FindWindowFromTitle ( "Bank a/c editor" , ref handle ) )
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
		private void CustDb_Click ( object sender , RoutedEventArgs e )
		{
			Window handle = null;
			if ( Utils . FindWindowFromTitle ( "customer account editor" , ref handle ) )
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
		private void DetDb_Click ( object sender , RoutedEventArgs e )
		{
			Window handle = null;
			if ( Utils . FindWindowFromTitle ( "details a/c editor" , ref handle ) )
			{
				handle . Focus ( );
				handle . BringIntoView ( );
				return;
			}
			else
			{
				DetailsDbView cdbv = new DetailsDbView ( null, this, null );
				cdbv . Show ( );
			}
		}

		private void LinkRecords_Click ( object sender , RoutedEventArgs e )
		{
			Flags . LinkviewerRecords = !Flags . LinkviewerRecords;
			if ( Flags . SqlBankViewer != null )
				Flags . SqlBankViewer . LinkRecords . IsChecked = Flags . LinkviewerRecords;
			if ( Flags . SqlCustViewer != null )
				Flags . SqlCustViewer . LinkRecords . IsChecked = Flags . LinkviewerRecords;
			if ( Flags . SqlDetViewer != null )
				Flags . SqlDetViewer . LinkRecords . IsChecked = Flags . LinkviewerRecords;
			if ( Flags . SqlMultiViewer != null )
				Flags . SqlMultiViewer . LinkRecords . IsChecked = Flags . LinkviewerRecords;
			if ( Flags . BankDbEditor != null )
				Flags . BankDbEditor . LinkRecords . IsChecked = Flags . LinkviewerRecords;
			if ( Flags . CustDbEditor != null )
				Flags . CustDbEditor . LinkRecords . IsChecked = Flags . LinkviewerRecords;
			if ( Flags . DetDbEditor != null )
				Flags . DetDbEditor . LinkRecords . IsChecked = Flags . LinkviewerRecords;
			LinkRecords . Refresh ( );
		}

		private void OntopChkbox_Click ( object sender , RoutedEventArgs e )
		{
			if ( this . Topmost )
			{
				OntopChkbox . IsChecked = false;
				this . Topmost = false;
			}
			else
			{
				OntopChkbox . IsChecked = true;
				this . Topmost = true;
			}
		}

		#region LINQ queries
		private void Linq1_Click ( object sender , RoutedEventArgs e )
		{
			//select items;
			Mouse . OverrideCursor = Cursors . Wait;
			var accounts = from items in SqlBankAccounts
					   where ( items . AcType == 1 )
					   orderby items . CustNo
					   select items;
			this . BankGrid . ItemsSource = accounts;
			var accounts1 = from items in SqlCustAccounts
					    where ( items . AcType == 1 )
					    orderby items . CustNo
					    select items;
			this . CustomerGrid . ItemsSource = accounts1;
			var accounts2 = from items in SqlDetAccounts
					    where ( items . AcType == 1 )
					    orderby items . CustNo
					    select items;
			this . DetailsGrid . ItemsSource = accounts2;
			StatusBar . Text = "Only Records matching Account Type = 1 are shown above";
			BankCount . Text = $"{Utils . GetPrettyGridStatistics ( this . BankGrid , this . BankGrid . SelectedIndex )}";
			CustCount . Text = $"{Utils . GetPrettyGridStatistics ( this . CustomerGrid , this . CustomerGrid . SelectedIndex )}";
			DetCount . Text = $"{Utils . GetPrettyGridStatistics ( this . DetailsGrid , this . DetailsGrid . SelectedIndex )}";

			Mouse . OverrideCursor = Cursors . Arrow;
		}
		private void Linq2_Click ( object sender , RoutedEventArgs e )
		{
			//select items;
			Mouse . OverrideCursor = Cursors . Wait;
			var accounts = from items in SqlBankAccounts
					   where ( items . AcType == 2 )
					   orderby items . CustNo
					   select items;
			this . BankGrid . ItemsSource = accounts;
			var accounts1 = from items in SqlCustAccounts
					    where ( items . AcType == 2 )
					    orderby items . CustNo
					    select items;
			this . CustomerGrid . ItemsSource = accounts1;
			var accounts2 = from items in SqlDetAccounts
					    where ( items . AcType == 2 )
					    orderby items . CustNo
					    select items;
			this . DetailsGrid . ItemsSource = accounts2;
			StatusBar . Text = "Only Records matching Account Type = 2 are shown above";
			BankCount . Text = $"{Utils . GetPrettyGridStatistics ( this . BankGrid , this . BankGrid . SelectedIndex )}";
			CustCount . Text = $"{Utils . GetPrettyGridStatistics ( this . CustomerGrid , this . CustomerGrid . SelectedIndex )}";
			DetCount . Text = $"{Utils . GetPrettyGridStatistics ( this . DetailsGrid , this . DetailsGrid . SelectedIndex )}";
			Mouse . OverrideCursor = Cursors . Arrow;
		}
		private void Linq3_Click ( object sender , RoutedEventArgs e )
		{
			//select items;
			Mouse . OverrideCursor = Cursors . Wait;
			var accounts = from items in SqlBankAccounts
					   where ( items . AcType == 3 )
					   orderby items . CustNo
					   select items;
			this . BankGrid . ItemsSource = accounts;
			var accounts1 = from items in SqlCustAccounts
					    where ( items . AcType == 3 )
					    orderby items . CustNo
					    select items;
			this . CustomerGrid . ItemsSource = accounts1;
			var accounts2 = from items in SqlDetAccounts
					    where ( items . AcType == 3 )
					    orderby items . CustNo
					    select items;
			this . DetailsGrid . ItemsSource = accounts2;
			StatusBar . Text = "Only Records matching Account Type = 3 are shown above";
			BankCount . Text = $"{Utils . GetPrettyGridStatistics ( this . BankGrid , this . BankGrid . SelectedIndex )}";
			CustCount . Text = $"{Utils . GetPrettyGridStatistics ( this . CustomerGrid , this . CustomerGrid . SelectedIndex )}";
			DetCount . Text = $"{Utils . GetPrettyGridStatistics ( this . DetailsGrid , this . DetailsGrid . SelectedIndex )}";
			Mouse . OverrideCursor = Cursors . Arrow;
		}
		private void Linq4_Click ( object sender , RoutedEventArgs e )
		{
			//select items;
			Mouse . OverrideCursor = Cursors . Wait;
			var accounts = from items in SqlBankAccounts
					   where ( items . AcType == 4 )
					   orderby items . CustNo
					   select items;
			this . BankGrid . ItemsSource = accounts;
			var accounts1 = from items in SqlCustAccounts
					    where ( items . AcType == 4 )
					    orderby items . CustNo
					    select items;
			this . CustomerGrid . ItemsSource = accounts1;
			var accounts2 = from items in SqlDetAccounts
					    where ( items . AcType == 4 )
					    orderby items . CustNo
					    select items;
			this . DetailsGrid . ItemsSource = accounts2;
			StatusBar . Text = "Only Records matching Account Type = 4 are shown above";
			BankCount . Text = $"{Utils . GetPrettyGridStatistics ( this . BankGrid , this . BankGrid . SelectedIndex )}";
			CustCount . Text = $"{Utils . GetPrettyGridStatistics ( this . CustomerGrid , this . CustomerGrid . SelectedIndex )}";
			DetCount . Text = $"{Utils . GetPrettyGridStatistics ( this . DetailsGrid , this . DetailsGrid . SelectedIndex )}";
			Mouse . OverrideCursor = Cursors . Arrow;
		}
		private void Linq5_Click ( object sender , RoutedEventArgs e )
		{
			int q = 1;
			//select All the items first;
			Mouse . OverrideCursor = Cursors . Wait;
			if ( q == 1 )
			{
				var accounts = from items in SqlBankAccounts orderby items . CustNo, items . BankNo select items;
				//Next Group BankAccountViewModel collection on Custno
				var grouped = accounts . GroupBy ( b => b . CustNo );
				//Now filter content down to only those a/c's with multiple Bank A/c's
				var sel = from g in grouped where g . Count ( ) > 1 select g;
				// Finally, iterate thru the list of grouped CustNo's matching to CustNo in the full accounts data
				// giving us ONLY the full records for any recordss that have > 1 Bank accounts
				List<BankAccountViewModel> output = new List<BankAccountViewModel> ( );
				foreach ( var item1 in sel )
				{
					foreach ( var item2 in accounts )
					{
						if ( item2 . CustNo . ToString ( ) == item1 . Key )
						{ output . Add ( item2 ); }
					}
				}
				this . BankGrid . ItemsSource = output;
			}
			if ( q == 1 )
			{
				var accounts = from items in SqlCustAccounts orderby items . CustNo, items . BankNo select items;
				//Next Group  collection on Custno
				var grouped = accounts . GroupBy ( b => b . CustNo );
				//Now filter content down to only those a/c's with multiple Bank A/c's
				var sel = from g in grouped where g . Count ( ) > 1 select g;
				// Finally, iterate thru the list of grouped CustNo's matching to CustNo in the full accounts data
				// giving us ONLY the full records for any recordss that have > 1 Bank accounts
				List<CustomerViewModel> output = new List<CustomerViewModel> ( );
				foreach ( var item1 in sel )
				{
					foreach ( var item2 in accounts )
					{
						if ( item2 . CustNo . ToString ( ) == item1 . Key )
						{ output . Add ( item2 ); }
					}
				}
				this . CustomerGrid . ItemsSource = output;
			}
			if ( q == 1 )
			{
				var accounts = from items in SqlDetAccounts orderby items . CustNo, items . BankNo select items;
				//Next Group  collection on Custno
				var grouped = accounts . GroupBy ( b => b . CustNo );
				//Now filter content down to only those a/c's with multiple Bank A/c's
				var sel = from g in grouped where g . Count ( ) > 1 select g;
				// Finally, iterate thru the list of grouped CustNo's matching to CustNo in the full accounts data
				// giving us ONLY the full records for any recordss that have > 1 Bank accounts
				List<DetailsViewModel> output = new List<DetailsViewModel> ( );

				//				System . Diagnostics . PresentationTraceSources . SetTraceLevel ( DetailsGrid . ItemContainerGenerator, System . Diagnostics . PresentationTraceLevel . High );

				foreach ( var item1 in sel )
				{
					foreach ( var item2 in accounts )
					{
						if ( item2 . CustNo . ToString ( ) == item1 . Key )
						{ output . Add ( item2 ); }
					}
				}
				this . DetailsGrid . ItemsSource = output;
			}
			StatusBar . Text = "Only Records of Customers with multiple Bank Accounts are shown above";
			BankCount . Text = $"{Utils . GetPrettyGridStatistics ( this . BankGrid , this . BankGrid . SelectedIndex )}";
			CustCount . Text = $"{Utils . GetPrettyGridStatistics ( this . CustomerGrid , this . CustomerGrid . SelectedIndex )}";
			DetCount . Text = $"{Utils . GetPrettyGridStatistics ( this . DetailsGrid , this . DetailsGrid . SelectedIndex )}";
			Mouse . OverrideCursor = Cursors . Arrow;
		}
		private void Linq6_Click ( object sender , RoutedEventArgs e )
		{
			Mouse . OverrideCursor = Cursors . Wait;
			var accounts = from items in SqlBankAccounts orderby items . CustNo, items . AcType select items;
			var accounts1 = from items in SqlCustAccounts orderby items . CustNo, items . AcType select items;
			var accounts2 = from items in SqlDetAccounts orderby items . CustNo, items . AcType select items;
			this . BankGrid . ItemsSource = accounts;
			this . CustomerGrid . ItemsSource = accounts1;
			this . DetailsGrid . ItemsSource = accounts2;
			StatusBar . Text = "All available Records are shown above in all three grids";
			BankCount . Text = $"{Utils . GetPrettyGridStatistics ( this . BankGrid , this . BankGrid . SelectedIndex )}";
			CustCount . Text = $"{Utils . GetPrettyGridStatistics ( this . CustomerGrid , this . CustomerGrid . SelectedIndex )}";
			DetCount . Text = $"{Utils . GetPrettyGridStatistics ( this . DetailsGrid , this . DetailsGrid . SelectedIndex )}";
			Mouse . OverrideCursor = Cursors . Arrow;
		}

		private void bankjoin_Click ( object sender , RoutedEventArgs e )
		{
			List<DetailsViewModel> output = new List<DetailsViewModel> ( );
			List<int> joinData = new List<int> ( );

			// create 2 lists first
			var bank = from item1 in SqlBankAccounts select item1;
			var detail = from item2 in SqlDetAccounts select item2;

			//select All the items first;				
			var accounts = from alldata in bank . Join (
				detail,
				bank => bank . CustNo,
				detail => detail . CustNo,
				( bank, detail ) => new
				{
					bank1 = bank . BankNo,
					bank2 = detail . BankNo,
					custno1 = bank . CustNo,
					custno2 = detail . CustNo,
					actype1 = detail . AcType,
					actype2 = bank . AcType,
					Balance1 = detail . Balance,
					Balance2 = detail . Balance,
				} )
					   select alldata;
			//accounts.So
			// Finally, iterate though the list of grouped CustNo's matching to CustNo in the full BankAccount data
			// giving us ONLY the full records for any records that have > 1 Bank accounts
			//foreach ( var item1 in sel )
			//{
			//	foreach ( var item2 in accounts )
			//	{
			//		if ( item2 . CustNo . ToString ( ) == item1 . Key )
			//		{ output . Add ( item2 ); }
			//	}
			//}
			DetailsGrid . ItemsSource = accounts;
			StatusBar . Text = $"Filtering completed, {output . Count} Multi Account records match";
			BankCount . Text = $"{Utils . GetPrettyGridStatistics ( this . BankGrid , this . BankGrid . SelectedIndex )}";
			CustCount . Text = $"{Utils . GetPrettyGridStatistics ( this . CustomerGrid , this . CustomerGrid . SelectedIndex )}";
			DetCount . Text = $"{Utils . GetPrettyGridStatistics ( this . DetailsGrid , this . DetailsGrid . SelectedIndex )}";
		}

		#endregion LINQ queries

		private void Exit_Click ( object sender , RoutedEventArgs e )
		{
			Close ( );
		}

		private void Options_Click ( object sender , RoutedEventArgs e )
		{

		}

		private void Minimize_click ( object sender , RoutedEventArgs e )
		{
			this . WindowState = WindowState . Normal;
		}

		private void Window_MouseDown ( object sender , MouseButtonEventArgs e )
		{

		}

		private void LinkGrid_Click ( object sender , RoutedEventArgs e )
		{
			if ( LinkGrids . IsChecked == true )
				GridsLinked = true;
			else
				GridsLinked = false;

		}

		/// <summary>
		///  Event handkler for data changes made by EditDb viewers only
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void EventControl_DataUpdated ( object sender , LoadedEventArgs e )
		{
			// Update ALL datagrids - IF we didnt   truiigger the change
			if ( sender == SqlBankAccounts || sender == SqlCustAccounts || sender == SqlDetAccounts )
			{
				Mouse . OverrideCursor = Cursors . Arrow;
				return;
			}
			//			await Utils . DoBeep ( 600, 300, true );
			//			await Utils . DoSingleBeep ( 500, 100, 5 ) . ConfigureAwait ( false );

			RefreshAllGrids ( CurrentDb , e . RowCount , e . Custno , e . Bankno );
			Mouse . OverrideCursor = Cursors . Arrow;
			inprogress = false;
		}

		#region Data Edited event creators

		/// <summary>
		/// Method that is called when Bank grid has a data change made to it.
		/// It updates ALL the Db's first, then triggers a ViewerDataUpdated()  EVENT
		/// to notify any other open viewers
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void BankGrid_RowEditEnding ( object sender , DataGridRowEditEndingEventArgs e )
		{
			string SearchCustNo = "";
			string SearchBankNo = "";
			// Save current positions so we can reposition later
			inprogress = true;

			// Set globals
			bindex = this . BankGrid . SelectedIndex;

			BankAccountViewModel CurrentBankSelectedRecord = this . BankGrid . CurrentItem as BankAccountViewModel;
			if ( CurrentBankSelectedRecord == null )
			{
				//				Console . WriteLine ( $"\nBank Grid ERROR - Currentitem is NULL on Entry to Selectionchanged !!\n" );
				//				Utils . DoErrorBeep ( 200, 100, 2 );
				//					await Utils . DoBeep ( 300, 300 ) . ConfigureAwait ( false );
				inprogress = false;
				return;
			}
			SearchCustNo = CurrentBankSelectedRecord . CustNo;
			SearchBankNo = CurrentBankSelectedRecord . BankNo;
			bindex = this . BankGrid . SelectedIndex;
			// This does the SQL update of the record that has been changed
			UpdateOnDataChange ( CurrentDb , e );
			EventControl . TriggerMultiViewerDataUpdated ( SqlBankAccounts ,
				new LoadedEventArgs
				{
					CallerType = "MULTIVIEWER" ,
					Bankno = SearchBankNo ,
					Custno = SearchCustNo ,
					CallerDb = "BANKACCOUNT" ,
					SenderGuid = this . Tag . ToString ( ) ,
					DataSource = null ,
					RowCount = this . BankGrid . SelectedIndex
				} );
			IsEditing = false;
			SaveCurrentIndex ( 1 , BankGrid . SelectedIndex );

			Utils . DoSingleBeep ( 200 , 300 , 1 );
		}


		/// <summary>
		/// Method that is called when Customer grid has a data change made to it.
		/// It updates ALL the Db's first, then triggers a ViewerDataUpdated()  EVENT
		/// to notify any other open viewers
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CustGrid_RowEditEnding ( object sender , DataGridRowEditEndingEventArgs e )
		{
			string SearchCustNo = "";
			string SearchBankNo = "";
			// Save current positions so we can reposition later
			inprogress = true;

			// Set globals
			cindex = this . CustomerGrid . SelectedIndex;
			CustomerViewModel CurrentBankSelectedRecord = this . CustomerGrid . CurrentItem as CustomerViewModel;
			if ( CurrentBankSelectedRecord == null )
			{
				//				Console . WriteLine ( $"\nCustomer Grid ERROR - Currentitem is NULL on Entry to Selectionchanged !!\n" );
				//				Utils . DoErrorBeep ( 250, 100, 3 );
				//					await Utils . DoBeep ( 300, 300 ) . ConfigureAwait ( false );
				inprogress = false;
				return;
			}
			SearchCustNo = CurrentBankSelectedRecord . CustNo;
			SearchBankNo = CurrentBankSelectedRecord . BankNo;
			cindex = this . CustomerGrid . SelectedIndex;

			// This does the SQL update of the record that has been changed
			UpdateOnDataChange ( CurrentDb , e );
			//			EventControl.TriggerMultiViewerDataUpdated
			EventControl . TriggerMultiViewerDataUpdated (
				SqlCustAccounts ,
				new LoadedEventArgs
				{
					CallerType = "MULTIVIEWER" ,
					Bankno = SearchBankNo ,
					Custno = SearchCustNo ,
					CallerDb = "CUSTOMER" ,
					SenderGuid = this . Tag . ToString ( ) ,
					DataSource = SqlCustAccounts ,
					RowCount = this . CustomerGrid . SelectedIndex
				} );
			IsEditing = false;
			Utils . DoSingleBeep ( 300 , 300 , 2 );
			SaveCurrentIndex ( 2 , CustomerGrid . SelectedIndex );
			inprogress = false;
		}
		/// <summary>
		/// Method that is called when Details grid has a data change made to it.
		/// It updates ALL the Db's first, then triggers a ViewerDataUpdated()  EVENT
		/// to notify any other open viewers
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DetailsGrid_RowEditEnding ( object sender , DataGridRowEditEndingEventArgs e )
		{
			string SearchCustNo = "";
			string SearchBankNo = "";
			// Save current positions so we can reposition later
			inprogress = true;

			// Set globals
			//dindex = this . DetailsGrid . SelectedIndex;
			DetailsViewModel CurrentBankSelectedRecord = this . DetailsGrid . CurrentItem as DetailsViewModel;
			if ( CurrentBankSelectedRecord == null )
			{
				//				Console . WriteLine ( $"\nDetails Grid ERROR - Currentitem is NULL on Entry to Selectionchanged !!\n" );
				//				Utils . DoErrorBeep ( 300, 100 , 4);
				inprogress = false;
				return;
			}
			SearchCustNo = CurrentBankSelectedRecord . CustNo;
			SearchBankNo = CurrentBankSelectedRecord . BankNo;
			dindex = this . DetailsGrid . SelectedIndex;

			// This does the SQL update of the record that has been changed
			UpdateOnDataChange ( CurrentDb , e );
			EventControl . TriggerMultiViewerDataUpdated ( SqlDetAccounts ,
				new LoadedEventArgs
				{
					CallerType = "MULTIVIEWER" ,
					Bankno = SearchBankNo ,
					Custno = SearchCustNo ,
					CallerDb = "DETAILS" ,
					SenderGuid = this . Tag . ToString ( ) ,
					DataSource = SqlDetAccounts ,
					RowCount = this . DetailsGrid . SelectedIndex
				} );
			IsEditing = false;
			Utils . DoSingleBeep ( 400 , 300 , 3 );
			SaveCurrentIndex ( 3 , DetailsGrid . SelectedIndex );
			inprogress = false;
		}
		#endregion Data Edited event creators



		/// <summary>
		/// Main Event handler for data changes made in this Multi viewer
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void EventControl_ViewerDataUpdated ( object sender , LoadedEventArgs e )
		{
			// Update ALL datagrids - IF we didnt   truiigger the change
			if ( sender == SqlBankAccounts )// || sender == MultiCustcollection || sender == MultiDetcollection )
			{
				// Bank updated a row, so just update Customer and Details
				Flags . SqlCustActive = true;
				//				AllCustomers . LoadCust ( SqlCustAccounts , "MULTIVIEWER" , 3 , true );
				//				DetailCollection . LoadDet ( "MULTIVIEWER" , 3 , true );
				if ( Flags . USECOPYDATA )
				{
					SqlCustAccounts = DapperSupport . GetCustObsCollection ( collection: SqlCustAccounts ,
					DbNameToLoad: Flags . COPYCUSTDATANAME ,
					Caller: "MULTIVIEWER" ,
					Notify: true );
				}
				else
				{
					SqlCustAccounts = DapperSupport . GetCustObsCollection ( collection: SqlCustAccounts ,
						Caller: "MULTIVIEWER" ,
						Notify: true );
				}
				Flags . SqlDetActive = true;

				if ( Flags . USECOPYDATA )
				{
					DapperSupport . GetDetailsObsCollection ( SqlDetAccounts ,
					DbNameToLoad: Flags . COPYDETDATANAME ,
					wantSort: true ,
					wantDictionary: false ,
					Notify: true ,
					Caller: "MULTIVIEWER" );
				}
				else
				{
					DapperSupport . GetDetailsObsCollection ( SqlDetAccounts ,
					wantSort: true ,
					wantDictionary: false ,
					Notify: true ,
					Caller: "MULTIVIEWER" );
				}
				Mouse . OverrideCursor = Cursors . Arrow;
				inprogress = false;
				return;
			}
			else if ( sender == SqlCustAccounts )// || sender == MultiCustcollection || sender == MultiDetcollection )
			{
				// Customer updated a row, so just update Bank and Details
				Flags . SqlCustActive = true;
				//				BankCollection . LoadBank ( SqlBankAccounts , "MULTIVIEWER" , 3 , true );
				if ( Flags . USECOPYDATA )
				{
					DapperSupport . GetBankObsCollection ( collection: SqlBankAccounts ,
					DbNameToLoad: Flags . COPYBANKDATANAME ,
					wantSort: true ,
					Notify: true ,
					Caller: "SQLDBVIEWER" );
				}
				else
				{
					DapperSupport . GetBankObsCollection ( collection: SqlBankAccounts ,
						wantSort: true ,
						Notify: true ,
						Caller: "MULTIVIEWER" );
				}
				Flags . SqlDetActive = true;
				//				DetailCollection . LoadDet ( "MULTIVIEWER" , 3 , true );
				if ( Flags . USECOPYDATA )
				{
					DapperSupport . GetDetailsObsCollection ( SqlDetAccounts ,
					DbNameToLoad: Flags . COPYDETDATANAME ,
					wantSort: true ,
					wantDictionary: false ,
					Notify: true ,
					Caller: "MULTIVIEWER" );
				}
				else
				{
					DapperSupport . GetDetailsObsCollection ( SqlDetAccounts ,
					wantSort: true ,
					wantDictionary: false ,
					Notify: true ,
					Caller: "MULTIVIEWER" );
				}
				Mouse . OverrideCursor = Cursors . Arrow;
				inprogress = false;
				return;
			}
			else if ( sender == SqlDetAccounts )// || sender == MultiCustcollection || sender == MultiDetcollection )
			{
				// Details updated a row, so just update Customer and Bank
				Flags . SqlBankActive = true;
				//				BankCollection . LoadBank ( SqlBankAccounts , "MULTIVIEWER" , 3 , true );
				if ( Flags . USECOPYDATA )
				{
					DapperSupport . GetBankObsCollection ( collection: SqlBankAccounts ,
					DbNameToLoad: Flags . COPYBANKDATANAME ,
					wantSort: true ,
					Notify: true ,
					Caller: "MULTIVIEWER" );
				}
				else
				{
					DapperSupport . GetBankObsCollection ( collection: SqlBankAccounts ,
						wantSort: true ,
						Notify: true ,
						Caller: "MULTIVIEWER" );
				}
				Flags . SqlCustActive = true;
				//				AllCustomers . LoadCust ( SqlCustAccounts , "MULTIVIEWER" , 3 , true );
				if ( Flags . USECOPYDATA )
				{
					SqlCustAccounts = DapperSupport . GetCustObsCollection ( collection: SqlCustAccounts ,
					DbNameToLoad: Flags . COPYCUSTDATANAME ,
					Caller: "MULTIVIEWER" ,
					Notify: true );
				}
				else
				{
					SqlCustAccounts = DapperSupport . GetCustObsCollection ( collection: SqlCustAccounts ,
						Caller: "MULTIVIEWER" ,
						Notify: true );
				}
				Mouse . OverrideCursor = Cursors . Arrow;
				inprogress = false;
				return;
			}
			//RefreshAllGrids ( CurrentDb, e . RowCount, e . Custno, e . Bankno );
		}
		/// <summary>
		/// Main Event handler for data changes made in EXTERNAL multiviewers
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void EventControl_SqlViewerDataUpdated ( object sender , LoadedEventArgs e )
		{
			// Update ALL datagrids - IF we didnt   triigger the change

			if ( e . CallerDb == "MULTIVIEWER" )
				return;

			Flags . SqlBankActive = true;
			//			BankCollection . LoadBank ( SqlBankAccounts , "MULTIVIEWER" , 3 , true );
			if ( Flags . USECOPYDATA )
			{
				DapperSupport . GetBankObsCollection ( collection: SqlBankAccounts ,
					DbNameToLoad: Flags . COPYBANKDATANAME ,
				wantSort: true ,
				Notify: true ,
				Caller: "MULTIVIEWER" );
			}
			else
			{
				DapperSupport . GetBankObsCollection ( collection: SqlBankAccounts ,
					wantSort: true ,
					Notify: true ,
					Caller: "MULTIVIEWER" );
			}
			Flags . SqlCustActive = true;
			//			AllCustomers . LoadCust ( SqlCustAccounts , "MULTIVIEWER" , 3 , true );
			//			DetailCollection . LoadDet ( "MULTIVIEWER" , 3 , true );
			if ( Flags . USECOPYDATA )
			{
				SqlCustAccounts = DapperSupport . GetCustObsCollection ( collection: SqlCustAccounts ,
					DbNameToLoad: Flags . COPYCUSTDATANAME ,
				Caller: "MULTIVIEWER" ,
				Notify: true );
			}
			else
			{
				SqlCustAccounts = DapperSupport . GetCustObsCollection ( collection: SqlCustAccounts ,
					Caller: "MULTIVIEWER" ,
					Notify: true );
			}
			Flags . SqlDetActive = true;
			if ( Flags . USECOPYDATA )
			{
				DapperSupport . GetDetailsObsCollection ( SqlDetAccounts ,
					DbNameToLoad: Flags . COPYDETDATANAME ,
				wantSort: true ,
				wantDictionary: false ,
				Notify: true ,
				Caller: "MULTIVIEWER" );
			}
			else
			{
				DapperSupport . GetDetailsObsCollection ( SqlDetAccounts ,
				wantSort: true ,
				wantDictionary: false ,
				Notify: true ,
				Caller: "MULTIVIEWER" );
			}
			Mouse . OverrideCursor = Cursors . Arrow;
			inprogress = false;
			return;
		}

		/// <summary>
		/// Generic method to send Index changed Event trigger so that
		/// other viewers can update thier own grids as relevant
		/// </summary>
		/// <param name="grid"></param>
		//*************************************************************************************************************//
		public void TriggerMultiViewerIndexChanged ( DataGrid grid )
		{
			string SearchCustNo = "";
			string SearchBankNo = "";

			if ( LoadingDbData )
				return;
			if ( grid == this . BankGrid )
			{
				BankAccountViewModel CurrentBankSelectedRecord = this . BankGrid . SelectedItem as BankAccountViewModel;
				if ( CurrentBankSelectedRecord == null )
					return;
				SearchCustNo = CurrentBankSelectedRecord . CustNo;
				SearchBankNo = CurrentBankSelectedRecord . BankNo;
				EventControl . TriggerMultiViewerIndexChanged ( this ,
					new IndexChangedArgs
					{
						Senderviewer = this ,
						Bankno = SearchBankNo ,
						Custno = SearchCustNo ,
						dGrid = this . BankGrid ,
						Sender = "BANKACCOUNT" ,
						SenderId = "MULTIVIEWER" ,
						Row = this . BankGrid . SelectedIndex
					} );
			}
			else if ( grid == this . CustomerGrid )
			{
				CustomerViewModel CurrentCustSelectedRecord = this . CustomerGrid . CurrentItem as CustomerViewModel;
				if ( CurrentCustSelectedRecord == null )
					return;
				SearchCustNo = CurrentCustSelectedRecord . CustNo;
				SearchBankNo = CurrentCustSelectedRecord . BankNo;
				EventControl . TriggerMultiViewerIndexChanged ( this ,
				new IndexChangedArgs
				{
					Senderviewer = this ,
					Bankno = SearchBankNo ,
					Custno = SearchCustNo ,
					dGrid = this . CustomerGrid ,
					Sender = "CUSTOMER" ,
					SenderId = "MULTIVIEWER" ,
					Row = this . CustomerGrid . SelectedIndex
				} );
			}
			else if ( grid == this . DetailsGrid )
			{
				DetailsViewModel CurrentDetSelectedRecord = this . DetailsGrid . CurrentItem as DetailsViewModel;
				if ( CurrentDetSelectedRecord == null )
					return;
				SearchCustNo = CurrentDetSelectedRecord . CustNo;
				SearchBankNo = CurrentDetSelectedRecord . BankNo;
				EventControl . TriggerMultiViewerIndexChanged ( this ,
					new IndexChangedArgs
					{
						Senderviewer = this ,
						Bankno = SearchBankNo ,
						Custno = SearchCustNo ,
						dGrid = this . DetailsGrid ,
						Sender = "DETAILS" ,
						SenderId = "MULTIVIEWER" ,
						Row = this . DetailsGrid . SelectedIndex
					} );
			}
		}
		private void Window_PreviewKeyDown ( object sender , System . Windows . Input . KeyEventArgs e )
		{
			return;

			DataGrid dg = null;
			int CurrentRow = 0;
			bool showdebug = false;

			if ( showdebug )
				Debug . WriteLine ( $"key1 = {key1},  Key = : {e . Key}" );

			if ( IsEditing )
				return;

			if ( e . Key == Key . LeftCtrl )
			{
				key1 = true;
				if ( showdebug )
					Debug . WriteLine ( $"key1 = set to TRUE" );
				return;
			}
			//if ( key1 )
			//{
			//	Utils . HandleCtrlFnKeys ( key1, e );
			//	key1 = false;
			//}
			if ( key1 && e . Key == Key . F3 )  // CTRL + F3
			{
				// list MultiViewer static indexes
				Debug . WriteLine ( $"\nMultiViewer static indexes" );
				Debug . WriteLine ( $"bindex = {bindex}" );
				Debug . WriteLine ( $"cindex = {cindex}" );
				Debug . WriteLine ( $"dindex = {dindex}" );
				e . Handled = true;
				key1 = false;
				return;
			}
			else if ( key1 && e . Key == Key . F5 )
			{
				// list Flags in Console
				Utils . GetWindowHandles ( );
				e . Handled = true;
				key1 = false;
				return;
			}
			else if ( key1 && e . Key == Key . F8 )  // CTRL + F8
			{
				// list various Flags in Console
				Flags . PrintSundryVariables ( "Window_PreviewKeyDown()" );
				e . Handled = true;
				key1 = false;
				return;
			}
			else if ( key1 && e . Key == Key . F6 )  // CTRL + F6
			{
				// list various Flags in Console
				Flags . UseBeeps = !Flags . UseBeeps;
				e . Handled = true;
				key1 = false;
				return;
			}
			else if ( key1 && e . Key == Key . F7 )  // CTRL + F7
			{
				// list various Flags in Console
				Flags . PrintDbInfo ( );
				e . Handled = true;
				key1 = false;
				return;
			}
			else if ( key1 && e . Key == Key . F9 )    // CTRL + F9
			{
				// lists all delegates & Events
				Debug . WriteLine ( "\nEvent subscriptions " );
				EventHandlers . ShowSubscribersCount ( );
				e . Handled = true;
				return;
			}
			else if ( key1 && e . Key == Key . System )     // CTRL + F10
			{
				// Major  listof GV[] variables (Guids etc]
				if ( showdebug )
					Debug . WriteLine ( "\nGridview GV[] Variables" );
				Flags . ListGridviewControlFlags ( 1 );
				key1 = false;
				e . Handled = true;
				return;
			}
			else if ( key1 && e . Key == Key . F11 )
			{
				Debug . WriteLine ( "\nAll Flag. variables" );
				Flags . ShowAllFlags ( );
				key1 = false;
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
			else if ( e . Key == Key . Up )
			{       // DataGrid keyboard navigation = UP
				if ( CurrentDb == "BANKACCOUNT" )
					dg = this . BankGrid;
				else if ( CurrentDb == "CUSTOMER" )
					dg = this . CustomerGrid;
				else
					dg = this . DetailsGrid;
				if ( dg . SelectedIndex > 0 )
				{
					dg . SelectedIndex--;
					dg . SelectedItem = dg . SelectedIndex;
					if ( dg . SelectedItem != null )
						Utils . ScrollRecordInGrid ( dg , dg . SelectedIndex );
				}
				if ( dg == BankGrid )
					SaveCurrentIndex ( 1 , BankGrid . SelectedIndex );
				else if ( dg == CustomerGrid )
					SaveCurrentIndex ( 2 , CustomerGrid . SelectedIndex );
				else if ( dg == DetailsGrid )
					SaveCurrentIndex ( 3 , DetailsGrid . SelectedIndex );
				if ( dg . SelectedItem == null )
					Utils . ScrollRecordInGrid ( dg , 0 );
				e . Handled = true;
				key1 = false;
				return;
			}
			else if ( e . Key == Key . Down )
			{       // DataGrid keyboard navigation = DOWN
				if ( CurrentDb == "BANKACCOUNT" )
					dg = this . BankGrid;
				else if ( CurrentDb == "CUSTOMER" )
					dg = this . CustomerGrid;
				else if ( CurrentDb == "DETAILS" )
					dg = this . DetailsGrid;
				if ( dg . SelectedIndex < dg . Items . Count - 1 )
				{
					dg . SelectedIndex++;
					dg . SelectedItem = dg . SelectedIndex;
					if ( dg . SelectedItem != null )
						Utils . ScrollRecordInGrid ( dg , dg . SelectedIndex );
				}
				if ( dg == BankGrid )
					SaveCurrentIndex ( 1 , BankGrid . SelectedIndex );
				else if ( dg == CustomerGrid )
					SaveCurrentIndex ( 2 , CustomerGrid . SelectedIndex );
				else if ( dg == DetailsGrid )
					SaveCurrentIndex ( 3 , DetailsGrid . SelectedIndex );
				if ( dg . SelectedItem == null )
					Utils . ScrollRecordInGrid ( dg , 0 );

				e . Handled = true;
				key1 = false;
				return;
			}
			else if ( e . Key == Key . PageUp )
			{       // DataGrid keyboard navigation = PAGE UP
				if ( CurrentDb == "BANKACCOUNT" )
					dg = this . BankGrid;
				else if ( CurrentDb == "CUSTOMER" )
					dg = this . CustomerGrid;
				else
					dg = this . DetailsGrid;
				if ( dg . SelectedIndex >= 10 )
				{
					dg . SelectedIndex -= 10;
					if ( dg . SelectedItem != null )
						Utils . ScrollRecordInGrid ( dg , dg . SelectedIndex );
				}
				else
				{
					dg . SelectedIndex = 0;
					if ( dg . SelectedItem != null )
						Utils . ScrollRecordInGrid ( dg , dg . SelectedIndex );
				}
				if ( dg == BankGrid )
					SaveCurrentIndex ( 1 , BankGrid . SelectedIndex );
				else if ( dg == CustomerGrid )
					SaveCurrentIndex ( 2 , CustomerGrid . SelectedIndex );
				else if ( dg == DetailsGrid )
					SaveCurrentIndex ( 3 , DetailsGrid . SelectedIndex );
				if ( dg . SelectedItem != null )
					Utils . ScrollRecordInGrid ( dg , dg . SelectedIndex );
				e . Handled = true;
				key1 = false;
				return;
			}
			else if ( e . Key == Key . PageDown )
			{       // DataGrid keyboard navigation = PAGE DOWN
				if ( CurrentDb == "BANKACCOUNT" )
					dg = this . BankGrid;
				else if ( CurrentDb == "CUSTOMER" )
					dg = this . CustomerGrid;
				else
					dg = this . DetailsGrid;
				if ( dg . SelectedIndex < dg . Items . Count - 10 )
				{
					dg . SelectedIndex += 10;
					if ( dg . SelectedItem != null )
						Utils . ScrollRecordInGrid ( dg , dg . SelectedIndex );
				}
				else
				{
					dg . SelectedIndex = dg . Items . Count - 1;
					if ( dg . SelectedItem != null )
						Utils . ScrollRecordInGrid ( dg , dg . SelectedIndex );
				}
				if ( dg == BankGrid )
					SaveCurrentIndex ( 1 , BankGrid . SelectedIndex );
				else if ( dg == CustomerGrid )
					SaveCurrentIndex ( 2 , CustomerGrid . SelectedIndex );
				else if ( dg == DetailsGrid )
					SaveCurrentIndex ( 3 , DetailsGrid . SelectedIndex );
				if ( dg . SelectedItem != null )
					Utils . ScrollRecordInGrid ( dg , dg . SelectedIndex );
				e . Handled = true;
				key1 = false;
				return;
			}
			else if ( e . Key == Key . Home )
			{       // DataGrid keyboard navigation = HOME
				if ( CurrentDb == "BANKACCOUNT" )
					dg = this . BankGrid;
				else if ( CurrentDb == "CUSTOMER" )
					dg = this . CustomerGrid;
				else
					dg = this . DetailsGrid;
				dg . SelectedIndex = 0;
				if ( dg . SelectedItem != null )
					Utils . ScrollRecordInGrid ( dg , dg . SelectedIndex );
				if ( dg . SelectedItem != null )
					Utils . ScrollRecordInGrid ( dg , dg . SelectedIndex );
				if ( dg == BankGrid )
					SaveCurrentIndex ( 1 , BankGrid . SelectedIndex );
				else if ( dg == CustomerGrid )
					SaveCurrentIndex ( 2 , CustomerGrid . SelectedIndex );
				else if ( dg == DetailsGrid )
					SaveCurrentIndex ( 3 , DetailsGrid . SelectedIndex );
				e . Handled = true;
				key1 = false;
				return;
			}
			else if ( e . Key == Key . End )
			{       // DataGrid keyboard navigation = END
				if ( CurrentDb == "BANKACCOUNT" )
					dg = this . BankGrid;
				else if ( CurrentDb == "CUSTOMER" )
					dg = this . CustomerGrid;
				else
					dg = this . DetailsGrid;
				dg . SelectedIndex = dg . Items . Count - 1;
				if ( dg . SelectedItem != null )
					Utils . ScrollRecordInGrid ( dg , dg . SelectedIndex );
				if ( dg . SelectedItem != null )
					Utils . ScrollRecordInGrid ( dg , dg . SelectedIndex );
				if ( dg == BankGrid )
					SaveCurrentIndex ( 1 , BankGrid . SelectedIndex );
				else if ( dg == CustomerGrid )
					SaveCurrentIndex ( 2 , CustomerGrid . SelectedIndex );
				else if ( dg == DetailsGrid )
					SaveCurrentIndex ( 3 , DetailsGrid . SelectedIndex );
				e . Handled = true;
				key1 = false;
				key1 = false;
				return;
			}
			else if ( e . Key == Key . Delete )
			{       // DataGrid keyboard navigation = DELETE
				  // This is a ONE SHOT PASS, In here The selected Record will be deleted from the Db's on disk
				  // After this  the Event callback should handle the update of this viewer + all/any other open viewers
				  //				int currentindex = 0;
				string bank = "";
				string cust = "";
				var v = e . OriginalSource . GetType ( );
				MultiViewer Thisviewer = this;

				// Check to see if Del was pressed while Editing a field (in ANY of our grids)
				// if we have pressed it with just a Row selected, it will return "DataGridCell"  in v.Name
				// else it will have cell info in it
				if ( v . Name != "DataGridCell" )
				{
					e . Handled = false;
					return;         //NOT a Row that is selected, so let OS handle it normally
				}

				// Just ONE of these will call the delete process
				if ( CurrentDb == "BANKACCOUNT" )
				{
					dg = Flags . SqlBankGrid;
					CurrentRow = dg . SelectedIndex;
					// Get and save the data in the row so we have access to it once it has gone from interface
					BankAccountViewModel BankRecord = this . BankGrid . SelectedItem as BankAccountViewModel;
					bank = BankRecord . BankNo;
					cust = BankRecord . CustNo;
					dg . ItemsSource = null;
					e . Handled = true;
					key1 = false;

					// Call the method to update any other Viewers that may be open
					//					EventControl . TriggerRecordDeleted ( CurrentDb, bank, cust, CurrentRow );
					EventControl . TriggerRecordDeleted ( this , new LoadedEventArgs
					{
						Bankno = bank ,
						Custno = cust ,
						CallerDb = "BANKACCOUNT" ,
						CurrSelection = CurrentRow ,
						SenderGuid = this . Tag . ToString ( ) ,
						DataSource = SqlBankAccounts ,
						RowCount = CurrentRow
					} );
					// Keep our focus in originating window for now
					Thisviewer . Activate ( );
					Thisviewer . Focus ( );
					return;
				}
				if ( CurrentDb == "CUSTOMER" )
				{
					dg = Flags . SqlCustGrid;
					CurrentRow = dg . SelectedIndex;
					// Get and save the data in the row so we have access to it once it has gone from interface
					CustomerViewModel CustRecord = this . CustomerGrid . SelectedItem as CustomerViewModel;
					bank = CustRecord . BankNo;
					cust = CustRecord . CustNo;
					dg . ItemsSource = null;
					AllCustomers . dtCust?.Clear ( );
					e . Handled = true;
					key1 = false;
					// Call the method to update any other Viewers that may be open
					//					EventControl . TriggerRecordDeleted ( CurrentDb, bank, cust, CurrentRow );
					EventControl . TriggerRecordDeleted ( this , new LoadedEventArgs
					{
						Bankno = bank ,
						Custno = cust ,
						CallerDb = "CUSTOMER" ,
						CurrSelection = CurrentRow ,
						SenderGuid = this . Tag . ToString ( ) ,
						DataSource = SqlCustAccounts ,
						RowCount = CurrentRow
					} );
					// Keep our focus in originating window for now
					Thisviewer . Activate ( );
					Thisviewer . Focus ( );
					return;
				}
				else if ( CurrentDb == "DETAILS" )
				{
					dg = Flags . SqlDetGrid;
					// Get and save the data in the row so we have access to it once it has gone from interface
					DetailsViewModel DetailsRecord = this . DetailsGrid . SelectedItem as DetailsViewModel;
					bank = DetailsRecord . BankNo;
					cust = DetailsRecord . CustNo;
					CurrentRow = dg . SelectedIndex;
					// Remove it form THIS DataGrid here
					dg . ItemsSource = null;
					e . Handled = true;
					key1 = false;

					// Call the method to update any other Viewers that may be open
					//					EventControl . TriggerRecordDeleted ( CurrentDb, bank, cust, CurrentRow );
					EventControl . TriggerRecordDeleted ( this , new LoadedEventArgs
					{
						Bankno = bank ,
						Custno = cust ,
						CallerDb = "DETAILS" ,
						CurrSelection = CurrentRow ,
						SenderGuid = this . Tag . ToString ( ) ,
						DataSource = SqlDetAccounts ,
						RowCount = CurrentRow
					} );
					// Keep our focus in originating window for now
					Thisviewer . Activate ( );
					Thisviewer . Focus ( );
					return;
				}
				e . Handled = false;
				// Tidy up our own grid after ourselves
				if ( dg . Items . Count > 0 && CurrentRow >= 0 )
					dg . SelectedIndex = CurrentRow;
				else if ( dg . Items . Count == 1 )
					dg . SelectedIndex = 0;

				//dg.SelectedIndex = Flags.
				dg . SelectedItem = dg . SelectedIndex;
				if ( dg . SelectedItem != null )
					Utils . ScrollRecordInGrid ( dg , dg . SelectedIndex );

			}
			e . Handled = false;
		}

		private async void BankGrid_PreviewMouseRightButtonDown ( object sender , MouseButtonEventArgs e )
		{
			ContextMenu cm = this . FindResource ( "ContextMenu1" ) as ContextMenu;
			cm . PlacementTarget = this . BankGrid as DataGrid;
			cm . IsOpen = true;
			CurrentDb = "BANKACCOUNT";


			//			if ( e . ChangedButton == MouseButton . Right )
			//			{
			//				DataGridRow RowData = new DataGridRow ( );
			//				int row = DataGridSupport . GetDataGridRowFromTree ( e, out RowData );
			//				if ( row == -1 ) row = 0;
			//				RowInfoPopup rip = new RowInfoPopup ( "BANKACCOUNT", BankGrid);
			//				rip . Topmost = true;
			//				rip . DataContext = RowData;
			//				rip . BringIntoView ( );
			//				rip . Focus ( );
			//				rip . ShowDialog ( );

			//				//If data has been changed, update everywhere
			//				// Update the row on return in case it has been changed
			//				if ( rip . IsDirty )
			//				{
			//					this . BankGrid . ItemsSource = null;
			//					this . BankGrid . Items . Clear ( );
			//					SqlBankAccounts = BankCollection . LoadBank ( SqlBankAccounts, "MULTIVIEWER", 1, true );
			////					this . BankGrid . ItemsSource = MultiBankcollection;
			//					//					StatusBar . Text = "Current Record Updated Successfully...";
			//					// Notify everyone else of the data change
			//					EventControl . TriggerViewerDataUpdated ( SqlBankAccounts,
			//						new LoadedEventArgs
			//						{
			//							CallerType = "MULTIVIEWER",
			//							CallerDb = "BANKACCOUNT",
			//							DataSource = SqlBankAccounts,
			//							RowCount = this . BankGrid . SelectedIndex
			//						} );
			//				}
			//				else
			//					this . BankGrid . SelectedItem = RowData . Item;

			//				// This sets up the selected Index/Item and scrollintoview in one easy FUNC function call (GridInitialSetup is  the FUNC name)
			//				//Utils . SetUpGridSelection ( this . BankGrid, row );
			//				//// This is essential to get selection activated again
			//				this . BankGrid . Focus ( );
			//			}
		}

		private async void CustGrid_PreviewMouseRightButtonDown ( object sender , MouseButtonEventArgs e )
		{
			ContextMenu cm = this . FindResource ( "ContextMenu1" ) as ContextMenu;
			cm . PlacementTarget = this . CustomerGrid as DataGrid;
			cm . IsOpen = true;
			CurrentDb = "CUSTOMER";
			//			if ( e . ChangedButton == MouseButton . Right )
			//			{
			//				DataGridRow RowData = new DataGridRow ( );
			//				int row = DataGridSupport . GetDataGridRowFromTree ( e, out RowData );
			//				if ( row == -1 ) row = 0;
			//				RowInfoPopup rip = new RowInfoPopup ( "CUSTOMER", CustomerGrid);
			//				rip . Topmost = true;
			//				rip . DataContext = RowData;
			//				rip . BringIntoView ( );
			//				rip . Focus ( );
			//				rip . ShowDialog ( );

			//				//If data has been changed, update everywhere
			//				// Update the row on return in case it has been changed
			//				if ( rip . IsDirty )
			//				{
			//					this . CustomerGrid . ItemsSource = null;
			//					this . CustomerGrid . Items . Clear ( );
			//					SqlCustAccounts = await CustCollection . LoadCust ( SqlCustAccounts, "MULTIVIEWER", 1, true );
			////					this . CustomerGrid . ItemsSource = MultiCustcollection;
			//					//					StatusBar . Text = "Current Record Updated Successfully...";
			//					// Notify everyone else of the data change
			//					EventControl . TriggerViewerDataUpdated ( SqlCustAccounts,
			//						new LoadedEventArgs
			//						{
			//							CallerType = "MULTIVIEWER",
			//							CallerDb = "CUSTOMER",
			//							DataSource = SqlCustAccounts,
			//							RowCount = this . CustomerGrid . SelectedIndex
			//						} );
			//				}
			//				else
			//					this . CustomerGrid . SelectedItem = RowData . Item;

			//				// This sets up the selected Index/Item and scrollintoview in one easy FUNC function call (GridInitialSetup is  the FUNC name)
			////				Utils . SetUpGridSelection ( this . CustomerGrid, row );
			//				// This is essential to get selection activated again
			//				this . CustomerGrid . Focus ( );
			//			}
		}

		private async void DetGrid_PreviewMouseRightButtonDown ( object sender , MouseButtonEventArgs e )
		{
			ContextMenu cm = this . FindResource ( "ContextMenu1" ) as ContextMenu;
			cm . PlacementTarget = this . DetailsGrid as DataGrid;
			cm . IsOpen = true;
			CurrentDb = "DETAILS";
			//			if ( e . ChangedButton == MouseButton . Right )
			//			{
			//				DataGridRow RowData = new DataGridRow ( );
			//				int row = DataGridSupport . GetDataGridRowFromTree ( e, out RowData );
			//				if ( row == -1 ) row = 0;
			//				RowInfoPopup rip = new RowInfoPopup ( "DETAILS", DetailsGrid);
			//				rip . Topmost = true;
			//				rip . DataContext = RowData;
			//				rip . BringIntoView ( );
			//				rip . Focus ( );
			//				rip . ShowDialog ( );

			//				//If data has been changed, update everywhere
			//				// Update the row on return in case it has been changed
			//				if ( rip . IsDirty )
			//				{
			//					this . DetailsGrid . ItemsSource = null;
			//					this . DetailsGrid . Items . Clear ( );
			//					SqlDetAccounts = await DetCollection . LoadDet ( SqlDetAccounts, 1, true );
			////					this . DetailsGrid . ItemsSource = MultiDetcollection;
			//					//					StatusBar . Text = "Current Record Updated Successfully...";
			//					// Notify everyone else of the data change
			//					EventControl . TriggerViewerDataUpdated ( SqlDetAccounts,
			//						new LoadedEventArgs
			//						{
			//							CallerType = "MULTIVIEWER",
			//							CallerDb = "DETAILS",
			//							DataSource = SqlDetAccounts,
			//							RowCount = this . DetailsGrid . SelectedIndex
			//						} );
			//				}
			//				else
			//					this . DetailsGrid . SelectedItem = RowData . Item;

			//				// This sets up the selected Index/Item and scrollintoview in one easy FUNC function call (GridInitialSetup is  the FUNC name)
			////				Utils . SetUpGridSelection ( this . DetailsGrid, row );
			//				// This is essential to get selection activated again
			//				this . DetailsGrid . Focus ( );
			//			}
		}

		private void BankGrid_BeginningEdit ( object sender , DataGridBeginningEditEventArgs e )
		{
			IsEditing = true;
		}

		private void CustomerGrid_BeginningEdit ( object sender , DataGridBeginningEditEventArgs e )
		{
			IsEditing = true;

		}

		private void DetailsGrid_BeginningEdit ( object sender , DataGridBeginningEditEventArgs e )
		{
			IsEditing = true;
		}
		/// <summary>
		/// Savres the current selectedIndex for each grid
		/// </summary>
		/// <param name="type"></param>
		/// <param name="index"></param>
		private void SaveCurrentIndex ( int type , int index )
		{
			if ( type == 1 )
				bindex = index;
			if ( type == 2 )
				cindex = index;
			if ( type == 3 )
				dindex = index;
		}

		private void ExportBankCSV_Click ( object sender , RoutedEventArgs e )
		{

		}

		private void ExportCustCSV_Click ( object sender , RoutedEventArgs e )
		{

		}

		private void ExportDetCSV_Click ( object sender , RoutedEventArgs e )
		{

		}

		private void ImportBankCSV_Click ( object sender , RoutedEventArgs e )
		{

		}

		private void ImportCustCSV_Click ( object sender , RoutedEventArgs e )
		{

		}

		private void ImportDetCSV_Click ( object sender , RoutedEventArgs e )
		{

		}
		private void BankGrid_PreviewDragEnter ( object sender , DragEventArgs e )
		{
			e . Effects = ( DragDropEffects ) DragDropEffects . Move;
			Debug . WriteLine ( $"Setting drag cursor...." );

		}
		private void Grids_PreviewMouseLeftButtondown ( object sender , MouseButtonEventArgs e )
		{
			// Gotta make sure it is not anywhere in the Scrollbar we clicked on 
			if ( Utils . HitTestScrollBar ( sender , e ) )
			{ ScrollBarMouseMove = true; return; }
			//			if ( Utils . HitTestHeaderBar ( sender , e ) )
			//				return;
			_startPoint = e . GetPosition ( null );
			// Make sure the left mouse button is pressed down so we are really moving a record
			if ( e . LeftButton == MouseButtonState . Pressed )
			{
				IsLeftButtonDown = true;
				this . Focus ( );
			}
		}
		private void Grids_PreviewMouseLeftButtonup ( object sender , MouseButtonEventArgs e )
		{
			ScrollBarMouseMove = false;
		}

		private void Drag_Click ( object sender , RoutedEventArgs e )
		{
			DragDropClient ddc = new DragDropClient ( );
			ddc . Show ( );
		}

		private void BankGrid_PreviewMouseMove ( object sender , MouseEventArgs e )
		{
			Point mousePos = e . GetPosition ( null );
			Vector diff = _startPoint - mousePos;

			if ( e . LeftButton == MouseButtonState . Pressed &&
			    Math . Abs ( diff . X ) > SystemParameters . MinimumHorizontalDragDistance ||
			    Math . Abs ( diff . Y ) > SystemParameters . MinimumVerticalDragDistance )
			{
				// Make sure the left mouse button is pressed down so we are really moving a record
				if ( e . LeftButton == MouseButtonState . Pressed )
				{
					if ( ScrollBarMouseMove )
					{
						return;
					}

					if ( BankGrid . SelectedItem != null )
					{
						// We are dragging from the BANK grid
						//Working string version
						BankAccountViewModel bvm = new BankAccountViewModel ( );
						bvm = BankGrid . SelectedItem as BankAccountViewModel;
						string str = GetExportRecords . CreateTextFromRecord ( bvm, null, null, true, false );
						string dataFormat = DataFormats . Text;
						DataObject dataObject = new DataObject ( dataFormat, str );
						System . Windows . DragDrop . DoDragDrop (
						BankGrid ,
						dataObject ,
						DragDropEffects . Move );
					}
				}
			}
		}

		private void CustomerGrid_PreviewMouseMove ( object sender , MouseEventArgs e )
		{
			Point mousePos = e.GetPosition(null);
			Vector diff = _startPoint - mousePos;

			if ( e . LeftButton == MouseButtonState . Pressed &&
			    Math . Abs ( diff . X ) > SystemParameters . MinimumHorizontalDragDistance ||
			    Math . Abs ( diff . Y ) > SystemParameters . MinimumVerticalDragDistance )
			{
				// Make sure the left mouse button is pressed down so we are really moving a record
				if ( e . LeftButton == MouseButtonState . Pressed )
				{
					if ( CustomerGrid . SelectedItem != null )
					{
						if ( ScrollBarMouseMove )
						{
							return;
						}
						// We are dragging from the Customer grid
						//Working string version
						CustomerViewModel cvm = new CustomerViewModel();
						cvm = CustomerGrid . SelectedItem as CustomerViewModel;
						string str = GetExportRecords.CreateTextFromRecord(null, null, cvm, true, false);
						string dataFormat = DataFormats.Text;
						DataObject dataObject = new DataObject(dataFormat, str);
						System . Windows . DragDrop . DoDragDrop (
						CustomerGrid ,
						dataObject ,
						DragDropEffects . Move );
					}
				}
			}
		}

		private void DetailsGrid_PreviewMouseMove ( object sender , MouseEventArgs e )
		{
			Point mousePos = e.GetPosition(null);
			Vector diff = _startPoint - mousePos;

			if ( e . LeftButton == MouseButtonState . Pressed &&
			    Math . Abs ( diff . X ) > SystemParameters . MinimumHorizontalDragDistance ||
			    Math . Abs ( diff . Y ) > SystemParameters . MinimumVerticalDragDistance )
			{
				// Make sure the left mouse button is pressed down so we are really moving a record
				if ( e . LeftButton == MouseButtonState . Pressed )
				{
					if ( DetailsGrid . SelectedItem != null )
					{
						if ( ScrollBarMouseMove )
						{
							return;
						}
						// We are dragging from the DETAILS grid
						//Working string version
						DetailsViewModel dvm = new DetailsViewModel();
						dvm = DetailsGrid . SelectedItem as DetailsViewModel;
						string str = GetExportRecords.CreateTextFromRecord(null, dvm, null, true, false);
						string dataFormat = DataFormats.Text;
						DataObject dataObject = new DataObject(dataFormat, str);
						System . Windows . DragDrop . DoDragDrop (
						BankGrid ,
						dataObject ,
						DragDropEffects . Move );
					}
				}
			}
		}

		private void ContextEdit_Click ( object sender , RoutedEventArgs e )
		{
			RowInfoPopup rip = ( RowInfoPopup ) null;
			// handle flags to let us know WE have triggered the selectedIndex change
			//			MainWindow . DgControl . SelectionChangeInitiator = 2; // tells us it is a EditDb initiated the record change
			DataGridRow RowData = new DataGridRow ( );
			if ( CurrentDb == "BANKACCOUNT" )
			{
				BankAccountViewModel bvm = new BankAccountViewModel ( );
				bvm = this . BankGrid . SelectedItem as BankAccountViewModel;
				rip = new RowInfoPopup ( "BANKACCOUNT" , BankGrid );
				rip . DataContext = bvm;
			}
			else if ( CurrentDb == "CUSTOMER" )
			{
				CustomerViewModel cvm = new CustomerViewModel ( );
				cvm = this . CustomerGrid . SelectedItem as CustomerViewModel;
				rip = new RowInfoPopup ( "CUSTOMER" , CustomerGrid );
				rip . DataContext = cvm;
			}
			else if ( CurrentDb == "DETAILS" )
			{
				DetailsViewModel dvm = new DetailsViewModel ( );
				dvm = this . DetailsGrid . SelectedItem as DetailsViewModel;
				rip = new RowInfoPopup ( "DETAILS" , DetailsGrid );
				rip . DataContext = dvm;
			}
			rip . Topmost = true;
			rip . DataContext = RowData;
			rip . BringIntoView ( );
			rip . Focus ( );
			rip . ShowDialog ( );

			//If data has been changed, update everywhere
			// Update the row on return in case it has been changed
			//if ( rip . IsDirty )
			//{
			//	this . BankGrid . ItemsSource = null;
			//	this . BankGrid . Items . Clear ( );
			//	// Save our reserve collection
			//	BankReserved = null;

			//	BankCollection . LoadBank ( SqlBankcollection, "SQLDBVIEWER", 1, true );
			//	this . BankGrid . ItemsSource = SqlBankcollection;
			//	StatusBar . Text = "Current Record Updated Successfully...";
			//	// Notify everyone else of the data change
			//	EventControl . TriggerViewerDataUpdated ( SqlBankcollection,
			//		new LoadedEventArgs
			//		{
			//			CallerType = "SQLDBVIEWER",
			//			CallerDb = "BANKACCOUNT",
			//			DataSource = SqlBankcollection,
			//			RowCount = this . BankGrid . SelectedIndex
			//		} );
			//}
			//else
			//	this . BankGrid . SelectedItem = RowData . Item;

			// This sets up the selected Index/Item and scrollintoview in one easy FUNC function call (GridInitialSetup is  the FUNC name)
			Utils . SetUpGridSelection ( this . BankGrid , this . BankGrid . SelectedIndex );
			//ParseButtonText ( true );
			//Count . Text = $"{Utils . GetPrettyGridStatistics ( this . BankGrid, this . BankGrid . SelectedIndex )}";
			//				Count . Text = $"{this . BankGrid . SelectedIndex} / { this . BankGrid . Items . Count . ToString ( )}";
			//				Count . Text = this . BankGrid . Items . Count . ToString ( );
			// This is essential to get selection activated again
			this . BankGrid . Focus ( );
		}

		private void ContextSave_Click ( object sender , RoutedEventArgs e )
		{
			string path = "";
			if ( CurrentDb == "BANKACCOUNT" )
			{
				path = @"C:\\Users\\Ianch\\Documents\\BankCollectiondata.json";
				JsonSupport . JsonSerialize ( SqlBankAccounts , path );
				MessageBox . Show ( $"The Db data has been saved successfully in 'Json' format ...\n\nFile is : [{path}]" , "Data Persistence System" );
			}
			else if ( CurrentDb == "CUSTOMER" )
			{
				path = @"C:\\Users\\Ianch\\Documents\\CustCollectiondata.json";
				JsonSupport . JsonSerialize ( SqlCustAccounts , path );
				MessageBox . Show ( $"The Db data has been saved successfully in 'Json' format ...\n\nFile is : [{path}]" , "Data Persistence System" );
			}
			else if ( CurrentDb == "DETAILS" )
			{
				path = @"C:\\Users\\Ianch\\Documents\\DetCollectiondata.json";
				JsonSupport . JsonSerialize ( SqlDetAccounts , path );
				MessageBox . Show ( $"The Db data  has been saved successfully in 'Json' format ...\n\nFile is : [{path}]" , "Data Persistence System" );
			}
		}

		private void ContextShowJson_Click ( object sender , RoutedEventArgs e )
		{
			if ( CurrentDb == "BANKACCOUNT" )
			{
				// Works fine !!! 22/6/21
				// grab current record and  convert it to a Json record
				BankAccountViewModel bvm = new BankAccountViewModel ( );
				bvm = this . BankGrid . SelectedItem as BankAccountViewModel;
				JObject obj = JObject . FromObject ( bvm );
				string s = JsonConvert . SerializeObject ( new { obj } );
				// we have our string in 's'
				// // show it in a messagebox fully formatted				
				//				int rows = 0;
				MessageBox . Show ( JsonSupport . CreateFormattedJsonOutput ( s , "BankAccount" ) . ToString ( ) , "Json formatted record data" );
			}
			else if ( CurrentDb == "CUSTOMER" )
			{
				CustomerViewModel cvm = new CustomerViewModel ( );
				cvm = this . CustomerGrid . SelectedItem as CustomerViewModel;
				JObject obj = JObject . FromObject ( cvm );
				string s = JsonConvert . SerializeObject ( new { obj } );
				// we have our string in 's'
				// // show it in a messagebox fully formatted				
				//				int rows = 0;
				MessageBox . Show ( JsonSupport . CreateFormattedJsonOutput ( s , "Customer" ) . ToString ( ) , "Json formatted record data" );
			}
			else if ( CurrentDb == "DETAILS" )
			{
				DetailsViewModel dvm = new DetailsViewModel ( );
				dvm = this . DetailsGrid . SelectedItem as DetailsViewModel;
				JObject obj = JObject . FromObject ( dvm );
				string s = JsonConvert . SerializeObject ( new { obj } );
				// we have our string in 's'
				// // show it in a messagebox fully formatted				
				//				int rows = 0;
				MessageBox . Show ( JsonSupport . CreateFormattedJsonOutput ( s , "Details" ) . ToString ( ) , "Json formatted record data" );
			}
		}
		private void ContextDisplayJsonData_Click ( object sender , RoutedEventArgs e )
		{
			//============================================//
			//MENU ITEM 'Read and display JSON File'
			//============================================//
			object DbData = new object ( );
			//			string path = "";
			//			string jsonresult = "";

			Progressbar pbar = new Progressbar ( );
			pbar . Show ( );
			Mouse . OverrideCursor = Cursors . Wait;
			StatusBar . Text = "Please wait, This process can take a little while !!";
			this . Refresh ( );
			//We need to save current Collectionview as a Json (binary) data to disk
			// this is the best way to save persistent data in Json format
			//using tmp folder for interim file that we will then display

			if ( CurrentDb == "BANKACCOUNT" )
				JsonSupport . CreateShowJsonText ( false , CurrentDb , SqlBankAccounts );
			else if ( CurrentDb == "CUSTOMER" )
				JsonSupport . CreateShowJsonText ( false , CurrentDb , SqlCustAccounts );
			else if ( CurrentDb == "DETAILS" )
				JsonSupport . CreateShowJsonText ( false , CurrentDb , SqlDetAccounts );
		}

		private async void ContextCreateJsonOutput_Click ( object sender , RoutedEventArgs e )
		{
			int rows = 0;
			//			Stopwatch sw = new Stopwatch ( );
			//Read the data from disk file BankCollectiondata.json & then
			// parse  it out into traditional JSON format for viewing
			// Takes a while though !!! about 20 seconds for 5000 records
			//			JsonReader reader;
			BankAccountViewModel bvm = new BankAccountViewModel ( );
			//Read Json (binary) data from disk
			object obj = JsonSupport . JsonDeserialize ( @"C:\\users\ianch\documents\BankCollectiondata.json " );
			string Output = "", jsonstring = "";
			//			StringBuilder sb = Utils . CreateJsonFileFromJsonObject ( obj, out Output );
			Debug . WriteLine ( $"\nStarting creation of JSON format file of database ..." );
			//			sw . Start ( );
			jsonstring = JsonSupport . CreateFormattedJsonOutput ( Output , "Bank" );
			//jsonstring = tmp . ToString ( );
			//			sw . Stop ( );
			//			Debug . WriteLine ( $"Completed creation of JSON format file of database - {rows} were createdin {( double ) sw . ElapsedMilliseconds / ( double ) 1000} seconds...\n" );
			// Read string data back
			// We now have a formatted JSON style output buffer :- jsonstring
			// Save it to disk so wecan display it in Wordpad or whatever is chosen ?
			string path = @"C:\users\ianch\Documents\Formatteddata.json";
			File . WriteAllText ( path , jsonstring );
			try
			{
				//Setup our delegate
				QualifyingFileLocations FindPathHandler = SupportMethods . qualifiers;
				// pass the delegate method thru to our search for executable path method
				// It contains all the specialist paths we want to have searched
				// WORKS VERY WELL 15/6/21
				string test = SupportMethods . FindExecutePath ( $"Notepad.exe jsonstring", SupportMethods . qualifiers );
			}
			catch ( Exception ex )
			{
				Debug . WriteLine ( $"Failure in Utils.FindExecutePath()\n{ex . Message}, {ex . Data}" );
				//return false;
			}
			Process p = null;
			// Show the traditional JSON output we have just created
			p = Process . Start ( "Wordpad.exe" , path );
		}

		private void ContextClose_Click ( object sender , RoutedEventArgs e )
		{
			this . Close ( );
		}

		private void ContextSettings_Click ( object sender , RoutedEventArgs e )
		{
			Setup setup = new Setup ( );
			setup . Show ( );
			setup . BringIntoView ( );
			setup . Topmost = true;
			this . Focus ( );
		}
		public struct scrollData
		{
			public double Banktop { get; set; }
			public double Bankbottom { get; set; }
			public double BankVisible { get; set; }
			public double Custtop { get; set; }
			public double Custbottom { get; set; }
			public double CustVisible { get; set; }
			public double Dettop { get; set; }
			public double Detbottom { get; set; }
			public double DetVisible { get; set; }
		}
		public scrollData ScrollData = new scrollData ( );

		private void Columns_SelectionChanged ( object sender , SelectionChangedEventArgs e )
		{
			ListBox lb = sender as ListBox;
			var  Content = lb . SelectedItem;
			var selection = int.Parse(Content.ToString());
			if ( selection >= 0 && selection <= 2 )
			{
				//					int[] sortorder = { 2,3,1,5,4,7,6,0};
				DataGridSupport . SortBankColumns ( BankGrid , DGBankColumnsCollection , selection );
				BankGrid . Refresh ( );
				DataGridSupport . SortCustomerColumns ( CustomerGrid , DGCustColumnsCollection , selection );
				CustomerGrid . Refresh ( );
				DataGridSupport . SortDetailsColumns ( DetailsGrid , DGDetailsColumnsCollection , selection );
				DetailsGrid . Refresh ( );
			}
		}
	}
}
