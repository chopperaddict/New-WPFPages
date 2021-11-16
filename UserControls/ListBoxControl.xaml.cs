using System;
using System . Collections . Generic;
using System . Collections . ObjectModel;
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
        /// Interaction logic for ListBoxControl.xaml
        /// This is a "Wrapped" ListBox that handles significant control over colors in the listbox for most modes
        /// such as NO Mouseover/Selected/Unselected, Mouseover/Selected/Unselected etc by setting the DP's
        /// provide by thsi control
        /// </summary>
        public partial class ListBoxControl : UserControl
        {
                #region Data Handling
  
                #endregion
                public ListBoxControl ( )
                {
                        InitializeComponent ( );
                }


                #region Dependency Properties (for  this wrapped Listbox)
                // These can all be called from any of the windows hosting one of these Usecontrol's


                #region DatabaseType - expects a string like "NWORDER" "NWCUSTOMER", "BANKACCOUNT", "CUSTOMER"  ETC
                public string DatabaseType
                {
                        get
                        {
                                return ( string ) GetValue ( DatabaseTypeProperty );
                        }
                        set
                        {
                                SetValue ( DatabaseTypeProperty, value );
                        }
                }
               public static readonly DependencyProperty DatabaseTypeProperty =
                    DependencyProperty . Register ( "DatabaseType", typeof ( string ), typeof ( ListBoxControl ), new PropertyMetadata ( (string)"NULL" ) );
                #endregion DatabaseType

                #region FontSize
                new public double FontSize
                {
                        get
                        {
                                return ( double) GetValue ( FontSizeProperty );
                        }
                        set
                        {
                                SetValue ( FontSizeProperty, value );
                        }
                }
                new public static readonly DependencyProperty FontSizeProperty =
                        DependencyProperty . Register ( "FontSize",
                        typeof ( double),
                        typeof ( ListBoxControl ),
                        new PropertyMetadata ( ( double) 12), OnFontSizeChangedProperty );
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
                        typeof ( ListBoxControl ),
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
                        typeof ( ListBoxControl ),
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
                        typeof ( ListBoxControl ),
                        new PropertyMetadata ( ( Brush ) Brushes . Black ), OnForegroundPropertyChanged );
                private static bool OnForegroundPropertyChanged ( object value )
                {
                        Console . WriteLine ( $"ForegroundPropertyChanged :=  {value}" );
                        return true;
                }
                #endregion

                #region GroupedDataTemplate (UNUSED)
                //public DataTemplate GroupedDataTemplate
                //{
                //        get
                //        {
                //                return ( DataTemplate ) GetValue ( GroupedDataTemplateProperty );
                //        }
                //        set
                //        {
                //                SetValue ( GroupedDataTemplateProperty, value );
                //        }
                //}
                //public static readonly DependencyProperty GroupedDataTemplateProperty =
                //       DependencyProperty . Register ( "GroupedDataTemplate",
                //       typeof ( DataTemplate ),
                //       typeof ( ListBoxControl ),
                //       new PropertyMetadata ( ( DataTemplate ) default ), OnGroupedDataTemplatePropertyChanged );
                //private static bool OnGroupedDataTemplatePropertyChanged ( object value )
                //{
                //        Console . WriteLine ( $"GroupedDataTemplateProperty Changed:=  {value}" );
                //        return true;
                //}
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
                       typeof ( ListBoxControl ),
                       new PropertyMetadata ( ( CollectionViewSource ) null ), OnItemsSourcePropertyChanged );
                private static bool OnItemsSourcePropertyChanged ( object value )
                {
                        Console . WriteLine ( $"ListBox ItemsSource Changed:=  {value}" );
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
                        typeof ( ListBoxControl ),
                        new PropertyMetadata ( ( double ) 18 ), OnItemHeightPropertyChangedProperty );
                private static bool OnItemHeightPropertyChangedProperty ( object value )
                {
                        Console . WriteLine ( $"ItemHeight Changed to :  {value}" );
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
                       typeof ( ListBoxControl ),
                       new PropertyMetadata ( ( DataTemplate ) default ), OnItemTemplatePropertyChanged );
                private static bool OnItemTemplatePropertyChanged ( object value )
                {
                        if ( value != null )
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
                        typeof ( ListBoxControl ),
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
                        }
                }
                public static readonly DependencyProperty SelectionForegroundProperty =
                        DependencyProperty . Register ( "SelectionForeground",
                        typeof ( Brush ),
                        typeof ( ListBoxControl ),
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
                        typeof ( ListBoxControl ),
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
                        typeof ( ListBoxControl ),
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
                         }
                }
                public static readonly DependencyProperty MouseoverSelectedForegroundProperty =
                        DependencyProperty . Register ( "MouseoverSelectedForeground",
                        typeof ( Brush ),
                        typeof ( ListBoxControl ),
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
                        typeof ( ListBoxControl ),
                        new PropertyMetadata ( ( Brush ) Brushes . LightGray ), OnMouseoverSelectedBackgroundProperty );
                private static bool OnMouseoverSelectedBackgroundProperty ( object value )
                {
                        Console . WriteLine ( $"MouseoverSelectedBackgroundProperty:= {value}" );
                        return true;
                }
                #endregion MouseoverSelectedBackground

                #region SelectionChanged
                public SelectionChangedEventHandler SelectionChanged
                {
                        get
                        {
                                return ( SelectionChangedEventHandler ) GetValue ( SelectionChangedProperty );
                        }
                        set
                        {
                                SetValue ( SelectionChangedProperty, value );
                        }
                }
                public static readonly DependencyProperty SelectionChangedProperty =
                       DependencyProperty . Register ( "SelectionChanged",
                       typeof ( SelectionChangedEventHandler ),
                       typeof ( ListBoxControl ),
                       new PropertyMetadata ( ( SelectionChangedEventHandler ) default ), OnSelectionChangedEvent );
                private static bool OnSelectionChangedEvent( object value )
                {
                        if ( value != null )
                                Console . WriteLine ( $"SelectionChanged:= {value}" );
                        return true;
                }
                #endregion


                #endregion Dependency Properties
                private  async void ListBoxControl_Loaded ( object sender, RoutedEventArgs e )
                {
                        string dtype = "";
                        if ( DesignerProperties . GetIsInDesignMode ( this ) == true )
                        {
                                dtype = GetValue ( DatabaseTypeProperty ) . ToString ( );
                                if ( dtype == "NULL" )
                                        return;
                                if ( dtype . Contains ( "NWORDER" ) )
                                {
                                        ListboxControl . ItemsSource = null;
                                        NwOrderCollection NwOrders = new NwOrderCollection ( );
                                        NwOrders . Clear ( );
                                        ListboxControl . Items . Clear ( );
                                        // Load NorthWind Order database
                                        NwOrders . StdLoadOrders ( "" );
                                        ListboxControl . ItemsSource = NwOrders;
                                        DataContext = NwOrders;
                                        ListboxControl . UpdateLayout ( );
                                        //CollectionView view = ( CollectionView ) CollectionViewSource . GetDefaultView ( ListboxControl . ItemsSource );
                                        //view . SortDescriptions . Add ( new SortDescription ( "OrderId", ListSortDirection . Ascending ) );
                                }
                                if ( dtype . Contains ( "NWCUST" ) )
                                {
                                        ListboxControl . ItemsSource = null;
                                        ObservableCollection<nwcustomer> NwCust= new ObservableCollection<nwcustomer> ( );
                                        NwCust . Clear ( );
                                        ListboxControl . Items . Clear ( );
                                        // Load NorthWind Order database
                                        nwcustomer nwcust = new nwcustomer ( );
                                       NwCust = nwcust . GetNwCustomers ( );
                                        ListboxControl . ItemsSource = NwCust;
                                        DataContext = NwCust;
                                        ListboxControl . UpdateLayout ( );
                                }
                                if ( dtype . Contains ( "BANKACCOUNT" ) )
                                {
                                        ListboxControl . ItemsSource = null;
                                        BankCollection bc = new BankCollection ( );
                                        bc =  BankCollection . LoadBank ( bc, "" );        
                                        ListboxControl . Items . Clear ( );
                                        // Load NorthWind Order database
                                        ListboxControl . ItemsSource = bc;
                                        DataContext = bc;
                                        ListboxControl . UpdateLayout ( );
                                }
                                if ( dtype . Contains ( "CUSTOMER" ) )
                                {
                                        ListboxControl . ItemsSource = null;
                                        CustCollection CustViewcollection = new CustCollection ( );
                                        CustViewcollection = await CustCollection . LoadCust ( CustViewcollection, "CUSTDBVIEW", 3, true );
                                        ListboxControl . Items . Clear ( );
                                        // Load NorthWind Order database
                                        ListboxControl . ItemsSource = CustViewcollection;
                                        DataContext = CustViewcollection;
                                        ListboxControl . UpdateLayout ( );
                                        //CollectionView view = ( CollectionView ) CollectionViewSource . GetDefaultView ( ListboxControl . ItemsSource );
                                        //view . SortDescriptions . Add ( new SortDescription ( "OrderId", ListSortDirection . Ascending ) );
                                }
                        }

                }

        }
}
