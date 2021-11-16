using System;
using System . ComponentModel;
using System . Runtime . CompilerServices;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Input;
using System . Windows . Media;
using System . Windows . Shapes;

using WPFPages . Views;

namespace WPFPages . UserControls
{
        /// <summary>
        /// Interaction logic for ShadowLabelControl.xaml
        /// </summary>
        public partial class ShadowLabelControl : UserControl
        {
                public ShadowLabelControl ( )
                {
                        this . DataContext = this;
                        InitializeComponent ( );
                        DoSetup ( );
                }

                   public void DoSetup ( )
                {
                        this . Background = Brushes . DarkGray;
                        this . BtnTextColor = Brushes . Black;
                        this . TextTopOffset = 0;
                        this . BtnColorDown = Brushes . Aqua;
                        this . BtnText = "Text here...";
                        //this . BtnTextColor = Brushes.White;
                        BorderPadding = 0;
                        if ( !UseStandardBackground && LinearFill . Fill != ( LinearGradientBrush ) null )
                                border . Background = Background;
                        else
                                LinearFill . Fill = LinearBackground;

                        this . Refresh ( );
                }

                public LinearGradientBrush lgb
                {
                        get; set;
                }

                //		private GradientDisplay gw = null;
                public bool Loading = true;

                private LinearGradientBrush brush1;
                public Rectangle Rect;
                public Point RectSize;
                public static Ellipse H2;
                public static Ellipse H3;


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

 
                #region Dependencies

                //#############################

                #region Background

                new public Brush Background
                {
                        get
                        {
                                this . Refresh ( );
                                return ( Brush ) GetValue ( BackgroundProperty );
                        }

                        set
                        {
                                SetValue ( BackgroundProperty, value );
                         }
                }

                new public static readonly DependencyProperty BackgroundProperty =
                        DependencyProperty . Register ( "Background",
                        typeof ( Brush ),
                        typeof ( ShadowLabelControl ),
                        new PropertyMetadata ( new SolidColorBrush ( Colors . Gray ), OnBackgroundhangedCallBack ) );

                private static void OnBackgroundhangedCallBack ( DependencyObject d, DependencyPropertyChangedEventArgs e )
                {
                        Console . WriteLine ($"SHADOWLABELCONTROL : Background set to {e.NewValue}");
                }

                #endregion Background

                #region BorderPadding

                public double BorderPadding
                {
                        get
                        {
                                return ( double ) GetValue ( BorderPaddingProperty );
                        }

                        set
                        {
                                SetValue ( BorderPaddingProperty, value );
                        }
                }

                public static readonly DependencyProperty BorderPaddingProperty =
                        DependencyProperty . Register ( "BorderPadding",
                        typeof ( double ),
                        typeof ( ShadowLabelControl ),
                        new PropertyMetadata ( default ), OnBorderPaddingPropertyChanged );

                private static bool OnBorderPaddingPropertyChanged ( object value )
                {
                        return true;
                }

                #endregion BorderPadding

                #region BorderColor

                /// <summary>
                /// Color of the border around the top surface of the button
                /// </summary>
                public Brush BorderColor
                {
                        get
                        {
                                return ( Brush ) GetValue ( BorderColorProperty );
                        }

                        set
                        {
                                SetValue ( BorderColorProperty, value );
                                RectBtn . Refresh ( );
                        }
                        //set{}
                }

                public static readonly DependencyProperty BorderColorProperty =
                        DependencyProperty . Register ( "BorderColor",
                        typeof ( Brush ),
                        typeof ( ShadowLabelControl ),
                        new PropertyMetadata ( new SolidColorBrush ( Colors . Transparent ) ) );

                #endregion BorderColor

                #region BorderShadowColor
                public Color BorderShadowColor
                {
                        get
                        {
                                return ( Color ) GetValue ( BorderShadowColorProperty );
                        }
                        set
                        {
                                SetValue ( BorderShadowColorProperty, value );
                        }
                  }

                public static readonly DependencyProperty BorderShadowColorProperty =
                        DependencyProperty . Register ( "BorderShadowColor",
                        typeof ( Color ),
                        typeof ( ShadowLabelControl ),
                        new PropertyMetadata ( Colors . DarkSlateGray ) );

                #endregion BorderShadowColor

                #region BorderShadowBlurSize
                public double BorderShadowBlurSize
                {
                        get
                        {
                                return ( double ) GetValue ( BorderShadowBlurSizeProperty );
                        }

                        //set { }
                        set
                        {
                                SetValue ( BorderShadowBlurSizeProperty, value );
                        }
                }

                public static readonly DependencyProperty BorderShadowBlurSizeProperty =
                        DependencyProperty . Register ( "BorderShadowBlurSize",
                        typeof ( double ),
                        typeof ( ShadowLabelControl ),
                        new PropertyMetadata ( ( double ) 5 ), OnBorderShadowBlurSizeProperty );

                private static bool OnBorderShadowBlurSizeProperty ( object value )
                {
                         return true;
                }

                #endregion BorderShadowBlurSize

                #region BorderShadowDepth
                public double BorderShadowDepth
                {
                        get
                        {
                                return ( double ) GetValue ( BorderShadowDepthProperty );
                        }

                        //set { }
                        set
                        {
                                SetValue ( BorderShadowDepthProperty, value );
                          }
                }

                public static readonly DependencyProperty BorderShadowDepthProperty =
                        DependencyProperty . Register ( "BorderShadowDepth",
                        typeof ( double ),
                        typeof ( ShadowLabelControl ),
                        new PropertyMetadata ( ( double ) 2 ), OnBorderShadowDepthPropertyChanged );

                private static bool OnBorderShadowDepthPropertyChanged ( object value )
                {
                        return true;
                }

                #endregion BorderShadowDepth

                #region BorderShadowDirection
                public double BorderShadowDirection
                {
                        get
                        {
                                return ( double ) GetValue ( BorderShadowDirectionProperty );
                        }
                        set
                        {
                                SetValue ( BorderShadowDirectionProperty, value );
                                ButtonText . Refresh ( );
                        }
                }

                public static readonly DependencyProperty BorderShadowDirectionProperty =
                        DependencyProperty . Register ( "BorderShadowDirection",
                        typeof ( double ),
                        typeof ( ShadowLabelControl ),
                        new PropertyMetadata ( ( double ) 33 ), OnBorderShadowDirectionChanged );

                private static bool OnBorderShadowDirectionChanged ( object value )
                {
                        return true;
                }

                #endregion BorderShadowDirection

                #region BorderShadowOpacity

                public double BorderShadowOpacity
                {
                        get
                        {
                                return ( double ) GetValue ( BorderShadowOpacityProperty );
                        }

                          set
                        {
                                SetValue ( BorderShadowOpacityProperty, value );
                         }
                }

                public static readonly DependencyProperty BorderShadowOpacityProperty =
                        DependencyProperty . Register ( "BorderShadowOpacity",
                        typeof ( double ),
                        typeof ( ShadowLabelControl ),
                        new PropertyMetadata ( ( double ) 1.0 ), OnBorderShadowOpacityProperty );

                private static bool OnBorderShadowOpacityProperty ( object value )
                {
                         return true;
                }

                #endregion BorderShadowOpacity

                #region BtnColorDown

                /// <summary>
                /// Color of the border around the top surface of the button
                /// </summary>
                public Brush BtnColorDown
                {
                        get
                        {
                                return ( Brush ) GetValue ( BtnColorDownProperty );
                        }

                        set
                        {
                                SetValue ( BtnColorDownProperty, value );
                                RectBtn . Refresh ( );
                        }
                }

                public static readonly DependencyProperty BtnColorDownProperty =
                        DependencyProperty . Register ( "BtnColorDown",
                        typeof ( Brush ),
                        typeof ( ShadowLabelControl ),
                        new PropertyMetadata ( new SolidColorBrush ( Colors . Transparent ) ) );

                #endregion BtnColorDown

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
                         }
                 }

                public static readonly DependencyProperty BtnTextProperty =
                        DependencyProperty . Register ( "BtnText",
                        typeof ( string ),
                        typeof ( ShadowLabelControl ),
                        new FrameworkPropertyMetadata ( "", new PropertyChangedCallback ( OnBtnTextChangedCallBack ) ) );

                private static void OnBtnTextChangedCallBack ( DependencyObject sender, DependencyPropertyChangedEventArgs e )
                {
                }

                #endregion BtnText

                #region BorderThickness

                /// <summary>
                /// Width of the border line around the top of the button
                /// </summary>
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
                        typeof ( ShadowLabelControl ),
                        new PropertyMetadata ( ( double ) 0 ) );

                #endregion BorderThickness

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
                        //set{}
                }

                public static readonly DependencyProperty BtnTextDownProperty =
                        DependencyProperty . Register ( "BtnTextDown",
                        typeof ( string ),
                        typeof ( ShadowLabelControl ),
                        new PropertyMetadata ( "" ) );

                #endregion BtnTextDown

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
                        typeof ( ShadowLabelControl ),
                        new PropertyMetadata ( new SolidColorBrush ( Colors . White ) ) );

                #endregion BtnTextColor

                #region BtnTextColorDown

                /// <summary>
                /// Color of the button Text when it is depressed
                /// </summary>
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
                        typeof ( ShadowLabelControl ),
                        new PropertyMetadata ( new SolidColorBrush ( Colors . White ) ) );

                #endregion BtnTextColorDown

                #region CornerRadius
                public double CornerRadius
                {
                        get
                        {
                                return ( double ) GetValue ( CornerRadiusProperty );
                        }set
                        {
                                SetValue ( CornerRadiusProperty, value );
                        }
                }

                public static readonly DependencyProperty CornerRadiusProperty =
                        DependencyProperty . Register ( "CornerRadius",
                        typeof ( double ),
                        typeof ( ShadowLabelControl ),
                        new PropertyMetadata ( ( double ) 0 ), OnCornerradiusPropertyChanged );

                private static bool OnCornerradiusPropertyChanged ( object value )
                {
                        return true;
                }
                #endregion CornerRadius

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
                        typeof ( ShadowLabelControl ),
                        new FrameworkPropertyMetadata ( "Normal", new PropertyChangedCallback ( OnFontDecorationChangedCallBack ) ) );

                private static void OnFontDecorationChangedCallBack ( DependencyObject sender, DependencyPropertyChangedEventArgs e )
                {
                }

                #endregion FontDecoration

                #region LinearBackground
                public Brush LinearBackground
                {
                        get
                        {
                                this . Refresh ( );
                                return ( LinearGradientBrush ) GetValue ( LinearBackgroundProperty );
                        }
                        set
                        {
                                SetValue ( LinearBackgroundProperty, value );
                      }
                }

                public static readonly DependencyProperty LinearBackgroundProperty =
                        DependencyProperty . Register ( "LinearBackground",
                        typeof ( LinearGradientBrush ),
                        typeof ( ShadowLabelControl ),
                        new PropertyMetadata ( default, OnLinearBackgroundCallBack ) );

                private static void OnLinearBackgroundCallBack ( DependencyObject d, DependencyPropertyChangedEventArgs e )
                {
                 }

                #endregion LinearBackground

                #region LinearBackgroundDown
                public Brush LinearBackgroundDown
                {
                        get
                        {
                                this . Refresh ( );
                                return ( LinearGradientBrush ) GetValue ( LinearBackgroundDownProperty );
                        }
                        set
                        {
                                SetValue ( LinearBackgroundDownProperty, value );
                        }
                }

                public static readonly DependencyProperty LinearBackgroundDownProperty =
                        DependencyProperty . Register ( "LinearBackgroundDown",
                        typeof ( LinearGradientBrush ),
                        typeof ( ShadowLabelControl ),
                        new PropertyMetadata ( default, OnLinearBackgroundDownCallBack ) );

                private static void OnLinearBackgroundDownCallBack ( DependencyObject d, DependencyPropertyChangedEventArgs e )
                {
                }

                #endregion LinearBackgroundDown

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
                        typeof ( ShadowLabelControl ),
                        new PropertyMetadata ( new SolidColorBrush ( Colors . DarkGray ) ), OnMouseoverColorPropertyChangedCallBack );

                private static bool OnMouseoverColorPropertyChangedCallBack ( object value )
                {
                         return value != null ? true : false;
                }

                #endregion MouseoverColor

                #region RectCorner
                public double RectCorner
                {
                        get
                        {
                                return ( double ) GetValue ( RectCornerProperty );
                        }
                       set
                        {
                                SetValue ( RectCornerProperty, value );
                        }
                }

                // Using a DependencyProperty as the backing store for RectCorner.  This enables animation, styling, binding, etc...
                public static readonly DependencyProperty RectCornerProperty =
                    DependencyProperty . Register ( "RectCorner", typeof ( double ), typeof ( ShadowLabelControl ), new PropertyMetadata ( ( double ) 0 ) );

                #endregion RectCorner

                #region RotateAngle

                //public double RotateAngle
                //{
                //	get
                //	{
                //		return ( double ) GetValue ( RotateAngleProperty );
                //	}
                //	//set { }
                //	set
                //	{
                //		SetValue ( RotateAngleProperty, value );
                //		ButtonText . Refresh ( );
                //	}
                //}
                //public static readonly DependencyProperty RotateAngleProperty =
                //	DependencyProperty . Register ( "RotateAngle",
                //	typeof ( double ),
                //	typeof ( ShadowLabelControl ),
                //	new PropertyMetadata ( ( double ) 0 ), OnRotateAngleChanged );

                //private static bool OnRotateAngleChanged ( object value )
                //{
                //	//			Console . WriteLine ( $"RotateAngle  = {value}" );

                //	return true;
                //}

                #endregion RotateAngle

                #region ShadowDownColor 
                public Color ShadowDownColor
                {
                        get
                        {
                                return ( Color ) GetValue ( ShadowDownColorProperty );
                        }
                        set
                        {
                                SetValue ( ShadowDownColorProperty, value );
                        }
                        //set{}
                }
                public static readonly DependencyProperty ShadowDownColorProperty =
                        DependencyProperty . Register ( "ShadowDownColor",
                        typeof ( Color ),
                        typeof ( ShadowLabelControl ),
                        new PropertyMetadata ( Colors . White ) );

                #endregion ShadowDownColor  (NOT USED)

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
                    DependencyProperty . Register ( "SkewX", typeof ( double ), typeof ( ShadowLabelControl ), new PropertyMetadata ( 0.0 ) );

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
                    DependencyProperty . Register ( "SkewY", typeof ( double ), typeof ( ShadowLabelControl ), new PropertyMetadata ( 0.0 ) );
                #endregion

                #region ShowBorder (UNUSED)

                //              public Visibility ShowBorder
                //{
                //	get
                //	{
                //		return ( Visibility ) GetValue ( ShowBorderProperty );
                //	}
                //	set
                //	{
                //		SetValue ( ShowBorderProperty, value );
                //		ButtonText . Refresh ( );
                //	}
                //}
                //public static readonly DependencyProperty ShowBorderProperty =
                //	DependencyProperty . Register ( "ShowBorder",
                //	typeof ( Visibility ),
                //	typeof ( ShadowLabelControl ),
                //	new PropertyMetadata ( Visibility . Visible ) );

                #endregion ShowBorder (UNUSED)

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
                        typeof ( ShadowLabelControl ),
                        new FrameworkPropertyMetadata ( "Text Area", new PropertyChangedCallback ( OnTextChangedCallBack ) ) );

                private static void OnTextChangedCallBack ( DependencyObject sender, DependencyPropertyChangedEventArgs e )
                {
                 }

                #endregion Text

                #region TextShadowBlurSize
                public double TextShadowBlurSize
                {
                        get
                        {
                                return ( double ) GetValue ( TextShadowBlurSizeProperty );
                        }
                       set
                        {
                                SetValue ( TextShadowBlurSizeProperty, value );
                       }
                }

                public static readonly DependencyProperty TextShadowBlurSizeProperty =
                        DependencyProperty . Register ( "TextShadowBlurSize",
                        typeof ( double ),
                        typeof ( ShadowLabelControl ),
                        new PropertyMetadata ( ( double ) 0 ), OnTextShadowBlurSizeProperty );

                private static bool OnTextShadowBlurSizeProperty ( object value )
                {
                          return true;
                }

                #endregion TextShadowBlurSize

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
                        typeof ( ShadowLabelControl ),
                        new PropertyMetadata ( ( double ) 33 ), OnTextShadowDirectionPropertyProperty );

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
                        }
                }

                public static readonly DependencyProperty TextShadowColorProperty =
                        DependencyProperty . Register ( "TextShadowColor",
                        typeof ( Color ),
                        typeof ( ShadowLabelControl ),
                        new PropertyMetadata ( Colors .White ) );

                #endregion TextShadowColor

                #region TextShadowColorDown
                public Color TextShadowColorDown
                {
                        get
                        {
                                return ( Color ) GetValue ( TextShadowColorDownProperty );
                        }
                        set
                        {
                                SetValue ( TextShadowColorDownProperty, value );
                        }
                }

                public static readonly DependencyProperty TextShadowColorDownProperty =
                        DependencyProperty . Register ( "TextShadowColorDown",
                        typeof ( Color ),
                        typeof ( ShadowLabelControl ),
                        new PropertyMetadata ( Colors . Black ) );
               
                #endregion

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
                        typeof ( ShadowLabelControl ),
                        new PropertyMetadata ( ( double ) 1 ), OnTextShadowOpacityProperty );

                private static bool OnTextShadowOpacityProperty ( object value )
                {
                        return true;
                }

                #endregion TextShadowOpacity
 
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
                        typeof ( ShadowLabelControl ),
                        new PropertyMetadata ( ( double ) 1 ), OnTextShadowSizePropertyProperty );

                private static bool OnTextShadowSizePropertyProperty ( object value )
                {
                         return true;
                }

                #endregion TextShadowSize

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
                        typeof ( ShadowLabelControl ),
                        new PropertyMetadata ( ( double ) 1 ), OnTextHeightScalePropertyChanged );

                private static bool OnTextHeightScalePropertyChanged ( object value )
                {
                        return true;
                }

                #endregion TextWidthScale

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
                        typeof ( ShadowLabelControl ),
                        new PropertyMetadata ( ( double ) 1 ), OnTextWidthScalePropertyChanged );

                private static bool OnTextWidthScalePropertyChanged ( object value )
                {
                       return true;
                }

                #endregion TextWidthScale

                #region TextTopOffset
                public int TextTopOffset
                {
                        get
                        {
                                return ( int ) GetValue ( TextTopOffsetProperty );
                        }
                        set
                        {
                                SetValue ( TextTopOffsetProperty, value );
                         }
                }

                public static readonly DependencyProperty TextTopOffsetProperty =
                        DependencyProperty . Register ( "TextTopOffset",
                        typeof ( int ),
                        typeof ( ShadowLabelControl ),
                        new PropertyMetadata (0 ), OnTextTopOffsetPropertyChanged );

                private static bool OnTextTopOffsetPropertyChanged ( object value )
                {
                    return true;
                }

                #endregion TextTopOffset

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
                        typeof ( ShadowLabelControl ),
                        new PropertyMetadata ( ( double ) 0 ), OnTextLeftOffsetPropertyPropertyChanged );

                private static bool OnTextLeftOffsetPropertyPropertyChanged ( object value )
                {
                      return true;
                }

                #endregion TextLeftOffset

                #region TextSize
                public int TextSize
                {
                        get
                        {
                                return ( int ) GetValue ( TextSizeProperty );
                        }

                        set
                        {
                                SetValue ( TextSizeProperty, value );
                        }
                }

                public static readonly DependencyProperty TextSizeProperty =
                        DependencyProperty . Register ( "TextSize",
                        typeof ( int ),
                        typeof ( ShadowLabelControl ),
                        new PropertyMetadata ( 18 ), OnTextSizeChanged );

                private static bool OnTextSizeChanged ( object value )
                {
                        return true;
                }

                #endregion TextSize

                #region TextTop
                public double TextTop
                {
                        get
                        {
                                return ( double ) GetValue ( TextTopProperty );
                        }
                        set
                        {
                                 SetValue ( TextTopProperty, value );
                        }
                }

                public static readonly DependencyProperty TextTopProperty =
                        DependencyProperty . Register ( "TextTop",
                        typeof ( double ),
                        typeof ( ShadowLabelControl ),
                        new PropertyMetadata ( ( double ) 0 ), OnTextTopChanged );

                private static bool OnTextTopChanged ( object value )
                {
                        return true;
                }

                #endregion TextWidth

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
                        typeof ( ShadowLabelControl ),
                        new PropertyMetadata ( ( double ) 0 ), OnTextWidthChanged );

                private static bool OnTextWidthChanged ( object value )
                {
                        return true;
                }

                #endregion TextWidth

                //#region TextWrap
                //public TextWrapping TextWrap
                //{
                //        get
                //        {
                //                return ( TextWrapping ) GetValue ( TextWrapProperty );
                //        }
                //        set
                //        {
                //                SetValue ( TextWrapProperty, value );
                //        }
                //}

                //// Using a DependencyProperty as the backing store for TextWrapping.  This enables animation, styling, binding, etc...
                //public static readonly DependencyProperty TextWrapProperty =
                //    DependencyProperty . Register ( "TextWrap", typeof ( TextWrapping ), typeof ( ShadowLabelControl ), new PropertyMetadata ( TextWrapping.NoWrap ) );

               #region UseStandardBackground

                public bool UseStandardBackground
                {
                        get
                        {
                                return ( bool ) GetValue ( UseStandardBackgroundProperty );
                        }

                        set
                        {
                                SetValue ( UseStandardBackgroundProperty, value );
                      }
                }

                // Using a DependencyProperty as the backing store for UseStandardBackground.  This enables animation, styling, binding, etc...
                public static readonly DependencyProperty UseStandardBackgroundProperty =
                    DependencyProperty . Register ( "UseStandardBackground", typeof ( bool ), typeof ( ShadowLabelControl ),
                            new PropertyMetadata ( false ), OnswitchChanged );

                private static bool OnswitchChanged ( object value )
                {
                        return true;
                }

                #endregion UseStandardBackground

                 #region TextShadowColor

                //public Color ShadowColor
                //{
                //	get
                //	{
                //		return ( Color ) GetValue ( ShadowColorProperty );
                //	}
                //	set
                //	{
                //		SetValue ( ShadowColorProperty, value );
                //		ButtonText . Refresh ( );
                //	}
                //	//set{}
                //}
                //public static readonly DependencyProperty ShadowColorProperty =
                //	DependencyProperty . Register ( "ShadowColor",
                //	typeof ( Color ),
                //	typeof ( ShadowLabelControl ),
                //	new PropertyMetadata ( Colors . DarkGray ) );

                #endregion TextShadowColor

                #region TextShadowDepth (UNUSED)

                //              public double TextShadowDepth
                //              {
                //	get
                //	{
                //		return ( double ) GetValue ( TextShadowDepthProperty );
                //	}
                //	//set { }
                //	set
                //	{
                //		SetValue ( TextShadowDepthProperty, value );
                //		ButtonText . Refresh ( );
                //	}

                //}
                //public static readonly DependencyProperty TextShadowDepthProperty =
                //	DependencyProperty . Register ( "TextShadowDepth",
                //	typeof ( double ),
                //	typeof ( ShadowLabelControl ),
                //	new PropertyMetadata ( ( double ) 2 ), OnTextShadowDepthPropertyChanged );

                //private static bool OnTextShadowDepthPropertyChanged ( object value )
                //{
                //	//			Console . WriteLine ( $"TextSize DP = {value}" );
                //	//RectangleControl . Refresh();
                //	return true;
                //}

                #endregion TextShadowDepth (UNUSED)

                   #region ShadowBlur (unused)

                //public double ShadowBlur
                //{
                //	get
                //	{
                //		return ( double ) GetValue ( ShadowBlurProperty );
                //	}
                //	//set { }
                //	set
                //	{
                //		SetValue ( ShadowBlurProperty, value );
                //		ButtonText . Refresh ( );
                //	}
                //}
                //public static readonly DependencyProperty ShadowBlurProperty =
                //	DependencyProperty . Register ( "ShadowBlur",
                //	typeof ( double ),
                //	typeof ( ShadowLabelControl ),
                //	new PropertyMetadata ( ( double ) 0 ), OnShadowBlurPropertyProperty );

                //private static bool OnShadowBlurPropertyProperty ( object value )
                //{
                //	return true;
                //}

                #endregion ShadowBlur (unused)

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
                        //			object child = null;
                        this . DataContext = this;

                        // Set initial colors
                        lgb = new LinearGradientBrush ( );
                        Brush1 = ( LinearGradientBrush ) FindResource ( "Greenbackground" );
                        BorderBlack = new SolidColorBrush ( Colors . Black );
                        if ( this . UseStandardBackground )
                        {
                                this . LinearFill . Visibility = Visibility . Hidden;
                                this . LinearFill . Fill = null;
                        }
                        else
                        {
                                this . LinearFill . Fill = LinearBackground;
                                this . LinearFill . Visibility = Visibility . Visible;
                        }
                        this . LinearFill . Refresh ( );
                        this . RectBtn . Refresh ( );
                        if ( this . BtnTextDown == "" )
                                this . BtnTextDown = this . BtnText;
                        //	Convert CorenrRadius to doube for use with RadiusX & Y in Rectangle
                        this . RectCorner = ( Convert . ToDouble ( this . CornerRadius ) );
                        this . Refresh ( );
                }

                /// <summary>
                /// Change color of the Text on mouseover
                /// </summary>
                /// <param name="sender"></param>
                /// <param name="e"></param>
                private void RectBtn_MouseEnter ( object sender, MouseEventArgs e )
                {
                        this . dropshadow . Color = this . TextShadowColorDown;
                        this . ButtonText . Foreground = this . BtnTextColorDown;
                        if ( !this . UseStandardBackground && this . LinearFill . Fill != ( LinearGradientBrush ) null )
                        {
                                this . LinearFill . Fill = this . LinearBackgroundDown;
                                this . LinearFill . Refresh ( );
                        }
                        else
                        {
                                border . Background = this . MouseoverColor;
                                this . border . Refresh ( );
                        }
                        return;
                }

                private void RectBtn_MouseLeave ( object sender, MouseEventArgs e )
                {
                        this . border . Background = this . Background;
                        this . dropshadow . Color = this . BorderShadowColor;
                        this . ButtonText . Foreground = this . BtnTextColor;
                        if ( !this . UseStandardBackground && this . LinearFill . Fill != ( LinearGradientBrush ) null )
                                this . LinearFill . Fill = this . LinearBackground;
                        else
                                this . border . Background = this . Background;

                        this . border . Refresh ( );
                        return;
                }

                public override void OnApplyTemplate ( )
                {
                        base . OnApplyTemplate ( );

                        if ( Template != null )
                        {
                                var v = this . GetTemplateChild ( "LinearFill" );
                                //				Console . WriteLine ( $"v = {v}" );
                        }
                        return;
                }

                   private void ContainerGrid_SizeChanged ( object sender, SizeChangedEventArgs e )
                {
                        Grid grid = sender as Grid;

                        this . ButtonText . Height = grid . ActualHeight;
                        this . ButtonText . Width = grid . ActualWidth;
                        Thickness t = grid . Margin;
                        t . Top = 0;
                        t . Left = 0;
                        t . Right = 0;
                        t . Bottom = 0;
                        this . ButtonText . Margin = t;
                        Thickness tr = this . RectBtn . Margin;
                }

                private void ShadowTextControl_Loaded ( object sender, RoutedEventArgs e )
                {
                }

                private void border_loaded ( object sender, RoutedEventArgs e )
                {
                        //if ( this . LinearFill . Fill == ( LinearGradientBrush ) null )
                        //        this . LinearFill . Visibility = Visibility . Hidden;
                        //else
                        //        this . border . Background = Brushes . Transparent;
                }

                private void border_PreviewMouseRightButtonDown ( object sender, MouseButtonEventArgs e )
                {
                        try
                        {
                                this . TextWidth += 5;
                                this . TextTop += 5;
                        }
                        catch (Exception ex)
                        {
                                Console . WriteLine ($"Error= {ex.Message}, {ex.Data}, {ex.GetType()}, {ex.Source}");
                        }
                }

                private void border_PreviewMouseRightButtonUp ( object sender, MouseButtonEventArgs e )
                {
                        this.TextWidth-= 5;
                        this . TextTop -= 5;
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
                //private void GetData ( object sender )
                //{
                //	var element = sender as FrameworkElement;
                //	var cp = this . FindName ( "Btn6Content" );
                //	var v = element as RectangleControl;
                //}

                //private void RectangularButton_Loaded ( object sender, RoutedEventArgs e )
                //{
                //	RectangleControl rb = sender as RectangleControl;

                //	//You can use this format almost anywhere to change a Dependency Poperty
                //	//			SetValue ( BtnTextProperty, "here ya go !" );
                //}
        }
}
