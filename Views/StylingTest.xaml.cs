using System;
using System . Collections . ObjectModel;
using System . ComponentModel;
using System . Linq;
using System . Threading;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Data;
using System . Windows . Input;
using System . Windows . Media;
using System . Windows . Media . Animation;

using WPFPages . Commands;
using WPFPages . ViewModels;

namespace WPFPages . Views
{
	public partial class Stylingtest : Window
	{
		private bool DataSwitched = false;
		private bool ScrollBarMouseMove = false;
		public nworder nwOrder = new nworder ( );
		private bool  IsLeftButtonDown = false;
		private Point _startPoint =new Point();
		private int GridSelection=0;
		private NwOrderCollection NwOrders = new NwOrderCollection ( );
		private NwOrderCollection NwOrder1 = new NwOrderCollection ( );

		private CollectionView orderview1 = new CollectionView ( "" );
		private ObservableCollection<nwcustomer> NwCustomers = new ObservableCollection<nwcustomer> ( );
		private ObservableCollection<nwcustomer> NwCustomers1 = new ObservableCollection<nwcustomer> ( );
		private ObservableCollection<nwcustomer> NwCustomers3 = new ObservableCollection<nwcustomer> ( );

		private ObservableCollection<nwcustomer> NwCustomers6 = new ObservableCollection<nwcustomer> ( );
		ObservableCollection<CustomerViewModel> Customers1 = new ObservableCollection<CustomerViewModel> ( );

		private static readonly DataGridColumn dataGridColumn   ;
		private DataGridColumn[] DGBankColumnsCollection = {dataGridColumn,dataGridColumn,dataGridColumn,dataGridColumn,
			dataGridColumn,dataGridColumn,dataGridColumn,dataGridColumn };
		private DataGridColumn[] DGCustColumnsCollection
			= {dataGridColumn,dataGridColumn,dataGridColumn,dataGridColumn,dataGridColumn,dataGridColumn,dataGridColumn,dataGridColumn,dataGridColumn,
			dataGridColumn,dataGridColumn,dataGridColumn,dataGridColumn,dataGridColumn ,dataGridColumn };
		private DataGridColumn[] DGDetailsColumnsCollection= {dataGridColumn,dataGridColumn,dataGridColumn,dataGridColumn,
			dataGridColumn,dataGridColumn,dataGridColumn,dataGridColumn };

		private CollectionView view1 = null;
		private CollectionView view2 = null;
		private CollectionView view3 = null;
		private CollectionView view4 = null;
		nworder Nwview1 = new nworder ( );
		nwcustomer nwc = new nwcustomer ( );

		//Handle click event  from 3DButton
		public event RoutedEventHandler Click;

		public Stylingtest ( )
		{
			InitializeComponent ( );
			this . Show ( );
			MouseMove += Utils . Grab_MouseMove;
			KeyDown += Window_PreviewKeyDown;
		}

		private void Window_PreviewKeyDown ( object sender , KeyEventArgs e )
		{
			if ( e . Key == Key . F11 )
			{
				if ( Utils . ControlsHitList . Count == 0 )
					return;
				Utils . Grabscreen ( this , Utils . ControlsHitList [ 0 ] . VisualHit , null , sender as Control );
			}
		}


		TextBox tb;
		// how to access template named controls in c#
		public override void OnApplyTemplate ( )
		{
			base . OnApplyTemplate ( );

			if ( Template != null )
			{
				Console . WriteLine ( $"TargetType = {Template . TargetType}" );
				TemplateContent tc = Template .Template;
				Console . WriteLine ( $"{tc . ToString ( )}" );

				if ( Template . HasContent )
				{
					tb = GetTemplateChild ( "Controlname" ) as TextBox;
					var v = GetTemplateChild ( "RectBtn" );
				}
			}
			return;
		}
		private async void CustGrid_Loaded ( object sender , RoutedEventArgs e )
		{
			EventControl . CustDataLoaded += EventControl_CustDataLoaded;
			//DataGridUtilities . LoadDataGridColumns ( CustGrid , "DGMultiCustomerColumns" );
			//DataGridUtilities . LoadDataGridTextColumns ( CustGrid , "DGMultiCustomerTextColumns" );
			//DataGridSupport . SortCustomerColumns ( CustGrid , DGCustColumnsCollection );

			this . CustGrid . ItemsSource = null;
			this . CustGrid . Items . Clear ( );
			AllCustomers CustViewcollection = new AllCustomers ( );
			CustViewcollection = AllCustomers . LoadCust ( CustViewcollection , "CUSTDBVIEW" , 3 , true );
		}

		private void EventControl_CustDataLoaded ( object sender , LoadedEventArgs e )
		{
			LView3 . ItemsSource = null;
			//                        Customers1 = e . DataSource as CustomerCollection;
			LView3 . ItemsSource = e . DataSource as AllCustomers;
			//                        CustGrid . ItemsSource = e . DataSource as CustCollection;

			//                        CustGrid . ItemsSource = view1;
			Customers1 = e . DataSource as ObservableCollection<CustomerViewModel>;
			CustGrid . ItemsSource = Customers1;

			if ( CollectionViewSource . GetDefaultView ( Customers1 ) . IsEmpty )
				return;
			AllCustomers custs = new AllCustomers ( );
			CollectionViewSource custview = new CollectionViewSource ( );
			Customers1 = ( ObservableCollection<CustomerViewModel> ) CollectionViewSource . GetDefaultView ( Customers1 );
			if ( view1 != ( CollectionView ) null && view1 . Count > 0 )
				view1 . SortDescriptions . Clear ( );
			view1 . SortDescriptions . Add ( new SortDescription ( "Surname" , ListSortDirection . Ascending ) );
			CustGrid . ItemsSource = view1;

			this . CustGrid . SelectedIndex = 0;
			this . CustGrid . SelectedItem = 0;
			this . CustGrid . CurrentItem = 0;
			this . CustGrid . UpdateLayout ( );
			this . CustGrid . Refresh ( );
		}
		private void storyboard_Loaded ( object sender , RoutedEventArgs e )
		{
			if ( NwOrders . Count > 0 )
				NwOrders . Clear ( );
			if ( NwCustomers . Count > 0 )
				NwCustomers . Clear ( );

			LView1 . Items . Clear ( );
			LView1 . ItemsSource = null;
			LView2 . Items . Clear ( );
			LView2 . ItemsSource = null;
			LView4 . Items . Clear ( );
			LView4 . ItemsSource = null;

			//Spawn remote threads to load NW order/Customer Db data
			Thread thread1 = new Thread ( loadnworders );
			thread1 . IsBackground = true;
			Thread thread2 = new Thread ( loadcustomers );
			thread2 . IsBackground = true;
			//Handle click event  from 3DButton
			Click += new RoutedEventHandler ( Threedbtn_Click );
			// Kick the threads off
			thread1 . Start ( );
			thread2 . Start ( );
			//Wait till threads have completed loading db's
			while ( thread1 . ThreadState == ThreadState . Background )
				Thread . Sleep ( 100 );
			while ( thread2 . ThreadState == ThreadState . Background )
				Thread . Sleep ( 100 );
			CheckOrders ( );
			CheckCustomers ( );
			CustomersDbDataTemplateSelector cdb = new CustomersDbDataTemplateSelector();
			//cdb.SelectTemplate(object o, Dependencyobject dp)
		}

		private void CheckOrders ( )
		{
			// Load NwOrders Db into ListBox/View's
			NwOrder1 = NwOrders;
			view1 = new CollectionView ( NwOrder1 );
			view1 = ( CollectionView ) CollectionViewSource . GetDefaultView ( NwOrder1 );
			if ( view1 != ( CollectionView ) null && view1 . Count > 0 )
				view1 . SortDescriptions . Clear ( );
			view1 . SortDescriptions . Add ( new SortDescription ( "ShipName" , ListSortDirection . Ascending ) );
			LView1 . ItemsSource = view1;
			DataContext = view1;
			LView1 . UpdateLayout ( );
			LView1 . Refresh ( );
			Nwview1 = view1 . CurrentItem as nworder;
		}
		private void CheckCustomers ( )
		{
			// Load NwCustomers Db into ListBox/View's
			view4 = new CollectionView ( NwCustomers );
			view4 = ( CollectionView ) CollectionViewSource . GetDefaultView ( NwCustomers1 );
			if ( view4 != ( CollectionView ) null && view4 . Count > 0 )
				view4 . SortDescriptions . Clear ( );
			view4 . SortDescriptions . Add ( new SortDescription ( "PostalCode" , ListSortDirection . Ascending ) );
			LView2 . ItemsSource = view4;
			DataContext = view4;
			view3 = new CollectionView ( NwCustomers3 );
			view3 = ( CollectionView ) CollectionViewSource . GetDefaultView ( NwCustomers3 );
			if ( view3 != ( CollectionView ) null && view3 . Count > 0 )
				view3 . SortDescriptions . Clear ( );
			view3 . SortDescriptions . Add ( new SortDescription ( "CompanyName" , ListSortDirection . Ascending ) );
			LView4 . ItemsSource = view3;
			LView4 . Refresh ( );
			DataContext = view3;
			NwCustomers6 = NwCustomers;
			LView2 . ItemsSource = NwCustomers;
			LView2 . Refresh ( );
			// do  The sortng with CollectionView
			//view2 = ( CollectionView ) CollectionViewSource .GetDefaultView ( NwCustomers6 );
			//if ( view2 != ( CollectionView ) null && view2 .Count > 0 )
			//	view2 .SortDescriptions .Clear ( );
			//view2 .SortDescriptions .Add ( new SortDescription ( "Country" , ListSortDirection .Ascending ) );
			//view2 .SortDescriptions .Add ( new SortDescription ( "Region" , ListSortDirection .Ascending ) );
			//view2 .SortDescriptions .Add ( new SortDescription ( "City" , ListSortDirection .Ascending ) );
			//LView2 .ItemsSource = NwCustomers6;
			nwcustomer nwcust = new nwcustomer ( );
		}

		private void loadnworders ( )
		{
			// Called as a background Thread
			NwOrders = NwOrders . StdLoadOrders ( "" );
			LoadOrderDb ( );
		}
		private void loadcustomers ( )
		{
			// Called as a background Thread
			NwCustomers = nwc . GetNwCustomers ( );
			NwCustomers1 = NwCustomers;
			NwCustomers3 = NwCustomers;
			LoadOrderDb ( );
		}

		private void LoadOrderDb ( )
		{
			//Wait for threads to complete loading the Db's
			while ( true )
			{
				//Thread .Sleep ( 200 );
				if ( NwOrders . Count > 0 )
				{
					break;
				}
			}
		}
		private void LoadcustomerDb ( )
		{
			//Wait for threads to complete loading the Db's
			while ( true )
			{
				//Thread .Sleep ( 200 );
				if ( NwCustomers . Count > 0 )
				{
					break;
				}
			}
		}
		private void Threedbtn_Click ( object sender , RoutedEventArgs e )
		{
			ObservableCollection<nwcustomer> NwCust = new ObservableCollection<nwcustomer> ( );

			if ( DataSwitched )
			{
				if ( NwCustomers . Count == 0 )
					NwCustomers = nwc . GetNwCustomers ( );

				//NwCustomers6 . Clear ( );
				// assign to our Obs collection
				NwCustomers6 = NwCustomers;
				LView2 . ItemsSource = null;
				//LView2 . ListviewControl . Items . Clear ( );
				LView2 . ItemsSource = NwCustomers6;

				CollectionView view2;
				// do  The sortng with CollectionView
				view2 = ( CollectionView ) CollectionViewSource . GetDefaultView ( NwCustomers6 );
				if ( view2 != ( CollectionView ) null && view2 . Count > 0 )
					view2 . SortDescriptions . Clear ( );
				view2 . SortDescriptions . Add ( new SortDescription ( "Country" , ListSortDirection . Ascending ) );
				view2 . SortDescriptions . Add ( new SortDescription ( "Region" , ListSortDirection . Ascending ) );
				view2 . SortDescriptions . Add ( new SortDescription ( "City" , ListSortDirection . Ascending ) );
				//NwCustomers6 = view2;
				LView2 . ItemsSource = NwCustomers6;

				nwc = view2 . CurrentItem as nwcustomer;
				DataSwitched = false;

				RotateControl ( LView2 , 360 , 2.0 );
				//Lview2 . ScaleTransform ( );
				//ScaleTransform . CenterYProperty = 0.5;
				return;
			}
			else
			{
				view2 = ( CollectionView ) CollectionViewSource . GetDefaultView ( NwCustomers6 );
				if ( view2 != ( CollectionView ) null && view2 . Count > 0 )
					view2 . SortDescriptions . Clear ( );
				view2 . SortDescriptions . Add ( new SortDescription ( "City" , ListSortDirection . Ascending ) );
				view2 . SortDescriptions . Add ( new SortDescription ( "Region" , ListSortDirection . Ascending ) );
				view2 . SortDescriptions . Add ( new SortDescription ( "Country" , ListSortDirection . Ascending ) );
				//NwCustomers6 = view2;
				LView2 . ItemsSource = view2;
				//                                LView2 . ItemsSource = NwCustomers;
				RotateControl ( LView2 , 360 , 2.0 );
			}

			var Customers = NwCustomers6 . AsEnumerable ( ) . GroupBy ( d => new
			{
				d . Country,
				d . City
			} )
				. Select ( gg => new
				{
					Country = gg . Key . Country,
					_City = gg . Key . City,
                                //pcode = gg.Key.PostalCode
                        } )
				. ToList ( );

			// By here, I have a list of countries + Cities alone in no particular order
			//so organise theentire Db into ordered sequence by Countries and then Cities
			int y = Customers . Count ( );

			foreach ( var country in Customers )
			{
				foreach ( var item in NwCustomers6 )
				{
					if ( item . Country == country . Country )
					{
						foreach ( var city in item . Country )
						{
							NwCust . Add ( item );
						}
					}
				}
			}
			// We now have a list arranged by City's inside Country's in our "Wrapped" ListView
			LView2 . ItemsSource = NwCust;
			DataSwitched = true;
			//RenderTargetBitmap bmp = Utils . RenderBitmap ( LView2 );
		}

		//                public event RoutedEventHandler Click;

		#region COMMANDS
		BreakCommand _breakCommand = new BreakCommand ( );
		public BreakCommand Breakcommand
		{
			get
			{
				return _breakCommand;
			}
			set
			{
				_breakCommand = value;
			}
		}
		#endregion COMMANDS



		#region Data Handling

		// Blue Background
		private void LView1_Loaded ( object sender , RoutedEventArgs e )
		{
			//NwOrder1 . Clear ( );
			//NwOrder1 = NwOrders;
			//LView1 . Items . Clear ( );
			//LView1 . ItemsSource = null;
			//view1 = new CollectionView ( NwOrder1 );
			//view1 = ( CollectionView ) CollectionViewSource . GetDefaultView ( NwOrder1 );
			//if ( view1 != ( CollectionView ) null && view1 . Count > 0 )
			//        view1 . SortDescriptions . Clear ( );
			//view1 . SortDescriptions . Add ( new SortDescription ( "ShipName", ListSortDirection . Ascending ) );
			//LView1 . ItemsSource = view1;
			//DataContext = view1;
			//LView1 . UpdateLayout ( );
			//Nwview1 = view1 . CurrentItem as nworder;
			//Thread.Sleep ( 1000 );

		}
		public static void RotateControl ( FrameworkElement ctrl , double angle , double duration )
		{
			DoubleAnimation da = new DoubleAnimation ( );
			da . From = 0;
			da . To = angle;
			da . Duration = new Duration ( TimeSpan . FromSeconds ( duration ) );
			//da . DecelerationRatio = 0.4;
			RotateTransform rt = new RotateTransform ( );
			ctrl . RenderTransform = rt;
			rt . BeginAnimation ( RotateTransform . AngleProperty , da );
		}
		// Yellow Background
		private void LView2_Loaded ( object sender , RoutedEventArgs e )
		{
		}

		// Magenta Background
		private async void LView3_Loaded ( object sender , RoutedEventArgs e )
		{
		}

		// Red Background
		private void LView4_Loaded ( object sender , RoutedEventArgs e )
		{
		}

		//// Orange Background
		private void LView5_Loaded ( object sender , RoutedEventArgs e )
		{
			//        NwOrder2 . Clear ( );
			//        NwOrder2 = NwOrders;
			//        LView5 . Items . Clear ( );
			//        view2 = new CollectionView ( NwOrder2 );
			//        view2 = ( CollectionView ) CollectionViewSource . GetDefaultView ( NwOrder2 );
			//        if ( view2 != ( CollectionView ) null && view2 . Count > 0 )
			//                view2 . SortDescriptions . Clear ( );
			//        view2 . SortDescriptions . Add ( new SortDescription ( "CustomerId", ListSortDirection . Ascending ) );
			//        LView5 . ItemsSource = view2;
			//        DataContext = view2;
			//        LView5 . UpdateLayout ( );
			//        Nwview2 = view2 . CurrentItem as nworder;
		}

		//// Black Background Grid view
		private void LView6_Loaded ( object sender , RoutedEventArgs e )
		{
			//        if ( NwOrders . Count == 0 )
			//                NwOrders . StdLoadOrders ( "" );
			//        NwGridOrder1 = NwOrders;
			//        LView6 . Items . Clear ( );

			//        // Get a view and handle sorting
			//        CollectionView view9;
			//        view9 =  (CollectionView)CollectionViewSource . GetDefaultView ( NwGridOrder1 );
			//       // view6 = ( CollectionView ) CollectionViewSource . GetDefaultView ( NwGridOrder1 );
			//        if ( view9 != ( CollectionView ) null && view9 . Count > 0 )
			//                view9 . SortDescriptions . Clear ( );
			//       // view6 . SortDescriptions . Add ( new SortDescription ( "ShipName", ListSortDirection . Descending ) );

			//        view9 . SortDescriptions . Add ( new SortDescription ( "ShipName", ListSortDirection . Descending ) );
			//        //Assign our sorted data to our ItemsSource
			//        LView6 . ItemsSource = view9;
			//        // Set up current item pointer 
			//       // Nwview6 = view6 . CurrentItem as nworder;
			//        // set up our DataContext for this ListBox
			//        ListViewControl lbc1 = sender as ListViewControl;
			//        //lbc1 . DataContext = view9;
		}

		//Gray default
		// This has independent nworder data due to the sorting performed in the method
		private void LView7_Loaded ( object sender , RoutedEventArgs e )
		{
			//  NwCustomers6 . Clear ( );
			// assign to our Obs collection
			//NwCustomers6 = NwCustomers;

			//LView2 . Items . Clear ( );
			//LView2 . ItemsSource = NwCustomers;

			//// do  The sortng with CollectionView
			//view2 = ( CollectionView ) CollectionViewSource . GetDefaultView ( NwCustomers6 );
			//if ( view2 != ( CollectionView ) null && view2 . Count > 0 )
			//        view2 . SortDescriptions . Clear ( );
			//view2 . SortDescriptions . Add ( new SortDescription ( "Country", ListSortDirection . Ascending ) );
			//view2 . SortDescriptions . Add ( new SortDescription ( "Region", ListSortDirection . Ascending ) );
			//view2 . SortDescriptions . Add ( new SortDescription ( "City", ListSortDirection . Ascending ) );
			////NwCustomers6 = view2;
			//LView2 . ItemsSource = NwCustomers6;
			//nwcustomer nwcust = new nwcustomer ( );

			//nwc = view2 . CurrentItem as nwcustomer;

			//nwcustomer custsort = new nwcustomer ( );

		}

		#endregion

		private void Items_CurrentChanging ( object sender , System . ComponentModel . CurrentChangingEventArgs e )
		{
			// This gets called when loading the grid !!!
		}

		private void Lv1_SelectionChanged ( object sender , SelectionChangedEventArgs e )
		{
			//Save current data itrem selection to nworder class
			//nwOrder = Lv1 . Items [ Lv1 . SelectedIndex ] as nworder;
			//nwOrder . SelectedItem = nwOrder;
			//nwOrder . SelectedIndex = Lv1 . SelectedIndex;
		}

		private void ThreeDeeBtnControl_MouseDown ( object sender , MouseButtonEventArgs e )
		{
			var Customers = NwCustomers6 . AsEnumerable ( ) . GroupBy ( d => new
			{
				d . Country,
				d . City
			} )
				. Select ( gg => new
				{
					Country = gg . Key . Country,
					_City = gg . Key . City,
                                //pcode = gg.Key.PostalCode
                        } )
				. ToList ( );

			ObservableCollection<nwcustomer> NwCust = new ObservableCollection<nwcustomer> ( );
			// By here, I have a list of countries + Cities alone in no particular order
			//so organise theentire Db into ordered sequence by Countries and then Cities
			int y = Customers . Count ( );

			foreach ( var country in Customers )
			{
				foreach ( var item in NwCustomers6 )
				{
					if ( item . Country == country . Country )
					{
						foreach ( var city in item . Country )
						{
							NwCust . Add ( item );
							//Console . WriteLine ( $"{item . Country}, {item . City}, {item . CompanyName}" );
						}
					}
				}
			}

			// We now have a list arranged by City's inside Country's in our "Wrapped" ListView
			LView2 . ItemsSource = NwCust;

		}

		private void ThreeDeeBtnControl_MouseDown ( object sender , RoutedEventArgs e )
		{
			var Customers = NwCustomers6 . AsEnumerable ( ) . GroupBy ( d => new
			{
				d . Country,
				d . City
			} )
				. Select ( gg => new
				{
					Country = gg . Key . Country,
					_City = gg . Key . City,
                                //pcode = gg.Key.PostalCode
                        } )
				. ToList ( );

			ObservableCollection<nwcustomer> NwCust = new ObservableCollection<nwcustomer> ( );
			// By here, I have a list of countries + Cities alone in no particular order
			//so organise theentire Db into ordered sequence by Countries and then Cities
			int y = Customers . Count ( );

			foreach ( var country in Customers )
			{
				foreach ( var item in NwCustomers6 )
				{
					if ( item . Country == country . Country )
					{
						foreach ( var city in item . Country )
						{
							NwCust . Add ( item );
							//Console . WriteLine ( $"{item . Country}, {item . City}, {item . CompanyName}" );
						}
					}
				}
			}

			// We now have a list arranged by City's inside Country's in our "Wrapped" ListView
			LView2 . ItemsSource = NwCust;

		}

		private void StdListview_SelectionChanged ( object sender , SelectionChangedEventArgs e )
		{
			ListView lv = sender as ListView;
		}

		private void APTestingControl_Loaded ( object sender , RoutedEventArgs e )
		{

		}

		private void ShadowLabelControl_PreviewMouseDoubleClick ( object sender , MouseButtonEventArgs e )
		{
			this . Close ( );
		}

		private async void DataGrid_Loaded ( object sender , RoutedEventArgs e )
		{
			this . BankGrid . ItemsSource = null;
			this . BankGrid . Items . Clear ( );
			EventControl . BankDataLoaded += EventControl_BankDataLoaded1;
			int counter = 0;
			foreach ( var item in BankGrid . Columns )
			{
				DGBankColumnsCollection [ counter++ ] = item;
			}
			DataGridSupport . SortBankColumns ( BankGrid , DGBankColumnsCollection );

			BankCollection BankViewcollection = new BankCollection ( );
			await BankCollection . LoadBank ( BankViewcollection , "BANKDATA" , 7 , true );
			//BankViewcollection = BankCollection .LoadBank ( BankViewcollection , "BANKDATA" );
			//CollectionViewSource  BankviewerView = CollectionViewSource . GetDefaultView ( BankViewcollection );
			BankGrid . Refresh ( );
			// Set our grids items source
			//this .BankGrid .ItemsSource = BankViewcollection;

			//this .BankGrid .SelectedIndex = 0;
			//this .BankGrid .SelectedItem = 0;
			//this .BankGrid .CurrentItem = 0;
			//this .BankGrid .UpdateLayout ( );
			//this .BankGrid .Refresh ( );

		}

		private void EventControl_BankDataLoaded1 ( object sender , LoadedEventArgs e )
		{
			BankCollection BankViewcollection = new BankCollection ( );
			BankViewcollection = e . DataSource as BankCollection;
			BankViewcollection = BankViewcollection;

			BankGrid . Refresh ( );
			// Set our grids items source
			this . BankGrid . ItemsSource = BankViewcollection;

			this . BankGrid . SelectedIndex = 0;
			this . BankGrid . SelectedItem = 0;
			this . BankGrid . CurrentItem = 0;
			this . BankGrid . UpdateLayout ( );
			this . BankGrid . Refresh ( );
		}

		private void storyboard_Closed ( object sender , EventArgs e )
		{
			Customers1 . Clear ( );
			NwOrders . Clear ( );
			BankGrid . ItemsSource = null;
			BankGrid . Items . Clear ( );
			CustGrid . ItemsSource = null;
			CustGrid . Items . Clear ( );
			EventControl . CustDataLoaded -= EventControl_CustDataLoaded;

		}

		private void bankgrid_PreviewDragEnter ( object sender , DragEventArgs e )
		{
			e . Effects = ( DragDropEffects ) DragDropEffects . Move;
		}

		private void bankgrid_PreviewMouseLeftButtonup ( object sender , MouseButtonEventArgs e )
		{
			ScrollBarMouseMove = false;
		}

		private void bankgrid_PreviewMouseMove ( object sender , MouseEventArgs e )
		{
			bool IsCust = false;
			Point mousePos = e.GetPosition(null);
			Vector diff = _startPoint - mousePos;
			string t1 = sender.GetType().ToString();
			if ( e . LeftButton == MouseButtonState . Pressed &&
			    Math . Abs ( diff . X ) > SystemParameters . MinimumHorizontalDragDistance ||
			    Math . Abs ( diff . Y ) > SystemParameters . MinimumVerticalDragDistance )
			{
				if ( IsLeftButtonDown && e . LeftButton == MouseButtonState . Pressed )
				{
					bool isvalid = false;
					if ( t1 . Contains ( "ListBox" ) || t1 . Contains ( "ListView" ) )
					{
						ListBox lb;
						ListView lv;
						isvalid = true;
						if ( t1 . Contains ( "ListBox" ) )
							lb = sender as ListBox;
						else if ( t1 . Contains ( "ListView" ) )
							lv = sender as ListView;
						IsCust = true;
					}
					else if ( t1 . Contains ( "DataGrid" ) )
					{
						isvalid = true;
						DataGrid dg = sender as DataGrid;
						if ( dg . Name == "CustGrid" )
							IsCust = true;
					}
					if ( isvalid )
					{
						if ( ScrollBarMouseMove )
							return;
						// We are dragging from the DETAILS grid							
						//Working string version
						if ( IsCust == false )
						{
							BankAccountViewModel bvm = new BankAccountViewModel();
							//if ( t1 .Contains ( "ListView" ) )
							//      bvm = UCListbox .SelectedItem as BankAccountViewModel;
							//else
							bvm = BankGrid . SelectedItem as BankAccountViewModel;
							string str = GetExportRecords.CreateTextFromRecord(bvm, null, null, true, false);
							string dataFormat = DataFormats.Text;
							DataObject dataObject = new DataObject(dataFormat, str);
							DragDrop . DoDragDrop (
							BankGrid ,
							dataObject ,
							DragDropEffects . Copy );
						}
						else
						{
							CustomerViewModel cvm = new CustomerViewModel();
							if ( t1 . Contains ( "ListBox" ) )
								cvm = LView3 . SelectedItem as CustomerViewModel;
							else if ( t1 . Contains ( "ListView" ) )
								cvm = LView2 . SelectedItem as CustomerViewModel;
							else
								cvm = CustGrid . SelectedItem as CustomerViewModel;
							string str = GetExportRecords.CreateTextFromRecord(null, null, cvm, true, false);
							string dataFormat = DataFormats.Text;
							DataObject dataObject = new DataObject(dataFormat, str);
							DragDrop . DoDragDrop (
							CustGrid ,
							dataObject ,
							DragDropEffects . Copy );
						}
						IsLeftButtonDown = false;
					}
				}
			}
		}

		private void bankgrid_PreviewMouseLeftButtondown ( object sender , MouseButtonEventArgs e )
		{
			// Gotta make sure it is not anywhere in the Scrollbar we clicked on 
			if ( Utils . HitTestScrollBar ( sender , e ) )
			{
				ScrollBarMouseMove = true;
				return;
			}
			if ( Utils . HitTestHeaderBar ( sender , e ) )
				return;

			_startPoint = e . GetPosition ( null );
			// Make sure the left mouse button is pressed down so we are really moving a record
			if ( e . LeftButton == MouseButtonState . Pressed )
			{
				IsLeftButtonDown = true;
			}
			string t = sender.GetType().ToString();
			object b = e.OriginalSource;
			if ( t . Contains ( "DataGrid" ) )
			{
				DataGrid dg = sender as DataGrid;
				GridSelection = dg . SelectedIndex;
			}
			else
			{
				//ListSelection = UCListbox .SelectedIndex;
				//GridSelection = ListSelection;
				//datagrid .SelectedIndex = ListSelection;
			}
		}

		private void Customer_PreviewDragEnter ( object sender , DragEventArgs e )
		{
			e . Effects = ( DragDropEffects ) DragDropEffects . Move;
		}
		private void Customer_PreviewMouseLeftButtonup ( object sender , MouseButtonEventArgs e )
		{
			ScrollBarMouseMove = false;
		}
		private void Customer_PreviewMouseLeftButtondown ( object sender , MouseButtonEventArgs e )
		{
			// Gotta make sure it is not anywhere in the Scrollbar we clicked on 
			if ( Utils . HitTestScrollBar ( sender , e ) )
			{
				ScrollBarMouseMove = true;
				return;
			}
			if ( Utils . HitTestHeaderBar ( sender , e ) )
				return;

			_startPoint = e . GetPosition ( null );
			// Make sure the left mouse button is pressed down so we are really moving a record
			if ( e . LeftButton == MouseButtonState . Pressed )
			{
				IsLeftButtonDown = true;
			}
			string t = sender.GetType().ToString();
			object b = e.OriginalSource;
			if ( t . Contains ( "ListView" ) )
			{
				ListView dg = sender as ListView;
				GridSelection = dg . SelectedIndex;
			}
		}

		private void storyboard_Closing ( object sender , CancelEventArgs e )
		{
			if ( view1 != null && !view1 . IsEmpty )
				view1 . DetachFromSourceCollection ( );
			if ( view2 != null && !view2 . IsEmpty )
				view2 . DetachFromSourceCollection ( );
			if ( view3 != null && !view3 . IsEmpty )
				view3 . DetachFromSourceCollection ( );
			if ( view4 != null && !view4 . IsEmpty )
				view4 . DetachFromSourceCollection ( );
			ObservableCollection<DataGridColumn> dgc2 = CustGrid.Columns;
			dgc2 . Clear ( );
			ObservableCollection<DataGridColumn> dgc1 = BankGrid.Columns;
			dgc1 . Clear ( );

		}
	}
}

