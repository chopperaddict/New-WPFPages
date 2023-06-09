﻿using System;
using System . Collections . Generic;
using System . Diagnostics;
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

namespace WPFPages . Views
{
	/// <summary>
	/// Interaction logic for GroupedAccounts.xaml
	/// </summary>
	public partial class GroupedAccounts : Window
	{
		public  BankCollection SqlBankcollection { get; set; }
		CollectionView view { get; set; }
		public GroupedAccounts ( )
		{
			InitializeComponent ( );
			//Create a grouping  so we can layout by AcType - don't know how this works really
			//but it works well.  It reads the list of items in the Control (a ListView in this case)
			// and creates another form of Collection, a Collectionview()
			if ( SqlBankcollection == null  || SqlBankcollection . Count == 0 )
			{
				Mouse . OverrideCursor = System . Windows . Input . Cursors . Wait;
				EventControl . BankDataLoaded += EventControl_BankDataLoaded;
				Utils . LoadBankDbGeneric ( bvm: SqlBankcollection , Notify: true , maxrecords: 200 );
				Mouse . OverrideCursor = System . Windows . Input . Cursors . Arrow;
				MouseMove += Grab_MouseMove;
				KeyDown += Window_PreviewKeyDown;
			}
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

		private async void EventControl_BankDataLoaded ( object sender, LoadedEventArgs e )
		{
			if ( e . CallerType != "BANKLISTVIEW" )
			{
				//TaskFactory task = new TaskFactory ( );// () =>  LoadGrids (e ) );
				//await task . StartNew ( ( ) => LoadSqlData ( e ) );
				SqlBankcollection = e . DataSource as BankCollection;
				// Create a Collection as this is what the grouping system Demands
				CollectionView view = ( CollectionView ) CollectionViewSource . GetDefaultView ( SqlBankcollection );
				if ( view != null )
				{
					PropertyGroupDescription groupDescription = new PropertyGroupDescription ( "AcType" );
					view . GroupDescriptions . Add ( groupDescription );
				}
				else
				{
					//whhoops - no view
					Debug . WriteLine ($"Failed to create collectionView");
					Console . Beep ( 300, 3); 
				}
				lview3 . ItemsSource = view;
				lview3 . Refresh ( );
				Mouse . OverrideCursor = System . Windows . Input . Cursors . Arrow;
			}
		}
//		private async Task<bool> LoadSqlData ( LoadedEventArgs e )
//		{
//			BankCollection bc = new BankCollection ( );
//			bc = e . DataSource as BankCollection;
//			//Sort data by AcType so we can show themgrouped
//			var accounts = from items in bc
//				       where ( items . AcType == 1 )
//				       orderby items.AcType, items.CustNo, items.BankNo
//				       select items;

//			// massage view and create a new BankCollection as ItemsSource
//			lview3 . Items . Clear ( );
//			SqlBankcollection = new BankCollection (  );
//			foreach ( var item in accounts )
//			{
//				SqlBankcollection . Add ( item );
//			}
////			MainWindow . TestBankcollection = SqlBankcollection;
//			lview3 . ItemsSource = SqlBankcollection;
//			lview3 . Refresh ( );
//			return true;
//		}

		private void Expander_Drop ( object sender, System . Windows . DragEventArgs e )
		{

		}
	}
}
