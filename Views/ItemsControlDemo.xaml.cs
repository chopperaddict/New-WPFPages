using System;
using System . Collections . Generic;
using System . ComponentModel;
using System . Data;
using System . Linq;
using System . Text;
using System . Threading . Tasks;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Data;
using System . Windows . Documents;
using System . Windows . Input;
using System . Windows . Media;
using System . Windows . Media . Imaging;
using System . Windows . Shapes;

using WPFPages . ViewModels;

namespace WPFPages . Views
{
	/// <summary>
	/// Interaction logic for ItemsControlDemo.xaml
	/// </summary>
	public partial class ItemsControlDemo : Window
	{
		BankCollection BankViewcollection = new BankCollection();
		CollectionView BankviewerView;
		public ItemsControlDemo ( )
		{
			InitializeComponent ( );
			EventControl . BankDataLoaded += EventControl_BankDataLoaded;
			Utils . SetupWindowDrag ( this );
		}

		private void EventControl_BankDataLoaded ( object sender , LoadedEventArgs e )
		{
			// Event handler for BankDataLoaded
			if ( e . DataSource == null )
				return;

			itemsControl . ItemsSource = null;
			itemsControl . Items . Clear ( );
			itemsControl2 . ItemsSource = null;
			itemsControl2 . Items . Clear ( );

			BankViewcollection = e . DataSource as BankCollection;
			itemsControl . ItemsSource = BankViewcollection;
			itemsControl2 . ItemsSource = BankViewcollection;
		}

		private void Window_Loaded ( object sender , RoutedEventArgs e )
		{
			if ( DesignerProperties . GetIsInDesignMode ( this ) == true )
			{
				Utils . LoadBankDbGeneric ( bvm: BankViewcollection , Notify: true , lowvalue:1055000, highvalue:1060000, maxrecords: 40 );
			}
			else
			{
				//Using my generic bank loading method in library
				Utils . LoadBankDbGeneric ( bvm: BankViewcollection , Notify: true , maxrecords: 40 );
			}
		}


	}
}
