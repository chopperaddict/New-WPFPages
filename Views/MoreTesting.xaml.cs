﻿using System . ComponentModel;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Data;
using System . Windows . Input;

using WPFPages . Commands;

namespace WPFPages . Views
{
	/// <summary>
	/// Interaction logic for MoreTesting.xaml
	/// </summary>
	public partial class MoreTesting : Window
        {
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

                # region Data Handling
                public NwOrderCollection NwOrders = new NwOrderCollection ( );
                public nworder nwOrder = new nworder ( );

                #endregion
                public MoreTesting ( )
                {
                        InitializeComponent ( );
                        Utils . SetupWindowDrag ( this );
                }
            private void ChecksMouseMove ( object sender , MouseEventArgs e )
            {
                  e . Handled = true;
                  if ( e . RightButton == MouseButtonState . Pressed )
                        return;
            }

            private void Moretesting_Loaded ( object sender, RoutedEventArgs e )
                {
                        NwOrders . Clear ( );
                        Lv1 . Items . Clear ( );
                        Lv1 . Items . CurrentChanging += Items_CurrentChanging;
                        NwOrders . StdLoadOrders ( "" );
                        Lv1 . ItemsSource = NwOrders;
                        DataContext = nwOrder;
                        Lv1 . UpdateLayout ( );
                        CollectionView view = ( CollectionView ) CollectionViewSource . GetDefaultView ( Lv1. ItemsSource );
                        view . SortDescriptions . Add ( new SortDescription ( "OrderId", ListSortDirection . Ascending ) );
                        nwOrder = view . CurrentItem as nworder;
                  //                        nwOrder = Lv1 . SelectedItem as nworder;
                  MouseMove += Grab_MouseMove;
                  KeyDown += Window_PreviewKeyDown;

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

            private void Items_CurrentChanging ( object sender, System . ComponentModel . CurrentChangingEventArgs e )
                {
                        // This gets called when loading the grid !!!
                }

                private void Lv1_SelectionChanged ( object sender, SelectionChangedEventArgs e )
                {
                        //Save current data itrem selection to nworder class
                        nwOrder = Lv1 . Items [ Lv1 . SelectedIndex ] as nworder;
                        nwOrder . SelectedItem = nwOrder;
                        nwOrder . SelectedIndex = Lv1 . SelectedIndex;
                }
        }
}
