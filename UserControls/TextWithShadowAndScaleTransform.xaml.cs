using System;
using System . Collections . Generic;
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
	/// Interaction logic for TextWithShadowAndScaleTransform.xaml
	/// </summary>
	public partial class TextWithShadowAndScaleTransform : UserControl
	{
		public TextWithShadowAndScaleTransform ( )
		{
//			this . DataContext = this;
			InitializeComponent ( );
			this . DataContext = this;
		}

		#region Dependency Properties

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
				tb. Refresh ( );
			}
		}
		public static readonly DependencyProperty ShowBorderProperty =
			DependencyProperty . Register ( "ShowBorder",
			typeof ( Visibility ),
			typeof ( TextWithShadowAndScaleTransform ),
			new PropertyMetadata ( Visibility . Visible ) );
		#endregion
		
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
				tb. Refresh ( );
			}
		}
		public static readonly DependencyProperty TextProperty =
			DependencyProperty . Register ( "Text",
			typeof ( string),
			typeof ( TextWithShadowAndScaleTransform ),
			new PropertyMetadata ( "") );
		#endregion

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
				ShadowTextBlockGrid . Refresh ( );
			}
		}
		public static readonly DependencyProperty TextShadowDirectionProperty =
			DependencyProperty . Register ( "TextShadowDirection",
			typeof ( double ),
			typeof ( TextWithShadowAndScaleTransform ),
			new PropertyMetadata ( ( double ) 330 ), OnTextShadowDirectionPropertyProperty );

		private static bool OnTextShadowDirectionPropertyProperty ( object value )
		{
			return true;
		}
		#endregion

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
				ShadowTextBlockGrid . Refresh ( );
			}
			//set{}
		}
		public static readonly DependencyProperty TextShadowColorProperty =
			DependencyProperty . Register ( "TextShadowColor",
			typeof ( Color ),
			typeof ( TextWithShadowAndScaleTransform ),
			new PropertyMetadata ( Colors . DarkGray ) );

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
				ShadowTextBlockGrid . Refresh ( );
			}

		}
		public static readonly DependencyProperty TextShadowOpacitProperty =
			DependencyProperty . Register ( "TextShadowOpacity",
			typeof ( double ),
			typeof ( TextWithShadowAndScaleTransform ),
			new PropertyMetadata ( ( double ) 0.5 ), OnTextShadowOpacityProperty );

		private static bool OnTextShadowOpacityProperty ( object value )
		{
			return true;
		}
		#endregion

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
				ShadowTextBlockGrid . Refresh ( );
			}
		}
		public static readonly DependencyProperty TextShadowRadiusProperty =
			DependencyProperty . Register ( "TextShadowRadius",
			typeof ( double ),
			typeof ( TextWithShadowAndScaleTransform),
			new PropertyMetadata ( ( double ) 1 ), OnTextShadowRadiusProperty );

		private static bool OnTextShadowRadiusProperty ( object value )
		{
			return true;
		}
		#endregion

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
				ShadowTextBlockGrid . Refresh ( );
			}
		}
		public static readonly DependencyProperty TextShadowSizeProperty =
			DependencyProperty . Register ( "TextShadowSize",
			typeof ( double ),
			typeof ( TextWithShadowAndScaleTransform ),
			new PropertyMetadata ( ( double ) 2 ), OnTextShadowSizePropertyProperty );

		private static bool OnTextShadowSizePropertyProperty ( object value )
		{
			//			Console . WriteLine ( $"ShadowBlurSizeProperty = {value}" );

			return true;
		}
		#endregion

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
			//set { }
			set
			{
				SetValue ( TextWidthScaleProperty, value );
				ShadowTextBlockGrid . Refresh ( );
			}

		}
		public static readonly DependencyProperty TextWidthScaleProperty =
			DependencyProperty . Register ( "TextWidthScale",
			typeof ( double ),
			typeof ( TextWithShadowAndScaleTransform ),
			new PropertyMetadata ( ( double ) 1 ), OnTextWidthScalePropertyChanged );

		private static bool OnTextWidthScalePropertyChanged ( object value )
		{
			//			Console . WriteLine ( $"TextWidthScaleProperty   = {value}" );
			return true;
		}
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
				if ( value < 50 )
					value = 120;
				SetValue ( TextHeightProperty, value );
				tb. Refresh ( );
			}
		}
		public static readonly DependencyProperty TextHeightProperty =
			DependencyProperty . Register ( "TextHeight",
			typeof ( int ),
			typeof ( TextWithShadowAndScaleTransform ),
			new PropertyMetadata ( 35 ), OnTextHeightPropertyPropertyChanged );

		private static bool OnTextHeightPropertyPropertyChanged ( object value )
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
		#endregion

		#region TextSize
		/// <summary>
		/// Size of the button text
		/// </summary>
		public int TextSize
		{
			get
			{
				return ( int ) GetValue ( TextSizeProperty );
			}
			set
			{
				SetValue ( TextSizeProperty, value );
				tb. Refresh ( );
			}
		}
		public static readonly DependencyProperty TextSizeProperty =
			DependencyProperty . Register ( "TextSize",
			typeof ( int ),
			typeof ( TextWithShadowAndScaleTransform),
			new PropertyMetadata ( 18 ), OnTextSizeChanged );

		private static bool OnTextSizeChanged ( object value )
		{
			return true;
		}
		#endregion

		#region CornerRadius
		public CornerRadius CornerRadius
		{
			get
			{
				return ( CornerRadius ) GetValue ( CornerRadiusProperty );
			}
			set
			{
				SetValue ( CornerRadiusProperty, value );
				ShadowTextBlockGrid . Refresh ( );
			}
		}
		public static readonly DependencyProperty CornerRadiusProperty =
			DependencyProperty . Register ( "CornerRadius",
			typeof ( CornerRadius),
			typeof ( TextWithShadowAndScaleTransform ),
			new PropertyMetadata ( new CornerRadius(5)) , OnCornerRadiusChanged );

		private static bool OnCornerRadiusChanged ( object value )
		{
			return true;
		}
		#endregion

		

		#endregion Dependency Properties
	}
}
