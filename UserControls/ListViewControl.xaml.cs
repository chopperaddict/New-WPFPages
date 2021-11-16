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
        public partial class ListViewControl : UserControl
        {
                public ListViewControl ( )
                {
                        InitializeComponent ( );
                }
                #region Data Handling
                public NwOrderDesignCollection NwOrders = new NwOrderDesignCollection ( );
                public nworder nwOrder = new nworder ( );

                #endregion

                #region Dependency Properties

                #region FontSize
                new public double FontSize
                {
                        get
                        {
                                return ( double ) GetValue ( FontSizeProperty );
                        }
                        set
                        {
                                SetValue ( FontSizeProperty, value );
                        }
                }
                new public static readonly DependencyProperty FontSizeProperty =
                        DependencyProperty . Register ( "FontSize",
                        typeof ( double ),
                        typeof ( ListViewControl ),
                        new PropertyMetadata ( ( double ) 12 ), OnFontSizeChangedProperty );
                private static bool OnFontSizeChangedProperty ( object value )
                {
                        Console . WriteLine ( $"FontSize Changed to :  {value}" );
                        return true;
                }
                #endregion

                #region FontWeightSelected
                public FontWeight FontWeightSelected
                {
                        get
                        {
                                return ( FontWeight ) GetValue ( FontWeightSelectedProperty );
                        }
                        set
                        {
                                SetValue ( FontWeightSelectedProperty, value );
                        }
                }
                public static readonly DependencyProperty FontWeightSelectedProperty =
                       DependencyProperty . Register ( "FontWeightSelected",
                       typeof ( FontWeight ),
                       typeof ( ListViewControl ),
                       new PropertyMetadata ( ( FontWeight ) default ), OnFontWeightSelectedChangedProperty );
                private static bool OnFontWeightSelectedChangedProperty ( object value )
                {
                        Console . WriteLine ( $"FontWeightSelectedChanged to :  {value}" );
                        return true;
                }
                #endregion

                #region FontWeightSelectedMouseover
                public FontWeight FontWeightSelectedMouseover
                {
                        get
                        {
                                return ( FontWeight ) GetValue ( FontWeightSelectedMouseoverProperty );
                        }
                        set
                        {
                                SetValue ( FontWeightSelectedMouseoverProperty, value );
                        }
                }
                public static readonly DependencyProperty FontWeightSelectedMouseoverProperty =
                       DependencyProperty . Register ( "FontWeightSelectedMouseover",
                       typeof ( FontWeight ),
                       typeof ( ListViewControl ),
                       new PropertyMetadata ( ( FontWeight ) default ), OnFontWeightSelectedMouseoverPropertyChangedProperty );
                private static bool OnFontWeightSelectedMouseoverPropertyChangedProperty ( object value )
                {
                        Console . WriteLine ( $"FontWeightChanged to :  {value}" );
                        return true;
                }
                #endregion

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
                        }
                }
                new public static readonly DependencyProperty ForegroundProperty =
                        DependencyProperty . Register ( "Foreground",
                        typeof ( Brush ),
                        typeof ( ListViewControl ),
                        new PropertyMetadata ( ( Brush ) Brushes . Black ), OnForegroundPropertyChanged );
                private static bool OnForegroundPropertyChanged ( object value )
                {
                        Console . WriteLine ( $"ForegroundPropertyChanged :=  {value}" );
                        return true;
                }
                #endregion

                #region ItemHeight
                public double ItemHeight
                {
                        get
                        {
                                return ( double ) GetValue ( ItemHeightProperty );
                        }
                        set
                        {
                                SetValue ( ItemHeightProperty, value );
                        }
                }
                public static readonly DependencyProperty ItemHeightProperty =
                       DependencyProperty . Register ( "ItemHeight",
                       typeof ( double ),
                       typeof ( ListViewControl ),
                       new PropertyMetadata ( ( double ) 18 ), OnItemHeightPropertyChangedProperty );
                private static bool OnItemHeightPropertyChangedProperty ( object value )
                {
                        Console . WriteLine ( $"ItemHeight Changed to :  {value}" );
                        return true;
                }
                #endregion

                #region ItemsSource
                public CollectionViewSource ItemsSource
                {
                        get
                        {
                                return ( CollectionViewSource ) GetValue ( ItemsSourceProperty );
                        }
                        set
                        {
                                SetValue ( ItemsSourceProperty, value );
                        }
                }
                public static readonly DependencyProperty ItemsSourceProperty =
                       DependencyProperty . Register ( "ItemsSource",
                       typeof ( CollectionViewSource ),
                       typeof ( ListViewControl ),
                       new PropertyMetadata ( ( CollectionViewSource ) null ), OnItemsSourceChanged );
                private static bool OnItemsSourceChanged ( object value )
                {
                        Console . WriteLine ( $"ListView ItemsSource Changed:=  {value}" );
                        return true;
                }
                #endregion

                #region ItemTemplate
                public DataTemplate ItemTemplate
                {
                        get
                        {
                                return ( DataTemplate ) GetValue ( ItemTemplateProperty );
                        }
                        set
                        {
                                 SetValue ( ItemTemplateProperty, value );
                         }
                }
                public static readonly DependencyProperty ItemTemplateProperty =
                       DependencyProperty . Register ( "ItemTemplate",
                       typeof ( DataTemplate ),
                       typeof ( ListViewControl ),
                       new PropertyMetadata ( ( DataTemplate ) default ), OnItemTemplatePropertyChanged );
                private static bool OnItemTemplatePropertyChanged ( object value )
                {
                           if (value != null)
                                Console . WriteLine ( $"ItemTemplate Changed:= {value}" );
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
                        typeof ( ListViewControl ),
                        new PropertyMetadata ( ( Brush ) Brushes . Black ), OnSelectionBackgroundProperty );
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
                        }
                }
                public static readonly DependencyProperty SelectionForegroundProperty =
                        DependencyProperty . Register ( "SelectionForeground",
                        typeof ( Brush ),
                        typeof ( ListViewControl ),
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
                        }
                }
                public static readonly DependencyProperty MouseoverForegroundProperty =
                        DependencyProperty . Register ( "MouseoverForeground",
                        typeof ( Brush ),
                        typeof ( ListViewControl ),
                        new PropertyMetadata ( ( Brush ) Brushes . White ), OnMouseoverForegroundProperty );
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
                        typeof ( ListViewControl ),
                        new PropertyMetadata ( ( Brush ) Brushes . DarkGray ), OnMouseoverBackgroundProperty );
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
                        }
                }
                public static readonly DependencyProperty MouseoverSelectedForegroundProperty =
                        DependencyProperty . Register ( "MouseoverSelectedForeground",
                        typeof ( Brush ),
                        typeof ( ListViewControl ),
                        new PropertyMetadata ( ( Brush ) Brushes . White ), OnMouseoverSelectedForegroundProperty );
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
                        typeof ( ListViewControl ),
                        new PropertyMetadata ( ( Brush ) Brushes . Red), OnMouseoverSelectedBackgroundProperty );
                private static bool OnMouseoverSelectedBackgroundProperty ( object value )
                {
                        Console . WriteLine ( $"MouseoverSelectedBackgroundProperty:= {value}" );
                        return true;
                }
                #endregion MouseoverSelectedBackground

                //#region SelectionChangedEventHandler
                //public SelectionChangedEventHandler SelectionChangedEventHandler
                //{
                //        get
                //        {
                //                return ( SelectionChangedEventHandler ) GetValue ( SelectionChangedEventHandlerProperty );
                //        }
                //        set
                //        {
                //                SetValue ( SelectionChangedEventHandlerProperty, value );
                //        }
                //}
                //public static readonly DependencyProperty SelectionChangedEventHandlerProperty =
                //       DependencyProperty . Register ( "SelectionChangedEventHandler",
                //       typeof ( SelectionChangedEventHandler ),
                //       typeof ( ListViewControl ),
                //       new PropertyMetadata ( ( SelectionChangedEventHandler ) null ), OnSelectionChangedEventHandlerProperty );
                //private static bool OnSelectionChangedEventHandlerProperty ( object value )
                //{
                //        if ( value != null )
                //                Console . WriteLine ( $"ItemTemplate Changed:= {value}" );
                //        return true;
                //}
                //#endregion

                #endregion Dependency Properties
                private void ListviewControl_Loaded ( object sender, RoutedEventArgs e )
                {
                        if ( DesignerProperties . GetIsInDesignMode ( this ) == true )
                        {
                                NwOrders . Clear ( );
                                ListviewControl . Items . Clear ( );
                                NwOrders . StdLoadOrders ( "" );
                                ListviewControl . ItemsSource = NwOrders;
                                DataContext = nwOrder;
                                
                                CollectionView view = ( CollectionView ) CollectionViewSource . GetDefaultView ( ListviewControl . ItemsSource );
                                view . SortDescriptions . Add ( new SortDescription ( "OrderId", ListSortDirection . Ascending ) );
                                nwOrder = view . CurrentItem as nworder;
                                nwOrder = ListviewControl . SelectedItem as nworder;
                        }
                        //SetBinding ( FontWeightProperty, "Black" );

                }
        }
}
