using System;
using System . Windows . Media;
using System . Windows;


namespace WPFPages . Views
{
	public class DataGridColorCtrlAp : DependencyObject
	{
		#region Attached Properties

		//#region AlternatingRowBackground
		//public static Brush GetAlternatingRowBackground ( DependencyObject obj )
		//{
		//	return ( Brush ) obj . GetValue ( AlternatingRowBackgroundProperty );
		//}
		//public static void SetAlternatingRowBackground ( DependencyObject obj , Brush value )
		//{
		//	obj . SetValue ( AlternatingRowBackgroundProperty , value );
		//}
		//// Using a DependencyProperty as the backing store for AlternatingRowBackground.  This enables animation, styling, binding, etc...
		//public static readonly DependencyProperty AlternatingRowBackgroundProperty =
		//	DependencyProperty.RegisterAttached("AlternatingRowBackground", typeof(Brush), typeof(DataGridColorCtrlAp), new PropertyMetadata(Brushes.Transparent));
		//#endregion AlternatingRowBackground

		#region Background
		public static readonly DependencyProperty BackgroundProperty
		= DependencyProperty . RegisterAttached ("Background",typeof ( Brush ),typeof ( DataGridColorCtrlAp ),new PropertyMetadata ( Brushes . Aquamarine ), OnBackgroundChanged );
		public static Brush GetBackground ( DependencyObject d )
		{
			return ( Brush ) d . GetValue ( BackgroundProperty );
		}
		public static void SetBackground ( DependencyObject d , Brush value )
		{
			Console . WriteLine ( $"AP : setting Background to {value}" );
			d . SetValue ( BackgroundProperty , value );
		}
		private static bool OnBackgroundChanged ( object value )
		{
			Console . WriteLine ( $"AP : OnBackgrounhanged = {value}" );
			return true;
		}
		#endregion BackgroundColor

		#region BackgroundMouseover
		public static readonly DependencyProperty BackgroundMouseoverProperty
		= DependencyProperty . RegisterAttached (
		"BackgroundMouseover",
		typeof ( Brush ),
		typeof ( DataGridColorCtrlAp ),
		new PropertyMetadata ( Brushes . LightBlue), OnBackgroundMouseoverChanged );
		public static Brush GetBackgroundMouseover ( DependencyObject d )
		{
			return ( Brush ) d . GetValue ( BackgroundMouseoverProperty );
		}
		public static void SetBackgroundMouseover ( DependencyObject d , Brush value )
		{
			Console . WriteLine ( $"AP : setting Background1 to {value}" );
			d . SetValue ( BackgroundMouseoverProperty , value );
		}
		private static bool OnBackgroundMouseoverChanged ( object value )
		{
			Console . WriteLine ( $"AP : OnBackgroundMouseoverchanged = {value}" );
			return true;
		}
		#endregion BackgroundMouseover									  

		#region BackgroundSelected
		public static readonly DependencyProperty BackgroundSelectedProperty
		= DependencyProperty . RegisterAttached (
		"BackgroundSelected",
		typeof ( Brush ),
		typeof ( DataGridColorCtrlAp ),
		new PropertyMetadata ( Brushes . Blue ), OnBackgroundSelectedChanged );
		public static Brush GetBackground1 ( DependencyObject d )
		{
			return ( Brush ) d . GetValue ( BackgroundSelectedProperty );
		}
		public static void SetBackgroundSelected ( DependencyObject d , Brush value )
		{
			Console . WriteLine ( $"AP : setting BackgroundSelected to {value}" );
			d . SetValue ( BackgroundSelectedProperty , value );
		}
		private static bool OnBackgroundSelectedChanged ( object value )
		{
			Console . WriteLine ( $"AP : OnBackgroundSelectedchanged = {value}" );
			return true;
		}
		#endregion BackgroundSelected

		#region BackgroundMouseoverSelected
		public static readonly DependencyProperty BackgroundMouseoverSelectedProperty
		= DependencyProperty . RegisterAttached (
		"BackgroundMouseoverSelected",
		typeof ( Brush ),
		typeof ( DataGridColorCtrlAp ),
		new PropertyMetadata ( Brushes . Blue ) );
		public static Brush GetBackgroundMouseoverSelected ( DependencyObject d )
		{
			return ( Brush ) d . GetValue ( BackgroundMouseoverSelectedProperty );
		}
		public static void SetBackgroundMouseoverSelected ( DependencyObject d , Brush value )
		{
			Console . WriteLine ( $"AP : setting Background1 to {value}" );
			d . SetValue ( BackgroundMouseoverSelectedProperty , value );
		}
			#endregion BackgroundMouseoverSelected


		#region BorderBrush
		public static Brush GetBorderBrush ( DependencyObject obj )
		{
			return ( Brush ) obj . GetValue ( BorderBrushProperty );
		}

		public static void SetBorderBrush ( DependencyObject obj , Brush value )
		{
			obj . SetValue ( BorderBrushProperty , value );
		}

		// Using a DependencyProperty as the backing store for BorderBrush1.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty BorderBrushProperty =
			DependencyProperty . RegisterAttached ( "BorderBrush", typeof ( Brush ), typeof ( DataGridColorCtrlAp ), new PropertyMetadata ( Brushes . Transparent ) );
		#endregion BorderBrush1

		#region BorderThickness
		public static Thickness GetBorderThickness ( DependencyObject obj )
		{
			return ( Thickness ) obj . GetValue ( BorderThicknessProperty );
		}

		public static void SetBorderThickness1 ( DependencyObject obj , Thickness value )
		{
			obj . SetValue ( BorderThicknessProperty , value );
		}

		// Using a DependencyProperty as the backing store for BorderThickness1.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty BorderThicknessProperty =
			DependencyProperty . RegisterAttached ( "BorderThickness", typeof ( Thickness ), typeof ( DataGridColorCtrlAp ), new PropertyMetadata ( default ) );
		#endregion BorderThickness1

		#region FontSize 
		public static double GetFontSize ( DependencyObject obj )
		{
			return ( double ) obj . GetValue ( FontSizeProperty );
		}

		public static void SetFontSize ( DependencyObject obj , double value )
		{
			obj . SetValue ( FontSizeProperty , value );
		}

		// Using a DependencyProperty as the backing store for FontSize1.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty FontSizeProperty =
			  DependencyProperty . RegisterAttached ( "FontSize", typeof ( double ), typeof ( DataGridColorCtrlAp ), new PropertyMetadata ( ( double ) 14) );
		#endregion FontSize1 

		#region FontWeight 
		public static FontWeight GetFontWeight ( DependencyObject obj )
		{
			return ( FontWeight ) obj . GetValue ( FontWeightProperty );
		}

		public static void SetFontWeight ( DependencyObject obj , FontWeight value )
		{
			obj . SetValue ( FontWeightProperty , value );
		}

		// Using a DependencyProperty as the backing store for FontWeight1.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty FontWeightProperty =
			  DependencyProperty . RegisterAttached ( "FontWeight", typeof ( FontWeight ), typeof ( DataGridColorCtrlAp ), new PropertyMetadata ( default ) );
		#endregion FontWeight 

		#region FontWeightSelected
		public static FontWeight GetFontWeightSelected ( DependencyObject obj )
		{
			return ( FontWeight ) obj . GetValue ( FontWeightSelectedProperty );
		}

		public static void SetFontWeightSelected ( DependencyObject obj , FontWeight value )
		{
			obj . SetValue ( FontWeightSelectedProperty , value );
		}

		// Using a DependencyProperty as the backing store for FontWeight1.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty FontWeightSelectedProperty =
			  DependencyProperty . RegisterAttached ( "FontWeightSelected", typeof ( FontWeight ), typeof ( DataGridColorCtrlAp ), new PropertyMetadata ((FontWeight)default ) );
		#endregion FontWeight1 

		
		#region Foreground
		public static readonly DependencyProperty ForegroundProperty= DependencyProperty . RegisterAttached ("Foreground",typeof ( Brush ),typeof ( DataGridColorCtrlAp ),
		new PropertyMetadata ( Brushes . Black ) );
		public static Brush GetForeground ( DependencyObject d )
		{
			return ( Brush ) d . GetValue ( ForegroundProperty );
		}
		public static void SetForeground ( DependencyObject d , Brush value )
		{
			d . SetValue ( ForegroundProperty , value );
		}
		#endregion Foreground

		#region ForegroundMouseover
		public static Brush GetForegroundMouseover	( DependencyObject obj )
		{
			return ( Brush ) obj . GetValue ( ForegroundMouseoverProperty );
		}

		public static void SetForegroundMouseover ( DependencyObject obj , Brush value )
		{
			obj . SetValue ( ForegroundMouseoverProperty , value );
		}

		// Using a DependencyProperty as the backing store for ForegroundMouseover.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ForegroundMouseoverProperty =
			DependencyProperty.RegisterAttached("ForegroundMouseover", typeof(Brush), typeof(DataGridColorCtrlAp), new PropertyMetadata((Brush)Brushes.Black));
		#endregion ForegroundMouseover

		#region ForegroundSelected
		public static Brush GetForegroundSelected ( DependencyObject obj )			
		{
			return ( Brush ) obj . GetValue ( ForegroundSelectedProperty );
		}

		public static void SetForegroundSelected ( DependencyObject obj , Brush value )
		{
			obj . SetValue ( ForegroundSelectedProperty , value );
		}

		// Using a DependencyProperty as the backing store for SForegroundSelected.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ForegroundSelectedProperty =
			DependencyProperty.RegisterAttached("ForegroundSelected", typeof(Brush), typeof(DataGridColorCtrlAp), new PropertyMetadata(Brushes.White));
		#endregion ForegroundSelected

		#region ForegroundMouseoverSelected
		public static Brush GetForegroundMouseoverSelected	( DependencyObject obj )
		{
			return ( Brush ) obj . GetValue ( ForegroundMouseoverSelectedProperty );
		}

		public static void SetForegroundMouseoverSelected ( DependencyObject obj , Brush value )
		{
			obj . SetValue ( ForegroundMouseoverSelectedProperty , value );
		}
		public static readonly DependencyProperty ForegroundMouseoverSelectedProperty =
			DependencyProperty.RegisterAttached("ForegroundMouseoverSelected", typeof(Brush), typeof(DataGridColorCtrlAp), new PropertyMetadata(Brushes.White));
		#endregion ForegroundMouseoverSelected



		#region ItemHeight
		public static readonly DependencyProperty ItemHeightProperty
			= DependencyProperty . RegisterAttached (
			"ItemHeight",
			typeof ( double ),
			typeof ( DataGridColorCtrlAp ),
			new PropertyMetadata ( ( double ) 20 ), OnItemheightChanged );

		public static double GetItemHeight ( DependencyObject d )
		{
			return ( double ) d . GetValue ( ItemHeightProperty );
		}
		public static void SetItemHeight ( DependencyObject d , double value )
		{
			d . SetValue ( ItemHeightProperty , value );
		}
		private static bool OnItemheightChanged ( object value )
		{
			Console . WriteLine ( $"AP : ONItemHeightchanged = {value}" );

			return true;
		}
		#endregion ItemHeight				
	
		#region dumyAPstring
		public static readonly DependencyProperty dummyAPstringProperty =
			  DependencyProperty . RegisterAttached ( "dummyAPstring",
				    typeof ( string ), typeof ( DataGridColorCtrlAp ),
				    new PropertyMetadata ( ( string ) "DummyAPstring from AP ! " ), OnstringSet );
		public static string GetdummyAPstring ( DependencyObject obj )
		{
			return ( string ) obj . GetValue ( dummyAPstringProperty );
		}
		public static void SetdummyAPstring ( DependencyObject obj , string value )
		{
			obj . SetValue ( dummyAPstringProperty , value );
		}
		private static bool OnstringSet ( object value )
		{
			Console . WriteLine ( $"AP.dummyAPstring set to : {value}" );
			return true;
		}
		#endregion dumyAPstring

		#region test2 
		public static readonly DependencyProperty test2Property =
			  DependencyProperty . RegisterAttached ( "test2",
				typeof ( string ), typeof ( DataGridColorCtrlAp ), new PropertyMetadata ( ( string ) "99" ) );
		public static string Gettest2 ( DependencyObject obj )
		{
			return ( string ) obj . GetValue ( test2Property );
		}
		public static void Settest2 ( DependencyObject obj , string value )
		{
			obj . SetValue ( test2Property , value );
		}
		#endregion test2 

		#region test 
		public static readonly DependencyProperty testProperty =
			  DependencyProperty . RegisterAttached ( "test",
				typeof ( int ), typeof ( DataGridColorCtrlAp ), new PropertyMetadata ( 32767 ), Ontestchanged );

		public static int Gettest ( DependencyObject obj )
		{
			return ( int ) obj . GetValue ( testProperty );
		}
		public static void Settest ( DependencyObject obj , int value )
		{
			obj . SetValue ( testProperty , value );
		}


		private static bool Ontestchanged ( object value )
		{
			Console . WriteLine ( $"AP : test value changed to {value}" );
			return true;
		}
		#endregion test 

		#region dblvalue
		public static double Getdblvalue ( DependencyObject obj )
		{
			return ( double ) obj . GetValue ( dblvalueProperty );
		}

		public static void Setdblvalue ( DependencyObject obj , double value )
		{
			obj . SetValue ( dblvalueProperty , value );
		}

		// Using a DependencyProperty as the backing store for dblvalue.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty dblvalueProperty =
			  DependencyProperty . RegisterAttached ( "dblvalue", typeof ( double ), typeof ( DataGridColorCtrlAp ), new PropertyMetadata ( ( double ) 23.864 ), Ondblvaluechanged );

		private static bool Ondblvaluechanged ( object value )
		{
			Console . WriteLine ( $"dblvaluechanged = {value}" );
			return true;
		}
		#endregion dblvalue


		#endregion Attached Properties
	}
}
