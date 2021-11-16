using System . ComponentModel;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Input;
using System . Windows . Media;
using System . Windows . Shapes;

using WPFPages . UserControls;

namespace WPFPages . Views
{
        /// <summary>
        /// Interaction logic for ThreeDeeBtnControl.xaml
        /// </summary>
        public partial class ThreeDeeBtnControl : UserControl
        {
                /// How to pass a click event thru to end user of any UserControl
                // Must declare the event
                public event RoutedEventHandler Click;
                
                // This passes the Click event (from ThreeDeeBtnControl) on to user windows
                private void OnButtonClick ( object sender, RoutedEventArgs e )
                {
                        if ( this . Click != null )
                        {
                                this . Click ( this, e );
                        }
                }
                public LinearGradientBrush lgb
                {
                        get; set;
                }

                //		private GradientDisplay gw = null;
                public bool Loading = true;

                private LinearGradientBrush brush1;
                public static Ellipse H1;
                public static Ellipse H2;
                public static Ellipse H3;
                public static Ellipse Buttonside;
                public static Ellipse Pressed;

                //#############################

                #region Size CONTROL Variables

                //#############################

                public bool Startup = false;
                //#############################

                #endregion Size CONTROL Variables

                //#############################

                //#############################

                #region colors for use in system

                //#############################
                public LinearGradientBrush Brush1
                {
                        get
                        {
                                return brush1;
                        }

                        set
                        {
                                brush1 = value;
                                OnPropertyChanged ( "Brush1" );
                        }
                }

                //		private LinearGradientBrush brush2;
                public Brush BorderGreen;

                public Brush BorderYellow;
                public Brush BorderOrange;
                public Brush BorderBlue;
                public Brush BorderBlack;

                //#############################

                #endregion colors for use in system

                //#############################

                //#############################

                #region internal 'color' variables

                //#############################

                public int GradientStyle
                {
                        get
                        {
                                return GradientStyle;
                        }

                        set
                        {
                                GradientStyle = value;
                                InvalidateVisual ( );
                        }
                }

                //#############################

                #endregion internal 'color' variables

                //#############################

                public ThreeDeeBtnControl ( )
                {
                        this . DataContext = this;
                        InitializeComponent ( );
                        //DoErrorBeep ( 280, 100, 3 );
                }
                
                #region startup
                private void ThreeDBttn_Loaded ( object sender, RoutedEventArgs e )
                {
                        this . DataContext = this;
                        lgb = new LinearGradientBrush ( );
                        Brush1 = ( LinearGradientBrush ) FindResource ( "Greenbackground" );
                        BorderGreen = new SolidColorBrush ( Colors . Green );
                        BorderBlack = new SolidColorBrush ( Colors . Black );
                        SetStartupDefaults ( );
                        {
                                ReportStatus ( );
                                DataContext = this;
                        }
                        // Store our buttons size into DP's
                        ThreeDeeBtnControl tdb = sender as ThreeDeeBtnControl;
                        ControlHeight = ( int ) tdb . ActualHeight;
                        ControlWidth = ( int ) tdb . ActualWidth;

                        // Calculate an offset for the button hole to avoid too much push down on smaller height buttons
                        // works quite well PressedHoleHeight is read  by Mouseover "Setter" to set the hole's height
                        int calcheight = ( int ) tdb . ActualHeight;
                        if ( ControlWidth > 500 )
                                calcheight = ControlHeight;
                        else if ( ControlWidth > 400 )
                                calcheight = ControlHeight - 10;                //	#4
                        else if ( ControlWidth > 300 )
                                calcheight = ControlHeight - 12;   // #1
                        else if ( ControlWidth > 200 )
                                calcheight = ControlHeight - 15;        //	#3
                        else if ( ControlWidth > 150 )
                                calcheight = ControlHeight - 17;
                        else if ( ControlWidth > 100 )
                                calcheight = ControlHeight - 19;                // Tab control
                        else if ( ControlWidth > 75 )
                                calcheight = ControlHeight - 22;                //  #2 smallest
                        else if ( ControlWidth > 50 )
                                calcheight = ControlHeight - 25;
                        else if ( ControlWidth > 20 )
                                calcheight = ControlHeight - 30;
                        PressedHoleHeight = calcheight;
                        //SetValue ( BtnTextProperty, "here ya go !");
                }

                private void SetStartupDefaults ( )
                {
                        // Startup Setup
                        ThreeDBtn . Foreground = BtnTextColor;
                        ThreeDBtn . FontSize = TextHeight == 0 ? 22 : TextHeight;
                        H1 = new Ellipse ( );
                        H2 = new Ellipse ( );
                        H3 = new Ellipse ( );
                        H1 . Fill = FillTop;
                        H2 . Fill = FillSide;
                        H3 . Fill = FillHole;
                        ThreeDBttn . Height = Height;
                        ThreeDBttn . Width = Width;
                }
                #endregion

  
                #region Dependencies

                //#############################

                #region BtnTextColor
                public Brush BtnTextColor
                {
                        get
                        {
                                return ( Brush ) GetValue ( BtnTextColorProperty );
                        }
                       set
                        {
                                SetValue ( BtnTextColorProperty, value );
                          }
                }

                public static readonly DependencyProperty BtnTextColorProperty =
                        DependencyProperty . Register ( "BtnTextColor",
                        typeof ( Brush ),
                        typeof ( ThreeDeeBtnControl ),
                        new PropertyMetadata ( new SolidColorBrush ( Colors . Black ) ) );

                #endregion BtnTextColor

                #region ControlHeight
                 public int ControlHeight
                {
                        get
                        {
                                return ( int ) GetValue ( ControlHeightProperty );
                        }
                       set
                        {
                                SetValue ( ControlHeightProperty, value );
                        }
                }

                public static readonly DependencyProperty ControlHeightProperty =
                        DependencyProperty . Register ( "ControlHeight",
                        typeof ( int ),
                        typeof ( ThreeDeeBtnControl ),
                        new FrameworkPropertyMetadata ( 75, new PropertyChangedCallback ( OnControlHeightChanged ) ) );

                private static void OnControlHeightChanged ( DependencyObject d, DependencyPropertyChangedEventArgs e )
                {
                 }

                #endregion ControlHeight

                #region ControlWidth
            public int ControlWidth
                {
                        get
                        {
                                return ( int ) GetValue ( ControlWidthProperty );
                        }
                       set
                        {
                                SetValue ( ControlWidthProperty, value );
                         }
                }

                public static readonly DependencyProperty ControlWidthProperty =
                        DependencyProperty . Register ( "ControlWidth",
                        typeof ( int ),
                        typeof ( ThreeDeeBtnControl ),
                        new PropertyMetadata ( 100 ), OnControlWidthChanged );

                private static bool OnControlWidthChanged ( object value )
                {
                        return true;
                }

                #endregion ControlWidth

                #region FillSide
             public Brush FillSide
                {
                        get
                        {
                                return ( Brush ) GetValue ( FillSideProperty );
                        }
                        set
                        {
                                SetValue ( FillSideProperty, value );
                        }
                        //set{}
                }

                public static readonly DependencyProperty FillSideProperty =
                        DependencyProperty . Register ( "FillSide",
                        typeof ( Brush ),
                        typeof ( ThreeDeeBtnControl ),
                        new PropertyMetadata ( new SolidColorBrush ( Colors . Green) ), OnFillSideChangedCallBack );

                private static bool OnFillSideChangedCallBack ( object value )
                {
                         return true;
                }

                #endregion FillSide

                #region FillHole
                public Brush FillHole
                {
                        get
                        {
                                return ( Brush ) GetValue ( FillHoleProperty );
                        }
                  set
                        {
                                SetValue ( FillHoleProperty, value );
                         }
                }

                public static readonly DependencyProperty FillHoleProperty =
                        DependencyProperty . Register ( "FillHole",
                        typeof ( Brush ),
                        typeof ( ThreeDeeBtnControl ),
                        new PropertyMetadata ( new SolidColorBrush ( Colors . Black ) ), OnFillHoleChangedCallBack );

                private static bool OnFillHoleChangedCallBack ( object value )
                {
                        return true ;
                }

                #endregion FillHole

                #region FillTop
                public Brush FillTop
                {
                        get
                        {
                                return ( Brush ) GetValue ( FillTopProperty );
                        }
                        set
                        {
                                SetValue ( FillTopProperty, value );
                          }
                }

                public static readonly DependencyProperty FillTopProperty =
                        DependencyProperty . Register ( "FillTop",
                        typeof ( Brush ),
                        typeof ( ThreeDeeBtnControl ),
                        new PropertyMetadata ( new SolidColorBrush ( Colors . Lime ), OnFillTopChangedCallBack ) );

                private static void OnFillTopChangedCallBack ( DependencyObject d, DependencyPropertyChangedEventArgs e )
                {
               }

                #endregion FillTop

                #region FillShadow
                public Brush FillShadow
                {
                        get
                        {
                                return ( Brush ) GetValue ( FillShadowProperty );
                        }
                        set
                        {
                                SetValue ( FillShadowProperty, value );
                        }
                }

                public static readonly DependencyProperty FillShadowProperty =
                        DependencyProperty . RegisterAttached ( "FillShadow",
                        typeof ( Brush ),
                        typeof ( ThreeDeeBtnControl ),
                        new FrameworkPropertyMetadata ( new SolidColorBrush ( Colors . DarkSlateGray ), OnFillShadowChangedCallBack ) );

                private static void OnFillShadowChangedCallBack ( DependencyObject d, DependencyPropertyChangedEventArgs e )
                {
                        RectangleControl tc = d as RectangleControl;
                        //tc . FillHole . Opacity = (double) e . NewValue;
                }

                #endregion FillShadow

                #region PressedHoleHeight
                public int PressedHoleHeight
                {
                        get
                        {
                                return ( int ) GetValue ( PressedHoleHeightProperty );
                        }
                         set
                        {
                                SetValue ( PressedHoleHeightProperty, value );
                        }
                }

                public static readonly DependencyProperty PressedHoleHeightProperty =
                        DependencyProperty . Register ( "PressedHoleHeight",
                        typeof ( int ),
                        typeof ( ThreeDeeBtnControl ),
                        new FrameworkPropertyMetadata ( 150, new PropertyChangedCallback ( OnPressedHoleHeightPropertyChangedCallBack ) ) );

                private static void OnPressedHoleHeightPropertyChangedCallBack ( DependencyObject d, DependencyPropertyChangedEventArgs e )
                {
                        ThreeDeeBtnControl td = d as ThreeDeeBtnControl;
                }

                public static int PressedHeight
                {
                        get; set;
                }

                #endregion PressedHoleHeight

                #region RotateAngle
                public double RotateAngle
                {
                        get
                        {
                                return ( double ) GetValue ( RotateAngleProperty );
                        }
                        set
                        {
                                SetValue ( RotateAngleProperty, value );
                        }
                }

                public static readonly DependencyProperty RotateAngleProperty =
                        DependencyProperty . Register ( "RotateAngle",
                        typeof ( double ),
                        typeof ( ThreeDeeBtnControl ),
                        new PropertyMetadata ( ( double ) 0 ), OnRotateAngleChanged );

                private static bool OnRotateAngleChanged ( object value )
                {
                        return true;
                }

                #endregion RotateAngle

                #region SkewX
                public double SkewX
                {
                        get
                        {
                                return ( double ) GetValue ( SkewXProperty );
                        }
                        set
                        {
                                SetValue ( SkewXProperty, value );
                        }
                }

                // Using a DependencyProperty as the backing store for SkewX.  This enables animation, styling, binding, etc...
                public static readonly DependencyProperty SkewXProperty =
                    DependencyProperty . Register ( "SkewX", typeof ( double ), typeof ( ThreeDeeBtnControl ), new PropertyMetadata ( 0.0 ) );

                #endregion

                #region SkewY
                public double SkewY
                {
                        get
                        {
                                return ( double ) GetValue ( SkewYProperty );
                        }
                        set
                        {
                                SetValue ( SkewYProperty, value );
                        }
                }

                // Using a DependencyProperty as the backing store for SkewY.  This enables animation, styling, binding, etc...
                public static readonly DependencyProperty SkewYProperty =
                    DependencyProperty . Register ( "SkewY", typeof ( double ), typeof ( ThreeDeeBtnControl ), new PropertyMetadata ( 0.0 ) );
                #endregion

                #region TextHeight
                 public int TextHeight
                {
                        get
                        {
                                return ( int ) GetValue ( TextHeightProperty );
                        }
                         set
                        {
                                SetValue ( TextHeightProperty, value );
                        }
                }

                public static readonly DependencyProperty TextHeightProperty =
                        DependencyProperty . Register ( "TextHeight",
                        typeof ( int ),
                        typeof ( ThreeDeeBtnControl ),
                        new PropertyMetadata ( 18 ), OnTextHeightChanged );

                private static bool OnTextHeightChanged ( object value )
                {
                        //			Console . WriteLine ( $"TextHeight DP = {value}" );
                        //ThreeDeeBtnControl . Refresh();
                        return true;
                }

                #endregion TextHeight
  
                #region TextWidthScale
                 public double TextWidthScale
                {
                        get
                        {
                                return ( double ) GetValue ( TextWidthScaleProperty );
                        }
                       set
                        {
                                SetValue ( TextWidthScaleProperty, value );
                         }
                }

                public static readonly DependencyProperty TextWidthScaleProperty =
                        DependencyProperty . Register ( "TextWidthScale",
                        typeof ( double ),
                        typeof ( ThreeDeeBtnControl ),
                        new PropertyMetadata ( ( double ) 1 ), OnTextWidthScalePropertyChanged );

                private static bool OnTextWidthScalePropertyChanged ( object value )
                {
                         return true;
                }

                #endregion TextWidthScale

                #region TextHeightScale
               public double TextHeightScale
                {
                        get
                        {
                                return ( double ) GetValue ( TextHeightScaleProperty );
                        }
                        set
                        {
                                SetValue ( TextHeightScaleProperty, value );
                        }
                }

                public static readonly DependencyProperty TextHeightScaleProperty =
                        DependencyProperty . Register ( "TextHeightScale",
                        typeof ( double ),
                        typeof ( ThreeDeeBtnControl ),
                        new PropertyMetadata ( ( double ) 1 ), OnTextHeightScalePropertyPropertyChanged );

                private static bool OnTextHeightScalePropertyPropertyChanged ( object value )
                {
                        //			Console . WriteLine ( $"TextHeightScaleProperty = {value}" );

                        return true;
                }

                #endregion TextHeightScale

                #region TextWidth
            public int TextWidth
                {
                        get
                        {
                                return ( int ) GetValue ( TextWidthProperty );
                        }
                        set
                        {
                                SetValue ( TextWidthProperty, value );
                                this . Refresh ( );
                        }
                 }

                public static readonly DependencyProperty TextWidthProperty =
                        DependencyProperty . Register ( "TextWidth",
                        typeof ( int ),
                        typeof ( ThreeDeeBtnControl ),
                        new PropertyMetadata ( default ), OnTextWidthChanged );

                private static bool OnTextWidthChanged ( object value )
                {
                         return true;
                }

                #endregion TextWidth

                #region BtnText
             public string BtnText
                {
                        get
                        {
                                return ( string ) GetValue ( BtnTextProperty );
                        }
                        set
                        {
                                SetValue ( BtnTextProperty, value );
                                this . Refresh ( );
                        }
                }

                public static readonly DependencyProperty BtnTextProperty =
                        DependencyProperty . Register ( "BtnText",
                        typeof ( string ),
                        typeof ( ThreeDeeBtnControl ),
                        new FrameworkPropertyMetadata ( "BtnText", new PropertyChangedCallback ( OnBtnTextChangedCallBack ) ) );

                private static void OnBtnTextChangedCallBack ( DependencyObject sender, DependencyPropertyChangedEventArgs e )
                {
                 }

                private DependencyPropertyDescriptor BtnTextLength = DependencyPropertyDescriptor .
                            FromProperty ( ThreeDeeBtnControl . BtnTextProperty, typeof ( ThreeDeeBtnControl ) );

                #endregion BtnText

                #region FontType (Unused)

                //public FontStyle FontType
                //{
                //        get
                //        {
                //                return ( FontStyle ) GetValue ( FontTypeProperty );
                //        }
                //        set
                //        {
                //                SetValue ( FontTypeProperty, value );
                //        }
                //        //set{}
                //}
                //public static readonly DependencyProperty FontTypeProperty =
                //        DependencyProperty . Register ( "FontType",
                //        typeof ( FontStyle ),
                //        typeof ( ThreeDeeBtnControl ),
                //        new FrameworkPropertyMetadata ( default, new PropertyChangedCallback ( OnFontTypeChangedCallBack ) ) );
                //private static void OnFontTypeChangedCallBack ( DependencyObject sender, DependencyPropertyChangedEventArgs e )
                //{
                //        Console . WriteLine ( $"Fontype DP changed to [{ e . NewValue}]" );

                //        // Save width of our text to DP TextWidth
                //        ThreeDeeBtnControl tc = sender as ThreeDeeBtnControl;
                //        FontStyle s = ( FontStyle ) e . NewValue;
                //        tc . FontStyle = s;
                //        //			Console . WriteLine ( $"BtnText changed - Width changed to [{ s . Length}]" );
                //}

                #endregion FontType (Unused)

                #region BtnTextDown
                public string BtnTextDown
                {
                        get
                        {
                                return ( string ) GetValue ( BtnTextDownProperty );
                        }
                       set
                        {
                                SetValue ( BtnTextColorProperty, value );
                                this . Refresh ( );
                        }
                 }

                public static readonly DependencyProperty BtnTextDownProperty =
                        DependencyProperty . Register ( "BtnTextDown",
                        typeof ( string ),
                        typeof ( ThreeDeeBtnControl ),
                        new PropertyMetadata ( "" ) );

                #endregion BtnTextDown

                #region BtnBorder
               public Brush BtnBorder
                {
                        get
                        {
                                return ( Brush ) GetValue ( BtnBorderProperty );
                        }
                       set
                        {
                                SetValue ( BtnTextColorProperty, value );
                        }
                }

                public static readonly DependencyProperty BtnBorderProperty =
                        DependencyProperty . Register ( "BtnBorder",
                        typeof ( Brush ),
                        typeof ( ThreeDeeBtnControl ),
                        new PropertyMetadata ( new SolidColorBrush ( Colors . Transparent ) ) );

                #endregion BtnBorder

                #region BorderWidth
               public double BorderWidth
                {
                        get
                        {
                                return ( double ) GetValue ( BorderWidthProperty );
                        }
                      set
                        {
                                SetValue ( BorderWidthProperty, value );
                         }
                }

                public static readonly DependencyProperty BorderWidthProperty =
                        DependencyProperty . Register ( "BorderWidth",
                        typeof ( double ),
                        typeof ( ThreeDeeBtnControl ),
                        new PropertyMetadata ( ( double ) 0 ), OnBorderWidthChanged );

                private static bool OnBorderWidthChanged ( object value )
                {
                        return true;
                }

                #endregion BorderWidth

                 #region BtnTextColorDown
                public Brush BtnTextColorDown
                {
                        get
                        {
                                return ( Brush ) GetValue ( BtnTextColorDownProperty );
                        }
                      set
                        {
                                SetValue ( BtnTextColorProperty, value );
                        }
                }

                public static readonly DependencyProperty BtnTextColorDownProperty =
                        DependencyProperty . Register ( "BtnTextColorDown",
                        typeof ( Brush ),
                        typeof ( ThreeDeeBtnControl ),
                        new PropertyMetadata ( new SolidColorBrush ( Colors . Black ) ) );

                #endregion BtnTextColor

                #region FontDecoration
             public string FontDecoration
                {
                        get
                        {
                                return ( string ) GetValue ( FontDecorationProperty );
                        }
                        set
                        {
                                SetValue ( FontDecorationProperty, value );
                                this . Refresh ( );
                        }
                }

                public static readonly DependencyProperty FontDecorationProperty =
                        DependencyProperty . Register ( "FontDecoration",
                        typeof ( string ),
                        typeof ( ThreeDeeBtnControl ),
                        new FrameworkPropertyMetadata ( "Normal", new PropertyChangedCallback ( OnFontDecorationChangedCallBack ) ) );

                private static void OnFontDecorationChangedCallBack ( DependencyObject sender, DependencyPropertyChangedEventArgs e )
                {
                }

                //#endregion

                //#region FillTop
                //public Brush FillTop
                //{
                //	get
                //	{
                //		return ( Brush ) GetValue ( FillTopProperty );
                //	}
                //	set
                //	{
                //		SetValue ( FillTopProperty, value );
                //		this . Refresh ( );
                //	}
                //}
                //public static readonly DependencyProperty FillTopProperty =
                //	DependencyProperty . Register ( "FillTop",
                //	typeof ( Brush ),
                //	typeof ( ThreeDeeBtnControl ),
                //	new PropertyMetadata ( new SolidColorBrush ( Colors . Transparent ) ), OnFillTopChangedCallBack );
                //private static bool OnFillTopChangedCallBack ( object value )
                //{
                //	//			Console . WriteLine ( $"FillTop Property changed to [{ value}]" );
                //	return value != null ? true : false;
                //}
                //#endregion

                //#region FillSide
                //public Brush FillSide
                //{
                //	get
                //	{
                //		return ( Brush ) GetValue ( FillSideProperty );
                //	}
                //	set
                //	{
                //		SetValue ( FillSideProperty, value );
                //		this . Refresh ( );
                //	}
                //	//set{}
                //}
                //public static readonly DependencyProperty FillSideProperty =
                //	DependencyProperty . Register ( "FillSide",
                //	typeof ( Brush ),
                //	typeof ( ThreeDeeBtnControl ),
                //	new PropertyMetadata ( new SolidColorBrush ( Colors . Transparent ) ), OnFillSideChangedCallBack );
                //private static bool OnFillSideChangedCallBack ( object value )
                //{
                //	//			Console . WriteLine ( $"FillSide Property changed to [{ value}]" );
                //	return value != null ? true : false;
                //}
                //#endregion

                //#region FillHole
                //public Brush FillHole
                //{
                //	get
                //	{
                //		return ( Brush ) GetValue ( FillHoleProperty );
                //	}
                //	set
                //	{
                //		SetValue ( FillHoleProperty, value );
                //		this . Refresh ( );
                //	}
                //}
                //public static readonly DependencyProperty FillHoleProperty =
                //	DependencyProperty . Register ( "FillHole",
                //	typeof ( Brush ),
                //	typeof ( ThreeDeeBtnControl ),
                //	new PropertyMetadata ( new SolidColorBrush ( Colors . Transparent ) ), OnFillHoleChangedCallBack );
                //private static bool OnFillHoleChangedCallBack ( object value )
                //{
                //	//			Console . WriteLine ( $"FillHoleProperty changed to [{ value}]" );
                //	return value != null ? true : false;
                //}
                //#endregion

                //#region FillShadow
                //public Brush FillShadow
                //{
                //	get
                //	{
                //		return ( Brush ) GetValue ( FillShadowProperty );
                //	}
                //	set
                //	{
                //		value . Opacity = 250;
                //		SetValue ( FillShadowProperty, value );
                //		this . Refresh ( );
                //	}
                //}
                //public static readonly DependencyProperty FillShadowProperty =
                //	DependencyProperty . RegisterAttached ( "FillShadow",
                //	typeof ( Brush ),
                //	typeof ( ThreeDeeBtnControl ),
                //	new FrameworkPropertyMetadata ( new SolidColorBrush ( Colors . Beige ), OnFillShadowChangedCallBack ) );

                //private static void OnFillShadowChangedCallBack ( DependencyObject d, DependencyPropertyChangedEventArgs e )
                //{
                //	ThreeDeeBtnControl tc = d as ThreeDeeBtnControl;
                //	//tc . FillHole . Opacity = (double) e . NewValue;
                //}
                //              #endregion

                #region ShowBorder

                /// <summary>
                /// Show or hide the side border that gives button a more 3D effect
                /// </summary>
                public Visibility ShowBorder
                {
                        get
                        {
                                return ( Visibility ) GetValue ( ShowBorderProperty );
                        }

                        set
                        {
                                SetValue ( FillHoleProperty, value );
                                this . Refresh ( );
                        }
                }

                public static readonly DependencyProperty ShowBorderProperty =
                        DependencyProperty . Register ( "ShowBorder",
                        typeof ( Visibility ),
                        typeof ( ThreeDeeBtnControl ),
                        new PropertyMetadata ( Visibility . Visible ) );

                #endregion ShowBorder

                //#############################

                #endregion FontDecoration

                //#############################

                #region PropertyChanged

                //#############################

                public event PropertyChangedEventHandler PropertyChanged;

                private void OnPropertyChanged ( string propertyName )
                {
                        PropertyChanged?.Invoke ( this, new PropertyChangedEventArgs ( propertyName ) );
                        OnApplyTemplate ( );
                        //	this . VerifyPropertyName ( propertyName );

                        if ( this . PropertyChanged != null )
                        {
                                //				var e = new PropertyChangedEventArgs ( propertyName );
                                //			this . PropertyChanged ( this , e );
                        }
                }

                #endregion PropertyChanged

                private void Button_Click ( object sender, RoutedEventArgs e )
                {
                        Ellipse elip = sender as Ellipse;
                        //Console . WriteLine ( $"Button Clicked [{ elip.ToString()}]" );
                }

                public override void OnApplyTemplate ( )
                {
                        base . OnApplyTemplate ( );

                        if ( Template != null )
                        {
                                //var v = Template . FindName ( "H1", H1 );
                                var h1 = this . GetTemplateChild ( "H1" );
                                var buttonside = this . GetTemplateChild ( "Buttonside" );
                                var h3 = this . GetTemplateChild ( "H3" );
                                //				Console . WriteLine ($"v = {v}");
                                //Template . FindName ( "ThreeDBtn" , H1);
                                //.var canvas = VisualTreeHelper . GetChild ( this . Template . FindName ( "H1", this ) as DependencyObject, 0 );
                        }
                }

                private void ReportStatus ( )
                {
                        //Console . WriteLine ( $"REPORT On Loading : \n"
                        //	+ $" BtnText		: {BtnText}\n"
                        //	+ $" BtnTextColor	: {BtnTextColor}\n"
                        //	+ $" FontSize		: {FontSize}\n"
                        //	+ $" FontDecoration	:	{FontDecoration}\n" +
                        //	   $" FillTop	: {FillTop . ToString ( )}\n" +
                        //	   $" FillSide	: {FillSide . ToString ( )}\n" +
                        //	   $" FillHole	: {FillHole?.ToString ( )}\n"
                        //	   );
                }

                /// <summary>
                /// Change color of the Text on mouseover
                /// </summary>
                /// <param name="sender"></param>
                /// <param name="e"></param>
                private void ThreeDBtn_MouseEnter ( object sender, MouseEventArgs e )
                {
                        ThreeDBtn . Foreground = BtnTextColorDown;
                        ThreeDBtn . Content = BtnTextDown != "" ? BtnTextDown : BtnText;

                        Button b = sender as Button;
                        string s = BtnText;
                        PressedHeight = ( int ) b . ActualHeight;
                        //			if ( PressedHeight < 200 )
                        //				PressedHeight -= 2;

                        //Console . WriteLine ( $"ControlHeight= {ControlHeight}" );
                        //Console . WriteLine ( $"PressedHeight= {PressedHeight}" );
                        //Console . WriteLine ( $"Button Text = \"{s}\", Height= {TextHeight}" );
                        //Console . WriteLine ( $"Button Text = \"{s}\", length = {s . Length}" );
                        //Console . WriteLine ( $"Button Width = {b . ActualWidth}, Height = {b . ActualHeight}\n" );
                }

                /// <summary>
                /// Change color of the Text after mouseover
                /// </summary>
                /// <param name="sender"></param>
                /// <param name="e"></param>
                private void ThreeDBtn_MouseLeave ( object sender, MouseEventArgs e )
                {
                        ThreeDBtn . Foreground = BtnTextColor;
                        ThreeDBtn . Content = BtnText;
                }

                private void ThreeDBtn_Loaded ( object sender, RoutedEventArgs e )
                {
                        Button b = sender as Button;
                        // Store our buttons size into DP's
                        ControlHeight = ( int ) b . ActualHeight;
                        ControlWidth = ( int ) b . ActualWidth;
                }

  
                private void Btn6Content_Loaded ( object sender, RoutedEventArgs e )
                {
                        this . DataContext = this;
                        lgb = new LinearGradientBrush ( );
                        BorderGreen = new SolidColorBrush ( Colors . Green );
                        BorderBlack = new SolidColorBrush ( Colors . Black );
                        GetData ( null );
                        //			CurrentTextHeight =55;
                        SetStartupDefaults ( );
                        {
                                ReportStatus ( );
                                DataContext = this;
                        }
                }

                /// <summary>
                /// iterate thru a object to find controls ?
                /// </summary>
                /// <param name="sender"></param>
                private void GetData ( object sender )
                {
                        var element = sender as FrameworkElement;
                        var cp = this . FindName ( "Btn6Content" );
                        var v = element as ThreeDeeBtnControl;
                }

                private void Frame_Loaded ( object sender, RoutedEventArgs e )
                {
                        var element = sender as Border;
                        ControlWidth = ( int ) element . Width;
                }

  
                #endregion Dependencies

                //$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$//
                // MY FIRST ATTACHED PROPERTY !!!
                //public int EllipseSize
                //{
                //	get
                //	{
                //		return ( int ) GetValue ( EllipseSizeProperty );
                //	}
                //	set
                //	{
                //		SetValue ( EllipseSizeProperty , value );
                //	}
                //}
                //public int EllipseHeight
                //{
                //	get; set;
                //}
                //public static readonly DependencyProperty EllipseHeightProperty =
                //			DependencyProperty . RegisterAttached ( "EllipseHeight",
                //			typeof ( int ),
                //			typeof ( ThreeDeeBtnControl ),
                //			new PropertyMetadata ( 125, EllipseHeightChanged ) );

                //private static void EllipseHeightChanged ( DependencyObject d, DependencyPropertyChangedEventArgs e )
                //{
                //	ThreeDeeBtnControl el = d as ThreeDeeBtnControl;
                //	int size = ( int ) e . NewValue;
                //	el . Height = size;

                //}
                //public int EllipseWidth
                //{
                //	get; set;
                //}
                //public static readonly DependencyProperty EllipseWidthProperty =
                //			DependencyProperty . RegisterAttached ( "EllipseWidth",
                //			typeof ( int ),
                //			typeof ( ThreeDeeBtnControl ),
                //			new PropertyMetadata ( 125, EllipseHeightChanged ) );

                //private static void EllipseHeightChanged ( DependencyObject d, DependencyPropertyChangedEventArgs e )
                //{
                //	ThreeDeeBtnControl el = d as ThreeDeeBtnControl;
                //	int size = ( int ) e . NewValue;
                //	el . Height = size;

                //}

                //public static bool EllipseSizeChanged ( UIElement element, Boolean value )
                //{
                //	element . SetValue ( EllipseSizeProperty, value );
                //	Console . WriteLine ( $"EllipseSize  = {value . ToString ( )}" );
                //	return true;
                //}
                //public static Boolean GetTestProperty ( UIElement element )
                //{
                //	return ( Boolean ) element . GetValue ( EllipseSizeProperty );
                //}
                //$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$//
        }
}
