using System;
using System .Collections .Generic;
using System .Globalization;
using System .Linq;
using System .Text;
using System .Threading .Tasks;
using System .Windows .Data;

namespace WPFPages .Converts
{
	public partial class ToggleBoolConverter : IValueConverter
	{
		public object Convert ( object value , Type targetType , object parameter , CultureInfo culture )
		{
			if ( ( string ) value == "Hidden" )
				value = "Visible";
			else
				value = "Hidden";
			return value;
			//return System.Convert.ToInt32(value);
		}



		public object ConvertBack ( object value , Type targetType , object parameter , CultureInfo culture )
		{

			return ( string ) value;
		}
	}
}
