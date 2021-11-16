using System . ComponentModel;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Input;
using System . Windows . Media;
using System . Windows . Media . Effects;
using System . Windows . Shapes;

using WPFPages . Views;

namespace WPFPages . UserControls
{
        /// <summary>
        /// Interaction logic for RectangleControl.xaml
        /// </summary>
        public partial class RectangleControl : UserControl
        {
                public RectangleControl ( )
                {
                        this . DataContext = this;
                        InitializeComponent ( );
                 }
   
                public LinearGradientBrush lgb
                {
                        get; set;
                }
                public bool Loading = true;
                private LinearGradientBrush brush1;
                public Rectangle Rect;
                public Point RectSize;
                public static Ellipse H2;
                public static Ellipse H3;
                public Rectangle BtnRectangle
                {
                        get; set;
                }
                public Thickness RectThickness
                {
                        get; set;
                }
                public int GradientStyle
                {
                        get
                        {
                                return 0;
                        }

                        set
                        {
                                GradientStyle = value;
                                //				InvalidateVisual ( );
                        }
                }
                public bool Startup = false;

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

                //$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$//
                #region Dependencies

                //#############################


                #region Background
                new public Brush Background
                {
                        get
                        {
                                return ( Brush ) GetValue ( BackgroundProperty );
                        }
                        set
                        {
                                SetValue ( BackgroundProperty, value );
                                this . UpdateLayout ( );
                        }
                }
                // Using a DependencyProperty as the backing store for Background.  This enables animation, styling, binding, etc...
                new public static readonly DependencyProperty BackgroundProperty =
                    DependencyProperty . Register ( "Background", typeof ( Brush ), typeof ( RectangleControl ), new PropertyMetadata ( new SolidColorBrush (Color.FromArgb( 0xFD, 0x84, 0x77, 0x95) )));

                #endregion

                #region BorderBrush
                new public Brush BorderBrush
                {
                        get
                        {
                                return ( Brush ) GetValue ( BorderBrushProperty );
                        }
                        set
                        {
                                SetValue ( BorderBrushProperty, value );
                        }
                 }

                new public static readonly DependencyProperty BorderBrushProperty =
                        DependencyProperty . Register ( "BorderBrush",
                        typeof ( Brush ),
                        typeof ( RectangleControl ),
                        new PropertyMetadata ( new SolidColorBrush ( Colors . Transparent ) ) );

                #endregion BtnBorder

                #region BorderThickness
                new public double BorderThickness
                {
                        get
                        {
                                return ( double ) GetValue ( BorderThicknessProperty );
                        }
                        set
                        {
                                SetValue ( BorderThicknessProperty, value );
                         }
                 }

                new public static readonly DependencyProperty BorderThicknessProperty =
                        DependencyProperty . Register ( "BorderThickness",
                        typeof ( double ),
                        typeof ( RectangleControl ),
                        new PropertyMetadata ( ( double ) 0 ) );

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
                                SetValue ( BtnTextColorDownProperty, value );
                        }
                }

                public static readonly DependencyProperty BtnTextColorDownProperty =
                        DependencyProperty . Register ( "BtnTextColorDown",
                        typeof ( Brush ),
                        typeof ( RectangleControl ),
                        new PropertyMetadata ( new SolidColorBrush ( Colors . Black ) ) );

                #endregion BtnTextColorDown

                #region BtnDownThrow
                public int BtnDownThrow
                {
                        get
                        {
                                return ( int ) GetValue ( BtnDownThrowProperty );
                        }
                        set
                        {
                                 SetValue ( BtnDownThrowProperty, value );
                          }
                }

                public static readonly DependencyProperty BtnDownThrowProperty =
                        DependencyProperty . Register ( "BtnDownThrow",
                        typeof ( int),
                        typeof ( RectangleControl ),
                        new PropertyMetadata ( 1 ), OnBtnDownThrowPropertyChanged );

                private static bool OnBtnDownThrowPropertyChanged ( object value )
                {
                        return true;
                }

                #endregion TextHeight

                #region BtnTextDown
                public string BtnTextDown
                {
                        get
                        {
                                return ( string ) GetValue ( BtnTextDownProperty );
                        }
                       set
                        {
                                SetValue ( BtnTextDownProperty, value );
                        }
                 }

                public static readonly DependencyProperty BtnTextDownProperty =
                        DependencyProperty . Register ( "BtnTextDown",
                        typeof ( string ),
                        typeof ( RectangleControl ),
                        new PropertyMetadata ( "" ) );

                #endregion BtnTextDown   
  
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
                        typeof ( RectangleControl ),
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
                        typeof ( RectangleControl ),
                        new PropertyMetadata ( ( int ) 100 ), OnControlWidthChanged );

                private static bool OnControlWidthChanged ( object value )
                {
                        return true;
                }

                #endregion ControlWidth

                #region CornerRadius    
                public double CornerRadius
                {
                        get
                        {
                                return ( double ) GetValue ( CornerRadiusProperty );
                        }
                        set
                        {
                                SetValue ( CornerRadiusProperty, value );
                        }
                }

                // Using a DependencyProperty as the backing store for CornerRadius.  This enables animation, styling, binding, etc...
                public static readonly DependencyProperty CornerRadiusProperty =
                    DependencyProperty . Register ( "CornerRadius", typeof ( double ), typeof ( RectangleControl ), new PropertyMetadata ( (double)0 ) );

                #endregion

                //#region FillTop
                //public Brush FillTop
                //{
                //        get
                //        {
                //                 return ( Brush ) GetValue ( FillTopProperty );
                //        }
                //        set
                //        {
                //                SetValue ( FillTopProperty, value );
                //        }
                //}

                //public static readonly DependencyProperty FillTopProperty =
                //        DependencyProperty . Register ( "FillTop",
                //        typeof ( Brush ),
                //        typeof ( RectangleControl ),
                //        new PropertyMetadata ( new SolidColorBrush ( Colors . Gray ), OnFillTopChangedCallBack ) );

                //private static void OnFillTopChangedCallBack ( DependencyObject d, DependencyPropertyChangedEventArgs e )
                //{
                // }

                //#endregion FillTop

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
              }

                public static readonly DependencyProperty FillSideProperty =
                        DependencyProperty . Register ( "FillSide",
                        typeof ( Brush ),
                        typeof ( RectangleControl ),
                        new PropertyMetadata ( new SolidColorBrush ( Colors . Transparent ) ), OnFillSideChangedCallBack );

                private static bool OnFillSideChangedCallBack ( object value )
                {
                        return value != null ? true : false;
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
                        typeof ( RectangleControl ),
                        new PropertyMetadata ( new SolidColorBrush ( Colors . Transparent ) ), OnFillHoleChangedCallBack );

                private static bool OnFillHoleChangedCallBack ( object value )
                {
                         return value != null ? true : false;
                }

                #endregion FillHole

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
                        typeof ( RectangleControl ),
                        new FrameworkPropertyMetadata ( new SolidColorBrush ( Colors . DarkSlateGray ), OnFillShadowChangedCallBack ) );

                private static void OnFillShadowChangedCallBack ( DependencyObject d, DependencyPropertyChangedEventArgs e )
                {
                 }

                #endregion FillShadow

                #region FontSize
                new public int FontSize
                {
                        get
                        {
                                return ( int ) GetValue ( FontSizeProperty );
                        }
                        set
                        {
                                SetValue ( FontSizeProperty, value );
                        }
                }

                new public static readonly DependencyProperty FontSizeProperty =
                        DependencyProperty . Register ( "FontSize",
                        typeof ( int ),
                        typeof ( RectangleControl ),
                        new PropertyMetadata ( 12 ), OnFontSizeChanged );

                private static bool OnFontSizeChanged ( object value )
                {
                        return true;
                }

                #endregion TextSize

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
                       }
                }

                public static readonly DependencyProperty FontDecorationProperty =
                        DependencyProperty . Register ( "FontDecoration",
                        typeof ( string ),
                        typeof ( RectangleControl ),
                        new FrameworkPropertyMetadata ( "Normal", new PropertyChangedCallback ( OnFontDecorationChangedCallBack ) ) );

                private static void OnFontDecorationChangedCallBack ( DependencyObject sender, DependencyPropertyChangedEventArgs e )
                {
                 }

                #endregion FontDecoration

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
                                ButtonText . Refresh ( );
                        }
                        //set{}
                }

                new public static readonly DependencyProperty ForegroundProperty =
                        DependencyProperty . Register ( "Foreground",
                        typeof ( Brush ),
                        typeof ( RectangleControl ),
                        new PropertyMetadata ( new SolidColorBrush ( Colors . White ) ) );

                #endregion BtnTextColor

                #region MouseoverColor
                public Brush MouseoverColor
                {
                        get
                        {
                                return ( Brush ) GetValue ( MouseoverColorProperty );
                        }
                        set
                        {
                                SetValue ( MouseoverColorProperty, value );
                        }
                }

                public static readonly DependencyProperty MouseoverColorProperty =
                        DependencyProperty . Register ( "MouseoverColor",
                        typeof ( Brush ),
                        typeof ( RectangleControl ),
                        new PropertyMetadata ( new SolidColorBrush ( Colors . LightGreen ) ), OnMouseoverColorPropertyChangedCallBack );

                private static bool OnMouseoverColorPropertyChangedCallBack ( object value )
                {
                         return value != null ? true : false;
                }

                #endregion MouseoverColor

                #region PressedBtnHeight
                public int PressedBtnHeight
                {
                        get
                        {
                                return ( int ) GetValue ( PressedBtnHeightProperty );
                        }
                        set
                        {
                                SetValue ( PressedBtnHeightProperty, value );
                         }
                }

                public static readonly DependencyProperty PressedBtnHeightProperty =
                        DependencyProperty . Register ( "PressedBtnHeight",
                        typeof ( int ),
                        typeof ( RectangleControl ),
                        new FrameworkPropertyMetadata ( 85 ) );
                #endregion PressedBtnHeight

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
                        typeof ( RectangleControl ),
                        new PropertyMetadata ( ( double ) 0 ), OnRotateAngleChanged );

                private static bool OnRotateAngleChanged ( object value )
                {
                           return true;
                }
                #endregion RotateAngle

                #region ShadowBlurColor
                public Color ShadowBlurColor
                {
                        get
                        {
                                return ( Color ) GetValue ( ShadowBlurColorProperty );
                        }
                        set
                        {
                                SetValue ( ShadowBlurColorProperty, value );
                       }
               }

                public static readonly DependencyProperty ShadowBlurColorProperty =
                        DependencyProperty . Register ( "ShadowBlurColor",
                        typeof ( Color ),
                        typeof ( RectangleControl ),
                        new PropertyMetadata ( Colors . LightSlateGray ) );

                #endregion ShadowBlurColor

                #region ShadowBlurRadius
                public double ShadowBlurRadius
                {
                        get
                        {
                                return ( double ) GetValue ( ShadowBlurRadiusProperty );
                        }
                       set
                        {
                                SetValue ( ShadowBlurRadiusProperty, value );
                       }
                }

                public static readonly DependencyProperty ShadowBlurRadiusProperty =
                        DependencyProperty . Register ( "ShadowBlurRadius",
                        typeof ( double ),
                        typeof ( RectangleControl ),
                        new PropertyMetadata ( ( double ) 10 ), OnShadowBlurRadiusProperty );

                private static bool OnShadowBlurRadiusProperty ( object value )
                {
                        return true;
                }

                #endregion ShadowBlurRadius

                #region ShadowBlurSize
                public double ShadowBlurSize
                {
                        get
                        {
                                return ( double ) GetValue ( ShadowBlurSizeProperty );
                        }
                        set
                        {
                                SetValue ( ShadowBlurSizeProperty, value );
                        }
                }

                public static readonly DependencyProperty ShadowBlurSizeProperty =
                        DependencyProperty . Register ( "ShadowBlurSize",
                        typeof ( double ),
                        typeof ( RectangleControl ),
                        new PropertyMetadata ( ( double ) 0 ), OnShadowBlurSizeProperty );

                private static bool OnShadowBlurSizeProperty ( object value )
                {
                          return true;
                }

                #endregion ShadowBlurSize

                #region ShadowDepth
                public double ShadowDepth
                {
                        get
                        {
                                return ( double ) GetValue ( ShadowDepthProperty );
                        }
                       set
                        {
                                SetValue ( ShadowDepthProperty, value );
                        }
                }

                public static readonly DependencyProperty ShadowDepthProperty =
                        DependencyProperty . Register ( "ShadowDepth",
                        typeof ( double ),
                        typeof ( RectangleControl ),
                        new PropertyMetadata ( ( double ) 5 ), OnShadowDepthPropertyChanged );

                private static bool OnShadowDepthPropertyChanged ( object value )
                {
                        return true;
                }

                #endregion ShadowDepth

                #region ShadowDirection
                public double ShadowDirection
                {
                        get
                        {
                                return ( double ) GetValue ( ShadowDirectionProperty );
                        }
                        set
                        {
                                SetValue ( ShadowDirectionProperty, value );
                        }
                }

                public static readonly DependencyProperty ShadowDirectionProperty =
                        DependencyProperty . Register ( "ShadowDirection",
                        typeof ( double ),
                        typeof ( RectangleControl ),
                        new PropertyMetadata ( ( double ) 33 ), OnShadowDirectionChanged );

                private static bool OnShadowDirectionChanged ( object value )
                {
                        return true;
                }

                #endregion ShadowDirection

                #region ShadowOpacity
                public double ShadowOpacity
                {
                        get
                        {
                                return ( double ) GetValue ( ShadowOpacityProperty );
                        }
                        set
                        {
                                SetValue ( ShadowOpacityProperty, value );
                       }
                }

                public static readonly DependencyProperty ShadowOpacityProperty =
                        DependencyProperty . Register ( "ShadowOpacity",
                        typeof ( double ),
                        typeof ( RectangleControl ),
                        new PropertyMetadata ( ( double ) 1 ), OnShadowOpacityProperty );

                private static bool OnShadowOpacityProperty ( object value )
                {
                        return true;
                }

                #endregion ShadowOpacity

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
                        }
                }

                public static readonly DependencyProperty ShowBorderProperty =
                        DependencyProperty . Register ( "ShowBorder",
                        typeof ( Visibility ),
                        typeof ( RectangleControl ),
                        new PropertyMetadata ( Visibility . Visible ) );

                #endregion ShowBorder
 
                #region Text
                public string Text
                {
                        get
                        {
                                return ( string ) GetValue ( TextProperty );
                        }
                        set
                        {
                                SetValue ( TextProperty, value );
                        }
                }

                public static readonly DependencyProperty TextProperty =
                        DependencyProperty . Register ( "Text",
                        typeof ( string ),
                        typeof ( RectangleControl ),
                        new FrameworkPropertyMetadata ( "Text", new PropertyChangedCallback ( OnTextChangedCallBack ) ) );

                private static void OnTextChangedCallBack ( DependencyObject sender, DependencyPropertyChangedEventArgs e )
                {
                }

                private DependencyPropertyDescriptor BtnTextLength = DependencyPropertyDescriptor .
                            FromProperty ( RectangleControl . TextProperty, typeof ( RectangleControl ) );

                #endregion BtnText

                #region TextShadowDirection
                public double TextShadowDirection
                {
                        get
                        {
                                return ( double ) GetValue ( TextShadowDirectionProperty );
                        }
                        set
                        {
                                SetValue ( TextShadowDirectionProperty, value );
                         }
                }

                public static readonly DependencyProperty TextShadowDirectionProperty =
                        DependencyProperty . Register ( "TextShadowDirection",
                        typeof ( double ),
                        typeof ( RectangleControl ),
                        new PropertyMetadata ( ( double ) 330 ), OnTextShadowDirectionPropertyProperty );

                private static bool OnTextShadowDirectionPropertyProperty ( object value )
                {
                        return true;
                }

                #endregion TextShadowDirection

                #region TextShadowColor
                public Color TextShadowColor
                {
                        get
                        {
                                return ( Color ) GetValue ( TextShadowColorProperty );
                        }
                        set
                        {
                                SetValue ( TextShadowColorProperty, value );
                                ButtonText . Refresh ( );
                        }
                }

                public static readonly DependencyProperty TextShadowColorProperty =
                        DependencyProperty . Register ( "TextShadowColor",
                        typeof ( Color ),
                        typeof ( RectangleControl ),
                        new PropertyMetadata ( Colors . White ) );

                #endregion TextShadowColor

                #region TextShadowOpacity
                public double TextShadowOpacity
                {
                        get
                        {
                                return ( double ) GetValue ( TextShadowOpacitProperty );
                        }
                        set
                        {
                                SetValue ( TextShadowOpacitProperty, value );
                        }
                }

                public static readonly DependencyProperty TextShadowOpacitProperty =
                        DependencyProperty . Register ( "TextShadowOpacity",
                        typeof ( double ),
                        typeof ( RectangleControl ),
                        new PropertyMetadata ( ( double ) 0.5 ), OnTextShadowOpacityProperty );

                private static bool OnTextShadowOpacityProperty ( object value )
                {
                        return true;
                }

                #endregion TextShadowOpacity

                #region TextShadowRadius
                public double TextShadowRadius
                {
                        get
                        {
                                return ( double ) GetValue ( TextShadowRadiusProperty );
                        }
                         set
                        {
                                SetValue ( TextShadowRadiusProperty, value );
                        }
                }

                public static readonly DependencyProperty TextShadowRadiusProperty =
                        DependencyProperty . Register ( "TextShadowRadius",
                        typeof ( double ),
                        typeof ( RectangleControl ),
                        new PropertyMetadata ( ( double ) 1 ), OnTextShadowRadiusProperty );

                private static bool OnTextShadowRadiusProperty ( object value )
                {
                        return true;
                }

                #endregion TextShadowRadius

                #region TextShadowSize
                public double TextShadowSize
                {
                        get
                        {
                                return ( double ) GetValue ( TextShadowSizeProperty );
                        }
                        set
                        {
                                SetValue ( TextShadowSizeProperty, value );
                        }
                }

                public static readonly DependencyProperty TextShadowSizeProperty =
                        DependencyProperty . Register ( "TextShadowSize",
                        typeof ( double ),
                        typeof ( RectangleControl ),
                        new PropertyMetadata ( ( double ) 2 ), OnTextShadowSizePropertyProperty );

                private static bool OnTextShadowSizePropertyProperty ( object value )
                {
                        return true;
                }

                #endregion TextShadowSize

                #region TextLeftOffset
               public double TextLeftOffset
                {
                        get
                        {
                                return ( double ) GetValue ( TextLeftOffsetProperty );
                        }
                      set
                        {
                                SetValue ( TextLeftOffsetProperty, value );
                        }
                }

                public static readonly DependencyProperty TextLeftOffsetProperty =
                        DependencyProperty . Register ( "TextLeftOffset",
                        typeof ( double ),
                        typeof ( RectangleControl ),
                        new PropertyMetadata ( ( double ) 1 ), OnTextLeftOffsetPropertyChanged );

                private static bool OnTextLeftOffsetPropertyChanged ( object value )
                {
                        return true;
                }

                #endregion TextWidthScale

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
                        typeof ( RectangleControl ),
                        new PropertyMetadata ( 5 ), OnTextHeightPropertyPropertyChanged );

                private static bool OnTextHeightPropertyPropertyChanged ( object value )
                {
                         return true;
                }

                #endregion TextHeight
 
                #region TextWidth
                public double TextWidth
                {
                        get
                        {
                                return ( double ) GetValue ( TextWidthProperty );
                        }
                        set
                        {
                                  SetValue ( TextWidthProperty, value );
                       }
                }

                public static readonly DependencyProperty TextWidthProperty =
                        DependencyProperty . Register ( "TextWidth",
                        typeof ( double ),
                        typeof ( RectangleControl ),
                        new PropertyMetadata ((double) 0 ), OnTextWidthChanged );

                private static bool OnTextWidthChanged ( object value )
                {
                        return true;
                }

                #endregion TextWidth

                //#############################

                #endregion Dependencies

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

                private void RectBtn_Loaded ( object sender, RoutedEventArgs e )
                {
                        this . DataContext = this;

                        // Set initial colors
                        lgb = new LinearGradientBrush ( );
                        Brush1 = ( LinearGradientBrush ) FindResource ( "Greenbackground" );
                        BorderBlack = new SolidColorBrush ( Colors . Black );
                         //Save size of button rectangle
                        this . RectThickness = this . RectBtn . Margin;
  
                        Thickness thickness = new Thickness ( );
                        thickness = this . ButtonText . Margin;
                        thickness . Top -= 10;
                        this . ButtonText . Margin = thickness;
                          this . RectBtn . Fill = Background;
                        if ( this . BtnTextDown == "" )
                                this . BtnTextDown = Text;
                }

                /// <summary>
                /// Change color of the Text on mouseover
                /// </summary>
                /// <param name="sender"></param>
                /// <param name="e"></param>
                private void RectBtn_MouseEnter ( object sender, MouseEventArgs e )
                {
                        // Move button & Text Right and Down
                        TranslateTransform translateTransform = new TranslateTransform ( );
                        translateTransform . X += 3;
                        translateTransform . Y -= BtnDownThrow; 
                        int i = BtnDownThrow;
                        this . RectBtn . RenderTransform = translateTransform;
                        this . ButtonText . RenderTransform = translateTransform;

                        //Move Shadow to match above
                       DropShadowEffect dropShadowEffect = new DropShadowEffect ( );
                        dropShadowEffect . BlurRadius =ShadowBlurRadius;
                        dropShadowEffect . Color = Color.FromArgb((byte)0xFF, ( byte ) 0x00, ( byte ) 0x00, ( byte ) 0x00 );//ShadowBlurColor;
                        dropShadowEffect . Direction = ShadowDirection- 140;//235;
                        dropShadowEffect . Opacity = 0.2; //ShadowOpacity;
                        dropShadowEffect.ShadowDepth = ShadowDepth;
                        dropShadowEffect . ShadowDepth = ShadowBlurSize;

                        this . RectBtn . Effect = dropShadowEffect;
                        this . RectBtn . Fill = MouseoverColor;
                        this . RectBtn . Refresh ( );
                        e . Handled = true;
                        return;
                }

                private void RectBtn_MouseLeave ( object sender, MouseEventArgs e )
                {
                        TranslateTransform translateTransform = new TranslateTransform ( );
                        this . RectBtn . Margin = RectThickness;
                        translateTransform . X -=3;
                        translateTransform . Y += BtnDownThrow;
                        this . ButtonText . RenderTransform = translateTransform;
                        this . RectBtn . RenderTransform = translateTransform;

                        DropShadowEffect dropShadowEffect = new DropShadowEffect ( );
                        dropShadowEffect . BlurRadius = ShadowBlurRadius;
                        dropShadowEffect . Color = ShadowBlurColor;
                        dropShadowEffect . Direction = 49;
                        dropShadowEffect . Opacity = ShadowOpacity;
                        dropShadowEffect . ShadowDepth = ShadowBlurSize;
                        this . RectBtn . Effect = dropShadowEffect;
                        this . RectBtn . Fill = Background;
                        this . RectBtn . Refresh ( );
                        e . Handled = true;
                        return;
                }

                public override void OnApplyTemplate ( )
                {
                        base . OnApplyTemplate ( );

                        if ( Template != null )
                        {
                                var v = this . GetTemplateChild ( "RectBtn" );
                        }
                        return;
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

                // How to allow a click event from Usercontrol  to get back to  "Click" in client app
                //public event RoutedEventHandler MouseLeftButtonDown;
                //void OnMouseLeftButtonDown ( object sender, RoutedEventArgs e )
                //{
                //	if ( this . MouseLeftButtonDown != null )
                //	{
                //		this . MouseLeftButtonDown ( this, e );
                //	}
                //}
                /// <summary>
                /// iterate thru a object to find controls ?
                /// </summary>
                /// <param name="sender"></param>
                private void GetData ( object sender )
                {
                        var element = sender as FrameworkElement;
                        var cp = this . FindName ( "Btn6Content" );
                        var v = element as RectangleControl;
                }

                private void RectangularButton_Loaded ( object sender, RoutedEventArgs e )
                {
                 }
        }
}
