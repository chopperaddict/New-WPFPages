using System;
using System . Collections . Generic;
using System . ComponentModel;
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
using System . Windows . Navigation;
using System . Windows . Shapes;

using WPFPages . Views;

namespace WPFPages . UserControls
{
        /// <summary>
        /// Interaction logic for WrappedListbox.xaml
        /// </summary>
        public partial class WrappedListbox : UserControl
        {
                //public WrappedLBoxClass WrappedLB;
                public NwOrderDesignCollection NwOrders = new NwOrderDesignCollection ( );
                public nworder nwOrder = new nworder ( );

                public WrappedListbox ( )
                {
                        InitializeComponent ( );
                }

                #region DEPENDENCY OBECTS

                #region Foreground
                new public Brush Foreground
                {
                        get
                        {
                                return ( Brush ) GetValue ( ForegroundProperty );
                        }
                        set
                        {
                                SetValue ( ForegroundProperty, value );
                                SetValue ( CurrentTextColorProperty, value );
                        }
                }
                new public static readonly DependencyProperty ForegroundProperty =
                        DependencyProperty . Register ( "Foreground",
                        typeof ( Brush ),
                        typeof ( WrappedListbox ),
                        new PropertyMetadata ( ( Brush ) Brushes . Black ), OnForegroundPropertyChanged );
                private static bool OnForegroundPropertyChanged ( object value )
                {
                        Console . WriteLine ( $"ForegroundPropertyChanged :=  {value}" );
                        return true;
                }
                #endregion

                #region CurrentTextColor
                public Brush CurrentTextColor
                {
                        get
                        {
                                return ( Brush ) GetValue ( CurrentTextColorProperty );
                        }
                        set
                        {
                                SetValue ( CurrentTextColorProperty, value );
                        }
                }
                public static readonly DependencyProperty CurrentTextColorProperty =
                        DependencyProperty . Register ( "CurrentTextColor",
                        typeof ( Brush ),
                        typeof ( WrappedListbox ),
                        new PropertyMetadata ( ( Brush ) Brushes . Black ), OnCurrentTextColorProperty );
                private static bool OnCurrentTextColorProperty ( object value )
                {
                        Console . WriteLine ( $"CurrentTextColorProperty Changed to :  {value}" );
                        return true;
                }
                #endregion

                #region SelectionBackground
                public Brush SelectionBackground
                {
                        get
                        {
                                return ( Brush ) GetValue ( SelectionBackgroundProperty );
                        }
                        set
                        {
                                SetValue ( SelectionBackgroundProperty, value );
                        }
                }
                public static readonly DependencyProperty SelectionBackgroundProperty =
                        DependencyProperty . Register ( "SelectionBackground",
                        typeof ( Brush ),
                        typeof ( WrappedListbox ),
                        new PropertyMetadata ( ( Brush ) Brushes . White ), OnSelectionBackgroundProperty );
                private static bool OnSelectionBackgroundProperty ( object value )
                {
                        Console . WriteLine ( $"SelectionBackgroundProperty  :=  {value}" );
                        return true;
                }
                #endregion

                #region SelectionForeground
                public Brush SelectionForeground
                {
                        get
                        {
                                return ( Brush ) GetValue ( SelectionForegroundProperty );
                        }
                        set
                        {
                                SetValue ( SelectionForegroundProperty, value );
                                SetValue ( CurrentTextColorProperty, value );
                        }
                }
                public static readonly DependencyProperty SelectionForegroundProperty =
                        DependencyProperty . Register ( "SelectionForeground",
                        typeof ( Brush ),
                        typeof ( WrappedListbox ),
                        new PropertyMetadata ( ( Brush ) Brushes . White ), OnSelectionSelectionForegroundProperty );
                private static bool OnSelectionSelectionForegroundProperty ( object value )
                {
                        Console . WriteLine ( $"SelectionForeground := {value}" );
                        return true;
                }
                #endregion SelectionForeground

                #region MouseoverForeground
                public Brush MouseoverForeground
                {
                        get
                        {
                                return ( Brush ) GetValue ( MouseoverForegroundProperty );
                        }
                        set
                        {
                                SetValue ( MouseoverForegroundProperty, value );
                                SetValue ( CurrentTextColorProperty, value );
                        }
                }
                public static readonly DependencyProperty MouseoverForegroundProperty =
                        DependencyProperty . Register ( "MouseoverForeground",
                        typeof ( Brush ),
                        typeof ( WrappedListbox ),
                        new PropertyMetadata ( ( Brush ) Brushes . Blue ), OnMouseoverForegroundProperty );
                private static bool OnMouseoverForegroundProperty ( object value )
                {
                        Console . WriteLine ( $"MouseoverForegroundProperty := {value}" );
                        return true;
                }
                #endregion MouseoverForeground

                #region MouseoverBackground
                public Brush MouseoverBackground
                {
                        get
                        {
                                return ( Brush ) GetValue ( MouseoverBackgroundProperty );
                        }
                        set
                        {
                                SetValue ( MouseoverBackgroundProperty, value );
                        }
                }
                public static readonly DependencyProperty MouseoverBackgroundProperty =
                        DependencyProperty . Register ( "MouseoverBackground",
                        typeof ( Brush ),
                        typeof ( WrappedListbox ),
                        new PropertyMetadata ( ( Brush ) Brushes . LightGray ), OnMouseoverBackgroundProperty );
                private static bool OnMouseoverBackgroundProperty ( object value )
                {
                        Console . WriteLine ( $"MouseoverBackgroundProperty:= {value}" );
                        return true;
                }
                #endregion MouseoverForeground

                #region MouseoverSelectedForeground
                public Brush MouseoverSelectedForeground
                {
                        get
                        {
                                return ( Brush ) GetValue ( MouseoverSelectedForegroundProperty );
                        }
                        set
                        {
                                SetValue ( MouseoverSelectedForegroundProperty, value );
                                SetValue ( CurrentTextColorProperty, value );
                        }
                }
                public static readonly DependencyProperty MouseoverSelectedForegroundProperty =
                        DependencyProperty . Register ( "MouseoverSelectedForeground",
                        typeof ( Brush ),
                        typeof ( WrappedListbox ),
                        new PropertyMetadata ( ( Brush ) Brushes . Yellow ), OnMouseoverSelectedForegroundProperty );
                private static bool OnMouseoverSelectedForegroundProperty ( object value )
                {
                        Console . WriteLine ( $"MouseoverSelectedForegroundProperty := {value}" );
                        return true;
                }
                #endregion MouseoverForeground

                #region MouseoverSelectedBackground
                public Brush MouseoverSelectedBackground
                {
                        get
                        {
                                return ( Brush ) GetValue ( MouseoverSelectedBackgroundProperty );
                        }
                        set
                        {
                                SetValue ( MouseoverSelectedBackgroundProperty, value );
                        }
                }
                public static readonly DependencyProperty MouseoverSelectedBackgroundProperty =
                        DependencyProperty . Register ( "MouseoverSelectedBackground",
                        typeof ( Brush ),
                        typeof ( WrappedListbox),
                        new PropertyMetadata ( ( Brush ) Brushes . LightGray ), OnMouseoverSelectedBackgroundProperty );
                private static bool OnMouseoverSelectedBackgroundProperty ( object value )
                {
                        Console . WriteLine ( $"MouseoverSelectedBackgroundProperty:= {value}" );
                        return true;
                }
                #endregion MouseoverSelectedBackground

                #endregion
                public static WrappedLBoxClass  GetWrappedLBox ( )
                {
                        return new WrappedLBoxClass() ;
                }
                private void Wrappedlistbox_Loaded ( object sender, RoutedEventArgs e )
                {
                        if ( DesignerProperties . GetIsInDesignMode ( this ) == true )
                        {
                                NwOrders . Clear ( );
                                Listbox. Items . Clear ( );
                                NwOrders . StdLoadOrders ( "" );
                                Listbox . ItemsSource = NwOrders;
                                DataContext = nwOrder;
                                Listbox . UpdateLayout ( );
                                CollectionView view = ( CollectionView ) CollectionViewSource . GetDefaultView ( Listbox . ItemsSource );
                                view . SortDescriptions . Add ( new SortDescription ( "OrderId", ListSortDirection . Ascending ) );
                                nwOrder = view . CurrentItem as nworder;
                                nwOrder = Listbox . SelectedItem as nworder;
                        }
                }
        }
}
