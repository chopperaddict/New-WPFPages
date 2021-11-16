using System;
using System . Diagnostics;
using System . Globalization;
using System . Windows;
using System . Windows . Data;
using System . Windows . Media;

namespace WPFPages . Converts
{
	public class Int2BrushConverter : IValueConverter
	{
		public object Convert ( object value, Type targetType, object parameter, CultureInfo culture )
		{
			if ( value == DependencyProperty . UnsetValue )
				return DependencyProperty . UnsetValue;
			// Assumes receiving an int value and Returns a Brush
			// Used by AcType to color each datagrid row to match the AcType of the account
			Brush br = null;
			string s = parameter . ToString ( );
			try
			{
				int val = System . Convert . ToInt32 ( s );
				br = Utils.GetBrushFromInt ( val );
			}
			catch ( Exception ex ) { Debug . WriteLine ( $"{ex . Message}, {ex . Data}" ); }

			return br;
		}
		public object ConvertBack ( object value, Type targetType, object parameter, CultureInfo culture )
		{
			return null as object;
		}
	}

}