using System;
using System .Globalization;
using System .Windows .Data;

namespace WPFPages .Converts
{
	public partial class ToggleVisibilityConverter : IValueConverter
	{
		public object Convert ( object value, Type targetType, object parameter, CultureInfo culture )
		{
			if ( ( bool ) value == true )
				value = "Visible";
			else
				value = "Hidden";
			return value; 
		}
		public object ConvertBack ( object value, Type targetType, object parameter, CultureInfo culture )
		{
			if ( ( string ) value == "Hidden" )
				value = false;
			else
				value = true;
			return value;

		}
	}
}
