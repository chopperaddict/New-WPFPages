using System;
using System .Collections .Generic;
using System .Collections .ObjectModel;
using System .Linq;
using System .Text;
using System .Threading;
using System .Threading .Tasks;
using System .Windows;
using System .Windows .Controls;
using System .Windows .Data;
using System .Windows .Documents;
using System .Windows .Input;
using System .Windows .Media;
using System .Windows .Media .Imaging;
using System .Windows .Shapes;

using WPFPages . ViewModels;

namespace WPFPages .Views
{
	/// <summary>
	/// Interaction logic for TestNWView.xaml
	/// </summary>
	public partial class TestNWView : Window
	{
		#region Object Declarations
		private NwOrderCollection NwOrders = new NwOrderCollection ( );
		private nworder nwo= new nworder ( );

		private ObservableCollection<nwcustomer> NwCustomers = new ObservableCollection<nwcustomer> ( );
		private nwcustomer nwc = new nwcustomer ( );

		private ObservableCollection<NWSupplier> NwSuppliers= new ObservableCollection<NWSupplier> ( );
		private NWSupplierCollection nws = new NWSupplierCollection();
		private NWSupplier nwsupplier = new NWSupplier();
		private CollectionView NWSupplierView;

		#endregion Object Declarations

		public TestNWView ( )
		{
			InitializeComponent ( );
			
		}

		private void Window_Loaded ( object sender , RoutedEventArgs e )
		{
			NwSuppliers = nws . LoadNWSuppliers ( -1 );
			// Load NWSuppliers and assign to View
			NWSupplierView = CollectionViewSource . GetDefaultView ( NwSuppliers ) as CollectionView;
			Datagrid . ItemsSource = NWSupplierView;
			Listview . ItemsSource = NWSupplierView;
		}

		#region Unused NW Database loadmethods
		//private void loadnworders ( )
		//{
		//	// Called as a background Thread
		//	NwOrders = NwOrders .StdLoadOrders ( "" );
		//}
		//private void loadcustomers ( )
		//{
		//	// Called as a background Thread
		//	NwCustomers = nwc .GetNwCustomers ( );
		//}
		#endregion Unused NW Database loadmethods

	}
}
