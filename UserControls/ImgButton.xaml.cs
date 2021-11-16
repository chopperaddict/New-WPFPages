#define DEBUG
using System;
using System . ComponentModel;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Input;
using System . Windows . Media;
using System . Windows . Media . Effects;

using Newtonsoft . Json . Linq;

using WPFPages . Views;

using XamlGeneratedNamespace;

namespace WPFPages . UserControls
{
        /// <summary>
        /// Interaction logic for ImgButton.xaml
        /// </summary>
        public partial class ImgButton : UserControl
        {
                public static ImgButton ThisControl;

                public ImgButton ( )
                {
                        InitializeComponent ( );
                        this . DataContext = this;
                        ThisControl = this;

                        Height = 55;
                        Width = 175;
                        this . OuterGrid . Refresh ( );
                        this . Refresh ( );
                }

                private void Imgbutton_Loaded ( object sender, RoutedEventArgs e )
                {
                     
                }
                private void ImgButton_GetHelp ( object sender, bool e )
                {
                        MessageBox . Show ( $"Sorry, Not yet implemented..." );
                }

                #region Attached properties

                #region TextColumnWidth
                public static double GetTextColumnWidth ( DependencyObject obj )
                {
                        return ( double ) obj . GetValue ( TextColumnWidthProperty );
                }

                public static void SetTextColumnWidth ( DependencyObject obj, double value )
                {
                        obj . SetValue ( TextColumnWidthProperty, value );
                }

                // Using a DependencyProperty as the backing store for ImageWidth.  This enables animation, styling, binding, etc...
                public static readonly DependencyProperty TextColumnWidthProperty =
                    DependencyProperty . RegisterAttached ( "TextColumnWidth", typeof ( double ), typeof ( ImgButton ), new PropertyMetadata ( ( double ) 0, SetTextWidth ) );

                private static void SetTextWidth ( DependencyObject d, DependencyPropertyChangedEventArgs e )
                {
                        //        Image img = d as Image;
                        //        int currentwidth = (int)img.Width;
                        //        int twidth = (int)d.GetValue(TextWidthProperty);
                        //        d. SetValue ( TextWidthProperty, currentwidth  -  (int)e . NewValue );
                        Console . WriteLine ( $"IMGBUTTON : Attached TEXTCOLUMNWIDTH property now set to {( double ) e . NewValue}" );
                }

                #endregion

                #endregion Attached properties

                #region Dependency properties

                #region BackColor

                public Brush BackColor
                {
                        get
                        {
                                Imgbutton . Refresh ( );
                                return ( Brush ) GetValue ( BackColorProperty );
                        }

                        set
                        {
                                SetValue ( BackColorProperty, value );
                        }
                }

                public static readonly DependencyProperty BackColorProperty =
                        DependencyProperty . Register ( "BackColor",
                        typeof ( Brush ),
                        typeof ( ImgButton ),
                        new PropertyMetadata ( new SolidColorBrush ( Colors . Gray ), OnBackColorChangedCallBack ) );

                private static void OnBackColorChangedCallBack ( DependencyObject d, DependencyPropertyChangedEventArgs e )
                {
                }

                #endregion BackColor

                #region BorderColor

                public Brush BorderColor
                {
                        get
                        {
                                return ( Brush ) GetValue ( BorderColorProperty );
                        }

                        set
                        {
                                SetValue ( BorderColorProperty, value );
                        }
                        //set{}
                }

                public static readonly DependencyProperty BorderColorProperty =
                        DependencyProperty . Register ( "BorderColor",
                        typeof ( Brush ),
                        typeof ( ImgButton ),
                        new PropertyMetadata ( new SolidColorBrush ( Colors . Transparent ) ) );

                #endregion BorderColor

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
                        //set { }
                }

                public static readonly DependencyProperty BorderWidthProperty =
                        DependencyProperty . Register ( "BorderWidth",
                        typeof ( double ),
                        typeof ( ImgButton ),
                        new PropertyMetadata ( ( double ) 0 ) );

                #endregion BorderWidth

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
                        //set{}
                }

                public static readonly DependencyProperty BtnTextProperty =
                        DependencyProperty . Register ( "BtnText",
                        typeof ( string ),
                        typeof ( ImgButton ),
                        new FrameworkPropertyMetadata ( "Button Text", new PropertyChangedCallback ( OnBtnTextChangedCallBack ) ) );

                private static void OnBtnTextChangedCallBack ( DependencyObject sender, DependencyPropertyChangedEventArgs e )
                {
                        Console . WriteLine ( $"IMGBUTTON : BtnText set to  [{e . NewValue}]." );
                }

                #endregion BtnText

                #region BtnTextColor
                /// </summary>
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
                        //set{}
                }

                public static readonly DependencyProperty BtnTextColorProperty =
                        DependencyProperty . Register ( "BtnTextColor",
                        typeof ( Brush ),
                        typeof ( ImgButton ),
                        new PropertyMetadata ( new SolidColorBrush ( Colors . White ) ) );

                #endregion BtnTextColor

                #region Cornerradius

                public CornerRadius Cornerradius
                {
                        get
                        {
                                return ( CornerRadius ) GetValue ( CornerradiusProperty );
                        }

                        set
                        {
                                SetValue ( CornerradiusProperty, value );
                      }
                        //set{}
                }

                public static readonly DependencyProperty CornerradiusProperty =
                        DependencyProperty . Register ( "Cornerradius",
                        typeof ( CornerRadius ),
                        typeof ( ImgButton ),
                        new PropertyMetadata ( default ), OnCornerradiusPropertyChanged );

                private static bool OnCornerradiusPropertyChanged ( object value )
                {
                        //int val = Convert . ToInt32 ( value );
                        //Console . WriteLine ( $"TextWidth received = {value}" );
                        //if ( val < 100 )
                        //{
                        //	value = 120 as object;
                        //	val = 120;
                        //}
                        //Console . WriteLine ( $"TextWidth returned = {val}" );
                        return true;
                }

                #endregion Cornerradius

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
                        //set{}
                }

                public static readonly DependencyProperty FontDecorationProperty =
                        DependencyProperty . Register ( "FontDecoration",
                        typeof ( string ),
                        typeof ( ImgButton ),
                        new FrameworkPropertyMetadata ( "Normal", new PropertyChangedCallback ( OnFontDecorationChangedCallBack ) ) );

                private static void OnFontDecorationChangedCallBack ( DependencyObject sender, DependencyPropertyChangedEventArgs e )
                {
                        //			Console . WriteLine ( $"FontDecoration Property changed to [{ value}]" );
                        //RectangleControl tc = sender as RectangleControl;
                        //FontWeight fw = ( FontWeight)e . NewValue ;
                        //tc . FontWeight = fw;
                }

                #endregion FontDecoration

                #region ImgSource
                public string ImgSource
                {
                        get
                        {
                                return ( string ) GetValue ( ImgSourceProperty );
                        }
                        set
                        {
                                SetValue ( ImgSourceProperty, value );
                        }
                 }

                public static readonly DependencyProperty ImgSourceProperty =
                        DependencyProperty . Register ( "ImgSource",
                        typeof ( string ),
                        typeof ( ImgButton ),
                        new FrameworkPropertyMetadata ( "", new PropertyChangedCallback ( OnImgSourcePropertyCallBack ) ) );

                private static void OnImgSourcePropertyCallBack ( DependencyObject sender, DependencyPropertyChangedEventArgs e )
                {
                        Console . WriteLine ( $"IMGBUTTON : ImgSource  set to  [{e . NewValue}]." );
                }

                #endregion Source

                #region ImgHeight
                public double ImgHeight
                {
                        get
                        {
                                return ( double ) GetValue ( ImgHeightProperty );
                        }

                        set
                        {
                                SetValue ( ImgHeightProperty, value );
                         }
                }

                public static readonly DependencyProperty ImgHeightProperty =
                        DependencyProperty . Register ( "ImgHeight",
                        typeof ( double ),
                        typeof ( ImgButton ),
                        new FrameworkPropertyMetadata ( ( double ) 15, new PropertyChangedCallback ( OnHeightChangedCallBack ) ) );

                private static void OnHeightChangedCallBack ( DependencyObject d, DependencyPropertyChangedEventArgs e )
                {
                         Console . WriteLine ( $"IMGBUTTON : ImageHeight set to  [{e . NewValue}]." );
                }

                #endregion ImgWidth

                #region ImageLeftOffset 
                public double ImageLeftOffset
                {
                        get
                        {
                                return ( double ) GetValue ( ImageLeftOffsetProperty );
                        }
                        set
                        {
                                SetValue ( ImageLeftOffsetProperty, value );
                        }
                }

                public static readonly DependencyProperty ImageLeftOffsetProperty =
                        DependencyProperty . Register ( "ImageLeftOffset",
                        typeof ( double ),
                        typeof ( ImgButton ),
                        new FrameworkPropertyMetadata ( Double . NaN, FrameworkPropertyMetadataOptions . None, new PropertyChangedCallback ( OnImageLeftOffsetChanged ), new CoerceValueCallback ( Coerceleft ) ) );

                private static bool isvalidOffset ( object value )
                {
                        return true;
                }

                private static object Coerceleft ( DependencyObject d, object baseValue )
                {
                        object result;
                        ThisControl ?. Refresh ( );
                        DependencyObject dep = new DependencyObject ( );
                        result = ( double ) baseValue;
                        double width = ( double ) dep . GetValue ( ImgWidthProperty );
                        if ( ( double ) result > 0 && width > 0 )
                                dep . SetValue ( ImageFullOffsetProperty, ( double ) result + width );
                        ThisControl ?. BtnImage . Refresh ( );
                        return ( object ) result;
                }

                private static void OnImageLeftOffsetChanged ( DependencyObject d, DependencyPropertyChangedEventArgs e )
                {

                }

                private static object CoerceValue ( DependencyObject d, object baseValue )
                {
                        DependencyObject dep = new DependencyObject ( );
                        object width = dep . GetValue ( ImageFullOffsetProperty );
                        return baseValue;

                }



                #endregion ImageLeftOffset

                #region ImageFullOffset 
                public double ImageFullOffset
                {
                        get
                        {
                                return ( double ) GetValue ( ImageFullOffsetProperty );
                        }
                        set
                        {
                                SetValue ( ImageFullOffsetProperty, value );
                          }
                }

                public static readonly DependencyProperty ImageFullOffsetProperty =
                        DependencyProperty . Register ( "ImageFullOffset",
                        typeof ( double ),
                        typeof ( ImgButton ),
                        new PropertyMetadata ( ( double ) 0 ), OnImageFullOffsetChanged );

                private static bool OnImageFullOffsetChanged ( object value )
                {
                        ThisControl ?. BtnImage . BringIntoView ( );
                        ThisControl ?. BtnImage . Refresh ( );
                        ThisControl ?. Refresh ( );
                        Console . WriteLine ( $"ImageFullOffset has been saved = {( double ) value}" );
                        return true;
                }

                #endregion ImageLeftOffset

                #region ImageTopOffset
                public double ImageTopOffset
                {
                        get
                        {
                                return ( double ) GetValue ( ImageTopOffsetProperty );
                        }
                        set
                        {
                                SetValue ( ImageTopOffsetProperty, value );
                        }
                }

                public static readonly DependencyProperty ImageTopOffsetProperty =
                        DependencyProperty . Register ( "ImageTopOffset",
                        typeof ( double ),
                        typeof ( ImgButton ),
                        new PropertyMetadata ( ( double ) 0 ), OnImageTopOffsetChanged );

                private static bool OnImageTopOffsetChanged ( object value )
                {
                        ThisControl ?. BtnImage . Refresh ( );
                        ThisControl?. BtnImage . BringIntoView ( );
                        ThisControl?. Refresh ( );
                        return true;
                }

                #endregion ImageTopOffset

                #region ImgWidth
                public double ImgWidth
                {
                        get
                        {
                                return ( double ) GetValue ( ImgWidthProperty );
                        }
                        set
                        {
                                SetValue ( ImgWidthProperty, value );
                         }
                }

                public static readonly DependencyProperty ImgWidthProperty =
                        DependencyProperty . Register ( "ImgWidth",
                        typeof ( double ),
                        typeof ( ImgButton ),
                        new FrameworkPropertyMetadata ( ( double ) 15, new PropertyChangedCallback ( OnWidthChangedCallBack ) ) );

                private static void OnWidthChangedCallBack ( DependencyObject d, DependencyPropertyChangedEventArgs e )
                {
                        ThisControl ?.BtnImage . Refresh ( );
                        ThisControl ?. Refresh ( );

                        Console . WriteLine ( $"IMGBUTTON : ImageWidth set to  [{e . NewValue}]." );
                }

                #endregion ImgWidth

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
                        typeof ( ImgButton ),
                        new PropertyMetadata ( ( double ) 30 ), OnShadowDirectionPropertyProperty );

                private static bool OnShadowDirectionPropertyProperty ( object value )
                {
                        ThisControl?.TextblockBorder . Refresh ( );
                        ThisControl?. Imgbutton . Refresh ( );
                        return true;
                }

                #endregion TextShadowDirection

                #region ShadowOpacity
                public double ShadowOpacity
                {
                        get
                        {
                                return ( double ) GetValue ( ShadowOpacityProperty );
                        }

                        //set { }
                        set
                        {
                                SetValue ( ShadowOpacityProperty, value );
                         }
                }

                public static readonly DependencyProperty ShadowOpacityProperty =
                        DependencyProperty . Register ( "ShadowOpacity",
                        typeof ( double ),
                        typeof ( ImgButton ),
                        new PropertyMetadata ( ( double ) 1 ), OnShadowOpacityProperty );

                private static bool OnShadowOpacityProperty ( object value )
                {
                        ThisControl?.DisplayStackpanel . Refresh ( );
                        ThisControl?. Imgbutton . Refresh ( );
                        return true;
                }

                #endregion ShadowOpacity

                #region ShadowSize
                public double ShadowSize
                {
                        get
                        {
                                return ( double ) GetValue ( ShadowSizeProperty );
                        }

                        //set { }
                        set
                        {
                                SetValue ( ShadowSizeProperty, value );
                        }
                }

                public static readonly DependencyProperty ShadowSizeProperty =
                        DependencyProperty . Register ( "ShadowSize",
                        typeof ( double ),
                        typeof ( ImgButton ),
                        new PropertyMetadata ( ( double ) 0 ), OnShadowSizeProperty );

                private static bool OnShadowSizeProperty ( object value )
                {
                        ThisControl?.TextblockBorder . Refresh ( );
                        ThisControl ?. Imgbutton . Refresh ( );
                        return true;
                }
                #endregion ShadowBlurSize

                #region ShadowBlurRadius
                public double ShadowBlurRadius
                {
                        get
                        {
                                return ( double ) GetValue ( ShadowBlurRadiusProperty );
                        }

                        //set { }
                        set
                        {
                                SetValue ( ShadowBlurRadiusProperty, value );
                         }
                }

                public static readonly DependencyProperty ShadowBlurRadiusProperty =
                        DependencyProperty . Register ( "ShadowBlurRadius",
                        typeof ( double ),
                        typeof ( ImgButton ),
                        new PropertyMetadata ( ( double ) 0 ), OnShadowBlurRadiusProperty );

                private static bool OnShadowBlurRadiusProperty ( object value )
                {
                        ThisControl?.TextblockBorder . Refresh ( );
                        ThisControl ?. Imgbutton . Refresh ( );
                        return true;
                }
                #endregion ShadowBlurRadius

                #region ShadowColor
                public Color ShadowColor
                {
                        get
                        {
                                return ( Color ) GetValue ( ShadowColorProperty );
                        }

                        set
                        {
                                SetValue ( ShadowColorProperty, value );
                          }
                        //set{}
                }

                public static readonly DependencyProperty ShadowColorProperty =
                        DependencyProperty . Register ( "ShadowColor",
                        typeof ( Color ),
                        typeof ( ImgButton ),
                        new PropertyMetadata ( Colors . Transparent ) );

                #endregion ShadowBlurColor

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
                    DependencyProperty . Register ( "SkewX", typeof ( double ), typeof ( ImgButton ), new PropertyMetadata ( 0.0 ) );

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
                    DependencyProperty . Register ( "SkewY", typeof ( double ), typeof ( ImgButton ), new PropertyMetadata ( 0.0 ) );
                #endregion

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
                        typeof ( ImgButton ),
                        new PropertyMetadata ( ( int ) 12 ), OnTextSizeChanged );

                private static bool OnTextSizeChanged ( object value )
                {
                        ThisControl?.TextBorder . Refresh ( );
                        ThisControl?.Imgbutton . Refresh ( );
                        return true;
                }

                #endregion TextSize

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
                        typeof ( ImgButton ),
                        new PropertyMetadata ( default ), OnTextWidthChanged );

                private static bool OnTextWidthChanged ( object value )
                {
                       // ThisControl.BtnTextBlock . Refresh ( );
                       Console . WriteLine ( $" *** TextWidth set to {( double ) value}" );
                        return true;
                }

                #endregion TextWidth

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
                        //set{}
                }

                public static readonly DependencyProperty TextHeightProperty =
                        DependencyProperty . Register ( "TextHeight",
                        typeof ( int ),
                        typeof ( ImgButton ),
                        new PropertyMetadata ( 40 ), OnTextHeightPropertyPropertyChanged );

                private static bool OnTextHeightPropertyPropertyChanged ( object value )
                {
                        ThisControl ?. BtnTextBlock . Refresh ( );
                        ThisControl ?. Refresh ( );
                        return true;
                }

                #endregion TextHeight

                #region TextHeightScale
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
                       }
                }

                public static readonly DependencyProperty TextHeightScaleProperty =
                        DependencyProperty . Register ( "TextHeightScale",
                        typeof ( double ),
                        typeof ( ImgButton ),
                        new PropertyMetadata ( ( double ) 1.0 ), OnTextHeightScalePropertyPropertyChanged );

                private static bool OnTextHeightScalePropertyPropertyChanged ( object value )
                {
                        ThisControl ?.BtnTextBlock . Refresh ( );
                        ThisControl ?. Refresh ( );
                        return true;
                }

                #endregion TextHeightScale

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
                                BtnTextBlock . Refresh ( );
                                ThisControl ?. Refresh ( );
                        }
                }

                public static readonly DependencyProperty TextTopOffsetProperty =
                        DependencyProperty . Register ( "TextTopOffset",
                        typeof ( int ),
                        typeof ( ImgButton ),
                        new PropertyMetadata ( 0 ), OnTextTopOffsetPropertyChanged );

                private static bool OnTextTopOffsetPropertyChanged ( object value )
                {
                        //int val = Convert . ToInt32 ( value );
                        //Console . WriteLine ( $"TextWidth received = {value}" );
                        //if ( val < 100 )
                        //{
                        //	value = 120 as object;
                        //	val = 120;
                        //}
                        //Console . WriteLine ( $"TextWidth returned = {val}" );
                        return true;
                }

                #endregion TextTopOffset

                #region TextTopPadding
                public double TextTopPadding
                {
                        get
                        {
                                return ( double ) GetValue ( TextTopPaddingProperty );
                        }

                        //set { }
                        set
                        {
                                SetValue ( TextTopPaddingProperty, value );
                                BtnTextBlock . Refresh ( );
                                ThisControl ?. Refresh ( );
                        }
                }

                public static readonly DependencyProperty TextTopPaddingProperty =
                        DependencyProperty . Register ( "TextTopPadding",
                        typeof ( double ),
                        typeof ( ImgButton ),
                        new PropertyMetadata ( ( double ) -8 ), OnTextTopPaddingChanged );

                private static bool OnTextTopPaddingChanged ( object value )
                {
                        return true;
                }

                #endregion TextTopPadding

                #region TextWidthPadding
                public double TextWidthPadding
                {
                        get
                        {
                                return ( double ) GetValue ( TextWidthPaddingProperty );
                        }

                        //set { }
                        set
                        {
                                SetValue ( TextWidthPaddingProperty, value );
                                BtnTextBlock . Refresh ( );
                                ThisControl ?. Refresh ( );
                        }
                }

                public static readonly DependencyProperty TextWidthPaddingProperty =
                        DependencyProperty . Register ( "TextWidthPadding",
                        typeof ( double ),
                        typeof ( ImgButton ),
                        new PropertyMetadata ( ( double ) 0 ), OnTextWidthPaddingPropertyChanged );

                private static bool OnTextWidthPaddingPropertyChanged ( object value )
                {
                        return true;
                }

                #endregion TextWidthPadding

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
                                BtnTextBlock . Refresh ( );
                                ThisControl ?. Refresh ( );
                        }
                }

                public static readonly DependencyProperty TextWidthScaleProperty =
                        DependencyProperty . Register ( "TextWidthScale",
                        typeof ( double ),
                        typeof ( ImgButton ),
                        new PropertyMetadata ( ( double ) 1 ), OnTextWidthScalePropertyChanged );

                private static bool OnTextWidthScalePropertyChanged ( object value )
                {
                        Console . WriteLine ( $"IMGBUTTON : TextWidthScaleProperty   = {value}" );
                        return true;
                }

                #endregion TextWidthScale

                #region Url
                public string Url
                {
                        get
                        {
                                return ( string ) GetValue ( UrlProperty );
                        }

                        set
                        {
                                SetValue ( UrlProperty, value );
                                BtnTextBlock . Refresh ( );
                                ThisControl ?. Refresh ( );
                        }
                        //set{}
                }

                public static readonly DependencyProperty UrlProperty =
                        DependencyProperty . Register ( "Url",
                        typeof ( string ),
                        typeof ( ImgButton ),
                        new FrameworkPropertyMetadata ( "", new PropertyChangedCallback ( OnUrlChangedCallBack ) ) );

                private static void OnUrlChangedCallBack ( DependencyObject sender, DependencyPropertyChangedEventArgs e )
                {
                        Console . WriteLine ( $"IMGBUTTON : Url DP changed - Image set to [{ e . NewValue}]" );
                }

                #endregion Url

                #region TextWrap
                public TextWrapping TextWrap
                {
                        get
                        {
                                return ( TextWrapping ) GetValue ( TextWrapProperty );
                        }

                        set
                        {
                                SetValue ( TextWrapProperty, value );
                                BtnTextBlock . Refresh ( );
                                ThisControl ?. Refresh ( );
                        }
                }

                public static readonly DependencyProperty TextWrapProperty =
                        DependencyProperty . Register ( "TextWrap",
                        typeof ( TextWrapping ),
                        typeof ( ImgButton ),
                        new FrameworkPropertyMetadata ( TextWrapping . Wrap, new PropertyChangedCallback ( OnTextWrapPropertyChangedCallBack ) ) );

                private static void OnTextWrapPropertyChangedCallBack ( DependencyObject d, DependencyPropertyChangedEventArgs e )
                {
                        Console . WriteLine ( $"IMGBUTTON : OnTextWrap set to  [{e . NewValue}]." );
                }

                #endregion TextWrap

                #region TextShadowOpacity
                public double TextShadowOpacity
                {
                        get
                        {
                                return ( double ) GetValue ( TextShadowOpacitProperty );
                        }

                        //set { }
                        set
                        {
                                SetValue ( TextShadowOpacitProperty, value );
                                BtnTextBlock . Refresh ( );
                                ThisControl ?. Refresh ( );
                        }
                }

                public static readonly DependencyProperty TextShadowOpacitProperty =
                        DependencyProperty . Register ( "TextShadowOpacity",
                        typeof ( double ),
                        typeof ( ImgButton ),
                        new PropertyMetadata ( ( double ) 1.0 ), OnTextShadowOpacityProperty );

                private static bool OnTextShadowOpacityProperty ( object value )
                {
                        Console . WriteLine ( $"IMGBUTTON : TextShadowOpacity set to  [{value}]." );
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

                        //set { }
                        set
                        {
                                SetValue ( TextShadowSizeProperty, value );
                                BtnTextBlock . Refresh ( );
                                Imgbutton . Refresh ( );
                        }
                }

                public static readonly DependencyProperty TextShadowSizeProperty =
                        DependencyProperty . Register ( "TextShadowSize",
                        typeof ( double ),
                        typeof ( ImgButton ),
                        new PropertyMetadata ( ( double ) 1 ), OnTextShadowSizePropertyProperty );

                private static bool OnTextShadowSizePropertyProperty ( object value )
                {
                        Console . WriteLine ( $"IMGBUTTON : TextShadowSize set to  [{value}]." );
                        return true;
                }

                #endregion TextShadowSize

                #region TextShadowRadius
                public double TextShadowRadius
                {
                        get
                        {
                                return ( double ) GetValue ( TextShadowRadiusProperty );
                        }

                        //set { }
                        set
                        {
                                SetValue ( TextShadowRadiusProperty, value );
                                BtnTextBlock . Refresh ( );
                                Imgbutton . Refresh ( );
                        }
                }

                public static readonly DependencyProperty TextShadowRadiusProperty =
                        DependencyProperty . Register ( "TextShadowRadius",
                        typeof ( double ),
                        typeof ( ImgButton ),
                        new PropertyMetadata ( ( double ) 2 ), OnTextShadowRadiusProperty );

                private static bool OnTextShadowRadiusProperty ( object value )
                {
                        Console . WriteLine ( $"IMGBUTTON : TextShadowRadiusProperty = {value}" );
                        return true;
                }

                #endregion TextShadowRadius

                #region TextShadowDirection
                public double TextShadowDirection
                {
                        get
                        {
                                return ( double ) GetValue ( TextShadowDirectionProperty );
                        }

                        //set { }
                        set
                        {
                                SetValue ( TextShadowDirectionProperty, value );
                                BtnTextBlock . Refresh ( );
                                Imgbutton . Refresh ( );
                        }
                }

                public static readonly DependencyProperty TextShadowDirectionProperty =
                        DependencyProperty . Register ( "TextShadowDirection",
                        typeof ( double ),
                        typeof ( ImgButton ),
                        new PropertyMetadata ( ( double ) 30 ), OnTextShadowDirectionPropertyProperty );

                private static bool OnTextShadowDirectionPropertyProperty ( object value )
                {
                        Console . WriteLine ( $"IMGBUTTON :TextShadowDirectionProperty = {value}" );
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
                                BtnTextBlock . Refresh ( );
                                Imgbutton . Refresh ( );
                        }
                        //set{}
                }

                public static readonly DependencyProperty TextShadowColorProperty =
                        DependencyProperty . Register ( "TextShadowColor",
                        typeof ( Color ),
                        typeof ( ImgButton ),
                        new PropertyMetadata ( Colors . Red ) );

                #endregion TextShadowColor

                #endregion Dependency proiperties

                private void RectBtn_MouseEnter ( object sender, MouseEventArgs e )
                {
                        // Move button & Text Right and Down
                        TranslateTransform translateTransform = new TranslateTransform ( );
                        translateTransform . X += 2;
                        translateTransform . Y -= 1;
                        this . OuterGrid . RenderTransform = translateTransform;

                        // Move Shadow to match above
                        DropShadowEffect dropShadowEffect = new DropShadowEffect ( );
                        dropShadowEffect . BlurRadius = ShadowBlurRadius;
                        dropShadowEffect . Color = TextShadowColor;
                        dropShadowEffect . Direction = 235;
                        dropShadowEffect . Opacity = ShadowOpacity;
                        dropShadowEffect . ShadowDepth = ShadowSize;

                        this . OuterGrid . Effect = dropShadowEffect;
                        this . OuterGrid . Refresh ( );
                        return;
                }

                private void RectBtn_MouseLeave ( object sender, MouseEventArgs e )
                {
                        TranslateTransform translateTransform = new TranslateTransform ( );
                        translateTransform . X -= 2;
                        translateTransform . Y += 1;
                        this . OuterGrid . RenderTransform = translateTransform;

                        DropShadowEffect dropShadowEffect = new DropShadowEffect ( );
                        dropShadowEffect . BlurRadius = ShadowBlurRadius;
                        dropShadowEffect . Color = TextShadowColor;
                        dropShadowEffect . Direction = 49;
                        dropShadowEffect . Opacity = ShadowOpacity;
                        dropShadowEffect . ShadowDepth = ShadowSize;
                        this . OuterGrid . Effect = dropShadowEffect;
                        this . OuterGrid . Refresh ( );
                        return;
                }

                /// <summary>
                /// How to pass a click event thru to end user of this control
                /// </summary>
                public event RoutedEventHandler Click;

                private void OnClick ( object sender, MouseButtonEventArgs e )
                {
                        if ( this . Click != null )
                        {
                                this . Click ( this, e );
                        }
                }

                private void RectBtn_Loaded ( object sender, RoutedEventArgs e )
                {
                        //Forece outer shadow to be displayed
                        RectBtn_MouseEnter ( null, null );
                        RectBtn_MouseLeave ( null, null );
                        this . Refresh ( );
                }

                private void BtnTextBlock_Loaded ( object sender, RoutedEventArgs e )
                {
                        double width = BtnTextBlock . Width;
                        double textwidthpadding = ( double ) ThisControl ?. GetValue ( TextWidthPaddingProperty );
                        if ( textwidthpadding != 0 )
                        {
                                ThisControl ?. SetValue ( TextWidthProperty, width - textwidthpadding );
                                Console . WriteLine ( $"Text loaded - TextWidthProperty = {width}, padding={textwidthpadding}, TextWidthProperty set to ={ThisControl ?. GetValue ( TextWidthProperty )}\n" );
                        }
                        else
                        {
                                ThisControl ?. SetValue ( TextWidthProperty, width );
                                Console . WriteLine ( $"Text loaded - TextWidthProperty = {width}, padding={textwidthpadding}, TextWidthProperty set to ={ThisControl ?. GetValue ( TextWidthProperty )}\n" );
                        }
                }

                private void BtnImage_Loaded ( object sender, RoutedEventArgs e )
                {
                 }

                private void OuterGrid_Loaded ( object sender, RoutedEventArgs e )
                {
                        if ( DesignerProperties . GetIsInDesignMode ( this ) == false)
                        {
                                if ( ImgBorder . Visibility == Visibility . Hidden )
                                {
                                        ImgBorder . Visibility = Visibility . Visible;
                                        BtnImage . Visibility = Visibility . Visible;
                                }
                                else
                                {
                                        BtnImage . Visibility = Visibility . Visible;
                                        ImgBorder . BorderBrush = Brushes . Transparent;
                                }
                        }
                }

                private void ImgButton_PreviewKeyDown ( object sender, KeyEventArgs e )
                {
                        if ( e . Key == Key . F1 )
                                MessageBox . Show ( $"Sorry, Not yet implemented..." );
                }

                private void OuterGrid_Unloaded ( object sender, RoutedEventArgs e )
                {
                }

                private void BtnImage_Initialized ( object sender, EventArgs e )
                {
                        int i = 0;
                        //BtnImage . Refresh ( );
                }

  
        }
}
