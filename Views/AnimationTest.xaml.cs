using System;
using System . CodeDom;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . Data;
using System . Data . SqlClient;
using System . Diagnostics;
using System . Threading;
using System . Windows;
using System . Windows . Automation . Peers;
using System . Windows . Controls;
using System . Windows . Documents;
using System . Windows . Input;
using System . Windows . Media;
using System . Windows . Media . Animation;
using System . Windows . Media . Imaging;
using System . Windows . Threading;

using WPFPages . UserControls;
using WPFPages . ViewModels;

namespace WPFPages . Views
{
        /// <summary>
        /// Interaction logic for AnimationTe.xaml
        /// </summary>
        public partial class AnimationTest : Window
        {
                private static int counter = 0;
                private Point MousePosition = new Point ( 0, 0 );
                private bool MouseDn = false;
                private int CurrentIndex = 0;
                private bool ComboChangeinProcess = false;
                public static List<string> treedata = new List<string> ( );

                public static ObservableCollection<TopLevel> Tlevel = new ObservableCollection<TopLevel> ( );
                public static ObservableCollection<MidLevel> Mlevel = new ObservableCollection<MidLevel> ( );
                public static ObservableCollection<BaseLevel> Blevel = new ObservableCollection<BaseLevel> ( );

                public static ObservableCollection<nwcustomer> nwc = new ObservableCollection<nwcustomer> ( );
                public static nwcustomer NwCust = new nwcustomer ( );
                Lazy<ObservableCollection<nwcustomer>> LoadCustomers = new Lazy<ObservableCollection<nwcustomer>> ( ( ) => Loadcustomers ( ) );
                private static Point currentwinpos;
                private static Point currentCtrlpos;
                private static Point menupos;

                #region std properties

                private bool IsMenuOpen
                {
                        get; set;
                }

                private bool Menu1Open
                {
                        get; set;
                }

                private bool Menu2Open
                {
                        get; set;
                }

                private bool Menu3Open
                {
                        get; set;
                }

                private bool Menu4Open
                {
                        get; set;
                }

                private bool Menu5Open
                {
                        get; set;
                }

                private bool Menu6Open
                {
                        get; set;
                }

                private bool Menu7Open
                {
                        get; set;
                }

                #endregion std properties

                private DispatcherTimer timer = new ( );
                private long _timercount;
                public long timercount
                {
                        get
                        {
                                return ( long ) _timercount;
                        }

                        set
                        {
                                _timercount = value;
                        }
                }

                private void UiElement_Click ( object sender, RoutedEventArgs e )
                {
                        UIElement uiElement = sender as UIElement;
                        //                        ShowDependencyProperties ( Btn1);
                        ImgButton . ThisControl . BtnImage . Refresh ( );
                        // //DependencyProperty dp = new DependencyProperty (this );
                        //object obj =  ImgButton.ThisControl . GetValue ( ImgButton.ImageFullOffsetProperty );
                        if ( ImgButton . ThisControl . ImgBorder . Visibility == Visibility . Hidden )
                        {
                                ImgButton . ThisControl . ImgBorder . Visibility = Visibility . Visible;
                                ImgButton . ThisControl . BtnImage . Visibility = Visibility . Visible;
                        }
                        else
                        {
                                ImgButton . ThisControl . BtnImage . Visibility = Visibility . Visible;
                                ImgButton . ThisControl . ImgBorder . BorderBrush = Brushes . Transparent;
                        }
                        ImgButton . ThisControl . BtnImage . UpdateLayout ( );
                }

                #region Full Properties

                private string menuOpenText = "Hide Menu Options";

                public string MenuOpenText
                {
                        get
                        {
                                return menuOpenText;
                        }

                        set
                        {
                                menuOpenText = value;
                        }
                }

                private string menu0Text = "Open 3D Buttons Panel";

                public string Menu0Text
                {
                        get
                        {
                                return menu0Text;
                        }

                        set
                        {
                                menu0Text = value;
                        }
                }

                private string menu1Text = "Image Buttons";

                public string Menu1Text
                {
                        get
                        {
                                return menu1Text;
                        }

                        set
                        {
                                menu1Text = value;
                        }
                }

                private string menu2Text = "Expander Demo";

                public string Menu2Text
                {
                        get
                        {
                                return menu2Text;
                        }

                        set
                        {
                                menu2Text = value;
                        }
                }

                private string menu3Text = "Panel 3";

                public string Menu3Text
                {
                        get
                        {
                                return menu3Text;
                        }

                        set
                        {
                                menu3Text = value;
                        }
                }

                private string menu4Text = "Tab Control Panel";

                public string Menu4Text
                {
                        get
                        {
                                return menu4Text;
                        }

                        set
                        {
                                menu4Text = value;
                        }
                }

                private string menu5Text = "3D Button Panel";
                public string Menu5Text
                {
                        get
                        {
                                return menu5Text;
                        }

                        set
                        {
                                menu5Text = value;
                        }
                }

                private string menu6Text = "Experiments";
                public string Menu6Text
                {
                        get
                        {
                                return menu6Text;
                        }

                        set
                        {
                                menu6Text = value;
                        }
                }

                private string menu7Text = "Unused 1";
                public string Menu7Text
                {
                        get
                        {
                                return menu7Text;
                        }

                        set
                        {
                                menu7Text = value;
                        }
                }

                #endregion Full Properties

                public AnimationTest ( )
                {
                        InitializeComponent ( );
                        Utils . SetupWindowDrag ( this );

                        Storyboard s = ( Storyboard ) TryFindResource ( "HideLeftMenu" );
                        s . Begin ( );
                        this . LeftMenuTogglePanel . Opacity = 0.0;
                        timercount = 0L;
                        timer . Interval = TimeSpan . FromMilliseconds ( 1 );
                        timer . Tick += Timer_Tick;
                        timer . Start ( );
                        this.DataContext = nwc;

                        //TreeviewDataModel tvdm = new TreeviewDataModel ( );
                        //Tlevel = TreeviewDataModel . Toplevel;
                        //Mlevel = TreeviewDataModel . Midlevel;
                        //Blevel = TreeviewDataModel . Baselevel;
                        //Tree1 . ItemsSource = null;
                        //Tree1 . Items . Clear ( );
                        //Tree1 . ItemsSource = Tlevel;

                        expander . IsExpanded = false;
                        expander2 . IsExpanded = false;
                        ImageTest1 . Refresh ( );

                }
                private void AnimWin_Loaded ( object sender, RoutedEventArgs e )
                {
                        this . LeftMenuTogglePanel . Opacity = 0.0;
                }

                #region Attached properties

                #region RowHeight
                public static double GetRowHeight ( DependencyObject obj )
                {
                        return ( double ) obj . GetValue ( RowHeightProperty );
                }

                public static void SetRowHeight ( DependencyObject obj, double value )
                {
                        obj . SetValue ( RowHeightProperty, value );
                }

                // Using a DependencyProperty as the backing store for ImageWidth.  This enables animation, styling, binding, etc...
                public static readonly DependencyProperty RowHeightProperty =
                    DependencyProperty . RegisterAttached ( "RowHeight", typeof ( double ), typeof ( AnimationTest ), new PropertyMetadata ( ( double ) 15, SetTextHeightProperty ) );

                private static void SetTextHeightProperty ( DependencyObject d, DependencyPropertyChangedEventArgs e )
                {
                        //if(e.NewValue )
                        double i = ( double ) d . GetValue ( Grid . HeightProperty );
                        d . SetValue ( TextHeightProperty, i );
                }

                #endregion

                #endregion Attached properties

                #region Dependency properties

                #region ShowBorder

                public Visibility ShowBorder
                {
                        get
                        {
                                return ( Visibility ) GetValue ( ShowBorderProperty );
                        }

                        set
                        {
                                SetValue ( ShowBorderProperty, value );
                                //                                ButtonText . Refresh ( );
                        }
                }

                public static readonly DependencyProperty ShowBorderProperty =
                        DependencyProperty . Register ( "ShowBorder",
                        typeof ( Visibility ),
                        typeof ( AnimationTest ),
                        new PropertyMetadata ( Visibility . Visible ) );

                #endregion ShowBorder

                #region TextHeight

                /// <summary>
                /// Size of the button text we store for use elsewhere
                /// </summary>
                public int TextHeight
                {
                        get
                        {
                                return ( int ) GetValue ( TextHeightProperty );
                        }

                        set
                        {
                                SetValue ( TextHeightProperty, value );
                                this . Refresh ( );
                        }
                        //set{}
                }

                public static readonly DependencyProperty TextHeightProperty =
                        DependencyProperty . Register ( "TextHeightProperty",
                        typeof ( int ),
                        typeof ( AnimationTest ),
                        new PropertyMetadata ( 40 ) );

                #endregion TextHeight

                #region TextHeightScale

                /// <summary>
                /// Set to a value of  -x to +x to shrink or stretch text on a button
                /// normally range is between 0 & 1
                /// </summary>
                public double TextHeightScale
                {
                        get
                        {
                                return ( double ) GetValue ( TextHeightScaleProperty );
                        }

                        //set { }
                        set
                        {
                                SetValue ( TextHeightScaleProperty, value );
                                this . Refresh ( );
                        }
                }

                public static readonly DependencyProperty TextHeightScaleProperty =
                        DependencyProperty . Register ( "TextHeightScale",
                        typeof ( double ),
                        typeof ( AnimationTest ),
                        new PropertyMetadata ( ( double ) 1 ), OnTextHeightScalePropertyPropertyChanged );

                private static bool OnTextHeightScalePropertyPropertyChanged ( object value )
                {
                        //			Console . WriteLine ( $"TextHeightScaleProperty = {value}" );

                        return true;
                }

                #endregion TextHeightScale

                #region TextTopZero

                /// <summary>
                /// Size of the button text we store for use elsewhere
                /// </summary>
                public Thickness TextTopZero
                {
                        get
                        {
                                return ( Thickness ) GetValue ( TextTopZeroProperty );
                        }

                        set
                        {
                                SetValue ( TextTopZeroProperty, value );
                                this . Refresh ( );
                        }
                        //set{}
                }

                public static readonly DependencyProperty TextTopZeroProperty =
                        DependencyProperty . Register ( "TextTopZero",
                        typeof ( Thickness ),
                        typeof ( AnimationTest ),
                        new PropertyMetadata ( new Thickness ( 0, 0, 0, 0 ) ) );

                #endregion TextTopZero

                #region TextWidthScale

                /// <summary>
                /// Set to a value of  -x to +x to shrink or stretch text on a button
                /// normally range is between 0 & 1
                /// </summary>
                public double TextWidthScale
                {
                        get
                        {
                                return ( double ) GetValue ( TextWidthScaleProperty );
                        }

                        set
                        {
                                SetValue ( TextWidthScaleProperty, value );
                                this . Refresh ( );
                        }
                }

                public static readonly DependencyProperty TextWidthScaleProperty =
                        DependencyProperty . Register ( "TextWidthScale",
                        typeof ( double ),
                        typeof ( AnimationTest ),
                        new PropertyMetadata ( ( double ) 1.0 ), OnTextWidthScalePropertyChanged );

                private static bool OnTextWidthScalePropertyChanged ( object value )
                {
                        //			Console . WriteLine ( $"TextWidthScaleProperty   = {value}" );
                        return true;
                }

                #endregion TextWidthScale

                #region TextShadowColor (UNUSED)

                //public Color TextShadowColor
                //{
                //        get
                //        {
                //                return ( Color ) GetValue ( TextShadowColorProperty );
                //        }

                //        set
                //        {
                //                SetValue ( TextShadowColorProperty, value );
                //                this . Refresh ( );
                //        }
                //        //set{}
                //}

                //public static readonly DependencyProperty TextShadowColorProperty =
                //        DependencyProperty . Register ( "TextShadowColor",
                //        typeof ( Color ),
                //        typeof ( AnimationTest ),
                //        new PropertyMetadata ( Colors . DarkGray ) );

                #endregion TextShadowColor

                #region TextShadowOpacity (UNUSED)

                //public double TextShadowOpacity
                //{
                //        get
                //        {
                //                return ( double ) GetValue ( TextShadowOpacitProperty );
                //        }

                //        set
                //        {
                //                SetValue ( TextShadowOpacitProperty, value );
                //                this . Refresh ( );
                //        }
                //}

                //public static readonly DependencyProperty TextShadowOpacitProperty =
                //        DependencyProperty . Register ( "TextShadowOpacity",
                //        typeof ( double ),
                //        typeof ( AnimationTest ),
                //        new PropertyMetadata ( ( double ) 0.5 ), OnTextShadowOpacityProperty );

                //private static bool OnTextShadowOpacityProperty ( object value )
                //{
                //        return true;
                //}

                #endregion TextShadowOpacity

                #region TextShadowRadius (UNUSED)

                //public double TextShadowRadius
                //{
                //        get
                //        {
                //                return ( double ) GetValue ( TextShadowRadiusProperty );
                //        }

                //        //set { }
                //        set
                //        {
                //                SetValue ( TextShadowRadiusProperty, value );
                //                this . Refresh ( );
                //        }
                //}

                //public static readonly DependencyProperty TextShadowRadiusProperty =
                //        DependencyProperty . Register ( "TextShadowRadius",
                //        typeof ( double ),
                //        typeof ( AnimationTest ),
                //        new PropertyMetadata ( ( double ) 1 ), OnTextShadowRadiusProperty );

                //private static bool OnTextShadowRadiusProperty ( object value )
                //{
                //        return true;
                //}

                #endregion TextShadowRadius

                #region TextShadowSize (UNUSED)
                //public double TextShadowSize
                //{
                //        get
                //        {
                //                return ( double ) GetValue ( TextShadowSizeProperty );
                //        }

                //        //set { }
                //        set
                //        {
                //                SetValue ( TextShadowSizeProperty, value );
                //                this . Refresh ( );
                //        }
                //}

                //public static readonly DependencyProperty TextShadowSizeProperty =
                //        DependencyProperty . Register ( "TextShadowSize",
                //        typeof ( double ),
                //        typeof ( AnimationTest ),
                //        new PropertyMetadata ( ( double ) 2 ), OnTextShadowSizePropertyProperty );

                //private static bool OnTextShadowSizePropertyProperty ( object value )
                //{
                //        //			Console . WriteLine ( $"ShadowBlurSizeProperty = {value}" );

                //        return true;
                //}

                #endregion TextShadowSize

                #endregion Dependency properties

                #region Load NW Data from SQl server

                public static ObservableCollection<nwcustomer> Loadcustomers ( )
                {
                        DataTable dt = new DataTable ( "Customers" );
                        string ConString = ( string ) Properties . Settings . Default [ "NorthwindConnectionString" ];

                        string CmdString = string . Empty;
                        try
                        {
                                using ( SqlConnection con = new SqlConnection ( ConString ) )
                                {
                                        CmdString = $"SELECT *  FROM Customers ";
                                        SqlCommand cmd = new SqlCommand ( CmdString, con );
                                        SqlDataAdapter sda = new SqlDataAdapter ( cmd );
                                        sda . Fill ( dt );
                                }
                        }
                        catch ( Exception ex )
                        {
                                Debug . WriteLine ( $"Data={ex . Data}, {ex . Message}\n[{CmdString}]" );
                        }
                        CreateCustCollection ( dt );
                        return nwc;
                }

                public static bool CreateCustCollection ( DataTable dt )
                {
                        int count = 0;
                        try
                        {
                                for ( int i = 0 ; i < dt . Rows . Count ; i++ )
                                {
                                        nwc . Add ( new nwcustomer
                                        {
                                                CustomerId = dt . Rows [ i ] [ 0 ] . ToString ( ),
                                                CompanyName = dt . Rows [ i ] [ 1 ] . ToString ( ),
                                                ContactName = dt . Rows [ i ] [ 2 ] . ToString ( ),
                                                ContactTitle = dt . Rows [ i ] [ 3 ] . ToString ( ),
                                                Address = dt . Rows [ i ] [ 4 ] . ToString ( ),
                                                Fax = dt . Rows [ i ] [ 10 ] . ToString ( ),
                                                Region = dt . Rows [ i ] [ 6 ] . ToString ( ),
                                                Country = dt . Rows [ i ] [ 8 ] . ToString ( ),
                                                PostalCode = dt . Rows [ i ] [ 7 ] . ToString ( ),
                                                Phone = dt . Rows [ i ] [ 9 ] . ToString ( ),
                                                City = dt . Rows [ i ] [ 5 ] . ToString ( ),
                                        } );
                                        count = i;
                                }
                        }
                        catch ( Exception ex )
                        {
                                Debug . WriteLine ( $"DETAILS : ERROR in  LoadDetCollection() : loading Details into ObservableCollection \"DetCollection\" : [{ex . Message}] : {ex . Data} ...." );
                                MessageBox . Show ( $"DETAILS : ERROR in  LoadDetCollection() : loading Details into ObservableCollection \"DetCollection\" : [{ex . Message}] : {ex . Data} ...." );
                                return false;
                        }
                        finally
                        {
                        }
                        return true;
                }

                #endregion Load NW Data from SQl server

                private void Timer_Tick ( object sender, EventArgs e )
                {
                        // this is called every millisecond by dispatchTimer
                        if ( timercount >= long . MaxValue )
                                timercount = 0;
                        timercount++;
                        //			Console . WriteLine ( $"{timercount}" );
                }

                #region menu Bar Control

                /// <summary>
                /// Makes the Button menu disappear/Reappear to/from bottom right
                /// </summary>
                /// <param name="sender"></param>
                /// <param name="e"></param>
                private void Image_MouseLeftButtonDown ( object sender, MouseButtonEventArgs e )
                {
                        Storyboard s = null;
                        if ( counter == 0 )
                        {
                                //Hide menu
                                s = ( Storyboard ) TryFindResource ( "SlideStackPanelRight" );
                                var uri = new Uri ( "/Icons/left-arrow.png", UriKind . Relative );
                                var bitmap = new BitmapImage ( uri );
                                this . OpenButton . Source = bitmap;
                                OpenButton.ToolTip="Click to open 3DButton Control Menu";
//                                this . myimage . Visibility = Visibility . Visible;
                                 this . myimage2 . Visibility = Visibility . Visible;
                                s . Begin ( );
                               counter ++;
                        }
                        else
                        {
                                //Show menu
                                 var uri = new Uri ( "/Icons/right-arrow.png", UriKind . Relative );
                                var bitmap = new BitmapImage ( uri );
                                this . OpenButton . Source = bitmap;
                                OpenButton . ToolTip = "Click to close 3DButton Control Menu";
   //                             this . myimage . Visibility = Visibility . Visible;
                                this . myimage2 . Visibility = Visibility . Visible;
                                this . RectbuttonStackPanel . Visibility = Visibility . Visible;
                                s = ( Storyboard ) TryFindResource ( "SlideStackPanelLeft" );
                                s . Begin ( );
                                counter = 0;
                        }
                        try
                        {
                                if ( s != null )
                                        s . Begin ( );
                        }
                        catch
                        {
                        }
                        return;
                }

                #endregion menu Bar Control

                #region scale changing

                //		private void TestRectangle_MouseLeftButtonUp ( object sender, MouseButtonEventArgs e )
                //		{
                ////			Btn1 . TextWidthScale += 0.1;
                //			this . TestEllipse . TextWidthScale += 0.1;
                //			this . TestEllipse_Copy . TextWidthScale += 0.1;
                //			this . Btn2_Copy . TextWidthScale += 0.1;
                ////			this . ScaleDimensions . Text = $"{this . Btn1 . TextHeightScale } : {this . Btn1 . TextWidthScale }";
                //		}

                private void TextScaleDn ( object sender, MouseButtonEventArgs e )
                {
                        this . ToolBtn1 . TextWidthScale -= 0.1;
                        this . TestEllipse . TextWidthScale -= 0.1;
                        this . TestEllipse_Copy . TextWidthScale -= 0.1;
                        this . Btn2_Copy . TextWidthScale -= 0.1;
                        this . ScaleDimensions . Text = $"{this . ToolBtn1 . TextHeightScale } : {this . ToolBtn1 . TextWidthScale }";
                }

                private void TextScaleUp ( object sender, MouseButtonEventArgs e )
                {
                        this . ToolBtn1 . TextWidthScale += 0.1;
                        this . TestEllipse . TextWidthScale += 0.1;
                        this . TestEllipse_Copy . TextWidthScale += 0.1;
                        this . Btn2_Copy . TextWidthScale += 0.1;
                        this . ScaleDimensions . Text = $"{this . ToolBtn1 . TextHeightScale } : {this . ToolBtn1 . TextWidthScale }";
                }

                private void TextHeightScaleUp ( object sender, MouseButtonEventArgs e )
                {
                        this . ToolBtn1 . TextHeightScale += 0.1;
                        this . TestEllipse . TextHeightScale += 0.1;
                        this . TestEllipse_Copy . TextHeightScale += 0.1;
                        this . Btn2_Copy . TextHeightScale += 0.1;
                        this . ScaleDimensions . Text = $"{this . ToolBtn1 . TextHeightScale } : {this . ToolBtn1 . TextWidthScale }";
                }

                private void TextHeightScaleDn ( object sender, MouseButtonEventArgs e )
                {
                        this . ToolBtn1 . TextHeightScale -= 0.1;
                        this . TestEllipse . TextHeightScale -= 0.1;
                        this . TestEllipse_Copy . TextHeightScale -= 0.1;
                        this . Btn2_Copy . TextHeightScale -= 0.1;
                        this . ScaleDimensions . Text = $"{this . ToolBtn1 . TextHeightScale } : {this . ToolBtn1 . TextWidthScale }";
                }

                #endregion scale changing

                #region Drag functionality

                private void DragEllipse ( object sender, MouseButtonEventArgs e )
                {
                        MousePosition = Mouse . GetPosition ( LeftPanel );
                        MouseDn = true;
                }

                private void Btn1_MouseMove ( object sender, MouseEventArgs e )
                {
                        if ( MouseDn )
                        {
                                ThreeDeeBtnControl tdbc = sender as ThreeDeeBtnControl;
                                if ( tdbc == null )
                                        return;
                        }
                }

                private void DrqagEllipseStop ( object sender, MouseButtonEventArgs e )
                {
                        MouseDn = false;
                }

                #endregion Drag functionality

                private void Togglepanel ( object sender, MouseButtonEventArgs e )
                {
                        if ( LeftPanel . Visibility == Visibility . Visible )
                        {
                                this . LeftPanel . Visibility = Visibility . Hidden;
                                this . TabControlPanel . Visibility = Visibility . Visible;
                                //You can use this format almost anywhere to changfe a Dependency Poperty
                        }
                        else
                        {
                                this . LeftPanel . Visibility = Visibility . Visible;
                                this . TabControlPanel . Visibility = Visibility . Hidden;
                        }
                }

                private void Toggle_Click ( object sender, RoutedEventArgs e )
                {
                        if ( this . LeftPanel . Visibility == Visibility . Visible )
                        {
                                this . LeftPanel . Visibility = Visibility . Hidden;
                                this . TabControlPanel . Visibility = Visibility . Visible;
                        }
                        else
                        {
                                this . LeftPanel . Visibility = Visibility . Visible;
                                this . TabControlPanel . Visibility = Visibility . Hidden;
                        }
                }

                private void Tb1_MouseLeftButtonDown ( object sender, MouseButtonEventArgs e )
                {
                        if ( this . SlideOutMenu . Width > 0 )
                        {
                                Storyboard s = ( Storyboard ) TryFindResource ( "HideLeftMenu" );
                                s . Begin ( );
                                //				SlideOutMenu . Visibility = Visibility . Visible;
                                this . Tb1 . Visibility = Visibility . Visible;
                                //  this . SlideOutMenu . Visibility = Visibility . Hidden;
                        }
                        else
                        {
                                this . SlideOutMenu . Visibility = Visibility . Visible;
                                Storyboard s = ( Storyboard ) TryFindResource ( "ShowLeftMenu" );
                                s . Begin ( );
                                //				SlideOutMenu . Visibility = Visibility . Collapsed;
                                this . Tb1 . Visibility = Visibility . Hidden;
                        }
                }

                private void Menu1_Click ( object sender, MouseButtonEventArgs e )
                {
                        this . Panel2 . Visibility = Visibility . Hidden;
                        //			this . Panel3 . Visibility = Visibility . Hidden;
                        this . Panel3 . Visibility = Visibility . Hidden;
                        this . Panel4 . Visibility = Visibility . Hidden;
                        this . TabControlPanel . Visibility = Visibility . Hidden;
                        this . ThreeDButtonsPanel . Visibility = Visibility . Hidden;

                        this . Panel1 . Visibility = Visibility . Visible;
                        this . Title = "Image Buttons visible";
                }

                private void Menu2_Click ( object sender, MouseButtonEventArgs e )
                {
                        this . Panel1 . Visibility = Visibility . Hidden;
                        this . Panel3 . Visibility = Visibility . Hidden;
                        this . Panel4 . Visibility = Visibility . Hidden;
                        this . TabControlPanel . Visibility = Visibility . Hidden;
                        this . ThreeDButtonsPanel . Visibility = Visibility . Hidden;

                        this . Panel2 . Visibility = Visibility . Visible;
                        this . Title = "Expander Demo visible";
                }

                private void Menu3_Click ( object sender, MouseButtonEventArgs e )
                {
                        this . Panel1 . Visibility = Visibility . Hidden;
                        this . Panel2 . Visibility = Visibility . Hidden;
                        this . Panel3 . Visibility = Visibility . Hidden;
                        this . Panel4 . Visibility = Visibility . Hidden;
                        this . TabControlPanel . Visibility = Visibility . Hidden;
                        this . ThreeDButtonsPanel . Visibility = Visibility . Hidden;

                        this . Panel3 . Visibility = Visibility . Visible;
                        TreeviewDataModel tvdm = new TreeviewDataModel ( );

                        Tlevel = TreeviewDataModel . Toplevel;
                        Mlevel = TreeviewDataModel . Midlevel;
                        Blevel = TreeviewDataModel . Baselevel;
                        Tree1 . ItemsSource = null;
                        Tree1 . Items . Clear ( );
                        Tree1 . ItemsSource = Tlevel;
                        //Tree1 . DataContext = treedata;
                        Title = "Panel3 (TreeView) visible";
                }

                private void Menu4_Click ( object sender, MouseButtonEventArgs e )
                {
                        this . Panel1 . Visibility = Visibility . Hidden;
                        this . Panel2 . Visibility = Visibility . Hidden;
                        this . Panel3 . Visibility = Visibility . Hidden;
                        this . Panel4 . Visibility = Visibility . Hidden;
                        this . ThreeDButtonsPanel . Visibility = Visibility . Hidden;

                        this . TabControlPanel . Visibility = Visibility . Visible;
                        this . LeftslidingMenuMarker . Visibility = Visibility . Visible;
                        this . Title = "TabControlPanel visible";
                }

                private void Menu5_Click ( object sender, MouseButtonEventArgs e )
                {
                        this . Panel1 . Visibility = Visibility . Hidden;
                        this . Panel2 . Visibility = Visibility . Hidden;
                        this . Panel3 . Visibility = Visibility . Hidden;
                        this . Panel4 . Visibility = Visibility . Hidden;
                        this . TabControlPanel . Visibility = Visibility . Hidden;

                        this . ThreeDButtonsPanel . Visibility = Visibility . Visible;

                        this . Title = "ThreeDButtonsPanel visible";
                }

                private void Menu6_Click ( object sender, MouseButtonEventArgs e )
                {
                        this . Panel1 . Visibility = Visibility . Hidden;
                        this . Panel2 . Visibility = Visibility . Hidden;
                        this . Panel3 . Visibility = Visibility . Hidden;
                        this . TabControlPanel . Visibility = Visibility . Hidden;
                        this . ThreeDButtonsPanel . Visibility = Visibility . Hidden;

                        this . Panel4 . Visibility = Visibility . Visible;
                        this . Title = "Mixed buttons visible";
                }

                private void Menu7_Click ( object sender, MouseButtonEventArgs e )
                {
                        this . Panel1 . Visibility = Visibility . Hidden;
                        this . Panel2 . Visibility = Visibility . Hidden;
                        this . Panel3 . Visibility = Visibility . Hidden;
                        this . Panel4 . Visibility = Visibility . Hidden;
                        this . TabControlPanel . Visibility = Visibility . Hidden;
                        this . ThreeDButtonsPanel . Visibility = Visibility . Hidden;
                        this . Title = "Blank panel";
                }
       
                private void Expand_MenuItemBorder0 ( object sender, MouseEventArgs e )
                {
                }

                private void MenuOpen_Click ( object sender, MouseButtonEventArgs e )
                {
                        if ( Menu2 . Visibility == Visibility . Visible )
                        {
                                ShowMenuOptions ( null, false );
                                //				this.Text = "Show Menu Options";
                                this . SlideOutMenu . Background = new SolidColorBrush ( Colors . Transparent );
                                //				SlideOutMenu. Height = 25;
                        }
                        else
                        {
                                ShowMenuOptions ( null, true );
                                //				Text= "Show Menu Options";
                                //				MenuOpen . Text = "Hide Menu Options";
                                this . SlideOutMenu . Background = ( Brush ) FindResource ( "Black5" );
                                //				SlideOutMenu . Height = 500;
                        }
                }

                private void Expand_MenuItemBorderOpen ( object sender, MouseEventArgs e )
                {
                        //if ( Menu1 . Visibility == Visibility . Visible )
                        //	ShowMenuOptions ( null, false );
                        //else
                        ShowMenuOptions ( null, true );
                }

                private void ShowMenuOptions ( Border menu, bool open )
                {
                        if ( menu == null && open )
                        {
                                this . Menu1 . Visibility = Visibility . Visible;
                                this . Menu2 . Visibility = Visibility . Visible;
                                this . Menu3 . Visibility = Visibility . Visible;
                                this . Menu4 . Visibility = Visibility . Visible;
                                this . Menu5 . Visibility = Visibility . Visible;
                                this . Menu6 . Visibility = Visibility . Visible;
                                this . Menu7 . Visibility = Visibility . Visible;
                                return;
                        }
                        else if ( menu == null && open == false )
                        {
                                this . Menu1 . Visibility = Visibility . Collapsed;
                                this . Menu2 . Visibility = Visibility . Collapsed;
                                this . Menu3 . Visibility = Visibility . Collapsed;
                                this . Menu4 . Visibility = Visibility . Collapsed;
                                this . Menu5 . Visibility = Visibility . Collapsed;
                                this . Menu6 . Visibility = Visibility . Collapsed;
                                this . Menu7 . Visibility = Visibility . Collapsed;
                                return;
                        }
                        this . Menu1 . Visibility = Visibility . Collapsed;
                        this . Menu2 . Visibility = Visibility . Collapsed;
                        this . Menu3 . Visibility = Visibility . Collapsed;
                        this . Menu4 . Visibility = Visibility . Collapsed;
                        this . Menu5 . Visibility = Visibility . Collapsed;
                        this . Menu6 . Visibility = Visibility . Collapsed;
                        this . Menu7 . Visibility = Visibility . Collapsed;
                        this . AnimWin . Refresh ( );
                        //if ( menu == Menu1 && open )
                        //{
                        //	Menu1 . Visibility = Visibility . Visible;
                        //}
                        //if ( menu == Menu2 && open )
                        //{
                        //	Menu2 . Visibility = Visibility . Visible;
                        //}
                        //else if ( menu == Menu3 && open )
                        //{
                        //	Menu3 . Visibility = Visibility . Visible;
                        //}
                        //else if ( menu == Menu4 && open )
                        //{
                        //	Menu4 . Visibility = Visibility . Visible;
                        //}
                        //else if ( menu == Menu5 && open )

                        //{
                        //	Menu6 . Visibility = Visibility . Visible;
                        //}
                        //else if ( menu == Menu6 && open )
                        //{
                        //	Menu6 . Visibility = Visibility . Visible;
                        //}
                        //else if ( menu == Menu7 && open )
                        //{
                        //	Menu7 . Visibility = Visibility . Visible;
                        //}
                }

                private void Expanded ( Border border, TextBlock textblock, string originaltext, string newtext )
                {
                        double dtemp = 0;
                        textblock . Text = "";
                        textblock . Height = 0;
                        textblock . Text = originaltext + newtext;
                        textblock . Refresh ( );
                        dtemp = textblock . ActualHeight + 15;
                        border . Height = dtemp;
                        textblock . Height = dtemp;
                        textblock . Refresh ( );
                        border . Height = dtemp + 15;
                        border . Refresh ( );
                          Thickness t = textblock . Margin;
                        t . Top = 0;
                        t . Bottom = 0;
                        t . Left = 0;
                        t . Right = 0;
                        textblock . Margin = t;
                        textblock . Refresh ( );
                        IsMenuOpen = true;
                        border . Refresh ( );
                        Console . WriteLine ( $"Menu {border . Name} Opened ...." );
                        ReSetOpenFlags ( border, true );
                }

                private void Contract ( Border border, TextBlock textblock, string originaltext )
                {
                        if ( CheckCallerOpen ( border ) == false )
                                return;
                         textblock . Text = "";
                        textblock . Height = 0;
                        border . Height = 0;
                        textblock . Text = originaltext;
                        textblock . Refresh ( );
                         border . Height = 0;
                        border . Refresh ( );
                        border . Height = textblock . ActualHeight + 15;
                        textblock . Height = border . Height;
                        textblock . Refresh ( );
                        Thickness t = textblock . Margin;
                        t . Top = 0;
                        t . Bottom = 0;
                        t . Left = 0;
                        t . Right = 0;
                        textblock . Margin = t;
                        textblock . Refresh ( );
                        IsMenuOpen = false;
                        Console . WriteLine ( $"Menu {border . Name} Closed ...." );
                        ReSetOpenFlags ( border, false );
                }

                private bool CheckCallerOpen ( Border border )
                {
                        //if ( border == Menu1 )
                        //	return Menu1Open;
                        //if ( border == Menu2 )
                        //	return Menu2Open;
                        //else if ( border == Menu3 )
                        //	return Menu3Open;
                        //else if ( border == Menu4 )
                        //	return Menu4Open;
                        //else if ( border == Menu5 )
                        //	return Menu5Open;
                        //else if ( border == Menu6 )
                        //	return Menu6Open;
                        //else if ( border == Menu7 )
                        //	return Menu7Open;
                        return false;
                }

                private void ReSetOpenFlags ( Border border, bool setting )
                {
                        if ( setting == true )
                        {
                                this . Menu1Open = false;
                                this . Menu2Open = false;
                                this . Menu3Open = false;
                                this . Menu4Open = false;
                                this . Menu5Open = false;
                                this . Menu6Open = false;
                                this . Menu7Open = false;
                        }

                        //if ( border == Menu1 )
                        //	Menu1Open = setting;
                        //if ( border == Menu2 )
                        //	Menu2Open = setting;
                        //else if ( border == Menu3 )
                        //	Menu3Open = setting;
                        //else if ( border == Menu4 )
                        //	Menu4Open = setting;
                        //else if ( border == Menu5 )
                        //	Menu5Open = setting;
                        //else if ( border == Menu6 )
                        //	Menu5Open = setting;
                        //else if ( border == Menu7 )
                        //	Menu7Open = setting;
                }

                private void SlideMenu_mouseEnter ( object sender, MouseEventArgs e )
                {
                        this . LeftMenuTogglePanel . Opacity = 1;
                }

                private void ItemContainerGenerator_StatusChanged ( object sender, EventArgs e )
                {
                        //ListBoxItem lbi = ( ListBoxItem ) lv1 . ItemContainerGenerator . ContainerFromIndex ( lv1 . SelectedIndex ) as ListBoxItem;
                        //if ( lbi != null )
                        //{
                        //        //                                lv1.Items.B
                        //        Console . WriteLine ( $"lv1 Selected : lbi={lbi}, IsSelected = {lbi . IsSelected}, Index = {lv1 . SelectedIndex}" );
                        //        //  lbi . Background = ( Brush ) FindResource ( "Red5" );
                        //        lbi . Foreground = ( Brush ) FindResource ( "Magenta1" );
                        //        lv1 . Refresh ( );
                        //}
                }

                #region Expander handlers
                private void expander_Expanded ( object sender, RoutedEventArgs e )
                {
                        Expander ex = sender as Expander;
                        expander2 . Visibility = Visibility . Collapsed;
                        ex . BringIntoView ( );
                        expander . Height = 250;


                }

                private void expander_Collapsed ( object sender, RoutedEventArgs e )
                {
                        expander . Height = 45;
                        Expander ex = sender as Expander;
                        expander2 . Visibility = Visibility . Visible;
                        ex . BringIntoView ( );
                }
                private void lv1_Loaded ( object sender, RoutedEventArgs e )
                {
                        if ( lv1 . Items . Count > 1 && dg1 . Items . Count > 1 )
                                return;
                        if ( lv1 . SelectedItem == null && lv1 . Items . Count == 1 )
                        {
                                lv1 . Items . Clear ( );
                        }
                        if ( lv2 . SelectedItem == null && lv2 . Items . Count == 1 )
                        {
                                lv2 . Items . Clear ( );
                        }
                        //==================
                        // This is being LAZY LOADED
                        //==================
                        nwc = LoadCustomers . Value;

                        dg1 . ItemsSource = null;
                        dg1 . ItemsSource = nwc;
                        lv1 . ItemsSource = null;
                        lv1 . ItemsSource = nwc;
                        lv1 . SelectedIndex = 0;
                        lv2 . ItemsSource = null;
                        lv2 . ItemsSource = nwc;
                        lv2 . SelectedIndex = 0;
                        cb1 . ItemsSource = nwc;
                        cb2 . ItemsSource = nwc;
                        cb3 . ItemsSource = nwc;
                        cb4 . ItemsSource = nwc;
                        lv1 . ItemContainerGenerator . StatusChanged += ItemContainerGenerator_StatusChanged;
                }

                private void expander2_Expanded ( object sender, RoutedEventArgs e )
                {
                        Expander ex = sender as Expander;
                        ex . BringIntoView ( );
                        expander2 . Height = 350;
                        if ( grid1 != null )
                                grid1 . Height = 290;
                        if ( lv1 != null )
                                lv1 . Height = 225;
                }

                private void expander2_Collapsed ( object sender, RoutedEventArgs e )
                {
                        expander2 . Height = 45;
                }

                private void cb2_SelectionChanged ( object sender, SelectionChangedEventArgs e )
                {
                        lv1 . SelectedIndex = cb2 . SelectedIndex;
                        cb1 . SelectedIndex = lv1 . SelectedIndex;
                        cb2 . SelectedIndex = lv1 . SelectedIndex;
                }

                private void cb_SelectionChanged ( object sender, SelectionChangedEventArgs e )
                {
                        int select = 0;
                        //                       if ( ComboChangeinProcess )
                        //                            return;
                        if ( ( ComboBox ) sender == cb1 )
                        {
                                ComboChangeinProcess = true;
                                lv1 . SelectedIndex = cb1 . SelectedIndex;
                                select = cb1 . SelectedIndex;
                                cb2 . SelectedIndex = cb1 . SelectedIndex;
                                lv1 . BringIntoView ( );
                                Utils . ScrollLBRecordIntoView ( lv1, cb1 . SelectedIndex );
                                lv1 . Refresh ( );
                        }
                        else
                        {
                                ComboChangeinProcess = true;
                                lv1 . SelectedIndex = cb2 . SelectedIndex;
                                select = cb2 . SelectedIndex;
                                cb1 . SelectedIndex = cb2 . SelectedIndex;
                                lv1 . BringIntoView ( );
                                Utils . ScrollLBRecordIntoView ( lv1, cb2 . SelectedIndex );
                        }
                        ComboChangeinProcess = false;
                        //lbi = ( ListBoxItem ) lv1. ItemContainerGenerator . ContainerFromIndex ( select ) as ListBoxItem ;
                }
                private void lv1_Selected ( object sender, SelectionChangedEventArgs e )
                {
                        // change colour of newly elected item
                        ListBox lb = sender as ListBox;
                        //ListBoxItem lbi = new ListBoxItem();

                        //// First reset last selected item
                        //lbi = ( ListBoxItem ) ( lb . ItemContainerGenerator . ContainerFromIndex ( CurrentIndex) );
                        //if ( lbi == null )
                        //        return;
  
                        ComboChangeinProcess = true;
                        cb1 . SelectedIndex = lb . SelectedIndex;
                        cb2 . SelectedIndex = lb . SelectedIndex;
                        CurrentIndex = lb . SelectedIndex;
                  }

                private void cb2_Selected ( object sender, RoutedEventArgs e )
                {
                }
                private void cb1_Loaded ( object sender, RoutedEventArgs e )
                {
                        cb1 . SelectedIndex = 0;
                }

                private void cb2_Loaded ( object sender, RoutedEventArgs e )
                {
                        cb2 . SelectedIndex = 0;
                }

                private void cb3_Loaded ( object sender, RoutedEventArgs e )
                {
                        cb3 . SelectedIndex = 0;
                }

                private void cb4_Loaded ( object sender, RoutedEventArgs e )
                {
                        cb4 . SelectedIndex = 0;
                }

                private ListBoxItem GetLbItem ( ListBox lb )
                {
                        if ( lb . Items . Count == 0 )
                                return null;

                        var index = lb . SelectedIndex;
                        if ( index < 0 )
                                return null;

                        ListBoxItem item = lb . ItemContainerGenerator . ContainerFromIndex ( index ) as ListBoxItem;
                        if ( item == ( ListBoxItem ) null )
                                return null;

                        return item;
                }

                #endregion

                private void Exit_Window ( object sender, MouseButtonEventArgs e )
                {
                        this . Close ( );
                }
        }
}
